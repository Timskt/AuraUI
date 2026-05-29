using Avalonia;
using Avalonia.Media;

namespace AuraUI.Controls.Charts.Graph;

/// <summary>
/// Represents a directed or undirected edge between two graph nodes.
/// Supports straight lines, quadratic curves, cubic curves, arrows, and labels.
/// </summary>
public class GraphEdge
{
    /// <summary>ID of the source node.</summary>
    public string Source { get; set; } = string.Empty;

    /// <summary>ID of the target node.</summary>
    public string Target { get; set; } = string.Empty;

    /// <summary>Edge weight (affects rendering thickness and layout).</summary>
    public double Weight { get; set; } = 1.0;

    /// <summary>Line style (solid, dashed, dotted).</summary>
    public EdgeLineStyle Style { get; set; } = EdgeLineStyle.Solid;

    /// <summary>Curve type for edge path.</summary>
    public EdgeCurveType CurveType { get; set; } = EdgeCurveType.Straight;

    /// <summary>Arrow configuration.</summary>
    public ArrowPosition Arrow { get; set; } = ArrowPosition.Target;

    /// <summary>Arrow size in pixels.</summary>
    public double ArrowSize { get; set; } = 8;

    /// <summary>Edge label text.</summary>
    public string? Label { get; set; }

    /// <summary>Font size for the edge label.</summary>
    public double LabelFontSize { get; set; } = 10;

    /// <summary>
    /// Offset for curve bending. Positive values bend one way, negative the other.
    /// Only used for Quadratic and Cubic curve types.
    /// </summary>
    public double CurveOffset { get; set; }

    /// <summary>Stroke color brush for the edge.</summary>
    public IBrush? Color { get; set; }

    /// <summary>Line thickness.</summary>
    public double Thickness { get; set; } = 1.5;

    /// <summary>Current visual state.</summary>
    public GraphElementState State { get; set; } = GraphElementState.Default;

    /// <summary>Whether this edge is currently selected.</summary>
    public bool IsSelected { get; set; }

    /// <summary>Whether this edge is currently hidden.</summary>
    public bool IsHidden { get; set; }

    /// <summary>User-defined data payload.</summary>
    public object? Data { get; set; }

    /// <summary>
    /// Renders this edge between the given source and target world positions.
    /// </summary>
    public void Render(DrawingContext context, Point sourcePos, Point targetPos, double zoom)
    {
        if (IsHidden) return;

        var effectiveColor = GetEffectiveColor();
        var pen = CreatePen(effectiveColor, zoom);
        var arrowSize = ArrowSize * zoom;

        // Compute the curve path
        StreamGeometry geometry;
        Point? controlPoint;
        Point? controlPoint2;
        ComputeCurve(sourcePos, targetPos, zoom, out geometry, out controlPoint, out controlPoint2);

        // Draw the edge path
        context.DrawGeometry(null, pen, geometry);

        // Draw arrows
        if (Arrow is ArrowPosition.Target or ArrowPosition.Both)
        {
            var arrowDir = controlPoint2.HasValue
                ? Normalize(Subtract(targetPos, controlPoint2.Value))
                : Normalize(Subtract(targetPos, sourcePos));
            DrawArrowhead(context, targetPos, arrowDir, arrowSize, effectiveColor);
        }

        if (Arrow is ArrowPosition.Source or ArrowPosition.Both)
        {
            var arrowDir = controlPoint.HasValue
                ? Normalize(Subtract(sourcePos, controlPoint.Value))
                : Normalize(Subtract(sourcePos, targetPos));
            DrawArrowhead(context, sourcePos, arrowDir, arrowSize, effectiveColor);
        }

        // Draw label at midpoint
        if (!string.IsNullOrEmpty(Label))
        {
            var mid = new Point((sourcePos.X + targetPos.X) / 2, (sourcePos.Y + targetPos.Y) / 2);
            IBrush labelBrush = State == GraphElementState.Inactive
                ? new SolidColorBrush(Colors.Gray, 0.5)
                : Brushes.DarkSlateGray;
            var ft = new FormattedText(Label,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface("Segoe UI"),
                LabelFontSize * zoom,
                labelBrush);
            context.DrawText(ft, new Point(mid.X - ft.Width / 2, mid.Y - ft.Height - 4 * zoom));
        }
    }

    /// <summary>
    /// Hit-tests whether the given point is close to this edge's path.
    /// </summary>
    public bool HitTest(Point sourcePos, Point targetPos, Point testPoint, double tolerance = 6.0)
    {
        if (CurveType == EdgeCurveType.Straight)
        {
            return DistanceToSegment(testPoint, sourcePos, targetPos) <= tolerance;
        }

        // For curves, sample along the path
        for (double t = 0; t <= 1.0; t += 0.05)
        {
            var p = SampleCurve(sourcePos, targetPos, t);
            var dx = testPoint.X - p.X;
            var dy = testPoint.Y - p.Y;
            if (dx * dx + dy * dy <= tolerance * tolerance)
                return true;
        }
        return false;
    }

    private void ComputeCurve(Point source, Point target, double zoom,
        out StreamGeometry geometry, out Point? cp1, out Point? cp2)
    {
        cp1 = null;
        cp2 = null;

        var streamGeom = new StreamGeometry();
        using (var ctx = streamGeom.Open())
        {
            switch (CurveType)
            {
                case EdgeCurveType.Straight:
                    ctx.BeginFigure(source, false);
                    ctx.LineTo(target);
                    break;

                case EdgeCurveType.Quadratic:
                {
                    var mid = new Point((source.X + target.X) / 2, (source.Y + target.Y) / 2);
                    var perp = Perpendicular(Normalize(Subtract(target, source)));
                    cp1 = new Point(
                        mid.X + perp.X * CurveOffset * zoom,
                        mid.Y + perp.Y * CurveOffset * zoom);
                    ctx.BeginFigure(source, false);
                    ctx.QuadraticBezierTo(cp1.Value, target);
                    break;
                }

                case EdgeCurveType.Cubic:
                {
                    var dx = target.X - source.X;
                    var dy = target.Y - source.Y;
                    var offset = CurveOffset * zoom;
                    cp1 = new Point(source.X + dx * 0.33 + offset, source.Y + dy * 0.33 - offset);
                    cp2 = new Point(source.X + dx * 0.66 - offset, source.Y + dy * 0.66 + offset);
                    ctx.BeginFigure(source, false);
                    ctx.CubicBezierTo(cp1.Value, cp2.Value, target);
                    break;
                }
            }
        }

        geometry = streamGeom;
    }

    private Point SampleCurve(Point source, Point target, double t)
    {
        return CurveType switch
        {
            EdgeCurveType.Straight => new Point(
                source.X + (target.X - source.X) * t,
                source.Y + (target.Y - source.Y) * t),
            EdgeCurveType.Quadratic => QuadraticBezier(source, GetQuadControl(source, target), target, t),
            EdgeCurveType.Cubic => CubicBezier(source, GetCubicControl1(source, target),
                GetCubicControl2(source, target), target, t),
            _ => source
        };
    }

    private Point GetQuadControl(Point source, Point target)
    {
        var mid = new Point((source.X + target.X) / 2, (source.Y + target.Y) / 2);
        var perp = Perpendicular(Normalize(Subtract(target, source)));
        return new Point(mid.X + perp.X * CurveOffset, mid.Y + perp.Y * CurveOffset);
    }

    private Point GetCubicControl1(Point source, Point target)
    {
        var dx = target.X - source.X;
        var dy = target.Y - source.Y;
        return new Point(source.X + dx * 0.33 + CurveOffset, source.Y + dy * 0.33 - CurveOffset);
    }

    private Point GetCubicControl2(Point source, Point target)
    {
        var dx = target.X - source.X;
        var dy = target.Y - source.Y;
        return new Point(source.X + dx * 0.66 - CurveOffset, source.Y + dy * 0.66 + CurveOffset);
    }

    private static Point QuadraticBezier(Point p0, Point p1, Point p2, double t)
    {
        var mt = 1 - t;
        return new Point(mt * mt * p0.X + 2 * mt * t * p1.X + t * t * p2.X,
                         mt * mt * p0.Y + 2 * mt * t * p1.Y + t * t * p2.Y);
    }

    private static Point CubicBezier(Point p0, Point p1, Point p2, Point p3, double t)
    {
        var mt = 1 - t;
        return new Point(
            mt * mt * mt * p0.X + 3 * mt * mt * t * p1.X + 3 * mt * t * t * p2.X + t * t * t * p3.X,
            mt * mt * mt * p0.Y + 3 * mt * mt * t * p1.Y + 3 * mt * t * t * p2.Y + t * t * t * p3.Y);
    }

    private Pen CreatePen(IBrush? brush, double zoom)
    {
        var b = brush ?? Brushes.LightGray;
        var dash = Style switch
        {
            EdgeLineStyle.Dashed => new DashStyle(new double[] { 6, 3 }, 0),
            EdgeLineStyle.Dotted => new DashStyle(new double[] { 2, 3 }, 0),
            _ => null
        };
        return dash != null
            ? new Pen(b, Thickness * zoom, dash)
            : new Pen(b, Thickness * zoom);
    }

    private IBrush? GetEffectiveColor()
    {
        return State switch
        {
            GraphElementState.Inactive => new SolidColorBrush(Colors.LightGray, 0.3),
            GraphElementState.Selected => Color ?? Brushes.DodgerBlue,
            GraphElementState.Active => Color ?? Brushes.CornflowerBlue,
            _ => Color ?? Brushes.LightGray
        };
    }

    private void DrawArrowhead(DrawingContext context, Point tip, Point direction, double size, IBrush? brush)
    {
        var angle = Math.Atan2(direction.Y, direction.X);
        var a1 = angle + Math.PI * 0.85;
        var a2 = angle - Math.PI * 0.85;
        var p1 = new Point(tip.X + size * Math.Cos(a1), tip.Y + size * Math.Sin(a1));
        var p2 = new Point(tip.X + size * Math.Cos(a2), tip.Y + size * Math.Sin(a2));

        var geometry = new StreamGeometry();
        using (var ctx = geometry.Open())
        {
            ctx.BeginFigure(tip, true);
            ctx.LineTo(p1);
            ctx.LineTo(p2);
            ctx.EndFigure(true);
        }
        context.DrawGeometry(brush, null, geometry);
    }

    // ── Vector math helpers ──

    private static Point Subtract(Point a, Point b) => new(a.X - b.X, a.Y - b.Y);
    private static Point Normalize(Point v)
    {
        var len = Math.Sqrt(v.X * v.X + v.Y * v.Y);
        return len > 1e-10 ? new Point(v.X / len, v.Y / len) : new Point(0, 0);
    }
    private static Point Perpendicular(Point v) => new(-v.Y, v.X);

    private static double DistanceToSegment(Point p, Point a, Point b)
    {
        var dx = b.X - a.X;
        var dy = b.Y - a.Y;
        var lenSq = dx * dx + dy * dy;
        if (lenSq < 1e-10) return Math.Sqrt((p.X - a.X) * (p.X - a.X) + (p.Y - a.Y) * (p.Y - a.Y));
        var t = Math.Clamp(((p.X - a.X) * dx + (p.Y - a.Y) * dy) / lenSq, 0, 1);
        var proj = new Point(a.X + t * dx, a.Y + t * dy);
        return Math.Sqrt((p.X - proj.X) * (p.X - proj.X) + (p.Y - proj.Y) * (p.Y - proj.Y));
    }
}
