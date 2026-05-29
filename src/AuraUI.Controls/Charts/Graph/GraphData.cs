using Avalonia.Collections;

namespace AuraUI.Controls.Charts.Graph;

/// <summary>
/// Data model for a graph visualization. Contains nodes, edges, and optional combo
/// (node group) collections. Provides CRUD operations and neighbor lookup.
/// </summary>
public class GraphData
{
    private readonly AvaloniaList<GraphNode> _nodes = new();
    private readonly AvaloniaList<GraphEdge> _edges = new();
    private readonly AvaloniaList<GraphCombo> _combos = new();

    // Fast lookup dictionaries
    private readonly Dictionary<string, GraphNode> _nodeIndex = new();
    private readonly Dictionary<string, List<GraphEdge>> _adjacency = new();

    /// <summary>All nodes in the graph.</summary>
    public AvaloniaList<GraphNode> Nodes => _nodes;

    /// <summary>All edges in the graph.</summary>
    public AvaloniaList<GraphEdge> Edges => _edges;

    /// <summary>All combos (node groups) in the graph.</summary>
    public AvaloniaList<GraphCombo> Combos => _combos;

    /// <summary>Raised when the graph data changes (node/edge added/removed).</summary>
    public event EventHandler? GraphChanged;

    public GraphData()
    {
        _nodes.CollectionChanged += (_, _) => OnChanged();
        _edges.CollectionChanged += (_, _) => OnChanged();
        _combos.CollectionChanged += (_, _) => OnChanged();
    }

    // ── Node operations ──

    /// <summary>Adds a node and updates internal indexes.</summary>
    public void AddNode(GraphNode node)
    {
        if (node == null) throw new ArgumentNullException(nameof(node));
        if (string.IsNullOrEmpty(node.Id)) throw new ArgumentException("Node must have an Id.", nameof(node));
        if (_nodeIndex.ContainsKey(node.Id))
            throw new InvalidOperationException($"Node with Id '{node.Id}' already exists.");

        _nodes.Add(node);
        _nodeIndex[node.Id] = node;
        _adjacency[node.Id] = new List<GraphEdge>();
        OnChanged();
    }

    /// <summary>Removes a node and all its connected edges.</summary>
    public bool RemoveNode(string nodeId)
    {
        if (!_nodeIndex.TryGetValue(nodeId, out var node))
            return false;

        // Remove connected edges
        if (_adjacency.TryGetValue(nodeId, out var edges))
        {
            foreach (var edge in edges.ToList())
                RemoveEdge(edge);
        }

        _nodes.Remove(node);
        _nodeIndex.Remove(nodeId);
        _adjacency.Remove(nodeId);
        OnChanged();
        return true;
    }

    /// <summary>Gets a node by its ID, or null if not found.</summary>
    public GraphNode? GetNode(string nodeId)
    {
        _nodeIndex.TryGetValue(nodeId, out var node);
        return node;
    }

    // ── Edge operations ──

    /// <summary>Adds an edge and updates adjacency.</summary>
    public void AddEdge(GraphEdge edge)
    {
        if (edge == null) throw new ArgumentNullException(nameof(edge));
        if (!_nodeIndex.ContainsKey(edge.Source))
            throw new ArgumentException($"Source node '{edge.Source}' not found.", nameof(edge));
        if (!_nodeIndex.ContainsKey(edge.Target))
            throw new ArgumentException($"Target node '{edge.Target}' not found.", nameof(edge));

        _edges.Add(edge);
        _adjacency[edge.Source].Add(edge);
        _adjacency[edge.Target].Add(edge);
        OnChanged();
    }

    /// <summary>Removes a specific edge.</summary>
    public bool RemoveEdge(GraphEdge edge)
    {
        if (!_edges.Contains(edge)) return false;

        _edges.Remove(edge);
        if (_adjacency.TryGetValue(edge.Source, out var srcEdges))
            srcEdges.Remove(edge);
        if (_adjacency.TryGetValue(edge.Target, out var tgtEdges))
            tgtEdges.Remove(edge);
        OnChanged();
        return true;
    }

    /// <summary>Removes all edges between two nodes.</summary>
    public int RemoveEdgesBetween(string sourceId, string targetId)
    {
        var toRemove = _edges.Where(e =>
            (e.Source == sourceId && e.Target == targetId) ||
            (e.Source == targetId && e.Target == sourceId)).ToList();

        foreach (var e in toRemove)
            RemoveEdge(e);
        return toRemove.Count;
    }

    // ── Query operations ──

    /// <summary>Gets all neighbor node IDs of the given node.</summary>
    public IEnumerable<string> GetNeighbors(string nodeId)
    {
        if (!_adjacency.TryGetValue(nodeId, out var edges))
            yield break;

        foreach (var edge in edges)
        {
            var other = edge.Source == nodeId ? edge.Target : edge.Source;
            yield return other;
        }
    }

    /// <summary>Gets all neighbor nodes of the given node.</summary>
    public IEnumerable<GraphNode> GetNeighborNodes(string nodeId)
    {
        return GetNeighbors(nodeId)
            .Where(id => _nodeIndex.ContainsKey(id))
            .Select(id => _nodeIndex[id]);
    }

    /// <summary>Gets all edges connected to the given node.</summary>
    public IEnumerable<GraphEdge> GetConnectedEdges(string nodeId)
    {
        if (_adjacency.TryGetValue(nodeId, out var edges))
            return edges;
        return Enumerable.Empty<GraphEdge>();
    }

    /// <summary>Gets the edge between two nodes, if any.</summary>
    public GraphEdge? GetEdge(string sourceId, string targetId)
    {
        return _edges.FirstOrDefault(e =>
            (e.Source == sourceId && e.Target == targetId) ||
            (e.Source == targetId && e.Target == sourceId));
    }

    /// <summary>Gets the degree (number of connections) of a node.</summary>
    public int GetDegree(string nodeId)
    {
        return _adjacency.TryGetValue(nodeId, out var edges) ? edges.Count : 0;
    }

    /// <summary>Gets all nodes with no connections.</summary>
    public IEnumerable<GraphNode> GetIsolatedNodes()
    {
        return _nodeIndex.Values.Where(n => GetDegree(n.Id) == 0);
    }

    /// <summary>Finds all connected components in the graph.</summary>
    public List<List<GraphNode>> GetConnectedComponents()
    {
        var visited = new HashSet<string>();
        var components = new List<List<GraphNode>>();

        foreach (var node in _nodes)
        {
            if (visited.Contains(node.Id)) continue;
            var component = new List<GraphNode>();
            var stack = new Stack<string>();
            stack.Push(node.Id);

            while (stack.Count > 0)
            {
                var current = stack.Pop();
                if (!visited.Add(current)) continue;
                var currentNode = _nodeIndex[current];
                component.Add(currentNode);

                foreach (var neighbor in GetNeighbors(current))
                {
                    if (!visited.Contains(neighbor))
                        stack.Push(neighbor);
                }
            }

            components.Add(component);
        }

        return components;
    }

    /// <summary>Clears all nodes, edges, and combos.</summary>
    public void Clear()
    {
        _nodes.Clear();
        _edges.Clear();
        _combos.Clear();
        _nodeIndex.Clear();
        _adjacency.Clear();
        OnChanged();
    }

    /// <summary>Rebuilds internal indexes. Call after bulk modifications.</summary>
    public void RebuildIndex()
    {
        _nodeIndex.Clear();
        _adjacency.Clear();

        foreach (var node in _nodes)
        {
            _nodeIndex[node.Id] = node;
            _adjacency[node.Id] = new List<GraphEdge>();
        }

        foreach (var edge in _edges)
        {
            if (_nodeIndex.ContainsKey(edge.Source) && _nodeIndex.ContainsKey(edge.Target))
            {
                _adjacency[edge.Source].Add(edge);
                _adjacency[edge.Target].Add(edge);
            }
        }
    }

    private void OnChanged()
    {
        GraphChanged?.Invoke(this, EventArgs.Empty);
    }
}

/// <summary>
/// A combo groups multiple nodes together visually (e.g., a bounding box around a cluster).
/// </summary>
public class GraphCombo
{
    /// <summary>Unique identifier for this combo.</summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>Display label for the combo.</summary>
    public string? Label { get; set; }

    /// <summary>IDs of the nodes contained in this combo.</summary>
    public List<string> NodeIds { get; set; } = new();

    /// <summary>Fill color for the combo background.</summary>
    public Avalonia.Media.IBrush? Color { get; set; }

    /// <summary>Padding around the contained nodes.</summary>
    public double Padding { get; set; } = 20;

    /// <summary>Corner radius of the combo bounding box.</summary>
    public double CornerRadius { get; set; } = 8;
}
