using Avalonia;
using Avalonia.Input;

namespace AuraUI.Controls.Charts.Graph.Behaviors;

/// <summary>
/// Behavior that handles mouse wheel zoom on the graph canvas.
/// Zooms centered on the cursor position.
/// </summary>
public class ZoomCanvas
{
    /// <summary>Minimum zoom level.</summary>
    public double MinZoom { get; set; } = 0.1;

    /// <summary>Maximum zoom level.</summary>
    public double MaxZoom { get; set; } = 5.0;

    /// <summary>Zoom step per wheel notch.</summary>
    public double ZoomStep { get; set; } = 0.1;

    /// <summary>
    /// If true, enables zoom optimization by throttling zoom events.
    /// </summary>
    public bool EnableOptimize { get; set; }

    /// <summary>Current zoom level.</summary>
    public double CurrentZoom { get; set; } = 1.0;

    /// <summary>Current pan offset (in screen pixels).</summary>
    public Point PanOffset { get; set; }

    /// <summary>
    /// Handles mouse wheel events to zoom in/out at the cursor position.
    /// </summary>
    /// <returns>True if the zoom was applied.</returns>
    public bool HandleWheel(PointerWheelEventArgs e)
    {
        var delta = e.Delta.Y;
        if (delta == 0) return false;

        var pointerPos = e.GetPosition(e.Source as Visual);
        var oldZoom = CurrentZoom;

        // Compute new zoom
        var zoomDelta = delta > 0 ? ZoomStep : -ZoomStep;
        CurrentZoom = Math.Clamp(CurrentZoom + zoomDelta, MinZoom, MaxZoom);

        if (Math.Abs(CurrentZoom - oldZoom) < 1e-6) return false;

        // Adjust pan so the point under the cursor stays fixed
        var zoomRatio = CurrentZoom / oldZoom;
        PanOffset = new Point(
            pointerPos.X - (pointerPos.X - PanOffset.X) * zoomRatio,
            pointerPos.Y - (pointerPos.Y - PanOffset.Y) * zoomRatio);

        return true;
    }

    /// <summary>
    /// Transforms a screen-space point to graph-space coordinates.
    /// </summary>
    public Point ScreenToGraph(Point screenPos)
    {
        return new Point(
            (screenPos.X - PanOffset.X) / CurrentZoom,
            (screenPos.Y - PanOffset.Y) / CurrentZoom);
    }

    /// <summary>
    /// Transforms a graph-space point to screen-space coordinates.
    /// </summary>
    public Point GraphToScreen(Point graphPos)
    {
        return new Point(
            graphPos.X * CurrentZoom + PanOffset.X,
            graphPos.Y * CurrentZoom + PanOffset.Y);
    }

    /// <summary>
    /// Resets zoom and pan to default.
    /// </summary>
    public void Reset()
    {
        CurrentZoom = 1.0;
        PanOffset = new Point(0, 0);
    }
}
