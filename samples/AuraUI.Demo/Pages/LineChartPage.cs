using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using AuraUI.Controls.Charts;
using AuraUI.Controls.Charts.Series;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class LineChartPage : ComponentPageBase
{
    public override string ComponentName => "Line Chart";
    public override string Description => "A line chart for displaying trends over time with multiple series, markers, and smooth curves.";
    public override string Category => "Charts";

    protected override Control BuildContent()
    {
        var chart = new Chart { Width = 650, Height = 300, Title = "Monthly Sales" };
        var months = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
        var sales2024 = new double[] { 42, 48, 55, 52, 68, 75, 82, 78, 90, 95, 88, 102 };
        var sales2023 = new double[] { 35, 40, 45, 43, 55, 60, 65, 62, 72, 78, 70, 85 };

        var series2024 = new LineSeries { Title = "Sales 2024", Color = new SolidColorBrush(Color.Parse("#0078D4")), StrokeThickness = 2, ShowMarkers = true, MarkerShape = MarkerShape.Circle, MarkerSize = 5 };
        var series2023 = new LineSeries { Title = "Sales 2023", Color = new SolidColorBrush(Color.Parse("#107C10")), StrokeThickness = 2, ShowMarkers = true, MarkerShape = MarkerShape.Circle, MarkerSize = 4 };

        for (int i = 0; i < 12; i++)
        {
            series2024.DataPoints.Add(new ChartDataPoint(i, sales2024[i], months[i]));
            series2023.DataPoints.Add(new ChartDataPoint(i, sales2023[i], months[i]));
        }

        chart.XAxis.Categories = months;
        chart.XAxis.Scale = AxisScale.Category;
        chart.YAxis.Title = "Sales ($K)";
        chart.YAxis.ShowGridLines = true;
        chart.Series.Add(series2024);
        chart.Series.Add(series2023);

        return new StackPanel
        {
            Spacing = 32,
            Children =
            {
                CreateExampleSection("Monthly Sales (2 Series)", chart,
                    @"<charts:Chart x:Name=""LineChart"" Width=""650"" Height=""300""
              Title=""Monthly Sales""/>",
                    @"var series = new LineSeries
{
    Title = ""Sales 2024"",
    Color = new SolidColorBrush(Color.Parse(""#0078D4"")),
    StrokeThickness = 2,
    ShowMarkers = true,
    MarkerShape = MarkerShape.Circle,
    MarkerSize = 5
};

for (int i = 0; i < 12; i++)
    series.DataPoints.Add(new ChartDataPoint(i, values[i], months[i]));

chart.XAxis.Categories = months;
chart.XAxis.Scale = AxisScale.Category;
chart.YAxis.Title = ""Sales ($K)"";
chart.Series.Add(series);",
                    @"public partial class ChartViewModel : ViewModelBase
{
    public ObservableCollection<ChartDataPoint> Sales2024 { get; } = new();
    public ObservableCollection<ChartDataPoint> Sales2023 { get; } = new();

    public ChartViewModel()
    {
        var months = new[] { ""Jan"", ""Feb"", ""Mar"", ""Apr"",
                             ""May"", ""Jun"", ""Jul"", ""Aug"",
                             ""Sep"", ""Oct"", ""Nov"", ""Dec"" };
        var values = new double[] { 42, 48, 55, 52, 68, 75,
                                    82, 78, 90, 95, 88, 102 };

        for (int i = 0; i < 12; i++)
            Sales2024.Add(new ChartDataPoint(i, values[i], months[i]));
    }
}"),
                CreateApiTable(GetApiProperties()),
                CreateGuidelines(
                    "Use line charts for time-series data, trend analysis, and comparing multiple series over the same period.",
                    "Limit to 3-4 series for readability. Use different colors and dash styles for distinction. Always label axes.")
            }
        };
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Title", Type = "string", Default = "null", Description = "Chart title" },
        new ApiProperty { PropertyName = "Series", Type = "IList<ChartSeries>", Default = "[]", Description = "Data series" },
        new ApiProperty { PropertyName = "XAxis", Type = "ChartAxis", Default = "", Description = "X-axis configuration" },
        new ApiProperty { PropertyName = "YAxis", Type = "ChartAxis", Default = "", Description = "Y-axis configuration" },
    };
}
