using Avalonia;

namespace AuraUI.Controls.Charts.Graph.Layouts;

/// <summary>
/// Concentric layout groups nodes into concentric circles by a property (degree, level, etc.).
/// Unlike radial layout, all nodes on the same ring share the same concentric property value.
/// </summary>
public class ConcentricLayout
{
    /// <summary>Minimum spacing between nodes on the same ring.</summary>
    public double MinNodeSpacing { get; set; } = 30;

    /// <summary>Whether to prevent node overlap by adjusting ring radius.</summary>
    public bool PreventOverlap { get; set; } = true;

    /// <summary>Strategy for assigning concentric levels.</summary>
    public ConcentricBy ConcentricBy { get; set; } = ConcentricBy.Degree;

    /// <summary>
    /// Explicit level assignments per node ID. Used when ConcentricBy is Property.
    /// Nodes not in this dictionary are placed on the outermost ring.
    /// </summary>
    public Dictionary<string, int>? LevelMap { get; set; }

    /// <summary>Minimum radius for the innermost ring.</summary>
    public double MinRadius { get; set; } = 40;

    /// <summary>Radius increment between rings.</summary>
    public double RadiusStep { get; set; } = 80;

    /// <summary>
    /// Applies the concentric layout.
    /// </summary>
    public void Apply(GraphData data, Rect bounds)
    {
        if (data.Nodes.Count == 0) return;

        var center = new Point(bounds.X + bounds.Width / 2, bounds.Y + bounds.Height / 2);

        // Assign levels
        var nodeLevels = new Dictionary<string, int>();
        switch (ConcentricBy)
        {
            case ConcentricBy.Degree:
                AssignByDegree(data, nodeLevels);
                break;
            case ConcentricBy.Property:
                AssignByProperty(data, nodeLevels);
                break;
            case ConcentricBy.Depth:
                AssignByDepth(data, nodeLevels);
                break;
        }

        // Group by level
        var levelGroups = new Dictionary<int, List<GraphNode>>();
        foreach (var node in data.Nodes)
        {
            var level = nodeLevels.TryGetValue(node.Id, out var l) ? l : 0;
            if (!levelGroups.ContainsKey(level))
                levelGroups[level] = new List<GraphNode>();
            levelGroups[level].Add(node);
        }

        // Position nodes on rings
        foreach (var (level, nodesInLevel) in levelGroups.OrderBy(kv => kv.Key))
        {
            var radius = MinRadius + level * RadiusStep;

            if (PreventOverlap && nodesInLevel.Count > 0)
            {
                // Ensure minimum angular spacing
                var circumference = 2 * Math.PI * radius;
                var requiredCircumference = nodesInLevel.Count * MinNodeSpacing;
                if (requiredCircumference > circumference)
                {
                    radius = requiredCircumference / (2 * Math.PI);
                }
            }

            var count = nodesInLevel.Count;
            var angleStep = 2 * Math.PI / Math.Max(count, 1);

            for (int i = 0; i < count; i++)
            {
                var angle = angleStep * i - Math.PI / 2;
                nodesInLevel[i].Position = new Point(
                    center.X + radius * Math.Cos(angle),
                    center.Y + radius * Math.Sin(angle));
            }
        }
    }

    private void AssignByDegree(GraphData data, Dictionary<string, int> levels)
    {
        // Group by degree, assign rings by degree bucket
        var degrees = data.Nodes.Select(n => data.GetDegree(n.Id)).Distinct().OrderBy(d => d).ToList();
        var degreeToLevel = new Dictionary<int, int>();
        for (int i = 0; i < degrees.Count; i++)
            degreeToLevel[degrees[i]] = i;

        foreach (var node in data.Nodes)
        {
            var degree = data.GetDegree(node.Id);
            levels[node.Id] = degreeToLevel[degree];
        }
    }

    private void AssignByProperty(GraphData data, Dictionary<string, int> levels)
    {
        if (LevelMap != null)
        {
            foreach (var (id, level) in LevelMap)
                levels[id] = level;
        }
        // Unassigned nodes go to level 0
        foreach (var node in data.Nodes)
        {
            if (!levels.ContainsKey(node.Id))
                levels[node.Id] = 0;
        }
    }

    private void AssignByDepth(GraphData data, Dictionary<string, int> levels)
    {
        // BFS from the node with the highest degree
        var root = data.Nodes.OrderByDescending(n => data.GetDegree(n.Id)).FirstOrDefault();
        if (root == null) return;

        var visited = new HashSet<string>();
        var queue = new Queue<(string id, int depth)>();
        queue.Enqueue((root.Id, 0));
        visited.Add(root.Id);

        while (queue.Count > 0)
        {
            var (id, depth) = queue.Dequeue();
            levels[id] = depth;

            foreach (var neighbor in data.GetNeighbors(id))
            {
                if (visited.Add(neighbor))
                    queue.Enqueue((neighbor, depth + 1));
            }
        }

        // Unvisited nodes
        foreach (var node in data.Nodes)
        {
            if (!levels.ContainsKey(node.Id))
                levels[node.Id] = 0;
        }
    }
}
