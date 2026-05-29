using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using AuraUI.Controls.Charts;
using AuraUI.Controls.Charts.Series;

namespace AuraUI.Demo.Pages;

public class HeatmapPage : ComponentPageBase
{
    public override string ComponentName => "Heatmap";
    public override string Description => "A heatmap chart for visualizing data density across two dimensions with color gradients.";
    public override string Category => "Charts";

    protected override Control BuildContent()
    {
        var chart = new Chart { Width = 650, Height = 300, Title = "Weekly Activity" };
        var heatmapSeries = new HeatmapSeries { ShowLabels = true, CellGap = 2, MinColor = Color.Parse("#f7fbff"), MaxColor = Color.Parse("#08519c") };
        heatmapSeries.XLabels = new[] { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };
        heatmapSeries.YLabels = new[] { "9 AM", "12 PM", "3 PM", "6 PM", "9 PM" };
        var rng = new Random(42);
        for (int y = 0; y < 5; y++)
            for (int x = 0; x < 7; x++)
            {
                var baseValue = x < 5 ? 60 : 20;
                var timeBonus = y is 1 or 2 ? 30 : 0;
                heatmapSeries.DataPoints.Add(new ChartHeatmapData(x, y, Math.Clamp(baseValue + timeBonus + rng.Next(-15, 16), 0, 100)));
            }
        chart.XAxis.Categories = heatmapSeries.XLabels;
        chart.XAxis.Scale = AxisScale.Category;
        chart.Series.Add(heatmapSeries);

        return new StackPanel
        {
            Spacing = 32,
            Children =
            {
                CreateExampleSection("Weekly Activity", chart,
                    @"<charts:Chart x:Name=""HeatmapChart"" Width=""650"" Height=""300""
              Title=""Weekly Activity""/>",
                    @"var heatmapSeries = new HeatmapSeries
{
    ShowLabels = true,
    CellGap = 2,
    MinColor = Color.Parse(""#f7fbff""),
    MaxColor = Color.Parse(""#08519c"")
};
heatmapSeries.XLabels = new[] { ""Mon"", ""Tue"", ... };
heatmapSeries.YLabels = new[] { ""9 AM"", ""12 PM"", ... };

for (int y = 0; y < rows; y++)
    for (int x = 0; x < cols; x++)
        heatmapSeries.DataPoints.Add(new ChartHeatmapData(x, y, value));

chart.Series.Add(heatmapSeries);"),
                CreateGuidelines("Use heatmaps for visualizing activity patterns, density distributions, and correlation matrices.", "Use intuitive color scales (light to dark). Show labels on both axes. Include a legend for the color scale.")
            }
        };
    }
}
