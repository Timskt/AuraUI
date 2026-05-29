using Avalonia;

namespace AuraUI.Controls.Charts.Graph.Behaviors;

/// <summary>
/// Behavior for collapsing and expanding subtrees in a graph.
/// Collapsed subtrees are represented by a single node; expanding restores children.
/// </summary>
public class CollapseExpand
{
    /// <summary>What triggers collapse/expand: click or double-click.</summary>
    public BehaviorTrigger Trigger { get; set; } = BehaviorTrigger.Click;

    /// <summary>Whether to animate the collapse/expand transition.</summary>
    public bool Animate { get; set; } = true;

    /// <summary>Animation duration in milliseconds.</summary>
    public double AnimationDuration { get; set; } = 300;

    /// <summary>
    /// Tracks which edges are collapsed (source -> list of hidden target IDs).
    /// </summary>
    private readonly Dictionary<string, HashSet<string>> _collapsedEdges = new();

    /// <summary>Set of node IDs that are currently hidden due to being in a collapsed subtree.</summary>
    private readonly HashSet<string> _hiddenNodeIds = new();

    /// <summary>Set of edge indices that are hidden due to collapse.</summary>
    private readonly HashSet<int> _hiddenEdgeIndices = new();

    /// <summary>Raised when collapse/expand state changes.</summary>
    public event EventHandler? StateChanged;

    /// <summary>
    /// Handles a click on a node to toggle collapse/expand.
    /// </summary>
    /// <returns>True if the state changed.</returns>
    public bool HandleClick(
        GraphData data,
        string nodeId,
        Point pointerPos,
        Func<GraphNode, Point> getWorldPos,
        bool isDoubleClick)
    {
        // Only respond to the configured trigger
        if (Trigger == BehaviorTrigger.Click && isDoubleClick) return false;
        if (Trigger == BehaviorTrigger.DoubleClick && !isDoubleClick) return false;

        var node = data.GetNode(nodeId);
        if (node == null) return false;

        // Check if the node has children (outgoing edges)
        var outgoingEdges = data.GetConnectedEdges(nodeId)
            .Where(e => e.Source == nodeId)
            .ToList();

        if (outgoingEdges.Count == 0) return false;

        if (node.IsCollapsed)
            ExpandNode(data, nodeId);
        else
            CollapseNode(data, nodeId);

        StateChanged?.Invoke(this, EventArgs.Empty);
        return true;
    }

    /// <summary>
    /// Collapses the subtree rooted at the given node.
    /// Hides all descendant nodes and edges.
    /// </summary>
    public void CollapseNode(GraphData data, string nodeId)
    {
        var node = data.GetNode(nodeId);
        if (node == null) return;

        node.IsCollapsed = true;
        _collapsedEdges[nodeId] = new HashSet<string>();

        // BFS to find all descendants
        var descendants = new HashSet<string>();
        var queue = new Queue<string>();

        foreach (var edge in data.GetConnectedEdges(nodeId).Where(e => e.Source == nodeId))
        {
            _collapsedEdges[nodeId].Add(edge.Target);
            queue.Enqueue(edge.Target);
        }

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            if (descendants.Contains(current)) continue;
            descendants.Add(current);

            foreach (var edge in data.GetConnectedEdges(current).Where(e => e.Source == current))
            {
                if (!descendants.Contains(edge.Target))
                    queue.Enqueue(edge.Target);
            }
        }

        // Hide descendant nodes
        foreach (var descendantId in descendants)
        {
            _hiddenNodeIds.Add(descendantId);
            var descendant = data.GetNode(descendantId);
            if (descendant != null)
                descendant.IsHidden = true;
        }

        // Hide edges within the collapsed subtree
        UpdateHiddenEdges(data);
    }

    /// <summary>
    /// Expands the subtree rooted at the given node.
    /// Restores hidden descendant nodes and edges.
    /// </summary>
    public void ExpandNode(GraphData data, string nodeId)
    {
        var node = data.GetNode(nodeId);
        if (node == null) return;

        node.IsCollapsed = false;

        if (!_collapsedEdges.TryGetValue(nodeId, out var directChildren))
            return;

        // BFS to find all descendants that were hidden
        var toRestore = new HashSet<string>();
        var queue = new Queue<string>(directChildren);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            if (!_hiddenNodeIds.Contains(current)) continue;
            toRestore.Add(current);

            // If this descendant was also collapsed, don't expand its children
            if (_collapsedEdges.ContainsKey(current)) continue;

            foreach (var edge in data.GetConnectedEdges(current).Where(e => e.Source == current))
            {
                if (_hiddenNodeIds.Contains(edge.Target))
                    queue.Enqueue(edge.Target);
            }
        }

        // Restore nodes
        foreach (var id in toRestore)
        {
            _hiddenNodeIds.Remove(id);
            var n = data.GetNode(id);
            if (n != null)
                n.IsHidden = false;
        }

        _collapsedEdges.Remove(nodeId);
        UpdateHiddenEdges(data);
    }

    /// <summary>
    /// Expands all collapsed subtrees.
    /// </summary>
    public void ExpandAll(GraphData data)
    {
        foreach (var nodeId in _collapsedEdges.Keys.ToList())
        {
            var node = data.GetNode(nodeId);
            if (node != null) node.IsCollapsed = false;
        }
        _collapsedEdges.Clear();
        _hiddenNodeIds.Clear();

        foreach (var node in data.Nodes) node.IsHidden = false;
        foreach (var edge in data.Edges) edge.IsHidden = false;
        _hiddenEdgeIndices.Clear();

        StateChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>Gets the set of currently hidden node IDs.</summary>
    public IReadOnlySet<string> HiddenNodeIds => _hiddenNodeIds;

    /// <summary>Gets the set of currently hidden edge indices.</summary>
    public IReadOnlySet<int> HiddenEdgeIndices => _hiddenEdgeIndices;

    private void UpdateHiddenEdges(GraphData data)
    {
        _hiddenEdgeIndices.Clear();
        for (int i = 0; i < data.Edges.Count; i++)
        {
            var edge = data.Edges[i];
            var hidden = _hiddenNodeIds.Contains(edge.Source) || _hiddenNodeIds.Contains(edge.Target);
            edge.IsHidden = hidden;
            if (hidden) _hiddenEdgeIndices.Add(i);
        }
    }
}
