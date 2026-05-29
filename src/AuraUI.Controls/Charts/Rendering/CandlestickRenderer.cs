using Avalonia;
using Avalonia.Media;
using AuraUI.Controls.Charts.Series;

namespace AuraUI.Controls.Charts.Rendering;

/// <summary>
/// Renders CandlestickSeries as OHLC candlestick bars. Each candle shows
/// open/high/low/close with colored bodies and thin wicks.
/// </summary>
public class CandlestickRenderer : IChartRenderer
{
    public string Key => "Candlestick";

    public void Render(
        DrawingContext context,
        ChartSeries series,
        Rect plotArea,
        ChartAxis? xAxis,
        ChartAxis? yAxis,
        double progress,
        IReadOnlyList<ChartSeries> allSeries,
        ChartRenderContext? renderContext = null)
    {
        if (series is not CandlestickSeries candle || !series.IsVisible) return;
        if (xAxis == null || yAxis == null) return;

        var data = candle.DataPoints;
        if (data.Count == 0) return;

        var upColor = candle.UpColor ?? new SolidColorBrush(Color.Parse("#26A69A"));
        var downColor = candle.DownColor ?? new SolidColorBrush(Color.Parse("#EF5350"));

        var categoryWidth = plotArea.Width / data.Count;
        var bodyWidth = candle.BodyWidth > 0
            ? candle.BodyWidth
            : categoryWidth * 0.6;

        for (int i = 0; i < data.Count; i++)
        {
            var d = data[i];
            var centerX = xAxis.ValueToPixel(d.X);
            var openY = yAxis.ValueToPixel(d.Open);
            var closeY = yAxis.ValueToPixel(d.Close);
            var highY = yAxis.ValueToPixel(d.High);
            var lowY = yAxis.ValueToPixel(d.Low);

            var isUp = d.Close >= d.Open;
            var candleBrush = isUp ? upColor : downColor;
            var candlePen = new Pen(candleBrush, 1);

            // Animate: scale from center
            var animatedBodyTop = isUp
                ? closeY + (openY - closeY) * (1 - progress)
                : openY + (closeY - openY) * (1 - progress);
            var animatedBodyBottom = isUp
                ? closeY
                : closeY;

            // Body
            var bodyRect = new Rect(
                centerX - bodyWidth / 2,
                Math.Min(openY, closeY),
                bodyWidth,
                Math.Abs(closeY - openY) * progress);

            context.DrawRectangle(candleBrush, null, bodyRect);

            // Wicks
            if (candle.ShowWick)
            {
                var wickPen = new Pen(candleBrush, candle.WickThickness);

                // Upper wick (to high)
                var bodyTop = Math.Min(openY, closeY);
                context.DrawLine(wickPen,
                    new Point(centerX, highY),
                    new Point(centerX, bodyTop));

                // Lower wick (to low)
                var bodyBottom = Math.Max(openY, closeY);
                context.DrawLine(wickPen,
                    new Point(centerX, bodyBottom),
                    new Point(centerX, lowY));
            }
        }
    }

    public ChartHitResult? HitTest(
        Point pointerPosition,
        ChartSeries series,
        Rect plotArea,
        ChartAxis? xAxis,
        ChartAxis? yAxis,
        IReadOnlyList<ChartSeries> allSeries)
    {
        if (series is not CandlestickSeries candle) return null;
        if (xAxis == null || yAxis == null) return null;

        var data = candle.DataPoints;
        if (data.Count == 0) return null;

        var categoryWidth = plotArea.Width / data.Count;
        var bodyWidth = candle.BodyWidth > 0 ? candle.BodyWidth : categoryWidth * 0.6;

        for (int i = 0; i < data.Count; i++)
        {
            var d = data[i];
            var centerX = xAxis.ValueToPixel(d.X);
            var bodyLeft = centerX - bodyWidth / 2;
            var bodyTop = Math.Min(yAxis.ValueToPixel(d.Open), yAxis.ValueToPixel(d.Close));
            var bodyBottom = Math.Max(yAxis.ValueToPixel(d.Open), yAxis.ValueToPixel(d.Close));

            // Expand hit area to include wicks
            var hitRect = new Rect(bodyLeft, yAxis.ValueToPixel(d.High),
                bodyWidth, yAxis.ValueToPixel(d.Low) - yAxis.ValueToPixel(d.High));

            if (hitRect.Contains(pointerPosition))
            {
                return new ChartHitResult
                {
                    Series = series,
                    DataIndex = i,
                    HitPosition = new Point(centerX, bodyTop)
                };
            }
        }

        return null;
    }
}
