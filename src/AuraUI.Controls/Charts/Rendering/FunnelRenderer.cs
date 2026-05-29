using Avalonia;
using Avalonia.Media;
using AuraUI.Controls.Charts.Series;

namespace AuraUI.Controls.Charts.Rendering;

/// <summary>
/// Renders FunnelSeries as trapezoidal segments stacked vertically.
/// Each segment width is proportional to its value relative to the maximum.
/// </summary>
public class FunnelRenderer : IChartRenderer
{
    public string Key => "Funnel";

    public void Render(
        DrawingContext context,
        ChartSeries series,
        Rect plotArea,
        ChartAxis? xAxis,
        ChartAxis? yAxis,
        double progress,
        IReadOnlyList<ChartSeries> allSeries)
    {
        if (series is not Series.FunnelSeries funnel || !series.IsVisible) return;

        var items = funnel.Items;
        if (items.Count == 0) return;

        var maxValue = items.Max(s => s.Value);
        if (maxValue <= 0) return;

        var segmentHeight = (plotArea.Height - funnel.Gap * (items.Count - 1)) / items.Count;
        var centerX = plotArea.Center.X;
        var topWidth = plotArea.Width * 0.9;
        var neckWidth = topWidth * funnel.NeckRatio;
        var availableWidth = topWidth - neckWidth;

        var currentY = plotArea.Top;

        for (int i = 0; i < items.Count; i++)
        {
            var item = items[i];
            var widthRatio = (item.Value / maxValue) * progress;
            var nextRatio = i < items.Count - 1
                ? (items[i + 1].Value / maxValue) * progress
                : funnel.NeckRatio * progress;

            var currentWidth = neckWidth + availableWidth * widthRatio;
            var nextWidth = neckWidth + availableWidth * nextRatio;

            var color = item.Color ?? GetFunnelColor(i);
            var brush = new SolidColorBrush(((SolidColorBrush)color).Color);

            // Build trapezoid
            var geometry = new PathGeometry();
            var figure = new PathFigure
            {
                StartPoint = new Point(centerX - currentWidth / 2, currentY),
                IsClosed = true
            };
            figure.Segments!.Add(new LineSegment { Point = new Point(centerX + currentWidth / 2, currentY) });
            figure.Segments!.Add(new LineSegment { Point = new Point(centerX + nextWidth / 2, currentY + segmentHeight) });
            figure.Segments!.Add(new LineSegment { Point = new Point(centerX - nextWidth / 2, currentY + segmentHeight) });
            geometry.Figures!.Add(figure);

            context.DrawGeometry(brush, null, geometry);

            // Labels
            if (funnel.ShowLabels && progress >= 1.0)
            {
                var labelText = item.Label ?? "";
                if (funnel.ShowValues)
                    labelText += $"  {item.Value}";

                if (!string.IsNullOrWhiteSpace(labelText))
                {
                    var formattedText = new FormattedText(labelText,
                        System.Globalization.CultureInfo.CurrentCulture,
                        FlowDirection.LeftToRight,
                        new Typeface("Segoe UI", FontStyle.Normal, FontWeight.Normal),
                        12.0,
                        Brushes.White);

                    var labelX = funnel.LabelPosition switch
                    {
                        FunnelLabelPosition.Left => plotArea.Left + 8,
                        FunnelLabelPosition.Right => plotArea.Right - formattedText.Width - 8,
                        _ => centerX - formattedText.Width / 2
                    };
                    var labelY = currentY + (segmentHeight - formattedText.Height) / 2;

                    context.DrawText(formattedText, new Point(labelX, labelY));
                }
            }

            currentY += segmentHeight + funnel.Gap;
        }
    }

    public ChartHitResult? HitTest(
        Point pointerPosition,
        ChartSeries series,
        Rect plotArea,
        ChartAxis? xAxis,
        ChartAxis? yAxis,
        IReadOnlyList<ChartSeries> allSeries)
    {
        if (series is not Series.FunnelSeries funnel) return null;
        if (funnel.Items.Count == 0) return null;

        var segmentHeight = (plotArea.Height - funnel.Gap * (funnel.Items.Count - 1)) / funnel.Items.Count;
        var currentY = plotArea.Top;

        for (int i = 0; i < funnel.Items.Count; i++)
        {
            if (pointerPosition.Y >= currentY && pointerPosition.Y < currentY + segmentHeight)
            {
                return new ChartHitResult
                {
                    Series = series,
                    SliceData = funnel.Items[i],
                    DataIndex = i,
                    HitPosition = pointerPosition
                };
            }
            currentY += segmentHeight + funnel.Gap;
        }

        return null;
    }

    private static IBrush GetFunnelColor(int index)
    {
        return new SolidColorBrush(LineRenderer.DefaultPalette[index % LineRenderer.DefaultPalette.Length]);
    }
}
