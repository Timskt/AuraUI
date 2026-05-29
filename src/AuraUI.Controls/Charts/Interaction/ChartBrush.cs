using Avalonia;
using Avalonia.Media;

namespace AuraUI.Controls.Charts.Interaction;

/// <summary>
/// Brush selection for chart data. Allows users to click and drag
/// to select an area, highlighting data points within the selection.
///
/// Supports multiple selection types:
///   - <see cref="BrushType.LineX"/>: vertical range selection (select by X range)
///   - <see cref="BrushType.LineY"/>: horizontal range selection (select by Y range)
///   - <see cref="BrushType.Rect"/>: rectangular area selection
///   - <see cref="BrushType.Polygon"/>: polygon area selection (rect for now)
///
/// Rendering:
///   - Semi-transparent highlight overlay for the selected area
///   - Dashed border around the selection
///   - Selected data points are visually emphasized
///
/// Events:
///   - <see cref="BrushSelectionChanged"/> fires when selection changes
/// </summary>
public class ChartBrush
{
    /// <summary>Whether brush selection is enabled.</summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// Type of brush selection.
    /// </summary>
    public BrushType BrushType { get; set; } = BrushType.Rect;

    /// <summary>Current selection rectangle (in pixel coordinates).</summary>
    public Rect? SelectionRect { get; private set; }

    /// <summary>Brush for the selection area fill.</summary>
    public IBrush? SelectionFill { get; set; }

    /// <summary>Brush for the selection area border.</summary>
    public IBrush? SelectionStroke { get; set; }

    /// <summary>Border thickness for the selection outline.</summary>
    public double BorderThickness { get; set; } = 1.0;

    /// <summary>Whether to use a dashed border for the selection.</summary>
    public bool DashedBorder { get; set; } = true;

    /// <summary>Whether a brush operation is currently active.</summary>
    public bool IsActive { get; private set; }

    /// <summary>Indices of data points within the selection.</summary>
    public IReadOnlyList<int> SelectedIndices => _selectedIndices;

    /// <summary>
    /// Raised when the brush selection changes.
    /// Provides the selected data range in data coordinates.
    /// </summary>
    public event EventHandler<BrushSelectionChangedEventArgs>? BrushSelectionChanged;

    private readonly List<int> _selectedIndices = new();
    private Point _brushStart;
    private Point _brushEnd;
    private bool _isBrushing;

    /// <summary>
    /// Whether a selection is currently active or visible.
    /// </summary>
    public bool HasSelection => IsActive && SelectionRect.HasValue;

    /// <summary>
    /// Get the selected data range converted from pixel to data coordinates.
    /// </summary>
    public (double XMin, double XMax, double YMin, double YMax) GetSelectionRange(ChartAxis xAxis, ChartAxis yAxis)
    {
        if (SelectionRect == null) return (0, 0, 0, 0);

        var rect = SelectionRect.Value;
        var xMin = xAxis.PixelToValue(rect.Left);
        var xMax = xAxis.PixelToValue(rect.Right);
        var yMin = yAxis.PixelToValue(rect.Bottom); // Bottom = min in data space
        var yMax = yAxis.PixelToValue(rect.Top);    // Top = max in data space
        return (Math.Min(xMin, xMax), Math.Max(xMin, xMax), Math.Min(yMin, yMax), Math.Max(yMin, yMax));
    }

    /// <summary>
    /// Render the brush selection overlay.
    /// </summary>
    public void Render(DrawingContext context, Rect plotArea)
    {
        if (!IsEnabled || !IsActive || SelectionRect == null) return;

        var rect = SelectionRect.Value;
        if (rect.Width < 2 || rect.Height < 2) return;

        var fill = SelectionFill ?? new SolidColorBrush(Color.Parse("#6750A4"), 0.1);
        var stroke = SelectionStroke ?? new SolidColorBrush(Color.Parse("#6750A4"), 0.8);

        Pen pen;
        if (DashedBorder)
        {
            pen = new Pen(stroke, BorderThickness, new DashStyle([4, 2], 0));
        }
        else
        {
            pen = new Pen(stroke, BorderThickness);
        }

        context.DrawRectangle(fill, pen, rect);
    }

    /// <summary>
    /// Handle pointer pressed to start brush selection.
    /// </summary>
    public bool HandlePointerPressed(Point position, Rect plotArea)
    {
        if (!IsEnabled) return false;
        if (!plotArea.Contains(position)) return false;

        _brushStart = position;
        _brushEnd = position;
        _isBrushing = true;
        SelectionRect = new Rect(position, new Size(0, 0));
        _selectedIndices.Clear();
        IsActive = true;
        return true;
    }

    /// <summary>
    /// Handle pointer moved to update brush selection.
    /// </summary>
    public bool HandlePointerMoved(Point position, Rect plotArea)
    {
        if (!_isBrushing || !IsEnabled) return false;

        // Clamp to plot area
        var clamped = new Point(
            Math.Clamp(position.X, plotArea.Left, plotArea.Right),
            Math.Clamp(position.Y, plotArea.Top, plotArea.Bottom));

        _brushEnd = clamped;

        var x = Math.Min(_brushStart.X, clamped.X);
        var y = Math.Min(_brushStart.Y, clamped.Y);
        var w = Math.Abs(clamped.X - _brushStart.X);
        var h = Math.Abs(clamped.Y - _brushStart.Y);

        // For LineX/LineY, extend the selection across the full range
        if (BrushType == BrushType.LineX)
        {
            y = plotArea.Top;
            h = plotArea.Height;
        }
        else if (BrushType == BrushType.LineY)
        {
            x = plotArea.Left;
            w = plotArea.Width;
        }

        SelectionRect = new Rect(x, y, w, h);
        return true;
    }

    /// <summary>
    /// Handle pointer released to finalize brush selection.
    /// </summary>
    public bool HandlePointerReleased()
    {
        if (!_isBrushing) return false;
        _isBrushing = false;

        // Fire selection changed event
        if (SelectionRect.HasValue)
        {
            var rect = SelectionRect.Value;
            BrushSelectionChanged?.Invoke(this, new BrushSelectionChangedEventArgs
            {
                XMin = rect.Left,
                XMax = rect.Right,
                YMin = rect.Top,
                YMax = rect.Bottom,
                BrushType = BrushType
            });
        }

        return true;
    }

    /// <summary>
    /// Find all data points within the current selection.
    /// </summary>
    public List<BrushSelectedPoint> FindSelectedPoints(
        IReadOnlyList<ChartSeries> series,
        Rect plotArea,
        ChartAxis xAxis,
        ChartAxis yAxis)
    {
        var result = new List<BrushSelectedPoint>();
        if (!HasSelection || SelectionRect == null) return result;

        var selRect = SelectionRect.Value;

        foreach (var s in series)
        {
            if (!s.IsVisible) continue;
            if (s is not XYChartSeries xy) continue;

            for (int i = 0; i < xy.DataPoints.Count; i++)
            {
                var dp = xy.DataPoints[i];
                var px = xAxis.ValueToPixel(dp.X);
                var py = yAxis.ValueToPixel(dp.Y);
                var pixelPoint = new Point(px, py);

                bool inside = BrushType switch
                {
                    BrushType.LineX => px >= selRect.Left && px <= selRect.Right,
                    BrushType.LineY => py >= selRect.Top && py <= selRect.Bottom,
                    BrushType.Rect => selRect.Contains(pixelPoint),
                    BrushType.Polygon => selRect.Contains(pixelPoint),
                    _ => false
                };

                if (inside)
                {
                    result.Add(new BrushSelectedPoint
                    {
                        Series = s,
                        DataPoint = dp,
                        DataIndex = i,
                        PixelPosition = pixelPoint
                    });
                }
            }
        }

        return result;
    }

    /// <summary>
    /// Test which data points fall within the current selection.
    /// Updates the SelectedIndices list.
    /// </summary>
    public void UpdateSelection(IReadOnlyList<Point> dataPointPositions)
    {
        _selectedIndices.Clear();
        if (SelectionRect == null) return;

        var rect = SelectionRect.Value;
        for (int i = 0; i < dataPointPositions.Count; i++)
        {
            if (rect.Contains(dataPointPositions[i]))
                _selectedIndices.Add(i);
        }
    }

    /// <summary>
    /// Clear the current brush selection.
    /// </summary>
    public void Clear()
    {
        SelectionRect = null;
        IsActive = false;
        _selectedIndices.Clear();
        _isBrushing = false;
    }
}

/// <summary>
/// Event args for brush selection changes.
/// </summary>
public class BrushSelectionChangedEventArgs : EventArgs
{
    /// <summary>Minimum X value of the selection in pixel/data coordinates.</summary>
    public double XMin { get; init; }

    /// <summary>Maximum X value of the selection in pixel/data coordinates.</summary>
    public double XMax { get; init; }

    /// <summary>Minimum Y value of the selection in pixel/data coordinates.</summary>
    public double YMin { get; init; }

    /// <summary>Maximum Y value of the selection in pixel/data coordinates.</summary>
    public double YMax { get; init; }

    /// <summary>The brush type used for this selection.</summary>
    public BrushType BrushType { get; init; }
}

/// <summary>
/// A data point within the brush selection.
/// </summary>
public class BrushSelectedPoint
{
    public required ChartSeries Series { get; init; }
    public required ChartDataPoint DataPoint { get; init; }
    public required int DataIndex { get; init; }
    public required Point PixelPosition { get; init; }
}
