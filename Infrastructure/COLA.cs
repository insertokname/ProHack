using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Infrastructure
{
    public enum Axis { X, Y }

    public class MapNode
    {
        private int TilesW, TilesH;
        public Bitmap Map;

        public float X, Y;

        public float PixelW => TilesW * 16f;
        public float PixelH => TilesH * 16f;
        public float CenterX => X + PixelW / 2f;
        public float CenterY => Y + PixelH / 2f;

        public MapNode(Bitmap bmp)
        {
            Map = bmp;
            TilesW = bmp.Width / 16;
            TilesH = bmp.Height / 16;
        }
    }

    /// <summary>
    /// Enforces:  Right.X >= Left.X + Left.PixelW + Padding   (Axis.X)
    ///        or  Right.Y >= Left.Y + Left.PixelH + Padding   (Axis.Y)
    ///
    /// Usage by teleporter side:
    ///   Teleporter on RIGHT of A  -> new SeparationConstraint(A, B, Axis.X)
    ///   Teleporter on LEFT  of A  -> new SeparationConstraint(B, A, Axis.X)
    ///   Teleporter on BOTTOM of A -> new SeparationConstraint(A, B, Axis.Y)
    ///   Teleporter on TOP    of A -> new SeparationConstraint(B, A, Axis.Y)
    /// </summary>
    public class SeparationConstraint
    {
        public MapNode Left, Right;
        public Axis Axis;
        public float Padding;

        public SeparationConstraint(MapNode left, MapNode right, Axis axis, float padding = 64f)
        {
            Left = left;
            Right = right;
            Axis = axis;
            Padding = padding;
        }

        public float RequiredGap =>
            Axis == Axis.X ? Left.PixelW + Padding
                           : Left.PixelH + Padding;
    }

    public static class ColaLayout
    {
        const float RepulsionStrength = 250_000f;
        const float SpringK = 0.035f;
        const float Damping = 0.85f;
        const int Iterations = 800;
        const int ProjectionPasses = 10;
        const int CanvasMargin = 48;

        public static Bitmap Solve(
            List<MapNode> nodes,
            List<(MapNode From, MapNode To)> edges,
            List<SeparationConstraint> constraints)
        {
            if (nodes.Count == 0) return new Bitmap(1, 1);

            if (nodes.Count == 1)
            {
                nodes[0].X = CanvasMargin;
                nodes[0].Y = CanvasMargin;
                return Render(nodes, edges);
            }

            InitPositions(nodes);

            var idx = new Dictionary<MapNode, int>(nodes.Count);
            for (int i = 0; i < nodes.Count; i++) idx[nodes[i]] = i;

            var vx = new float[nodes.Count];
            var vy = new float[nodes.Count];

            for (int iter = 0; iter < Iterations; iter++)
            {
                float heat = 1f - (float)iter / Iterations;
                ApplyForces(nodes, edges, idx, vx, vy, heat);

                for (int p = 0; p < ProjectionPasses; p++)
                {
                    ProjectSeparationConstraints(constraints);
                    RemoveOverlaps(nodes);
                }
            }

            // Final hard-enforcement pass.
            for (int p = 0; p < 30; p++)
            {
                ProjectSeparationConstraints(constraints);
                RemoveOverlaps(nodes);
            }

            return Render(nodes, edges);
        }

        static void ApplyForces(
            List<MapNode> nodes,
            List<(MapNode From, MapNode To)> edges,
            Dictionary<MapNode, int> idx,
            float[] vx, float[] vy,
            float heat)
        {
            var fx = new float[nodes.Count];
            var fy = new float[nodes.Count];

            // Repulsion — area-weighted so large nodes push proportionally harder.
            for (int i = 0; i < nodes.Count; i++)
                for (int j = i + 1; j < nodes.Count; j++)
                {
                    var a = nodes[i];
                    var b = nodes[j];

                    float dx = b.CenterX - a.CenterX;
                    float dy = b.CenterY - a.CenterY;
                    float dist2 = dx * dx + dy * dy + 1f;
                    float dist = MathF.Sqrt(dist2);
                    float area = (a.PixelW * a.PixelH + b.PixelW * b.PixelH) * 0.5f;
                    float force = (RepulsionStrength * area) / (dist2 * 10_000f);
                    float nx = dx / dist;
                    float ny = dy / dist;

                    fx[i] -= force * nx; fy[i] -= force * ny;
                    fx[j] += force * nx; fy[j] += force * ny;
                }

            // Spring attraction along edges.
            foreach (var (from, to) in edges)
            {
                int i = idx[from], j = idx[to];
                float dx = to.CenterX - from.CenterX;
                float dy = to.CenterY - from.CenterY;
                fx[i] += SpringK * dx; fy[i] += SpringK * dy;
                fx[j] -= SpringK * dx; fy[j] -= SpringK * dy;
            }

            for (int i = 0; i < nodes.Count; i++)
            {
                vx[i] = (vx[i] + fx[i] * heat) * Damping;
                vy[i] = (vy[i] + fy[i] * heat) * Damping;
                nodes[i].X += vx[i];
                nodes[i].Y += vy[i];
            }
        }

        static void ProjectSeparationConstraints(List<SeparationConstraint> constraints)
        {
            foreach (var c in constraints)
            {
                float leftPos = c.Axis == Axis.X ? c.Left.X : c.Left.Y;
                float rightPos = c.Axis == Axis.X ? c.Right.X : c.Right.Y;

                float violation = (leftPos + c.RequiredGap) - rightPos;
                if (violation <= 0f) continue;

                float half = violation * 0.5f;
                if (c.Axis == Axis.X) { c.Left.X -= half; c.Right.X += half; }
                else { c.Left.Y -= half; c.Right.Y += half; }
            }
        }

        static void RemoveOverlaps(List<MapNode> nodes)
        {
            for (int i = 0; i < nodes.Count; i++)
                for (int j = i + 1; j < nodes.Count; j++)
                {
                    var a = nodes[i];
                    var b = nodes[j];

                    float penX = MathF.Min(a.X + a.PixelW, b.X + b.PixelW) - MathF.Max(a.X, b.X);
                    float penY = MathF.Min(a.Y + a.PixelH, b.Y + b.PixelH) - MathF.Max(a.Y, b.Y);

                    // Both must be positive for a real overlap.
                    if (penX <= 0f || penY <= 0f) continue;

                    float half;
                    if (penX <= penY)
                    {
                        half = penX * 0.5f;
                        if (a.CenterX <= b.CenterX) { a.X -= half; b.X += half; }
                        else { a.X += half; b.X -= half; }
                    }
                    else
                    {
                        half = penY * 0.5f;
                        if (a.CenterY <= b.CenterY) { a.Y -= half; b.Y += half; }
                        else { a.Y += half; b.Y -= half; }
                    }
                }
        }

        static Bitmap Render(List<MapNode> nodes, List<(MapNode From, MapNode To)> edges)
        {
            float minX = float.MaxValue, minY = float.MaxValue;
            foreach (var n in nodes)
            {
                if (n.X < minX) minX = n.X;
                if (n.Y < minY) minY = n.Y;
            }
            foreach (var n in nodes)
            {
                n.X -= minX - CanvasMargin;
                n.Y -= minY - CanvasMargin;
            }

            float maxX = 0f, maxY = 0f;
            foreach (var n in nodes)
            {
                if (n.X + n.PixelW > maxX) maxX = n.X + n.PixelW;
                if (n.Y + n.PixelH > maxY) maxY = n.Y + n.PixelH;
            }

            var bmp = new Bitmap((int)maxX + CanvasMargin, (int)maxY + CanvasMargin);
            using var g = Graphics.FromImage(bmp);
            g.Clear(Color.FromArgb(30, 30, 30));
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            using var edgePen = new Pen(Color.FromArgb(140, 190, 230), 1.5f);
            foreach (var (from, to) in edges)
                g.DrawLine(edgePen, from.CenterX, from.CenterY, to.CenterX, to.CenterY);

            foreach (var n in nodes)
            {
                var rect = new RectangleF(n.X, n.Y, n.PixelW, n.PixelH);
                if (n.Map != null)
                {
                    g.DrawImage(n.Map, rect);
                }
                else
                {
                    using var fill = new SolidBrush(Color.FromArgb(55, 75, 100));
                    using var border = new Pen(Color.FromArgb(140, 190, 230), 1.5f);
                    g.FillRectangle(fill, rect);
                    g.DrawRectangle(border, rect.X, rect.Y, rect.Width, rect.Height);
                }
            }

            return bmp;
        }

        static void InitPositions(List<MapNode> nodes)
        {
            float totalPerimeter = 0f;
            foreach (var n in nodes) totalPerimeter += n.PixelW + n.PixelH;
            float radius = MathF.Max(300f, totalPerimeter / MathF.PI);

            for (int i = 0; i < nodes.Count; i++)
            {
                float angle = 2f * MathF.PI * i / nodes.Count;
                nodes[i].X = radius + MathF.Cos(angle) * radius - nodes[i].PixelW * 0.5f;
                nodes[i].Y = radius + MathF.Sin(angle) * radius - nodes[i].PixelH * 0.5f;
            }
        }
    }
}
