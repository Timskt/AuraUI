using Avalonia;
using Avalonia.Input;

namespace AuraUI.Controls.Charts.Graph.Behaviors;

/// <summary>
/// Behavior for selecting graph nodes and edges by clicking.
/// Supports single and multi-select modes.
/// </summary>
public class ClickSelect
{
    /// <summary>Whether multi-select is enabled (ctrl+click to add to selection).</summary>
    public bool Multiple { get; set; }

    /// <summary>What triggers selection: single click or double click.</summary>
    public BehaviorTrigger Trigger { get; set; } = BehaviorTrigger.Click;

    /// <summary>Currently selected node IDs.</summary>
    public HashSet<string> SelectedNodeIds { get; } = new();

    /// <summary>Currently selected edge indices.</summary>
    public HashSet<int> SelectedEdgeIndices { get; } = new();

    /// <summary>Raised when the selection changes.</summary>
    public event EventHandler? SelectionChanged;

    /// <summary>
    /// Handles a click event and updates selection state.
    /// </summary>
    /// <returns>True if the selection state changed.</returns>
    public bool HandleClick(
        IList<GraphNode> nodes,
        IList<GraphEdge> edges,
        Point pointerPos,
        Func<GraphNode, Point> getWorldPos,
        Func<GraphEdge, Point, Point, bool> hitTestEdge,
        KeyModifiers modifiers,
        bool isDoubleClick)
    {
        // Only respond to the configured trigger
        if (Trigger == BehaviorTrigger.Click && isDoubleClick) return false;
        if (Trigger == BehaviorTrigger.DoubleClick && !isDoubleClick) return false;

        var ctrlHeld = modifiers.HasFlag(KeyModifiers.Control) || modifiers.HasFlag(KeyModifiers.Meta);

        // Check nodes first (higher z-priority)
        for (int i = nodes.Count - 1; i >= 0; i--)
        {
            var node = nodes[i];
            var worldPos = getWorldPos(node);
            if (node.HitTest(worldPos, pointerPos))
            {
                if (!Multiple || !ctrlHeld)
                {
                    // Single select: clear previous selection
                    ClearSelection(nodes, edges);
                }

                if (SelectedNodeIds.Contains(node.Id))
                {
                    SelectedNodeIds.Remove(node.Id);
                    node.IsSelected = false;
                    node.State = GraphElementState.Default;
                }
                else
                {
                    SelectedNodeIds.Add(node.Id);
                    node.IsSelected = true;
                    node.State = GraphElementState.Selected;
                }

                SelectionChanged?.Invoke(this, EventArgs.Empty);
                return true;
            }
        }

        // Check edges
        for (int i = 0; i < edges.Count; i++)
        {
            var edge = edges[i];
            if (edge.IsHidden) continue;

            // Find source and target nodes
            var sourceNode = nodes.FirstOrDefault(n => n.Id == edge.Source);
            var targetNode = nodes.FirstOrDefault(n => n.Id == edge.Target);
            if (sourceNode == null || targetNode == null) continue;

            if (hitTestEdge(edge, getWorldPos(sourceNode), getWorldPos(targetNode)))
            {
                if (!Multiple || !ctrlHeld)
                    ClearSelection(nodes, edges);

                if (SelectedEdgeIndices.Contains(i))
                {
                    SelectedEdgeIndices.Remove(i);
                    edge.IsSelected = false;
                    edge.State = GraphElementState.Default;
                }
                else
                {
                    SelectedEdgeIndices.Add(i);
                    edge.IsSelected = true;
                    edge.State = GraphElementState.Selected;
                }

                SelectionChanged?.Invoke(this, EventArgs.Empty);
                return true;
            }
        }

        // Clicked on empty space — clear selection
        if (!ctrlHeld)
        {
            ClearSelection(nodes, edges);
            SelectionChanged?.Invoke(this, EventArgs.Empty);
            return true;
        }

        return false;
    }

    /// <summary>Clears all selection state.</summary>
    public void ClearSelection(IList<GraphNode> nodes, IList<GraphEdge> edges)
    {
        foreach (var nodeId in SelectedNodeIds)
        {
            var node = nodes.FirstOrDefault(n => n.Id == nodeId);
            if (node != null)
            {
                node.IsSelected = false;
                node.State = GraphElementState.Default;
            }
        }
        SelectedNodeIds.Clear();

        foreach (var idx in SelectedEdgeIndices)
        {
            if (idx < edges.Count)
            {
                edges[idx].IsSelected = false;
                edges[idx].State = GraphElementState.Default;
            }
        }
        SelectedEdgeIndices.Clear();
    }
}
