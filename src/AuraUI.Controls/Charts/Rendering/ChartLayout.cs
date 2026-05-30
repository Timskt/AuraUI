using Avalonia;
using Avalonia.Collections;
using AuraUI.Controls.Charts.Interaction;

namespace AuraUI.Controls.Charts.Rendering;

/// <summary>
/// Computes chart layout: plot area rect, legend position, axis space
/// reservation, and data zoom slider space. Extracted from Chart.cs for
/// focused testing and reuse.
///
/// Layout pipeline (called from ArrangeOverride):
///   1. Start with full chart size minus padding
///   2. Reserve space for title
///   3. Reserve space for legend (top/bottom/left/right)
///   4. Reserve space for axes (labels + titles)
///   5. Reserve space for data zoom slider
///   6. Compute final plot area rect
///   7. Update axis PlotArea references
/// </summary>
internal static class ChartLayout
{
    // Layout constants (matching original Chart.cs values)
    private const double AxisLabelWidth = 50;
    private const double AxisLabelHeight = 30;
    private const double AxisTitleHeight = 20;

    /// <summary>
    /// Compute the plot area and legend layout rect for the chart.
    /// </summary>
    /// <param name="size">The final chart size (from ArrangeOverride).</param>
    /// <param name="padding">Chart padding (from ChartPadding property).</param>
    /// <param name="title">Chart title text (null if no title).</param>
    /// <param name="subtitle">Chart subtitle text (null if no subtitle).</param>
    /// <param name="titleFontSize">Title font size.</param>
    /// <param name="legend">Legend configuration.</param>
    /// <param name="xAxis">X axis configuration.</param>
    /// <param name="yAxis">Y axis configuration.</param>
    /// <param name="yAxisRight">Right Y axis configuration.</param>
    /// <param name="hasRightAxis">Whether the right Y axis is active.</param>
    /// <param name="dataZoom">DataZoom configuration.</param>
    /// <returns>The computed plot area rect.</returns>
    public static Rect ComputePlotArea(
        Size size,
        Thickness padding,
        string? title,
        string? subtitle,
        double titleFontSize,
        ChartLegend legend,
        ChartAxis xAxis,
        ChartAxis yAxis,
        ChartAxis yAxisRight,
        bool hasRightAxis,
        DataZoom dataZoom)
    {
        var left = padding.Left;
        var top = padding.Top;
        var right = size.Width - padding.Right;
        var bottom = size.Height - padding.Bottom;

        // Reserve space for title
        if (!string.IsNullOrEmpty(title))
            top += titleFontSize + 8;
        if (!string.IsNullOrEmpty(subtitle))
            top += titleFontSize - 1;

        // Reserve space for legend
        if (legend.IsVisible && legend.Position != LegendPosition.None)
        {
            switch (legend.Position)
            {
                case LegendPosition.Top:
                    legend.LayoutRect = new Rect(left, top, right - left, 24);
                    top += 28;
                    break;
                case LegendPosition.Bottom:
                    legend.LayoutRect = new Rect(left, bottom - 24, right - left, 24);
                    bottom -= 28;
                    break;
                case LegendPosition.Left:
                    legend.LayoutRect = new Rect(left, top, 100, bottom - top);
                    left += 104;
                    break;
                case LegendPosition.Right:
                    legend.LayoutRect = new Rect(right - 100, top, 100, bottom - top);
                    right -= 104;
                    break;
            }
        }

        // Reserve space for axes
        if (xAxis.ShowLabels)
            bottom -= AxisLabelHeight;
        if (!string.IsNullOrEmpty(xAxis.Title))
            bottom -= AxisTitleHeight;

        if (yAxis.ShowLabels)
            left += AxisLabelWidth;
        if (!string.IsNullOrEmpty(yAxis.Title))
            left += AxisTitleHeight;

        // Reserve space for right Y axis if active
        if (hasRightAxis)
        {
            if (yAxisRight.ShowLabels)
                right -= AxisLabelWidth;
            if (!string.IsNullOrEmpty(yAxisRight.Title))
                right -= AxisTitleHeight;
        }

        // Reserve space for DataZoom slider (only for Slider type, not Inside)
        if (dataZoom.IsVisible && dataZoom.ZoomType == DataZoomType.Slider)
        {
            bottom -= dataZoom.Height + 8; // 8px gap between plot and slider
        }

        return new Rect(left, top, right - left, bottom - top);
    }

    /// <summary>
    /// Update axis PlotArea references from the computed plot area.
    /// </summary>
    public static void UpdateAxisLayout(
        Rect plotArea,
        ChartAxis xAxis,
        ChartAxis yAxis,
        ChartAxis yAxisRight)
    {
        xAxis.PlotArea = plotArea;
        yAxis.PlotArea = plotArea;
        yAxisRight.PlotArea = plotArea;
    }

    /// <summary>
    /// Recompute axis ranges from series data. Returns whether the right Y axis is active.
    /// </summary>
    public static bool RecomputeAxes(
        AvaloniaList<ChartSeries> series,
        ChartAxis xAxis,
        ChartAxis yAxis,
        ChartAxis yAxisRight,
        Rect plotArea)
    {
        xAxis.ComputeAutoRange(series);
        yAxis.ComputeAutoRange(series);

        // Check if any series uses YAxisIndex > 0 (right axis)
        var hasRightAxis = false;
        for (int i = 0; i < series.Count; i++)
        {
            if (series[i] is XYChartSeries xy && xy.YAxisIndex > 0 && series[i].IsVisible)
            {
                hasRightAxis = true;
                break;
            }
        }

        if (hasRightAxis)
        {
            yAxisRight.PlotArea = plotArea;
            yAxisRight.ComputeAutoRange(series);
        }

        return hasRightAxis;
    }
}
