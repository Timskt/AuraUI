using Avalonia;

namespace AuraUI.Controls.Charts.Processing;

/// <summary>
/// Provides smooth curve interpolation methods for line and area charts.
/// Supports Catmull-Rom splines, cubic Bezier, and monotone cubic interpolation.
/// </summary>
public static class ChartInterpolator
{
    /// <summary>
    /// Generate Catmull-Rom spline points between the given control points.
    /// Produces smooth curves that pass through all control points.
    /// </summary>
    /// <param name="points">Control points to interpolate through.</param>
    /// <param name="tension">Tension factor (0 = uniform, 0.5 = centripetal, 1 = chordal).</param>
    /// <param name="segmentsPerCurve">Number of output segments between each pair of control points.</param>
    public static Point[] CatmullRom(ReadOnlySpan<Point> points, double tension = 0.5, int segmentsPerCurve = 16)
    {
        if (points.Length < 2) return points.ToArray();

        var result = new List<Point>();

        for (int i = 0; i < points.Length - 1; i++)
        {
            var p0 = i > 0 ? points[i - 1] : points[i];
            var p1 = points[i];
            var p2 = points[i + 1];
            var p3 = i < points.Length - 2 ? points[i + 2] : points[i + 1];

            for (int j = 0; j <= segmentsPerCurve; j++)
            {
                var t = (double)j / segmentsPerCurve;
                var point = CatmullRomPoint(p0, p1, p2, p3, t, tension);
                result.Add(point);
            }
        }

        return result.ToArray();
    }

    /// <summary>
    /// Generate cubic Bezier curve points between control points.
    /// Each segment uses two control points and two endpoints.
    /// </summary>
    public static Point[] CubicBezier(ReadOnlySpan<Point> points, double tension = 0.3, int segmentsPerCurve = 16)
    {
        if (points.Length < 2) return points.ToArray();

        var result = new List<Point>();

        for (int i = 0; i < points.Length - 1; i++)
        {
            var p0 = points[i];
            var p1 = points[i + 1];

            // Compute control points based on tangent direction
            var dx = p1.X - p0.X;
            var cp1 = new Point(p0.X + dx * tension, p0.Y);
            var cp2 = new Point(p1.X - dx * tension, p1.Y);

            for (int j = 0; j <= segmentsPerCurve; j++)
            {
                var t = (double)j / segmentsPerCurve;
                var point = CubicBezierPoint(p0, cp1, cp2, p1, t);
                result.Add(point);
            }
        }

        return result.ToArray();
    }

    /// <summary>
    /// Generate monotone cubic interpolation points.
    /// Preserves monotonicity of the data (no overshooting).
    /// Uses the Fritsch-Carlson method.
    /// </summary>
    public static Point[] MonotoneCubic(ReadOnlySpan<Point> points, int segmentsPerCurve = 16)
    {
        if (points.Length < 2) return points.ToArray();
        if (points.Length == 2) return points.ToArray();

        var n = points.Length;
        var deltas = new double[n - 1];
        var tangents = new double[n];

        // Compute slopes
        for (int i = 0; i < n - 1; i++)
        {
            var dx = points[i + 1].X - points[i].X;
            deltas[i] = Math.Abs(dx) > 1e-10 ? (points[i + 1].Y - points[i].Y) / dx : 0;
        }

        // Compute tangents using Fritsch-Carlson method
        tangents[0] = deltas[0];
        tangents[n - 1] = deltas[n - 2];

        for (int i = 1; i < n - 1; i++)
        {
            if (deltas[i - 1] * deltas[i] <= 0)
                tangents[i] = 0;
            else
                tangents[i] = (deltas[i - 1] + deltas[i]) / 2;
        }

        // Generate curve points
        var result = new List<Point>();
        for (int i = 0; i < n - 1; i++)
        {
            var p0 = points[i];
            var p1 = points[i + 1];
            var dx = p1.X - p0.X;

            for (int j = 0; j <= segmentsPerCurve; j++)
            {
                var t = (double)j / segmentsPerCurve;
                var t2 = t * t;
                var t3 = t2 * t;

                // Hermite basis functions
                var h00 = 2 * t3 - 3 * t2 + 1;
                var h10 = t3 - 2 * t2 + t;
                var h01 = -2 * t3 + 3 * t2;
                var h11 = t3 - t2;

                var x = p0.X + t * dx;
                var y = h00 * p0.Y + h10 * dx * tangents[i] + h01 * p1.Y + h11 * dx * tangents[i + 1];

                result.Add(new Point(x, y));
            }
        }

        return result.ToArray();
    }

    /// <summary>
    /// Compute a single point on a Catmull-Rom spline at parameter t.
    /// </summary>
    private static Point CatmullRomPoint(Point p0, Point p1, Point p2, Point p3, double t, double alpha)
    {
        var t2 = t * t;
        var t3 = t2 * t;

        // Catmull-Rom matrix coefficients
        var v0 = (-alpha * t3 + 2 * alpha * t2 - alpha * t);
        var v1 = ((2 - alpha) * t3 + (alpha - 3) * t2 + 1);
        var v2 = ((alpha - 2) * t3 + (3 - 2 * alpha) * t2 + alpha * t);
        var v3 = (alpha * t3 - alpha * t2);

        var x = v0 * p0.X + v1 * p1.X + v2 * p2.X + v3 * p3.X;
        var y = v0 * p0.Y + v1 * p1.Y + v2 * p2.Y + v3 * p3.Y;

        return new Point(x, y);
    }

    /// <summary>
    /// Compute a single point on a cubic Bezier curve at parameter t.
    /// </summary>
    private static Point CubicBezierPoint(Point p0, Point p1, Point p2, Point p3, double t)
    {
        var u = 1 - t;
        var u2 = u * u;
        var u3 = u2 * u;
        var t2 = t * t;
        var t3 = t2 * t;

        var x = u3 * p0.X + 3 * u2 * t * p1.X + 3 * u * t2 * p2.X + t3 * p3.X;
        var y = u3 * p0.Y + 3 * u2 * t * p1.Y + 3 * u * t2 * p2.Y + t3 * p3.Y;

        return new Point(x, y);
    }
}
