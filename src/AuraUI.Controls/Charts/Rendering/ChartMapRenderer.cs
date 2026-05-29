using Avalonia;
using Avalonia.Media;

namespace AuraUI.Controls.Charts.Rendering;

/// <summary>
/// Renders ChartMapSeries as a geographic map with choropleth regions,
/// scatter overlays, and line connections.
///
/// Algorithm:
///   1. Project lat/lon coordinates to pixel positions using the selected projection
///   2. Build PathGeometry for each region polygon
///   3. Fill regions with choropleth colors or default color
///   4. Draw scatter markers at projected positions
///   5. Draw line connections (straight or curved great-circle arcs)
///
/// Projections supported:
///   - Equirectangular: simple linear mapping
///   - Mercator: cylindrical conformal (standard web map)
///   - Robinson: pseudo-cylindrical (good for world maps)
/// </summary>
public class ChartMapRenderer : IChartRenderer
{
    public string Key => "ChartMap";

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
        if (series is not Geo.ChartMapSeries map || !series.IsVisible) return;

        // 1. Draw regions (choropleth)
        foreach (var region in map.Regions)
        {
            DrawRegion(context, region, map, plotArea, progress);
        }

        // 2. Draw lines
        foreach (var line in map.Lines)
        {
            DrawMapLine(context, line, map, plotArea, progress);
        }

        // 3. Draw scatter points
        foreach (var point in map.ScatterPoints)
        {
            DrawScatterPoint(context, point, map, plotArea, progress);
        }

        // 4. Draw labels
        if (map.ShowLabels && progress >= 1.0)
        {
            foreach (var region in map.Regions)
            {
                DrawRegionLabel(context, region, map, plotArea);
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
        if (series is not Geo.ChartMapSeries map) return null;

        // Hit test scatter points
        int idx = 0;
        foreach (var point in map.ScatterPoints)
        {
            var pixel = ProjectPoint(point.Longitude, point.Latitude, map, plotArea);
            var dx = pointerPosition.X - pixel.X;
            var dy = pointerPosition.Y - pixel.Y;
            var dist = Math.Sqrt(dx * dx + dy * dy);

            if (dist < point.Size + 4)
            {
                return new ChartHitResult
                {
                    Series = series,
                    DataIndex = idx,
                    HitPosition = pixel,
                    Distance = dist
                };
            }
            idx++;
        }

        // Hit test regions (simple bounding box check)
        idx = 0;
        foreach (var region in map.Regions)
        {
            foreach (var polygon in region.Polygons)
            {
                if (polygon.Length < 3) continue;

                var pixels = polygon.Select(p => ProjectPoint(p.X, p.Y, map, plotArea)).ToArray();
                if (IsPointInPolygon(pointerPosition, pixels))
                {
                    return new ChartHitResult
                    {
                        Series = series,
                        DataIndex = idx,
                        HitPosition = pointerPosition
                    };
                }
            }
            idx++;
        }

        return null;
    }

    // ────────────────────────────────────────────────
    //  Region rendering
    // ────────────────────────────────────────────────

    private static void DrawRegion(DrawingContext context, Geo.GeoRegion region,
        Geo.ChartMapSeries map, Rect plotArea, double progress)
    {
        foreach (var polygon in region.Polygons)
        {
            if (polygon.Length < 3) continue;

            var pixelPoints = polygon
                .Select(p => ProjectPoint(p.X, p.Y, map, plotArea))
                .ToArray();

            // Build PathGeometry
            var geometry = new PathGeometry();
            var figure = new PathFigure
            {
                StartPoint = pixelPoints[0],
                IsClosed = true
            };

            for (int i = 1; i < pixelPoints.Length; i++)
            {
                figure.Segments!.Add(new LineSegment { Point = pixelPoints[i] });
            }

            geometry.Figures!.Add(figure);

            // Determine fill color
            IBrush fillBrush;
            if (region.Color != null)
            {
                fillBrush = region.Color;
            }
            else if (map.ChoroplethMap != null)
            {
                fillBrush = map.ChoroplethMap.GetColor(region.Value);
            }
            else
            {
                fillBrush = map.DefaultRegionColor ?? Brushes.LightSteelBlue;
            }

            // Apply opacity for animation
            if (fillBrush is SolidColorBrush scb)
            {
                fillBrush = new SolidColorBrush(scb.Color) { Opacity = map.FillOpacity * progress };
            }

            var strokePen = new Pen(
                map.StrokeColor ?? Brushes.White,
                map.StrokeThickness);

            context.DrawGeometry(fillBrush, strokePen, geometry);
        }
    }

    // ────────────────────────────────────────────────
    //  Scatter point rendering
    // ────────────────────────────────────────────────

    private static void DrawScatterPoint(DrawingContext context, Geo.MapScatterPoint point,
        Geo.ChartMapSeries map, Rect plotArea, double progress)
    {
        var pixel = ProjectPoint(point.Longitude, point.Latitude, map, plotArea);
        var size = point.Size * progress;
        var color = point.Color ?? Brushes.Red;
        var brush = new SolidColorBrush(((SolidColorBrush)color).Color) { Opacity = 0.8 };
        var pen = new Pen(Brushes.White, 1.0);

        context.DrawEllipse(brush, pen, pixel, size / 2, size / 2);

        // Name label
        if (map.ShowLabels && !string.IsNullOrEmpty(point.Name) && progress >= 1.0)
        {
            var formattedText = new FormattedText(point.Name,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface("Segoe UI", FontStyle.Normal, FontWeight.Normal),
                map.LabelFontSize,
                Brushes.DimGray);

            context.DrawText(formattedText,
                new Point(pixel.X + size / 2 + 2, pixel.Y - formattedText.Height / 2));
        }
    }

    // ────────────────────────────────────────────────
    //  Line rendering
    // ────────────────────────────────────────────────

    private static void DrawMapLine(DrawingContext context, Geo.MapLine line,
        Geo.ChartMapSeries map, Rect plotArea, double progress)
    {
        var start = ProjectPoint(line.StartLongitude, line.StartLatitude, map, plotArea);
        var end = ProjectPoint(line.EndLongitude, line.EndLatitude, map, plotArea);

        var color = line.Color ?? Brushes.OrangeRed;
        var pen = new Pen(color, line.Thickness,
            dashStyle: new DashStyle(new double[] { 4, 2 }, 0));

        if (line.Curved)
        {
            // Draw curved arc (great circle approximation)
            var midX = (start.X + end.X) / 2;
            var midY = (start.Y + end.Y) / 2;
            var dx = end.X - start.X;
            var dy = end.Y - start.Y;
            var dist = Math.Sqrt(dx * dx + dy * dy);

            // Perpendicular offset for curve
            var curveAmount = dist * 0.2;
            var ctrlX = midX - dy / dist * curveAmount;
            var ctrlY = midY + dx / dist * curveAmount;

            var geometry = new PathGeometry();
            var figure = new PathFigure { StartPoint = start, IsClosed = false };
            figure.Segments!.Add(new QuadraticBezierSegment
            {
                Point1 = new Point(ctrlX, ctrlY),
                Point2 = end
            });
            geometry.Figures!.Add(figure);

            context.DrawGeometry(null, pen, geometry);
        }
        else
        {
            context.DrawLine(pen, start, end);
        }
    }

    // ────────────────────────────────────────────────
    //  Label rendering
    // ────────────────────────────────────────────────

    private static void DrawRegionLabel(DrawingContext context, Geo.GeoRegion region,
        Geo.ChartMapSeries map, Rect plotArea)
    {
        if (string.IsNullOrEmpty(region.Name)) return;
        if (region.Polygons.Count == 0 || region.Polygons[0].Length < 3) return;

        // Compute centroid of first polygon
        var polygon = region.Polygons[0];
        var centroidLon = polygon.Average(p => p.X);
        var centroidLat = polygon.Average(p => p.Y);
        var centroid = ProjectPoint(centroidLon, centroidLat, map, plotArea);

        // Check if centroid is within plot area
        if (!plotArea.Contains(centroid)) return;

        var formattedText = new FormattedText(region.Name,
            System.Globalization.CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight,
            new Typeface("Segoe UI", FontStyle.Normal, FontWeight.Normal),
            map.LabelFontSize,
            Brushes.DimGray);

        context.DrawText(formattedText,
            new Point(centroid.X - formattedText.Width / 2, centroid.Y - formattedText.Height / 2));
    }

    // ────────────────────────────────────────────────
    //  Projection math
    // ────────────────────────────────────────────────

    /// <summary>
    /// Project a lat/lon coordinate to pixel position within the plot area.
    /// </summary>
    private static Point ProjectPoint(double lon, double lat, Geo.ChartMapSeries map, Rect plotArea)
    {
        // Apply centering
        var adjustedLon = lon - map.CenterLongitude;
        var adjustedLat = lat - map.CenterLatitude;

        double x, y;

        switch (map.Projection)
        {
            case Geo.MapProjection.Mercator:
                x = adjustedLon;
                y = -Math.Log(Math.Tan(Math.PI / 4 + adjustedLat * Math.PI / 360)) * 180 / Math.PI;
                break;

            case Geo.MapProjection.Robinson:
                // Robinson projection approximation
                var absLat = Math.Abs(adjustedLat);
                var latIdx = absLat / 90 * 10;
                var idx = Math.Min(9, (int)latIdx);
                var frac = latIdx - idx;

                var lenA = RobinsonLenTable[idx];
                var lenB = RobinsonLenTable[Math.Min(10, idx + 1)];
                var len = lenA + frac * (lenB - lenA);

                x = adjustedLon * len;
                y = adjustedLat > 0 ? -adjustedLat * 0.5 : adjustedLat * 0.5;
                break;

            default: // Equirectangular
                x = adjustedLon;
                y = -adjustedLat;
                break;
        }

        // Apply zoom and map to pixel coordinates
        var scale = map.Zoom * plotArea.Width / 360;
        var pixelX = plotArea.Center.X + x * scale;
        var pixelY = plotArea.Center.Y + y * scale;

        return new Point(pixelX, pixelY);
    }

    // Robinson projection length table (half the parallel length for each 5-degree latitude band)
    private static readonly double[] RobinsonLenTable =
    {
        1.0000, 0.9986, 0.9954, 0.9900, 0.9822,
        0.9730, 0.9600, 0.9427, 0.9216, 0.8962, 0.8560
    };

    /// <summary>
    /// Point-in-polygon test using ray casting algorithm.
    /// </summary>
    private static bool IsPointInPolygon(Point point, Point[] polygon)
    {
        bool inside = false;
        int j = polygon.Length - 1;

        for (int i = 0; i < polygon.Length; i++)
        {
            if ((polygon[i].Y > point.Y) != (polygon[j].Y > point.Y) &&
                point.X < (polygon[j].X - polygon[i].X) * (point.Y - polygon[i].Y) /
                (polygon[j].Y - polygon[i].Y) + polygon[i].X)
            {
                inside = !inside;
            }
            j = i;
        }

        return inside;
    }
}
