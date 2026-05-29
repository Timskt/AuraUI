using Avalonia;
using Avalonia.Animation.Easings;
using Avalonia.Input;
using Avalonia.Threading;

namespace AuraUI.Controls.Charts.Interaction;

/// <summary>
/// Manages zoom and pan state for a chart. Supports:
///   - Mouse wheel zoom (centered on pointer position)
///   - Click-and-drag pan
///   - Pinch-to-zoom (touch)
///   - Double-click to reset zoom
///   - Smooth zoom animation via DispatcherTimer interpolation
///   - Zoom constraints (min/max zoom level)
///
/// The zoom/pan state is stored as axis range overrides that the chart
/// applies during the layout pass.
/// </summary>
internal sealed class ChartZoomPan : IDisposable
{
    private bool _isPanning;
    private Point _panStart;
    private double _panStartXMin, _panStartXMax;
    private double _panStartYMin, _panStartYMax;

    // Smooth zoom animation state
    private DispatcherTimer? _smoothZoomTimer;
    private DateTime _smoothZoomStart;
    private static readonly TimeSpan SmoothZoomDuration = TimeSpan.FromMilliseconds(200);
    private Easing _smoothZoomEasing = new CubicEaseOut();

    // Zoom animation: these are the "from" values; the "to" values are written directly to axis
    private double _animFromXMin, _animFromXMax;
    private double _animFromYMin, _animFromYMax;
    private double _animToXMin, _animToXMax;
    private double _animToYMin, _animToYMax;
    private bool _isAnimatingZoom;

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
    /// Whether to use smooth animation when zooming.
    /// </summary>
    public bool SmoothZoom { get; set; } = true;

    /// <summary>
    /// Current zoom center in data coordinates.
    /// </summary>
    public Point ZoomCenter { get; private set; }

    /// <summary>
    /// Current zoom factor (1.0 = no zoom, 2.0 = zoomed in 2x).
    /// </summary>
    public double ZoomFactor { get; set; } = 1.0;

    /// <summary>
    /// Raised when the zoom range changes (after animation completes or immediately).
    /// </summary>
    public event Action? ZoomRangeChanged;

    /// <summary>
    /// Whether a smooth zoom animation is currently running.
    /// </summary>
    public bool IsAnimatingZoom => _isAnimatingZoom;

    /// <summary>
    /// Handle mouse wheel for zoom.
    /// </summary>
    public bool HandleWheel(PointerWheelEventArgs e, ChartAxis xAxis, ChartAxis yAxis, Rect plotArea)
    {
        if (!IsEnabled) return false;
        if (!plotArea.Contains(e.GetPosition((Visual)e.Source!))) return false;

        var pointerPos = e.GetPosition((Visual)e.Source!);
        var zoomDelta = e.Delta.Y > 0 ? 1.1 : 0.9;

        if (SmoothZoom)
        {
            StartSmoothZoom(xAxis, yAxis, pointerPos, plotArea, zoomDelta);
        }
        else
        {
            ZoomAxis(xAxis, pointerPos.X, plotArea.Left, plotArea.Width, zoomDelta, MinZoomRange);
            ZoomAxis(yAxis, pointerPos.Y, plotArea.Top, plotArea.Height, zoomDelta, MinZoomRange);
        }

        ZoomFactor *= zoomDelta;
        ZoomRangeChanged?.Invoke();
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

        // Cancel any in-progress zoom animation
        CancelSmoothZoom();

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

        if (SmoothZoom)
        {
            // Animate back to auto-range
            StartSmoothZoomToRange(xAxis, yAxis, double.NaN, double.NaN, double.NaN, double.NaN);
        }
        else
        {
            xAxis.EffectiveMin = double.NaN;
            xAxis.EffectiveMax = double.NaN;
            yAxis.EffectiveMin = double.NaN;
            yAxis.EffectiveMax = double.NaN;
        }

        ZoomFactor = 1.0;
        ZoomRangeChanged?.Invoke();
        return true;
    }

    /// <summary>
    /// Programmatically zoom to a specific X range (used by DataZoom).
    /// </summary>
    public void ZoomToXRange(ChartAxis xAxis, ChartAxis yAxis, double xMin, double xMax, Rect plotArea)
    {
        if (!IsEnabled) return;

        if (SmoothZoom)
        {
            StartSmoothZoomToRange(xAxis, yAxis, xMin, xMax, yAxis.EffectiveMin, yAxis.EffectiveMax);
        }
        else
        {
            xAxis.EffectiveMin = xMin;
            xAxis.EffectiveMax = xMax;
        }

        ZoomRangeChanged?.Invoke();
    }

    public bool IsPanning => _isPanning;

    // ────────────────────────────────────────────────
    //  Smooth zoom animation
    // ────────────────────────────────────────────────

    private void StartSmoothZoom(ChartAxis xAxis, ChartAxis yAxis, Point pointerPos, Rect plotArea, double zoomDelta)
    {
        // Save current as "from"
        _animFromXMin = xAxis.EffectiveMin;
        _animFromXMax = xAxis.EffectiveMax;
        _animFromYMin = yAxis.EffectiveMin;
        _animFromYMax = yAxis.EffectiveMax;

        // Compute "to" by applying the zoom
        var toXMin = xAxis.EffectiveMin;
        var toXMax = xAxis.EffectiveMax;
        var toYMin = yAxis.EffectiveMin;
        var toYMax = yAxis.EffectiveMax;

        ComputeZoomedRange(ref toXMin, ref toXMax, pointerPos.X, plotArea.Left, plotArea.Width, zoomDelta, MinZoomRange);
        ComputeZoomedRange(ref toYMin, ref toYMax, pointerPos.Y, plotArea.Top, plotArea.Height, zoomDelta, MinZoomRange);

        _animToXMin = toXMin;
        _animToXMax = toXMax;
        _animToYMin = toYMin;
        _animToYMax = toYMax;

        BeginSmoothAnimation(xAxis, yAxis);
    }

    private void StartSmoothZoomToRange(ChartAxis xAxis, ChartAxis yAxis, double xMin, double xMax, double yMin, double yMax)
    {
        _animFromXMin = xAxis.EffectiveMin;
        _animFromXMax = xAxis.EffectiveMax;
        _animFromYMin = yAxis.EffectiveMin;
        _animFromYMax = yAxis.EffectiveMax;

        _animToXMin = xMin;
        _animToXMax = xMax;
        _animToYMin = yMin;
        _animToYMax = yMax;

        BeginSmoothAnimation(xAxis, yAxis);
    }

    private void BeginSmoothAnimation(ChartAxis xAxis, ChartAxis yAxis)
    {
        _smoothZoomStart = DateTime.UtcNow;
        _isAnimatingZoom = true;

        if (_smoothZoomTimer == null)
        {
            _smoothZoomTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(16) };
            _smoothZoomTimer.Tick += (_, _) => OnSmoothZoomTick(xAxis, yAxis);
        }

        _smoothZoomTimer.Start();
    }

    private void OnSmoothZoomTick(ChartAxis xAxis, ChartAxis yAxis)
    {
        var elapsed = DateTime.UtcNow - _smoothZoomStart;
        var rawProgress = SmoothZoomDuration.TotalMilliseconds > 0
            ? elapsed.TotalMilliseconds / SmoothZoomDuration.TotalMilliseconds
            : 1.0;

        var progress = _smoothZoomEasing.Ease(Math.Clamp(rawProgress, 0.0, 1.0));

        // Lerp from -> to
        xAxis.EffectiveMin = Lerp(_animFromXMin, _animToXMin, progress);
        xAxis.EffectiveMax = Lerp(_animFromXMax, _animToXMax, progress);
        yAxis.EffectiveMin = Lerp(_animFromYMin, _animToYMin, progress);
        yAxis.EffectiveMax = Lerp(_animFromYMax, _animToYMax, progress);

        ZoomRangeChanged?.Invoke();

        if (rawProgress >= 1.0)
        {
            _smoothZoomTimer?.Stop();
            _isAnimatingZoom = false;
        }
    }

    private void CancelSmoothZoom()
    {
        if (_isAnimatingZoom)
        {
            _smoothZoomTimer?.Stop();
            _isAnimatingZoom = false;
        }
    }

    private static double Lerp(double a, double b, double t) => a + (b - a) * t;

    private static void ComputeZoomedRange(ref double min, ref double max, double pointerPixel, double plotStart, double plotSize, double zoomDelta, double minRange)
    {
        var dataRange = max - min;
        if (dataRange <= 0) return;

        var fraction = (pointerPixel - plotStart) / plotSize;
        var pivotValue = min + fraction * dataRange;

        var newRange = dataRange / zoomDelta;
        newRange = Math.Max(newRange, minRange);
        newRange = Math.Min(newRange, dataRange * 1000);

        min = pivotValue - fraction * newRange;
        max = pivotValue + (1 - fraction) * newRange;
    }

    private static void ZoomAxis(ChartAxis axis, double pointerPixel, double plotStart, double plotSize, double zoomDelta, double minRange)
    {
        var dataRange = axis.EffectiveMax - axis.EffectiveMin;
        if (dataRange <= 0) return;

        var fraction = (pointerPixel - plotStart) / plotSize;
        var pivotValue = axis.EffectiveMin + fraction * dataRange;

        var newRange = dataRange / zoomDelta;
        newRange = Math.Max(newRange, minRange);
        newRange = Math.Min(newRange, dataRange * 1000);

        axis.EffectiveMin = pivotValue - fraction * newRange;
        axis.EffectiveMax = pivotValue + (1 - fraction) * newRange;
    }

    public void Dispose()
    {
        _smoothZoomTimer?.Stop();
        if (_smoothZoomTimer != null)
        {
            _smoothZoomTimer.Tick -= (_, _) => { };
            _smoothZoomTimer = null;
        }
    }
}
