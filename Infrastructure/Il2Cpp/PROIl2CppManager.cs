using System.Diagnostics;
using System.Drawing;
using Domain;
using Infrastructure.Il2Cpp.Core;

namespace Infrastructure.Il2Cpp;

/// <summary>
/// IL2CPP-based game state reader for PRO.
/// </summary>
/// <remarks>
/// <para>
/// <b>Architecture overview</b><br/>
/// <list type="number">
///   <item>
///     <see cref="LoadGameAsync"/> locates the PROClient process and calls
///     <see cref="FridaChannel.ConnectAsync"/>, which attaches Frida and loads
///     the in-process JS agent.  The agent discovers all class and field offsets
///     <em>by name</em> from the IL2CPP reflection API, then signals readiness.
///   </item>
///   <item>
///     All subsequent reads (<see cref="ReadAllAsync"/>, individual properties)
///     send a <c>{cmd:"read"}</c> request to the resident agent, which reads
///     memory in-process and returns values over Frida's message channel.
///     Frida stays attached for the lifetime of the game session — this avoids
///     the cold-start issues of the one-shot approach where memory may not yet
///     be initialized.
///   </item>
/// </list>
/// </para>
///
/// <para>
/// <b>Error handling</b><br/>
/// Every read is retried up to <see cref="MaxRetries"/> times with a short delay.
/// If the agent reports that a pointer in the chain is null (game still loading),
/// the retry loop handles it transparently.  On permanent failure,
/// <see cref="Il2CppAccessException"/> is thrown and <see cref="IsGameOpened"/>
/// becomes <see langword="false"/>; call <see cref="LoadGameAsync"/> again to
/// re-attach.
/// </para>
///
/// <para>
/// <b>API surface</b><br/>
/// This class intentionally mirrors <c>PROMemoryManager</c> so existing call
/// sites need minimal changes.
/// </para>
///
/// See <c>Infrastructure/Il2Cpp/UPDATE.md</c> for the post-patch update procedure.
/// </remarks>
public sealed class PROIl2CppManager : IAsyncDisposable
{
    // ── Configuration ─────────────────────────────────────────────────────────

    /// <summary>Maximum number of read retries before throwing.</summary>
    private const int MaxRetries = 3;

    /// <summary>Delay between retries (matches the original PROMemoryManager's 10 ms).</summary>
    private static readonly TimeSpan RetryDelay = TimeSpan.FromMilliseconds(10);

    /// <summary>PROClient process name (without extension).</summary>
    private const string ProcessName = "PROClient";

    // ── State ─────────────────────────────────────────────────────────────────

    private FridaChannel? _channel;    private Process?      _process;    private readonly SemaphoreSlim _initLock = new(1, 1);

    // ── Public API — lifecycle ────────────────────────────────────────────────

    /// <summary>
    /// <see langword="true"/> when attached to a running game process.
    /// </summary>
    public bool IsGameOpened => _channel?.IsConnected == true;

    /// <summary>
    /// The PROClient <see cref="System.Diagnostics.Process"/> object, or
    /// <see langword="null"/> if <see cref="LoadGameAsync"/> has not succeeded.
    /// Used by call sites that need to send key presses via Win32.
    /// </summary>
    public Process? Process => _process;

    /// <summary>
    /// Synchronous wrapper around <see cref="LoadGameAsync"/> for call sites
    /// (e.g. WinForms event handlers) that cannot use <see langword="await"/>.
    /// </summary>
    /// <returns>
    ///   A tuple of success flag and an optional error message.
    /// </returns>
    public (bool success, string? error) LoadGame()
        => Task.Run(() => LoadGameAsync()).GetAwaiter().GetResult();

    /// <summary>
    /// Finds the PROClient process and attaches the Frida agent.
    /// The agent stays resident until <see cref="DisposeAsync"/> is called.
    /// </summary>
    /// <returns>
    ///   A tuple: <see langword="true"/> and <see langword="null"/> on success;
    ///   <see langword="false"/> and an error message if the game is not running or setup fails.
    /// </returns>
    /// <remarks>
    /// Safe to call repeatedly — re-uses the existing session if the game is
    /// still running and the channel is connected.
    /// </remarks>
    public async Task<(bool success, string? error)> LoadGameAsync(CancellationToken ct = default)
    {
        if (IsGameOpened) return (true, null);

        await _initLock.WaitAsync(ct).ConfigureAwait(false);
        try
        {
            if (IsGameOpened) return (true, null); // double-checked

            Process? proc = Process.GetProcessesByName(ProcessName).FirstOrDefault();
            if (proc is null) return (false, $"Process '{ProcessName}' was not found.");

            // Dispose any stale channel from a previous session.
            if (_channel is not null)
            {
                await _channel.DisposeAsync().ConfigureAwait(false);
                _channel = null;
            }

            var channel = new FridaChannel();
            await channel.ConnectAsync(proc.Id, ct).ConfigureAwait(false);

            _channel = channel;
            _process = proc;
            _process.EnableRaisingEvents = true;
            _process.Exited += (_, _) => _channel = null;
            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
        finally
        {
            _initLock.Release();
        }
    }

    // ── Public API — batch read ───────────────────────────────────────────────

    /// <summary>
    /// Read all tracked game values in a single Frida RPC round-trip.
    /// </summary>
    /// <remarks>
    /// Preferred over the individual synchronous properties in hot loops —
    /// one call reads everything.
    /// </remarks>
    /// <exception cref="Il2CppAccessException">
    ///   Read failed after <see cref="MaxRetries"/> attempts.
    /// </exception>
    public async Task<GameStateSnapshot> ReadAllAsync(CancellationToken ct = default)
        => await ReadWithRetryAsync(ct).ConfigureAwait(false);

    // ── Public API — individual properties ───────────────────────────────────
    // Synchronous wrappers kept for drop-in compatibility with PROMemoryManager.
    // In new code prefer ReadAllAsync().

    /// <summary>Current battle-menu state.</summary>
    public SelectedMenuEnum SelectedMenu
        => ReadWithRetrySync().SelectedMenu;

    /// <summary><see langword="true"/> when the Items sub-menu is open.</summary>
    public bool IsItemMenuSelected => SelectedMenu == SelectedMenuEnum.ItemsMenu;

    /// <summary><see langword="true"/> when no sub-menu is open (fight/none state).</summary>
    public bool IsNoMenuSelected => SelectedMenu == SelectedMenuEnum.FightOrNoneMenu;

    /// <summary>
    /// Pokémon ID of the active wild encounter, or <c>0</c> when not in battle.
    /// Maps to <c>DSSock.OtherPoke.oyu</c>.
    /// </summary>
    public int CurrentEncounterId => ReadWithRetrySync().CurrentEncounterId;

    /// <summary>
    /// <see langword="true"/> when inside a battle.
    /// Maps to <c>DSSock.ply ≠ 0</c>.
    /// </summary>
    public bool IsBattling => ReadWithRetrySync().IsBattling;

    /// <summary>
    /// Shiny flag (<c>0</c> = not shiny).
    /// Maps to <c>DSSock.OtherPoke.oyy</c>.
    /// </summary>
    public int ShinyForm => ReadWithRetrySync().ShinyForm;

    /// <summary>
    /// Event-form flag (<c>0</c> = not an event Pokémon).
    /// Maps to <c>DSSock.OtherPoke.oyz</c>.
    /// </summary>
    public int EventForm => ReadWithRetrySync().EventForm;

    /// <summary>
    /// <see langword="true"/> when the current encounter is shiny or an event Pokémon.
    /// </summary>
    public bool IsSpecial => ReadWithRetrySync().IsSpecial;
    /// <summary>Player tile X coordinate. Maps to <c>DSSock.TargetPos.x</c>.</summary>
    public float PlayerXPos => ReadWithRetrySync().PlayerX;

    /// <summary>Player tile Y coordinate. Maps to <c>DSSock.TargetPos.y</c>.</summary>
    public float PlayerYPos => ReadWithRetrySync().PlayerY;

    /// <summary>Player tile position as a <see cref="PointF"/>.</summary>
    public PointF PlayerPos => new(PlayerXPos, PlayerYPos);
    // ── IAsyncDisposable ──────────────────────────────────────────────────────

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        _initLock.Dispose();
        _process = null;

        FridaChannel? ch = _channel;
        _channel = null;
        if (ch is not null)
            await ch.DisposeAsync().ConfigureAwait(false);
    }

    // ── Private ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Async read with retry — preferred path used by <see cref="ReadAllAsync"/>.
    /// </summary>
    private async Task<GameStateSnapshot> ReadWithRetryAsync(CancellationToken ct)
    {
        FridaChannel channel = GetChannel();
        Exception?   last    = null;

        for (int attempt = 1; attempt <= MaxRetries; attempt++)
        {
            try
            {
                return await channel.ReadAsync(ct).ConfigureAwait(false);
            }
            catch (Il2CppAccessException ex)
            {
                last = ex;
                if (attempt < MaxRetries)
                    await Task.Delay(RetryDelay, ct).ConfigureAwait(false);
            }
        }

        // All retries exhausted — invalidate so LoadGameAsync can reconnect.
        _channel = null;
        throw new Il2CppAccessException(
            $"Game state read failed after {MaxRetries} attempts. "
          + "Ensure you are logged in and on the latest version of ProHack. "
          + $"Last error: {last?.Message}",
            last!);
    }

    /// <summary>
    /// Synchronous read with retry — used by the individual property accessors for
    /// backward compatibility with call sites that expect synchronous reads.
    /// Blocks the calling thread for the duration of the Frida round-trip.
    /// </summary>
    private GameStateSnapshot ReadWithRetrySync()
    {
        FridaChannel channel = GetChannel();
        Exception?   last    = null;

        for (int attempt = 1; attempt <= MaxRetries; attempt++)
        {
            try
            {
                return channel.ReadAsync().GetAwaiter().GetResult();
            }
            catch (Il2CppAccessException ex)
            {
                last = ex;
                if (attempt < MaxRetries)
                    Thread.Sleep(RetryDelay);
            }
        }

        _channel = null;
        throw new Il2CppAccessException(
            $"Game state read failed after {MaxRetries} attempts. "
          + "Ensure you are logged in and on the latest version of ProHack. "
          + $"Last error: {last?.Message}",
            last!);
    }

    private FridaChannel GetChannel()
        => _channel ?? throw new Il2CppAccessException(
            "Game not loaded. Call LoadGameAsync() first.");
}
