using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using AuraUI.Controls.Charts;
using AuraUI.Controls.Charts.Series;

namespace AuraUI.Demo.Pages;

public class GaugeChartPage : ComponentPageBase
{
    public override string ComponentName => "Gauge Chart";
    public override string Description => "A gauge chart for displaying a single value within a range, with color segments and tick marks.";
    public override string Category => "Charts";

    protected override Control BuildContent()
    {
        var chart = new Chart { Width = 300, Height = 300, Title = "Performance Score" };
        var gaugeSeries = new GaugeSeries
        {
            Value = 78, Minimum = 0, Maximum = 100,
            Color = new SolidColorBrush(Color.Parse("#107C10")),
            TrackColor = new SolidColorBrush(Color.Parse("#E0E0E0")),
            StrokeWidth = 14, ShowCenterLabel = true, ShowTickMarks = true, TickCount = 10,
            LabelFormat = "{0:F0}%", Mode = GaugeMode.ThreeQuarter,
            Segments = new GaugeSegment[]
            {
                new() { From = 0, To = 50, Color = new SolidColorBrush(Color.Parse("#D83B01")) },
                new() { From = 50, To = 80, Color = new SolidColorBrush(Color.Parse("#FFB900")) },
                new() { From = 80, To = 100, Color = new SolidColorBrush(Color.Parse("#107C10")) }
            }
        };
        chart.Series.Add(gaugeSeries);

        return new StackPanel
        {
            Spacing = 32,
            Children =
            {
                CreateExampleSection("Performance Score", chart,
                    @"<charts:Chart x:Name=""GaugeChart"" Width=""300"" Height=""300""
              Title=""Performance Score""/>",
                    @"var gaugeSeries = new GaugeSeries
{
    Value = 78, Minimum = 0, Maximum = 100,
    StrokeWidth = 14,
    ShowCenterLabel = true,
    ShowTickMarks = true,
    Mode = GaugeMode.ThreeQuarter,
    Segments = new GaugeSegment[]
    {
        new() { From = 0, To = 50, Color = red },
        new() { From = 50, To = 80, Color = yellow },
        new() { From = 80, To = 100, Color = green }
    }
};
chart.Series.Add(gaugeSeries);"),
                CreateGuidelines("Use gauge charts for displaying KPIs, scores, and single metrics within a defined range.", "Use color segments for qualitative ranges. Show the numeric value in the center. Use ThreeQuarter mode for compact display.")
            }
        };
    }
}
