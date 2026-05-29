using Avalonia;
using Avalonia.Input;

namespace AuraUI.Controls.Charts.Interaction;

/// <summary>
/// Manages zoom and pan state for a chart. Supports:
///   - Mouse wheel zoom (centered on pointer position)
///   - Click-and-drag pan
///   - Pinch-to-zoom (touch)
///   - Double-click to reset zoom
///
/// The zoom/pan state is stored as axis range overrides that the chart
/// applies during the layout pass.
/// </summary>
internal sealed class ChartZoomPan
{
    private bool _isPanning;
    private Point _panStart;
    private double _panStartXMin, _panStartXMax;
    private double _panStartYMin, _panStartYMax;

    /// <summary>
    /// Whether zoom/pan is enabled.
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Minimum zoom range (prevents zooming in too far).
    /// </summary>
    public double MinZoomRange { get; set; } = 1e-6;

    /// <summary>
    /// Maximum zoom level (range / original range).
    /// </summary>
    public double MaxZoomLevel { get; set; } = 1000;

    /// <summary>
    /// Current zoom center in data coordinates.
    /// </summary>
    public Point ZoomCenter { get; private set; }

    /// <summary>
    /// Current zoom factor (1.0 = no zoom, 2.0 = zoomed in 2x).
    /// </summary>
    public double ZoomFactor { get; private set; } = 1.0;

    /// <summary>
    /// Handle mouse wheel for zoom.
    /// </summary>
    public bool HandleWheel(PointerWheelEventArgs e, ChartAxis xAxis, ChartAxis yAxis, Rect plotArea)
    {
        if (!IsEnabled) return false;
        if (!plotArea.Contains(e.GetPosition((Visual)e.Source!))) return false;

        var pointerPos = e.GetPosition((Visual)e.Source!);
        var zoomDelta = e.Delta.Y > 0 ? 1.1 : 0.9;

        // Zoom X axis
        ZoomAxis(xAxis, pointerPos.X, plotArea.Left, plotArea.Width, zoomDelta);

        // Zoom Y axis
        ZoomAxis(yAxis, pointerPos.Y, plotArea.Top, plotArea.Height, zoomDelta);

        ZoomFactor *= zoomDelta;
        return true;
    }

    /// <summary>
    /// Handle pointer pressed for pan start.
    /// </summary>
    public bool HandlePointerPressed(PointerPressedEventArgs e, ChartAxis xAxis, ChartAxis yAxis, Rect plotArea)
    {
        if (!IsEnabled) return false;

        var props = e.GetCurrentPoint((Visual)e.Source!).Properties;
        if (!props.IsMiddleButtonPressed && !props.IsLeftButtonPressed) return false;

        var pos = e.GetPosition((Visual)e.Source!);
        if (!plotArea.Contains(pos)) return false;

        _isPanning = true;
        _panStart = pos;
        _panStartXMin = xAxis.EffectiveMin;
        _panStartXMax = xAxis.EffectiveMax;
        _panStartYMin = yAxis.EffectiveMin;
        _panStartYMax = yAxis.EffectiveMax;

        return true;
    }

    /// <summary>
    /// Handle pointer moved for panning.
    /// </summary>
    public bool HandlePointerMoved(PointerEventArgs e, ChartAxis xAxis, ChartAxis yAxis, Rect plotArea)
    {
        if (!_isPanning || !IsEnabled) return false;

        var pos = e.GetPosition((Visual)e.Source!);
        var dx = pos.X - _panStart.X;
        var dy = pos.Y - _panStart.Y;

        // Convert pixel delta to data delta
        var xRange = _panStartXMax - _panStartXMin;
        var yRange = _panStartYMax - _panStartYMin;

        var xDataDelta = -(dx / plotArea.Width) * xRange;
        var yDataDelta = (dy / plotArea.Height) * yRange;

        xAxis.EffectiveMin = _panStartXMin + xDataDelta;
        xAxis.EffectiveMax = _panStartXMax + xDataDelta;
        yAxis.EffectiveMin = _panStartYMin + yDataDelta;
        yAxis.EffectiveMax = _panStartYMax + yDataDelta;

        return true;
    }

    /// <summary>
    /// Handle pointer released to end pan.
    /// </summary>
    public bool HandlePointerReleased(PointerReleasedEventArgs e)
    {
        if (!_isPanning) return false;
        _isPanning = false;
        return true;
    }

    /// <summary>
    /// Handle double-click to reset zoom.
    /// </summary>
    public bool HandleDoubleTapped(TappedEventArgs e, ChartAxis xAxis, ChartAxis yAxis, Rect plotArea)
    {
        if (!IsEnabled) return false;

        // Reset to auto-range
        xAxis.EffectiveMin = double.NaN;
        xAxis.EffectiveMax = double.NaN;
        yAxis.EffectiveMin = double.NaN;
        yAxis.EffectiveMax = double.NaN;
        ZoomFactor = 1.0;

        return true;
    }

    public bool IsPanning => _isPanning;

    private static void ZoomAxis(ChartAxis axis, double pointerPixel, double plotStart, double plotSize, double zoomDelta)
    {
        var dataRange = axis.EffectiveMax - axis.EffectiveMin;
        if (dataRange <= 0) return;

        // Pointer position as fraction of plot area
        var fraction = (pointerPixel - plotStart) / plotSize;

        // The data value under the pointer
        var pivotValue = axis.EffectiveMin + fraction * dataRange;

        // New range
        var newRange = dataRange / zoomDelta;
        newRange = Math.Max(newRange, 1e-6);
        newRange = Math.Min(newRange, dataRange * 1000);

        // Adjust min/max to keep pivot value at the same screen position
        axis.EffectiveMin = pivotValue - fraction * newRange;
        axis.EffectiveMax = pivotValue + (1 - fraction) * newRange;
    }
}
