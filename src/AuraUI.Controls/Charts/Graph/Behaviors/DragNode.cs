using Avalonia;

namespace AuraUI.Controls.Charts.Graph.Behaviors;

/// <summary>
/// Behavior that allows dragging graph nodes to reposition them.
/// Optionally delegates to a layout recalculation after drag ends.
/// All positions passed to this behavior should be in graph-space coordinates.
/// </summary>
public class DragNode
{
    /// <summary>Whether to delegate to layout recalculation after drag.</summary>
    public bool EnableDelegate { get; set; }

    /// <summary>
    /// Delegate called after a drag completes (if EnableDelegate is true).
    /// Use this to trigger a layout pass.
    /// </summary>
    public Action? DelegateStyle { get; set; }

    /// <summary>Whether a node is currently being dragged.</summary>
    public bool IsDragging { get; private set; }

    /// <summary>The node currently being dragged, or null.</summary>
    public GraphNode? DraggedNode { get; private set; }

    /// <summary>Offset from the node center to the initial click point, in graph space.</summary>
    private Point _dragOffset;

    /// <summary>
    /// Handles pointer pressed. If the pointer hits a node, begins dragging.
    /// </summary>
    /// <param name="nodes">All graph nodes.</param>
    /// <param name="pointerPos">Pointer position in graph space.</param>
    /// <param name="getWorldPos">Function to get a node's graph-space position.</param>
    /// <returns>True if a drag was initiated.</returns>
    public bool HandlePointerPressed(IList<GraphNode> nodes, Point pointerPos, Func<GraphNode, Point> getWorldPos)
    {
        // Find the node under the pointer (check in reverse for z-order)
        for (int i = nodes.Count - 1; i >= 0; i--)
        {
            var node = nodes[i];
            if (node.IsHidden) continue;
            var worldPos = getWorldPos(node);
            if (node.HitTest(worldPos, pointerPos))
            {
                IsDragging = true;
                DraggedNode = node;
                _dragOffset = new Point(pointerPos.X - worldPos.X, pointerPos.Y - worldPos.Y);
                node.IsFixed = true;
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Handles pointer moved during drag. Updates the dragged node's position.
    /// </summary>
    /// <param name="graphPos">Pointer position in graph space.</param>
    /// <returns>True if the drag was active and the node was moved.</returns>
    public bool HandlePointerMoved(Point graphPos)
    {
        if (!IsDragging || DraggedNode == null) return false;

        DraggedNode.Position = new Point(
            graphPos.X - _dragOffset.X,
            graphPos.Y - _dragOffset.Y);

        return true;
    }

    /// <summary>
    /// Handles pointer released. Ends the drag and optionally triggers layout.
    /// </summary>
    public bool HandlePointerReleased()
    {
        if (!IsDragging || DraggedNode == null) return false;

        DraggedNode.IsFixed = false;
        IsDragging = false;
        DraggedNode = null;

        if (EnableDelegate)
            DelegateStyle?.Invoke();

        return true;
    }
}
