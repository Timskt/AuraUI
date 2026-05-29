using Avalonia;
using Avalonia.Media;
using AuraUI.Controls.Charts.Series;

namespace AuraUI.Controls.Charts.Rendering;

/// <summary>
/// Renders HeatmapSeries as a grid of colored cells. Each cell color is
/// interpolated between MinColor and MaxColor based on its value.
/// </summary>
public class HeatmapRenderer : IChartRenderer
{
    public string Key => "Heatmap";

    public void Render(
        DrawingContext context,
        ChartSeries series,
        Rect plotArea,
        ChartAxis? xAxis,
        ChartAxis? yAxis,
        double progress,
        IReadOnlyList<ChartSeries> allSeries)
    {
        if (series is not HeatmapSeries heatmap || !series.IsVisible) return;

        var data = heatmap.DataPoints;
        if (data.Count == 0) return;

        // Determine grid dimensions
        int maxX = 0, maxY = 0;
        double minVal = double.MaxValue, maxVal = double.MinValue;
        foreach (var d in data)
        {
            if (d.X > maxX) maxX = d.X;
            if (d.Y > maxY) maxY = d.Y;
            if (d.Value < minVal) minVal = d.Value;
            if (d.Value > maxVal) maxVal = d.Value;
        }

        int cols = maxX + 1;
        int rows = maxY + 1;
        if (cols <= 0 || rows <= 0) return;

        var valRange = maxVal - minVal;
        if (Math.Abs(valRange) < 1e-10) valRange = 1;

        var cellWidth = (plotArea.Width - heatmap.CellGap * (cols - 1)) / cols;
        var cellHeight = (plotArea.Height - heatmap.CellGap * (rows - 1)) / rows;

        foreach (var d in data)
        {
            var normalized = (d.Value - minVal) / valRange * progress;
            var color = InterpolateColor(heatmap.MinColor, heatmap.MaxColor, normalized);
            var brush = new SolidColorBrush(color);

            var x = plotArea.Left + d.X * (cellWidth + heatmap.CellGap);
            var y = plotArea.Top + d.Y * (cellHeight + heatmap.CellGap);
            var cellRect = new Rect(x, y, cellWidth, cellHeight);

            context.DrawRectangle(brush, null, cellRect);

            if (heatmap.ShowLabels && progress >= 1.0)
            {
                var label = d.Label ?? d.Value.ToString("F1");
                var formattedText = new FormattedText(label,
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Segoe UI", FontStyle.Normal, FontWeight.Normal),
                    Math.Min(cellHeight * 0.4, 12),
                    normalized > 0.5 ? Brushes.White : Brushes.Black);

                context.DrawText(formattedText,
                    new Point(x + (cellWidth - formattedText.Width) / 2,
                              y + (cellHeight - formattedText.Height) / 2));
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
        if (series is not HeatmapSeries heatmap) return null;
        if (heatmap.DataPoints.Count == 0) return null;

        int maxX = 0, maxY = 0;
        foreach (var d in heatmap.DataPoints)
        {
            if (d.X > maxX) maxX = d.X;
            if (d.Y > maxY) maxY = d.Y;
        }

        int cols = maxX + 1;
        int rows = maxY + 1;
        var cellWidth = (plotArea.Width - heatmap.CellGap * (cols - 1)) / cols;
        var cellHeight = (plotArea.Height - heatmap.CellGap * (rows - 1)) / rows;

        var relX = pointerPosition.X - plotArea.Left;
        var relY = pointerPosition.Y - plotArea.Top;
        int col = (int)(relX / (cellWidth + heatmap.CellGap));
        int row = (int)(relY / (cellHeight + heatmap.CellGap));

        if (col < 0 || col >= cols || row < 0 || row >= rows) return null;

        foreach (var d in heatmap.DataPoints)
        {
            if (d.X == col && d.Y == row)
            {
                return new ChartHitResult
                {
                    Series = series,
                    DataIndex = heatmap.DataPoints.IndexOf(d),
                    HitPosition = pointerPosition
                };
            }
        }

        return null;
    }

    private static Color InterpolateColor(Color min, Color max, double t)
    {
        t = Math.Clamp(t, 0, 1);
        return Color.FromArgb(
            255,
            (byte)(min.R + (max.R - min.R) * t),
            (byte)(min.G + (max.G - min.G) * t),
            (byte)(min.B + (max.B - min.B) * t));
    }
}
