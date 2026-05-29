using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using AuraUI.Controls.Charts;
using AuraUI.Controls.Charts.Series;

namespace AuraUI.Demo.Pages;

public class CandlestickChartPage : ComponentPageBase
{
    public override string ComponentName => "Candlestick Chart";
    public override string Description => "A candlestick chart for financial data showing open, high, low, and close prices.";
    public override string Category => "Charts";

    protected override Control BuildContent()
    {
        var chart = new Chart { Width = 650, Height = 300, Title = "Stock Prices" };
        var candlestickSeries = new CandlestickSeries { UpColor = new SolidColorBrush(Color.Parse("#107C10")), DownColor = new SolidColorBrush(Color.Parse("#D83B01")), WickThickness = 1, ShowWick = true };
        var ohlcData = new (double o, double h, double l, double c)[]
        {
            (150, 155, 148, 153), (153, 158, 151, 156), (156, 157, 149, 150), (150, 154, 147, 152),
            (152, 160, 151, 159), (159, 162, 155, 157), (157, 158, 150, 151), (151, 156, 149, 155),
            (155, 163, 154, 161), (161, 165, 158, 160), (160, 162, 154, 155), (155, 159, 152, 158),
            (158, 166, 157, 164), (164, 168, 162, 163), (163, 165, 157, 158)
        };
        for (int i = 0; i < ohlcData.Length; i++)
        {
            var (o, h, l, c) = ohlcData[i];
            candlestickSeries.DataPoints.Add(new OhlcDataPoint(i, o, h, l, c) { Label = $"Day {i + 1}" });
        }
        chart.XAxis.Title = "Trading Day";
        chart.YAxis.Title = "Price ($)";
        chart.YAxis.ShowGridLines = true;
        chart.Series.Add(candlestickSeries);

        return new StackPanel
        {
            Spacing = 32,
            Children =
            {
                CreateExampleSection("Stock Prices", chart,
                    @"<charts:Chart x:Name=""CandlestickChart"" Width=""650"" Height=""300""
              Title=""Stock Prices""/>",
                    @"var candlestickSeries = new CandlestickSeries
{
    UpColor = new SolidColorBrush(Color.Parse(""#107C10"")),
    DownColor = new SolidColorBrush(Color.Parse(""#D83B01"")),
    WickThickness = 1
};

for (int i = 0; i < data.Length; i++)
    candlestickSeries.DataPoints.Add(
        new OhlcDataPoint(i, open, high, low, close));

chart.Series.Add(candlestickSeries);"),
                CreateGuidelines("Use candlestick charts for stock price analysis, forex trading, and any OHLC data.", "Green for up days, red for down days. Show tooltips with exact OHLC values. Include volume bars below.")
            }
        };
    }
}
