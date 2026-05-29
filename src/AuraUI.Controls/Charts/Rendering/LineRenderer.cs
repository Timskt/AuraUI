using System.Buffers;
using Avalonia;
using Avalonia.Media;

namespace AuraUI.Controls.Charts.Rendering;

/// <summary>
/// Renders LineSeries and AreaSeries. This is the most complex renderer
/// as it handles interpolation, area fill, markers, and animation.
///
/// Rendering pipeline:
///   1. Map data points to pixel coordinates via axis.ValueToPixel()
///   2. Apply data virtualization (viewport culling + LTTB downsampling)
///   3. Check geometry cache — if hit, reuse cached StreamGeometry
///   4. On cache miss: build PathGeometry from pixel points
///   5. Store in cache for next frame
///   6. Draw the line path via context.DrawGeometry()
///   7. If area fill: build a closed path and draw with semi-transparent brush
///   8. If markers: draw marker shapes at each data point
///
/// Performance optimizations:
///   - GeometryCache: StreamGeometry objects cached per series, invalidated only on data/size change
///   - DataVirtualizer: LTTB downsampling for >1000 points, viewport culling via binary search
///   - ArrayPool: Point[] arrays rented from pool, no per-frame allocations
///   - Benchmark: render time and geometry count tracked for profiling
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
        IReadOnlyList<ChartSeries> allSeries,
        ChartRenderContext? renderContext = null)
    {
        if (series is not Series.LineSeries line || !series.IsVisible) return;
        if (xAxis == null || yAxis == null) return;

        var dataPoints = line.DataPoints;
        if (dataPoints.Count == 0) return;

        var color = ResolveColor(line);
        var pen = new Pen(color, line.StrokeThickness);

        // Apply dash style
        if (line.DashStyle != null && line.DashStyle.Length > 0)
        {
            pen = new Pen(color, line.StrokeThickness,
                new DashStyle(line.DashStyle, 0));
        }

        // ─── Try geometry cache ───
        var seriesHash = series.SeriesIndex;
        var dataHash = ChartRenderContext.HashDataPoints(dataPoints);
        var sizeHash = renderContext?.PlotAreaSizeHash ?? 0;

        if (renderContext != null &&
            renderContext.GeometryCache.TryGet(seriesHash, dataHash, sizeHash, progress,
                out var cachedLinePath, out var cachedAreaPath))
        {
            // Cache hit — draw cached geometries directly
            DrawCachedGeometries(context, cachedLinePath, cachedAreaPath, line, color, pen, plotArea);
            return;
        }

        // ─── Cache miss — build geometries ───
        // Map data to pixel coordinates using pooled array
        var points = MapToPixelsPooled(dataPoints, xAxis, yAxis, plotArea, line);

        // Apply data virtualization (viewport culling + LTTB downsampling)
        var visiblePoints = DataVirtualizer.GetVisiblePoints(points, dataPoints, plotArea, progress);

        // Track point counts for benchmarking
        var originalCount = points.Length;
        var visibleCount = visiblePoints.Length;

        // Build and cache geometries
        Avalonia.Media.StreamGeometry? linePath = null;
        Avalonia.Media.StreamGeometry? areaPath = null;

        if (renderContext != null)
        {
            renderContext.Benchmark.BeginGeometryBuild();
        }

        // ─── Area Fill ───
        if (line.ShowArea)
        {
            var areaGeom = BuildAreaPath(visiblePoints, plotArea, line.Interpolation, line.SmoothTension);
            if (areaGeom != null)
            {
                areaPath = areaGeom;
                var areaBrush = new SolidColorBrush(((SolidColorBrush)color).Color)
                {
                    Opacity = line.AreaOpacity
                };
                context.DrawGeometry(areaBrush, null, areaGeom);
            }
        }

        // ─── Line ───
        var lineGeom = BuildLinePath(visiblePoints, line.Interpolation, line.SmoothTension);
        if (lineGeom != null)
        {
            linePath = lineGeom;
            context.DrawGeometry(null, pen, lineGeom);
        }

        if (renderContext != null)
        {
            renderContext.Benchmark.EndGeometryBuild();
            renderContext.Benchmark.RecordPointCounts(visibleCount, Math.Max(0, originalCount - visibleCount));
        }

        // Store in cache for next frame
        renderContext?.GeometryCache.Store(seriesHash, dataHash, sizeHash, progress, linePath, areaPath);

        // Return pooled array
        ChartRenderContext.ReturnPointArray(points);

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
        IReadOnlyList<ChartSeries> allSeries,
        ChartRenderContext? renderContext = null)
    {
        if (series is not Series.LineSeries line) return null;
        if (xAxis == null || yAxis == null) return null;

        var dataPoints = line.DataPoints;
        if (dataPoints.Count == 0) return null;

        var points = MapToPixelsPooled(dataPoints, xAxis, yAxis, plotArea, line);
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

        // Return pooled array
        ChartRenderContext.ReturnPointArray(points);

        if (bestIndex < 0) return null;

        return new ChartHitResult
        {
            Series = series,
            DataPoint = dataPoints[bestIndex],
            DataIndex = bestIndex,
            HitPosition = new Point(
                xAxis.ValueToPixel(dataPoints[bestIndex].X),
                yAxis.ValueToPixel(dataPoints[bestIndex].Y)),
            Distance = bestDist
        };
    }

    // ────────────────────────────────────────────────
    //  Draw cached geometries (fast path)
    // ────────────────────────────────────────────────

    private static void DrawCachedGeometries(
        DrawingContext context,
        Avalonia.Media.StreamGeometry? cachedLinePath,
        Avalonia.Media.StreamGeometry? cachedAreaPath,
        Series.LineSeries line,
        IBrush color,
        Pen pen,
        Rect plotArea)
    {
        if (cachedAreaPath != null && line.ShowArea)
        {
            var areaBrush = new SolidColorBrush(((SolidColorBrush)color).Color)
            {
                Opacity = line.AreaOpacity
            };
            context.DrawGeometry(areaBrush, null, cachedAreaPath);
        }

        if (cachedLinePath != null)
        {
            context.DrawGeometry(null, pen, cachedLinePath);
        }
    }

    // ────────────────────────────────────────────────
    //  Internal helpers
    // ────────────────────────────────────────────────

    /// <summary>
    /// Map data points to pixel coordinates using a pooled Point[] array.
    /// The caller must return the array via ChartRenderContext.ReturnPointArray().
    /// </summary>
    internal static Point[] MapToPixelsPooled(
        Avalonia.Collections.AvaloniaList<ChartDataPoint> dataPoints,
        ChartAxis xAxis,
        ChartAxis yAxis,
        Rect plotArea,
        XYChartSeries series)
    {
        var points = ChartRenderContext.RentPointArray(dataPoints.Count);
        for (int i = 0; i < dataPoints.Count; i++)
        {
            var dp = dataPoints[i];
            var x = xAxis.ValueToPixel(dp.X);
            var y = yAxis.ValueToPixel(dp.Y);
            points[i] = new Point(x, y);
        }
        return points;
    }

    /// <summary>
    /// Map data points to pixel coordinates (allocates new array).
    /// Used by AreaRenderer and other renderers that don't manage pooling.
    /// </summary>
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
    /// Build a StreamGeometry for the line connecting the given points.
    /// Uses StreamGeometry for lower allocation overhead than PathGeometry.
    /// </summary>
    internal static Avalonia.Media.StreamGeometry? BuildLinePath(
        ReadOnlySpan<Point> points,
        ChartInterpolation interpolation,
        double tension)
    {
        if (points.Length < 2) return null;

        var geometry = new Avalonia.Media.StreamGeometry();
        using (var ctx = geometry.Open())
        {
            ctx.BeginFigure(points[0], false);

            switch (interpolation)
            {
                case ChartInterpolation.Linear:
                    for (int i = 1; i < points.Length; i++)
                        ctx.LineTo(points[i]);
                    break;

                case ChartInterpolation.MonotoneCubic:
                    AddMonotoneCubicToStream(ctx, points, tension);
                    break;

                case ChartInterpolation.StepBefore:
                    for (int i = 1; i < points.Length; i++)
                    {
                        ctx.LineTo(new Point(points[i].X, points[i - 1].Y));
                        ctx.LineTo(points[i]);
                    }
                    break;

                case ChartInterpolation.StepAfter:
                    for (int i = 1; i < points.Length; i++)
                    {
                        ctx.LineTo(new Point(points[i - 1].X, points[i].Y));
                        ctx.LineTo(points[i]);
                    }
                    break;
            }

            ctx.EndFigure(false);
        }

        return geometry;
    }

    /// <summary>
    /// Build a closed StreamGeometry for area fill (line + baseline).
    /// </summary>
    internal static Avalonia.Media.StreamGeometry? BuildAreaPath(
        ReadOnlySpan<Point> points,
        Rect plotArea,
        ChartInterpolation interpolation,
        double tension)
    {
        if (points.Length < 2) return null;

        var geometry = new Avalonia.Media.StreamGeometry();
        using (var ctx = geometry.Open())
        {
            // Start at bottom-left, move up to first point
            ctx.BeginFigure(new Point(points[0].X, plotArea.Bottom), true);
            ctx.LineTo(points[0]);

            // Trace the line
            switch (interpolation)
            {
                case ChartInterpolation.Linear:
                    for (int i = 1; i < points.Length; i++)
                        ctx.LineTo(points[i]);
                    break;

                case ChartInterpolation.MonotoneCubic:
                    AddMonotoneCubicToStream(ctx, points, tension);
                    break;

                default:
                    for (int i = 1; i < points.Length; i++)
                        ctx.LineTo(points[i]);
                    break;
            }

            // Close back to baseline
            ctx.LineTo(new Point(points[^1].X, plotArea.Bottom));
            ctx.EndFigure(true);
        }

        return geometry;
    }

    /// <summary>
    /// Add monotone cubic Bezier segments to a StreamGeometryContext.
    /// Uses the Fritsch-Carlson method for monotone interpolation.
    /// </summary>
    private static void AddMonotoneCubicToStream(
        Avalonia.Media.StreamGeometryContext ctx,
        ReadOnlySpan<Point> points,
        double tension)
    {
        if (points.Length < 3)
        {
            for (int i = 1; i < points.Length; i++)
                ctx.LineTo(points[i]);
            return;
        }

        // Compute tangents using Fritsch-Carlson method
        var n = points.Length;

        // Use stack allocation for small arrays, heap for large
        Span<double> deltas = n <= 128 ? stackalloc double[n - 1] : new double[n - 1];
        Span<double> tangents = n <= 128 ? stackalloc double[n] : new double[n];

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

            ctx.CubicBezierTo(cp1, cp2, p1);
        }
    }

    /// <summary>
    /// Build a PathGeometry for the line connecting the given points.
    /// Used as fallback when StreamGeometry caching is not available.
    /// </summary>
    internal static PathGeometry? BuildLinePathLegacy(
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
    /// Used as fallback when StreamGeometry caching is not available.
    /// </summary>
    internal static PathGeometry? BuildAreaPathLegacy(
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

        // Use stack allocation for small arrays, heap for large
        Span<double> deltas = n <= 128 ? stackalloc double[n - 1] : new double[n - 1];
        Span<double> tangents = n <= 128 ? stackalloc double[n] : new double[n];

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
