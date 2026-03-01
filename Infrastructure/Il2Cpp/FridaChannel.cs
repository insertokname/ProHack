using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Json;
using Frida;
using Infrastructure.Il2Cpp.Core;

namespace Infrastructure.Il2Cpp;

/// <summary>
/// Manages a persistent Frida session with the game process.
/// The in-process JS agent handles all memory reads, returning values via
/// Frida's message-passing protocol.
/// </summary>
/// <remarks>
/// <para>
/// <b>Connection lifecycle</b><br/>
/// Call <see cref="ConnectAsync"/> once after locating the game process.
/// The agent stays loaded for the duration of the session — Frida is never
/// detached until <see cref="DisposeAsync"/> is called.
/// </para>
///
/// <para>
/// <b>Anti-detection (handled in the JS agent)</b><br/>
/// On load the agent runs the <c>hide-frida-agent</c> routine
/// (<see href="https://github.com/nblog/hide-frida-agent"/>):
/// <list type="bullet">
///   <item>Unlinks <c>frida-agent.dll</c> from all three PEB loader lists.</item>
///   <item>Erases its PE headers from memory.</item>
///   <item>Intercepts <c>NtQueryVirtualMemory</c> to deny queries for the agent range.</item>
/// </list>
/// Additionally, the script is loaded under a random Unity-like module name chosen
/// per session from <see cref="_moduleNamePool"/>.
/// </para>
///
/// <para>
/// <b>Request protocol</b><br/>
/// C# → agent: <c>{"id":1,"cmd":"read"}</c><br/>
/// Agent → C#: <c>{"id":1,"ok":true,"data":{selectedMenu,isBattling,...}}</c>
///         or  <c>{"id":1,"ok":false,"error":"..."}</c><br/>
/// Agent → C# (startup): <c>{"cmd":"ready","ok":true}</c>
///                    or  <c>{"cmd":"ready","ok":false,"error":"..."}</c>
/// </para>
/// </remarks>
internal sealed class FridaChannel : IAsyncDisposable
{
    // ── Mixed-mode assembly resolver ─────────────────────────────────────────
    // Frida.dll (FridaCLR 17.x) is a mixed-mode C++/CLI assembly: .NET loads it
    // as a managed assembly. In a single-file
    // publish it is not bundled in the exe (excluded by the Presentation.csproj
    // target), so we embed it inside Infrastructure.dll as an EmbeddedResource and
    // extract it to LocalDownloads on first launch.  The AssemblyResolve hook
    // intercepts .NET's "I can't find Frida" failure and supplies the on-disk copy.
    static FridaChannel()
    {
        AppDomain.CurrentDomain.AssemblyResolve += (_, args) =>
        {
            string simpleName = new AssemblyName(args.Name).Name ?? string.Empty;
            if (!simpleName.Equals("Frida", StringComparison.OrdinalIgnoreCase))
                return null;

            string? path = TryExtractEmbeddedFrida();
            return path is not null ? Assembly.LoadFrom(path) : null;
        };
    }

    /// <summary>
    /// Extracts <c>Frida.dll</c> from this assembly's embedded resources to
    /// <see cref="FolderManager.LocalDownloads"/> so .NET can load it as a
    /// mixed-mode C++/CLI managed assembly.
    /// </summary>
    /// <returns>
    ///   The absolute path to the extracted file, or <see langword="null"/> on
    ///   failure (a diagnostic log is written to
    ///   <c>LocalDownloads\frida-extract.log</c>).
    /// </returns>
    private static string? TryExtractEmbeddedFrida()
    {
        try
        {
            Assembly asm = typeof(FridaChannel).Assembly; // Infrastructure.dll
            using Stream? src = asm.GetManifestResourceStream("Frida.dll");
            if (src is null) return null;

            string path = Path.Combine(FolderManager.LocalDownloads(), "Frida.dll");

            if (!File.Exists(path))
            {
                using FileStream dst = File.Open(
                    path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                src.CopyTo(dst);
            }

            return path;
        }
        catch (Exception ex)
        {
            try
            {
                File.WriteAllText(
                    Path.Combine(FolderManager.LocalDownloads(), "frida-extract.log"),
                    ex.ToString());
            }
            catch { }
            return null;
        }
    }

    private static readonly string[] _moduleNamePool =
    [
        "Insertokname.dll",
        // "UnityEngine.PhysicsModule",
        // "UnityEngine.AudioModule",
        // "UnityEngine.InputModule",
        // "UnityPlayer.CoreInterface",
        // "UnityEngine.RuntimeGraphics",
        // "UnityPlayer.AssetStreaming",
        // "UnityEngine.NativeRenderer",
        // "UnityPlayer.SubsystemManager",
    ];

    private static readonly Random _rng = new();

    private static string PickScriptName()
        => _moduleNamePool[_rng.Next(_moduleNamePool.Length)];

    // ── Agent resource ────────────────────────────────────────────────────────

    private const string AgentResourceSuffix = "Il2Cpp.Agent.Il2CppRpcAgent.js";

    private static string LoadAgentSource()
    {
        Assembly asm    = typeof(FridaChannel).Assembly;
        string resource = asm.GetManifestResourceNames()
            .FirstOrDefault(n => n.EndsWith(AgentResourceSuffix, StringComparison.Ordinal))
            ?? throw new InvalidOperationException(
                $"Embedded agent resource not found (suffix: '{AgentResourceSuffix}'). "
              + "Ensure 'Il2Cpp/Agent/Il2CppRpcAgent.js' is marked as EmbeddedResource.");

        using Stream      stream = asm.GetManifestResourceStream(resource)!;
        using StreamReader sr    = new(stream);
        return sr.ReadToEnd();
    }

    // ── Persistent session state ──────────────────────────────────────────────

    private DeviceManager? _deviceManager; // must stay alive — owns the GLib event loop
    private Session?       _session;
    private Script?        _script;

    // Monotonically increasing request ID, matched to responses by the agent.
    private int _nextId;

    // Pending reads: requestId → completion source for the data JsonElement.
    private readonly ConcurrentDictionary<int, TaskCompletionSource<JsonElement>> _pending = new();

    // Set once by the agent's {cmd:"ready"} startup message.
    private TaskCompletionSource<bool>? _readyTcs;

    // ── Public API ────────────────────────────────────────────────────────────

    /// <summary>
    /// <see langword="true"/> after <see cref="ConnectAsync"/> succeeds and
    /// before <see cref="DisposeAsync"/> is called.
    /// </summary>
    public bool IsConnected => _script is not null;

    /// <summary>
    /// Attach Frida to <paramref name="pid"/>, load the agent, and wait until
    /// it signals readiness (IL2CPP layout discovery complete).
    /// </summary>
    /// <remarks>
    /// No-op if already connected.
    /// </remarks>
    /// <exception cref="Il2CppAccessException">
    ///   Frida could not attach, the agent reported a layout-discovery error,
    ///   or the agent did not signal ready within 30 seconds.
    /// </exception>
    internal async Task ConnectAsync(int pid, CancellationToken ct = default)
    {
        if (IsConnected) return;

        string source     = LoadAgentSource();
        string scriptName = PickScriptName();

        _readyTcs = new TaskCompletionSource<bool>(
            TaskCreationOptions.RunContinuationsAsynchronously);

        try
        {
            // ── Attach + load ─────────────────────────────────────────────────
            // DeviceManager is stored as a field — disposing it would stop the GLib
            // event loop that delivers Frida messages, so it must remain alive.
            await Task.Run(() =>
            {
                _deviceManager  = new DeviceManager();
                Device local    = _deviceManager.EnumerateDevices().First(d => d.Type == DeviceType.Local);

                _session        = local.Attach((uint)pid);
                _script         = _session.CreateScript(source, scriptName);
                _script.Message += OnMessage;
                _script.Load();

            }, ct).ConfigureAwait(false);

            // ── Wait for {cmd:"ready"} from the agent ─────────────────────────
            using var linked = CancellationTokenSource.CreateLinkedTokenSource(ct);
            linked.CancelAfter(TimeSpan.FromSeconds(30));
            try
            {
                await _readyTcs.Task.WaitAsync(linked.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (!ct.IsCancellationRequested)
            {
                throw new Il2CppAccessException(
                    "connect",
                    "Agent did not signal ready within 30 seconds. The game may still be loading.");
            }
        }
        catch (Exception ex) when (ex is not Il2CppAccessException)
        {
            throw new Il2CppAccessException(
                $"Frida attach failed for PID {pid}: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Ask the in-process agent to read all game values from memory and return
    /// them as a <see cref="GameStateSnapshot"/>.
    /// </summary>
    /// <remarks>
    /// Multiple concurrent calls are safe — each request carries a unique ID
    /// and responses are routed back to the correct caller.
    /// </remarks>
    /// <exception cref="Il2CppAccessException">
    ///   Not connected, the agent reported a read error (e.g. a pointer in the
    ///   chain is null because the game is still loading), or the request timed out.
    /// </exception>
    internal async Task<GameStateSnapshot> ReadAsync(CancellationToken ct = default)
    {
        if (_script is null)
            throw new Il2CppAccessException(
                "read", "Not connected. Call ConnectAsync first.");

        int id  = Interlocked.Increment(ref _nextId);
        var tcs = new TaskCompletionSource<JsonElement>(
            TaskCreationOptions.RunContinuationsAsynchronously);

        _pending.TryAdd(id, tcs);
        try
        {
            // Post the read request to the agent's recv() handler.
            _script.Post(JsonSerializer.Serialize(new { id, cmd = "read" }));

            // Wait up to 5 s for the agent's response.
            using var linked = CancellationTokenSource.CreateLinkedTokenSource(ct);
            linked.CancelAfter(TimeSpan.FromSeconds(5));

            JsonElement data;
            try
            {
                data = await tcs.Task.WaitAsync(linked.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (!ct.IsCancellationRequested)
            {
                throw new Il2CppAccessException(
                    "read", "Read request timed out after 5 seconds.");
            }

            return ParseSnapshot(data);
        }
        finally
        {
            _pending.TryRemove(id, out _);
        }
    }

    // ── IAsyncDisposable ──────────────────────────────────────────────────────

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        // Cancel all in-flight reads so callers don't hang.
        foreach (var (_, tcs) in _pending)
            tcs.TrySetCanceled();
        _pending.Clear();

        Script?        script  = _script;
        Session?       session = _session;
        DeviceManager? mgr     = _deviceManager;
        _script        = null;
        _session       = null;
        _deviceManager = null;

        await Task.Run(() =>
        {
            try { script?.Unload();  } catch { /* best-effort */ }
            try { session?.Detach(); } catch { /* best-effort */ }
            try { mgr?.Dispose();    } catch { /* best-effort */ }
        }).ConfigureAwait(false);
    }

    // ── Message routing ───────────────────────────────────────────────────────

    /// <summary>
    /// Called on the Frida scheduler thread for every <c>send()</c> call in the agent.
    /// Frida wraps the payload as: <c>{"type":"send","payload":{...}}</c>.
    /// </summary>
    private void OnMessage(object? sender, ScriptMessageEventArgs e)
    {
        JsonDocument outer;
        try { outer = JsonDocument.Parse(e.Message); }
        catch { return; }

        using (outer)
        {
            var root = outer.RootElement;
            if (!root.TryGetProperty("type",    out var typeEl)  || typeEl.GetString() != "send") return;
            if (!root.TryGetProperty("payload", out var payload))                                   return;

            // ── Agent startup "ready" signal ──────────────────────────────────
            if (payload.TryGetProperty("cmd", out var cmdEl) && cmdEl.GetString() == "ready")
            {
                bool ok = payload.TryGetProperty("ok", out var okEl) && okEl.GetBoolean();
                if (ok)
                {
                    _readyTcs?.TrySetResult(true);
                }
                else
                {
                    string err = payload.TryGetProperty("error", out var errEl)
                        ? errEl.GetString() ?? "unknown agent error"
                        : "agent sent ok=false with no error field";
                    _readyTcs?.TrySetException(new Il2CppAccessException("connect", err));
                }
                return;
            }

            // ── Read responses (matched by id) ────────────────────────────────
            if (!payload.TryGetProperty("id", out var idEl)) return;
            int id = idEl.GetInt32();
            if (!_pending.TryGetValue(id, out var tcs)) return;

            bool readOk = payload.TryGetProperty("ok", out var readOkEl) && readOkEl.GetBoolean();
            if (readOk && payload.TryGetProperty("data", out var dataEl))
            {
                tcs.TrySetResult(dataEl.Clone());
            }
            else
            {
                string err = payload.TryGetProperty("error", out var readErrEl)
                    ? readErrEl.GetString() ?? "unknown read error"
                    : "agent returned ok=false without a data or error field";
                tcs.TrySetException(new Il2CppAccessException("read", err));
            }
        }
    }

    // ── Snapshot parsing ──────────────────────────────────────────────────────

    /// <summary>
    /// Parse the <c>data</c> JsonElement returned by the agent's read command
    /// into a <see cref="GameStateSnapshot"/>.
    /// </summary>
    private static GameStateSnapshot ParseSnapshot(JsonElement data)
    {
        int   selectedMenu       = data.GetProperty("selectedMenu").GetInt32();
        int   currentEncounterId = data.GetProperty("currentEncounterId").GetInt32();
        bool  isBattling         = data.GetProperty("isBattling").GetBoolean();
        int   shinyForm          = data.GetProperty("shinyForm").GetInt32();
        int   eventForm          = data.GetProperty("eventForm").GetInt32();
        float playerX            = data.GetProperty("playerX").GetSingle();
        float playerY            = data.GetProperty("playerY").GetSingle();

        return GameStateSnapshot.FromRaw(
            selectedMenuRaw:    selectedMenu,
            currentEncounterId: currentEncounterId,
            isBattlingRaw:      isBattling ? 1 : 0,
            shinyForm:          shinyForm,
            eventForm:          eventForm,
            playerX:            playerX,
            playerY:            playerY);
    }
}
