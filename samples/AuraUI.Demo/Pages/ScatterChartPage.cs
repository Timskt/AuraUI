using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using AuraUI.Controls.Charts;
using AuraUI.Controls.Charts.Series;

namespace AuraUI.Demo.Pages;

public class ScatterChartPage : ComponentPageBase
{
    public override string ComponentName => "Scatter Chart";
    public override string Description => "A scatter plot for visualizing correlations and distributions between two variables.";
    public override string Category => "Charts";

    protected override Control BuildContent()
    {
        var chart = new Chart { Width = 650, Height = 300, Title = "Height vs Weight" };
        var scatterSeries = new ScatterSeries { Title = "Subjects", Color = new SolidColorBrush(Color.Parse("#E3008C")), MarkerSize = 7, FillOpacity = 0.7, MarkerShape = MarkerShape.Circle };
        var rng = new System.Random(42);
        for (int i = 0; i < 50; i++)
        {
            var height = 150 + rng.NextDouble() * 45;
            var weight = height * 0.6 + rng.NextDouble() * 30 - 15;
            scatterSeries.DataPoints.Add(new ChartDataPoint(System.Math.Round(height, 1), System.Math.Round(weight, 1)));
        }
        chart.XAxis.Title = "Height (cm)";
        chart.YAxis.Title = "Weight (kg)";
        chart.YAxis.ShowGridLines = true;
        chart.Series.Add(scatterSeries);

        return new StackPanel
        {
            Spacing = 32,
            Children =
            {
                CreateExampleSection("Height vs Weight Correlation", chart,
                    @"<charts:Chart x:Name=""ScatterChart"" Width=""650"" Height=""300""
              Title=""Height vs Weight""/>",
                    @"var scatterSeries = new ScatterSeries
{
    Title = ""Subjects"",
    Color = new SolidColorBrush(Color.Parse(""#E3008C"")),
    MarkerSize = 7,
    FillOpacity = 0.7
};

for (int i = 0; i < data.Length; i++)
    scatterSeries.DataPoints.Add(new ChartDataPoint(x, y));

chart.Series.Add(scatterSeries);"),
                CreateGuidelines("Use scatter charts for correlation analysis, outlier detection, and distribution visualization.", "Use different colors for categories. Add trend lines for correlation. Consider bubble charts for 3-variable data.")
            }
        };
    }
}
