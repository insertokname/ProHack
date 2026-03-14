namespace Infrastructure.Il2Cpp.Core;

/// <summary>
/// Controls the fidelity of a map screenshot request.
/// </summary>
public enum ScreenshotMode
{
    /// <summary>
    /// Full quality: reads tile-sheet textures from the GPU, composites all four
    /// tile layers with proper sprites.  Time varies by map (~3–15 s for fresh maps,
    /// near-instant on subsequent screenshots of the same map).
    /// </summary>
    Normal,

    /// <summary>
    /// Fast mode: skips all GPU texture reads entirely.  Returns a flat gray image
    /// with collision, water, grass, ledge, and link overlays — visually identical
    /// to <see cref="Normal"/> for everything except the tile sprites.
    /// Typically completes in under 100 ms regardless of map size.
    /// </summary>
    LowRes,
}
