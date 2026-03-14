using Infrastructure;
using Infrastructure.Il2Cpp;
using Infrastructure.Il2Cpp.Core;
using Infrastructure.Pathing;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Presentation;

public partial class PathingForm : Form
{
    private const int TileSize = 16;
    private const int TransitionPadding = 24;
    private const int PollIntervalMs = 10;
    private const float PointDeltaThreshold = 0.2f;

    private readonly PROIl2CppManager _manager;
    private PlayerPath _currentPath = new();
    // private string? _pathFilePath;

    private CancellationTokenSource? _recordCts;
    private string? _currentMapName;
    private int _currentSegIdx = -1;
    private float _lastX = float.NaN, _lastY = float.NaN;

    private Bitmap? _canvas;

    public PathingForm(PROIl2CppManager manager)
    {
        _manager = manager;
        InitializeComponent();
    }

    private void SetStatus(string text)
    {
        if (InvokeRequired)
        {
            Invoke(() => SetStatus(text));
            return;
        }

        lblStatus.Text = text;
    }

    private void btnRecord_Click(object sender, EventArgs e)
        => _ = StartRecordingAsync();

    private void btnStop_Click(object sender, EventArgs e)
        => _recordCts?.Cancel();

    private async void btnRefreshView_Click(object sender, EventArgs e)
        => await RefreshViewAsync();

    private async void btnOpen_Click(object sender, EventArgs e)
    {
        var pathsDir = Path.Combine(FolderManager.LocalDownloads(), "Paths");
        Directory.CreateDirectory(pathsDir);

        using var dlg = new OpenFileDialog
        {
            Filter = "Path files (*.PROHackPath)|*.PROHackPath|All files (*.*)|*.*",
            InitialDirectory = pathsDir,
            Title = "Open Player Path",
        };

        if (dlg.ShowDialog() != DialogResult.OK)
            return;

        try
        {
            _currentPath.Dispose();
            _currentPath = PlayerPath.LoadFromFile(dlg.FileName);
            _currentMapName = _currentPath.Segments.LastOrDefault()?.MapName;
            _currentSegIdx = _currentPath.Segments.Count - 1;

            if (_currentSegIdx >= 0)
            {
                var last = _currentPath.Segments[_currentSegIdx];
                if (last.Points.Count > 0)
                {
                    _lastX = last.Points[^1].X;
                    _lastY = last.Points[^1].Y;
                }
            }

            btnSave.Enabled = _currentPath.Segments.Count > 0;
            button1.Enabled = true;
            SetStatus($"Loaded: {Path.GetFileName(dlg.FileName)}\n{_currentPath.Segments.Count} segment(s).");
            await RefreshViewAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to load path:\n{ex.Message}",
                "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
        if (_currentPath.Segments.Count == 0)
        {
            MessageBox.Show("Nothing to save.", "Save",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        using var dlg = new SaveFileDialog
        {
            Filter = "Path files (*.PROHackPath)|*.PROHackPath",
            FileName = Path.GetFileName("path.PROHackPath"),
            Title = "Save Player Path",
        };

        if (dlg.ShowDialog() != DialogResult.OK)
            return;

        try
        {
            _currentPath.SaveToFile(dlg.FileName);
            SetStatus($"Saved: {Path.GetFileName(dlg.FileName)}");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to save path:\n{ex.Message}",
                "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void button1_Click(object sender, EventArgs e)
    {
        using var dlg = new SaveFileDialog
        {
            Filter = "Images (*.png)|*.png",
            FileName = "path.png",
            Title = "Save Path Image",
        };

        if (dlg.ShowDialog() != DialogResult.OK)
            return;

        picturePath.Image?.Save(dlg.FileName);
    }

    private async Task StartRecordingAsync()
    {
        _recordCts?.Dispose();
        _recordCts = new CancellationTokenSource();

        btnRecord.Enabled = false;
        btnStop.Enabled = true;
        btnOpen.Enabled = false;
        btnSave.Enabled = false;
        button1.Enabled = false;
        btnRefreshView.Enabled = false;
        SetStatus("Recording...");

        var mode = cbHighRes.Checked ? ScreenshotMode.Normal : ScreenshotMode.LowRes;

        _currentPath.Dispose();
        _currentPath = new PlayerPath();
        _currentMapName = null;
        _currentSegIdx = -1;
        _lastX = _lastY = float.NaN;

        // var pathsDir = Path.Combine(FolderManager.LocalDownloads(), "Paths");
        // Directory.CreateDirectory(pathsDir);
        // _pathFilePath = Path.Combine(pathsDir, $"_path_{DateTime.Now:yyyyMMdd_HHmmss}.PROHackPath");
        // Directory.CreateDirectory(PlayerPath.GetMapsDir(_pathFilePath));

        try
        {
            await CaptureCurrentMapAsync(mode, isInitialCapture: true, _recordCts.Token);
            await RecordLoopAsync(mode, _recordCts.Token);
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            btnRecord.Enabled = true;
            btnStop.Enabled = false;
            btnOpen.Enabled = true;
            btnRefreshView.Enabled = true;
            btnSave.Enabled = _currentPath.Segments.Count > 0;
            button1.Enabled = true;

            if (_currentPath.Segments.Count > 0)
            {
                // _currentPath.SaveToFile(_pathFilePath!);
                // SetStatus($"Saved: {Path.GetFileName(_pathFilePath)}\n{_currentPath.Segments.Count} segment(s).");
                await RefreshViewAsync();
            }
            else
            {
                SetStatus("Recording stopped - no data captured.");
            }
        }
    }

    private async Task RecordLoopAsync(ScreenshotMode mode, CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                var snapshot = await _manager.ReadAllAsync(ct);

                if (_currentSegIdx < 0)
                {
                    await CaptureCurrentMapAsync(mode, isInitialCapture: true, ct);
                }
                else if (!string.Equals(snapshot.MapName, _currentMapName, StringComparison.Ordinal))
                {
                    await CaptureCurrentMapAsync(mode, isInitialCapture: false, ct);
                    await RefreshViewAsync();
                }
                else
                {
                    AppendCurrentPosition(snapshot.PlayerX, snapshot.PlayerY);
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                SetStatus($"Error:\n{ex.Message}");
            }

            await Task.Delay(PollIntervalMs, ct);
        }
    }

    private async Task CaptureCurrentMapAsync(ScreenshotMode mode, bool isInitialCapture, CancellationToken ct)
    {
        MapScreenshot shot = default;
        Exception? lastError = null;

        for (int attempt = 0; attempt < 3; attempt++)
        {
            try
            {
                shot = await _manager.CaptureMapScreenshotAsync(mode, ct);
                if (_currentPath.Segments.Count > 0 && shot.Height == _currentPath.Segments[_currentPath.Segments.Count - 1].Height
                    && shot.Width == _currentPath.Segments[_currentPath.Segments.Count - 1].Width)
                {
                    Debug.WriteLine("Found a segment that has duplicate height and width! Waiting a bit and trying again!");
                    await Task.Delay(1000, ct);
                    shot = await _manager.CaptureMapScreenshotAsync(mode, ct);
                }
                lastError = null;
                break;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                lastError = ex;
                await Task.Delay(250, ct);
            }
        }

        if (lastError is not null)
            throw lastError;

        var existingSegment = _currentPath.Segments.FirstOrDefault(s => s.MapName == shot.MapName);
        if (existingSegment != null)
        {
            existingSegment.Points.Add(new PathPoint(float.NaN, float.NaN));
            _currentPath.Segments.Remove(existingSegment);
            _currentPath.Segments.Add(existingSegment);
            _currentSegIdx = _currentPath.Segments.Count - 1;
            _currentMapName = existingSegment.MapName;
            _lastX = _lastY = float.NaN;
        }

        if (!isInitialCapture && string.Equals(shot.MapName, _currentMapName, StringComparison.Ordinal))
            return;

        var previousSection = _currentSegIdx >= 0
            ? DetectTransitionSection(_currentPath.Segments[_currentSegIdx])
            : (int?)null;

        var segment = new PathSegment
        {
            Map = BitmapFromScreenshot(shot),
            MapName = shot.MapName,
            Width = shot.Width,
            Height = shot.Height,
            StartX = shot.StartX,
            StartY = shot.StartY,
            CollidersBase64 = EncodeGrid(shot.Colliders, shot.Width, shot.Height),
            LinksBase64 = EncodeGrid(shot.Links, shot.Width, shot.Height),
        };

        _currentPath.Segments.Add(segment);
        _currentSegIdx = _currentPath.Segments.Count - 1;
        _currentMapName = shot.MapName;
        _lastX = _lastY = float.NaN;

        AppendCurrentPosition(shot.PlayerX, shot.PlayerY, force: true);

        string sectionText = previousSection is int section
            ? $"Transition section: {section} ({SectionToText(section)})\n"
            : string.Empty;

        SetStatus(
            $"Recording...\n" +
            $"Map: {shot.MapName} ({shot.Width}x{shot.Height})\n" +
            sectionText +
            $"Segments: {_currentPath.Segments.Count}\n" +
            $"Mode: {(mode == ScreenshotMode.LowRes ? "low-res" : "high-res")}");
    }

    private void AppendCurrentPosition(float worldX, float worldY, bool force = false)
    {
        if (_currentSegIdx < 0)
            return;

        var seg = _currentPath.Segments[_currentSegIdx];
        float localX = worldX - seg.StartX;
        float localY = seg.StartY - worldY;

        if (!force && seg.Points.Count > 0
            && MathF.Abs(localX - _lastX) <= PointDeltaThreshold
            && MathF.Abs(localY - _lastY) <= PointDeltaThreshold)
        {
            return;
        }

        seg.Points.Add(new PathPoint(localX, localY));
        _lastX = localX;
        _lastY = localY;

        SetStatus(
            $"Recording...\n" +
            $"Map: {seg.MapName} ({seg.Width}x{seg.Height})\n" +
            $"Segments: {_currentPath.Segments.Count}\n" +
            $"Points on current map: {seg.Points.Count}\n" +
            $"Local: ({localX:F2}, {localY:F2})");
    }

    private async Task RefreshViewAsync()
    {
        if (_currentPath.Segments.Count == 0)
        {
            SetStatus("No path loaded.");
            return;
        }

        btnRefreshView.Enabled = false;
        SetStatus("Rendering...");
        try
        {
            var bmp = await Task.Run(() => RenderPathView(_currentPath));
            _canvas?.Dispose();
            _canvas = bmp;
            picturePath.Image = _canvas;
            picturePath.Size = _canvas.Size;
            SetStatus($"View updated.\n{_currentPath.Segments.Count} segment(s).");
        }
        catch (Exception ex)
        {
            SetStatus($"Render error:\n{ex.Message}");
        }
        finally
        {
            btnRefreshView.Enabled = true;
        }
    }

    private Bitmap RenderPathView(PlayerPath path)
    {
        if (path.Segments.Count == 0)
            return new Bitmap(1, 1);

        var nodes = new List<MapNode>(path.Segments.Count);
        var edges = new List<(MapNode From, MapNode To)>();
        var constraints = new List<SeparationConstraint>();

        for (int i = 0; i < path.Segments.Count; i++)
        {
            var seg = path.Segments[i];

            var node = new MapNode(seg.Map);
            nodes.Add(node);

            if (i == 0)
                continue;

            edges.Add((nodes[i - 1], node));
            AddSectionConstraints(
                nodes[i - 1],
                node,
                DetectTransitionSection(path.Segments[i - 1]),
                constraints);
        }

        var canvas = ColaLayout.Solve(nodes, edges, constraints);
        using var g = Graphics.FromImage(canvas);
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

        using var pathPen = new Pen(Color.FromArgb(220, 80, 230, 120), 2.5f)
        {
            StartCap = System.Drawing.Drawing2D.LineCap.Round,
            EndCap = System.Drawing.Drawing2D.LineCap.Round,
            LineJoin = System.Drawing.Drawing2D.LineJoin.Round,
        };
        using var transitionPen = new Pen(Color.FromArgb(180, 240, 220, 90), 1.5f)
        {
            DashStyle = System.Drawing.Drawing2D.DashStyle.Dash,
        };
        using var pointBrush = new SolidBrush(Color.FromArgb(230, 80, 230, 120));
        using var startBrush = new SolidBrush(Color.FromArgb(240, 255, 255, 90));
        using var endBrush = new SolidBrush(Color.FromArgb(240, 255, 90, 90));
        using var labelFont = new Font("Segoe UI", 8.5f, FontStyle.Bold);
        using var labelBrush = new SolidBrush(Color.FromArgb(235, 235, 235));
        using var shadowBrush = new SolidBrush(Color.FromArgb(170, 0, 0, 0));
        using var metaFont = new Font("Segoe UI", 7f);
        using var metaBrush = new SolidBrush(Color.FromArgb(200, 180, 220, 255));

        PointF TileToCanvas(MapNode node, PathPoint point)
            => new(node.X + (point.X + 0.5f) * TileSize, node.Y + (point.Y + 0.5f) * TileSize);

        for (int i = 0; i < path.Segments.Count; i++)
        {
            var seg = path.Segments[i];
            var node = nodes[i];

            g.DrawString(seg.MapName, labelFont, shadowBrush, node.X + 5f, node.Y + 5f);
            g.DrawString(seg.MapName, labelFont, labelBrush, node.X + 4f, node.Y + 4f);

            if (i > 0)
            {
                int section = DetectTransitionSection(path.Segments[i - 1]);
                g.DrawString($"from {section} ({SectionToText(section)})", metaFont, metaBrush, node.X + 4f, node.Y + 22f);
            }

            var splitPaths = new List<List<PathPoint>>();
            var current = new List<PathPoint>();

            for (int j = 0; j < seg.Points.Count; j++)
            {
                if (float.IsNaN(seg.Points[j].X) && float.IsNaN(seg.Points[j].Y))
                {
                    splitPaths.Add(current);
                    current = [];
                }
                else
                {
                    current.Add(seg.Points[j]);
                }
            }

            splitPaths.Add(current);

            foreach (var splitPath in splitPaths.Where(p => p.Count > 2))
            {
                g.DrawLines(pathPen, splitPath.Select(point => TileToCanvas(node, point)).ToArray());
            }

            foreach (var point in seg.Points)
            {
                var canvasPoint = TileToCanvas(node, point);
                g.FillEllipse(pointBrush, canvasPoint.X - 2f, canvasPoint.Y - 2f, 4f, 4f);
            }

            if (i == 0 && seg.Points.Count > 0)
            {
                var start = TileToCanvas(node, seg.Points[0]);
                g.FillEllipse(startBrush, start.X - 5f, start.Y - 5f, 10f, 10f);
            }

            if (i == path.Segments.Count - 1 && seg.Points.Count > 0)
            {
                var end = TileToCanvas(node, seg.Points[^1]);
                g.FillEllipse(endBrush, end.X - 5f, end.Y - 5f, 10f, 10f);
            }

            if (i + 1 < path.Segments.Count && seg.Points.Count > 0 && path.Segments[i + 1].Points.Count > 0)
            {
                var from = TileToCanvas(node, seg.Points[^1]);
                var to = TileToCanvas(nodes[i + 1], path.Segments[i + 1].Points[0]);
                g.DrawLine(transitionPen, from, to);
            }
        }

        return canvas;
    }

    // private static Bitmap LoadSegmentBitmap(PathSegment seg, string? pathFile, int index)
    // {
    //     if (!string.IsNullOrEmpty(pathFile))
    //     {
    //         string mapsDir = PlayerPath.GetMapsDir(pathFile);
    //         string exact = Path.Combine(mapsDir, $"{index:D4}_{SanitizeFileName(seg.MapName)}.png");
    //         if (File.Exists(exact))
    //             return new Bitmap(exact);

    //         string legacy = Path.Combine(mapsDir, SanitizeFileName(seg.MapName) + ".png");
    //         if (File.Exists(legacy))
    //             return new Bitmap(legacy);
    //     }

    //     return RenderFromColliders(seg);
    // }

    private static void AddSectionConstraints(
        MapNode previous,
        MapNode current,
        int section,
        List<SeparationConstraint> constraints)
    {
        int col = (section - 1) % 3; // 0=left, 1=center, 2=right
        int row = (section - 1) / 3; // 0=top, 1=middle, 2=bottom

        if (col == 0)
        {
            constraints.Add(new SeparationConstraint(current, previous, Axis.X, TransitionPadding));
        }
        else if (col == 2)
        {
            constraints.Add(new SeparationConstraint(previous, current, Axis.X, TransitionPadding));
        }
        if (row == 0)
        {
            constraints.Add(new SeparationConstraint(current, previous, Axis.Y, TransitionPadding));
        }
        else if (row == 2)
        {
            constraints.Add(new SeparationConstraint(previous, current, Axis.Y, TransitionPadding));
        }
    }

    private static int DetectTransitionSection(PathSegment segment)
    {
        if (segment.Points.Count == 0)
            return 5;

        // last player location should be teleporter location
        float x = segment.Points[^1].X;
        float y = segment.Points[^1].Y;

        int col = Bucket3(x, segment.Width);
        int row = Bucket3(y, segment.Height);
        return row * 3 + col + 1;
    }

    private static int Bucket3(float value, int size)
    {
        if (size <= 1)
            return 1;

        if (value <= size * 0.3f)
        {
            return 0;
        }
        else if (value <= size * 0.7f)
        {
            return 1;
        }
        else
        {
            return 2;
        }
    }

    private static string SectionToText(int section) => section switch
    {
        1 => "top-left",
        2 => "top",
        3 => "top-right",
        4 => "left",
        5 => "center",
        6 => "right",
        7 => "bottom-left",
        8 => "bottom",
        9 => "bottom-right",
        _ => "center",
    };

    private static string? EncodeGrid(byte[,]? grid, int width, int height)
    {
        if (grid is null)
            return null;

        var raw = new byte[width * height];
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                raw[x * height + y] = grid[x, y];

        return Convert.ToBase64String(raw);
    }

    private static Bitmap RenderFromColliders(PathSegment seg)
    {
        int width = Math.Max(1, seg.Width);
        int height = Math.Max(1, seg.Height);
        var bmp = new Bitmap(width * TileSize, height * TileSize);

        using var g = Graphics.FromImage(bmp);
        var colliders = seg.DecodeColliders();
        var links = seg.DecodeLinks();

        using var wallBrush = new SolidBrush(Color.FromArgb(110, 35, 35));
        using var ledgeBrush = new SolidBrush(Color.FromArgb(150, 95, 35));
        using var waterBrush = new SolidBrush(Color.FromArgb(35, 75, 155));
        using var grassBrush = new SolidBrush(Color.FromArgb(45, 95, 45));
        using var freeBrush = new SolidBrush(Color.FromArgb(75, 75, 75));
        using var linkBrush = new SolidBrush(Color.FromArgb(100, 60, 130, 255));

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                byte collider = colliders?[x, y] ?? 0;
                var brush = collider switch
                {
                    TileCollider.Wall => wallBrush,
                    TileCollider.LedgeDown or TileCollider.LedgeRight or TileCollider.LedgeLeft => ledgeBrush,
                    TileCollider.Water => waterBrush,
                    TileCollider.Grass => grassBrush,
                    _ => freeBrush,
                };

                g.FillRectangle(brush, x * TileSize, y * TileSize, TileSize, TileSize);

                if (links?[x, y] != 0)
                    g.FillRectangle(linkBrush, x * TileSize, y * TileSize, TileSize, TileSize);
            }

        return bmp;
    }

    private static void SaveMapImage(MapScreenshot shot, string mapsDir)
    {
        Directory.CreateDirectory(mapsDir);

        string uniqueName = Path.Combine(mapsDir, $"{Directory.GetFiles(mapsDir, "*.png").Length:D4}_{SanitizeFileName(shot.MapName)}.png");
        if (File.Exists(uniqueName))
            return;

        try
        {
            using var bmp = BitmapFromScreenshot(shot);
            bmp.Save(uniqueName, ImageFormat.Png);
        }
        catch
        {
        }
    }

    private static Bitmap BitmapFromScreenshot(MapScreenshot shot)
    {
        byte[] raw = (byte[])shot.RgbaData.Clone();
        for (int i = 0; i < raw.Length; i += 4)
            (raw[i], raw[i + 2]) = (raw[i + 2], raw[i]);

        var bmp = new Bitmap(shot.ImgWidth, shot.ImgHeight, PixelFormat.Format32bppArgb);
        var data = bmp.LockBits(
            new Rectangle(0, 0, shot.ImgWidth, shot.ImgHeight),
            ImageLockMode.WriteOnly,
            PixelFormat.Format32bppArgb);
        Marshal.Copy(raw, 0, data.Scan0, raw.Length);
        bmp.UnlockBits(data);
        return bmp;
    }

    private static string SanitizeFileName(string name)
        => string.Concat(name.Select(c => Path.GetInvalidFileNameChars().Contains(c) ? '_' : c));

    private void PathingForm_Load(object sender, EventArgs e)
    {
        btnSave.Enabled = false;
        btnStop.Enabled = false;
        button1.Enabled = false;
        picturePath.SizeMode = PictureBoxSizeMode.AutoSize;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _recordCts?.Cancel();
            _recordCts?.Dispose();
            _canvas?.Dispose();
            components?.Dispose();
        }

        base.Dispose(disposing);
    }
}
