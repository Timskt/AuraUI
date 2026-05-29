namespace AuraUI.Controls.Charts.G2;

/// <summary>
/// Interaction type for chart interactivity.
/// </summary>
public enum InteractionType
{
    /// <summary>Highlight an element on hover (opacity/color change).</summary>
    ElementHighlight,

    /// <summary>Select an element on click (persistent highlight).</summary>
    ElementSelect,

    /// <summary>Highlight a group of related elements on hover.</summary>
    ElementHighlightByGroup,

    /// <summary>Select a group of related elements on click.</summary>
    ElementSelectByGroup,

    /// <summary>Brush selection on X-axis (vertical brush, selects by X range).</summary>
    BrushX,

    /// <summary>Brush selection on Y-axis (horizontal brush, selects by Y range).</summary>
    BrushY,

    /// <summary>2D rectangular brush selection (selects by X and Y range).</summary>
    Brush2D,

    /// <summary>Polygon brush selection.</summary>
    BrushPolygon,

    /// <summary>Filter data by clicking legend items.</summary>
    LegendFilter,

    /// <summary>Highlight series by hovering legend items.</summary>
    LegendHighlight,

    /// <summary>Show tooltip on hover.</summary>
    Tooltip,

    /// <summary>Zoom by mouse scroll wheel.</summary>
    Zoom,

    /// <summary>Zoom on X-axis only.</summary>
    ZoomX,

    /// <summary>Zoom on Y-axis only.</summary>
    ZoomY,

    /// <summary>Scroll to see more data (scrollbar-like behavior).</summary>
    Scroll,

    /// <summary>Drag to pan (move the visible data window).</summary>
    DragMove,

    /// <summary>Slider-based zoom (range slider below the chart).</summary>
    SliderZoom,

    /// <summary>Track cursor with a vertical line across all Y values.</summary>
    CursorTracker,

    /// <summary>
    /// Active region interaction: highlights the nearest data point
    /// and shows a crosshair.
    /// </summary>
    ActiveRegion,

    /// <summary>View linked interaction: synchronize multiple views.</summary>
    ViewLinked,

    /// <summary>Custom interaction defined by a delegate.</summary>
    Custom
}

/// <summary>
/// Specification for a chart interaction. Interactions define how users interact
/// with chart elements through mouse, touch, or keyboard input.
/// </summary>
public class InteractionSpec
{
    /// <summary>The interaction type.</summary>
    public InteractionType Type { get; set; }

    /// <summary>
    /// Whether this interaction is enabled.
    /// </summary>
    public bool Enabled { get; set; } = true;

    // ────────────────────────────────────────────────
    //  Highlight / Select options
    // ────────────────────────────────────────────────

    /// <summary>
    /// Opacity of non-highlighted elements when highlighting.
    /// Range 0.0-1.0. Default 0.3.
    /// </summary>
    public double DimOpacity { get; set; } = 0.3;

    /// <summary>
    /// Stroke color for the highlighted element.
    /// </summary>
    public Avalonia.Media.IBrush? HighlightStroke { get; set; }

    /// <summary>
    /// Stroke width for the highlighted element.
    /// </summary>
    public double? HighlightStrokeWidth { get; set; }

    /// <summary>
    /// Fill color override for the selected element.
    /// </summary>
    public Avalonia.Media.IBrush? SelectFill { get; set; }

    /// <summary>
    /// Whether highlight is triggered by group (category) or individual element.
    /// </summary>
    public bool ByGroup { get; set; }

    // ────────────────────────────────────────────────
    //  Brush options
    // ────────────────────────────────────────────────

    /// <summary>
    /// Brush mask color (semi-transparent overlay outside selection).
    /// </summary>
    public Avalonia.Media.IBrush? BrushMaskColor { get; set; }

    /// <summary>
    /// Brush selection rectangle stroke.
    /// </summary>
    public Avalonia.Media.IBrush? BrushStroke { get; set; }

    /// <summary>
    /// Brush selection rectangle fill.
    /// </summary>
    public Avalonia.Media.IBrush? BrushFill { get; set; }

    /// <summary>
    /// Action to perform after brush selection.
    /// </summary>
    public BrushAction BrushAction { get; set; } = BrushAction.Filter;

    /// <summary>
    /// Whether the brush is movable after creation.
    /// </summary>
    public bool BrushMovable { get; set; }

    // ────────────────────────────────────────────────
    //  Zoom options
    // ────────────────────────────────────────────────

    /// <summary>
    /// Minimum zoom level (fraction of original range). Default 0.01.
    /// </summary>
    public double MinZoomLevel { get; set; } = 0.01;

    /// <summary>
    /// Maximum zoom level (fraction of original range). Default 1.0.
    /// </summary>
    public double MaxZoomLevel { get; set; } = 1.0;

    /// <summary>
    /// Zoom sensitivity (multiplier for scroll events). Default 1.0.
    /// </summary>
    public double ZoomSensitivity { get; set; } = 1.0;

    // ────────────────────────────────────────────────
    //  Scroll options
    // ────────────────────────────────────────────────

    /// <summary>
    /// Visible window size for scroll (number of data points visible at once).
    /// </summary>
    public int? ScrollWindowSize { get; set; }

    /// <summary>
    /// Scroll step size (number of data points per scroll).
    /// </summary>
    public int? ScrollStep { get; set; }

    // ────────────────────────────────────────────────
    //  Drag options
    // ────────────────────────────────────────────────

    /// <summary>
    /// Whether to allow drag on X-axis.
    /// </summary>
    public bool DragX { get; set; } = true;

    /// <summary>
    /// Whether to allow drag on Y-axis.
    /// </summary>
    public bool DragY { get; set; } = true;

    /// <summary>
    /// Whether to constrain drag to the data range.
    /// </summary>
    public bool ConstrainDrag { get; set; } = true;

    // ────────────────────────────────────────────────
    //  Custom
    // ────────────────────────────────────────────────

    /// <summary>
    /// Custom interaction delegate for InteractionType.Custom.
    /// </summary>
    public Func<object, object?>? CustomHandler { get; set; }

    /// <summary>
    /// Debounce interval in milliseconds for rapid-fire events (brush, zoom).
    /// Default 16ms (one frame at 60fps).
    /// </summary>
    public int DebounceMs { get; set; } = 16;

    // ────────────────────────────────────────────────
    //  Events
    // ────────────────────────────────────────────────

    /// <summary>
    /// Event raised when an element is highlighted.
    /// </summary>
    public event EventHandler<InteractionEventArgs>? Highlighted;

    /// <summary>
    /// Event raised when an element is selected.
    /// </summary>
    public event EventHandler<InteractionEventArgs>? Selected;

    /// <summary>
    /// Event raised when a brush selection changes.
    /// </summary>
    public event EventHandler<BrushEventArgs>? BrushChanged;

    /// <summary>
    /// Event raised when zoom level changes.
    /// </summary>
    public event EventHandler<ZoomEventArgs>? ZoomChanged;

    internal void RaiseHighlighted(InteractionEventArgs args) => Highlighted?.Invoke(this, args);
    internal void RaiseSelected(InteractionEventArgs args) => Selected?.Invoke(this, args);
    internal void RaiseBrushChanged(BrushEventArgs args) => BrushChanged?.Invoke(this, args);
    internal void RaiseZoomChanged(ZoomEventArgs args) => ZoomChanged?.Invoke(this, args);
}

/// <summary>
/// Action to perform after brush selection.
/// </summary>
public enum BrushAction
{
    /// <summary>Filter: show only selected data, hide the rest.</summary>
    Filter,

    /// <summary>Highlight: dim non-selected data but keep it visible.</summary>
    Highlight,

    /// <summary>No visible action — just fire the BrushChanged event.</summary>
    Notify
}

/// <summary>
/// Event args for element interaction events.
/// </summary>
public class InteractionEventArgs : EventArgs
{
    /// <summary>The data field value of the interacted element.</summary>
    public object? DataValue { get; set; }

    /// <summary>The data index of the interacted element.</summary>
    public int DataIndex { get; set; }

    /// <summary>The encoding channel that was interacted with.</summary>
    public string? Channel { get; set; }

    /// <summary>The pixel position of the interaction.</summary>
    public Avalonia.Point Position { get; set; }

    /// <summary>Additional data associated with the interaction.</summary>
    public object? Extra { get; set; }
}

/// <summary>
/// Event args for brush selection events.
/// </summary>
public class BrushEventArgs : EventArgs
{
    /// <summary>The selected X range (data values). Null if no X selection.</summary>
    public (double Min, double Max)? XRange { get; set; }

    /// <summary>The selected Y range (data values). Null if no Y selection.</summary>
    public (double Min, double Max)? YRange { get; set; }

    /// <summary>The selected data indices.</summary>
    public int[]? SelectedIndices { get; set; }

    /// <summary>The selected data values.</summary>
    public object[]? SelectedData { get; set; }
}

/// <summary>
/// Event args for zoom change events.
/// </summary>
public class ZoomEventArgs : EventArgs
{
    /// <summary>The new visible X range.</summary>
    public (double Min, double Max)? XRange { get; set; }

    /// <summary>The new visible Y range.</summary>
    public (double Min, double Max)? YRange { get; set; }

    /// <summary>The zoom level (1.0 = no zoom).</summary>
    public double ZoomLevel { get; set; } = 1.0;

    /// <summary>The zoom center point (data coordinates).</summary>
    public Avalonia.Point Center { get; set; }
}
