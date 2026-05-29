using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using AuraUI.Controls.Charts;
using AuraUI.Controls.Charts.Series;

namespace AuraUI.Demo.Pages;

public class RadarChartPage : ComponentPageBase
{
    public override string ComponentName => "Radar Chart";
    public override string Description => "A radar/spider chart for comparing multiple quantitative variables across categories.";
    public override string Category => "Charts";

    protected override Control BuildContent()
    {
        var chart = new Chart { Width = 400, Height = 400, Title = "Skill Assessment" };
        var radarSeries = new RadarSeries { Title = "Current", FillOpacity = 0.2, ShowMarkers = true };
        radarSeries.DataItems.Add(new ChartRadarData { Label = "Current", Values = new double[] { 85, 90, 70, 80, 75, 88 }, Color = new SolidColorBrush(Color.Parse("#0078D4")) });
        var radarSeries2 = new RadarSeries { Title = "Target", FillOpacity = 0.15, ShowMarkers = true };
        radarSeries2.DataItems.Add(new ChartRadarData { Label = "Target", Values = new double[] { 95, 95, 90, 90, 85, 95 }, Color = new SolidColorBrush(Color.Parse("#107C10")) });
        chart.XAxis.Categories = new[] { "Communication", "Technical", "Leadership", "Problem Solving", "Creativity", "Teamwork" };
        chart.Series.Add(radarSeries);
        chart.Series.Add(radarSeries2);

        return new StackPanel
        {
            Spacing = 32,
            Children =
            {
                CreateExampleSection("Skill Assessment", chart,
                    @"<charts:Chart x:Name=""RadarChart"" Width=""400"" Height=""400""
              Title=""Skill Assessment""/>",
                    @"var radarSeries = new RadarSeries { Title = ""Current"", FillOpacity = 0.2 };
radarSeries.DataItems.Add(new ChartRadarData
{
    Label = ""Current"",
    Values = new double[] { 85, 90, 70, 80, 75, 88 },
    Color = new SolidColorBrush(Color.Parse(""#0078D4""))
});

chart.XAxis.Categories = new[] { ""Communication"", ""Technical"", ... };
chart.Series.Add(radarSeries);"),
                CreateGuidelines("Use radar charts for comparing performance across multiple dimensions. They work well for skill assessments and feature comparisons.", "Limit to 6-8 axes. Use 2-3 overlapping series maximum. Fill areas with low opacity for readability.")
            }
        };
    }
}
