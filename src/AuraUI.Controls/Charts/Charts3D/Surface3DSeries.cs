using Avalonia;
using Avalonia.Media;

namespace AuraUI.Controls.Charts.Charts3D;

/// <summary>
/// A 3D surface chart series. Renders a surface defined by a 2D grid of Y values
/// at XZ positions, using wireframe or filled polygons.
///
/// Usage:
///   var series = new Surface3DSeries();
///   series.XValues = new[] { 0, 0.25, 0.5, 0.75, 1.0 };
///   series.ZValues = new[] { 0, 0.25, 0.5, 0.75, 1.0 };
///   series.YValues = new double[,] {  // 5x5 grid
///       { 0.1, 0.3, 0.5, 0.3, 0.1 },
///       { 0.3, 0.6, 0.8, 0.6, 0.3 },
///       ...
///   };
///
/// Rendering:
///   - Surface is rendered as a grid of quadrilateral polygons
///   - Each quad is depth-sorted and rendered back-to-front
///   - Color gradient mapping based on Y value
///   - Supports wireframe mode
/// </summary>
public class Surface3DSeries : Chart3DSeries
{
    /// <summary>X grid positions (columns). Length = number of columns in YValues.</summary>
    public static readonly StyledProperty<double[]?> XValuesProperty =
        AvaloniaProperty.Register<Surface3DSeries, double[]?>(nameof(XValues));

    /// <summary>Z grid positions (rows). Length = number of rows in YValues.</summary>
    public static readonly StyledProperty<double[]?> ZValuesProperty =
        AvaloniaProperty.Register<Surface3DSeries, double[]?>(nameof(ZValues));

    /// <summary>Y values as a 2D array [row, col]. Each entry is the height at that grid point.</summary>
    public static readonly StyledProperty<double[,]?> YValuesProperty =
        AvaloniaProperty.Register<Surface3DSeries, double[,]?>(nameof(YValues));

    /// <summary>Color map for gradient mapping. Uses linear interpolation between these colors.</summary>
    public static readonly StyledProperty<Color[]?> ColorMapProperty =
        AvaloniaProperty.Register<Surface3DSeries, Color[]?>(nameof(ColorMap));

    /// <summary>Whether to render as wireframe only (no filled polygons).</summary>
    public static readonly StyledProperty<bool> IsWireframeProperty =
        AvaloniaProperty.Register<Surface3DSeries, bool>(nameof(IsWireframe));

    /// <summary>Wireframe line color.</summary>
    public static readonly StyledProperty<IBrush?> WireframeBrushProperty =
        AvaloniaProperty.Register<Surface3DSeries, IBrush?>(nameof(WireframeBrush));

    /// <summary>Wireframe line thickness.</summary>
    public static readonly StyledProperty<double> WireframeThicknessProperty =
        AvaloniaProperty.Register<Surface3DSeries, double>(nameof(WireframeThickness), 0.5);

    /// <summary>Whether to draw surface edges (outline of each quad).</summary>
    public static readonly StyledProperty<bool> ShowEdgesProperty =
        AvaloniaProperty.Register<Surface3DSeries, bool>(nameof(ShowEdges), true);

    /// <summary>Edge line color.</summary>
    public static readonly StyledProperty<IBrush?> EdgeBrushProperty =
        AvaloniaProperty.Register<Surface3DSeries, IBrush?>(nameof(EdgeBrush));

    /// <summary>Fill opacity of the surface polygons.</summary>
    public static readonly StyledProperty<double> FillOpacityProperty =
        AvaloniaProperty.Register<Surface3DSeries, double>(nameof(FillOpacity), 0.85);

    // CLR wrappers
    public double[]? XValues { get => GetValue(XValuesProperty); set => SetValue(XValuesProperty, value); }
    public double[]? ZValues { get => GetValue(ZValuesProperty); set => SetValue(ZValuesProperty, value); }
    public double[,]? YValues { get => GetValue(YValuesProperty); set => SetValue(YValuesProperty, value); }
    public Color[]? ColorMap { get => GetValue(ColorMapProperty); set => SetValue(ColorMapProperty, value); }
    public bool IsWireframe { get => GetValue(IsWireframeProperty); set => SetValue(IsWireframeProperty, value); }
    public IBrush? WireframeBrush { get => GetValue(WireframeBrushProperty); set => SetValue(WireframeBrushProperty, value); }
    public double WireframeThickness { get => GetValue(WireframeThicknessProperty); set => SetValue(WireframeThicknessProperty, value); }
    public bool ShowEdges { get => GetValue(ShowEdgesProperty); set => SetValue(ShowEdgesProperty, value); }
    public IBrush? EdgeBrush { get => GetValue(EdgeBrushProperty); set => SetValue(EdgeBrushProperty, value); }
    public double FillOpacity { get => GetValue(FillOpacityProperty); set => SetValue(FillOpacityProperty, value); }

    /// <summary>Default color map: blue to cyan to green to yellow to red.</summary>
    private static readonly Color[] DefaultColorMap = new[]
    {
        Color.Parse("#2196F3"), // Blue
        Color.Parse("#00BCD4"), // Cyan
        Color.Parse("#4CAF50"), // Green
        Color.Parse("#FFEB3B"), // Yellow
        Color.Parse("#F44336"), // Red
    };

    // ────────────────────────────────────────────────
    //  Rendering
    // ────────────────────────────────────────────────

    internal override void CollectRenderables(
        Projection3D projection,
        List<(Chart3DSeries series, int index, double depth, Action<DrawingContext> render)> renderables)
    {
        var xVals = XValues;
        var zVals = ZValues;
        var yVals = YValues;
        if (xVals == null || zVals == null || yVals == null) return;

        var rows = yVals.GetLength(0);
        var cols = yVals.GetLength(1);
        if (rows < 2 || cols < 2) return;

        // Find Y range for color mapping
        var minY = double.MaxValue;
        var maxY = double.MinValue;
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                if (yVals[r, c] < minY) minY = yVals[r, c];
                if (yVals[r, c] > maxY) maxY = yVals[r, c];
            }
        }
        var yRange = maxY - minY;
        if (yRange < 0.001) yRange = 1;

        var colorMap = ColorMap ?? DefaultColorMap;
        var fillOpacity = FillOpacity;
        var wireframe = IsWireframe;
        var showEdges = ShowEdges;
        var edgeBrush = EdgeBrush ?? new SolidColorBrush(Colors.White, 0.3);
        var wireBrush = WireframeBrush ?? Brushes.DarkGray;
        var wireThick = WireframeThickness;
        var series = this;

        // Generate quads for each grid cell
        for (int r = 0; r < rows - 1; r++)
        {
            for (int c = 0; c < cols - 1; c++)
            {
                var c0 = Math.Min(c, xVals.Length - 1);
                var c1 = Math.Min(c + 1, xVals.Length - 1);
                var r0 = Math.Min(r, zVals.Length - 1);
                var r1 = Math.Min(r + 1, zVals.Length - 1);

                // Four corners of the quad in 3D
                var p00 = new Point3D(xVals[c0], yVals[r, c], zVals[r0]);
                var p10 = new Point3D(xVals[c1], yVals[r, c + 1], zVals[r0]);
                var p11 = new Point3D(xVals[c1], yVals[r + 1, c + 1], zVals[r1]);
                var p01 = new Point3D(xVals[c0], yVals[r + 1, c], zVals[r1]);

                // Center for depth sorting
                var center = new Point3D(
                    (p00.X + p10.X + p11.X + p01.X) / 4,
                    (p00.Y + p10.Y + p11.Y + p01.Y) / 4,
                    (p00.Z + p10.Z + p11.Z + p01.Z) / 4);
                var depth = projection.GetDepth(center);

                // Average Y for color
                var avgY = (p00.Y + p10.Y + p11.Y + p01.Y) / 4;
                var t = (avgY - minY) / yRange;
                var color = SampleColorMap(colorMap, t);

                // Project corners
                var s00 = projection.Project(p00);
                var s10 = projection.Project(p10);
                var s11 = projection.Project(p11);
                var s01 = projection.Project(p01);

                var quadColor = new SolidColorBrush(color, fillOpacity);
                var edge = showEdges ? edgeBrush : null;
                var quadIdx = r * cols + c;

                renderables.Add((series, quadIdx, depth, ctx =>
                {
                    if (wireframe)
                    {
                        var pen = new Pen(wireBrush, wireThick);
                        ctx.DrawLine(pen, s00, s10);
                        ctx.DrawLine(pen, s10, s11);
                        ctx.DrawLine(pen, s11, s01);
                        ctx.DrawLine(pen, s01, s00);
                    }
                    else
                    {
                        DrawQuadFilled(ctx, s00, s10, s11, s01, quadColor, edge);
                    }
                }));
            }
        }
    }

    /// <summary>Draw a filled quad with optional edge outline.</summary>
    private static void DrawQuadFilled(DrawingContext ctx, Point p0, Point p1, Point p2, Point p3,
        IBrush fill, IBrush? edgeBrush)
    {
        var geometry = new StreamGeometry();
        using (var sgCtx = geometry.Open())
        {
            sgCtx.BeginFigure(p0, true);
            sgCtx.LineTo(p1);
            sgCtx.LineTo(p2);
            sgCtx.LineTo(p3);
            sgCtx.EndFigure(true);
        }

        var pen = edgeBrush != null ? new Pen(edgeBrush, 0.5) : null;
        ctx.DrawGeometry(fill, pen, geometry);
    }

    /// <summary>Sample the color map at position t (0..1).</summary>
    private static Color SampleColorMap(Color[] colorMap, double t)
    {
        t = Math.Clamp(t, 0, 1);
        if (colorMap.Length == 0) return Colors.Gray;
        if (colorMap.Length == 1) return colorMap[0];

        var scaledT = t * (colorMap.Length - 1);
        var idx = (int)scaledT;
        var frac = scaledT - idx;

        if (idx >= colorMap.Length - 1) return colorMap[^1];

        var c0 = colorMap[idx];
        var c1 = colorMap[idx + 1];

        return Color.FromRgb(
            (byte)(c0.R + (c1.R - c0.R) * frac),
            (byte)(c0.G + (c1.G - c0.G) * frac),
            (byte)(c0.B + (c1.B - c0.B) * frac));
    }

    // ────────────────────────────────────────────────
    //  Hit testing
    // ────────────────────────────────────────────────

    internal override (int index, double distance)? HitTest(Point screenPos, Projection3D projection)
    {
        var xVals = XValues;
        var zVals = ZValues;
        var yVals = YValues;
        if (xVals == null || zVals == null || yVals == null) return null;

        var rows = yVals.GetLength(0);
        var cols = yVals.GetLength(1);
        (int index, double distance)? best = null;

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                var c0 = Math.Min(c, xVals.Length - 1);
                var r0 = Math.Min(r, zVals.Length - 1);
                var point3d = new Point3D(xVals[c0], yVals[r, c], zVals[r0]);
                var screenPoint = projection.Project(point3d);
                var dist = (screenPos - screenPoint).Length;

                if (dist < 15 && (!best.HasValue || dist < best.Value.distance))
                {
                    best = (r * cols + c, dist);
                }
            }
        }

        return best;
    }

    internal override string? GetTooltipText(int index)
    {
        var xVals = XValues;
        var zVals = ZValues;
        var yVals = YValues;
        if (xVals == null || zVals == null || yVals == null || index < 0) return null;

        var rows = yVals.GetLength(0);
        var cols = yVals.GetLength(1);
        var r = index / cols;
        var c = index % cols;

        if (r >= rows || c >= cols) return null;

        var c0 = Math.Min(c, xVals.Length - 1);
        var r0 = Math.Min(r, zVals.Length - 1);
        var name = Title ?? "Surface3D";
        return $"{name}\nX: {xVals[c0]:F2}\nY: {yVals[r, c]:F2}\nZ: {zVals[r0]:F2}";
    }
}
