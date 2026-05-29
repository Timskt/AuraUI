using Avalonia;
using Avalonia.Media;

namespace AuraUI.Core.Utilities;

/// <summary>
/// Static helper methods for creating <see cref="StreamGeometry"/> shapes
/// commonly used in custom controls and adorners.
/// </summary>
public static class GeometryUtil
{
    /// <summary>
    /// Creates an arc (partial circle) geometry.
    /// </summary>
    /// <param name="center">The center point of the arc.</param>
    /// <param name="radius">The radius of the arc.</param>
    /// <param name="startAngle">The start angle in degrees (0 = top, clockwise).</param>
    /// <param name="sweepAngle">The sweep angle in degrees (positive = clockwise).</param>
    /// <returns>A <see cref="StreamGeometry"/> representing the arc.</returns>
    public static StreamGeometry CreateArc(Point center, double radius, double startAngle, double sweepAngle)
    {
        var geometry = new StreamGeometry();

        using (var ctx = geometry.Open())
        {
            var startPoint = PolarToCartesian(center, radius, startAngle);
            ctx.BeginFigure(startPoint, false);

            const double step = 10.0;
            double current = 0;
            double absSweep = Math.Abs(sweepAngle);
            int direction = sweepAngle >= 0 ? 1 : -1;

            while (current < absSweep)
            {
                double next = Math.Min(current + step, absSweep);
                double angle = startAngle + (next * direction);
                var point = PolarToCartesian(center, radius, angle);
                ctx.LineTo(point);
                current = next;
            }
        }

        return geometry;
    }

    /// <summary>
    /// Creates a rounded rectangle geometry.
    /// </summary>
    /// <param name="rect">The bounding rectangle.</param>
    /// <param name="cornerRadius">The corner radii for each corner.</param>
    /// <returns>A <see cref="StreamGeometry"/> representing the rounded rectangle.</returns>
    public static StreamGeometry CreateRoundedRect(Rect rect, CornerRadius cornerRadius)
    {
        var geometry = new StreamGeometry();

        using (var ctx = geometry.Open())
        {
            double left = rect.X;
            double top = rect.Y;
            double right = rect.Right;
            double bottom = rect.Bottom;

            double tl = Math.Min(cornerRadius.TopLeft, Math.Min(rect.Width / 2, rect.Height / 2));
            double tr = Math.Min(cornerRadius.TopRight, Math.Min(rect.Width / 2, rect.Height / 2));
            double br = Math.Min(cornerRadius.BottomRight, Math.Min(rect.Width / 2, rect.Height / 2));
            double bl = Math.Min(cornerRadius.BottomLeft, Math.Min(rect.Width / 2, rect.Height / 2));

            ctx.BeginFigure(new Point(left + tl, top), true);

            // Top edge
            ctx.LineTo(new Point(right - tr, top));
            // Top-right corner
            ctx.ArcTo(new Point(right, top + tr), new Size(tr, tr), 0, false, SweepDirection.Clockwise);
            // Right edge
            ctx.LineTo(new Point(right, bottom - br));
            // Bottom-right corner
            ctx.ArcTo(new Point(right - br, bottom), new Size(br, br), 0, false, SweepDirection.Clockwise);
            // Bottom edge
            ctx.LineTo(new Point(bl, bottom));
            // Bottom-left corner
            ctx.ArcTo(new Point(left, bottom - bl), new Size(bl, bl), 0, false, SweepDirection.Clockwise);
            // Left edge
            ctx.LineTo(new Point(left, top + tl));
            // Top-left corner
            ctx.ArcTo(new Point(left + tl, top), new Size(tl, tl), 0, false, SweepDirection.Clockwise);
        }

        return geometry;
    }

    /// <summary>
    /// Creates a star (polygon) geometry with alternating outer and inner vertices.
    /// </summary>
    /// <param name="center">The center point of the star.</param>
    /// <param name="outerRadius">The radius to the outer points.</param>
    /// <param name="innerRadius">The radius to the inner points.</param>
    /// <param name="points">The number of star points (e.g. 5 for a five-pointed star).</param>
    /// <returns>A <see cref="StreamGeometry"/> representing the star.</returns>
    public static StreamGeometry CreateStar(Point center, double outerRadius, double innerRadius, int points)
    {
        if (points < 2)
            throw new ArgumentOutOfRangeException(nameof(points), "Star must have at least 2 points.");

        var geometry = new StreamGeometry();
        int totalVertices = points * 2;
        double angleStep = 360.0 / totalVertices;

        using (var ctx = geometry.Open())
        {
            var firstPoint = PolarToCartesian(center, outerRadius, -90);
            ctx.BeginFigure(firstPoint, true);

            for (int i = 1; i < totalVertices; i++)
            {
                double angle = -90 + (i * angleStep);
                double radius = i % 2 == 0 ? outerRadius : innerRadius;
                var point = PolarToCartesian(center, radius, angle);
                ctx.LineTo(point);
            }
        }

        return geometry;
    }

    /// <summary>
    /// Creates a speech-bubble path with a rounded rectangle body and a triangular arrow.
    /// </summary>
    /// <param name="contentRect">The bounding rectangle of the bubble body.</param>
    /// <param name="arrowTip">The point where the arrow tip should appear.</param>
    /// <param name="arrowSize">The size (width/height) of the arrow triangle.</param>
    /// <param name="cornerRadius">The corner radii for the bubble body.</param>
    /// <returns>A <see cref="StreamGeometry"/> representing the bubble path.</returns>
    public static StreamGeometry CreateBubblePath(Rect contentRect, Point arrowTip, double arrowSize, CornerRadius cornerRadius)
    {
        var geometry = new StreamGeometry();
        double halfArrow = arrowSize / 2;

        // Determine which edge the arrow sits on (closest edge)
        double distTop = Math.Abs(arrowTip.Y - contentRect.Top);
        double distBottom = Math.Abs(arrowTip.Y - contentRect.Bottom);
        double distLeft = Math.Abs(arrowTip.X - contentRect.Left);
        double distRight = Math.Abs(arrowTip.X - contentRect.Right);

        bool onBottom = distBottom <= distTop && distBottom <= distLeft && distBottom <= distRight;
        bool onLeft = !onBottom && distLeft <= distTop && distLeft <= distRight;
        bool onRight = !onBottom && !onLeft && distRight <= distTop;
        // Default to top if none match

        using (var ctx = geometry.Open())
        {
            double left = contentRect.X;
            double top = contentRect.Y;
            double right = contentRect.Right;
            double bottom = contentRect.Bottom;

            double tl = Math.Min(cornerRadius.TopLeft, Math.Min(contentRect.Width / 2, contentRect.Height / 2));
            double tr = Math.Min(cornerRadius.TopRight, Math.Min(contentRect.Width / 2, contentRect.Height / 2));
            double br = Math.Min(cornerRadius.BottomRight, Math.Min(contentRect.Width / 2, contentRect.Height / 2));
            double bl = Math.Min(cornerRadius.BottomLeft, Math.Min(contentRect.Width / 2, contentRect.Height / 2));

            if (onBottom)
            {
                // Arrow on bottom edge
                double clampX = Math.Clamp(arrowTip.X, left + bl + halfArrow, right - br - halfArrow);

                ctx.BeginFigure(new Point(left + tl, top), true);
                ctx.LineTo(new Point(right - tr, top));
                ctx.ArcTo(new Point(right, top + tr), new Size(tr, tr), 0, false, SweepDirection.Clockwise);
                ctx.LineTo(new Point(right, bottom - br));
                ctx.ArcTo(new Point(right - br, bottom), new Size(br, br), 0, false, SweepDirection.Clockwise);
                ctx.LineTo(new Point(clampX + halfArrow, bottom));
                ctx.LineTo(arrowTip);
                ctx.LineTo(new Point(clampX - halfArrow, bottom));
                ctx.LineTo(new Point(bl, bottom));
                ctx.ArcTo(new Point(left, bottom - bl), new Size(bl, bl), 0, false, SweepDirection.Clockwise);
                ctx.LineTo(new Point(left, top + tl));
                ctx.ArcTo(new Point(left + tl, top), new Size(tl, tl), 0, false, SweepDirection.Clockwise);
            }
            else if (onLeft)
            {
                // Arrow on left edge
                double clampY = Math.Clamp(arrowTip.Y, top + tl + halfArrow, bottom - bl - halfArrow);

                ctx.BeginFigure(new Point(left + tl, top), true);
                ctx.LineTo(new Point(right - tr, top));
                ctx.ArcTo(new Point(right, top + tr), new Size(tr, tr), 0, false, SweepDirection.Clockwise);
                ctx.LineTo(new Point(right, bottom - br));
                ctx.ArcTo(new Point(right - br, bottom), new Size(br, br), 0, false, SweepDirection.Clockwise);
                ctx.LineTo(new Point(bl, bottom));
                ctx.ArcTo(new Point(left, bottom - bl), new Size(bl, bl), 0, false, SweepDirection.Clockwise);
                ctx.LineTo(new Point(left, clampY + halfArrow));
                ctx.LineTo(arrowTip);
                ctx.LineTo(new Point(left, clampY - halfArrow));
                ctx.LineTo(new Point(left, top + tl));
                ctx.ArcTo(new Point(left + tl, top), new Size(tl, tl), 0, false, SweepDirection.Clockwise);
            }
            else if (onRight)
            {
                // Arrow on right edge
                double clampY = Math.Clamp(arrowTip.Y, top + tr + halfArrow, bottom - br - halfArrow);

                ctx.BeginFigure(new Point(left + tl, top), true);
                ctx.LineTo(new Point(right - tr, top));
                ctx.ArcTo(new Point(right, top + tr), new Size(tr, tr), 0, false, SweepDirection.Clockwise);
                ctx.LineTo(new Point(right, clampY - halfArrow));
                ctx.LineTo(arrowTip);
                ctx.LineTo(new Point(right, clampY + halfArrow));
                ctx.LineTo(new Point(right, bottom - br));
                ctx.ArcTo(new Point(right - br, bottom), new Size(br, br), 0, false, SweepDirection.Clockwise);
                ctx.LineTo(new Point(bl, bottom));
                ctx.ArcTo(new Point(left, bottom - bl), new Size(bl, bl), 0, false, SweepDirection.Clockwise);
                ctx.LineTo(new Point(left, top + tl));
                ctx.ArcTo(new Point(left + tl, top), new Size(tl, tl), 0, false, SweepDirection.Clockwise);
            }
            else
            {
                // Arrow on top edge (default)
                double clampX = Math.Clamp(arrowTip.X, left + tl + halfArrow, right - tr - halfArrow);

                ctx.BeginFigure(new Point(left + tl, top), true);
                ctx.LineTo(new Point(clampX - halfArrow, top));
                ctx.LineTo(arrowTip);
                ctx.LineTo(new Point(clampX + halfArrow, top));
                ctx.LineTo(new Point(right - tr, top));
                ctx.ArcTo(new Point(right, top + tr), new Size(tr, tr), 0, false, SweepDirection.Clockwise);
                ctx.LineTo(new Point(right, bottom - br));
                ctx.ArcTo(new Point(right - br, bottom), new Size(br, br), 0, false, SweepDirection.Clockwise);
                ctx.LineTo(new Point(bl, bottom));
                ctx.ArcTo(new Point(left, bottom - bl), new Size(bl, bl), 0, false, SweepDirection.Clockwise);
                ctx.LineTo(new Point(left, top + tl));
                ctx.ArcTo(new Point(left + tl, top), new Size(tl, tl), 0, false, SweepDirection.Clockwise);
            }
        }

        return geometry;
    }

    /// <summary>
    /// Converts polar coordinates to a Cartesian point.
    /// </summary>
    /// <param name="center">The origin point.</param>
    /// <param name="radius">The distance from the origin.</param>
    /// <param name="angleDegrees">The angle in degrees (0 = right, clockwise).</param>
    /// <returns>The Cartesian point.</returns>
    public static Point PolarToCartesian(Point center, double radius, double angleDegrees)
    {
        double angleRadians = angleDegrees * Math.PI / 180.0;
        return new Point(
            center.X + radius * Math.Cos(angleRadians),
            center.Y + radius * Math.Sin(angleRadians));
    }
}
