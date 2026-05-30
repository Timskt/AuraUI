using Avalonia;
using Avalonia.Input;
using AuraUI.Controls.Charts.Interaction;

namespace AuraUI.Controls.Charts.Rendering;

/// <summary>
/// Handles all pointer and wheel input for the chart: tooltip hover, click
/// events, zoom/pan, brush selection, toolbox interaction, data zoom slider,
/// and legend click-to-toggle. Extracted from Chart.cs for focused testing.
///
/// This class holds references to the chart's interactive state objects
/// (zoomPan, brushSelection, toolbox, dataZoom, legend) and modifies
/// tooltip state directly via the provided <see cref="TooltipState"/>.
/// </summary>
internal sealed class ChartInputHandler
{
    private readonly ChartZoomPan _zoomPan;
    private readonly ChartBrush _brushSelection;
    private readonly ChartToolbox _toolbox;
    private readonly DataZoom _dataZoom;
    private readonly ChartLegend _legend;
    private readonly ChartTooltip _tooltip;

    public ChartInputHandler(
        ChartZoomPan zoomPan,
        ChartBrush brushSelection,
        ChartToolbox toolbox,
        DataZoom dataZoom,
        ChartLegend legend,
        ChartTooltip tooltip)
    {
        _zoomPan = zoomPan;
        _brushSelection = brushSelection;
        _toolbox = toolbox;
        _dataZoom = dataZoom;
        _legend = legend;
        _tooltip = tooltip;
    }

    /// <summary>
    /// Result of a pointer move operation, indicating what action was taken.
    /// </summary>
    internal enum PointerMoveResult
    {
        None,
        Pan,
        Brush,
        DataZoom,
        Toolbox,
        TooltipUpdate,
        TooltipHide,
        OutsidePlot
    }

    /// <summary>
    /// Handle pointer movement for tooltip hit testing, zoom pan, brush selection,
    /// data zoom slider, and toolbox hover.
    /// </summary>
    /// <returns>Information about what happened (for the chart to act on).</returns>
    internal PointerMoveResult HandlePointerMoved(
        PointerEventArgs e,
        Point pos,
        Rect bounds,
        Rect plotArea,
        IReadOnlyList<ChartSeries> series,
        ChartAxis xAxis,
        ChartAxis yAxis,
        TooltipState tooltipState,
        List<TooltipSeriesEntry> axisEntriesBuffer,
        ChartTooltipRenderer.FindAxisEntriesFunc? findAxisEntries = null)
    {
        // Handle zoom pan
        if (_zoomPan.IsPanning)
        {
            _zoomPan.HandlePointerMoved(e, xAxis, yAxis, plotArea);
            return PointerMoveResult.Pan;
        }

        // Handle brush selection
        if (_brushSelection.IsActive)
        {
            _brushSelection.HandlePointerMoved(pos, plotArea);
            return PointerMoveResult.Brush;
        }

        // Handle data zoom slider
        if (_dataZoom.IsVisible)
        {
            var zoomBarRect = _dataZoom.ZoomType == DataZoomType.Inside
                ? plotArea
                : new Rect(plotArea.Left, plotArea.Bottom + 8, plotArea.Width, _dataZoom.Height);
            if (zoomBarRect.Contains(pos))
            {
                _dataZoom.HandlePointerMoved(pos, plotArea);
                return PointerMoveResult.DataZoom;
            }
        }

        // Handle toolbox hover
        if (_toolbox.IsVisible)
        {
            if (_toolbox.HandlePointerMoved(pos, bounds))
            {
                return PointerMoveResult.Toolbox;
            }
        }

        // Handle tooltip
        if (_tooltip.Trigger == TooltipTrigger.None || !_tooltip.IsEnabled)
            return PointerMoveResult.None;

        if (!plotArea.Contains(pos))
        {
            if (tooltipState.IsVisible)
            {
                tooltipState.IsVisible = false;
                return PointerMoveResult.TooltipHide;
            }
            return PointerMoveResult.OutsidePlot;
        }

        var hit = ChartHitTest.FindNearest(pos, series, plotArea, xAxis, yAxis);
        if (hit != null)
        {
            tooltipState.IsVisible = true;
            tooltipState.Position = hit.HitPosition;
            tooltipState.Series = hit.Series;
            tooltipState.DataPoint = hit.DataPoint;
            tooltipState.SliceData = hit.SliceData;
            tooltipState.DataIndex = hit.DataIndex;

            // For axis trigger: find all series at this X position
            if (_tooltip.Trigger == TooltipTrigger.Axis && hit.DataPoint != null)
            {
                tooltipState.CrosshairXValue = hit.DataPoint.X;
                tooltipState.CrosshairPixelX = xAxis.ValueToPixel(hit.DataPoint.X);
                if (findAxisEntries != null)
                    tooltipState.AxisEntries = findAxisEntries(hit.DataPoint.X, pos, series, xAxis, yAxis, axisEntriesBuffer);
            }

            return PointerMoveResult.TooltipUpdate;
        }
        else
        {
            if (tooltipState.IsVisible)
            {
                tooltipState.IsVisible = false;
                return PointerMoveResult.TooltipHide;
            }
            return PointerMoveResult.None;
        }
    }

    /// <summary>
    /// Result of a pointer press operation.
    /// </summary>
    internal enum PointerPressResult
    {
        None,
        PanStart,
        ToolboxHandled,
        DataZoomHandled,
        BrushStart,
        LegendToggle,
        PointClick
    }

    /// <summary>
    /// Handle pointer press for zoom pan, toolbox, data zoom, brush selection,
    /// legend toggle, and data point click.
    /// </summary>
    internal (PointerPressResult result, int legendIndex, ChartHitResult? hit) HandlePointerPressed(
        PointerPressedEventArgs e,
        Point pos,
        Rect bounds,
        Rect plotArea,
        IReadOnlyList<ChartSeries> series,
        ChartAxis xAxis,
        ChartAxis yAxis)
    {
        // Handle zoom pan
        if (_zoomPan.HandlePointerPressed(e, xAxis, yAxis, plotArea))
            return (PointerPressResult.PanStart, -1, null);

        // Handle toolbox button press
        if (_toolbox.IsVisible && _toolbox.HandlePointerPressed(pos, bounds))
            return (PointerPressResult.ToolboxHandled, -1, null);

        // Handle data zoom slider press
        if (_dataZoom.IsVisible)
        {
            var zoomBarRect = _dataZoom.ZoomType == DataZoomType.Inside
                ? plotArea
                : new Rect(plotArea.Left, plotArea.Bottom + 8, plotArea.Width, _dataZoom.Height);
            if (zoomBarRect.Contains(pos) && _dataZoom.HandlePointerPressed(pos, plotArea))
                return (PointerPressResult.DataZoomHandled, -1, null);
        }

        // Handle brush selection start
        if (_brushSelection.IsEnabled && plotArea.Contains(pos))
        {
            if (_brushSelection.HandlePointerPressed(pos, plotArea))
                return (PointerPressResult.BrushStart, -1, null);
        }

        // Check legend click-to-toggle
        if (_legend.EnableToggle && _legend.IsVisible && _legend.LayoutRect.Contains(pos))
        {
            var seriesIndex = _legend.HitTest(pos, series);
            if (seriesIndex >= 0 && seriesIndex < series.Count)
                return (PointerPressResult.LegendToggle, seriesIndex, null);
        }

        // Click on data point
        var hit = ChartHitTest.FindNearest(pos, series, plotArea, xAxis, yAxis);
        if (hit != null)
            return (PointerPressResult.PointClick, -1, hit);

        return (PointerPressResult.None, -1, null);
    }

    /// <summary>
    /// Handle pointer release for zoom pan, toolbox, data zoom, and brush selection.
    /// </summary>
    internal bool HandlePointerReleased(
        PointerReleasedEventArgs e,
        Point pos,
        Rect bounds)
    {
        var handled = false;

        if (_zoomPan.HandlePointerReleased(e))
            handled = true;

        if (_toolbox.IsVisible)
            _toolbox.HandlePointerReleased(pos, bounds);

        if (_dataZoom.IsVisible)
            _dataZoom.HandlePointerReleased();

        if (_brushSelection.IsActive)
        {
            _brushSelection.HandlePointerReleased();
            handled = true;
        }

        return handled;
    }

    /// <summary>
    /// Handle mouse wheel for zoom/pan.
    /// </summary>
    internal bool HandlePointerWheelChanged(
        PointerWheelEventArgs e,
        ChartAxis xAxis,
        ChartAxis yAxis,
        Rect plotArea)
    {
        return _zoomPan.HandleWheel(e, xAxis, yAxis, plotArea);
    }

    /// <summary>
    /// Handle double-tap for zoom reset.
    /// </summary>
    internal bool HandleDoubleTapped(
        TappedEventArgs e,
        ChartAxis xAxis,
        ChartAxis yAxis,
        Rect plotArea)
    {
        return _zoomPan.HandleDoubleTapped(e, xAxis, yAxis, plotArea);
    }

    /// <summary>
    /// Whether the zoom pan is currently active.
    /// </summary>
    internal bool IsPanning => _zoomPan.IsPanning;

    /// <summary>
    /// Reset zoom factor to 1.0.
    /// </summary>
    internal void ResetZoom() => _zoomPan.ZoomFactor = 1.0;
}
