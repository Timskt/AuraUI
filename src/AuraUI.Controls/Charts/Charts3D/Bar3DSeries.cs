using Avalonia;
using Avalonia.Media;
using AvaloniaColor = Avalonia.Media.Color;

namespace AuraUI.Controls.Charts.Charts3D;

/// <summary>
/// A 3D bar chart series. Renders rectangular bars at XZ positions with Y height
/// using isometric/perspective projection.
///
/// Usage:
///   var series = new Bar3DSeries();
///   series.XValues = new[] { 0, 0, 1, 1 };
///   series.YValues = new[] { 0.5, 0.8, 0.3, 0.9 };  // Heights
///   series.ZValues = new[] { 0, 1, 0, 1 };
///   series.Colors = new IBrush[] { Brushes.Red, Brushes.Blue, Brushes.Green, Brushes.Orange };
///
/// Rendering:
///   - Each bar is rendered as a 3D box with top, left, and right faces
///   - Faces are depth-sorted and rendered back-to-front
///   - Color coding by value or explicit per-bar colors
///   - Supports mouse drag rotation via parent Chart3D
/// </summary>
public class Bar3DSeries : Chart3DSeries
{
    /// <summary>X positions of the bars (normalized 0..1).</summary>
    public static readonly StyledProperty<double[]?> XValuesProperty =
        AvaloniaProperty.Register<Bar3DSeries, double[]?>(nameof(XValues));

    /// <summary>Y values (bar heights, normalized 0..1).</summary>
    public static readonly StyledProperty<double[]?> YValuesProperty =
        AvaloniaProperty.Register<Bar3DSeries, double[]?>(nameof(YValues));

    /// <summary>Z positions of the bars (normalized 0..1).</summary>
    public static readonly StyledProperty<double[]?> ZValuesProperty =
        AvaloniaProperty.Register<Bar3DSeries, double[]?>(nameof(ZValues));

    /// <summary>Per-bar color overrides. When null, a gradient based on Y value is used.</summary>
    public static readonly StyledProperty<IBrush[]?> ColorsProperty =
        AvaloniaProperty.Register<Bar3DSeries, IBrush[]?>(nameof(Colors));

    /// <summary>Width of each bar in normalized units (0..1 range).</summary>
    public static readonly StyledProperty<double> BarWidthProperty =
        AvaloniaProperty.Register<Bar3DSeries, double>(nameof(BarWidth), 0.08);

    /// <summary>Depth of each bar in normalized units (0..1 range).</summary>
    public static readonly StyledProperty<double> BarDepthProperty =
        AvaloniaProperty.Register<Bar3DSeries, double>(nameof(BarDepth), 0.08);

    /// <summary>Base color for the gradient (low values).</summary>
    public static readonly StyledProperty<Avalonia.Media.Color> ColorLowProperty =
        AvaloniaProperty.Register<Bar3DSeries, Avalonia.Media.Color>(nameof(ColorLow), Avalonia.Media.Color.Parse("#4FC3F7"));

    /// <summary>Top color for the gradient (high values).</summary>
    public static readonly StyledProperty<Avalonia.Media.Color> ColorHighProperty =
        AvaloniaProperty.Register<Bar3DSeries, Avalonia.Media.Color>(nameof(ColorHigh), Avalonia.Media.Color.Parse("#1565C0"));

    // CLR wrappers
    public double[]? XValues { get => GetValue(XValuesProperty); set => SetValue(XValuesProperty, value); }
    public double[]? YValues { get => GetValue(YValuesProperty); set => SetValue(YValuesProperty, value); }
    public double[]? ZValues { get => GetValue(ZValuesProperty); set => SetValue(ZValuesProperty, value); }
    public IBrush[]? Colors { get => GetValue(ColorsProperty); set => SetValue(ColorsProperty, value); }
    public double BarWidth { get => GetValue(BarWidthProperty); set => SetValue(BarWidthProperty, value); }
    public double BarDepth { get => GetValue(BarDepthProperty); set => SetValue(BarDepthProperty, value); }
    public AvaloniaColor ColorLow { get => GetValue(ColorLowProperty); set => SetValue(ColorLowProperty, value); }
    public AvaloniaColor ColorHigh { get => GetValue(ColorHighProperty); set => SetValue(ColorHighProperty, value); }

    // ────────────────────────────────────────────────
    //  Rendering
    // ────────────────────────────────────────────────

    internal override void CollectRenderables(
        Projection3D projection,
        List<(Chart3DSeries series, int index, double depth, Action<DrawingContext> render)> renderables)
    {
        var xVals = XValues;
        var yVals = YValues;
        var zVals = ZValues;
        if (xVals == null || yVals == null || zVals == null) return;

        var count = Math.Min(xVals.Length, Math.Min(yVals.Length, zVals.Length));
        var halfW = BarWidth / 2;
        var halfD = BarDepth / 2;

        for (int i = 0; i < count; i++)
        {
            var x = xVals[i];
            var y = yVals[i];
            var z = zVals[i];

            // Get bar color
            var barBrush = GetBarBrush(i, y);

            // Define the 8 corners of the bar (bottom and top faces)
            var corners = new Point3D[]
            {
                new(x - halfW, 0, z - halfD),     // 0: bottom-front-left
                new(x + halfW, 0, z - halfD),     // 1: bottom-front-right
                new(x + halfW, 0, z + halfD),     // 2: bottom-back-right
                new(x - halfW, 0, z + halfD),     // 3: bottom-back-left
                new(x - halfW, y, z - halfD),     // 4: top-front-left
                new(x + halfW, y, z - halfD),     // 5: top-front-right
                new(x + halfW, y, z + halfD),     // 6: top-back-right
                new(x - halfW, y, z + halfD),     // 7: top-back-left
            };

            // Project all corners
            var screen = new Point[8];
            var center = new Point3D(x, y / 2, z);
            var depth = projection.GetDepth(center);

            for (int c = 0; c < 8; c++)
                screen[c] = projection.Project(corners[c]);

            // Capture for closure
            var idx = i;
            var s = screen;
            var brush = barBrush;
            var opacity = Opacity;

            renderables.Add((this, idx, depth, ctx =>
            {
                DrawBar(ctx, s, brush, opacity);
            }));
        }
    }

    /// <summary>
    /// Draw a 3D bar as three visible faces (top, front-left, front-right).
    /// The faces rendered depend on the viewing angle, but for simplicity
    /// we always draw all three visible faces.
    /// </summary>
    private static void DrawBar(DrawingContext ctx, Point[] s, IBrush brush, double opacity)
    {
        // Face indices: [bottom-left, bottom-right, top-right, top-left]
        // Top face: 4,5,6,7
        // Front face: 0,1,5,4
        // Right face: 1,2,6,5

        var topFace = new[] { s[4], s[5], s[6], s[7] };
        var frontFace = new[] { s[0], s[1], s[5], s[4] };
        var rightFace = new[] { s[1], s[2], s[6], s[5] };

        // Create shaded versions for 3D effect
        var topBrush = AdjustBrightness(brush, 1.1, opacity);
        var frontBrush = AdjustBrightness(brush, 0.9, opacity);
        var rightBrush = AdjustBrightness(brush, 0.7, opacity);

        // Draw each face as a polygon
        DrawQuad(ctx, topFace, topBrush);
        DrawQuad(ctx, frontFace, frontBrush);
        DrawQuad(ctx, rightFace, rightBrush);
    }

    /// <summary>Draw a quadrilateral polygon.</summary>
    private static void DrawQuad(DrawingContext ctx, Point[] points, IBrush fill)
    {
        if (points.Length < 4) return;

        var geometry = new StreamGeometry();
        using (var sgCtx = geometry.Open())
        {
            sgCtx.BeginFigure(points[0], true);
            sgCtx.LineTo(points[1]);
            sgCtx.LineTo(points[2]);
            sgCtx.LineTo(points[3]);
            sgCtx.EndFigure(true);
        }

        ctx.DrawGeometry(fill, new Pen(Brushes.White, 0.5, new DashStyle(null, 0)), geometry);
    }

    /// <summary>Get the brush for a bar based on its index and Y value.</summary>
    private IBrush GetBarBrush(int index, double yValue)
    {
        var colors = Colors;
        if (colors != null && index < colors.Length && colors[index] != null)
            return colors[index];

        // Gradient based on Y value
        var t = Math.Clamp(yValue, 0, 1);
        var r = (byte)(ColorLow.R + (ColorHigh.R - ColorLow.R) * t);
        var g = (byte)(ColorLow.G + (ColorHigh.G - ColorLow.G) * t);
        var b = (byte)(ColorLow.B + (ColorHigh.B - ColorLow.B) * t);
        return new SolidColorBrush(AvaloniaColor.FromRgb(r, g, b));
    }

    /// <summary>Adjust brush brightness for 3D face shading.</summary>
    private static IBrush AdjustBrightness(IBrush brush, double factor, double opacity)
    {
        if (brush is SolidColorBrush scb)
        {
            var c = scb.Color;
            var r = (byte)Math.Clamp(c.R * factor, 0, 255);
            var g = (byte)Math.Clamp(c.G * factor, 0, 255);
            var b = (byte)Math.Clamp(c.B * factor, 0, 255);
            return new SolidColorBrush(AvaloniaColor.FromRgb(r, g, b), opacity);
        }
        return brush;
    }

    // ────────────────────────────────────────────────
    //  Hit testing
    // ────────────────────────────────────────────────

    internal override (int index, double distance)? HitTest(Point screenPos, Projection3D projection)
    {
        var xVals = XValues;
        var yVals = YValues;
        var zVals = ZValues;
        if (xVals == null || yVals == null || zVals == null) return null;

        var count = Math.Min(xVals.Length, Math.Min(yVals.Length, zVals.Length));
        (int index, double distance)? best = null;

        for (int i = 0; i < count; i++)
        {
            // Project bar center (midpoint of bar height)
            var center = new Point3D(xVals[i], yVals[i] / 2, zVals[i]);
            var screenCenter = projection.Project(center);
            var diff = new Point(screenPos.X - screenCenter.X, screenPos.Y - screenCenter.Y);
            var dist = Math.Sqrt(diff.X * diff.X + diff.Y * diff.Y);

            if (dist < 20 && (!best.HasValue || dist < best.Value.distance))
            {
                best = (i, dist);
            }
        }

        return best;
    }

    internal override string? GetTooltipText(int index)
    {
        var xVals = XValues;
        var yVals = YValues;
        var zVals = ZValues;
        if (xVals == null || yVals == null || zVals == null || index < 0) return null;

        var count = Math.Min(xVals.Length, Math.Min(yVals.Length, zVals.Length));
        if (index >= count) return null;

        var name = Title ?? "Bar3D";
        return $"{name}\nX: {xVals[index]:F2}\nY: {yVals[index]:F2}\nZ: {zVals[index]:F2}";
    }
}
