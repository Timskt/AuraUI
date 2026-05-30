using Avalonia;
using Avalonia.Media;

namespace AuraUI.Controls.Charts.Annotations;

/// <summary>
/// Base class for chart annotations (mark points, mark lines, mark areas).
/// Annotations are visual overlays that highlight specific data values or regions
/// without being part of the data series itself.
/// </summary>
public abstract class ChartAnnotation : AvaloniaObject
{
    /// <summary>
    /// Defines the <see cref="IsVisible"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsVisibleProperty =
        AvaloniaProperty.Register<ChartAnnotation, bool>(nameof(IsVisible), true);

    /// <summary>
    /// Defines the <see cref="Color"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> ColorProperty =
        AvaloniaProperty.Register<ChartAnnotation, IBrush?>(nameof(Color));

    public bool IsVisible { get => GetValue(IsVisibleProperty); set => SetValue(IsVisibleProperty, value); }
    public IBrush? Color { get => GetValue(ColorProperty); set => SetValue(ColorProperty, value); }

    /// <summary>
    /// Render this annotation onto the drawing context.
    /// </summary>
    public abstract void Render(DrawingContext context, Rect plotArea,
        ChartAxis? xAxis, ChartAxis? yAxis, IReadOnlyList<ChartSeries> allSeries);
}

/// <summary>
/// Renders a text label at a specific data point on the chart.
/// Useful for calling out notable data points, thresholds, or events.
///
/// Usage:
///   var text = new TextAnnotation();
///   text.X = 5;
///   text.Y = 100;
///   text.Text = "Peak";
///   chart.Annotations.Add(text);
/// </summary>
public class TextAnnotation : ChartAnnotation
{
    /// <summary>Defines the <see cref="X"/> styled property.</summary>
    public static readonly StyledProperty<double> XProperty =
        AvaloniaProperty.Register<TextAnnotation, double>(nameof(X));

    /// <summary>Defines the <see cref="Y"/> styled property.</summary>
    public static readonly StyledProperty<double> YProperty =
        AvaloniaProperty.Register<TextAnnotation, double>(nameof(Y));

    /// <summary>Defines the <see cref="Text"/> styled property.</summary>
    public static readonly StyledProperty<string?> TextProperty =
        AvaloniaProperty.Register<TextAnnotation, string?>(nameof(Text));

    /// <summary>Defines the <see cref="FontSize"/> styled property.</summary>
    public static readonly StyledProperty<double> FontSizeProperty =
        AvaloniaProperty.Register<TextAnnotation, double>(nameof(FontSize), 11.0);

    /// <summary>Defines the <see cref="FontWeight"/> styled property.</summary>
    public static readonly StyledProperty<FontWeight> FontWeightProperty =
        AvaloniaProperty.Register<TextAnnotation, FontWeight>(nameof(FontWeight), FontWeight.Normal);

    /// <summary>Horizontal pixel offset from the data point.</summary>
    public static readonly StyledProperty<double> OffsetXProperty =
        AvaloniaProperty.Register<TextAnnotation, double>(nameof(OffsetX));

    /// <summary>Vertical pixel offset from the data point (negative = above).</summary>
    public static readonly StyledProperty<double> OffsetYProperty =
        AvaloniaProperty.Register<TextAnnotation, double>(nameof(OffsetY), -16);

    /// <summary>When true, draws a thin connector line from the label to the data point.</summary>
    public static readonly StyledProperty<bool> ShowConnectorProperty =
        AvaloniaProperty.Register<TextAnnotation, bool>(nameof(ShowConnector));

    /// <summary>Defines the <see cref="BackgroundBrush"/> styled property.</summary>
    public static readonly StyledProperty<IBrush?> BackgroundBrushProperty =
        AvaloniaProperty.Register<TextAnnotation, IBrush?>(nameof(BackgroundBrush));

    /// <summary>Defines the <see cref="CornerRadius"/> styled property.</summary>
    public static readonly StyledProperty<double> CornerRadiusProperty =
        AvaloniaProperty.Register<TextAnnotation, double>(nameof(CornerRadius), 3.0);

    /// <summary>Defines the <see cref="Padding"/> styled property.</summary>
    public static readonly StyledProperty<Thickness> PaddingProperty =
        AvaloniaProperty.Register<TextAnnotation, Thickness>(nameof(Padding), new Thickness(4, 2));

    public double X { get => GetValue(XProperty); set => SetValue(XProperty, value); }
    public double Y { get => GetValue(YProperty); set => SetValue(YProperty, value); }
    public string? Text { get => GetValue(TextProperty); set => SetValue(TextProperty, value); }
    public double FontSize { get => GetValue(FontSizeProperty); set => SetValue(FontSizeProperty, value); }
    public FontWeight FontWeight { get => GetValue(FontWeightProperty); set => SetValue(FontWeightProperty, value); }
    public double OffsetX { get => GetValue(OffsetXProperty); set => SetValue(OffsetXProperty, value); }
    public double OffsetY { get => GetValue(OffsetYProperty); set => SetValue(OffsetYProperty, value); }
    public bool ShowConnector { get => GetValue(ShowConnectorProperty); set => SetValue(ShowConnectorProperty, value); }
    public IBrush? BackgroundBrush { get => GetValue(BackgroundBrushProperty); set => SetValue(BackgroundBrushProperty, value); }
    public double CornerRadius { get => GetValue(CornerRadiusProperty); set => SetValue(CornerRadiusProperty, value); }
    public Thickness Padding { get => GetValue(PaddingProperty); set => SetValue(PaddingProperty, value); }

    public override void Render(DrawingContext context, Rect plotArea,
        ChartAxis? xAxis, ChartAxis? yAxis, IReadOnlyList<ChartSeries> allSeries)
    {
        if (!IsVisible || string.IsNullOrEmpty(Text)) return;
        if (xAxis == null || yAxis == null) return;

        var color = Color ?? Brushes.Black;
        var typeface = new Typeface("Segoe UI", FontStyle.Normal, FontWeight);
        var formattedText = new FormattedText(Text,
            System.Globalization.CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight,
            typeface, FontSize, color);

        var dataPx = xAxis.ValueToPixel(X);
        var dataPy = yAxis.ValueToPixel(Y);
        var dataPoint = new Point(dataPx, dataPy);

        var labelX = dataPx + OffsetX - formattedText.Width / 2;
        var labelY = dataPy + OffsetY - formattedText.Height / 2;

        // Draw background pill if set
        if (BackgroundBrush is IBrush bg)
        {
            var pad = Padding;
            var bgRect = new Rect(
                labelX - pad.Left,
                labelY - pad.Top,
                formattedText.Width + pad.Left + pad.Right,
                formattedText.Height + pad.Top + pad.Bottom);
            context.DrawRectangle(bg, null, bgRect, CornerRadius, CornerRadius);
        }

        // Draw connector line
        if (ShowConnector)
        {
            var connectorPen = new Pen(color, 1.0, new DashStyle(new double[] { 2, 2 }, 0));
            var labelCenter = new Point(dataPx + OffsetX, dataPy + OffsetY);
            context.DrawLine(connectorPen, dataPoint, labelCenter);
        }

        // Draw text
        context.DrawText(formattedText, new Point(labelX, labelY));
    }
}

/// <summary>
/// Highlights the area between two data points (by X value) with a semi-transparent fill
/// and optional boundary lines. Useful for marking time ranges, forecast zones, or
/// significant periods in the data.
///
/// Usage:
///   var region = new DataRegionAnnotation();
///   region.StartX = 3;
///   region.EndX = 7;
///   region.Label = "Holiday Season";
///   chart.Annotations.Add(region);
/// </summary>
public class DataRegionAnnotation : ChartAnnotation
{
    /// <summary>Defines the <see cref="StartX"/> styled property.</summary>
    public static readonly StyledProperty<double> StartXProperty =
        AvaloniaProperty.Register<DataRegionAnnotation, double>(nameof(StartX));

    /// <summary>Defines the <see cref="EndX"/> styled property.</summary>
    public static readonly StyledProperty<double> EndXProperty =
        AvaloniaProperty.Register<DataRegionAnnotation, double>(nameof(EndX));

    /// <summary>Defines the <see cref="Label"/> styled property.</summary>
    public static readonly StyledProperty<string?> LabelProperty =
        AvaloniaProperty.Register<DataRegionAnnotation, string?>(nameof(Label));

    /// <summary>Defines the <see cref="FillOpacity"/> styled property.</summary>
    public static readonly StyledProperty<double> FillOpacityProperty =
        AvaloniaProperty.Register<DataRegionAnnotation, double>(nameof(FillOpacity), 0.1);

    /// <summary>When true, draws dashed vertical lines at start and end.</summary>
    public static readonly StyledProperty<bool> ShowBoundariesProperty =
        AvaloniaProperty.Register<DataRegionAnnotation, bool>(nameof(ShowBoundaries), true);

    /// <summary>Label position within the region.</summary>
    public static readonly StyledProperty<DataRegionLabelPosition> LabelPositionProperty =
        AvaloniaProperty.Register<DataRegionAnnotation, DataRegionLabelPosition>(nameof(LabelPosition), DataRegionLabelPosition.Top);

    public double StartX { get => GetValue(StartXProperty); set => SetValue(StartXProperty, value); }
    public double EndX { get => GetValue(EndXProperty); set => SetValue(EndXProperty, value); }
    public string? Label { get => GetValue(LabelProperty); set => SetValue(LabelProperty, value); }
    public double FillOpacity { get => GetValue(FillOpacityProperty); set => SetValue(FillOpacityProperty, value); }
    public bool ShowBoundaries { get => GetValue(ShowBoundariesProperty); set => SetValue(ShowBoundariesProperty, value); }
    public DataRegionLabelPosition LabelPosition { get => GetValue(LabelPositionProperty); set => SetValue(LabelPositionProperty, value); }

    public override void Render(DrawingContext context, Rect plotArea,
        ChartAxis? xAxis, ChartAxis? yAxis, IReadOnlyList<ChartSeries> allSeries)
    {
        if (!IsVisible) return;
        if (xAxis == null) return;

        var color = Color ?? Brushes.SteelBlue;
        var fillBrush = new SolidColorBrush(((SolidColorBrush)color).Color) { Opacity = FillOpacity };

        var px1 = xAxis.ValueToPixel(StartX);
        var px2 = xAxis.ValueToPixel(EndX);
        var regionRect = new Rect(Math.Min(px1, px2), plotArea.Top, Math.Abs(px2 - px1), plotArea.Height);

        // Fill region
        context.DrawRectangle(fillBrush, null, regionRect);

        // Boundary lines
        if (ShowBoundaries)
        {
            var boundaryPen = new Pen(color, 1.0, new DashStyle(new double[] { 4, 3 }, 0));
            context.DrawLine(boundaryPen, new Point(px1, plotArea.Top), new Point(px1, plotArea.Bottom));
            context.DrawLine(boundaryPen, new Point(px2, plotArea.Top), new Point(px2, plotArea.Bottom));
        }

        // Label
        if (!string.IsNullOrEmpty(Label))
        {
            var formattedText = new FormattedText(Label,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface("Segoe UI", FontStyle.Normal, FontWeight.SemiBold),
                10.0, color);

            double lx, ly;
            switch (LabelPosition)
            {
                case DataRegionLabelPosition.Top:
                    lx = regionRect.Center.X - formattedText.Width / 2;
                    ly = regionRect.Top + 4;
                    break;
                case DataRegionLabelPosition.Center:
                    lx = regionRect.Center.X - formattedText.Width / 2;
                    ly = regionRect.Center.Y - formattedText.Height / 2;
                    break;
                case DataRegionLabelPosition.Bottom:
                    lx = regionRect.Center.X - formattedText.Width / 2;
                    ly = regionRect.Bottom - formattedText.Height - 4;
                    break;
                default:
                    lx = regionRect.X + 4;
                    ly = regionRect.Top + 4;
                    break;
            }

            context.DrawText(formattedText, new Point(lx, ly));
        }
    }
}

/// <summary>
/// Label position within a data region annotation.
/// </summary>
public enum DataRegionLabelPosition
{
    Top,
    Center,
    Bottom
}

/// <summary>
/// Renders mark points (max, min, average) on chart series.
/// Mark points are symbols placed at specific data locations to highlight important values.
///
/// Usage:
///   var markPoint = new MarkPointAnnotation();
///   markPoint.MarkType = MarkType.Max;
///   markPoint.SymbolSize = 12;
///   chart.Annotations.Add(markPoint);
/// </summary>
public class MarkPointAnnotation : ChartAnnotation
{
    /// <summary>
    /// Defines the <see cref="MarkType"/> styled property.
    /// </summary>
    public static readonly StyledProperty<MarkType> MarkTypeProperty =
        AvaloniaProperty.Register<MarkPointAnnotation, MarkType>(nameof(MarkType));

    /// <summary>
    /// Defines the <see cref="SymbolSize"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> SymbolSizeProperty =
        AvaloniaProperty.Register<MarkPointAnnotation, double>(nameof(SymbolSize), 10.0);

    /// <summary>
    /// Defines the <see cref="ShowLabel"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowLabelProperty =
        AvaloniaProperty.Register<MarkPointAnnotation, bool>(nameof(ShowLabel), true);

    /// <summary>
    /// Defines the <see cref="LabelFontSize"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> LabelFontSizeProperty =
        AvaloniaProperty.Register<MarkPointAnnotation, double>(nameof(LabelFontSize), 10.0);

    /// <summary>
    /// Defines the <see cref="LabelFormat"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> LabelFormatProperty =
        AvaloniaProperty.Register<MarkPointAnnotation, string?>(nameof(LabelFormat));

    /// <summary>
    /// Defines the <see cref="SeriesIndex"/> styled property.
    /// Which series to apply the mark to (-1 = all series).
    /// </summary>
    public static readonly StyledProperty<int> SeriesIndexProperty =
        AvaloniaProperty.Register<MarkPointAnnotation, int>(nameof(SeriesIndex), -1);

    public MarkType MarkType { get => GetValue(MarkTypeProperty); set => SetValue(MarkTypeProperty, value); }
    public double SymbolSize { get => GetValue(SymbolSizeProperty); set => SetValue(SymbolSizeProperty, value); }
    public bool ShowLabel { get => GetValue(ShowLabelProperty); set => SetValue(ShowLabelProperty, value); }
    public double LabelFontSize { get => GetValue(LabelFontSizeProperty); set => SetValue(LabelFontSizeProperty, value); }
    public string? LabelFormat { get => GetValue(LabelFormatProperty); set => SetValue(LabelFormatProperty, value); }
    public int SeriesIndex { get => GetValue(SeriesIndexProperty); set => SetValue(SeriesIndexProperty, value); }

    /// <summary>
    /// Explicit mark points (overrides MarkType when set).
    /// </summary>
    public List<ChartMarkPoint> Points { get; } = new();

    public override void Render(DrawingContext context, Rect plotArea,
        ChartAxis? xAxis, ChartAxis? yAxis, IReadOnlyList<ChartSeries> allSeries)
    {
        if (!IsVisible) return;

        var color = Color ?? Brushes.Red;
        var symbolPen = new Pen(color, 2.0);
        var symbolBrush = new SolidColorBrush(((SolidColorBrush)color).Color) { Opacity = 0.8 };
        var half = SymbolSize / 2;

        // Render explicit points
        foreach (var point in Points)
        {
            if (point.X == null || point.Y == null) continue;
            if (xAxis == null || yAxis == null) continue;

            var px = xAxis.ValueToPixel(point.X.Value);
            var py = yAxis.ValueToPixel(point.Y.Value);
            var pt = new Point(px, py);

            // Draw diamond symbol
            DrawDiamondSymbol(context, pt, half, symbolBrush, symbolPen);

            if (ShowLabel)
            {
                var label = point.Label ?? FormatValue(point.Y.Value);
                DrawLabel(context, label, pt, half, color);
            }
        }

        // Render computed marks (max/min/average) for XY series
        for (int si = 0; si < allSeries.Count; si++)
        {
            if (SeriesIndex >= 0 && si != SeriesIndex) continue;
            if (allSeries[si] is not XYChartSeries xy || !xy.IsVisible) continue;
            if (xAxis == null || yAxis == null) continue;
            if (xy.DataPoints.Count == 0) continue;

            ChartDataPoint targetPoint;
            string markLabel;

            switch (MarkType)
            {
                case MarkType.Max:
                    targetPoint = xy.DataPoints.MaxBy(p => p.Y)!;
                    markLabel = "Max";
                    break;
                case MarkType.Min:
                    targetPoint = xy.DataPoints.MinBy(p => p.Y)!;
                    markLabel = "Min";
                    break;
                case MarkType.Average:
                    var avg = xy.DataPoints.Average(p => p.Y);
                    targetPoint = xy.DataPoints.OrderBy(p => Math.Abs(p.Y - avg)).First();
                    markLabel = "Avg";
                    break;
                default:
                    continue;
            }

            var px = xAxis.ValueToPixel(targetPoint.X);
            var py = yAxis.ValueToPixel(targetPoint.Y);
            var pt = new Point(px, py);

            DrawDiamondSymbol(context, pt, half, symbolBrush, symbolPen);

            if (ShowLabel)
            {
                var format = LabelFormat ?? "F1";
                var label = $"{markLabel}: {targetPoint.Y.ToString(format)}";
                DrawLabel(context, label, pt, half, color);
            }
        }
    }

    private static void DrawDiamondSymbol(DrawingContext context, Point center, double half,
        IBrush fill, Pen pen)
    {
        var geometry = new PathGeometry();
        var figure = new PathFigure { StartPoint = new Point(center.X, center.Y - half), IsClosed = true };
        figure.Segments!.Add(new LineSegment { Point = new Point(center.X + half, center.Y) });
        figure.Segments!.Add(new LineSegment { Point = new Point(center.X, center.Y + half) });
        figure.Segments!.Add(new LineSegment { Point = new Point(center.X - half, center.Y) });
        geometry.Figures!.Add(figure);
        context.DrawGeometry(fill, pen, geometry);
    }

    private static void DrawLabel(DrawingContext context, string text, Point position,
        double offset, IBrush color)
    {
        var formattedText = new FormattedText(text,
            System.Globalization.CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight,
            new Typeface("Segoe UI", FontStyle.Normal, FontWeight.SemiBold),
            10.0,
            color);

        context.DrawText(formattedText,
            new Point(position.X - formattedText.Width / 2, position.Y - offset - formattedText.Height - 2));
    }

    private string FormatValue(double value)
    {
        if (!string.IsNullOrEmpty(LabelFormat))
            return value.ToString(LabelFormat);
        return value.ToString("F1");
    }
}

/// <summary>
/// Renders mark lines (horizontal or vertical reference lines) on charts.
/// Mark lines are dashed lines that highlight specific values like averages or thresholds.
///
/// Usage:
///   var markLine = new MarkLineAnnotation();
///   markLine.MarkType = MarkType.Average;
///   chart.Annotations.Add(markLine);
/// </summary>
public class MarkLineAnnotation : ChartAnnotation
{
    /// <summary>
    /// Defines the <see cref="MarkType"/> styled property.
    /// </summary>
    public static readonly StyledProperty<MarkType> MarkTypeProperty =
        AvaloniaProperty.Register<MarkLineAnnotation, MarkType>(nameof(MarkType));

    /// <summary>
    /// Defines the <see cref="ShowLabel"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowLabelProperty =
        AvaloniaProperty.Register<MarkLineAnnotation, bool>(nameof(ShowLabel), true);

    /// <summary>
    /// Defines the <see cref="LabelFontSize"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> LabelFontSizeProperty =
        AvaloniaProperty.Register<MarkLineAnnotation, double>(nameof(LabelFontSize), 10.0);

    /// <summary>
    /// Defines the <see cref="Thickness"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> ThicknessProperty =
        AvaloniaProperty.Register<MarkLineAnnotation, double>(nameof(Thickness), 1.0);

    /// <summary>
    /// Defines the <see cref="DashStyle"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double[]?> DashStyleProperty =
        AvaloniaProperty.Register<MarkLineAnnotation, double[]?>(nameof(DashStyle));

    /// <summary>
    /// Defines the <see cref="SeriesIndex"/> styled property.
    /// </summary>
    public static readonly StyledProperty<int> SeriesIndexProperty =
        AvaloniaProperty.Register<MarkLineAnnotation, int>(nameof(SeriesIndex), -1);

    /// <summary>
    /// Defines the <see cref="IsVertical"/> styled property.
    /// When true, draws a vertical line instead of horizontal.
    /// </summary>
    public static readonly StyledProperty<bool> IsVerticalProperty =
        AvaloniaProperty.Register<MarkLineAnnotation, bool>(nameof(IsVertical));

    public MarkType MarkType { get => GetValue(MarkTypeProperty); set => SetValue(MarkTypeProperty, value); }
    public bool ShowLabel { get => GetValue(ShowLabelProperty); set => SetValue(ShowLabelProperty, value); }
    public double LabelFontSize { get => GetValue(LabelFontSizeProperty); set => SetValue(LabelFontSizeProperty, value); }
    public double Thickness { get => GetValue(ThicknessProperty); set => SetValue(ThicknessProperty, value); }
    public double[]? DashStyle { get => GetValue(DashStyleProperty); set => SetValue(DashStyleProperty, value); }
    public int SeriesIndex { get => GetValue(SeriesIndexProperty); set => SetValue(SeriesIndexProperty, value); }
    public bool IsVertical { get => GetValue(IsVerticalProperty); set => SetValue(IsVerticalProperty, value); }

    /// <summary>
    /// Explicit mark lines (overrides MarkType when set).
    /// </summary>
    public List<ChartMarkLine> Lines { get; } = new();

    public override void Render(DrawingContext context, Rect plotArea,
        ChartAxis? xAxis, ChartAxis? yAxis, IReadOnlyList<ChartSeries> allSeries)
    {
        if (!IsVisible) return;

        var color = Color ?? Brushes.OrangeRed;
        var pen = CreatePen(color);

        // Render explicit lines
        foreach (var line in Lines)
        {
            if (line.Value == null) continue;
            if (xAxis == null || yAxis == null) continue;

            if (IsVertical)
            {
                var px = xAxis.ValueToPixel(line.Value.Value);
                context.DrawLine(pen, new Point(px, plotArea.Top), new Point(px, plotArea.Bottom));
            }
            else
            {
                var py = yAxis.ValueToPixel(line.Value.Value);
                context.DrawLine(pen, new Point(plotArea.Left, py), new Point(plotArea.Right, py));
            }
        }

        // Render computed marks for XY series
        for (int si = 0; si < allSeries.Count; si++)
        {
            if (SeriesIndex >= 0 && si != SeriesIndex) continue;
            if (allSeries[si] is not XYChartSeries xy || !xy.IsVisible) continue;
            if (xAxis == null || yAxis == null) continue;
            if (xy.DataPoints.Count == 0) continue;

            double markValue;
            string markLabel;

            switch (MarkType)
            {
                case MarkType.Average:
                    markValue = xy.DataPoints.Average(p => p.Y);
                    markLabel = $"Average: {markValue:F1}";
                    break;
                case MarkType.Max:
                    markValue = xy.DataPoints.Max(p => p.Y);
                    markLabel = $"Max: {markValue:F1}";
                    break;
                case MarkType.Min:
                    markValue = xy.DataPoints.Min(p => p.Y);
                    markLabel = $"Min: {markValue:F1}";
                    break;
                default:
                    continue;
            }

            if (IsVertical)
            {
                var px = xAxis.ValueToPixel(markValue);
                context.DrawLine(pen, new Point(px, plotArea.Top), new Point(px, plotArea.Bottom));
            }
            else
            {
                var py = yAxis.ValueToPixel(markValue);
                context.DrawLine(pen, new Point(plotArea.Left, py), new Point(plotArea.Right, py));

                if (ShowLabel)
                {
                    var formattedText = new FormattedText(markLabel,
                        System.Globalization.CultureInfo.CurrentCulture,
                        FlowDirection.LeftToRight,
                        new Typeface("Segoe UI", FontStyle.Normal, FontWeight.Normal),
                        LabelFontSize,
                        color);

                    context.DrawText(formattedText, new Point(plotArea.Right - formattedText.Width - 4, py - formattedText.Height - 2));
                }
            }
        }
    }

    private Pen CreatePen(IBrush color)
    {
        if (DashStyle != null && DashStyle.Length > 0)
        {
            return new Pen(color, Thickness, new Avalonia.Media.DashStyle(DashStyle, 0));
        }
        return new Pen(color, Thickness, new Avalonia.Media.DashStyle(new double[] { 4, 4 }, 0));
    }
}

/// <summary>
/// Renders mark areas (highlighted regions) on charts.
/// Mark areas are semi-transparent rectangles that highlight ranges like forecast zones
/// or confidence intervals.
///
/// Usage:
///   var markArea = new MarkAreaAnnotation();
///   markArea.Value = 60;
///   markArea.Value2 = 80;
///   markArea.Label = "Target Range";
///   chart.Annotations.Add(markArea);
/// </summary>
public class MarkAreaAnnotation : ChartAnnotation
{
    /// <summary>
    /// Defines the <see cref="Value"/> styled property.
    /// Start value of the highlighted region.
    /// </summary>
    public static readonly StyledProperty<double> ValueProperty =
        AvaloniaProperty.Register<MarkAreaAnnotation, double>(nameof(Value));

    /// <summary>
    /// Defines the <see cref="Value2"/> styled property.
    /// End value of the highlighted region.
    /// </summary>
    public static readonly StyledProperty<double> Value2Property =
        AvaloniaProperty.Register<MarkAreaAnnotation, double>(nameof(Value2));

    /// <summary>
    /// Defines the <see cref="Label"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> LabelProperty =
        AvaloniaProperty.Register<MarkAreaAnnotation, string?>(nameof(Label));

    /// <summary>
    /// Defines the <see cref="FillOpacity"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> FillOpacityProperty =
        AvaloniaProperty.Register<MarkAreaAnnotation, double>(nameof(FillOpacity), 0.15);

    /// <summary>
    /// Defines the <see cref="IsVertical"/> styled property.
    /// When true, the area spans vertically (X range) instead of horizontally (Y range).
    /// </summary>
    public static readonly StyledProperty<bool> IsVerticalProperty =
        AvaloniaProperty.Register<MarkAreaAnnotation, bool>(nameof(IsVertical));

    /// <summary>
    /// Defines the <see cref="BorderThickness"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> BorderThicknessProperty =
        AvaloniaProperty.Register<MarkAreaAnnotation, double>(nameof(BorderThickness));

    /// <summary>
    /// Defines the <see cref="SeriesIndex"/> styled property.
    /// </summary>
    public static readonly StyledProperty<int> SeriesIndexProperty =
        AvaloniaProperty.Register<MarkAreaAnnotation, int>(nameof(SeriesIndex), -1);

    public double Value { get => GetValue(ValueProperty); set => SetValue(ValueProperty, value); }
    public double Value2 { get => GetValue(Value2Property); set => SetValue(Value2Property, value); }
    public string? Label { get => GetValue(LabelProperty); set => SetValue(LabelProperty, value); }
    public double FillOpacity { get => GetValue(FillOpacityProperty); set => SetValue(FillOpacityProperty, value); }
    public bool IsVertical { get => GetValue(IsVerticalProperty); set => SetValue(IsVerticalProperty, value); }
    public double BorderThickness { get => GetValue(BorderThicknessProperty); set => SetValue(BorderThicknessProperty, value); }
    public int SeriesIndex { get => GetValue(SeriesIndexProperty); set => SetValue(SeriesIndexProperty, value); }

    /// <summary>
    /// Explicit mark areas (overrides Value/Value2 when set).
    /// </summary>
    public List<ChartMarkArea> Areas { get; } = new();

    public override void Render(DrawingContext context, Rect plotArea,
        ChartAxis? xAxis, ChartAxis? yAxis, IReadOnlyList<ChartSeries> allSeries)
    {
        if (!IsVisible) return;
        if (xAxis == null || yAxis == null) return;

        var color = Color ?? Brushes.Orange;
        var fillBrush = new SolidColorBrush(((SolidColorBrush)color).Color) { Opacity = FillOpacity };
        Pen? borderPen = BorderThickness > 0
            ? new Pen(new SolidColorBrush(((SolidColorBrush)color).Color) { Opacity = FillOpacity * 2 }, BorderThickness)
            : null;

        // Render explicit areas
        foreach (var area in Areas)
        {
            Rect areaRect;
            if (IsVertical)
            {
                var px1 = xAxis.ValueToPixel(area.Value);
                var px2 = xAxis.ValueToPixel(area.Value2);
                areaRect = new Rect(Math.Min(px1, px2), plotArea.Top, Math.Abs(px2 - px1), plotArea.Height);
            }
            else
            {
                var py1 = yAxis.ValueToPixel(area.Value);
                var py2 = yAxis.ValueToPixel(area.Value2);
                areaRect = new Rect(plotArea.Left, Math.Min(py1, py2), plotArea.Width, Math.Abs(py2 - py1));
            }

            context.DrawRectangle(fillBrush, borderPen, areaRect);
        }

        // Render from Value/Value2 if no explicit areas
        if (Areas.Count == 0)
        {
            Rect areaRect;
            if (IsVertical)
            {
                var px1 = xAxis.ValueToPixel(Value);
                var px2 = xAxis.ValueToPixel(Value2);
                areaRect = new Rect(Math.Min(px1, px2), plotArea.Top, Math.Abs(px2 - px1), plotArea.Height);
            }
            else
            {
                var py1 = yAxis.ValueToPixel(Value);
                var py2 = yAxis.ValueToPixel(Value2);
                areaRect = new Rect(plotArea.Left, Math.Min(py1, py2), plotArea.Width, Math.Abs(py2 - py1));
            }

            context.DrawRectangle(fillBrush, borderPen, areaRect);

            // Label
            if (!string.IsNullOrEmpty(Label))
            {
                var formattedText = new FormattedText(Label,
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Segoe UI", FontStyle.Normal, FontWeight.Normal),
                    10.0,
                    color);

                context.DrawText(formattedText,
                    new Point(areaRect.X + 4, areaRect.Y + 4));
            }
        }
    }
}
