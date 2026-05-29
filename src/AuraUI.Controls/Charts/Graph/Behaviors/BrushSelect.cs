using Avalonia;
using Avalonia.Media;

namespace AuraUI.Controls.Charts.Graph.Behaviors;

/// <summary>
/// Behavior for area/brush selection of graph nodes.
/// Draws a selection overlay and selects all nodes within the brush area.
/// </summary>
public class BrushSelect
{
    /// <summary>Type of brush shape.</summary>
    public GraphBrushType BrushType { get; set; } = GraphBrushType.Rect;

    /// <summary>Fill color of the brush area.</summary>
    public IBrush? BrushFill { get; set; }

    /// <summary>Stroke color of the brush area.</summary>
    public IBrush? BrushStroke { get; set; }

    /// <summary>Whether brush selection is enabled.</summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>Whether a brush operation is currently active.</summary>
    public bool IsActive { get; private set; }

    /// <summary>Start point of the brush in screen coordinates.</summary>
    private Point _startPoint;

    /// <summary>Current end point of the brush in screen coordinates.</summary>
    private Point _currentPoint;

    /// <summary>Raised when brush selection completes.</summary>
    public event EventHandler? BrushCompleted;

    /// <summary>
    /// Handles pointer pressed to begin brush selection.
    /// </summary>
    /// <returns>True if brush was started.</returns>
    public bool HandlePointerPressed(Point pointerPos)
    {
        if (!IsEnabled) return false;
        IsActive = true;
        _startPoint = pointerPos;
        _currentPoint = pointerPos;
        return true;
    }

    /// <summary>
    /// Handles pointer moved to update brush area.
    /// </summary>
    public void HandlePointerMoved(Point pointerPos)
    {
        if (!IsActive) return;
        _currentPoint = pointerPos;
    }

    /// <summary>
    /// Handles pointer released. Selects nodes within the brush area.
    /// </summary>
    public void HandlePointerReleased(
        IList<GraphNode> nodes,
        Func<GraphNode, Point> getWorldPos)
    {
        if (!IsActive) return;
        IsActive = false;

        var rect = GetBrushRect();

        foreach (var node in nodes)
        {
            var worldPos = getWorldPos(node);
            if (IsInsideBrush(worldPos, rect))
            {
                node.IsSelected = true;
                node.State = GraphElementState.Selected;
            }
        }

        BrushCompleted?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Renders the brush overlay on the drawing context.
    /// </summary>
    public void Render(DrawingContext context)
    {
        if (!IsActive) return;

        var fill = BrushFill ?? new SolidColorBrush(Colors.DodgerBlue, 0.1);
        var stroke = BrushStroke ?? new SolidColorBrush(Colors.DodgerBlue, 0.5);
        var pen = new Pen(stroke, 1, new DashStyle(new double[] { 4, 2 }, 0));

        switch (BrushType)
        {
            case GraphBrushType.Rect:
                var rect = GetBrushRect();
                context.DrawRectangle(fill, pen, rect);
                break;

            case GraphBrushType.Circle:
                var center = new Point(
                    (_startPoint.X + _currentPoint.X) / 2,
                    (_startPoint.Y + _currentPoint.Y) / 2);
                var rx = Math.Abs(_currentPoint.X - _startPoint.X) / 2;
                var ry = Math.Abs(_currentPoint.Y - _startPoint.Y) / 2;
                context.DrawEllipse(fill, pen, center, rx, ry);
                break;

            case GraphBrushType.Polygon:
                // Simple diamond shape for polygon brush
                var rect2 = GetBrushRect();
                var geometry = new StreamGeometry();
                using (var ctx = geometry.Open())
                {
                    ctx.BeginFigure(new Point(rect2.Center.X, rect2.Top), true);
                    ctx.LineTo(new Point(rect2.Right, rect2.Center.Y));
                    ctx.LineTo(new Point(rect2.Center.X, rect2.Bottom));
                    ctx.LineTo(new Point(rect2.Left, rect2.Center.Y));
                    ctx.EndFigure(true);
                }
                context.DrawGeometry(fill, pen, geometry);
                break;
        }
    }

    /// <summary>Resets the brush state.</summary>
    public void Reset()
    {
        IsActive = false;
    }

    private Rect GetBrushRect()
    {
        var x = Math.Min(_startPoint.X, _currentPoint.X);
        var y = Math.Min(_startPoint.Y, _currentPoint.Y);
        var w = Math.Abs(_currentPoint.X - _startPoint.X);
        var h = Math.Abs(_currentPoint.Y - _startPoint.Y);
        return new Rect(x, y, w, h);
    }

    private bool IsInsideBrush(Point point, Rect rect)
    {
        return BrushType switch
        {
            GraphBrushType.Rect => rect.Contains(point),
            GraphBrushType.Circle => IsInsideEllipse(point, rect),
            GraphBrushType.Polygon => IsInsideDiamond(point, rect),
            _ => false
        };
    }

    private static bool IsInsideEllipse(Point point, Rect rect)
    {
        var cx = rect.Center.X;
        var cy = rect.Center.Y;
        var rx = rect.Width / 2;
        var ry = rect.Height / 2;
        if (rx < 1 || ry < 1) return false;
        var dx = (point.X - cx) / rx;
        var dy = (point.Y - cy) / ry;
        return dx * dx + dy * dy <= 1;
    }

    private static bool IsInsideDiamond(Point point, Rect rect)
    {
        var cx = rect.Center.X;
        var cy = rect.Center.Y;
        var rx = rect.Width / 2;
        var ry = rect.Height / 2;
        if (rx < 1 || ry < 1) return false;
        return Math.Abs(point.X - cx) / rx + Math.Abs(point.Y - cy) / ry <= 1;
    }
}
