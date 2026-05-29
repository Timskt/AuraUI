using Avalonia;
using Avalonia.Media;

namespace AuraUI.Controls.Charts.Interaction;

/// <summary>
/// Axis pointer provides crosshair or shadow indicators that follow the mouse cursor,
/// snapping to the nearest data point on the axis. Shows axis labels at the pointer position.
///
/// Modes:
///   - Line: Thin crosshair line at the pointer position
///   - Shadow: Shaded band at the axis position
///   - Cross: Both X and Y crosshair lines
///
/// Usage:
///   var axisPointer = new AxisPointer();
///   axisPointer.Mode = AxisPointerMode.Cross;
///   chart.AxisPointer = axisPointer;
/// </summary>
public class AxisPointer : AvaloniaObject
{
    /// <summary>
    /// Defines the <see cref="Mode"/> styled property.
    /// </summary>
    public static readonly StyledProperty<AxisPointerMode> ModeProperty =
        AvaloniaProperty.Register<AxisPointer, AxisPointerMode>(nameof(Mode), AxisPointerMode.Line);

    /// <summary>
    /// Defines the <see cref="IsVisible"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsVisibleProperty =
        AvaloniaProperty.Register<AxisPointer, bool>(nameof(IsVisible), true);

    /// <summary>
    /// Defines the <see cref="LineColor"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> LineColorProperty =
        AvaloniaProperty.Register<AxisPointer, IBrush?>(nameof(LineColor));

    /// <summary>
    /// Defines the <see cref="LineThickness"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> LineThicknessProperty =
        AvaloniaProperty.Register<AxisPointer, double>(nameof(LineThickness), 1.0);

    /// <summary>
    /// Defines the <see cref="ShadowColor"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> ShadowColorProperty =
        AvaloniaProperty.Register<AxisPointer, IBrush?>(nameof(ShadowColor));

    /// <summary>
    /// Defines the <see cref="ShadowWidth"/> styled property.
    /// Width of the shadow band in pixels (for Shadow mode).
    /// </summary>
    public static readonly StyledProperty<double> ShadowWidthProperty =
        AvaloniaProperty.Register<AxisPointer, double>(nameof(ShadowWidth), 40.0);

    /// <summary>
    /// Defines the <see cref="ShowLabels"/> styled property.
    /// Whether to show axis value labels at the pointer position.
    /// </summary>
    public static readonly StyledProperty<bool> ShowLabelsProperty =
        AvaloniaProperty.Register<AxisPointer, bool>(nameof(ShowLabels), true);

    /// <summary>
    /// Defines the <see cref="LabelFontSize"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> LabelFontSizeProperty =
        AvaloniaProperty.Register<AxisPointer, double>(nameof(LabelFontSize), 10.0);

    /// <summary>
    /// Defines the <see cref="LabelBackground"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> LabelBackgroundProperty =
        AvaloniaProperty.Register<AxisPointer, IBrush?>(nameof(LabelBackground));

    /// <summary>
    /// Defines the <see cref="SnapToData"/> styled property.
    /// Whether to snap the pointer to the nearest data point.
    /// </summary>
    public static readonly StyledProperty<bool> SnapToDataProperty =
        AvaloniaProperty.Register<AxisPointer, bool>(nameof(SnapToData), true);

    /// <summary>
    /// Defines the <see cref="DashStyle"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double[]?> DashStyleProperty =
        AvaloniaProperty.Register<AxisPointer, double[]?>(nameof(DashStyle));

    public AxisPointerMode Mode { get => GetValue(ModeProperty); set => SetValue(ModeProperty, value); }
    public bool IsVisible { get => GetValue(IsVisibleProperty); set => SetValue(IsVisibleProperty, value); }
    public IBrush? LineColor { get => GetValue(LineColorProperty); set => SetValue(LineColorProperty, value); }
    public double LineThickness { get => GetValue(LineThicknessProperty); set => SetValue(LineThicknessProperty, value); }
    public IBrush? ShadowColor { get => GetValue(ShadowColorProperty); set => SetValue(ShadowColorProperty, value); }
    public double ShadowWidth { get => GetValue(ShadowWidthProperty); set => SetValue(ShadowWidthProperty, value); }
    public bool ShowLabels { get => GetValue(ShowLabelsProperty); set => SetValue(ShowLabelsProperty, value); }
    public double LabelFontSize { get => GetValue(LabelFontSizeProperty); set => SetValue(LabelFontSizeProperty, value); }
    public IBrush? LabelBackground { get => GetValue(LabelBackgroundProperty); set => SetValue(LabelBackgroundProperty, value); }
    public bool SnapToData { get => GetValue(SnapToDataProperty); set => SetValue(SnapToDataProperty, value); }
    public double[]? DashStyle { get => GetValue(DashStyleProperty); set => SetValue(DashStyleProperty, value); }

    /// <summary>
    /// Current pointer position in data coordinates (set by the chart on mouse move).
    /// </summary>
    internal Point CurrentDataPosition { get; set; }

    /// <summary>
    /// Current pointer position in pixel coordinates.
    /// </summary>
    internal Point CurrentPixelPosition { get; set; }

    /// <summary>
    /// Whether the pointer is currently within the plot area.
    /// </summary>
    internal bool IsWithinPlot { get; set; }

    /// <summary>
    /// Render the axis pointer overlay.
    /// </summary>
    public void Render(DrawingContext context, Rect plotArea,
        ChartAxis? xAxis, ChartAxis? yAxis, IReadOnlyList<ChartSeries> allSeries)
    {
        if (!IsVisible || !IsWithinPlot) return;

        var lineColor = LineColor ?? Brushes.Gray;
        var pen = CreatePen(lineColor);

        switch (Mode)
        {
            case AxisPointerMode.Line:
                DrawLineMode(context, plotArea, xAxis, yAxis, pen);
                break;

            case AxisPointerMode.Shadow:
                DrawShadowMode(context, plotArea, xAxis, yAxis);
                break;

            case AxisPointerMode.Cross:
                DrawLineMode(context, plotArea, xAxis, yAxis, pen);
                DrawCrossMode(context, plotArea, xAxis, yAxis, pen);
                break;
        }

        // Draw labels
        if (ShowLabels)
        {
            DrawAxisLabels(context, plotArea, xAxis, yAxis, lineColor);
        }
    }

    /// <summary>
    /// Update the pointer position from mouse coordinates.
    /// </summary>
    public void UpdatePosition(Point pixelPosition, Rect plotArea,
        ChartAxis? xAxis, ChartAxis? yAxis, IReadOnlyList<ChartSeries> allSeries)
    {
        CurrentPixelPosition = pixelPosition;
        IsWithinPlot = plotArea.Contains(pixelPosition);

        if (!IsWithinPlot) return;

        if (SnapToData && allSeries != null)
        {
            SnapToNearestData(pixelPosition, plotArea, xAxis, yAxis, allSeries);
        }
        else
        {
            CurrentDataPosition = new Point(
                xAxis?.PixelToValue(pixelPosition.X) ?? pixelPosition.X,
                yAxis?.PixelToValue(pixelPosition.Y) ?? pixelPosition.Y);
        }
    }

    private void SnapToNearestData(Point pixelPosition, Rect plotArea,
        ChartAxis? xAxis, ChartAxis? yAxis, IReadOnlyList<ChartSeries> allSeries)
    {
        double bestDist = double.MaxValue;
        double bestX = 0, bestY = 0;
        double bestPx = 0, bestPy = 0;

        foreach (var series in allSeries)
        {
            if (series is not XYChartSeries xy || !xy.IsVisible) continue;
            if (xAxis == null || yAxis == null) continue;

            foreach (var dp in xy.DataPoints)
            {
                var px = xAxis.ValueToPixel(dp.X);
                var py = yAxis.ValueToPixel(dp.Y);
                var dx = pixelPosition.X - px;
                var dy = pixelPosition.Y - py;
                var dist = dx * dx + dy * dy;

                if (dist < bestDist)
                {
                    bestDist = dist;
                    bestX = dp.X;
                    bestY = dp.Y;
                    bestPx = px;
                    bestPy = py;
                }
            }
        }

        if (bestDist < 10000) // Within ~100px
        {
            CurrentDataPosition = new Point(bestX, bestY);
            CurrentPixelPosition = new Point(bestPx, bestPy);
        }
        else
        {
            CurrentDataPosition = new Point(
                xAxis?.PixelToValue(pixelPosition.X) ?? pixelPosition.X,
                yAxis?.PixelToValue(pixelPosition.Y) ?? pixelPosition.Y);
        }
    }

    private void DrawLineMode(DrawingContext context, Rect plotArea,
        ChartAxis? xAxis, ChartAxis? yAxis, Pen pen)
    {
        // Draw vertical line (X axis pointer)
        if (xAxis != null)
        {
            context.DrawLine(pen,
                new Point(CurrentPixelPosition.X, plotArea.Top),
                new Point(CurrentPixelPosition.X, plotArea.Bottom));
        }

        // For Cross mode, also draw horizontal
        if (Mode == AxisPointerMode.Cross && yAxis != null)
        {
            context.DrawLine(pen,
                new Point(plotArea.Left, CurrentPixelPosition.Y),
                new Point(plotArea.Right, CurrentPixelPosition.Y));
        }
    }

    private void DrawCrossMode(DrawingContext context, Rect plotArea,
        ChartAxis? xAxis, ChartAxis? yAxis, Pen pen)
    {
        if (yAxis != null)
        {
            context.DrawLine(pen,
                new Point(plotArea.Left, CurrentPixelPosition.Y),
                new Point(plotArea.Right, CurrentPixelPosition.Y));
        }
    }

    private void DrawShadowMode(DrawingContext context, Rect plotArea,
        ChartAxis? xAxis, ChartAxis? yAxis)
    {
        var shadowColor = ShadowColor ?? Brushes.LightGray;
        var shadowBrush = new SolidColorBrush(((SolidColorBrush)shadowColor).Color) { Opacity = 0.2 };

        var halfWidth = ShadowWidth / 2;
        var shadowRect = new Rect(
            CurrentPixelPosition.X - halfWidth,
            plotArea.Top,
            ShadowWidth,
            plotArea.Height);

        context.DrawRectangle(shadowBrush, null, shadowRect);
    }

    private void DrawAxisLabels(DrawingContext context, Rect plotArea,
        ChartAxis? xAxis, ChartAxis? yAxis, IBrush color)
    {
        var bgColor = LabelBackground ?? Brushes.DarkSlateGray;
        var bgBrush = new SolidColorBrush(((SolidColorBrush)bgColor).Color) { Opacity = 0.9 };
        var labelColor = Brushes.White;

        // X axis label
        if (xAxis != null)
        {
            var xValue = CurrentDataPosition.X;
            var labelText = FormatAxisValue(xValue, xAxis);

            var formattedText = new FormattedText(labelText,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface("Segoe UI", FontStyle.Normal, FontWeight.Normal),
                LabelFontSize,
                labelColor);

            var labelWidth = formattedText.Width + 8;
            var labelHeight = formattedText.Height + 4;
            var labelX = CurrentPixelPosition.X - labelWidth / 2;
            var labelY = plotArea.Bottom + 2;

            // Clamp to plot area
            labelX = Math.Max(plotArea.Left, Math.Min(plotArea.Right - labelWidth, labelX));

            var labelRect = new Rect(labelX, labelY, labelWidth, labelHeight);
            context.DrawRectangle(bgBrush, null, labelRect, 3, 3);
            context.DrawText(formattedText, new Point(labelX + 4, labelY + 2));
        }

        // Y axis label (for Cross mode)
        if (Mode == AxisPointerMode.Cross && yAxis != null)
        {
            var yValue = CurrentDataPosition.Y;
            var labelText = FormatAxisValue(yValue, yAxis);

            var formattedText = new FormattedText(labelText,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface("Segoe UI", FontStyle.Normal, FontWeight.Normal),
                LabelFontSize,
                labelColor);

            var labelWidth = formattedText.Width + 8;
            var labelHeight = formattedText.Height + 4;
            var labelX = plotArea.Left - labelWidth - 2;
            var labelY = CurrentPixelPosition.Y - labelHeight / 2;

            // Clamp
            labelY = Math.Max(plotArea.Top, Math.Min(plotArea.Bottom - labelHeight, labelY));

            var labelRect = new Rect(labelX, labelY, labelWidth, labelHeight);
            context.DrawRectangle(bgBrush, null, labelRect, 3, 3);
            context.DrawText(formattedText, new Point(labelX + 4, labelY + 2));
        }
    }

    private Pen CreatePen(IBrush color)
    {
        if (DashStyle != null && DashStyle.Length > 0)
        {
            return new Pen(color, LineThickness, new DashStyle(DashStyle, 0));
        }
        return new Pen(color, LineThickness);
    }

    private static string FormatAxisValue(double value, ChartAxis axis)
    {
        if (!string.IsNullOrEmpty(axis.LabelFormat))
            return value.ToString(axis.LabelFormat);

        if (axis.Scale == AxisScale.Category && axis.Categories != null)
        {
            var index = (int)Math.Round(value);
            if (index >= 0 && index < axis.Categories.Length)
                return axis.Categories[index];
        }

        return Math.Abs(value) >= 1000 ? value.ToString("N0") : value.ToString("F1");
    }
}
