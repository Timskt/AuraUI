using Avalonia;
using Avalonia.Media;

namespace AuraUI.Controls.Charts.Charts3D;

/// <summary>
/// Shape options for 3D scatter points.
/// </summary>
public enum PointShape3D
{
    Sphere,
    Cube,
    Diamond
}

/// <summary>
/// A 3D scatter plot series. Renders individual markers at 3D positions
/// with configurable size and color encoding.
///
/// Usage:
///   var series = new Scatter3DSeries();
///   series.XValues = xData;
///   series.YValues = yData;
///   series.ZValues = zData;
///   series.Sizes = sizeData;      // optional: maps to marker size
///   series.Colors = colorBrushes;  // optional: per-point colors
///
/// Rendering:
///   - Points are rendered as projected circles/shapes
///   - Size encoding: point sizes are mapped from the Sizes array
///   - Color encoding: per-point or gradient by value
///   - Depth-sorted for proper overlap
/// </summary>
public class Scatter3DSeries : Chart3DSeries
{
    /// <summary>X positions (normalized 0..1).</summary>
    public static readonly StyledProperty<double[]?> XValuesProperty =
        AvaloniaProperty.Register<Scatter3DSeries, double[]?>(nameof(XValues));

    /// <summary>Y positions (normalized 0..1).</summary>
    public static readonly StyledProperty<double[]?> YValuesProperty =
        AvaloniaProperty.Register<Scatter3DSeries, double[]?>(nameof(YValues));

    /// <summary>Z positions (normalized 0..1).</summary>
    public static readonly StyledProperty<double[]?> ZValuesProperty =
        AvaloniaProperty.Register<Scatter3DSeries, double[]?>(nameof(ZValues));

    /// <summary>Per-point size values. Mapped to marker size range.</summary>
    public static readonly StyledProperty<double[]?> SizesProperty =
        AvaloniaProperty.Register<Scatter3DSeries, double[]?>(nameof(Sizes));

    /// <summary>Per-point color overrides.</summary>
    public static readonly StyledProperty<IBrush[]?> ColorsProperty =
        AvaloniaProperty.Register<Scatter3DSeries, IBrush[]?>(nameof(Colors));

    /// <summary>Shape of the scatter points.</summary>
    public static readonly StyledProperty<PointShape3D> PointShapeProperty =
        AvaloniaProperty.Register<Scatter3DSeries, PointShape3D>(nameof(PointShape), PointShape3D.Sphere);

    /// <summary>Minimum marker size in pixels.</summary>
    public static readonly StyledProperty<double> MinSizeProperty =
        AvaloniaProperty.Register<Scatter3DSeries, double>(nameof(MinSize), 4.0);

    /// <summary>Maximum marker size in pixels.</summary>
    public static readonly StyledProperty<double> MaxSizeProperty =
        AvaloniaProperty.Register<Scatter3DSeries, double>(nameof(MaxSize), 24.0);

    /// <summary>Default marker size when no Sizes array is provided.</summary>
    public static readonly StyledProperty<double> DefaultSizeProperty =
        AvaloniaProperty.Register<Scatter3DSeries, double>(nameof(DefaultSize), 8.0);

    /// <summary>Base color for gradient mode (when Colors is null).</summary>
    public static readonly StyledProperty<Color> ColorLowProperty =
        AvaloniaProperty.Register<Scatter3DSeries, Color>(nameof(ColorLow), Color.Parse("#66BB6A"));

    /// <summary>Top color for gradient mode.</summary>
    public static readonly StyledProperty<Color> ColorHighProperty =
        AvaloniaProperty.Register<Scatter3DSeries, Color>(nameof(ColorHigh), Color.Parse("#C62828"));

    /// <summary>Fill opacity of markers.</summary>
    public static readonly StyledProperty<double> FillOpacityProperty =
        AvaloniaProperty.Register<Scatter3DSeries, double>(nameof(FillOpacity), 0.8);

    // CLR wrappers
    public double[]? XValues { get => GetValue(XValuesProperty); set => SetValue(XValuesProperty, value); }
    public double[]? YValues { get => GetValue(YValuesProperty); set => SetValue(YValuesProperty, value); }
    public double[]? ZValues { get => GetValue(ZValuesProperty); set => SetValue(ZValuesProperty, value); }
    public double[]? Sizes { get => GetValue(SizesProperty); set => SetValue(SizesProperty, value); }
    public IBrush[]? Colors { get => GetValue(ColorsProperty); set => SetValue(ColorsProperty, value); }
    public PointShape3D PointShape { get => GetValue(PointShapeProperty); set => SetValue(PointShapeProperty, value); }
    public double MinSize { get => GetValue(MinSizeProperty); set => SetValue(MinSizeProperty, value); }
    public double MaxSize { get => GetValue(MaxSizeProperty); set => SetValue(MaxSizeProperty, value); }
    public double DefaultSize { get => GetValue(DefaultSizeProperty); set => SetValue(DefaultSizeProperty, value); }
    public Color ColorLow { get => GetValue(ColorLowProperty); set => SetValue(ColorLowProperty, value); }
    public Color ColorHigh { get => GetValue(ColorHighProperty); set => SetValue(ColorHighProperty, value); }
    public double FillOpacity { get => GetValue(FillOpacityProperty); set => SetValue(FillOpacityProperty, value); }

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
        var sizes = Sizes;
        var colors = Colors;

        for (int i = 0; i < count; i++)
        {
            var point3d = new Point3D(xVals[i], yVals[i], zVals[i]);
            var screenPoint = projection.Project(point3d);
            var depth = projection.GetDepth(point3d);

            // Determine marker size
            double size;
            if (sizes != null && i < sizes.Length)
            {
                var minS = sizes.Min();
                var maxS = sizes.Max();
                var range = maxS - minS;
                var t = range > 0 ? (sizes[i] - minS) / range : 0.5;
                size = MinSize + t * (MaxSize - MinSize);
            }
            else
            {
                size = DefaultSize;
            }

            // Determine marker color
            IBrush brush;
            if (colors != null && i < colors.Length && colors[i] != null)
            {
                brush = colors[i];
            }
            else
            {
                var minV = yVals.Min();
                var maxV = yVals.Max();
                var range = maxV - minV;
                var t = range > 0 ? (yVals[i] - minV) / range : 0.5;
                var r = (byte)(ColorLow.R + (ColorHigh.R - ColorLow.R) * t);
                var g = (byte)(ColorLow.G + (ColorHigh.G - ColorLow.G) * t);
                var b = (byte)(ColorLow.B + (ColorHigh.B - ColorLow.B) * t);
                brush = new SolidColorBrush(Color.FromRgb(r, g, b), FillOpacity);
            }

            var idx = i;
            var sp = screenPoint;
            var s = size;
            var b2 = brush;
            var shape = PointShape;
            var opacity = Opacity;

            renderables.Add((this, idx, depth, ctx =>
            {
                DrawPoint(ctx, sp, s, b2, shape, opacity);
            }));
        }
    }

    /// <summary>Draw a single scatter point.</summary>
    private static void DrawPoint(DrawingContext ctx, Point screen, double size, IBrush brush,
        PointShape3D shape, double opacity)
    {
        var half = size / 2;
        var borderPen = new Pen(new SolidColorBrush(Colors.White, 0.6), 1.0);

        switch (shape)
        {
            case PointShape3D.Sphere:
                // Draw as ellipse with highlight
                ctx.DrawEllipse(brush, borderPen, screen, half, half);
                // Highlight dot
                var highlightBrush = new SolidColorBrush(Colors.White, 0.4 * opacity);
                ctx.DrawEllipse(highlightBrush, null,
                    new Point(screen.X - half * 0.3, screen.Y - half * 0.3),
                    half * 0.3, half * 0.3);
                break;

            case PointShape3D.Cube:
                // Draw as rectangle (projected cube is simplified to a square)
                var rect = new Rect(screen.X - half, screen.Y - half, size, size);
                ctx.DrawRectangle(brush, borderPen, rect, 2, 2);
                break;

            case PointShape3D.Diamond:
                // Draw as rotated square
                var diamond = new StreamGeometry();
                using (var dCtx = diamond.Open())
                {
                    dCtx.BeginFigure(new Point(screen.X, screen.Y - half), true);
                    dCtx.LineTo(new Point(screen.X + half, screen.Y));
                    dCtx.LineTo(new Point(screen.X, screen.Y + half));
                    dCtx.LineTo(new Point(screen.X - half, screen.Y));
                    dCtx.EndFigure(true);
                }
                ctx.DrawGeometry(brush, borderPen, diamond);
                break;
        }
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
            var point3d = new Point3D(xVals[i], yVals[i], zVals[i]);
            var screenPoint = projection.Project(point3d);
            var dist = (screenPos - screenPoint).Length;

            if (dist < 15 && (!best.HasValue || dist < best.Value.distance))
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

        var name = Title ?? "Scatter3D";
        var sizeInfo = Sizes != null && index < Sizes.Length ? $"\nSize: {Sizes[index]:F2}" : "";
        return $"{name}\nX: {xVals[index]:F2}\nY: {yVals[index]:F2}\nZ: {zVals[index]:F2}{sizeInfo}";
    }
}
