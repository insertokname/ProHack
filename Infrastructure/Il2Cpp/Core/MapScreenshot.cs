namespace Infrastructure.Il2Cpp.Core;

/// <summary>
/// Tile collision values returned in <see cref="MapScreenshot.Colliders"/>.
/// These mirror the raw byte values stored in the game's <c>MapCreator.Colliders</c> array.
/// </summary>
public static class TileCollider
{
    /// <summary>No collision — the tile is freely walkable.</summary>
    public const byte Free      = 0;
    /// <summary>Solid wall / obstacle — impassable.</summary>
    public const byte Wall      = 1;
    /// <summary>Ledge: can jump <em>down</em>, but not up.</summary>
    public const byte LedgeDown  = 2;
    /// <summary>Ledge: can jump <em>right</em>, but not left.</summary>
    public const byte LedgeRight = 3;
    /// <summary>Ledge: can jump <em>left</em>, but not right.</summary>
    public const byte LedgeLeft  = 4;
    /// <summary>Water — requires Surf to enter.</summary>
    public const byte Water      = 5;
    /// <summary>Tall grass — walkable, triggers wild encounters.</summary>
    public const byte Grass      = 6;
}

/// <summary>
/// Immutable snapshot of the current in-game map as a raw RGBA image.
/// </summary>
/// <param name="MapName">The internal name of the map (e.g. "Cerulean City").</param>
/// <param name="Width">Map width in tiles.</param>
/// <param name="Height">Map height in tiles.</param>
/// <param name="ImgWidth">Composited image width in pixels.</param>
/// <param name="ImgHeight">Composited image height in pixels.</param>
/// <param name="PlayerX">Player world tile-X position (NOT map-local; subtract <see cref="StartX"/> to get 0-based).</param>
/// <param name="PlayerY">Player world tile-Y position (NOT map-local; subtract <see cref="StartY"/> to get 0-based).</param>
/// <param name="StartX">World tile-X of the map's origin tile [0,0]. 0 if unknown.</param>
/// <param name="StartY">World tile-Y of the map's origin tile [0,0]. 0 if unknown.</param>
/// <param name="RgbaData">Raw RGBA pixel bytes, top-down, 4 bytes per pixel (R, G, B, A).</param>
/// <param name="Colliders">
///   Per-tile collision values indexed <c>[x, y]</c>. See <see cref="TileCollider"/> for
///   the meaning of each byte. <see langword="null"/> when the agent did not return the data.
/// </param>
/// <param name="Links">
///   Per-tile map-link markers indexed <c>[x, y]</c>. A non-zero value means the tile
///   triggers a transition to another map (e.g. building door, cave entrance).
///   <see langword="null"/> when the agent did not return the data.
/// </param>
public readonly record struct MapScreenshot(
    string    MapName,
    int       Width,
    int       Height,
    int       ImgWidth,
    int       ImgHeight,
    float     PlayerX,
    float     PlayerY,
    int       StartX,
    int       StartY,
    byte[]    RgbaData,
    byte[,]?  Colliders,
    byte[,]?  Links);
