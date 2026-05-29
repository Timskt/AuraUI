using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using AuraUI.Controls.Charts;
using AuraUI.Controls.Charts.Series;

namespace AuraUI.Demo.Pages;

public class AreaChartPage : ComponentPageBase
{
    public override string ComponentName => "Area Chart";
    public override string Description => "An area chart for visualizing volume and trends with filled regions and smooth curves.";
    public override string Category => "Charts";

    protected override Control BuildContent()
    {
        var chart = new Chart { Width = 650, Height = 300, Title = "Website Traffic" };
        var areaSeries = new AreaSeries { Title = "Page Views", Color = new SolidColorBrush(Color.Parse("#0078D4")), AreaOpacity = 0.25, SmoothTension = 0.3 };
        var traffic = new double[] { 1200, 1350, 1100, 1450, 1600, 1800, 1750, 1900, 2100, 1950, 2200, 2400, 2300, 2500, 2650 };
        for (int i = 0; i < traffic.Length; i++)
            areaSeries.DataPoints.Add(new ChartDataPoint(i + 1, traffic[i], $"Day {i + 1}"));
        chart.XAxis.Title = "Day";
        chart.YAxis.Title = "Page Views";
        chart.YAxis.ShowGridLines = true;
        chart.Series.Add(areaSeries);

        return new StackPanel
        {
            Spacing = 32,
            Children =
            {
                CreateExampleSection("Website Traffic", chart,
                    @"<charts:Chart x:Name=""AreaChart"" Width=""650"" Height=""300""
              Title=""Website Traffic""/>",
                    @"var areaSeries = new AreaSeries
{
    Title = ""Page Views"",
    Color = new SolidColorBrush(Color.Parse(""#0078D4"")),
    AreaOpacity = 0.25,
    SmoothTension = 0.3
};

for (int i = 0; i < data.Length; i++)
    areaSeries.DataPoints.Add(new ChartDataPoint(i, data[i]));

chart.Series.Add(areaSeries);"),
                CreateGuidelines("Use area charts for showing volume changes over time. Stack multiple series for composition analysis.", "Use opacity to show overlapping areas. Smooth curves improve readability for dense data.")
            }
        };
    }
}
