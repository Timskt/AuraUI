using Avalonia;
using Avalonia.Media;
using AuraUI.Controls.Charts.Series;

namespace AuraUI.Controls.Charts.Rendering;

/// <summary>
/// Renders BoxplotSeries as box-and-whisker plots. Each box shows
/// the statistical distribution: min, Q1, median, Q3, max, and outliers.
/// </summary>
public class BoxplotRenderer : IChartRenderer
{
    public string Key => "Boxplot";

    public void Render(
        DrawingContext context,
        ChartSeries series,
        Rect plotArea,
        ChartAxis? xAxis,
        ChartAxis? yAxis,
        double progress,
        IReadOnlyList<ChartSeries> allSeries)
    {
        if (series is not BoxplotSeries boxplot || !series.IsVisible) return;
        if (xAxis == null || yAxis == null) return;

        var data = boxplot.BoxData;
        if (data.Count == 0) return;

        var color = LineRenderer.ResolveColor(boxplot);
        var fillBrush = new SolidColorBrush(((SolidColorBrush)color).Color)
        {
            Opacity = boxplot.FillOpacity
        };
        var strokePen = new Pen(color, 1.5);
        var whiskerPen = new Pen(color, boxplot.WhiskerWidth);

        var categoryWidth = plotArea.Width / data.Count;
        var boxWidth = categoryWidth * boxplot.BoxWidth;

        for (int i = 0; i < data.Count; i++)
        {
            var d = data[i];
            var centerX = xAxis.ValueToPixel(d.X);

            var minY = yAxis.ValueToPixel(d.Min);
            var q1Y = yAxis.ValueToPixel(d.Q1);
            var medianY = yAxis.ValueToPixel(d.Median);
            var q3Y = yAxis.ValueToPixel(d.Q3);
            var maxY = yAxis.ValueToPixel(d.Max);

            // Animate: scale from median
            if (progress < 1.0)
            {
                minY = medianY + (minY - medianY) * progress;
                q1Y = medianY + (q1Y - medianY) * progress;
                q3Y = medianY + (q3Y - medianY) * progress;
                maxY = medianY + (maxY - medianY) * progress;
            }

            var boxLeft = centerX - boxWidth / 2;
            var boxColor = d.Color ?? color;
            var boxFillBrush = new SolidColorBrush(((SolidColorBrush)boxColor).Color)
            {
                Opacity = boxplot.FillOpacity
            };
            var boxStrokePen = new Pen(boxColor, 1.5);

            // Box (Q1 to Q3)
            var boxRect = new Rect(boxLeft, Math.Min(q1Y, q3Y), boxWidth, Math.Abs(q3Y - q1Y));
            context.DrawRectangle(boxFillBrush, boxStrokePen, boxRect);

            // Median line
            context.DrawLine(new Pen(boxColor, 2.5),
                new Point(boxLeft, medianY),
                new Point(boxLeft + boxWidth, medianY));

            // Upper whisker (Q3 to Max)
            context.DrawLine(whiskerPen,
                new Point(centerX, Math.Min(q1Y, q3Y)),
                new Point(centerX, maxY));

            // Upper whisker cap
            context.DrawLine(whiskerPen,
                new Point(centerX - boxWidth * 0.3, maxY),
                new Point(centerX + boxWidth * 0.3, maxY));

            // Lower whisker (Q1 to Min)
            context.DrawLine(whiskerPen,
                new Point(centerX, Math.Max(q1Y, q3Y)),
                new Point(centerX, minY));

            // Lower whisker cap
            context.DrawLine(whiskerPen,
                new Point(centerX - boxWidth * 0.3, minY),
                new Point(centerX + boxWidth * 0.3, minY));

            // Outliers
            if (d.Outliers != null && d.Outliers.Length > 0 && progress >= 1.0)
            {
                foreach (var outlier in d.Outliers)
                {
                    var outlierY = yAxis.ValueToPixel(outlier);
                    context.DrawEllipse(boxColor, null,
                        new Point(centerX, outlierY),
                        boxplot.OutlierSize / 2, boxplot.OutlierSize / 2);
                }
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
        if (series is not BoxplotSeries boxplot) return null;
        if (xAxis == null || yAxis == null) return null;

        var data = boxplot.BoxData;
        if (data.Count == 0) return null;

        var categoryWidth = plotArea.Width / data.Count;
        var boxWidth = categoryWidth * boxplot.BoxWidth;

        for (int i = 0; i < data.Count; i++)
        {
            var d = data[i];
            var centerX = xAxis.ValueToPixel(d.X);
            var boxLeft = centerX - boxWidth / 2;
            var minY = yAxis.ValueToPixel(d.Min);
            var maxY = yAxis.ValueToPixel(d.Max);

            var hitRect = new Rect(boxLeft, Math.Min(minY, maxY), boxWidth, Math.Abs(maxY - minY));
            if (hitRect.Contains(pointerPosition))
            {
                return new ChartHitResult
                {
                    Series = series,
                    DataIndex = i,
                    HitPosition = new Point(centerX, yAxis.ValueToPixel(d.Median))
                };
            }
        }

        return null;
    }
}
