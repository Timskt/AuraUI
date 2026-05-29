using Avalonia;
using Avalonia.Media;

namespace AuraUI.Controls.Charts.Rendering;

/// <summary>
/// Renders ThemeRiverSeries as stacked areas centered around a horizontal axis.
///
/// Algorithm:
///   1. Group data by X position (time axis)
///   2. For each X, compute stacked values for all categories
///   3. Center the stack around Y=0 (half above, half below)
///   4. Build filled PathGeometry for each category layer
///   5. Draw layers from bottom to top
///
/// Rendering uses StreamGeometry for performance with optional smooth interpolation.
/// </summary>
public class ThemeRiverRenderer : IChartRenderer
{
    public string Key => "ThemeRiver";

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
        if (series is not Series.ThemeRiverSeries river || !series.IsVisible) return;

        var dataItems = river.DataItems;
        if (dataItems.Count == 0) return;

        // Get categories
        var categories = river.Categories;
        if (categories == null || categories.Length == 0)
        {
            categories = dataItems.Select(d => d.Category ?? "").Distinct().ToArray();
        }
        if (categories.Length == 0) return;

        // Group data by X, then by category
        var xValues = dataItems.Select(d => d.X).Distinct().OrderBy(x => x).ToArray();
        if (xValues.Length < 2) return;

        // Build value matrix: [xIndex][catIndex] = value
        var valueMatrix = new double[xValues.Length][];
        for (int xi = 0; xi < xValues.Length; xi++)
        {
            valueMatrix[xi] = new double[categories.Length];
            var xData = dataItems.Where(d => Math.Abs(d.X - xValues[xi]) < 1e-10);
            foreach (var d in xData)
            {
                var catIdx = Array.IndexOf(categories, d.Category ?? "");
                if (catIdx >= 0)
                    valueMatrix[xi][catIdx] = d.Value;
            }
        }

        // Compute total at each X for centering
        var totals = new double[xValues.Length];
        for (int xi = 0; xi < xValues.Length; xi++)
            totals[xi] = valueMatrix[xi].Sum();

        // Map X values to pixel positions
        var xMin = xValues[0];
        var xMax = xValues[^1];
        var xRange = xMax - xMin;
        if (Math.Abs(xRange) < 1e-10) xRange = 1;

        var xPixels = new double[xValues.Length];
        for (int xi = 0; xi < xValues.Length; xi++)
        {
            var t = (xValues[xi] - xMin) / xRange;
            xPixels[xi] = plotArea.Left + t * plotArea.Width;
        }

        // Find max total for Y scaling
        var maxTotal = totals.Max();
        if (maxTotal <= 0) maxTotal = 1;
        var yScale = plotArea.Height / (maxTotal * 2); // Centered, so half above and half below

        var centerY = plotArea.Center.Y;

        // Draw each category layer
        for (int catIdx = 0; catIdx < categories.Length; catIdx++)
        {
            var color = GetLayerColor(catIdx, river, categories[catIdx]);
            var brush = new SolidColorBrush(((SolidColorBrush)color).Color)
            {
                Opacity = river.LayerOpacity * progress
            };

            // Compute top and bottom edges for this layer
            var topPoints = new Point[xValues.Length];
            var bottomPoints = new Point[xValues.Length];

            for (int xi = 0; xi < xValues.Length; xi++)
            {
                // Sum values below this layer (for stacking)
                var belowSum = 0.0;
                for (int ci = 0; ci < catIdx; ci++)
                    belowSum += valueMatrix[xi][ci];

                var thisValue = valueMatrix[xi][catIdx];
                var totalBelowCenter = totals[xi] / 2;

                // Center: offset so the stack is centered at centerY
                var bottomOffset = belowSum - totalBelowCenter;
                var topOffset = belowSum + thisValue - totalBelowCenter;

                var bottomY = centerY - bottomOffset * yScale;
                var topY = centerY - topOffset * yScale;

                // Apply animation
                bottomY = centerY + (bottomY - centerY) * progress;
                topY = centerY + (topY - centerY) * progress;

                topPoints[xi] = new Point(xPixels[xi], topY);
                bottomPoints[xi] = new Point(xPixels[xi], bottomY);
            }

            // Build filled path: top edge forward, bottom edge backward
            var geometry = new StreamGeometry();
            using (var ctx = geometry.Open())
            {
                ctx.BeginFigure(topPoints[0], true);

                if (river.Smooth && topPoints.Length >= 3)
                {
                    AddSmoothSegments(ctx, topPoints);
                }
                else
                {
                    for (int i = 1; i < topPoints.Length; i++)
                        ctx.LineTo(topPoints[i]);
                }

                // Bottom edge in reverse
                for (int i = bottomPoints.Length - 1; i >= 0; i--)
                    ctx.LineTo(bottomPoints[i]);

                ctx.EndFigure(true);
            }

            context.DrawGeometry(brush, null, geometry);

            // Draw label
            if (river.ShowLabels && progress >= 1.0)
            {
                var midIdx = xValues.Length / 2;
                var midTop = topPoints[midIdx];
                var midBottom = bottomPoints[midIdx];
                var labelY = (midTop.Y + midBottom.Y) / 2;
                var labelX = (midTop.X + midBottom.X) / 2;

                var formattedText = new FormattedText(categories[catIdx],
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Segoe UI", FontStyle.Normal, FontWeight.Normal),
                    river.LabelFontSize,
                    Brushes.White);

                if (formattedText.Height < Math.Abs(midTop.Y - midBottom.Y))
                {
                    context.DrawText(formattedText,
                        new Point(labelX - formattedText.Width / 2, labelY - formattedText.Height / 2));
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
        // Theme river hit testing is complex; return null for now
        return null;
    }

    private static void AddSmoothSegments(StreamGeometryContext ctx, Point[] points)
    {
        for (int i = 0; i < points.Length - 1; i++)
        {
            var p0 = i > 0 ? points[i - 1] : points[i];
            var p1 = points[i];
            var p2 = points[i + 1];
            var p3 = i + 2 < points.Length ? points[i + 2] : points[i + 1];

            var alpha = 0.5;
            var cp1 = new Point(
                p1.X + (p2.X - p0.X) * alpha / 3,
                p1.Y + (p2.Y - p0.Y) * alpha / 3);
            var cp2 = new Point(
                p2.X - (p3.X - p1.X) * alpha / 3,
                p2.Y - (p3.Y - p1.Y) * alpha / 3);

            ctx.CubicBezierTo(cp1, cp2, p2);
        }
    }

    private static IBrush GetLayerColor(int catIdx, Series.ThemeRiverSeries river, string category)
    {
        // Try to find color from data items
        var item = river.DataItems.FirstOrDefault(d => d.Category == category && d.Color != null);
        if (item?.Color != null) return item.Color;

        return new SolidColorBrush(LineRenderer.DefaultPalette[catIdx % LineRenderer.DefaultPalette.Length]);
    }
}
