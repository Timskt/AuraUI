using Avalonia;
using Avalonia.Media;

namespace AuraUI.Controls.Charts.Components;

/// <summary>
/// ECharts-style mark line component. Renders reference lines at specific values
/// to highlight thresholds, averages, or other important values.
///
/// Predefined types:
///   - <see cref="MarkType.Average"/>: line at the average value
///   - <see cref="MarkType.Min"/>: line at the minimum value
///   - <see cref="MarkType.Max"/>: line at the maximum value
///
/// Rendering:
///   - Horizontal, vertical, or diagonal lines drawn via DrawingContext
///   - Optional label at the midpoint of the line
///   - Configurable line style (solid, dashed, dotted)
/// </summary>
public class MarkLineComponent : AvaloniaObject
{
    /// <summary>Whether mark lines are visible.</summary>
    public static readonly StyledProperty<bool> IsVisibleProperty =
        AvaloniaProperty.Register<MarkLineComponent, bool>(nameof(IsVisible), true);

    /// <summary>Whether to show labels on the mark lines.</summary>
    public static readonly StyledProperty<bool> ShowLabelProperty =
        AvaloniaProperty.Register<MarkLineComponent, bool>(nameof(ShowLabel), true);

    /// <summary>Font size for the mark line labels.</summary>
    public static readonly StyledProperty<double> LabelFontSizeProperty =
        AvaloniaProperty.Register<MarkLineComponent, double>(nameof(LabelFontSize), 10);

    /// <summary>Label text color.</summary>
    public static readonly StyledProperty<IBrush?> LabelColorProperty =
        AvaloniaProperty.Register<MarkLineComponent, IBrush?>(nameof(LabelColor));

    /// <summary>Default line color.</summary>
    public static readonly StyledProperty<IBrush?> LineColorProperty =
        AvaloniaProperty.Register<MarkLineComponent, IBrush?>(nameof(LineColor));

    /// <summary>Default line thickness.</summary>
    public static readonly StyledProperty<double> LineThicknessProperty =
        AvaloniaProperty.Register<MarkLineComponent, double>(nameof(LineThickness), 1.0);

    /// <summary>Label position along the line (0 = start, 0.5 = middle, 1 = end).</summary>
    public static readonly StyledProperty<double> LabelPositionProperty =
        AvaloniaProperty.Register<MarkLineComponent, double>(nameof(LabelPosition), 0.5);

    // CLR wrappers
    public bool IsVisible { get => GetValue(IsVisibleProperty); set => SetValue(IsVisibleProperty, value); }
    public bool ShowLabel { get => GetValue(ShowLabelProperty); set => SetValue(ShowLabelProperty, value); }
    public double LabelFontSize { get => GetValue(LabelFontSizeProperty); set => SetValue(LabelFontSizeProperty, value); }
    public IBrush? LabelColor { get => GetValue(LabelColorProperty); set => SetValue(LabelColorProperty, value); }
    public IBrush? LineColor { get => GetValue(LineColorProperty); set => SetValue(LineColorProperty, value); }
    public double LineThickness { get => GetValue(LineThicknessProperty); set => SetValue(LineThicknessProperty, value); }
    public double LabelPosition { get => GetValue(LabelPositionProperty); set => SetValue(LabelPositionProperty, value); }

    /// <summary>
    /// Data lines to render. Each entry defines the line position and style.
    /// </summary>
    public List<MarkLineData> Data { get; set; } = new();

    /// <summary>
    /// Render all mark lines.
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

        var resolvedLines = ResolveDataLines(seriesDataPoints);
        var defaultColor = LineColor ?? seriesColor ?? Brushes.Red;
        var labelBrush = LabelColor ?? Brushes.DarkGray;
        var fontFamily = "Segoe UI";

        foreach (var ml in resolvedLines)
        {
            var color = ml.Color ?? defaultColor;
            var thickness = ml.Thickness > 0 ? ml.Thickness : LineThickness;

            Pen pen;
            if (ml.DashStyle != null && ml.DashStyle.Length > 0)
            {
                pen = new Pen(color, thickness, new DashStyle(ml.DashStyle, 0));
            }
            else
            {
                pen = new Pen(color, thickness);
            }

            Point start, end;
            if (ml.IsHorizontal)
            {
                var y = yAxis.ValueToPixel(ml.Value);
                start = new Point(plotArea.Left, y);
                end = new Point(plotArea.Right, y);
            }
            else if (ml.IsVertical)
            {
                var x = xAxis.ValueToPixel(ml.Value);
                start = new Point(x, plotArea.Top);
                end = new Point(x, plotArea.Bottom);
            }
            else
            {
                // Diagonal line from (X1, Y1) to (X2, Y2)
                var x1 = xAxis.ValueToPixel(ml.X1 ?? ml.Value);
                var y1 = yAxis.ValueToPixel(ml.Y1 ?? ml.Value);
                var x2 = xAxis.ValueToPixel(ml.X2 ?? ml.Value2 ?? ml.Value);
                var y2 = yAxis.ValueToPixel(ml.Y2 ?? ml.Value2 ?? ml.Value);
                start = new Point(x1, y1);
                end = new Point(x2, y2);
            }

            // Draw line
            context.DrawLine(pen, start, end);

            // Draw label
            if (ShowLabel)
            {
                var label = ml.Label ?? (ml.IsHorizontal ? ml.Value.ToString("F2") : ml.Value.ToString("F2"));
                if (string.IsNullOrEmpty(label)) continue;

                var formatted = new FormattedText(
                    label,
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface(fontFamily),
                    LabelFontSize,
                    labelBrush);

                // Position label along the line
                var t = LabelPosition;
                var labelX = start.X + (end.X - start.X) * t;
                var labelY = start.Y + (end.Y - start.Y) * t;

                // Offset label slightly above the line
                labelY -= formatted.Height + 4;

                // Clamp to plot area
                labelX = Math.Clamp(labelX, plotArea.Left, plotArea.Right - formatted.Width);
                labelY = Math.Clamp(labelY, plotArea.Top, plotArea.Bottom - formatted.Height);

                context.DrawText(formatted, new Point(labelX, labelY));
            }
        }
    }

    /// <summary>
    /// Resolve predefined types into concrete line data.
    /// </summary>
    private List<MarkLineData> ResolveDataLines(IReadOnlyList<ChartDataPoint> seriesData)
    {
        var result = new List<MarkLineData>();

        foreach (var ml in Data)
        {
            if (ml.Type.HasValue)
            {
                var resolved = ResolvePredefinedType(ml.Type.Value, seriesData);
                if (resolved != null)
                {
                    resolved.Label = ml.Label ?? resolved.Label;
                    resolved.Color = ml.Color ?? resolved.Color;
                    resolved.DashStyle = ml.DashStyle ?? resolved.DashStyle;
                    resolved.Thickness = ml.Thickness > 0 ? ml.Thickness : resolved.Thickness;
                    result.Add(resolved);
                }
            }
            else
            {
                result.Add(ml);
            }
        }

        return result;
    }

    private static MarkLineData? ResolvePredefinedType(MarkType type, IReadOnlyList<ChartDataPoint> data)
    {
        if (data.Count == 0) return null;

        switch (type)
        {
            case MarkType.Min:
                var minY = double.MaxValue;
                foreach (var pt in data)
                    if (pt.Y < minY) minY = pt.Y;
                return new MarkLineData { Value = minY, IsHorizontal = true, Label = $"Min: {minY:F2}" };

            case MarkType.Max:
                var maxY = double.MinValue;
                foreach (var pt in data)
                    if (pt.Y > maxY) maxY = pt.Y;
                return new MarkLineData { Value = maxY, IsHorizontal = true, Label = $"Max: {maxY:F2}" };

            case MarkType.Average:
                var sum = 0.0;
                foreach (var pt in data) sum += pt.Y;
                var avg = sum / data.Count;
                return new MarkLineData { Value = avg, IsHorizontal = true, Label = $"Avg: {avg:F2}" };

            default:
                return null;
        }
    }
}

/// <summary>
/// A single mark line data entry.
/// </summary>
public class MarkLineData
{
    /// <summary>Predefined type (min, max, average). When set, Value is computed from data.</summary>
    public MarkType? Type { get; set; }

    /// <summary>Primary value for the line (Y value for horizontal, X value for vertical).</summary>
    public double Value { get; set; }

    /// <summary>Secondary value for range or diagonal lines.</summary>
    public double? Value2 { get; set; }

    /// <summary>X1 coordinate for diagonal lines.</summary>
    public double? X1 { get; set; }

    /// <summary>Y1 coordinate for diagonal lines.</summary>
    public double? Y1 { get; set; }

    /// <summary>X2 coordinate for diagonal lines.</summary>
    public double? X2 { get; set; }

    /// <summary>Y2 coordinate for diagonal lines.</summary>
    public double? Y2 { get; set; }

    /// <summary>Whether this is a horizontal line (constant Y).</summary>
    public bool IsHorizontal { get; set; } = true;

    /// <summary>Whether this is a vertical line (constant X).</summary>
    public bool IsVertical { get; set; }

    /// <summary>Label text override.</summary>
    public string? Label { get; set; }

    /// <summary>Line color override.</summary>
    public IBrush? Color { get; set; }

    /// <summary>Dash pattern for the line (null = solid).</summary>
    public double[]? DashStyle { get; set; }

    /// <summary>Line thickness override.</summary>
    public double Thickness { get; set; }
}
