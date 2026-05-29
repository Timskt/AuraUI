using Avalonia;
using Avalonia.Media;

namespace AuraUI.Controls.Charts.Rendering;

/// <summary>
/// Renders BarSeries. Handles grouped bars, stacked bars, and horizontal orientation.
///
/// Bar width calculation:
///   - Available width = plotArea.Width / dataPointCount
///   - Bar width = available width * 0.7 (30% gap)
///   - Grouped: each series gets (barWidth / groupCount)
///   - Stacked: each series occupies the full bar width
/// </summary>
public class BarRenderer : IChartRenderer
{
    public string Key => "Bar";

    public void Render(
        DrawingContext context,
        ChartSeries series,
        Rect plotArea,
        ChartAxis? xAxis,
        ChartAxis? yAxis,
        double progress,
        IReadOnlyList<ChartSeries> allSeries,
        ChartRenderContext? renderContext = null)
    {
        if (series is not Series.BarSeries bar || !series.IsVisible) return;
        if (xAxis == null || yAxis == null) return;

        var dataPoints = bar.DataPoints;
        if (dataPoints.Count == 0) return;

        var color = LineRenderer.ResolveColor(bar);
        var brush = new SolidColorBrush(((SolidColorBrush)color).Color)
        {
            Opacity = bar.Opacity
        };
        var radius = bar.BarRadius;

        // Calculate bar width and grouping
        var groupSeries = GetGroupedSeries(bar, allSeries);
        var groupIndex = groupSeries.IndexOf(bar);
        var groupCount = groupSeries.Count;
        if (groupIndex < 0) groupIndex = 0;

        var categoryWidth = plotArea.Width / dataPoints.Count;
        var totalBarWidth = bar.BarWidth > 0 ? bar.BarWidth : categoryWidth * 0.7;
        var groupBarWidth = totalBarWidth / groupCount;

        if (renderContext != null)
        {
            renderContext.Benchmark.BeginGeometryBuild();
        }

        for (int i = 0; i < dataPoints.Count; i++)
        {
            var dp = dataPoints[i];
            var dataValue = dp.Y * progress; // Animate from 0

            if (bar.IsHorizontal)
            {
                var x = plotArea.Left;
                var xEnd = xAxis.ValueToPixel(dataValue);
                var y = yAxis.ValueToPixel(dp.X);
                var barTop = y - totalBarWidth / 2 + groupIndex * groupBarWidth;
                var barRect = new Rect(x, barTop, xEnd - x, groupBarWidth);
                DrawRoundedRect(context, brush, barRect, radius);
            }
            else
            {
                var x = xAxis.ValueToPixel(dp.X);
                var yBase = yAxis.ValueToPixel(0);
                var yValue = yAxis.ValueToPixel(dataValue);

                var barLeft = x - totalBarWidth / 2 + groupIndex * groupBarWidth;
                var barTop = Math.Min(yBase, yValue);
                var barHeight = Math.Abs(yValue - yBase);
                var barRect = new Rect(barLeft, barTop, groupBarWidth, barHeight);

                DrawRoundedRect(context, brush, barRect, radius);
            }
        }

        if (renderContext != null)
        {
            renderContext.Benchmark.EndGeometryBuild();
            renderContext.Benchmark.RecordPointCounts(dataPoints.Count, 0);
        }
    }

    public ChartHitResult? HitTest(
        Point pointerPosition,
        ChartSeries series,
        Rect plotArea,
        ChartAxis? xAxis,
        ChartAxis? yAxis,
        IReadOnlyList<ChartSeries> allSeries,
        ChartRenderContext? renderContext = null)
    {
        if (series is not Series.BarSeries bar) return null;
        if (xAxis == null || yAxis == null) return null;

        var dataPoints = bar.DataPoints;
        if (dataPoints.Count == 0) return null;

        var groupSeries = GetGroupedSeries(bar, allSeries);
        var groupIndex = groupSeries.IndexOf(bar);
        var groupCount = groupSeries.Count;
        if (groupIndex < 0) groupIndex = 0;

        var categoryWidth = plotArea.Width / dataPoints.Count;
        var totalBarWidth = bar.BarWidth > 0 ? bar.BarWidth : categoryWidth * 0.7;
        var groupBarWidth = totalBarWidth / groupCount;

        for (int i = 0; i < dataPoints.Count; i++)
        {
            var dp = dataPoints[i];
            var x = xAxis.ValueToPixel(dp.X);
            var yBase = yAxis.ValueToPixel(0);
            var yValue = yAxis.ValueToPixel(dp.Y);
            var barLeft = x - totalBarWidth / 2 + groupIndex * groupBarWidth;
            var barTop = Math.Min(yBase, yValue);
            var barHeight = Math.Abs(yValue - yBase);
            var barRect = new Rect(barLeft, barTop, groupBarWidth, barHeight);

            if (barRect.Contains(pointerPosition))
            {
                return new ChartHitResult
                {
                    Series = series,
                    DataPoint = dp,
                    DataIndex = i,
                    HitPosition = new Point(barRect.Center.X, barTop)
                };
            }
        }

        return null;
    }

    private static void DrawRoundedRect(DrawingContext context, IBrush brush, Rect rect, double radius)
    {
        if (radius <= 0)
        {
            context.DrawRectangle(brush, null, rect);
        }
        else
        {
            context.DrawRectangle(brush, null, rect, radius, radius);
        }
    }

    private static List<Series.BarSeries> GetGroupedSeries(Series.BarSeries target, IReadOnlyList<ChartSeries> allSeries)
    {
        var result = new List<Series.BarSeries>();
        foreach (var s in allSeries)
        {
            if (s is Series.BarSeries bar && s.IsVisible)
            {
                if (string.Equals(bar.StackGroup, target.StackGroup, StringComparison.Ordinal)
                    || (bar.StackGroup == null && target.StackGroup == null))
                {
                    result.Add(bar);
                }
            }
        }
        return result;
    }
}
