using Avalonia.Media;

namespace AuraUI.Controls.Charts;

/// <summary>
/// Pre-configured chart setups for common use cases.
/// Each preset returns a fully configured Chart instance ready for data binding.
///
/// Usage:
///   var chart = ChartPresets.DashboardLine();
///   chart.Title = "Sales Trend";
///   // Add series and data...
/// </summary>
public static class ChartPresets
{
    /// <summary>
    /// Dashboard line chart preset.
    /// Clean, minimal design with grid lines, smooth interpolation, and axis trigger tooltip.
    /// Ideal for time series, trends, and KPI tracking.
    /// </summary>
    public static Chart DashboardLine()
    {
        var chart = new Chart
        {
            TitleFontSize = 14,
            ChartPadding = new Avalonia.Thickness(16, 12, 16, 8),
            IsAnimated = true,
            AnimationDuration = TimeSpan.FromMilliseconds(600)
        };

        chart.XAxis.ShowLabels = true;
        chart.XAxis.ShowGridLines = false;
        chart.XAxis.ShowTicks = true;
        chart.XAxis.LabelFontSize = 10;

        chart.YAxis.ShowLabels = true;
        chart.YAxis.ShowGridLines = true;
        chart.YAxis.ShowTicks = false;
        chart.YAxis.LabelFontSize = 10;

        chart.Grid.ShowHorizontalLines = true;
        chart.Grid.ShowVerticalLines = false;
        chart.Grid.GridLineThickness = 0.5;

        chart.Tooltip.Trigger = TooltipTrigger.Axis;
        chart.Tooltip.ShowCrosshair = true;

        chart.Legend.IsVisible = true;
        chart.Legend.Position = LegendPosition.Bottom;

        return chart;
    }

    /// <summary>
    /// Dashboard bar chart preset.
    /// Grouped or stacked bars with rounded corners and optional labels.
    /// Ideal for comparisons, category breakdowns, and survey results.
    /// </summary>
    public static Chart DashboardBar()
    {
        var chart = new Chart
        {
            TitleFontSize = 14,
            ChartPadding = new Avalonia.Thickness(16, 12, 16, 8),
            IsAnimated = true,
            AnimationDuration = TimeSpan.FromMilliseconds(500)
        };

        chart.XAxis.ShowLabels = true;
        chart.XAxis.ShowGridLines = false;
        chart.XAxis.ShowTicks = true;
        chart.XAxis.LabelFontSize = 10;

        chart.YAxis.ShowLabels = true;
        chart.YAxis.ShowGridLines = true;
        chart.YAxis.ShowTicks = false;
        chart.YAxis.LabelFontSize = 10;

        chart.Grid.ShowHorizontalLines = true;
        chart.Grid.ShowVerticalLines = false;

        chart.Tooltip.Trigger = TooltipTrigger.Item;
        chart.Tooltip.ShowCrosshair = false;

        chart.Legend.IsVisible = true;
        chart.Legend.Position = LegendPosition.Bottom;

        return chart;
    }

    /// <summary>
    /// Dashboard pie chart preset.
    /// Standard pie/donut with labels, leader lines, and emphasis on hover.
    /// Ideal for composition, market share, and budget breakdowns.
    /// </summary>
    public static Chart DashboardPie()
    {
        var chart = new Chart
        {
            TitleFontSize = 14,
            ChartPadding = new Avalonia.Thickness(24, 12, 24, 8),
            IsAnimated = true,
            AnimationDuration = TimeSpan.FromMilliseconds(800)
        };

        chart.Tooltip.Trigger = TooltipTrigger.Item;
        chart.Tooltip.ShowCrosshair = false;

        chart.Legend.IsVisible = true;
        chart.Legend.Position = LegendPosition.Right;

        return chart;
    }

    /// <summary>
    /// Analytics scatter chart preset.
    /// Scatter plot with axis trigger tooltip and zoom/pan support.
    /// Ideal for correlation analysis, outlier detection, and clustering.
    /// </summary>
    public static Chart AnalyticsScatter()
    {
        var chart = new Chart
        {
            TitleFontSize = 14,
            ChartPadding = new Avalonia.Thickness(16, 12, 16, 8),
            IsAnimated = false // Scatter plots don't animate well
        };

        chart.XAxis.ShowLabels = true;
        chart.XAxis.ShowGridLines = true;
        chart.XAxis.ShowTicks = true;
        chart.XAxis.LabelFontSize = 10;

        chart.YAxis.ShowLabels = true;
        chart.YAxis.ShowGridLines = true;
        chart.YAxis.ShowTicks = true;
        chart.YAxis.LabelFontSize = 10;

        chart.Grid.ShowHorizontalLines = true;
        chart.Grid.ShowVerticalLines = true;
        chart.Grid.GridLineThickness = 0.3;

        chart.Tooltip.Trigger = TooltipTrigger.Item;
        chart.Tooltip.ShowCrosshair = true;

        chart.Legend.IsVisible = true;
        chart.Legend.Position = LegendPosition.Top;

        // Enable zoom/pan
        chart.DataZoom.IsVisible = true;
        chart.DataZoom.ZoomType = DataZoomType.Inside;

        return chart;
    }

    /// <summary>
    /// Financial candlestick chart preset.
    /// Candlestick chart with volume overlay and data zoom slider.
    /// Ideal for stock prices, trading data, and financial analysis.
    /// </summary>
    public static Chart FinancialCandlestick()
    {
        var chart = new Chart
        {
            TitleFontSize = 14,
            ChartPadding = new Avalonia.Thickness(16, 12, 16, 8),
            IsAnimated = false
        };

        chart.XAxis.ShowLabels = true;
        chart.XAxis.ShowGridLines = false;
        chart.XAxis.ShowTicks = true;
        chart.XAxis.LabelFontSize = 9;
        chart.XAxis.RotateLabels = true;
        chart.XAxis.LabelRotation = -45;

        chart.YAxis.ShowLabels = true;
        chart.YAxis.ShowGridLines = true;
        chart.YAxis.ShowTicks = false;
        chart.YAxis.LabelFontSize = 10;
        chart.YAxis.Title = "Price";

        chart.Grid.ShowHorizontalLines = true;
        chart.Grid.ShowVerticalLines = false;
        chart.Grid.GridLineThickness = 0.5;

        chart.Tooltip.Trigger = TooltipTrigger.Axis;
        chart.Tooltip.ShowCrosshair = true;
        chart.Tooltip.ValueFormat = "F2";

        chart.Legend.IsVisible = false;

        // Data zoom slider for time range selection
        chart.DataZoom.IsVisible = true;
        chart.DataZoom.ZoomType = DataZoomType.Slider;
        chart.DataZoom.Height = 30;

        return chart;
    }

    /// <summary>
    /// Statistical boxplot chart preset.
    /// Box-and-whisker plot with outlier display.
    /// Ideal for distribution analysis, quality control, and A/B testing.
    /// </summary>
    public static Chart StatisticalBoxplot()
    {
        var chart = new Chart
        {
            TitleFontSize = 14,
            ChartPadding = new Avalonia.Thickness(16, 12, 16, 8),
            IsAnimated = true,
            AnimationDuration = TimeSpan.FromMilliseconds(500)
        };

        chart.XAxis.ShowLabels = true;
        chart.XAxis.ShowGridLines = false;
        chart.XAxis.ShowTicks = true;
        chart.XAxis.LabelFontSize = 10;

        chart.YAxis.ShowLabels = true;
        chart.YAxis.ShowGridLines = true;
        chart.YAxis.ShowTicks = false;
        chart.YAxis.LabelFontSize = 10;

        chart.Grid.ShowHorizontalLines = true;
        chart.Grid.ShowVerticalLines = false;

        chart.Tooltip.Trigger = TooltipTrigger.Item;
        chart.Tooltip.ShowCrosshair = false;

        chart.Legend.IsVisible = false;

        return chart;
    }

    /// <summary>
    /// Multi-axis line chart preset.
    /// Two Y axes (left and right) with independent scales.
    /// Ideal for comparing metrics with different units (e.g., revenue vs. conversion rate).
    /// </summary>
    public static Chart MultiAxisLine()
    {
        var chart = DashboardLine();

        // Configure right axis
        chart.YAxisRight.ShowLabels = true;
        chart.YAxisRight.ShowAxisLine = true;
        chart.YAxisRight.ShowGridLines = false;
        chart.YAxisRight.LabelFontSize = 10;
        chart.YAxisRight.Title = "Secondary";

        return chart;
    }

    /// <summary>
    /// Real-time monitoring chart preset.
    /// Optimized for live data streaming with auto-scroll.
    /// Ideal for dashboards, monitoring, and live data visualization.
    /// </summary>
    public static Chart RealTimeMonitoring()
    {
        var chart = new Chart
        {
            TitleFontSize = 12,
            ChartPadding = new Avalonia.Thickness(12, 8, 12, 8),
            IsAnimated = false // No animation for real-time (avoids jank)
        };

        chart.XAxis.ShowLabels = true;
        chart.XAxis.ShowGridLines = false;
        chart.XAxis.ShowTicks = false;
        chart.XAxis.LabelFontSize = 9;

        chart.YAxis.ShowLabels = true;
        chart.YAxis.ShowGridLines = true;
        chart.YAxis.ShowTicks = false;
        chart.YAxis.LabelFontSize = 9;

        chart.Grid.ShowHorizontalLines = true;
        chart.Grid.ShowVerticalLines = false;
        chart.Grid.GridLineThickness = 0.3;

        chart.Tooltip.Trigger = TooltipTrigger.Axis;
        chart.Tooltip.ShowCrosshair = true;

        chart.Legend.IsVisible = false;

        return chart;
    }

    /// <summary>
    /// Funnel/pipeline chart preset.
    /// Vertical funnel with conversion rates between stages.
    /// Ideal for sales funnels, conversion tracking, and process analysis.
    /// </summary>
    public static Chart FunnelPipeline()
    {
        var chart = new Chart
        {
            TitleFontSize = 14,
            ChartPadding = new Avalonia.Thickness(24, 12, 24, 8),
            IsAnimated = true,
            AnimationDuration = TimeSpan.FromMilliseconds(800)
        };

        chart.Tooltip.Trigger = TooltipTrigger.Item;
        chart.Tooltip.ShowCrosshair = false;

        chart.Legend.IsVisible = false;

        return chart;
    }

    /// <summary>
    /// Heatmap chart preset.
    /// Grid-based heatmap with color gradient.
    /// Ideal for correlation matrices, activity calendars, and density visualization.
    /// </summary>
    public static Chart HeatmapGrid()
    {
        var chart = new Chart
        {
            TitleFontSize = 14,
            ChartPadding = new Avalonia.Thickness(16, 12, 16, 8),
            IsAnimated = true
        };

        chart.XAxis.ShowLabels = true;
        chart.XAxis.ShowGridLines = false;
        chart.XAxis.ShowTicks = false;
        chart.XAxis.LabelFontSize = 10;

        chart.YAxis.ShowLabels = true;
        chart.YAxis.ShowGridLines = false;
        chart.YAxis.ShowTicks = false;
        chart.YAxis.LabelFontSize = 10;

        chart.Grid.ShowHorizontalLines = false;
        chart.Grid.ShowVerticalLines = false;

        chart.Tooltip.Trigger = TooltipTrigger.Item;
        chart.Tooltip.ShowCrosshair = false;

        chart.Legend.IsVisible = true;
        chart.Legend.Position = LegendPosition.Right;

        return chart;
    }
}
