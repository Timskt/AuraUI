using Avalonia;
using Avalonia.Media;
using AuraUI.Controls.Charts.Series;

namespace AuraUI.Controls.Charts.Rendering;

/// <summary>
/// Renders GraphSeries as a network diagram with nodes and edges.
/// Supports both pre-positioned and force-directed layouts.
/// </summary>
public class GraphRenderer : IChartRenderer
{
    public string Key => "Graph";

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
        if (series is not GraphSeries graph || !series.IsVisible) return;
        if (graph.Nodes.Count == 0) return;

        var nodes = graph.Nodes;
        var edges = graph.Edges;

        // Map node positions to plot area
        var positions = MapNodePositions(nodes, plotArea, graph);

        // Draw edges
        var edgePen = new Pen(Brushes.Gray, graph.EdgeWidth);
        foreach (var edge in edges)
        {
            if (edge.Source < 0 || edge.Source >= positions.Length) continue;
            if (edge.Target < 0 || edge.Target >= positions.Length) continue;

            var edgeColor = edge.Color ?? Brushes.Gray;
            var brush = new SolidColorBrush(((SolidColorBrush)edgeColor).Color)
            {
                Opacity = graph.EdgeOpacity
            };

            var p1 = positions[edge.Source];
            var p2 = positions[edge.Target];

            var midPoint = new Point((p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2);

            if (graph.IsDirected)
            {
                // Draw with arrow head
                var angle = Math.Atan2(p2.Y - p1.Y, p2.X - p1.X);
                var arrowLength = 8.0;
                var arrowAngle = Math.PI / 6;

                // Shorten line to not overlap with arrow
                var targetNode = nodes[edge.Target];
                var shortenDist = targetNode.Size / 2;
                var shortened = new Point(
                    p2.X - shortenDist * Math.Cos(angle),
                    p2.Y - shortenDist * Math.Sin(angle));

                context.DrawLine(new Pen(brush, graph.EdgeWidth), p1, shortened);

                // Arrow head
                var arrow1 = new Point(
                    shortened.X - arrowLength * Math.Cos(angle - arrowAngle),
                    shortened.Y - arrowLength * Math.Sin(angle - arrowAngle));
                var arrow2 = new Point(
                    shortened.X - arrowLength * Math.Cos(angle + arrowAngle),
                    shortened.Y - arrowLength * Math.Sin(angle + arrowAngle));

                var arrowGeometry = new PathGeometry();
                var arrowFigure = new PathFigure { StartPoint = shortened, IsClosed = true };
                arrowFigure.Segments!.Add(new LineSegment { Point = arrow1 });
                arrowFigure.Segments!.Add(new LineSegment { Point = arrow2 });
                arrowGeometry.Figures!.Add(arrowFigure);
                context.DrawGeometry(brush, null, arrowGeometry);
            }
            else
            {
                context.DrawLine(new Pen(brush, graph.EdgeWidth), p1, p2);
            }
        }

        // Draw nodes
        for (int i = 0; i < nodes.Count; i++)
        {
            var node = nodes[i];
            var pos = positions[i];
            var radius = node.Size * Math.Min(progress, 1.0);

            var color = node.Color;
            if (color == null && node.Category >= 0 && node.Category < graph.Categories.Count)
            {
                color = graph.Categories[node.Category].Color;
            }
            color ??= GetGraphColor(i);

            var brush = new SolidColorBrush(((SolidColorBrush)color).Color);
            var pen = new Pen(Brushes.White, 2);

            context.DrawEllipse(brush, pen, pos, radius, radius);

            if (graph.ShowLabels && progress >= 1.0 && !string.IsNullOrEmpty(node.Name))
            {
                var formattedText = new FormattedText(node.Name!,
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Segoe UI", FontStyle.Normal, FontWeight.Normal),
                    10,
                    Brushes.Black);

                context.DrawText(formattedText,
                    new Point(pos.X - formattedText.Width / 2, pos.Y + radius + 2));
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
        if (series is not GraphSeries graph) return null;

        var positions = MapNodePositions(graph.Nodes, plotArea, graph);

        for (int i = graph.Nodes.Count - 1; i >= 0; i--)
        {
            var dx = pointerPosition.X - positions[i].X;
            var dy = pointerPosition.Y - positions[i].Y;
            var dist = Math.Sqrt(dx * dx + dy * dy);

            if (dist <= graph.Nodes[i].Size)
            {
                return new ChartHitResult
                {
                    Series = series,
                    DataIndex = i,
                    HitPosition = positions[i],
                    Distance = dist
                };
            }
        }

        return null;
    }

    private static Point[] MapNodePositions(IReadOnlyList<ChartGraphNode> nodes, Rect plotArea, GraphSeries graph)
    {
        var positions = new Point[nodes.Count];

        // Find coordinate bounds
        double minX = double.MaxValue, maxX = double.MinValue;
        double minY = double.MaxValue, maxY = double.MinValue;
        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i].X < minX) minX = nodes[i].X;
            if (nodes[i].X > maxX) maxX = nodes[i].X;
            if (nodes[i].Y < minY) minY = nodes[i].Y;
            if (nodes[i].Y > maxY) maxY = nodes[i].Y;
        }

        var rangeX = maxX - minX;
        var rangeY = maxY - minY;
        if (rangeX < 1e-10) rangeX = 1;
        if (rangeY < 1e-10) rangeY = 1;

        var padding = 20;
        var availWidth = plotArea.Width - padding * 2;
        var availHeight = plotArea.Height - padding * 2;

        for (int i = 0; i < nodes.Count; i++)
        {
            var nx = plotArea.Left + padding + ((nodes[i].X - minX) / rangeX) * availWidth;
            var ny = plotArea.Top + padding + ((nodes[i].Y - minY) / rangeY) * availHeight;
            positions[i] = new Point(nx, ny);
        }

        return positions;
    }

    private static IBrush GetGraphColor(int index)
    {
        return new SolidColorBrush(LineRenderer.DefaultPalette[index % LineRenderer.DefaultPalette.Length]);
    }
}
