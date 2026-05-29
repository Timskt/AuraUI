using Avalonia;
using Avalonia.Media;

namespace AuraUI.Controls.Charts;

/// <summary>
/// A toolbar rendered in the top-right corner of the chart, providing quick-access
/// buttons for common operations: save as image, view data table, zoom controls, etc.
///
/// Rendering:
///   - Icon buttons drawn via DrawingContext (no child controls)
///   - Buttons show tooltips on hover
///   - Buttons highlight on hover/press
///
/// The toolbox is fully configurable -- each button can be shown/hidden independently.
/// </summary>
public class ChartToolbox : AvaloniaObject
{
    /// <summary>Whether the toolbox is visible.</summary>
    public static readonly StyledProperty<bool> IsVisibleProperty =
        AvaloniaProperty.Register<ChartToolbox, bool>(nameof(IsVisible), true);

    /// <summary>Show the "Save as PNG" button.</summary>
    public static readonly StyledProperty<bool> ShowSaveAsImageProperty =
        AvaloniaProperty.Register<ChartToolbox, bool>(nameof(ShowSaveAsImage), true);

    /// <summary>Show the "Data View" button (shows raw data table).</summary>
    public static readonly StyledProperty<bool> ShowDataViewProperty =
        AvaloniaProperty.Register<ChartToolbox, bool>(nameof(ShowDataView), true);

    /// <summary>Show the "Zoom In" and "Zoom Out" buttons.</summary>
    public static readonly StyledProperty<bool> ShowZoomProperty =
        AvaloniaProperty.Register<ChartToolbox, bool>(nameof(ShowZoom), true);

    /// <summary>Show the "Reset Zoom" button.</summary>
    public static readonly StyledProperty<bool> ShowResetProperty =
        AvaloniaProperty.Register<ChartToolbox, bool>(nameof(ShowReset), true);

    /// <summary>Show the "Restore" button (reset all changes including data zoom, brush, etc.).</summary>
    public static readonly StyledProperty<bool> ShowRestoreProperty =
        AvaloniaProperty.Register<ChartToolbox, bool>(nameof(ShowRestore));

    /// <summary>Show the "Data Zoom" button (enable zoom via toolbox).</summary>
    public static readonly StyledProperty<bool> ShowDataZoomProperty =
        AvaloniaProperty.Register<ChartToolbox, bool>(nameof(ShowDataZoom));

    /// <summary>Show the "Magic Type" button (switch between line/bar/stack).</summary>
    public static readonly StyledProperty<bool> ShowMagicTypeProperty =
        AvaloniaProperty.Register<ChartToolbox, bool>(nameof(ShowMagicType));

    /// <summary>Show the "Brush" button (enable brush selection).</summary>
    public static readonly StyledProperty<bool> ShowBrushProperty =
        AvaloniaProperty.Register<ChartToolbox, bool>(nameof(ShowBrush));

    /// <summary>Image format for the save-as-image feature.</summary>
    public static readonly StyledProperty<SaveAsImageFormat> SaveFormatProperty =
        AvaloniaProperty.Register<ChartToolbox, SaveAsImageFormat>(nameof(SaveFormat), SaveAsImageFormat.Png);

    /// <summary>Pixel ratio for saved images (2 = retina).</summary>
    public static readonly StyledProperty<double> SavePixelRatioProperty =
        AvaloniaProperty.Register<ChartToolbox, double>(nameof(SavePixelRatio), 2.0);

    /// <summary>Whether the data view is read-only.</summary>
    public static readonly StyledProperty<bool> DataViewReadOnlyProperty =
        AvaloniaProperty.Register<ChartToolbox, bool>(nameof(DataViewReadOnly), true);

    /// <summary>The currently active magic chart type.</summary>
    public static readonly StyledProperty<MagicChartType> ActiveMagicTypeProperty =
        AvaloniaProperty.Register<ChartToolbox, MagicChartType>(nameof(ActiveMagicType), MagicChartType.Line);

    /// <summary>Size of each toolbar button in pixels.</summary>
    public static readonly StyledProperty<double> ButtonSizeProperty =
        AvaloniaProperty.Register<ChartToolbox, double>(nameof(ButtonSize), 24.0);

    /// <summary>Spacing between toolbar buttons.</summary>
    public static readonly StyledProperty<double> ButtonSpacingProperty =
        AvaloniaProperty.Register<ChartToolbox, double>(nameof(ButtonSpacing), 4.0);

    /// <summary>Icon color for the toolbar buttons.</summary>
    public static readonly StyledProperty<IBrush?> IconBrushProperty =
        AvaloniaProperty.Register<ChartToolbox, IBrush?>(nameof(IconBrush));

    /// <summary>Background brush for hovered button.</summary>
    public static readonly StyledProperty<IBrush?> HoverBrushProperty =
        AvaloniaProperty.Register<ChartToolbox, IBrush?>(nameof(HoverBrush));

    /// <summary>Background brush for pressed button.</summary>
    public static readonly StyledProperty<IBrush?> PressedBrushProperty =
        AvaloniaProperty.Register<ChartToolbox, IBrush?>(nameof(PressedBrush));

    /// <summary>Border radius for toolbar buttons.</summary>
    public static readonly StyledProperty<double> ButtonCornerRadiusProperty =
        AvaloniaProperty.Register<ChartToolbox, double>(nameof(ButtonCornerRadius), 4.0);

    // CLR wrappers
    public bool IsVisible { get => GetValue(IsVisibleProperty); set => SetValue(IsVisibleProperty, value); }
    public bool ShowSaveAsImage { get => GetValue(ShowSaveAsImageProperty); set => SetValue(ShowSaveAsImageProperty, value); }
    public bool ShowDataView { get => GetValue(ShowDataViewProperty); set => SetValue(ShowDataViewProperty, value); }
    public bool ShowZoom { get => GetValue(ShowZoomProperty); set => SetValue(ShowZoomProperty, value); }
    public bool ShowReset { get => GetValue(ShowResetProperty); set => SetValue(ShowResetProperty, value); }
    public double ButtonSize { get => GetValue(ButtonSizeProperty); set => SetValue(ButtonSizeProperty, value); }
    public double ButtonSpacing { get => GetValue(ButtonSpacingProperty); set => SetValue(ButtonSpacingProperty, value); }
    public IBrush? IconBrush { get => GetValue(IconBrushProperty); set => SetValue(IconBrushProperty, value); }
    public IBrush? HoverBrush { get => GetValue(HoverBrushProperty); set => SetValue(HoverBrushProperty, value); }
    public IBrush? PressedBrush { get => GetValue(PressedBrushProperty); set => SetValue(PressedBrushProperty, value); }
    public double ButtonCornerRadius { get => GetValue(ButtonCornerRadiusProperty); set => SetValue(ButtonCornerRadiusProperty, value); }
    public bool ShowRestore { get => GetValue(ShowRestoreProperty); set => SetValue(ShowRestoreProperty, value); }
    public bool ShowDataZoom { get => GetValue(ShowDataZoomProperty); set => SetValue(ShowDataZoomProperty, value); }
    public bool ShowMagicType { get => GetValue(ShowMagicTypeProperty); set => SetValue(ShowMagicTypeProperty, value); }
    public bool ShowBrush { get => GetValue(ShowBrushProperty); set => SetValue(ShowBrushProperty, value); }
    public SaveAsImageFormat SaveFormat { get => GetValue(SaveFormatProperty); set => SetValue(SaveFormatProperty, value); }
    public double SavePixelRatio { get => GetValue(SavePixelRatioProperty); set => SetValue(SavePixelRatioProperty, value); }
    public bool DataViewReadOnly { get => GetValue(DataViewReadOnlyProperty); set => SetValue(DataViewReadOnlyProperty, value); }
    public MagicChartType ActiveMagicType { get => GetValue(ActiveMagicTypeProperty); set => SetValue(ActiveMagicTypeProperty, value); }

    /// <summary>
    /// Raised when the "Save as Image" button is clicked.
    /// The chart should handle the actual save logic.
    /// </summary>
    public event Action? SaveAsImageRequested;

    /// <summary>
    /// Raised when the "Data View" button is clicked.
    /// The chart should display a data table.
    /// </summary>
    public event Action? DataViewRequested;

    /// <summary>
    /// Raised when the "Zoom In" button is clicked.
    /// </summary>
    public event Action? ZoomInRequested;

    /// <summary>
    /// Raised when the "Zoom Out" button is clicked.
    /// </summary>
    public event Action? ZoomOutRequested;

    /// <summary>
    /// Raised when the "Reset Zoom" button is clicked.
    /// </summary>
    public event Action? ResetZoomRequested;

    /// <summary>
    /// Raised when the "Restore" button is clicked.
    /// Should reset all chart state (zoom, brush, data transforms).
    /// </summary>
    public event Action? RestoreRequested;

    /// <summary>
    /// Raised when the "Data Zoom" button is clicked.
    /// Should enable/disable the data zoom control.
    /// </summary>
    public event Action? DataZoomRequested;

    /// <summary>
    /// Raised when the "Magic Type" button is clicked.
    /// Cycles through line/bar/stack chart types.
    /// </summary>
    public event Action<MagicChartType>? MagicTypeRequested;

    /// <summary>
    /// Raised when the "Brush" button is clicked.
    /// Should enable/disable brush selection mode.
    /// </summary>
    public event Action? BrushRequested;

    // Internal state for hover tracking
    private int _hoveredButtonIndex = -1;
    private int _pressedButtonIndex = -1;
    private Rect[] _buttonRects = Array.Empty<Rect>();

    /// <summary>
    /// Render the toolbox in the top-right corner of the chart.
    /// </summary>
    /// <param name="context">Drawing context.</param>
    /// <param name="chartBounds">The full chart bounds.</param>
    public void Render(DrawingContext context, Rect chartBounds)
    {
        if (!IsVisible) return;

        var buttons = GetVisibleButtons();
        if (buttons.Length == 0) return;

        var btnSize = ButtonSize;
        var spacing = ButtonSpacing;
        var totalWidth = buttons.Length * btnSize + (buttons.Length - 1) * spacing;

        // Position in top-right corner with some padding
        var startX = chartBounds.Right - totalWidth - 8;
        var startY = chartBounds.Top + 8;

        _buttonRects = new Rect[buttons.Length];

        for (int i = 0; i < buttons.Length; i++)
        {
            var rect = new Rect(startX + i * (btnSize + spacing), startY, btnSize, btnSize);
            _buttonRects[i] = rect;

            var iconColor = IconBrush
                ?? TryFindResource<IBrush>("AuraForegroundSecondaryBrush")
                ?? Brushes.Gray;

            // Hover/press background
            if (i == _pressedButtonIndex)
            {
                var pressedBg = PressedBrush
                    ?? TryFindResource<IBrush>("AuraSubtleBrush")
                    ?? new SolidColorBrush(Colors.Gray, 0.2);
                context.DrawRectangle(pressedBg, null, rect, ButtonCornerRadius, ButtonCornerRadius);
            }
            else if (i == _hoveredButtonIndex)
            {
                var hoverBg = HoverBrush
                    ?? TryFindResource<IBrush>("AuraSubtleBrush")
                    ?? new SolidColorBrush(Colors.Gray, 0.1);
                context.DrawRectangle(hoverBg, null, rect, ButtonCornerRadius, ButtonCornerRadius);
            }

            // Draw icon
            DrawIcon(context, buttons[i], rect, iconColor);
        }
    }

    /// <summary>
    /// Handle pointer moved to track hover state.
    /// </summary>
    /// <returns>True if the toolbox handled the event (pointer is over a button).</returns>
    public bool HandlePointerMoved(Point position, Rect chartBounds)
    {
        if (!IsVisible) return false;

        var oldHovered = _hoveredButtonIndex;
        _hoveredButtonIndex = -1;

        for (int i = 0; i < _buttonRects.Length; i++)
        {
            if (_buttonRects[i].Contains(position))
            {
                _hoveredButtonIndex = i;
                break;
            }
        }

        return _hoveredButtonIndex != oldHovered;
    }

    /// <summary>
    /// Handle pointer pressed to track press state.
    /// </summary>
    /// <returns>True if a button was pressed.</returns>
    public bool HandlePointerPressed(Point position, Rect chartBounds)
    {
        if (!IsVisible) return false;

        for (int i = 0; i < _buttonRects.Length; i++)
        {
            if (_buttonRects[i].Contains(position))
            {
                _pressedButtonIndex = i;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Handle pointer released to trigger button action.
    /// </summary>
    /// <returns>True if a button action was triggered.</returns>
    public bool HandlePointerReleased(Point position, Rect chartBounds)
    {
        if (!IsVisible || _pressedButtonIndex < 0) return false;

        var releasedIndex = _pressedButtonIndex;
        _pressedButtonIndex = -1;

        if (releasedIndex >= 0 && releasedIndex < _buttonRects.Length &&
            _buttonRects[releasedIndex].Contains(position))
        {
            var buttons = GetVisibleButtons();
            if (releasedIndex < buttons.Length)
            {
                TriggerAction(buttons[releasedIndex]);
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Get the tooltip text for the button under the pointer.
    /// </summary>
    public string? GetTooltip(Point position)
    {
        if (!IsVisible) return null;

        for (int i = 0; i < _buttonRects.Length; i++)
        {
            if (_buttonRects[i].Contains(position))
            {
                var buttons = GetVisibleButtons();
                if (i < buttons.Length)
                {
                    return buttons[i] switch
                    {
                        ToolboxButton.SaveAsImage => $"Save as {SaveFormat}",
                        ToolboxButton.DataView => DataViewReadOnly ? "View Data" : "Edit Data",
                        ToolboxButton.ZoomIn => "Zoom In",
                        ToolboxButton.ZoomOut => "Zoom Out",
                        ToolboxButton.ResetZoom => "Reset Zoom",
                        ToolboxButton.Restore => "Restore",
                        ToolboxButton.DataZoom => "Data Zoom",
                        ToolboxButton.MagicType => $"Switch to {GetNextMagicType()}",
                        ToolboxButton.Brush => "Brush Selection",
                        _ => null
                    };
                }
            }
        }

        return null;
    }

    // ────────────────────────────────────────────────
    //  Private helpers
    // ────────────────────────────────────────────────

    private ToolboxButton[] GetVisibleButtons()
    {
        var buttons = new List<ToolboxButton>();
        if (ShowSaveAsImage) buttons.Add(ToolboxButton.SaveAsImage);
        if (ShowDataView) buttons.Add(ToolboxButton.DataView);
        if (ShowZoom) { buttons.Add(ToolboxButton.ZoomIn); buttons.Add(ToolboxButton.ZoomOut); }
        if (ShowReset) buttons.Add(ToolboxButton.ResetZoom);
        if (ShowRestore) buttons.Add(ToolboxButton.Restore);
        if (ShowDataZoom) buttons.Add(ToolboxButton.DataZoom);
        if (ShowMagicType) buttons.Add(ToolboxButton.MagicType);
        if (ShowBrush) buttons.Add(ToolboxButton.Brush);
        return buttons.ToArray();
    }

    private void TriggerAction(ToolboxButton button)
    {
        switch (button)
        {
            case ToolboxButton.SaveAsImage:
                SaveAsImageRequested?.Invoke();
                break;
            case ToolboxButton.DataView:
                DataViewRequested?.Invoke();
                break;
            case ToolboxButton.ZoomIn:
                ZoomInRequested?.Invoke();
                break;
            case ToolboxButton.ZoomOut:
                ZoomOutRequested?.Invoke();
                break;
            case ToolboxButton.ResetZoom:
                ResetZoomRequested?.Invoke();
                break;
            case ToolboxButton.Restore:
                RestoreRequested?.Invoke();
                break;
            case ToolboxButton.DataZoom:
                DataZoomRequested?.Invoke();
                break;
            case ToolboxButton.MagicType:
                CycleMagicType();
                break;
            case ToolboxButton.Brush:
                BrushRequested?.Invoke();
                break;
        }
    }

    private MagicChartType GetNextMagicType()
    {
        return ActiveMagicType switch
        {
            MagicChartType.Line => MagicChartType.Bar,
            MagicChartType.Bar => MagicChartType.Stack,
            MagicChartType.Stack => MagicChartType.Line,
            _ => MagicChartType.Line
        };
    }

    private void CycleMagicType()
    {
        ActiveMagicType = GetNextMagicType();
        MagicTypeRequested?.Invoke(ActiveMagicType);
    }

    /// <summary>
    /// Draw a simple icon for the given button type.
    /// Icons are drawn as geometric shapes (no font/icon dependency).
    /// </summary>
    private static void DrawIcon(DrawingContext context, ToolboxButton button, Rect rect, IBrush color)
    {
        var pen = new Pen(color, 1.5);
        var cx = rect.Center.X;
        var cy = rect.Center.Y;
        var s = rect.Width * 0.3; // icon half-size

        switch (button)
        {
            case ToolboxButton.SaveAsImage:
                // Floppy disk / download icon: box with downward arrow
                var boxRect = new Rect(cx - s, cy - s * 0.6, s * 2, s * 1.6);
                context.DrawRectangle(null, pen, boxRect, 2, 2);
                // Arrow down
                context.DrawLine(pen, new Point(cx, cy - s * 0.3), new Point(cx, cy + s * 0.5));
                context.DrawLine(pen, new Point(cx - s * 0.4, cy + s * 0.1), new Point(cx, cy + s * 0.5));
                context.DrawLine(pen, new Point(cx + s * 0.4, cy + s * 0.1), new Point(cx, cy + s * 0.5));
                break;

            case ToolboxButton.DataView:
                // Table icon: grid lines
                var tableRect = new Rect(cx - s, cy - s * 0.7, s * 2, s * 1.4);
                context.DrawRectangle(null, pen, tableRect, 2, 2);
                // Horizontal line
                context.DrawLine(pen, new Point(cx - s, cy), new Point(cx + s, cy));
                // Vertical line
                context.DrawLine(pen, new Point(cx, cy - s * 0.7), new Point(cx, cy + s * 0.7));
                break;

            case ToolboxButton.ZoomIn:
                // Magnifying glass with +
                context.DrawEllipse(null, pen, new Point(cx - s * 0.15, cy - s * 0.15), s * 0.6, s * 0.6);
                context.DrawLine(pen, new Point(cx + s * 0.25, cy + s * 0.25), new Point(cx + s * 0.6, cy + s * 0.6));
                // Plus sign
                context.DrawLine(pen, new Point(cx - s * 0.15 - s * 0.3, cy - s * 0.15), new Point(cx - s * 0.15 + s * 0.3, cy - s * 0.15));
                context.DrawLine(pen, new Point(cx - s * 0.15, cy - s * 0.15 - s * 0.3), new Point(cx - s * 0.15, cy - s * 0.15 + s * 0.3));
                break;

            case ToolboxButton.ZoomOut:
                // Magnifying glass with -
                context.DrawEllipse(null, pen, new Point(cx - s * 0.15, cy - s * 0.15), s * 0.6, s * 0.6);
                context.DrawLine(pen, new Point(cx + s * 0.25, cy + s * 0.25), new Point(cx + s * 0.6, cy + s * 0.6));
                // Minus sign
                context.DrawLine(pen, new Point(cx - s * 0.15 - s * 0.3, cy - s * 0.15), new Point(cx - s * 0.15 + s * 0.3, cy - s * 0.15));
                break;

            case ToolboxButton.ResetZoom:
                // Reset/refresh icon: circular arrow
                var radius = s * 0.6;
                var arcPen = new Pen(color, 1.5);
                // Draw arc (approximated with a circle outline)
                context.DrawEllipse(null, arcPen, new Point(cx, cy), radius, radius);
                // Arrow at top
                context.DrawLine(pen, new Point(cx, cy - radius - s * 0.2), new Point(cx + s * 0.3, cy - radius + s * 0.1));
                context.DrawLine(pen, new Point(cx, cy - radius - s * 0.2), new Point(cx - s * 0.3, cy - radius + s * 0.1));
                break;

            case ToolboxButton.Restore:
                // Restore icon: counter-clockwise circular arrow
                var rRadius = s * 0.6;
                context.DrawEllipse(null, new Pen(color, 1.5), new Point(cx, cy), rRadius, rRadius);
                // Arrow at bottom-left (counter-clockwise)
                var arrowAngle = Math.PI * 0.75;
                var arrowX = cx + rRadius * Math.Cos(arrowAngle);
                var arrowY = cy + rRadius * Math.Sin(arrowAngle);
                context.DrawLine(pen, new Point(arrowX, arrowY), new Point(arrowX - s * 0.3, arrowY + s * 0.2));
                context.DrawLine(pen, new Point(arrowX, arrowY), new Point(arrowX + s * 0.1, arrowY + s * 0.3));
                break;

            case ToolboxButton.DataZoom:
                // Data zoom icon: magnifying glass with range bars
                context.DrawEllipse(null, pen, new Point(cx - s * 0.1, cy - s * 0.2), s * 0.45, s * 0.45);
                context.DrawLine(pen, new Point(cx + s * 0.2, cy + s * 0.1), new Point(cx + s * 0.5, cy + s * 0.4));
                // Small bars inside
                context.DrawLine(pen, new Point(cx - s * 0.3, cy - s * 0.1), new Point(cx + s * 0.1, cy - s * 0.1));
                context.DrawLine(pen, new Point(cx - s * 0.2, cy + s * 0.05), new Point(cx + s * 0.0, cy + s * 0.05));
                break;

            case ToolboxButton.MagicType:
                // Magic type icon: bar chart with line overlay
                // Bar chart
                context.DrawRectangle(null, pen, new Rect(cx - s * 0.6, cy - s * 0.2, s * 0.3, s * 0.7));
                context.DrawRectangle(null, pen, new Rect(cx - s * 0.2, cy - s * 0.5, s * 0.3, s * 1.0));
                context.DrawRectangle(null, pen, new Rect(cx + s * 0.2, cy - s * 0.3, s * 0.3, s * 0.8));
                // Line overlay
                context.DrawLine(new Pen(color, 1.5), new Point(cx - s * 0.45, cy + s * 0.1), new Point(cx - s * 0.05, cy - s * 0.3));
                context.DrawLine(new Pen(color, 1.5), new Point(cx - s * 0.05, cy - s * 0.3), new Point(cx + s * 0.35, cy - s * 0.1));
                break;

            case ToolboxButton.Brush:
                // Brush icon: paintbrush shape
                // Handle
                context.DrawLine(new Pen(color, 2), new Point(cx - s * 0.5, cy + s * 0.5), new Point(cx, cy));
                // Bristles
                context.DrawRectangle(color, null, new Rect(cx - s * 0.15, cy - s * 0.5, s * 0.3, s * 0.5), 2, 2);
                break;
        }
    }

    private T? TryFindResource<T>(string key) where T : class
    {
        try
        {
            if (global::Avalonia.Application.Current?.TryGetResource(key, global::Avalonia.Styling.ThemeVariant.Default, out var resource) == true
                && resource is T typed)
                return typed;
        }
        catch { }
        return null;
    }

    private enum ToolboxButton
    {
        SaveAsImage,
        DataView,
        ZoomIn,
        ZoomOut,
        ResetZoom,
        Restore,
        DataZoom,
        MagicType,
        Brush
    }
}
