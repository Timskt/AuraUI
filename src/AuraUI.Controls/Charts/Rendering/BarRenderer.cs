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
///   - Stacked: each series occupies the full bar width, accumulated vertically
///   - StackedPercent: bars normalized to 100%
///
/// New features:
///   - Stacked bar mode with proper accumulation across series
///   - Bar labels on top
///   - Border radius support
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

        var isStacked = bar.StackMode != StackMode.None;
        var categoryWidth = plotArea.Width / dataPoints.Count;
        var totalBarWidth = bar.BarWidth > 0 ? bar.BarWidth : categoryWidth * 0.7;
        var groupBarWidth = isStacked ? totalBarWidth : totalBarWidth / groupCount;

        // Pre-compute stacked offsets if in stacked mode
        double[]? stackedBottoms = null;
        double[]? stackedTotals = null;
        if (isStacked)
        {
            stackedBottoms = ComputeStackedOffsets(bar, allSeries, dataPoints.Count, isLower: true);
            stackedTotals = ComputeStackedOffsets(bar, allSeries, dataPoints.Count, isLower: false);
        }

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

                if (isStacked && stackedBottoms != null)
                {
                    // Stacked mode: each bar starts where the previous one ended
                    var stackBottom = stackedBottoms[i];
                    var yBase = yAxis.ValueToPixel(stackBottom);
                    var yValue = yAxis.ValueToPixel(stackBottom + Math.Abs(dataValue));

                    var barLeft = x - totalBarWidth / 2;
                    var barTop = Math.Min(yBase, yValue);
                    var barHeight = Math.Abs(yValue - yBase);
                    var barRect = new Rect(barLeft, barTop, groupBarWidth, barHeight);

                    DrawRoundedRect(context, brush, barRect, radius);
                }
                else
                {
                    // Grouped mode: bars side by side
                    var yBase = yAxis.ValueToPixel(0);
                    var yValue = yAxis.ValueToPixel(dataValue);

                    var barLeft = x - totalBarWidth / 2 + groupIndex * groupBarWidth;
                    var barTop = Math.Min(yBase, yValue);
                    var barHeight = Math.Abs(yValue - yBase);
                    var barRect = new Rect(barLeft, barTop, groupBarWidth, barHeight);

                    DrawRoundedRect(context, brush, barRect, radius);
                }
            }

            // Bar label on top
            if (bar.ShowBarLabels && progress >= 1.0)
            {
                var labelText = dataValue.ToString(bar.BarLabelFormat ?? "F1");
                var formattedText = new FormattedText(labelText,
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Segoe UI", FontStyle.Normal, FontWeight.Normal),
                    10.0,
                    Brushes.Gray);

                var labelX = xAxis.ValueToPixel(dp.X) - formattedText.Width / 2;
                double labelY;

                if (bar.IsHorizontal)
                {
                    labelY = yAxis.ValueToPixel(dp.X) - formattedText.Height - 2;
                }
                else if (isStacked && stackedTotals != null)
                {
                    // For stacked bars, place label at the top of the stack (only for the last series)
                    var isLastInStack = groupIndex == groupCount - 1;
                    if (isLastInStack)
                    {
                        labelY = yAxis.ValueToPixel(stackedTotals[i]) - formattedText.Height - 2;
                    }
                    else
                    {
                        labelY = 0; // Don't show label for non-top series in stack
                    }
                }
                else
                {
                    var yValue = yAxis.ValueToPixel(dataValue);
                    labelY = Math.Min(yAxis.ValueToPixel(0), yValue) - formattedText.Height - 2;
                }

                if (labelY > plotArea.Top - 2)
                {
                    context.DrawText(formattedText, new Point(labelX, labelY));
                }
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

        var isStacked = bar.StackMode != StackMode.None;
        var categoryWidth = plotArea.Width / dataPoints.Count;
        var totalBarWidth = bar.BarWidth > 0 ? bar.BarWidth : categoryWidth * 0.7;
        var groupBarWidth = isStacked ? totalBarWidth : totalBarWidth / groupCount;

        double[]? stackedBottoms = null;
        if (isStacked)
        {
            stackedBottoms = ComputeStackedOffsets(bar, allSeries, dataPoints.Count, isLower: true);
        }

        for (int i = 0; i < dataPoints.Count; i++)
        {
            var dp = dataPoints[i];
            var x = xAxis.ValueToPixel(dp.X);

            Rect barRect;
            if (isStacked && stackedBottoms != null)
            {
                var stackBottom = stackedBottoms[i];
                var yBase = yAxis.ValueToPixel(stackBottom);
                var yValue = yAxis.ValueToPixel(stackBottom + Math.Abs(dp.Y));
                var barLeft = x - totalBarWidth / 2;
                var barTop = Math.Min(yBase, yValue);
                var barHeight = Math.Abs(yValue - yBase);
                barRect = new Rect(barLeft, barTop, groupBarWidth, barHeight);
            }
            else
            {
                var yBase = yAxis.ValueToPixel(0);
                var yValue = yAxis.ValueToPixel(dp.Y);
                var barLeft = x - totalBarWidth / 2 + groupIndex * groupBarWidth;
                var barTop = Math.Min(yBase, yValue);
                var barHeight = Math.Abs(yValue - yBase);
                barRect = new Rect(barLeft, barTop, groupBarWidth, barHeight);
            }

            if (barRect.Contains(pointerPosition))
            {
                return new ChartHitResult
                {
                    Series = series,
                    DataPoint = dp,
                    DataIndex = i,
                    HitPosition = new Point(barRect.Center.X, barRect.Top)
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

    /// <summary>
    /// Compute cumulative stacked values for each data point index.
    /// When isLower=true, returns the bottom of each bar (sum of all series below).
    /// When isLower=false, returns the top of each bar (sum of all series including this one).
    /// </summary>
    private static double[] ComputeStackedOffsets(
        Series.BarSeries target,
        IReadOnlyList<ChartSeries> allSeries,
        int dataCount,
        bool isLower)
    {
        var offsets = new double[dataCount];
        var groupSeries = GetGroupedSeries(target, allSeries);

        foreach (var s in groupSeries)
        {
            if (ReferenceEquals(s, target))
            {
                if (!isLower) // When computing totals, add this series' values
                {
                    for (int i = 0; i < Math.Min(s.DataPoints.Count, dataCount); i++)
                        offsets[i] += Math.Abs(s.DataPoints[i].Y);
                }
                break; // For lower offsets, stop before adding this series
            }

            for (int i = 0; i < Math.Min(s.DataPoints.Count, dataCount); i++)
                offsets[i] += Math.Abs(s.DataPoints[i].Y);
        }

        return offsets;
    }
}
