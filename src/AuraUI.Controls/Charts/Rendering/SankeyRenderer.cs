using Avalonia;
using Avalonia.Media;
using AuraUI.Controls.Charts.Series;

namespace AuraUI.Controls.Charts.Rendering;

/// <summary>
/// Renders SankeySeries as a flow diagram with nodes as rectangles
/// and links as curved bands between them.
/// </summary>
public class SankeyRenderer : IChartRenderer
{
    public string Key => "Sankey";

    public void Render(
        DrawingContext context,
        ChartSeries series,
        Rect plotArea,
        ChartAxis? xAxis,
        ChartAxis? yAxis,
        double progress,
        IReadOnlyList<ChartSeries> allSeries)
    {
        if (series is not SankeySeries sankey || !series.IsVisible) return;
        if (sankey.Nodes.Count == 0 || sankey.Links.Count == 0) return;

        // Compute node positions using a simple layered layout
        var layout = ComputeSankeyLayout(sankey, plotArea);

        // Draw links first (behind nodes)
        foreach (var link in sankey.Links)
        {
            if (link.Source < 0 || link.Source >= layout.Count) continue;
            if (link.Target < 0 || link.Target >= layout.Count) continue;

            var sourceNode = layout[link.Source];
            var targetNode = layout[link.Target];

            var linkColor = link.Color ?? sourceNode.Color ?? LineRenderer.ResolveColor(sankey);
            var linkBrush = new SolidColorBrush(((SolidColorBrush)linkColor).Color)
            {
                Opacity = sankey.LinkOpacity
            };

            // Draw curved band
            var sx = sourceNode.Rect.Right;
            var sy = sourceNode.Rect.Center.Y;
            var tx = targetNode.Rect.Left;
            var ty = targetNode.Rect.Center.Y;
            var midX = (sx + tx) / 2;

            var thickness = Math.Max(2, link.Value / sourceNode.TotalOutput * sourceNode.Rect.Height * progress);
            var halfThick = thickness / 2;

            var geometry = new PathGeometry();
            var figure = new PathFigure { StartPoint = new Point(sx, sy - halfThick), IsClosed = true };
            figure.Segments!.Add(new BezierSegment
            {
                Point1 = new Point(midX, sy - halfThick),
                Point2 = new Point(midX, ty - halfThick),
                Point3 = new Point(tx, ty - halfThick)
            });
            figure.Segments!.Add(new LineSegment { Point = new Point(tx, ty + halfThick) });
            figure.Segments!.Add(new BezierSegment
            {
                Point1 = new Point(midX, ty + halfThick),
                Point2 = new Point(midX, sy + halfThick),
                Point3 = new Point(sx, sy + halfThick)
            });
            geometry.Figures!.Add(figure);

            context.DrawGeometry(linkBrush, null, geometry);
        }

        // Draw nodes
        for (int i = 0; i < layout.Count; i++)
        {
            var node = layout[i];
            var color = node.Color ?? GetSankeyColor(i);
            var brush = new SolidColorBrush(((SolidColorBrush)color).Color);

            var scaledRect = new Rect(node.Rect.X, node.Rect.Y,
                node.Rect.Width * Math.Min(progress, 1.0),
                node.Rect.Height);

            context.DrawRectangle(brush, null, scaledRect);

            if (sankey.ShowLabels && progress >= 1.0 && !string.IsNullOrEmpty(node.Name))
            {
                var fontSize = Math.Min(node.Rect.Height * 0.5, 12);
                if (fontSize < 7) continue;

                var formattedText = new FormattedText(node.Name!,
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Segoe UI", FontStyle.Normal, FontWeight.Normal),
                    fontSize,
                    Brushes.Black);

                var labelX = node.Rect.Right + 4;
                var labelY = node.Rect.Center.Y - formattedText.Height / 2;
                if (labelX + formattedText.Width > plotArea.Right)
                {
                    labelX = node.Rect.Left - formattedText.Width - 4;
                }

                context.DrawText(formattedText, new Point(labelX, labelY));
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
        if (series is not SankeySeries sankey) return null;

        var layout = ComputeSankeyLayout(sankey, plotArea);
        for (int i = 0; i < layout.Count; i++)
        {
            if (layout[i].Rect.Contains(pointerPosition))
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

    private static List<SankeyNodeLayout> ComputeSankeyLayout(SankeySeries sankey, Rect plotArea)
    {
        var result = new List<SankeyNodeLayout>();
        var nodes = sankey.Nodes;
        var links = sankey.Links;

        // Compute in/out degree for each node
        var totalIn = new double[nodes.Count];
        var totalOut = new double[nodes.Count];
        foreach (var link in links)
        {
            if (link.Source >= 0 && link.Source < nodes.Count)
                totalOut[link.Source] += link.Value;
            if (link.Target >= 0 && link.Target < nodes.Count)
                totalIn[link.Target] += link.Value;
        }

        // Assign columns (simple: sources=0, targets=max, intermediates by topological order)
        var columns = new int[nodes.Count];
        var isSource = new bool[nodes.Count];
        var isTarget = new bool[nodes.Count];
        foreach (var link in links)
        {
            if (link.Source >= 0 && link.Source < nodes.Count) isSource[link.Source] = true;
            if (link.Target >= 0 && link.Target < nodes.Count) isTarget[link.Target] = true;
        }

        for (int i = 0; i < nodes.Count; i++)
        {
            if (isSource[i] && !isTarget[i]) columns[i] = 0;
            else if (isTarget[i] && !isSource[i]) columns[i] = 2;
            else columns[i] = 1;
        }

        int maxCol = columns.Length > 0 ? columns.Max() : 0;
        if (maxCol == 0) maxCol = 1;
        var colWidth = (plotArea.Width - sankey.ColumnGap * (maxCol)) / (maxCol + 1);

        // Group by column and assign positions
        var colGroups = new Dictionary<int, List<int>>();
        for (int i = 0; i < nodes.Count; i++)
        {
            if (!colGroups.ContainsKey(columns[i]))
                colGroups[columns[i]] = new List<int>();
            colGroups[columns[i]].Add(i);
        }

        foreach (var (col, indices) in colGroups)
        {
            var colX = plotArea.Left + col * (colWidth + sankey.ColumnGap);
            var totalNodeValue = indices.Sum(idx => Math.Max(totalIn[idx], totalOut[idx], 1));
            var availableHeight = plotArea.Height - sankey.NodeGap * (indices.Count - 1);
            var y = plotArea.Top;

            foreach (var idx in indices)
            {
                var nodeValue = Math.Max(totalIn[idx], totalOut[idx], 1);
                var nodeHeight = Math.Max(10, availableHeight * (nodeValue / totalNodeValue));

                result.Add(new SankeyNodeLayout
                {
                    Node = nodes[idx],
                    Rect = new Rect(colX, y, sankey.NodeWidth, nodeHeight),
                    Color = nodes[idx].Color,
                    Name = nodes[idx].Name,
                    TotalOutput = totalOut[idx],
                    TotalInput = totalIn[idx]
                });

                y += nodeHeight + sankey.NodeGap;
            }
        }

        return result;
    }

    private static IBrush GetSankeyColor(int index)
    {
        return new SolidColorBrush(LineRenderer.DefaultPalette[index % LineRenderer.DefaultPalette.Length]);
    }

    private class SankeyNodeLayout
    {
        public ChartSankeyNode Node { get; set; } = null!;
        public Rect Rect { get; set; }
        public IBrush? Color { get; set; }
        public string? Name { get; set; }
        public double TotalOutput { get; set; }
        public double TotalInput { get; set; }
    }
}
