namespace Infrastructure.Il2Cpp.Core;

/// <summary>
/// Thrown when the IL2CPP Frida channel fails to read a game value.
/// Wraps the original error message forwarded from the in-process agent.
/// </summary>
/// <remarks>
/// Common root causes:
/// <list type="bullet">
///   <item><term>Setup not called</term>
///     <description><see cref="FridaChannel.SetupAsync"/> must complete before any read.</description></item>
///   <item><term>Player not logged in</term>
///     <description>The DSSock singleton is null while on the login screen.</description></item>
///   <item><term>Frida session lost</term>
///     <description>The game process was closed or crashed mid-session.</description></item>
///   <item><term>Stale class/field names</term>
///     <description>A game update renamed a class or field.  Re-run the diagnostic scripts
///     and update the agent's field-name constants (see UPDATE.md).</description></item>
/// </list>
/// </remarks>
public sealed class Il2CppAccessException : Exception
{
    /// <inheritdoc cref="Exception(string)"/>
    public Il2CppAccessException(string message)
        : base(message) { }

    /// <inheritdoc cref="Exception(string, Exception)"/>
    public Il2CppAccessException(string message, Exception innerException)
        : base(message, innerException) { }

    /// <summary>
    /// Constructs an exception carrying the raw agent error forwarded over the
    /// Frida message bus.
    /// </summary>
    /// <param name="method">The RPC method name that failed (e.g. "readGameState").</param>
    /// <param name="agentError">The error string returned by the in-process JS agent.</param>
    public Il2CppAccessException(string method, string agentError)
        : base($"Il2Cpp agent method '{method}' failed: {agentError}") { }
}
