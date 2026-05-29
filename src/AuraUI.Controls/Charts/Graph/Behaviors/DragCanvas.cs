using Avalonia;
using Avalonia.Input;

namespace AuraUI.Controls.Charts.Graph.Behaviors;

/// <summary>
/// Behavior that allows panning the graph canvas by clicking and dragging on empty space.
/// </summary>
public class DragCanvas
{
    /// <summary>Whether the canvas is currently being dragged (panned).</summary>
    public bool IsPanning { get; private set; }

    /// <summary>The pointer position when panning started.</summary>
    private Point _panStart;

    /// <summary>The pan offset when panning started.</summary>
    private Point _panOffsetStart;

    /// <summary>
    /// Handles pointer pressed on empty canvas space. Begins panning.
    /// </summary>
    /// <param name="pointerPos">Current pointer position.</param>
    /// <param name="currentPanOffset">Current pan offset of the canvas.</param>
    /// <returns>True if panning was initiated.</returns>
    public bool HandlePointerPressed(Point pointerPos, Point currentPanOffset)
    {
        IsPanning = true;
        _panStart = pointerPos;
        _panOffsetStart = currentPanOffset;
        return true;
    }

    /// <summary>
    /// Handles pointer moved during panning.
    /// </summary>
    /// <param name="pointerPos">Current pointer position.</param>
    /// <returns>The new pan offset.</returns>
    public Point HandlePointerMoved(Point pointerPos)
    {
        if (!IsPanning) return _panOffsetStart;

        return new Point(
            _panOffsetStart.X + (pointerPos.X - _panStart.X),
            _panOffsetStart.Y + (pointerPos.Y - _panStart.Y));
    }

    /// <summary>
    /// Handles pointer released. Ends panning.
    /// </summary>
    /// <returns>True if panning was active and is now ended.</returns>
    public bool HandlePointerReleased()
    {
        if (!IsPanning) return false;
        IsPanning = false;
        return true;
    }
}
