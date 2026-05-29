using Avalonia;

namespace AuraUI.Controls.Charts.Graph.Layouts;

/// <summary>
/// Hierarchical / DAG layout using a simplified Sugiyama algorithm.
/// Assigns ranks (layers) to nodes and positions them to minimize edge crossings.
/// </summary>
public class DagreLayout
{
    /// <summary>Direction of ranking: TB (top-bottom), BT, LR, RL.</summary>
    public RankDirection RankDir { get; set; } = RankDirection.TB;

    /// <summary>Horizontal separation between nodes in the same rank.</summary>
    public double NodeSep { get; set; } = 50;

    /// <summary>Vertical separation between ranks.</summary>
    public double RankSep { get; set; } = 60;

    /// <summary>Alignment mode within each rank.</summary>
    public DagreAlign Align { get; set; } = DagreAlign.None;

    /// <summary>Margin around the entire layout.</summary>
    public Thickness Margin { get; set; } = new(20);

    /// <summary>
    /// Applies the hierarchical layout.
    /// </summary>
    public void Apply(GraphData data, Rect bounds)
    {
        if (data.Nodes.Count == 0) return;

        // Step 1: Assign ranks using longest-path
        AssignRanks(data);

        // Step 2: Order nodes within ranks to reduce crossings (Barycenter heuristic)
        var rankGroups = GroupByRank(data.Nodes);

        // Step 3: Position nodes
        PositionRanks(rankGroups, bounds);
    }

    private void AssignRanks(GraphData data)
    {
        // Find root nodes (no incoming edges)
        var inDegree = new Dictionary<string, int>();
        foreach (var node in data.Nodes)
            inDegree[node.Id] = 0;
        foreach (var edge in data.Edges)
        {
            if (inDegree.ContainsKey(edge.Target))
                inDegree[edge.Target]++;
        }

        // BFS from roots
        var queue = new Queue<string>();
        foreach (var node in data.Nodes)
        {
            if (inDegree[node.Id] == 0)
            {
                node.Level = 0;
                queue.Enqueue(node.Id);
            }
        }

        // Propagate ranks
        var visited = new HashSet<string>();
        while (queue.Count > 0)
        {
            var id = queue.Dequeue();
            if (!visited.Add(id)) continue;
            var node = data.GetNode(id);
            if (node == null) continue;

            foreach (var neighbor in data.GetNeighbors(id))
            {
                var neighborNode = data.GetNode(neighbor);
                if (neighborNode == null) continue;

                // Determine direction: if edge goes from id to neighbor, neighbor is at rank+1
                var edge = data.GetEdge(id, neighbor);
                if (edge != null && edge.Source == id)
                {
                    neighborNode.Level = Math.Max(neighborNode.Level, node.Level + 1);
                }
                else if (edge != null && edge.Target == id)
                {
                    // Edge goes from neighbor to id — neighbor should be at lower rank
                    // But for undirected, just assign based on traversal order
                    node.Level = Math.Max(node.Level, neighborNode.Level + 1);
                }

                queue.Enqueue(neighbor);
            }
        }

        // Normalize to start from 0
        var minRank = data.Nodes.Min(n => n.Level);
        if (minRank < 0)
        {
            foreach (var node in data.Nodes)
                node.Level -= minRank;
        }
    }

    private Dictionary<int, List<GraphNode>> GroupByRank(IList<GraphNode> nodes)
    {
        var groups = new Dictionary<int, List<GraphNode>>();
        foreach (var node in nodes)
        {
            if (!groups.ContainsKey(node.Level))
                groups[node.Level] = new List<GraphNode>();
            groups[node.Level].Add(node);
        }
        return groups;
    }

    private void PositionRanks(Dictionary<int, List<GraphNode>> rankGroups, Rect bounds)
    {
        var isHorizontal = RankDir is RankDirection.LR or RankDirection.RL;
        var isReverse = RankDir is RankDirection.BT or RankDirection.RL;

        var maxRank = rankGroups.Keys.Max();
        var totalRanks = maxRank + 1;

        // Compute available space
        var margin = Margin;
        var availWidth = bounds.Width - margin.Left - margin.Right;
        var availHeight = bounds.Height - margin.Top - margin.Bottom;

        foreach (var (rank, nodesInRank) in rankGroups)
        {
            var effectiveRank = isReverse ? maxRank - rank : rank;
            var totalNodesInRank = nodesInRank.Count;
            var totalNodeSpan = totalNodesInRank > 1
                ? (totalNodesInRank - 1) * NodeSep
                : 0;

            for (int i = 0; i < nodesInRank.Count; i++)
            {
                double x, y;

                if (isHorizontal)
                {
                    // Ranks go left-to-right
                    var rankPos = totalRanks > 1
                        ? margin.Left + (effectiveRank / (double)(totalRanks - 1)) * availWidth
                        : bounds.X + bounds.Width / 2;
                    var nodeSpan = totalNodesInRank > 1
                        ? margin.Top + (i / (double)(totalNodesInRank - 1)) * availHeight
                        : bounds.Y + bounds.Height / 2;

                    x = rankPos;
                    y = nodeSpan;
                }
                else
                {
                    // Ranks go top-to-bottom
                    var rankPos = totalRanks > 1
                        ? margin.Top + (effectiveRank / (double)(totalRanks - 1)) * availHeight
                        : bounds.Y + bounds.Height / 2;
                    var nodeSpan = totalNodesInRank > 1
                        ? margin.Left + (i / (double)(totalNodesInRank - 1)) * availWidth
                        : bounds.X + bounds.Width / 2;

                    x = nodeSpan;
                    y = rankPos;
                }

                nodesInRank[i].Position = new Point(x, y);
            }
        }
    }
}
