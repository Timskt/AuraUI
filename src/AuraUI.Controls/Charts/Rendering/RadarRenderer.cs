using Avalonia;
using Avalonia.Media;

namespace AuraUI.Controls.Charts.Rendering;

/// <summary>
/// Renders RadarSeries on a radial grid. The radar chart is a self-contained
/// unit that draws both the grid (concentric polygons) and the data polygons.
///
/// The radar axes are defined by the chart's XAxis.Categories array.
/// Each category maps to an angle around the center.
/// </summary>
public class RadarRenderer : IChartRenderer
{
    public string Key => "Radar";

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
        if (series is not Series.RadarSeries radar || !series.IsVisible) return;

        var dataItems = radar.DataItems;
        if (dataItems.Count == 0) return;

        // Determine axis count from first data item
        var axisCount = dataItems[0].Values.Length;
        if (axisCount < 3) return;

        var center = new Point(plotArea.Center.X, plotArea.Center.Y);
        var radius = Math.Min(plotArea.Width, plotArea.Height) / 2 - 30;

        // Find max value across all data items for normalization
        var maxValue = 0.0;
        foreach (var item in dataItems)
            foreach (var v in item.Values)
                if (v > maxValue) maxValue = v;
        if (maxValue <= 0) maxValue = 1;

        // Draw grid (concentric polygons at 20%, 40%, 60%, 80%, 100%)
        DrawRadarGrid(context, center, radius, axisCount, 5);

        // Draw axis labels if categories are available
        if (xAxis?.Categories != null && xAxis.Categories.Length >= axisCount)
        {
            DrawRadarLabels(context, center, radius, axisCount, xAxis.Categories);
        }

        // Draw each data polygon
        foreach (var item in dataItems)
        {
            if (item.Values.Length < axisCount) continue;

            var color = item.Color ?? LineRenderer.ResolveColor(radar);
            var brush = new SolidColorBrush(((SolidColorBrush)color).Color)
            {
                Opacity = radar.FillOpacity * progress
            };
            var pen = new Pen(color, 2.0);

            var geometry = new PathGeometry();
            var figure = new PathFigure { IsClosed = true };

            for (int i = 0; i < axisCount; i++)
            {
                var angle = (360.0 / axisCount) * i - 90; // Start at top
                var rad = angle * Math.PI / 180;
                var normalizedValue = (item.Values[i] / maxValue) * progress;
                var x = center.X + radius * normalizedValue * Math.Cos(rad);
                var y = center.Y + radius * normalizedValue * Math.Sin(rad);

                if (i == 0)
                    figure.StartPoint = new Point(x, y);
                else
                    figure.Segments!.Add(new LineSegment { Point = new Point(x, y) });
            }

            geometry.Figures!.Add(figure);
            context.DrawGeometry(brush, pen, geometry);

            // Draw markers
            if (radar.ShowMarkers && progress >= 1.0)
            {
                for (int i = 0; i < axisCount; i++)
                {
                    var angle = (360.0 / axisCount) * i - 90;
                    var rad = angle * Math.PI / 180;
                    var normalizedValue = item.Values[i] / maxValue;
                    var x = center.X + radius * normalizedValue * Math.Cos(rad);
                    var y = center.Y + radius * normalizedValue * Math.Sin(rad);

                    context.DrawEllipse(color, null, new Point(x, y), 4, 4);
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
        IReadOnlyList<ChartSeries> allSeries,
        ChartRenderContext? renderContext = null)
    {
        // Radar hit testing checks proximity to each vertex
        if (series is not Series.RadarSeries radar) return null;
        if (radar.DataItems.Count == 0) return null;

        var axisCount = radar.DataItems[0].Values.Length;
        var center = new Point(plotArea.Center.X, plotArea.Center.Y);
        var radius = Math.Min(plotArea.Width, plotArea.Height) / 2 - 30;

        var maxValue = 0.0;
        foreach (var item in radar.DataItems)
            foreach (var v in item.Values)
                if (v > maxValue) maxValue = v;
        if (maxValue <= 0) maxValue = 1;

        var hitRadius = 10.0;
        int bestItem = -1, bestAxis = -1;
        double bestDist = double.MaxValue;

        for (int itemIdx = 0; itemIdx < radar.DataItems.Count; itemIdx++)
        {
            var item = radar.DataItems[itemIdx];
            for (int i = 0; i < Math.Min(item.Values.Length, axisCount); i++)
            {
                var angle = (360.0 / axisCount) * i - 90;
                var rad = angle * Math.PI / 180;
                var nv = item.Values[i] / maxValue;
                var x = center.X + radius * nv * Math.Cos(rad);
                var y = center.Y + radius * nv * Math.Sin(rad);
                var dx = pointerPosition.X - x;
                var dy = pointerPosition.Y - y;
                var dist = Math.Sqrt(dx * dx + dy * dy);

                if (dist < hitRadius && dist < bestDist)
                {
                    bestDist = dist;
                    bestItem = itemIdx;
                    bestAxis = i;
                }
            }
        }

        if (bestItem < 0) return null;

        return new ChartHitResult
        {
            Series = series,
            DataIndex = bestItem,
            HitPosition = pointerPosition
        };
    }

    private static void DrawRadarGrid(DrawingContext context, Point center, double radius, int axisCount, int levels)
    {
        var gridPen = new Pen(Brushes.LightGray, 0.5);

        for (int level = 1; level <= levels; level++)
        {
            var levelRadius = radius * level / levels;
            var geometry = new PathGeometry();
            var figure = new PathFigure { IsClosed = true };

            for (int i = 0; i < axisCount; i++)
            {
                var angle = (360.0 / axisCount) * i - 90;
                var rad = angle * Math.PI / 180;
                var x = center.X + levelRadius * Math.Cos(rad);
                var y = center.Y + levelRadius * Math.Sin(rad);

                if (i == 0)
                    figure.StartPoint = new Point(x, y);
                else
                    figure.Segments!.Add(new LineSegment { Point = new Point(x, y) });
            }

            geometry.Figures!.Add(figure);
            context.DrawGeometry(null, gridPen, geometry);
        }

        // Draw axis lines
        for (int i = 0; i < axisCount; i++)
        {
            var angle = (360.0 / axisCount) * i - 90;
            var rad = angle * Math.PI / 180;
            var endPt = new Point(
                center.X + radius * Math.Cos(rad),
                center.Y + radius * Math.Sin(rad));
            context.DrawLine(gridPen, center, endPt);
        }
    }

    private static void DrawRadarLabels(DrawingContext context, Point center, double radius, int axisCount, string[] categories)
    {
        var labelBrush = Brushes.Gray;
        var labelFontSize = 11.0;
        var labelRadius = radius + 16; // Offset labels beyond the grid

        for (int i = 0; i < Math.Min(axisCount, categories.Length); i++)
        {
            if (string.IsNullOrEmpty(categories[i])) continue;

            var angle = (360.0 / axisCount) * i - 90;
            var rad = angle * Math.PI / 180;
            var x = center.X + labelRadius * Math.Cos(rad);
            var y = center.Y + labelRadius * Math.Sin(rad);

            var formattedText = new FormattedText(
                categories[i],
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface("Segoe UI", FontStyle.Normal, FontWeight.Normal),
                labelFontSize,
                labelBrush);

            // Center the label on the axis endpoint
            var textX = x - formattedText.Width / 2;
            var textY = y - formattedText.Height / 2;

            // Adjust alignment based on position around the circle
            var cosVal = Math.Cos(rad);
            if (cosVal < -0.3) textX = x - formattedText.Width;
            else if (cosVal > 0.3) textX = x;

            var sinVal = Math.Sin(rad);
            if (sinVal < -0.3) textY = y - formattedText.Height;
            else if (sinVal > 0.3) textY = y;

            context.DrawText(formattedText, new Point(textX, textY));
        }
    }
}
