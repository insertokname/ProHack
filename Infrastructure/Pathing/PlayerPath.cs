using System.Drawing;
using System.Drawing.Imaging;
using System.IO.Compression;
using System.Xml;
using System.Xml.Serialization;

namespace Infrastructure.Pathing;

/// <summary>
/// A recorded sequence of tile positions across one or more maps.
/// Serialised as a zip containing path.xml + maps/N.jpg.
/// </summary>
[XmlRoot("PlayerPath")]
public class PlayerPath : IDisposable
{
    [XmlArray("Segments")]
    [XmlArrayItem("Segment")]
    public List<PathSegment> Segments { get; set; } = [];

    private static readonly XmlSerializer _sz = new(typeof(PlayerPath));

    private const string XmlEntry = "path.xml";
    private const string MapsDir = "maps/";

    public void SaveToFile(string path)
    {
        using var xmlMs = new MemoryStream();
        var xmlSettings = new XmlWriterSettings { Indent = true, IndentChars = "  " };
        using (var writer = XmlWriter.Create(xmlMs, xmlSettings))
        {
            _sz.Serialize(writer, this);
        }

        var jpegStreams = Segments
            .Select(seg =>
            {
                var ms = new MemoryStream();
                seg.Map.Save(ms, ImageFormat.Jpeg);
                return ms;
            })
            .ToList();

        using var zipMs = new MemoryStream();
        using (var archive = new ZipArchive(zipMs, ZipArchiveMode.Create, leaveOpen: true))
        {
            WriteEntry(archive, XmlEntry, xmlMs);

            for (int i = 0; i < jpegStreams.Count; i++)
            {
                WriteEntry(archive, $"{MapsDir}{i}.jpg", jpegStreams[i]);
            }
        }

        foreach (var js in jpegStreams)
        {
            js.Dispose();
        }

        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllBytes(path, zipMs.ToArray());
    }

    private static void WriteEntry(ZipArchive archive, string entryName, MemoryStream source)
    {
        var entry = archive.CreateEntry(entryName, CompressionLevel.Optimal);
        using var entryStream = entry.Open();
        source.Position = 0;
        source.CopyTo(entryStream);
    }

    public static PlayerPath LoadFromFile(string path)
    {
        using var zipMs = new MemoryStream(File.ReadAllBytes(path));
        using var archive = new ZipArchive(zipMs, ZipArchiveMode.Read);

        var xmlEntry = archive.GetEntry(XmlEntry)
            ?? throw new InvalidDataException($"'{XmlEntry}' not found in zip.");

        PlayerPath playerPath;
        using (var entryStream = xmlEntry.Open())
        using (var reader = new StreamReader(entryStream))
        {
            playerPath = (PlayerPath)(_sz.Deserialize(reader) ?? new PlayerPath());
        }

        for (int i = 0; i < playerPath.Segments.Count; i++)
        {
            var mapEntry = archive.GetEntry($"{MapsDir}{i}.jpg")
                ?? throw new InvalidDataException($"Missing bitmap for segment {i}.");

            using var entryStream = mapEntry.Open();
            var bitmapMs = new MemoryStream();
            entryStream.CopyTo(bitmapMs);
            bitmapMs.Position = 0;

            playerPath.Segments[i].Map = new Bitmap(bitmapMs);
        }

        return playerPath;
    }

    public void Dispose()
    {
        foreach (var seg in Segments)
        {
            seg.Dispose();
        }
    }
}

/// <summary>One continuous run on a single map.</summary>
public class PathSegment : IDisposable
{
    /// <summary>A bitmap of the whole map</summary>
    [XmlIgnore] public required Bitmap Map { get; set; }
    /// <summary>Game map name (e.g. "Cerulean City").</summary>
    [XmlAttribute] public string MapName { get; set; } = "";

    /// <summary>Map width in tiles.</summary>
    [XmlAttribute] public int Width { get; set; }

    /// <summary>Map height in tiles.</summary>
    [XmlAttribute] public int Height { get; set; }

    /// <summary>World tile-X of the map origin tile [0,0]. Used to convert world→local coords.</summary>
    [XmlAttribute] public int StartX { get; set; }

    /// <summary>World tile-Y of the map origin tile [0,0]. Used to convert world→local coords.</summary>
    [XmlAttribute] public int StartY { get; set; }

    /// <summary>Ordered list of tile positions visited on this map.</summary>
    [XmlArray("Points")]
    [XmlArrayItem("P")]
    public List<PathPoint> Points { get; set; } = [];

    /// <summary>
    /// Base64-encoded flat byte array of collider values, column-major
    /// (<c>index = x * Height + y</c>).
    /// </summary>
    [XmlElement("Colliders")]
    public string? CollidersBase64 { get; set; }

    /// <summary>
    /// Base64-encoded flat byte array of link values, column-major.
    /// Non-zero = map-transition tile.
    /// </summary>
    [XmlElement("Links")]
    public string? LinksBase64 { get; set; }

    /// <summary>Decode <see cref="CollidersBase64"/> to a <c>[Width, Height]</c> 2-D array.</summary>
    public byte[,]? DecodeColliders() => DecodeGrid(CollidersBase64, Width, Height);

    /// <summary>Decode <see cref="LinksBase64"/> to a <c>[Width, Height]</c> 2-D array.</summary>
    public byte[,]? DecodeLinks() => DecodeGrid(LinksBase64, Width, Height);

    private static byte[,]? DecodeGrid(string? b64, int w, int h)
    {
        if (b64 is null || w <= 0 || h <= 0)
            return null;
        byte[] raw = Convert.FromBase64String(b64);
        var grid = new byte[w, h];
        for (int x = 0; x < w; x++)
            for (int y = 0; y < h; y++)
            {
                int i = x * h + y;
                if (i < raw.Length)
                    grid[x, y] = raw[i];
            }
        return grid;
    }

    public void Dispose()
    {
        Map.Dispose();
    }
}

/// <summary>A single recorded tile position.</summary>
public class PathPoint
{
    [XmlAttribute] public float X { get; set; }
    [XmlAttribute] public float Y { get; set; }

    public PathPoint() { }
    public PathPoint(float x, float y) { X = x; Y = y; }
}