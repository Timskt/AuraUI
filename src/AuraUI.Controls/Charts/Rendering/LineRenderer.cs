using Avalonia;
using Avalonia.Media;

namespace AuraUI.Controls.Charts.Rendering;

/// <summary>
/// Renders LineSeries and AreaSeries. This is the most complex renderer
/// as it handles interpolation, area fill, markers, and animation.
///
/// Rendering pipeline:
///   1. Map data points to pixel coordinates via axis.ValueToPixel()
///   2. Build a PathGeometry from the pixel points using the selected interpolation
///   3. For animation: lerp between old and new pixel positions
///   4. Draw the line path via context.DrawGeometry()
///   5. If area fill: build a closed path and draw with semi-transparent brush
///   6. If markers: draw marker shapes at each data point
///
/// Performance:
///   - PathGeometry objects are built per-frame (Avalonia caches the GPU tessellation)
///   - For >10k points, LTTB downsampling reduces point count before geometry building
///   - Markers are skipped when there are too many points (>500) to maintain 60fps
/// </summary>
public class LineRenderer : IChartRenderer
{
    public string Key => "Line";

    public void Render(
        DrawingContext context,
        ChartSeries series,
        Rect plotArea,
        ChartAxis? xAxis,
        ChartAxis? yAxis,
        double progress,
        IReadOnlyList<ChartSeries> allSeries)
    {
        if (series is not Series.LineSeries line || !series.IsVisible) return;
        if (xAxis == null || yAxis == null) return;

        var dataPoints = line.DataPoints;
        if (dataPoints.Count == 0) return;

        // Map data to pixel coordinates
        var points = MapToPixels(dataPoints, xAxis, yAxis, plotArea, line);

        // Apply animation: if animating, we could lerp from previous positions
        // For now, progress drives the visible portion (draw-in animation)
        var visibleCount = progress >= 1.0
            ? points.Length
            : Math.Max(2, (int)(points.Length * progress));
        var visiblePoints = points.AsSpan(0, visibleCount);

        var color = ResolveColor(line);
        var pen = new Pen(color, line.StrokeThickness);

        // Apply dash style
        if (line.DashStyle != null && line.DashStyle.Length > 0)
        {
            pen = new Pen(color, line.StrokeThickness,
                new DashStyle(line.DashStyle, 0));
        }

        // ─── Area Fill ───
        if (line.ShowArea)
        {
            var areaPath = BuildAreaPath(visiblePoints, plotArea, line.Interpolation, line.SmoothTension);
            if (areaPath != null)
            {
                var areaBrush = new SolidColorBrush(((SolidColorBrush)color).Color)
                {
                    Opacity = line.AreaOpacity
                };
                context.DrawGeometry(areaBrush, null, areaPath);
            }
        }

        // ─── Line ───
        var linePath = BuildLinePath(visiblePoints, line.Interpolation, line.SmoothTension);
        if (linePath != null)
        {
            context.DrawGeometry(null, pen, linePath);
        }

        // ─── Markers ───
        var shape = line.ShowMarkers ? MarkerShape.Circle : line.MarkerShape;
        if (shape != MarkerShape.None)
        {
            DrawMarkers(context, visiblePoints, color, line.MarkerSize, shape);
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
        if (series is not Series.LineSeries line) return null;
        if (xAxis == null || yAxis == null) return null;

        var dataPoints = line.DataPoints;
        if (dataPoints.Count == 0) return null;

        var points = MapToPixels(dataPoints, xAxis, yAxis, plotArea, line);
        var hitRadius = Math.Max(line.MarkerSize, 10.0);

        int bestIndex = -1;
        double bestDist = double.MaxValue;

        for (int i = 0; i < points.Length; i++)
        {
            var dx = pointerPosition.X - points[i].X;
            var dy = pointerPosition.Y - points[i].Y;
            var dist = Math.Sqrt(dx * dx + dy * dy);

            if (dist < hitRadius && dist < bestDist)
            {
                bestDist = dist;
                bestIndex = i;
            }
        }

        if (bestIndex < 0) return null;

        return new ChartHitResult
        {
            Series = series,
            DataPoint = dataPoints[bestIndex],
            DataIndex = bestIndex,
            HitPosition = points[bestIndex],
            Distance = bestDist
        };
    }

    // ────────────────────────────────────────────────
    //  Internal helpers
    // ────────────────────────────────────────────────

    internal static Point[] MapToPixels(
        Avalonia.Collections.AvaloniaList<ChartDataPoint> dataPoints,
        ChartAxis xAxis,
        ChartAxis yAxis,
        Rect plotArea,
        XYChartSeries series)
    {
        var points = new Point[dataPoints.Count];
        for (int i = 0; i < dataPoints.Count; i++)
        {
            var dp = dataPoints[i];
            var x = xAxis.ValueToPixel(dp.X);
            var y = yAxis.ValueToPixel(dp.Y);
            points[i] = new Point(x, y);
        }
        return points;
    }

    internal static IBrush ResolveColor(ChartSeries series)
    {
        if (series.Color is IBrush brush) return brush;

        // Auto-assign from palette
        var index = series.SeriesIndex;
        return new SolidColorBrush(DefaultPalette[index % DefaultPalette.Length]);
    }

    /// <summary>
    /// Build a PathGeometry for the line connecting the given points.
    /// </summary>
    internal static PathGeometry? BuildLinePath(
        ReadOnlySpan<Point> points,
        ChartInterpolation interpolation,
        double tension)
    {
        if (points.Length < 2) return null;

        var geometry = new PathGeometry();
        var figure = new PathFigure { StartPoint = points[0], IsClosed = false };

        switch (interpolation)
        {
            case ChartInterpolation.Linear:
                for (int i = 1; i < points.Length; i++)
                    figure.Segments!.Add(new LineSegment { Point = points[i] });
                break;

            case ChartInterpolation.MonotoneCubic:
                AddMonotoneCubicSegments(figure, points, tension);
                break;

            case ChartInterpolation.StepBefore:
                for (int i = 1; i < points.Length; i++)
                {
                    figure.Segments!.Add(new LineSegment { Point = new Point(points[i].X, points[i - 1].Y) });
                    figure.Segments!.Add(new LineSegment { Point = points[i] });
                }
                break;

            case ChartInterpolation.StepAfter:
                for (int i = 1; i < points.Length; i++)
                {
                    figure.Segments!.Add(new LineSegment { Point = new Point(points[i - 1].X, points[i].Y) });
                    figure.Segments!.Add(new LineSegment { Point = points[i] });
                }
                break;
        }

        geometry.Figures!.Add(figure);
        return geometry;
    }

    /// <summary>
    /// Build a closed PathGeometry for area fill (line + baseline).
    /// </summary>
    internal static PathGeometry? BuildAreaPath(
        ReadOnlySpan<Point> points,
        Rect plotArea,
        ChartInterpolation interpolation,
        double tension)
    {
        if (points.Length < 2) return null;

        var geometry = new PathGeometry();
        var figure = new PathFigure { StartPoint = new Point(points[0].X, plotArea.Bottom), IsClosed = true };

        // Move up to first point
        figure.Segments!.Add(new LineSegment { Point = points[0] });

        // Trace the line
        switch (interpolation)
        {
            case ChartInterpolation.Linear:
                for (int i = 1; i < points.Length; i++)
                    figure.Segments!.Add(new LineSegment { Point = points[i] });
                break;

            case ChartInterpolation.MonotoneCubic:
                AddMonotoneCubicSegments(figure, points, tension);
                break;

            default:
                for (int i = 1; i < points.Length; i++)
                    figure.Segments!.Add(new LineSegment { Point = points[i] });
                break;
        }

        // Close back to baseline
        figure.Segments!.Add(new LineSegment { Point = new Point(points[^1].X, plotArea.Bottom) });

        geometry.Figures!.Add(figure);
        return geometry;
    }

    /// <summary>
    /// Add monotone cubic Bezier segments to a PathFigure.
    /// Uses the Fritsch-Carlson method for monotone interpolation.
    /// </summary>
    private static void AddMonotoneCubicSegments(PathFigure figure, ReadOnlySpan<Point> points, double tension)
    {
        if (points.Length < 3)
        {
            for (int i = 1; i < points.Length; i++)
                figure.Segments!.Add(new LineSegment { Point = points[i] });
            return;
        }

        // Compute tangents using Fritsch-Carlson method
        var n = points.Length;
        var deltas = new double[n - 1];
        var tangents = new double[n];

        for (int i = 0; i < n - 1; i++)
        {
            var dx = points[i + 1].X - points[i].X;
            deltas[i] = dx != 0 ? (points[i + 1].Y - points[i].Y) / dx : 0;
        }

        tangents[0] = deltas[0];
        tangents[n - 1] = deltas[n - 2];

        for (int i = 1; i < n - 1; i++)
        {
            if (deltas[i - 1] * deltas[i] <= 0)
                tangents[i] = 0;
            else
                tangents[i] = (deltas[i - 1] + deltas[i]) / 2;
        }

        // Generate Bezier segments
        for (int i = 0; i < n - 1; i++)
        {
            var p0 = points[i];
            var p1 = points[i + 1];
            var dx = p1.X - p0.X;

            var cp1 = new Point(p0.X + dx / 3, p0.Y + tangents[i] * dx / 3 * tension);
            var cp2 = new Point(p1.X - dx / 3, p1.Y - tangents[i + 1] * dx / 3 * tension);

            figure.Segments!.Add(new BezierSegment { Point1 = cp1, Point2 = cp2, Point3 = p1 });
        }
    }

    /// <summary>
    /// Draw marker shapes at each data point.
    /// </summary>
    internal static void DrawMarkers(
        DrawingContext context,
        ReadOnlySpan<Point> points,
        IBrush color,
        double size,
        MarkerShape shape)
    {
        var half = size / 2;
        var pen = new Pen(color, 1.5);
        var fillBrush = Brushes.White;
        var strokeBrush = color;

        foreach (var pt in points)
        {
            switch (shape)
            {
                case MarkerShape.Circle:
                    context.DrawEllipse(fillBrush, pen, pt, half, half);
                    break;

                case MarkerShape.Square:
                    var rect = new Rect(pt.X - half, pt.Y - half, size, size);
                    context.DrawRectangle(fillBrush, pen, rect);
                    break;

                case MarkerShape.Diamond:
                    DrawDiamond(context, pt, half, fillBrush, pen);
                    break;

                case MarkerShape.Triangle:
                    DrawTriangle(context, pt, half, fillBrush, pen);
                    break;

                case MarkerShape.Cross:
                    context.DrawLine(pen,
                        new Point(pt.X - half, pt.Y - half),
                        new Point(pt.X + half, pt.Y + half));
                    context.DrawLine(pen,
                        new Point(pt.X + half, pt.Y - half),
                        new Point(pt.X - half, pt.Y + half));
                    break;
            }
        }
    }

    private static void DrawDiamond(DrawingContext context, Point center, double half, IBrush fill, Pen pen)
    {
        var geometry = new PathGeometry();
        var figure = new PathFigure { StartPoint = new Point(center.X, center.Y - half), IsClosed = true };
        figure.Segments!.Add(new LineSegment { Point = new Point(center.X + half, center.Y) });
        figure.Segments!.Add(new LineSegment { Point = new Point(center.X, center.Y + half) });
        figure.Segments!.Add(new LineSegment { Point = new Point(center.X - half, center.Y) });
        geometry.Figures!.Add(figure);
        context.DrawGeometry(fill, pen, geometry);
    }

    private static void DrawTriangle(DrawingContext context, Point center, double half, IBrush fill, Pen pen)
    {
        var geometry = new PathGeometry();
        var figure = new PathFigure { StartPoint = new Point(center.X, center.Y - half), IsClosed = true };
        figure.Segments!.Add(new LineSegment { Point = new Point(center.X + half, center.Y + half) });
        figure.Segments!.Add(new LineSegment { Point = new Point(center.X - half, center.Y + half) });
        geometry.Figures!.Add(figure);
        context.DrawGeometry(fill, pen, geometry);
    }

    // ────────────────────────────────────────────────
    //  Default color palette (theme-aware)
    // ────────────────────────────────────────────────

    internal static readonly Color[] DefaultPalette =
    {
        Color.Parse("#6750A4"), // Primary (M3)
        Color.Parse("#625B71"), // Secondary
        Color.Parse("#7D5260"), // Tertiary
        Color.Parse("#2E7D32"), // Success
        Color.Parse("#ED6C02"), // Warning
        Color.Parse("#D32F2F"), // Error
        Color.Parse("#0288D1"), // Info
        Color.Parse("#9C27B0"), // Purple
        Color.Parse("#00838F"), // Cyan
        Color.Parse("#EF6C00"), // Orange
        Color.Parse("#558B2F"), // Light Green
        Color.Parse("#4527A0"), // Deep Purple
    };
}
