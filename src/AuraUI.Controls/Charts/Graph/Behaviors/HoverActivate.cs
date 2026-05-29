using Avalonia;

namespace AuraUI.Controls.Charts.Graph.Behaviors;

/// <summary>
/// Behavior that highlights connected edges and neighbor nodes when hovering over a node.
/// </summary>
public class HoverActivate
{
    /// <summary>State to apply to the hovered element and its neighbors.</summary>
    public GraphElementState ActiveState { get; set; } = GraphElementState.Active;

    /// <summary>State to apply to non-neighbor elements while hovering.</summary>
    public GraphElementState InactiveState { get; set; } = GraphElementState.Inactive;

    /// <summary>The currently hovered node ID, or null.</summary>
    public string? HoveredNodeId { get; private set; }

    /// <summary>Raised when the hover target changes.</summary>
    public event EventHandler? HoverChanged;

    /// <summary>
    /// Handles pointer moved to detect hover over nodes and update states.
    /// </summary>
    /// <returns>True if the hover state changed.</returns>
    public bool HandlePointerMoved(
        GraphData data,
        Point pointerPos,
        Func<GraphNode, Point> getWorldPos)
    {
        string? hitNodeId = null;

        // Find node under pointer
        for (int i = data.Nodes.Count - 1; i >= 0; i--)
        {
            var node = data.Nodes[i];
            var worldPos = getWorldPos(node);
            if (node.HitTest(worldPos, pointerPos))
            {
                hitNodeId = node.Id;
                break;
            }
        }

        // Check if hover target changed
        if (hitNodeId == HoveredNodeId) return false;

        // Reset previous states
        ResetStates(data);

        HoveredNodeId = hitNodeId;

        if (hitNodeId != null)
        {
            // Activate hovered node and its neighbors
            var hitNode = data.GetNode(hitNodeId);
            if (hitNode != null && hitNode.State == GraphElementState.Default)
                hitNode.State = ActiveState;

            var neighbors = new HashSet<string>(data.GetNeighbors(hitNodeId));

            foreach (var node in data.Nodes)
            {
                if (node.Id == hitNodeId) continue;
                if (node.State != GraphElementState.Selected)
                    node.State = neighbors.Contains(node.Id) ? ActiveState : InactiveState;
            }

            // Activate connected edges, deactivate others
            foreach (var edge in data.Edges)
            {
                if (edge.State == GraphElementState.Selected) continue;
                var isConnected = edge.Source == hitNodeId || edge.Target == hitNodeId;
                edge.State = isConnected ? ActiveState : InactiveState;
            }
        }

        HoverChanged?.Invoke(this, EventArgs.Empty);
        return true;
    }

    /// <summary>Resets all elements to default state (clears hover).</summary>
    public void Reset(GraphData data)
    {
        HoveredNodeId = null;
        ResetStates(data);
    }

    private void ResetStates(GraphData data)
    {
        foreach (var node in data.Nodes)
        {
            if (node.State != GraphElementState.Selected)
                node.State = GraphElementState.Default;
        }
        foreach (var edge in data.Edges)
        {
            if (edge.State != GraphElementState.Selected)
                edge.State = GraphElementState.Default;
        }
    }
}
