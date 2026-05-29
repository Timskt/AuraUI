using Avalonia;

namespace AuraUI.Controls.Charts.Graph.Layouts;

/// <summary>
/// Radial layout arranges nodes in concentric circles around a focus node.
/// Nodes at each BFS level are placed on the same ring.
/// </summary>
public class RadialLayout
{
    /// <summary>Center point of the radial layout.</summary>
    public Point Center { get; set; }

    /// <summary>Optional focus node ID. If set, BFS radiates from this node.</summary>
    public string? FocusNode { get; set; }

    /// <summary>
    /// Radius increment per concentric ring level.
    /// </summary>
    public double UnitRadius { get; set; } = 80;

    /// <summary>Preferred link distance (used for initial force refinement).</summary>
    public double LinkDistance { get; set; } = 60;

    /// <summary>Minimum angular separation between nodes on the same ring (radians).</summary>
    public double MinAngleSeparation { get; set; } = 0.2;

    /// <summary>
    /// Applies the radial layout using BFS distances from the focus node.
    /// </summary>
    public void Apply(GraphData data, Rect bounds)
    {
        if (data.Nodes.Count == 0) return;

        var center = Center.X == 0 && Center.Y == 0
            ? new Point(bounds.X + bounds.Width / 2, bounds.Y + bounds.Height / 2)
            : Center;

        // Determine focus node
        string? startId = FocusNode;
        if (string.IsNullOrEmpty(startId))
        {
            // Pick the node with the highest degree as focus
            var bestNode = data.Nodes.OrderByDescending(n => data.GetDegree(n.Id)).FirstOrDefault();
            startId = bestNode?.Id;
        }

        if (string.IsNullOrEmpty(startId)) return;

        // BFS to assign levels
        var levels = new Dictionary<string, int>();
        var queue = new Queue<string>();
        levels[startId] = 0;
        queue.Enqueue(startId);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            var currentLevel = levels[current];

            foreach (var neighbor in data.GetNeighbors(current))
            {
                if (!levels.ContainsKey(neighbor))
                {
                    levels[neighbor] = currentLevel + 1;
                    queue.Enqueue(neighbor);
                }
            }
        }

        // Assign default level for unvisited nodes
        var maxLevel = levels.Count > 0 ? levels.Values.Max() : 0;
        foreach (var node in data.Nodes)
        {
            if (!levels.ContainsKey(node.Id))
            {
                maxLevel++;
                levels[node.Id] = maxLevel;
            }
        }

        // Group nodes by level
        var levelGroups = new Dictionary<int, List<GraphNode>>();
        foreach (var node in data.Nodes)
        {
            var level = levels[node.Id];
            if (!levelGroups.ContainsKey(level))
                levelGroups[level] = new List<GraphNode>();
            levelGroups[level].Add(node);
        }

        // Position nodes on concentric rings
        foreach (var (level, nodesInLevel) in levelGroups)
        {
            var radius = level * UnitRadius;
            if (level == 0)
            {
                // Focus node at center
                nodesInLevel[0].Position = center;
                continue;
            }

            var count = nodesInLevel.Count;
            var angleStep = Math.Max(2 * Math.PI / count, MinAngleSeparation);

            for (int i = 0; i < count; i++)
            {
                var angle = angleStep * i - Math.PI / 2; // Start from top
                nodesInLevel[i].Position = new Point(
                    center.X + radius * Math.Cos(angle),
                    center.Y + radius * Math.Sin(angle));
            }
        }
    }
}
