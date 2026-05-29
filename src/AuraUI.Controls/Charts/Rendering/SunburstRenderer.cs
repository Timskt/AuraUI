using Avalonia;
using Avalonia.Media;
using AuraUI.Controls.Charts.Series;

namespace AuraUI.Controls.Charts.Rendering;

/// <summary>
/// Renders SunburstSeries as concentric ring segments. Each ring level
/// represents a depth of the hierarchy, with arc angles proportional to value.
/// </summary>
public class SunburstRenderer : IChartRenderer
{
    public string Key => "Sunburst";

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
        if (series is not SunburstSeries sunburst || !series.IsVisible) return;
        if (sunburst.Nodes.Count == 0) return;

        var center = new Point(plotArea.Center.X, plotArea.Center.Y);
        var maxRadius = Math.Min(plotArea.Width, plotArea.Height) / 2 - 10;
        var innerRadius = maxRadius * sunburst.InnerRadius;

        // Flatten hierarchy into arc segments
        var segments = new List<SunburstSegment>();
        var totalValue = sunburst.Nodes.Where(n => n.Value > 0).Sum(n => n.Value);
        if (totalValue <= 0) return;

        double startAngle = -90;
        foreach (var node in sunburst.Nodes)
        {
            if (node.Value <= 0) continue;
            var sweep = (node.Value / totalValue) * 360;
            FlattenHierarchy(node, startAngle, sweep, 0, sunburst.MaxVisibleLevels, segments);
            startAngle += sweep;
        }

        // Compute ring widths
        var maxDepth = segments.Count > 0 ? segments.Max(s => s.Depth) : 0;
        var ringWidth = maxDepth > 0
            ? (maxRadius - innerRadius) / (maxDepth + 1)
            : maxRadius - innerRadius;

        // Draw segments
        for (int i = 0; i < segments.Count; i++)
        {
            var seg = segments[i];
            var segInner = innerRadius + seg.Depth * ringWidth;
            var segOuter = segInner + ringWidth - 1; // 1px gap

            var animatedSweep = seg.Sweep * progress;
            if (animatedSweep < 0.1) continue;

            var color = seg.Color ?? GetSunburstColor(i);
            var brush = new SolidColorBrush(((SolidColorBrush)color).Color);

            var geometry = BuildArcGeometry(center, segInner, segOuter,
                seg.StartAngle + sunburst.PadAngle / 2,
                animatedSweep - sunburst.PadAngle);

            if (geometry != null)
            {
                context.DrawGeometry(brush, null, geometry);
            }

            // Labels
            if (sunburst.ShowLabels && progress >= 1.0 && !string.IsNullOrEmpty(seg.Name))
            {
                var midAngle = (seg.StartAngle + seg.Sweep / 2) * Math.PI / 180;
                var labelRadius = (segInner + segOuter) / 2;
                var labelX = center.X + labelRadius * Math.Cos(midAngle);
                var labelY = center.Y + labelRadius * Math.Sin(midAngle);

                var fontSize = Math.Min(ringWidth * 0.3, 11);
                if (fontSize >= 7 && seg.Sweep > 15)
                {
                    var formattedText = new FormattedText(seg.Name!,
                        System.Globalization.CultureInfo.CurrentCulture,
                        FlowDirection.LeftToRight,
                        new Typeface("Segoe UI", FontStyle.Normal, FontWeight.Normal),
                        fontSize,
                        Brushes.White);

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
        IReadOnlyList<ChartSeries> allSeries)
    {
        if (series is not SunburstSeries sunburst) return null;

        var center = new Point(plotArea.Center.X, plotArea.Center.Y);
        var maxRadius = Math.Min(plotArea.Width, plotArea.Height) / 2 - 10;
        var innerRadius = maxRadius * sunburst.InnerRadius;

        var dx = pointerPosition.X - center.X;
        var dy = pointerPosition.Y - center.Y;
        var dist = Math.Sqrt(dx * dx + dy * dy);

        if (dist < innerRadius || dist > maxRadius) return null;

        var angle = Math.Atan2(dy, dx) * 180 / Math.PI;
        if (angle < -90) angle += 360;

        // Flatten and check
        var segments = new List<SunburstSegment>();
        var totalValue = sunburst.Nodes.Where(n => n.Value > 0).Sum(n => n.Value);
        if (totalValue <= 0) return null;

        double startAngle = -90;
        foreach (var node in sunburst.Nodes)
        {
            if (node.Value <= 0) continue;
            var sweep = (node.Value / totalValue) * 360;
            FlattenHierarchy(node, startAngle, sweep, 0, sunburst.MaxVisibleLevels, segments);
            startAngle += sweep;
        }

        var maxDepth = segments.Count > 0 ? segments.Max(s => s.Depth) : 0;
        var ringWidth = maxDepth > 0 ? (maxRadius - innerRadius) / (maxDepth + 1) : maxRadius - innerRadius;

        var depth = (int)((dist - innerRadius) / ringWidth);

        for (int i = 0; i < segments.Count; i++)
        {
            var seg = segments[i];
            if (seg.Depth != depth) continue;

            var segStart = seg.StartAngle;
            var segEnd = seg.StartAngle + seg.Sweep;

            // Normalize angle to match
            var normAngle = angle;
            while (normAngle < segStart) normAngle += 360;
            while (normAngle > segEnd + 360) normAngle -= 360;

            if (normAngle >= segStart && normAngle <= segEnd)
            {
                return new ChartHitResult
                {
                    Series = series,
                    DataIndex = i,
                    HitPosition = pointerPosition
                };
            }
        }

        return null;
    }

    private static void FlattenHierarchy(ChartSunburstNode node, double startAngle, double sweep,
        int depth, int maxDepth, List<SunburstSegment> result)
    {
        if (depth > maxDepth) return;

        result.Add(new SunburstSegment
        {
            Name = node.Name,
            StartAngle = startAngle,
            Sweep = sweep,
            Depth = depth,
            Color = node.Color
        });

        if (node.Children.Count > 0)
        {
            var childTotal = node.Children.Where(c => c.Value > 0).Sum(c => c.Value);
            if (childTotal > 0)
            {
                var childStart = startAngle;
                foreach (var child in node.Children)
                {
                    if (child.Value <= 0) continue;
                    var childSweep = (child.Value / childTotal) * sweep;
                    FlattenHierarchy(child, childStart, childSweep, depth + 1, maxDepth, result);
                    childStart += childSweep;
                }
            }
        }
    }

    private static PathGeometry? BuildArcGeometry(Point center, double innerRadius, double outerRadius,
        double startAngleDeg, double sweepAngleDeg)
    {
        if (Math.Abs(sweepAngleDeg) < 0.1) return null;

        var startRad = startAngleDeg * Math.PI / 180;
        var endRad = (startAngleDeg + sweepAngleDeg) * Math.PI / 180;

        var outerStart = new Point(center.X + outerRadius * Math.Cos(startRad), center.Y + outerRadius * Math.Sin(startRad));
        var outerEnd = new Point(center.X + outerRadius * Math.Cos(endRad), center.Y + outerRadius * Math.Sin(endRad));
        var innerEnd = new Point(center.X + innerRadius * Math.Cos(endRad), center.Y + innerRadius * Math.Sin(endRad));
        var innerStart = new Point(center.X + innerRadius * Math.Cos(startRad), center.Y + innerRadius * Math.Sin(startRad));

        var geometry = new PathGeometry();
        var figure = new PathFigure { StartPoint = outerStart, IsClosed = true };

        // Outer arc
        figure.Segments!.Add(new ArcSegment
        {
            Point = outerEnd,
            Size = new Size(outerRadius, outerRadius),
            IsLargeArc = sweepAngleDeg > 180,
            SweepDirection = SweepDirection.Clockwise
        });

        // Line to inner
        figure.Segments!.Add(new LineSegment { Point = innerEnd });

        // Inner arc back
        figure.Segments!.Add(new ArcSegment
        {
            Point = innerStart,
            Size = new Size(innerRadius, innerRadius),
            IsLargeArc = sweepAngleDeg > 180,
            SweepDirection = (SweepDirection)1 // Counterclockwise
        });

        geometry.Figures!.Add(figure);
        return geometry;
    }

    private static IBrush GetSunburstColor(int index)
    {
        return new SolidColorBrush(LineRenderer.DefaultPalette[index % LineRenderer.DefaultPalette.Length]);
    }

    private class SunburstSegment
    {
        public string? Name { get; set; }
        public double StartAngle { get; set; }
        public double Sweep { get; set; }
        public int Depth { get; set; }
        public IBrush? Color { get; set; }
    }
}
