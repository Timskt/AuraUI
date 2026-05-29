using Avalonia;
using Avalonia.Media;

namespace AuraUI.Controls.Charts.Components;

/// <summary>
/// ECharts-style mark area component. Renders highlighted rectangular regions
/// to emphasize ranges of data (e.g., confidence bands, target zones, warning areas).
///
/// Rendering:
///   - Semi-transparent filled rectangles
///   - Optional border and label
///   - Can highlight horizontal ranges (Y band), vertical ranges (X band), or rectangular areas
/// </summary>
public class MarkAreaComponent : AvaloniaObject
{
    /// <summary>Whether mark areas are visible.</summary>
    public static readonly StyledProperty<bool> IsVisibleProperty =
        AvaloniaProperty.Register<MarkAreaComponent, bool>(nameof(IsVisible), true);

    /// <summary>Whether to show labels on the mark areas.</summary>
    public static readonly StyledProperty<bool> ShowLabelProperty =
        AvaloniaProperty.Register<MarkAreaComponent, bool>(nameof(ShowLabel), true);

    /// <summary>Font size for the mark area labels.</summary>
    public static readonly StyledProperty<double> LabelFontSizeProperty =
        AvaloniaProperty.Register<MarkAreaComponent, double>(nameof(LabelFontSize), 10);

    /// <summary>Label text color.</summary>
    public static readonly StyledProperty<IBrush?> LabelColorProperty =
        AvaloniaProperty.Register<MarkAreaComponent, IBrush?>(nameof(LabelColor));

    /// <summary>Default fill color for the mark areas.</summary>
    public static readonly StyledProperty<IBrush?> FillColorProperty =
        AvaloniaProperty.Register<MarkAreaComponent, IBrush?>(nameof(FillColor));

    /// <summary>Default opacity for the fill.</summary>
    public static readonly StyledProperty<double> OpacityProperty =
        AvaloniaProperty.Register<MarkAreaComponent, double>(nameof(Opacity), 0.15);

    /// <summary>Border brush for the mark areas.</summary>
    public static readonly StyledProperty<IBrush?> BorderBrushProperty =
        AvaloniaProperty.Register<MarkAreaComponent, IBrush?>(nameof(BorderBrush));

    /// <summary>Border thickness.</summary>
    public static readonly StyledProperty<double> BorderThicknessProperty =
        AvaloniaProperty.Register<MarkAreaComponent, double>(nameof(BorderThickness), 1.0);

    /// <summary>Label position within the area.</summary>
    public static readonly StyledProperty<TooltipPosition> LabelPositionProperty =
        AvaloniaProperty.Register<MarkAreaComponent, TooltipPosition>(nameof(LabelPosition), TooltipPosition.Inside);

    // CLR wrappers
    public bool IsVisible { get => GetValue(IsVisibleProperty); set => SetValue(IsVisibleProperty, value); }
    public bool ShowLabel { get => GetValue(ShowLabelProperty); set => SetValue(ShowLabelProperty, value); }
    public double LabelFontSize { get => GetValue(LabelFontSizeProperty); set => SetValue(LabelFontSizeProperty, value); }
    public IBrush? LabelColor { get => GetValue(LabelColorProperty); set => SetValue(LabelColorProperty, value); }
    public IBrush? FillColor { get => GetValue(FillColorProperty); set => SetValue(FillColorProperty, value); }
    public double Opacity { get => GetValue(OpacityProperty); set => SetValue(OpacityProperty, value); }
    public IBrush? BorderBrush { get => GetValue(BorderBrushProperty); set => SetValue(BorderBrushProperty, value); }
    public double BorderThickness { get => GetValue(BorderThicknessProperty); set => SetValue(BorderThicknessProperty, value); }
    public TooltipPosition LabelPosition { get => GetValue(LabelPositionProperty); set => SetValue(LabelPositionProperty, value); }

    /// <summary>
    /// Data areas to render. Each entry defines a rectangular region.
    /// </summary>
    public List<MarkAreaData> Data { get; set; } = new();

    /// <summary>
    /// Render all mark areas.
    /// </summary>
    /// <param name="context">Drawing context.</param>
    /// <param name="plotArea">Chart plot area rect.</param>
    /// <param name="xAxis">X axis for coordinate conversion.</param>
    /// <param name="yAxis">Y axis for coordinate conversion.</param>
    public void Render(
        DrawingContext context,
        Rect plotArea,
        ChartAxis xAxis,
        ChartAxis yAxis)
    {
        if (!IsVisible) return;

        var fontFamily = "Segoe UI";

        foreach (var ma in Data)
        {
            // Convert data coordinates to pixel coordinates
            double pixelX1, pixelY1, pixelX2, pixelY2;

            if (ma.IsHorizontalBand)
            {
                // Horizontal band: full X range, Y from Value to Value2
                pixelX1 = plotArea.Left;
                pixelX2 = plotArea.Right;
                pixelY1 = yAxis.ValueToPixel(ma.Value);
                pixelY2 = yAxis.ValueToPixel(ma.Value2);
            }
            else if (ma.IsVerticalBand)
            {
                // Vertical band: full Y range, X from Value to Value2
                pixelX1 = xAxis.ValueToPixel(ma.Value);
                pixelX2 = xAxis.ValueToPixel(ma.Value2);
                pixelY1 = plotArea.Top;
                pixelY2 = plotArea.Bottom;
            }
            else
            {
                // Rectangular area
                pixelX1 = xAxis.ValueToPixel(ma.X1);
                pixelY1 = yAxis.ValueToPixel(ma.Y1);
                pixelX2 = xAxis.ValueToPixel(ma.X2);
                pixelY2 = yAxis.ValueToPixel(ma.Y2);
            }

            // Normalize to left/top/width/height
            var left = Math.Min(pixelX1, pixelX2);
            var top = Math.Min(pixelY1, pixelY2);
            var width = Math.Abs(pixelX2 - pixelX1);
            var height = Math.Abs(pixelY2 - pixelY1);

            // Clamp to plot area
            left = Math.Max(left, plotArea.Left);
            top = Math.Max(top, plotArea.Top);
            width = Math.Min(width, plotArea.Right - left);
            height = Math.Min(height, plotArea.Bottom - top);

            if (width <= 0 || height <= 0) continue;

            var areaRect = new Rect(left, top, width, height);

            // Fill
            var fillColor = ma.Color ?? FillColor ?? new SolidColorBrush(Colors.Orange, Opacity);
            var pen = BorderBrush != null ? new Pen(BorderBrush, BorderThickness) : null;
            context.DrawRectangle(fillColor, pen, areaRect);

            // Label
            if (ShowLabel && !string.IsNullOrEmpty(ma.Label))
            {
                var labelBrush = LabelColor ?? Brushes.DarkGray;
                var formatted = new FormattedText(
                    ma.Label,
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface(fontFamily),
                    LabelFontSize,
                    labelBrush);

                double labelX, labelY;
                switch (LabelPosition)
                {
                    case TooltipPosition.Top:
                        labelX = left + (width - formatted.Width) / 2;
                        labelY = top + 4;
                        break;
                    case TooltipPosition.Bottom:
                        labelX = left + (width - formatted.Width) / 2;
                        labelY = top + height - formatted.Height - 4;
                        break;
                    case TooltipPosition.Left:
                        labelX = left + 4;
                        labelY = top + (height - formatted.Height) / 2;
                        break;
                    case TooltipPosition.Right:
                        labelX = left + width - formatted.Width - 4;
                        labelY = top + (height - formatted.Height) / 2;
                        break;
                    default: // Inside
                        labelX = left + (width - formatted.Width) / 2;
                        labelY = top + (height - formatted.Height) / 2;
                        break;
                }

                // Clamp label to area
                labelX = Math.Clamp(labelX, left + 2, left + width - formatted.Width - 2);
                labelY = Math.Clamp(labelY, top + 2, top + height - formatted.Height - 2);

                context.DrawText(formatted, new Point(labelX, labelY));
            }
        }
    }
}

/// <summary>
/// A single mark area data entry.
/// </summary>
public class MarkAreaData
{
    /// <summary>Primary value (Y for horizontal band, X for vertical band, or X1 for rect).</summary>
    public double Value { get; set; }

    /// <summary>Secondary value (Y2 for horizontal band, X2 for vertical band, or Y1 for rect).</summary>
    public double Value2 { get; set; }

    /// <summary>X1 coordinate for rectangular areas.</summary>
    public double X1 { get; set; }

    /// <summary>Y1 coordinate for rectangular areas.</summary>
    public double Y1 { get; set; }

    /// <summary>X2 coordinate for rectangular areas.</summary>
    public double X2 { get; set; }

    /// <summary>Y2 coordinate for rectangular areas.</summary>
    public double Y2 { get; set; }

    /// <summary>Whether this is a horizontal band (full X range, Y between Value and Value2).</summary>
    public bool IsHorizontalBand { get; set; } = true;

    /// <summary>Whether this is a vertical band (full Y range, X between Value and Value2).</summary>
    public bool IsVerticalBand { get; set; }

    /// <summary>Label text for the area.</summary>
    public string? Label { get; set; }

    /// <summary>Fill color override.</summary>
    public IBrush? Color { get; set; }

    /// <summary>Opacity override for the fill.</summary>
    public double Opacity { get; set; } = -1; // -1 means use default
}
