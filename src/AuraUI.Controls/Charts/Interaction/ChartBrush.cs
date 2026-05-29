using Avalonia;
using Avalonia.Media;

namespace AuraUI.Controls.Charts.Interaction;

/// <summary>
/// Brush selection for filtering chart data. Allows users to draw a rectangular
/// or lasso selection area to highlight and filter data points.
///
/// Supports:
///   - Rectangular selection (drag to draw a box)
///   - Selected points are highlighted, others dimmed
///   - Selection can be cleared by clicking outside
/// </summary>
public class ChartBrush
{
    /// <summary>Whether brush selection is enabled.</summary>
    public bool IsEnabled { get; set; }

    /// <summary>Current selection rectangle (in pixel coordinates).</summary>
    public Rect? SelectionRect { get; private set; }

    /// <summary>Brush for the selection area fill.</summary>
    public IBrush? SelectionFill { get; set; }

    /// <summary>Brush for the selection area border.</summary>
    public IBrush? SelectionStroke { get; set; }

    /// <summary>Whether a brush operation is currently active.</summary>
    public bool IsActive { get; private set; }

    /// <summary>Indices of data points within the selection.</summary>
    public IReadOnlyList<int> SelectedIndices => _selectedIndices;

    private readonly List<int> _selectedIndices = new();
    private Point _brushStart;
    private bool _isBrushing;

    /// <summary>
    /// Render the brush selection overlay.
    /// </summary>
    public void Render(DrawingContext context)
    {
        if (!IsEnabled || !IsActive || SelectionRect == null) return;

        var rect = SelectionRect.Value;
        if (rect.Width < 2 || rect.Height < 2) return;

        var fill = SelectionFill ?? new SolidColorBrush(Color.Parse("#6750A4"), 0.1);
        var stroke = SelectionStroke ?? new SolidColorBrush(Color.Parse("#6750A4"), 0.8);
        var pen = new Pen(stroke, 1, new DashStyle([4, 2], 0));

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

        var x = Math.Min(_brushStart.X, position.X);
        var y = Math.Min(_brushStart.Y, position.Y);
        var w = Math.Abs(position.X - _brushStart.X);
        var h = Math.Abs(position.Y - _brushStart.Y);

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
        return true;
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
