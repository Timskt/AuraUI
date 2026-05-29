using Avalonia;
using Avalonia.Media;

namespace AuraUI.Controls.Charts.Interaction;

/// <summary>
/// A slider-based zoom control for charts. Renders a range selection bar
/// that allows users to select a subset of data.
///
/// Supports two display modes:
///   - <see cref="DataZoomType.Slider"/>: renders below the chart as a separate control
///   - <see cref="DataZoomType.Inside"/>: renders as an overlay on the chart area
///
/// Rendering:
///   - Background track showing the full data range
///   - Selected range highlighted with handles on each end
///   - Drag handles to resize the selection
///   - Drag the middle to pan the selection
///
/// Properties:
///   - Start, End: normalized 0-1 range selection
///   - MinValue, MaxValue: explicit data range (if NaN, derived from axis)
///   - Height: slider height in pixels
///   - ZoomType: Slider or Inside
/// </summary>
public class DataZoom
{
    /// <summary>Whether the data zoom control is visible.</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>
    /// Display mode: Slider (below chart) or Inside (overlay on chart).
    /// </summary>
    public DataZoomType ZoomType { get; set; } = DataZoomType.Slider;

    /// <summary>Height of the data zoom bar in pixels.</summary>
    public double Height { get; set; } = 30;

    /// <summary>Start of the selected range (0-1).</summary>
    public double Start { get; set; }

    /// <summary>End of the selected range (0-1).</summary>
    public double End { get; set; } = 1.0;

    /// <summary>
    /// Minimum data value for the zoom range. When NaN, derived from axis auto-range.
    /// </summary>
    public double MinValue { get; set; } = double.NaN;

    /// <summary>
    /// Maximum data value for the zoom range. When NaN, derived from axis auto-range.
    /// </summary>
    public double MaxValue { get; set; } = double.NaN;

    /// <summary>Minimum span of the selection (0-1).</summary>
    public double MinSpan { get; set; } = 0.05;

    /// <summary>Maximum span of the selection (0-1, 1 = no limit).</summary>
    public double MaxSpan { get; set; } = 1.0;

    /// <summary>Background brush for the zoom bar.</summary>
    public IBrush? Background { get; set; }

    /// <summary>Brush for the selected range area.</summary>
    public IBrush? SelectionBrush { get; set; }

    /// <summary>Brush for the handle bars.</summary>
    public IBrush? HandleBrush { get; set; }

    /// <summary>Brush for the unselected masked area (inside mode only).</summary>
    public IBrush? MaskBrush { get; set; }

    /// <summary>Border brush for the data zoom control.</summary>
    public IBrush? BorderBrush { get; set; }

    /// <summary>Whether this zoom is for the X axis (true) or Y axis (false).</summary>
    public bool IsXAxis { get; set; } = true;

    /// <summary>
    /// Lock the current zoom level. When true, the user cannot change the zoom range
    /// via the slider (but programmatic changes still work).
    /// </summary>
    public bool ZoomLock { get; set; }

    /// <summary>
    /// Fill color for the selected range area (overrides SelectionBrush for slider track fill).
    /// </summary>
    public IBrush? FillerColor { get; set; }

    /// <summary>
    /// Style configuration for the drag handles.
    /// </summary>
    public DataZoomHandleStyle? HandleStyle { get; set; }

    /// <summary>
    /// Background configuration for the data preview area (mini chart in the slider).
    /// </summary>
    public DataZoomDataBackground? DataBackground { get; set; }

    /// <summary>The pixel rect allocated to this zoom control (set by chart layout).</summary>
    internal Rect LayoutRect { get; set; }

    /// <summary>
    /// Raised when the zoom range changes.
    /// </summary>
    public event Action<double, double>? RangeChanged;

    private bool _isDraggingHandle;
    private bool _isDraggingBody;
    private Point _dragStart;
    private double _dragStartStart;
    private double _dragStartEnd;
    private int _activeHandle; // -1 = left, 0 = body, 1 = right

    /// <summary>
    /// Render the data zoom control.
    /// </summary>
    /// <param name="context">Drawing context.</param>
    /// <param name="plotArea">The chart plot area rect.</param>
    /// <param name="dataPoints">Optional mini-chart data to render inside the zoom track.</param>
    public void Render(DrawingContext context, Rect plotArea, IList<ChartDataPoint>? dataPoints = null)
    {
        if (!IsVisible) return;

        // Render slider type (or Both)
        if (ZoomType == DataZoomType.Slider || ZoomType == DataZoomType.Both)
        {
            var sliderRect = new Rect(plotArea.Left, plotArea.Bottom + 8, plotArea.Width, Height);
            LayoutRect = sliderRect;
            RenderSlider(context, sliderRect, dataPoints);
        }

        // Render inside type (or Both)
        if (ZoomType == DataZoomType.Inside || ZoomType == DataZoomType.Both)
        {
            RenderInsideOverlay(context, plotArea);
        }
    }

    private void RenderSlider(DrawingContext context, Rect barRect, IList<ChartDataPoint>? dataPoints)
    {
        // Data background
        if (DataBackground != null)
        {
            var dataBgBrush = DataBackground.BackgroundColor ?? new SolidColorBrush(Colors.LightGray, 0.15);
            context.DrawRectangle(dataBgBrush, null, barRect);
        }

        // Background
        var bgBrush = Background ?? new SolidColorBrush(Colors.LightGray, 0.3);
        context.DrawRectangle(bgBrush, null, barRect);

        // Optional: draw mini chart preview
        if (dataPoints != null && dataPoints.Count > 1)
        {
            RenderMiniChart(context, barRect, dataPoints);
        }

        // Selected range
        var selLeft = barRect.Left + Start * barRect.Width;
        var selRight = barRect.Left + End * barRect.Width;
        var selRect = new Rect(selLeft, barRect.Top, selRight - selLeft, barRect.Height);

        var selBrush = FillerColor ?? SelectionBrush ?? new SolidColorBrush(Color.Parse("#6750A4"), 0.2);
        context.DrawRectangle(selBrush, null, selRect);

        // Handle style
        var handleBrush = HandleStyle?.Color ?? HandleBrush ?? new SolidColorBrush(Color.Parse("#6750A4"));
        var handleWidth = HandleStyle?.Width ?? 2.0;
        var handlePen = new Pen(handleBrush, handleWidth);

        // Left handle
        context.DrawLine(handlePen,
            new Point(selLeft, barRect.Top),
            new Point(selLeft, barRect.Bottom));

        // Right handle
        context.DrawLine(handlePen,
            new Point(selRight, barRect.Top),
            new Point(selRight, barRect.Bottom));

        // Handle grip indicators
        var dotY = barRect.Center.Y;
        var dotSize = 3.0;
        context.DrawEllipse(handleBrush, null, new Point(selLeft, dotY), dotSize, dotSize);
        context.DrawEllipse(handleBrush, null, new Point(selRight, dotY), dotSize, dotSize);

        // Lock indicator
        if (ZoomLock)
        {
            var lockBrush = new SolidColorBrush(Colors.Gray, 0.5);
            var lockRect = new Rect(barRect.Center.X - 6, barRect.Center.Y - 6, 12, 12);
            context.DrawRectangle(lockBrush, null, lockRect, 2, 2);
        }

        // Border
        if (BorderBrush is IBrush border)
        {
            var borderPen = new Pen(border, 1.0);
            context.DrawRectangle(null, borderPen, barRect);
        }
    }

    private void RenderInsideOverlay(DrawingContext context, Rect plotArea)
    {
        // Mask unselected areas
        var maskBrush = MaskBrush ?? new SolidColorBrush(Colors.Black, 0.35);
        var selLeft = plotArea.Left + Start * plotArea.Width;
        var selRight = plotArea.Left + End * plotArea.Width;

        // Left mask
        if (Start > 0)
        {
            var leftMask = new Rect(plotArea.Left, plotArea.Top, selLeft - plotArea.Left, plotArea.Height);
            context.DrawRectangle(maskBrush, null, leftMask);
        }
        // Right mask
        if (End < 1)
        {
            var rightMask = new Rect(selRight, plotArea.Top, plotArea.Right - selRight, plotArea.Height);
            context.DrawRectangle(maskBrush, null, rightMask);
        }
    }

    /// <summary>
    /// Render a miniature chart preview inside the zoom track.
    /// </summary>
    private void RenderMiniChart(DrawingContext context, Rect barRect, IList<ChartDataPoint> dataPoints)
    {
        if (dataPoints.Count < 2) return;

        double dataMin = double.MaxValue;
        double dataMax = double.MinValue;
        foreach (var pt in dataPoints)
        {
            if (pt.Y < dataMin) dataMin = pt.Y;
            if (pt.Y > dataMax) dataMax = pt.Y;
        }

        var dataRange = dataMax - dataMin;
        if (dataRange <= 0) dataRange = 1;

        var innerRect = new Rect(barRect.X + 2, barRect.Y + 2, barRect.Width - 4, barRect.Height - 4);
        var miniPath = new PathGeometry();
        var figure = new PathFigure { IsClosed = false };

        for (int i = 0; i < dataPoints.Count; i++)
        {
            var x = innerRect.Left + (i / (double)(dataPoints.Count - 1)) * innerRect.Width;
            var yNorm = (dataPoints[i].Y - dataMin) / dataRange;
            var y = innerRect.Bottom - yNorm * innerRect.Height;
            var pt = new Point(x, y);

            if (i == 0)
                figure.StartPoint = pt;
            else
                figure.Segments!.Add(new LineSegment { Point = pt });
        }

        miniPath.Figures!.Add(figure);
        var miniBrush = new SolidColorBrush(Color.Parse("#6750A4"), 0.4);
        context.DrawGeometry(null, new Pen(miniBrush, 1.0), miniPath);
    }

    /// <summary>
    /// Handle pointer pressed for zoom slider interaction.
    /// </summary>
    public bool HandlePointerPressed(Point position, Rect plotArea)
    {
        if (!IsVisible || ZoomLock) return false;

        var barRect = ZoomType == DataZoomType.Inside
            ? plotArea
            : new Rect(plotArea.Left, plotArea.Bottom + 8, plotArea.Width, Height);

        if (!barRect.Contains(position)) return false;

        var selLeft = barRect.Left + Start * barRect.Width;
        var selRight = barRect.Left + End * barRect.Width;
        var handleTolerance = 8.0;

        if (Math.Abs(position.X - selLeft) < handleTolerance)
        {
            _activeHandle = -1;
            _isDraggingHandle = true;
        }
        else if (Math.Abs(position.X - selRight) < handleTolerance)
        {
            _activeHandle = 1;
            _isDraggingHandle = true;
        }
        else if (position.X > selLeft && position.X < selRight)
        {
            _activeHandle = 0;
            _isDraggingBody = true;
        }
        else
        {
            return false;
        }

        _dragStart = position;
        _dragStartStart = Start;
        _dragStartEnd = End;
        return true;
    }

    /// <summary>
    /// Handle pointer moved for zoom slider dragging.
    /// </summary>
    public bool HandlePointerMoved(Point position, Rect plotArea)
    {
        if (!_isDraggingHandle && !_isDraggingBody) return false;

        var barWidth = plotArea.Width;
        var dx = (position.X - _dragStart.X) / barWidth;

        if (_isDraggingBody)
        {
            var span = _dragStartEnd - _dragStartStart;
            Start = Math.Clamp(_dragStartStart + dx, 0, 1 - span);
            End = Start + span;
        }
        else if (_activeHandle == -1)
        {
            Start = Math.Clamp(_dragStartStart + dx, 0, End - MinSpan);
        }
        else if (_activeHandle == 1)
        {
            End = Math.Clamp(_dragStartEnd + dx, Start + MinSpan, 1);
        }

        // Enforce min/max span
        var currentSpan = End - Start;
        if (currentSpan < MinSpan)
        {
            if (_activeHandle == -1) Start = End - MinSpan;
            else End = Start + MinSpan;
        }
        if (currentSpan > MaxSpan)
        {
            if (_activeHandle == -1) Start = End - MaxSpan;
            else End = Start + MaxSpan;
        }

        Start = Math.Clamp(Start, 0, 1);
        End = Math.Clamp(End, 0, 1);

        RangeChanged?.Invoke(Start, End);
        return true;
    }

    /// <summary>
    /// Handle pointer released to end dragging.
    /// </summary>
    public bool HandlePointerReleased()
    {
        if (!_isDraggingHandle && !_isDraggingBody) return false;
        _isDraggingHandle = false;
        _isDraggingBody = false;
        return true;
    }

    /// <summary>
    /// Get the effective axis range after applying the zoom selection.
    /// </summary>
    public (double Min, double Max) GetZoomedRange(double dataMin, double dataMax)
    {
        // Use explicit min/max if set
        if (!double.IsNaN(MinValue)) dataMin = MinValue;
        if (!double.IsNaN(MaxValue)) dataMax = MaxValue;

        var range = dataMax - dataMin;
        return (dataMin + Start * range, dataMin + End * range);
    }

    /// <summary>
    /// Set the zoom range from explicit data values.
    /// </summary>
    public void SetRangeFromData(double dataMin, double dataMax, double rangeMin, double rangeMax)
    {
        if (!double.IsNaN(MinValue)) dataMin = MinValue;
        if (!double.IsNaN(MaxValue)) dataMax = MaxValue;

        var totalRange = dataMax - dataMin;
        if (totalRange <= 0) return;

        Start = Math.Clamp((rangeMin - dataMin) / totalRange, 0, 1);
        End = Math.Clamp((rangeMax - dataMin) / totalRange, 0, 1);
    }
}

/// <summary>
/// Style configuration for data zoom drag handles.
/// </summary>
public class DataZoomHandleStyle
{
    /// <summary>Handle color.</summary>
    public Avalonia.Media.IBrush? Color { get; set; }

    /// <summary>Handle width in pixels.</summary>
    public double Width { get; set; } = 4;

    /// <summary>Handle border color.</summary>
    public Avalonia.Media.IBrush? BorderColor { get; set; }

    /// <summary>Handle border width.</summary>
    public double BorderWidth { get; set; }

    /// <summary>Handle opacity.</summary>
    public double Opacity { get; set; } = 1.0;
}

/// <summary>
/// Background configuration for the data preview area in a data zoom slider.
/// </summary>
public class DataZoomDataBackground
{
    /// <summary>Background color for the data preview area.</summary>
    public Avalonia.Media.IBrush? BackgroundColor { get; set; }

    /// <summary>Fill color for the mini chart area.</summary>
    public Avalonia.Media.IBrush? FillColor { get; set; }

    /// <summary>Stroke color for the mini chart line.</summary>
    public Avalonia.Media.IBrush? StrokeColor { get; set; }

    /// <summary>Stroke width for the mini chart line.</summary>
    public double StrokeWidth { get; set; } = 1.0;

    /// <summary>Fill opacity for the mini chart area.</summary>
    public double FillOpacity { get; set; } = 0.3;
}
