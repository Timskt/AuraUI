using Avalonia;
using Avalonia.Media;

namespace AuraUI.Controls.Charts.Interaction;

/// <summary>
/// A toolbox control for charts providing quick-access buttons for common
/// operations: save as image, data view, zoom in/out, and reset.
///
/// Rendering:
///   - Small icon buttons rendered in the top-right corner of the chart
///   - Buttons drawn via DrawingContext (not as child controls)
/// </summary>
public class ChartToolbox
{
    /// <summary>Whether the toolbox is visible.</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>Size of each toolbox button in pixels.</summary>
    public double ButtonSize { get; set; } = 24;

    /// <summary>Gap between buttons.</summary>
    public double Gap { get; set; } = 4;

    /// <summary>Whether to show the save-as-image button.</summary>
    public bool ShowSaveImage { get; set; } = true;

    /// <summary>Whether to show the data view button.</summary>
    public bool ShowDataView { get; set; } = true;

    /// <summary>Whether to show the zoom-in button.</summary>
    public bool ShowZoomIn { get; set; } = true;

    /// <summary>Whether to show the zoom-out button.</summary>
    public bool ShowZoomOut { get; set; } = true;

    /// <summary>Whether to show the reset button.</summary>
    public bool ShowReset { get; set; } = true;

    /// <summary>Background brush for buttons.</summary>
    public IBrush? ButtonBackground { get; set; }

    /// <summary>Foreground brush for button icons.</summary>
    public IBrush? ButtonForeground { get; set; }

    /// <summary>Pixel rect allocated to the toolbox.</summary>
    internal Rect LayoutRect { get; set; }

    /// <summary>Event raised when a toolbox button is clicked.</summary>
    public event EventHandler<ToolboxActionEventArgs>? ActionClicked;

    private readonly List<Rect> _buttonRects = new();

    /// <summary>
    /// Render the toolbox buttons.
    /// </summary>
    public void Render(DrawingContext context, Rect chartBounds)
    {
        if (!IsVisible) return;

        _buttonRects.Clear();

        var buttons = new List<ToolboxButton>();
        if (ShowSaveImage) buttons.Add(ToolboxButton.SaveImage);
        if (ShowDataView) buttons.Add(ToolboxButton.DataView);
        if (ShowZoomIn) buttons.Add(ToolboxButton.ZoomIn);
        if (ShowZoomOut) buttons.Add(ToolboxButton.ZoomOut);
        if (ShowReset) buttons.Add(ToolboxButton.Reset);

        if (buttons.Count == 0) return;

        var totalWidth = buttons.Count * ButtonSize + (buttons.Count - 1) * Gap;
        var x = chartBounds.Right - totalWidth - 8;
        var y = chartBounds.Top + 8;

        LayoutRect = new Rect(x, y, totalWidth, ButtonSize);

        var bg = ButtonBackground ?? new SolidColorBrush(Colors.White, 0.9);
        var fg = ButtonForeground ?? Brushes.DarkGray;
        var borderPen = new Pen(new SolidColorBrush(Colors.LightGray), 0.5);

        foreach (var button in buttons)
        {
            var btnRect = new Rect(x, y, ButtonSize, ButtonSize);
            _buttonRects.Add(btnRect);

            // Button background
            context.DrawRectangle(bg, borderPen, btnRect, 4, 4);

            // Draw icon
            DrawButtonIcon(context, button, btnRect, fg);

            x += ButtonSize + Gap;
        }
    }

    /// <summary>
    /// Handle pointer pressed to check if a button was clicked.
    /// </summary>
    public bool HandlePointerPressed(Point position)
    {
        if (!IsVisible) return false;

        var buttons = new List<ToolboxButton>();
        if (ShowSaveImage) buttons.Add(ToolboxButton.SaveImage);
        if (ShowDataView) buttons.Add(ToolboxButton.DataView);
        if (ShowZoomIn) buttons.Add(ToolboxButton.ZoomIn);
        if (ShowZoomOut) buttons.Add(ToolboxButton.ZoomOut);
        if (ShowReset) buttons.Add(ToolboxButton.Reset);

        for (int i = 0; i < _buttonRects.Count && i < buttons.Count; i++)
        {
            if (_buttonRects[i].Contains(position))
            {
                ActionClicked?.Invoke(this, new ToolboxActionEventArgs(buttons[i]));
                return true;
            }
        }

        return false;
    }

    private static void DrawButtonIcon(DrawingContext context, ToolboxButton button, Rect rect, IBrush brush)
    {
        var cx = rect.Center.X;
        var cy = rect.Center.Y;
        var s = rect.Width * 0.3;
        var pen = new Pen(brush, 1.5, lineCap: PenLineCap.Round);

        switch (button)
        {
            case ToolboxButton.SaveImage:
                // Disk icon
                var diskRect = new Rect(cx - s, cy - s, s * 2, s * 2);
                context.DrawRectangle(null, pen, diskRect, 2, 2);
                context.DrawLine(pen, new Point(cx - s * 0.5, cy - s * 0.3), new Point(cx + s * 0.5, cy - s * 0.3));
                context.DrawLine(pen, new Point(cx - s * 0.5, cy + s * 0.3), new Point(cx + s * 0.5, cy + s * 0.3));
                break;

            case ToolboxButton.DataView:
                // Table icon
                context.DrawRectangle(null, pen, new Rect(cx - s, cy - s, s * 2, s * 2), 2, 2);
                context.DrawLine(pen, new Point(cx, cy - s), new Point(cx, cy + s));
                context.DrawLine(pen, new Point(cx - s, cy), new Point(cx + s, cy));
                break;

            case ToolboxButton.ZoomIn:
                // Magnifier + icon
                context.DrawEllipse(null, pen, new Point(cx - s * 0.2, cy - s * 0.2), s * 0.7, s * 0.7);
                context.DrawLine(pen, new Point(cx + s * 0.3, cy + s * 0.3), new Point(cx + s * 0.8, cy + s * 0.8));
                context.DrawLine(pen, new Point(cx - s * 0.3, cy - s * 0.2), new Point(cx + s * 0.3, cy - s * 0.2));
                context.DrawLine(pen, new Point(cx - s * 0.0, cy - s * 0.5), new Point(cx - s * 0.0, cy + s * 0.1));
                break;

            case ToolboxButton.ZoomOut:
                // Magnifier - icon
                context.DrawEllipse(null, pen, new Point(cx - s * 0.2, cy - s * 0.2), s * 0.7, s * 0.7);
                context.DrawLine(pen, new Point(cx + s * 0.3, cy + s * 0.3), new Point(cx + s * 0.8, cy + s * 0.8));
                context.DrawLine(pen, new Point(cx - s * 0.3, cy - s * 0.2), new Point(cx + s * 0.3, cy - s * 0.2));
                break;

            case ToolboxButton.Reset:
                // Circular arrow icon
                context.DrawEllipse(null, pen, new Point(cx, cy), s * 0.7, s * 0.7);
                // Arrow head
                var arrowPt = new Point(cx + s * 0.7, cy);
                context.DrawLine(pen, arrowPt, new Point(arrowPt.X - s * 0.3, arrowPt.Y - s * 0.3));
                context.DrawLine(pen, arrowPt, new Point(arrowPt.X - s * 0.3, arrowPt.Y + s * 0.3));
                break;
        }
    }
}

/// <summary>
/// Types of toolbox buttons.
/// </summary>
public enum ToolboxButton
{
    SaveImage,
    DataView,
    ZoomIn,
    ZoomOut,
    Reset
}

/// <summary>
/// Event args for toolbox button clicks.
/// </summary>
public class ToolboxActionEventArgs : EventArgs
{
    public ToolboxButton Button { get; }

    public ToolboxActionEventArgs(ToolboxButton button)
    {
        Button = button;
    }
}
