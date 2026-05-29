using Avalonia;
using Avalonia.Media;

namespace AuraUI.Controls.Charts.Components;

/// <summary>
/// ECharts-style axis pointer component. Renders a crosshair or shadow indicator
/// at the cursor position to help users identify data values along axes.
///
/// Supports three modes:
///   - <see cref="AxisPointerType.Line"/>: thin crosshair line at cursor
///   - <see cref="AxisPointerType.Shadow"/>: shaded band at the nearest category
///   - <see cref="AxisPointerType.Cross"/>: both X and Y crosshair lines
///
/// The axis pointer snaps to the nearest data point when <see cref="Snap"/> is true.
///
/// Rendering:
///   - Lines drawn as dashed or solid strokes via DrawingContext
///   - Shadow drawn as a semi-transparent rectangle
///   - Label drawn at the axis intersection with configurable formatter
/// </summary>
public class AxisPointer : AvaloniaObject
{
    /// <summary>The type of axis pointer (Line, Shadow, Cross).</summary>
    public static readonly StyledProperty<AxisPointerType> TypeProperty =
        AvaloniaProperty.Register<AxisPointer, AxisPointerType>(nameof(Type), AxisPointerType.Line);

    /// <summary>
    /// Whether the pointer snaps to the nearest data point.
    /// When false, the pointer follows the cursor exactly.
    /// </summary>
    public static readonly StyledProperty<bool> SnapProperty =
        AvaloniaProperty.Register<AxisPointer, bool>(nameof(Snap));

    /// <summary>Show a label at the axis intersection point.</summary>
    public static readonly StyledProperty<bool> ShowLabelProperty =
        AvaloniaProperty.Register<AxisPointer, bool>(nameof(ShowLabel), true);

    /// <summary>Brush for the pointer line(s).</summary>
    public static readonly StyledProperty<IBrush?> LineBrushProperty =
        AvaloniaProperty.Register<AxisPointer, IBrush?>(nameof(LineBrush));

    /// <summary>Thickness of the pointer line(s) in pixels.</summary>
    public static readonly StyledProperty<double> LineThicknessProperty =
        AvaloniaProperty.Register<AxisPointer, double>(nameof(LineThickness), 1.0);

    /// <summary>Whether to use a dashed line style.</summary>
    public static readonly StyledProperty<bool> DashedLineProperty =
        AvaloniaProperty.Register<AxisPointer, bool>(nameof(DashedLine), true);

    /// <summary>Brush for the shadow area (Shadow type only).</summary>
    public static readonly StyledProperty<IBrush?> ShadowBrushProperty =
        AvaloniaProperty.Register<AxisPointer, IBrush?>(nameof(ShadowBrush));

    /// <summary>Font size for the axis pointer label.</summary>
    public static readonly StyledProperty<double> LabelFontSizeProperty =
        AvaloniaProperty.Register<AxisPointer, double>(nameof(LabelFontSize), 11.0);

    /// <summary>Text color for the axis pointer label.</summary>
    public static readonly StyledProperty<IBrush?> LabelColorProperty =
        AvaloniaProperty.Register<AxisPointer, IBrush?>(nameof(LabelColor));

    /// <summary>Background brush for the label.</summary>
    public static readonly StyledProperty<IBrush?> LabelBackgroundProperty =
        AvaloniaProperty.Register<AxisPointer, IBrush?>(nameof(LabelBackground));

    /// <summary>Font family for the label text.</summary>
    public static readonly StyledProperty<string?> LabelFontFamilyProperty =
        AvaloniaProperty.Register<AxisPointer, string?>(nameof(LabelFontFamily));

    // CLR wrappers
    public AxisPointerType Type { get => GetValue(TypeProperty); set => SetValue(TypeProperty, value); }
    public bool Snap { get => GetValue(SnapProperty); set => SetValue(SnapProperty, value); }
    public bool ShowLabel { get => GetValue(ShowLabelProperty); set => SetValue(ShowLabelProperty, value); }
    public IBrush? LineBrush { get => GetValue(LineBrushProperty); set => SetValue(LineBrushProperty, value); }
    public double LineThickness { get => GetValue(LineThicknessProperty); set => SetValue(LineThicknessProperty, value); }
    public bool DashedLine { get => GetValue(DashedLineProperty); set => SetValue(DashedLineProperty, value); }
    public IBrush? ShadowBrush { get => GetValue(ShadowBrushProperty); set => SetValue(ShadowBrushProperty, value); }
    public double LabelFontSize { get => GetValue(LabelFontSizeProperty); set => SetValue(LabelFontSizeProperty, value); }
    public IBrush? LabelColor { get => GetValue(LabelColorProperty); set => SetValue(LabelColorProperty, value); }
    public IBrush? LabelBackground { get => GetValue(LabelBackgroundProperty); set => SetValue(LabelBackgroundProperty, value); }
    public string? LabelFontFamily { get => GetValue(LabelFontFamilyProperty); set => SetValue(LabelFontFamilyProperty, value); }

    /// <summary>Custom label formatter. Receives the axis value and returns display text.</summary>
    public Func<double, string>? LabelFormatter { get; set; }

    /// <summary>
    /// Render the axis pointer indicator.
    /// </summary>
    /// <param name="context">Drawing context.</param>
    /// <param name="plotArea">Chart plot area rect.</param>
    /// <param name="position">Current cursor position.</param>
    /// <param name="xAxis">X axis for value conversion (null for non-XY charts).</param>
    /// <param name="yAxis">Y axis for value conversion.</param>
    /// <param name="snappedPosition">Optional snapped data point position (when Snap is true).</param>
    public void Render(
        DrawingContext context,
        Rect plotArea,
        Point position,
        ChartAxis? xAxis,
        ChartAxis? yAxis,
        Point? snappedPosition = null)
    {
        var drawPos = snappedPosition ?? position;

        // Clamp to plot area
        var cx = Math.Clamp(drawPos.X, plotArea.Left, plotArea.Right);
        var cy = Math.Clamp(drawPos.Y, plotArea.Top, plotArea.Bottom);

        var lineBrush = LineBrush ?? new SolidColorBrush(Colors.Gray, 0.4);
        Pen linePen;
        if (DashedLine)
        {
            linePen = new Pen(lineBrush, LineThickness, new DashStyle(new double[] { 4, 2 }, 0));
        }
        else
        {
            linePen = new Pen(lineBrush, LineThickness);
        }

        switch (Type)
        {
            case AxisPointerType.Line:
                // Vertical crosshair line
                context.DrawLine(linePen,
                    new Point(cx, plotArea.Top),
                    new Point(cx, plotArea.Bottom));

                // Label on X axis
                if (ShowLabel && xAxis != null)
                {
                    var value = xAxis.PixelToValue(cx);
                    DrawAxisLabel(context, plotArea, cx, plotArea.Bottom, value, xAxis, true);
                }
                break;

            case AxisPointerType.Shadow:
                // Shaded band centered on the cursor X position
                var shadowBrush = ShadowBrush ?? new SolidColorBrush(Colors.Gray, 0.08);
                var bandWidth = plotArea.Width / Math.Max(1, xAxis?.ComputedTicks.Length ?? 10);
                var shadowLeft = Math.Clamp(cx - bandWidth / 2, plotArea.Left, plotArea.Right);
                var shadowRight = Math.Clamp(cx + bandWidth / 2, plotArea.Left, plotArea.Right);
                var shadowRect = new Rect(shadowLeft, plotArea.Top, shadowRight - shadowLeft, plotArea.Height);
                context.DrawRectangle(shadowBrush, null, shadowRect);
                break;

            case AxisPointerType.Cross:
                // Vertical line
                context.DrawLine(linePen,
                    new Point(cx, plotArea.Top),
                    new Point(cx, plotArea.Bottom));
                // Horizontal line
                context.DrawLine(linePen,
                    new Point(plotArea.Left, cy),
                    new Point(plotArea.Right, cy));

                // Labels on both axes
                if (ShowLabel)
                {
                    if (xAxis != null)
                    {
                        var xVal = xAxis.PixelToValue(cx);
                        DrawAxisLabel(context, plotArea, cx, plotArea.Bottom, xVal, xAxis, true);
                    }
                    if (yAxis != null)
                    {
                        var yVal = yAxis.PixelToValue(cy);
                        DrawAxisLabel(context, plotArea, plotArea.Left, cy, yVal, yAxis, false);
                    }
                }
                break;
        }
    }

    /// <summary>
    /// Draw a value label at the axis position.
    /// </summary>
    private void DrawAxisLabel(
        DrawingContext context,
        Rect plotArea,
        double pixelX,
        double pixelY,
        double value,
        ChartAxis axis,
        bool isXAxis)
    {
        var fontFamily = LabelFontFamily ?? "Segoe UI";
        var fontSize = LabelFontSize;
        var textBrush = LabelColor ?? Brushes.White;
        var bgBrush = LabelBackground ?? new SolidColorBrush(Color.Parse("#313033"));

        var text = LabelFormatter != null
            ? LabelFormatter(value)
            : value.ToString("F2");

        var formatted = new FormattedText(
            text,
            System.Globalization.CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight,
            new Typeface(fontFamily, FontStyle.Normal, FontWeight.Normal),
            fontSize,
            textBrush);

        var padding = 4.0;
        var labelW = formatted.Width + padding * 2;
        var labelH = formatted.Height + padding * 2;

        double labelX, labelY;
        if (isXAxis)
        {
            labelX = pixelX - labelW / 2;
            labelY = pixelY + 4;
            // Clamp to chart bounds
            labelX = Math.Clamp(labelX, plotArea.Left, plotArea.Right - labelW);
        }
        else
        {
            labelX = pixelX - labelW - 4;
            labelY = pixelY - labelH / 2;
            labelX = Math.Clamp(labelX, plotArea.Left, plotArea.Right - labelW);
            labelY = Math.Clamp(labelY, plotArea.Top, plotArea.Bottom - labelH);
        }

        var labelRect = new Rect(labelX, labelY, labelW, labelH);
        context.DrawRectangle(bgBrush, null, labelRect, 3, 3);
        context.DrawText(formatted, new Point(labelX + padding, labelY + padding));
    }
}
