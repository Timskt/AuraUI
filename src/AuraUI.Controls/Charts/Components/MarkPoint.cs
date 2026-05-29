using Avalonia;
using Avalonia.Media;

namespace AuraUI.Controls.Charts.Components;

/// <summary>
/// ECharts-style mark point component. Renders markers at specific data points
/// to highlight min, max, average, or custom points.
///
/// Predefined types:
///   - <see cref="MarkType.Min"/>: marks the minimum value
///   - <see cref="MarkType.Max"/>: marks the maximum value
///   - <see cref="MarkType.Average"/>: marks the average value
///
/// Rendering:
///   - Symbol drawn at the data point position
///   - Optional label showing the value
///   - Customizable color, size, and symbol shape
/// </summary>
public class MarkPointComponent : AvaloniaObject
{
    /// <summary>Whether mark points are visible.</summary>
    public static readonly StyledProperty<bool> IsVisibleProperty =
        AvaloniaProperty.Register<MarkPointComponent, bool>(nameof(IsVisible), true);

    /// <summary>Symbol shape for the mark points.</summary>
    public static readonly StyledProperty<MarkerShape> SymbolProperty =
        AvaloniaProperty.Register<MarkPointComponent, MarkerShape>(nameof(Symbol), MarkerShape.Circle);

    /// <summary>Symbol size in pixels.</summary>
    public static readonly StyledProperty<double> SymbolSizeProperty =
        AvaloniaProperty.Register<MarkPointComponent, double>(nameof(SymbolSize), 8);

    /// <summary>Whether to show labels on the mark points.</summary>
    public static readonly StyledProperty<bool> ShowLabelProperty =
        AvaloniaProperty.Register<MarkPointComponent, bool>(nameof(ShowLabel), true);

    /// <summary>Font size for mark point labels.</summary>
    public static readonly StyledProperty<double> LabelFontSizeProperty =
        AvaloniaProperty.Register<MarkPointComponent, double>(nameof(LabelFontSize), 10);

    /// <summary>Label text color.</summary>
    public static readonly StyledProperty<IBrush?> LabelColorProperty =
        AvaloniaProperty.Register<MarkPointComponent, IBrush?>(nameof(LabelColor));

    /// <summary>Default color for mark point symbols.</summary>
    public static readonly StyledProperty<IBrush?> ColorProperty =
        AvaloniaProperty.Register<MarkPointComponent, IBrush?>(nameof(Color));

    /// <summary>Label position relative to the symbol.</summary>
    public static readonly StyledProperty<TooltipPosition> LabelPositionProperty =
        AvaloniaProperty.Register<MarkPointComponent, TooltipPosition>(nameof(LabelPosition), TooltipPosition.Top);

    // CLR wrappers
    public bool IsVisible { get => GetValue(IsVisibleProperty); set => SetValue(IsVisibleProperty, value); }
    public MarkerShape Symbol { get => GetValue(SymbolProperty); set => SetValue(SymbolProperty, value); }
    public double SymbolSize { get => GetValue(SymbolSizeProperty); set => SetValue(SymbolSizeProperty, value); }
    public bool ShowLabel { get => GetValue(ShowLabelProperty); set => SetValue(ShowLabelProperty, value); }
    public double LabelFontSize { get => GetValue(LabelFontSizeProperty); set => SetValue(LabelFontSizeProperty, value); }
    public IBrush? LabelColor { get => GetValue(LabelColorProperty); set => SetValue(LabelColorProperty, value); }
    public IBrush? Color { get => GetValue(ColorProperty); set => SetValue(ColorProperty, value); }
    public TooltipPosition LabelPosition { get => GetValue(LabelPositionProperty); set => SetValue(LabelPositionProperty, value); }

    /// <summary>
    /// Data points to mark. Each entry specifies the position, type, and optional overrides.
    /// </summary>
    public List<MarkPointData> Data { get; set; } = new();

    /// <summary>
    /// Render all mark points.
    /// </summary>
    /// <param name="context">Drawing context.</param>
    /// <param name="plotArea">Chart plot area rect.</param>
    /// <param name="xAxis">X axis for coordinate conversion.</param>
    /// <param name="yAxis">Y axis for coordinate conversion.</param>
    /// <param name="seriesDataPoints">Series data points for computing min/max/average.</param>
    /// <param name="seriesColor">Color of the parent series.</param>
    public void Render(
        DrawingContext context,
        Rect plotArea,
        ChartAxis xAxis,
        ChartAxis yAxis,
        IReadOnlyList<ChartDataPoint> seriesDataPoints,
        IBrush? seriesColor)
    {
        if (!IsVisible) return;

        var resolvedPoints = ResolveDataPoints(seriesDataPoints);
        var markColor = Color ?? seriesColor ?? Brushes.Red;
        var labelBrush = LabelColor ?? Brushes.DarkGray;
        var fontFamily = "Segoe UI";

        foreach (var mp in resolvedPoints)
        {
            var pixelX = xAxis.ValueToPixel(mp.X);
            var pixelY = yAxis.ValueToPixel(mp.Y);

            // Skip if outside plot area
            if (pixelX < plotArea.Left || pixelX > plotArea.Right ||
                pixelY < plotArea.Top || pixelY > plotArea.Bottom)
                continue;

            var point = new Point(pixelX, pixelY);
            var size = mp.Size > 0 ? mp.Size : SymbolSize;
            var pointColor = mp.Color ?? markColor;
            var shape = mp.Symbol ?? Symbol;

            // Draw symbol
            DrawSymbol(context, point, size, shape, pointColor);

            // Draw label
            if (ShowLabel)
            {
                var text = mp.Label ?? mp.Y.ToString("F2") ?? "";
                if (string.IsNullOrEmpty(text)) continue;

                var formatted = new FormattedText(
                    text,
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface(fontFamily),
                    LabelFontSize,
                    labelBrush);

                var labelPos = ComputeLabelPosition(point, size, formatted.Width, formatted.Height, LabelPosition);
                context.DrawText(formatted, labelPos);
            }
        }
    }

    /// <summary>
    /// Resolve predefined types (min, max, average) into concrete data points.
    /// </summary>
    private List<MarkPointData> ResolveDataPoints(IReadOnlyList<ChartDataPoint> seriesData)
    {
        var result = new List<MarkPointData>();

        foreach (var mp in Data)
        {
            if (mp.Type.HasValue)
            {
                var resolved = ResolvePredefinedType(mp.Type.Value, seriesData);
                if (resolved != null)
                {
                    resolved.Label = mp.Label ?? resolved.Label;
                    resolved.Color = mp.Color ?? resolved.Color;
                    result.Add(resolved);
                }
            }
            else
            {
                result.Add(mp);
            }
        }

        return result;
    }

    private static MarkPointData? ResolvePredefinedType(MarkType type, IReadOnlyList<ChartDataPoint> data)
    {
        if (data.Count == 0) return null;

        switch (type)
        {
            case MarkType.Min:
                var minPt = data[0];
                foreach (var pt in data)
                    if (pt.Y < minPt.Y) minPt = pt;
                return new MarkPointData { X = minPt.X, Y = minPt.Y, Label = $"Min: {minPt.Y:F2}" };

            case MarkType.Max:
                var maxPt = data[0];
                foreach (var pt in data)
                    if (pt.Y > maxPt.Y) maxPt = pt;
                return new MarkPointData { X = maxPt.X, Y = maxPt.Y, Label = $"Max: {maxPt.Y:F2}" };

            case MarkType.Average:
                var sum = 0.0;
                foreach (var pt in data) sum += pt.Y;
                var avg = sum / data.Count;
                // Place at the middle X
                var midX = data[data.Count / 2].X;
                return new MarkPointData { X = midX, Y = avg, Label = $"Avg: {avg:F2}" };

            default:
                return null;
        }
    }

    private static void DrawSymbol(DrawingContext context, Point center, double size, MarkerShape shape, IBrush color)
    {
        var halfSize = size / 2;

        switch (shape)
        {
            case MarkerShape.Circle:
                context.DrawEllipse(color, null, center, halfSize, halfSize);
                break;
            case MarkerShape.Square:
                var sqRect = new Rect(center.X - halfSize, center.Y - halfSize, size, size);
                context.DrawRectangle(color, null, sqRect);
                break;
            case MarkerShape.Diamond:
            {
                var diamond = new StreamGeometry();
                using (var sgCtx = diamond.Open())
                {
                    sgCtx.BeginFigure(new Point(center.X, center.Y - halfSize), true);
                    sgCtx.LineTo(new Point(center.X + halfSize, center.Y));
                    sgCtx.LineTo(new Point(center.X, center.Y + halfSize));
                    sgCtx.LineTo(new Point(center.X - halfSize, center.Y));
                    sgCtx.EndFigure(true);
                }
                context.DrawGeometry(color, null, diamond);
                break;
            }
            case MarkerShape.Triangle:
            {
                var tri = new StreamGeometry();
                using (var sgCtx = tri.Open())
                {
                    sgCtx.BeginFigure(new Point(center.X, center.Y - halfSize), true);
                    sgCtx.LineTo(new Point(center.X + halfSize, center.Y + halfSize));
                    sgCtx.LineTo(new Point(center.X - halfSize, center.Y + halfSize));
                    sgCtx.EndFigure(true);
                }
                context.DrawGeometry(color, null, tri);
                break;
            }
            default:
                context.DrawEllipse(color, null, center, halfSize, halfSize);
                break;
        }
    }

    private static Point ComputeLabelPosition(Point symbolCenter, double symbolSize, double textWidth, double textHeight, TooltipPosition position)
    {
        var offset = symbolSize / 2 + 4;
        return position switch
        {
            TooltipPosition.Top => new Point(symbolCenter.X - textWidth / 2, symbolCenter.Y - offset - textHeight),
            TooltipPosition.Bottom => new Point(symbolCenter.X - textWidth / 2, symbolCenter.Y + offset),
            TooltipPosition.Left => new Point(symbolCenter.X - offset - textWidth, symbolCenter.Y - textHeight / 2),
            TooltipPosition.Right => new Point(symbolCenter.X + offset, symbolCenter.Y - textHeight / 2),
            _ => new Point(symbolCenter.X - textWidth / 2, symbolCenter.Y - offset - textHeight)
        };
    }
}

/// <summary>
/// A single mark point data entry.
/// </summary>
public class MarkPointData
{
    /// <summary>Predefined type (min, max, average). When set, X/Y are computed from data.</summary>
    public MarkType? Type { get; set; }

    /// <summary>X value for custom mark points.</summary>
    public double X { get; set; }

    /// <summary>Y value for custom mark points.</summary>
    public double Y { get; set; }

    /// <summary>Label text override.</summary>
    public string? Label { get; set; }

    /// <summary>Color override for this mark point.</summary>
    public IBrush? Color { get; set; }

    /// <summary>Symbol shape override.</summary>
    public MarkerShape? Symbol { get; set; }

    /// <summary>Size override for this mark point.</summary>
    public double Size { get; set; }
}
