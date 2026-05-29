using Avalonia;
using Avalonia.Media;

namespace AuraUI.Controls.Charts.Rendering;

/// <summary>
/// Renders TreeSeries as a node-link tree diagram.
///
/// Algorithm:
///   1. Compute layout: assign (x, y) to each node based on depth and sibling order
///   2. Map layout coordinates to plot area pixels
///   3. Draw edges (orthogonal or curved) from parent to child
///   4. Draw nodes as circles
///   5. Draw labels next to nodes
///
/// Layout modes:
///   - LR/RL/TB/BT: orthogonal tree layout
///   - Radial: nodes arranged in concentric circles by depth
/// </summary>
public class TreeRenderer : IChartRenderer
{
    public string Key => "Tree";

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
        if (series is not Series.TreeSeries tree || !series.IsVisible) return;

        var roots = tree.Nodes;
        if (roots.Count == 0) return;

        if (tree.Mode == TreeMode.Radial)
        {
            RenderRadial(context, tree, roots, plotArea, progress);
        }
        else
        {
            RenderOrthogonal(context, tree, roots, plotArea, progress);
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
        if (series is not Series.TreeSeries tree) return null;

        var hitRadius = tree.NodeSize + 4;
        int bestIndex = -1;
        double bestDist = double.MaxValue;

        int nodeIndex = 0;
        foreach (var root in tree.Nodes)
        {
            foreach (var node in FlattenTree(root))
            {
                var dx = pointerPosition.X - node.LayoutX;
                var dy = pointerPosition.Y - node.LayoutY;
                var dist = Math.Sqrt(dx * dx + dy * dy);

                if (dist < hitRadius && dist < bestDist)
                {
                    bestDist = dist;
                    bestIndex = nodeIndex;
                }
                nodeIndex++;
            }
        }

        if (bestIndex < 0) return null;

        return new ChartHitResult
        {
            Series = series,
            DataIndex = bestIndex,
            HitPosition = pointerPosition,
            Distance = bestDist
        };
    }

    // ────────────────────────────────────────────────
    //  Orthogonal layout rendering
    // ────────────────────────────────────────────────

    private static void RenderOrthogonal(
        DrawingContext context,
        Series.TreeSeries tree,
        Avalonia.Collections.AvaloniaList<ChartTreeNode> roots,
        Rect plotArea,
        double progress)
    {
        var nodeColor = tree.NodeColor ?? Brushes.SteelBlue;
        var edgeColor = tree.EdgeColor ?? Brushes.LightGray;
        var edgePen = new Pen(edgeColor, tree.EdgeThickness);
        var nodeRadius = tree.NodeSize / 2;

        // Compute tree dimensions
        int maxDepth = 0;
        int totalLeaves = 0;
        foreach (var root in roots)
        {
            ComputeTreeMetrics(root, 0, ref maxDepth, ref totalLeaves);
        }

        if (maxDepth == 0) maxDepth = 1;
        if (totalLeaves == 0) totalLeaves = 1;

        var levelSep = tree.LevelSeparation > 0
            ? tree.LevelSeparation
            : (tree.Layout is TreeLayout.LR or TreeLayout.RL)
                ? plotArea.Width / (maxDepth + 1)
                : plotArea.Height / (maxDepth + 1);

        // Assign layout positions
        int leafIndex = 0;
        var leafSpacing = (tree.Layout is TreeLayout.LR or TreeLayout.RL)
            ? plotArea.Height / (totalLeaves + 1)
            : plotArea.Width / (totalLeaves + 1);

        foreach (var root in roots)
        {
            AssignOrthogonalLayout(root, 0, tree.Layout, plotArea, levelSep, leafSpacing, ref leafIndex, progress);
        }

        // Draw edges
        foreach (var root in roots)
        {
            DrawOrthogonalEdges(context, root, tree, edgePen, progress);
        }

        // Draw nodes
        foreach (var root in roots)
        {
            DrawNodes(context, root, tree, nodeColor, nodeRadius, progress);
        }
    }

    private static void ComputeTreeMetrics(ChartTreeNode node, int depth, ref int maxDepth, ref int totalLeaves)
    {
        if (depth > maxDepth) maxDepth = depth;
        if (node.Children.Count == 0)
        {
            totalLeaves++;
        }
        else
        {
            foreach (var child in node.Children)
                ComputeTreeMetrics(child, depth + 1, ref maxDepth, ref totalLeaves);
        }
    }

    private static void AssignOrthogonalLayout(
        ChartTreeNode node, int depth, TreeLayout layout, Rect plotArea,
        double levelSep, double leafSpacing, ref int leafIndex, double progress = 1.0)
    {
        if (node.Children.Count == 0)
        {
            leafIndex++;
            switch (layout)
            {
                case TreeLayout.LR:
                    node.LayoutX = plotArea.Left + depth * levelSep;
                    node.LayoutY = plotArea.Top + leafIndex * leafSpacing;
                    break;
                case TreeLayout.RL:
                    node.LayoutX = plotArea.Right - depth * levelSep;
                    node.LayoutY = plotArea.Top + leafIndex * leafSpacing;
                    break;
                case TreeLayout.TB:
                    node.LayoutX = plotArea.Left + leafIndex * leafSpacing;
                    node.LayoutY = plotArea.Top + depth * levelSep;
                    break;
                case TreeLayout.BT:
                    node.LayoutX = plotArea.Left + leafIndex * leafSpacing;
                    node.LayoutY = plotArea.Bottom - depth * levelSep;
                    break;
            }
        }
        else
        {
            // Recurse into children first
            foreach (var child in node.Children)
                AssignOrthogonalLayout(child, depth + 1, layout, plotArea, levelSep, leafSpacing, ref leafIndex, progress);

            // Position parent at center of children
            var avgX = node.Children.Average(c => c.LayoutX);
            var avgY = node.Children.Average(c => c.LayoutY);

            switch (layout)
            {
                case TreeLayout.LR:
                    node.LayoutX = plotArea.Left + depth * levelSep;
                    node.LayoutY = avgY;
                    break;
                case TreeLayout.RL:
                    node.LayoutX = plotArea.Right - depth * levelSep;
                    node.LayoutY = avgY;
                    break;
                case TreeLayout.TB:
                    node.LayoutX = avgX;
                    node.LayoutY = plotArea.Top + depth * levelSep;
                    break;
                case TreeLayout.BT:
                    node.LayoutX = avgX;
                    node.LayoutY = plotArea.Bottom - depth * levelSep;
                    break;
            }
        }

        // Apply animation offset from root
        node.LayoutX = plotArea.Center.X + (node.LayoutX - plotArea.Center.X) * progress;
        node.LayoutY = plotArea.Center.Y + (node.LayoutY - plotArea.Center.Y) * progress;
    }

    private static void DrawOrthogonalEdges(
        DrawingContext context, ChartTreeNode node, Series.TreeSeries tree,
        Pen edgePen, double progress)
    {
        foreach (var child in node.Children)
        {
            if (tree.CurvedEdges)
            {
                // Draw Bezier curve from parent to child
                var geometry = new PathGeometry();
                var figure = new PathFigure { StartPoint = new Point(node.LayoutX, node.LayoutY), IsClosed = false };

                double midX, midY;
                switch (tree.Layout)
                {
                    case TreeLayout.LR:
                    case TreeLayout.RL:
                        midX = (node.LayoutX + child.LayoutX) / 2;
                        figure.Segments!.Add(new BezierSegment
                        {
                            Point1 = new Point(midX, node.LayoutY),
                            Point2 = new Point(midX, child.LayoutY),
                            Point3 = new Point(child.LayoutX, child.LayoutY)
                        });
                        break;
                    default:
                        midY = (node.LayoutY + child.LayoutY) / 2;
                        figure.Segments!.Add(new BezierSegment
                        {
                            Point1 = new Point(node.LayoutX, midY),
                            Point2 = new Point(child.LayoutX, midY),
                            Point3 = new Point(child.LayoutX, child.LayoutY)
                        });
                        break;
                }

                geometry.Figures!.Add(figure);
                context.DrawGeometry(null, edgePen, geometry);
            }
            else
            {
                // Draw orthogonal lines
                switch (tree.Layout)
                {
                    case TreeLayout.LR:
                    case TreeLayout.RL:
                        var midX = (node.LayoutX + child.LayoutX) / 2;
                        context.DrawLine(edgePen,
                            new Point(node.LayoutX, node.LayoutY),
                            new Point(midX, node.LayoutY));
                        context.DrawLine(edgePen,
                            new Point(midX, node.LayoutY),
                            new Point(midX, child.LayoutY));
                        context.DrawLine(edgePen,
                            new Point(midX, child.LayoutY),
                            new Point(child.LayoutX, child.LayoutY));
                        break;
                    case TreeLayout.TB:
                    case TreeLayout.BT:
                        var midY = (node.LayoutY + child.LayoutY) / 2;
                        context.DrawLine(edgePen,
                            new Point(node.LayoutX, node.LayoutY),
                            new Point(node.LayoutX, midY));
                        context.DrawLine(edgePen,
                            new Point(node.LayoutX, midY),
                            new Point(child.LayoutX, midY));
                        context.DrawLine(edgePen,
                            new Point(child.LayoutX, midY),
                            new Point(child.LayoutX, child.LayoutY));
                        break;
                }
            }

            DrawOrthogonalEdges(context, child, tree, edgePen, progress);
        }
    }

    // ────────────────────────────────────────────────
    //  Radial layout rendering
    // ────────────────────────────────────────────────

    private static void RenderRadial(
        DrawingContext context,
        Series.TreeSeries tree,
        Avalonia.Collections.AvaloniaList<ChartTreeNode> roots,
        Rect plotArea,
        double progress)
    {
        var nodeColor = tree.NodeColor ?? Brushes.SteelBlue;
        var edgeColor = tree.EdgeColor ?? Brushes.LightGray;
        var edgePen = new Pen(edgeColor, tree.EdgeThickness);
        var nodeRadius = tree.NodeSize / 2;
        var center = new Point(plotArea.Center.X, plotArea.Center.Y);
        var maxRadius = Math.Min(plotArea.Width, plotArea.Height) / 2 - 20;

        // Compute max depth
        int maxDepth = 0;
        int totalLeaves = 0;
        foreach (var root in roots)
            ComputeTreeMetrics(root, 0, ref maxDepth, ref totalLeaves);

        if (maxDepth == 0) maxDepth = 1;
        var radiusStep = maxRadius / (maxDepth + 1);

        // Assign radial positions
        int leafIndex = 0;
        var totalAngle = 360.0 / roots.Count;
        int rootIdx = 0;

        foreach (var root in roots)
        {
            var startAngle = rootIdx * totalAngle;
            AssignRadialLayout(root, 0, center, radiusStep, startAngle, totalAngle, ref leafIndex, totalLeaves, progress);
            rootIdx++;
        }

        // Draw edges
        foreach (var root in roots)
            DrawRadialEdges(context, root, edgePen, progress);

        // Draw nodes
        foreach (var root in roots)
            DrawNodes(context, root, tree, nodeColor, nodeRadius, progress);
    }

    private static void AssignRadialLayout(
        ChartTreeNode node, int depth, Point center, double radiusStep,
        double startAngle, double angleSpan, ref int leafIndex, int totalLeaves, double progress = 1.0)
    {
        var radius = depth * radiusStep;
        double midAngle;

        if (node.Children.Count == 0)
        {
            leafIndex++;
            midAngle = startAngle + angleSpan * leafIndex / (totalLeaves + 1);
        }
        else
        {
            int childLeafCount = 0;
            CountLeaves(node, ref childLeafCount);
            var childAngleSpan = angleSpan * childLeafCount / (totalLeaves + 1);

            int childLeafIdx = 0;
            foreach (var child in node.Children)
            {
                int childLeaves = 0;
                CountLeaves(child, ref childLeaves);
                var childSpan = angleSpan * childLeaves / (totalLeaves + 1);
                AssignRadialLayout(child, depth + 1, center, radiusStep,
                    startAngle + childLeafIdx * (angleSpan / (totalLeaves + 1)),
                    childSpan, ref leafIndex, totalLeaves, progress);
                childLeafIdx += childLeaves;
            }

            midAngle = node.Children.Average(c => Math.Atan2(c.LayoutY - center.Y, c.LayoutX - center.X) * 180 / Math.PI);
        }

        var rad = midAngle * Math.PI / 180;
        node.LayoutX = center.X + radius * Math.Cos(rad);
        node.LayoutY = center.Y + radius * Math.Sin(rad);
    }

    private static void CountLeaves(ChartTreeNode node, ref int count)
    {
        if (node.Children.Count == 0) { count++; return; }
        foreach (var child in node.Children)
            CountLeaves(child, ref count);
    }

    private static void DrawRadialEdges(DrawingContext context, ChartTreeNode node, Pen edgePen, double progress)
    {
        foreach (var child in node.Children)
        {
            context.DrawLine(edgePen,
                new Point(node.LayoutX, node.LayoutY),
                new Point(child.LayoutX, child.LayoutY));
            DrawRadialEdges(context, child, edgePen, progress);
        }
    }

    // ────────────────────────────────────────────────
    //  Common drawing helpers
    // ────────────────────────────────────────────────

    private static void DrawNodes(
        DrawingContext context, ChartTreeNode node, Series.TreeSeries tree,
        IBrush defaultColor, double nodeRadius, double progress)
    {
        var color = node.Color ?? defaultColor;
        var brush = new SolidColorBrush(((SolidColorBrush)color).Color);
        var pen = new Pen(Brushes.White, 1.5);

        context.DrawEllipse(brush, pen, new Point(node.LayoutX, node.LayoutY), nodeRadius, nodeRadius);

        // Label
        if (tree.ShowLabels && !string.IsNullOrEmpty(node.Name) && progress >= 1.0)
        {
            var formattedText = new FormattedText(node.Name,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface("Segoe UI", FontStyle.Normal, FontWeight.Normal),
                tree.LabelFontSize,
                Brushes.DimGray);

            double labelX, labelY;
            switch (tree.Layout)
            {
                case TreeLayout.LR:
                    labelX = node.LayoutX + nodeRadius + 4;
                    labelY = node.LayoutY - formattedText.Height / 2;
                    break;
                case TreeLayout.RL:
                    labelX = node.LayoutX - nodeRadius - formattedText.Width - 4;
                    labelY = node.LayoutY - formattedText.Height / 2;
                    break;
                case TreeLayout.TB:
                    labelX = node.LayoutX - formattedText.Width / 2;
                    labelY = node.LayoutY + nodeRadius + 4;
                    break;
                case TreeLayout.BT:
                    labelX = node.LayoutX - formattedText.Width / 2;
                    labelY = node.LayoutY - nodeRadius - formattedText.Height - 4;
                    break;
                default:
                    labelX = node.LayoutX + nodeRadius + 4;
                    labelY = node.LayoutY - formattedText.Height / 2;
                    break;
            }

            context.DrawText(formattedText, new Point(labelX, labelY));
        }

        // Recurse
        foreach (var child in node.Children)
            DrawNodes(context, child, tree, defaultColor, nodeRadius, progress);
    }

    private static IEnumerable<ChartTreeNode> FlattenTree(ChartTreeNode node)
    {
        yield return node;
        foreach (var child in node.Children)
        {
            foreach (var descendant in FlattenTree(child))
                yield return descendant;
        }
    }
}
