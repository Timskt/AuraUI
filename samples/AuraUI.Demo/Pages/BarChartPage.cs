using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using AuraUI.Controls.Charts;
using AuraUI.Controls.Charts.Series;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class BarChartPage : ComponentPageBase
{
    public override string ComponentName => "Bar Chart";
    public override string Description => "A bar chart for comparing categorical data with rounded corners and customizable colors.";
    public override string Category => "Charts";

    protected override Control BuildContent()
    {
        var chart = new Chart { Width = 650, Height = 300, Title = "Revenue by Category" };
        var categories = new[] { "Electronics", "Clothing", "Food", "Books", "Sports", "Home" };
        var revenues = new double[] { 245, 180, 320, 95, 150, 210 };

        var barSeries = new BarSeries { Title = "Revenue", Color = new SolidColorBrush(Color.Parse("#5C2D91")), BarRadius = 4 };
        for (int i = 0; i < categories.Length; i++)
            barSeries.DataPoints.Add(new ChartDataPoint(i, revenues[i], categories[i]));

        chart.XAxis.Categories = categories;
        chart.XAxis.Scale = AxisScale.Category;
        chart.YAxis.Title = "Revenue ($K)";
        chart.YAxis.ShowGridLines = true;
        chart.Series.Add(barSeries);

        return new StackPanel
        {
            Spacing = 32,
            Children =
            {
                CreateExampleSection("Revenue by Category", chart,
                    @"<charts:Chart x:Name=""BarChart"" Width=""650"" Height=""300""
              Title=""Revenue by Category""/>",
                    @"var barSeries = new BarSeries
{
    Title = ""Revenue"",
    Color = new SolidColorBrush(Color.Parse(""#5C2D91"")),
    BarRadius = 4
};

for (int i = 0; i < categories.Length; i++)
    barSeries.DataPoints.Add(new ChartDataPoint(i, values[i], categories[i]));

chart.XAxis.Categories = categories;
chart.Series.Add(barSeries);"),
                CreateGuidelines(
                    "Use bar charts for comparing values across categories. Horizontal bars work well for long category names.",
                    "Sort bars by value for easier comparison. Use consistent colors within a single series. Label axes clearly.")
            }
        };
    }
}
