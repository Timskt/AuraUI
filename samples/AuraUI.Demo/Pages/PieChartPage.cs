using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using AuraUI.Controls.Charts;
using AuraUI.Controls.Charts.Series;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class PieChartPage : ComponentPageBase
{
    public override string ComponentName => "Pie Chart";
    public override string Description => "A pie/donut chart for displaying proportional data with labels and percentage values.";
    public override string Category => "Charts";

    protected override Control BuildContent()
    {
        var chart = new Chart { Width = 400, Height = 400, Title = "Market Share" };
        var pieSeries = new PieSeries { ShowLabels = true, ShowPercentage = true, InnerRadius = 0.35 };
        var slices = new[] {
            ("AuraUI", 35.0, "#0078D4"), ("Competitor A", 25.0, "#107C10"),
            ("Competitor B", 20.0, "#D83B01"), ("Competitor C", 12.0, "#5C2D91"), ("Others", 8.0, "#FFB900")
        };
        foreach (var (label, value, color) in slices)
            pieSeries.Slices.Add(new ChartSliceData(label, value) { Color = new SolidColorBrush(Color.Parse(color)) });
        chart.Series.Add(pieSeries);

        return new StackPanel
        {
            Spacing = 32,
            Children =
            {
                CreateExampleSection("Market Share (Donut)", chart,
                    @"<charts:Chart x:Name=""PieChart"" Width=""400"" Height=""400""
              Title=""Market Share""/>",
                    @"var pieSeries = new PieSeries
{
    ShowLabels = true,
    ShowPercentage = true,
    InnerRadius = 0.35  // donut style
};

pieSeries.Slices.Add(new ChartSliceData(""AuraUI"", 35)
{
    Color = new SolidColorBrush(Color.Parse(""#0078D4""))
});
// ... more slices

chart.Series.Add(pieSeries);"),
                CreateGuidelines(
                    "Use pie charts for showing parts of a whole. Donut charts are preferred for a modern look.",
                    "Limit to 5-7 slices. Use 'Others' for small values. Always show percentages or values.")
            }
        };
    }
}
