namespace AuraUI.Controls.Charts.G2;

/// <summary>
/// Annotation type for chart overlays.
/// </summary>
public enum AnnotationType
{
    /// <summary>Text label placed at a data point or arbitrary position.</summary>
    Text,

    /// <summary>
    /// Line annotation: horizontal, vertical, or diagonal.
    /// Used for reference lines, thresholds, trend lines.
    /// </summary>
    Line,

    /// <summary>
    /// Region annotation: highlights a rectangular area.
    /// Used for highlighting time periods, value ranges, etc.
    /// </summary>
    Region,

    /// <summary>
    /// Arc annotation: draws an arc on polar coordinates.
    /// Used for highlighting sectors in pie/radar charts.
    /// </summary>
    Arc,

    /// <summary>
    /// Image annotation: places an image at a specific position.
    /// </summary>
    Image,

    /// <summary>
    /// Data marker: marks a specific data point with a label and optional connector line.
    /// </summary>
    DataMarker,

    /// <summary>
    /// Data region: marks a region between two data points with shading and labels.
    /// </summary>
    DataRegion,

    /// <summary>
    /// Shape annotation: draws a custom shape (circle, rectangle, etc.) at a position.
    /// </summary>
    Shape,

    /// <summary>
    /// Trend line annotation: draws a regression or moving average line.
    /// </summary>
    TrendLine,

    /// <summary>
    /// Threshold/limit line: horizontal or vertical line at a fixed value.
    /// </summary>
    Threshold,

    /// <summary>
    /// Bracket annotation: connects two points with a bracket shape.
    /// </summary>
    Bracket,

    /// <summary>
    /// Badge annotation: a small badge/label attached to a data point.
    /// </summary>
    Badge
}

/// <summary>
/// Specification for a chart annotation. Annotations are visual overlays that provide
/// context, highlight important features, or add reference information to a chart.
/// </summary>
public class AnnotationSpec
{
    /// <summary>The annotation type.</summary>
    public AnnotationType Type { get; set; }

    /// <summary>
    /// Whether this annotation is visible.
    /// </summary>
    public bool Visible { get; set; } = true;

    // ────────────────────────────────────────────────
    //  Positioning
    // ────────────────────────────────────────────────

    /// <summary>
    /// X position (data value). For line annotations, this is the start X.
    /// </summary>
    public double? X { get; set; }

    /// <summary>
    /// Y position (data value). For line annotations, this is the start Y.
    /// </summary>
    public double? Y { get; set; }

    /// <summary>
    /// End X position for line/region/data-region annotations.
    /// </summary>
    public double? X2 { get; set; }

    /// <summary>
    /// End Y position for line/region/data-region annotations.
    /// </summary>
    public double? Y2 { get; set; }

    /// <summary>
    /// X position as a fraction of the plot area (0.0 = left, 1.0 = right).
    /// Used when positioning by pixel rather than data value.
    /// </summary>
    public double? XPercent { get; set; }

    /// <summary>
    /// Y position as a fraction of the plot area (0.0 = top, 1.0 = bottom).
    /// </summary>
    public double? YPercent { get; set; }

    /// <summary>
    /// Data field name for X position (resolved from data, not a raw value).
    /// </summary>
    public string? XField { get; set; }

    /// <summary>
    /// Data field name for Y position (resolved from data, not a raw value).
    /// </summary>
    public string? YField { get; set; }

    // ────────────────────────────────────────────────
    //  Text content
    // ────────────────────────────────────────────────

    /// <summary>
    /// Text content for text/data-marker/badge annotations.
    /// </summary>
    public string? Text { get; set; }

    /// <summary>
    /// Content lines for multi-line annotations.
    /// </summary>
    public string[]? TextLines { get; set; }

    // ────────────────────────────────────────────────
    //  Style
    // ────────────────────────────────────────────────

    /// <summary>Font size for text.</summary>
    public double FontSize { get; set; } = 12;

    /// <summary>Font family for text.</summary>
    public string? FontFamily { get; set; }

    /// <summary>Font weight for text.</summary>
    public Avalonia.Media.FontWeight FontWeight { get; set; } = Avalonia.Media.FontWeight.Normal;

    /// <summary>Text color.</summary>
    public Avalonia.Media.IBrush? TextBrush { get; set; }

    /// <summary>Line/stroke color.</summary>
    public Avalonia.Media.IBrush? Stroke { get; set; }

    /// <summary>Line/stroke thickness.</summary>
    public double StrokeThickness { get; set; } = 1;

    /// <summary>Line dash pattern (null = solid).</summary>
    public double[]? Dash { get; set; }

    /// <summary>Fill color for regions/arcs.</summary>
    public Avalonia.Media.IBrush? Fill { get; set; }

    /// <summary>Fill opacity (0.0-1.0).</summary>
    public double FillOpacity { get; set; } = 0.2;

    /// <summary>Corner radius for region annotations.</summary>
    public double CornerRadius { get; set; }

    // ────────────────────────────────────────────────
    //  Line annotation options
    // ────────────────────────────────────────────────

    /// <summary>
    /// Line direction. When Start/End positions are set, this is auto-detected.
    /// </summary>
    public AnnotationLineDirection LineDirection { get; set; } = AnnotationLineDirection.Horizontal;

    /// <summary>
    /// Whether to show an arrowhead at the end of the line.
    /// </summary>
    public bool ShowArrow { get; set; }

    /// <summary>
    /// Arrowhead size in pixels.
    /// </summary>
    public double ArrowSize { get; set; } = 8;

    // ────────────────────────────────────────────────
    //  Region annotation options
    // ────────────────────────────────────────────────

    /// <summary>
    /// Whether the region spans the full Y range (for vertical band highlighting).
    /// </summary>
    public bool SpanY { get; set; }

    /// <summary>
    /// Whether the region spans the full X range (for horizontal band highlighting).
    /// </summary>
    public bool SpanX { get; set; }

    // ────────────────────────────────────────────────
    //  Image annotation options
    // ────────────────────────────────────────────────

    /// <summary>
    /// Image source URL or path for image annotations.
    /// </summary>
    public string? ImageSource { get; set; }

    /// <summary>
    /// Image width in pixels.
    /// </summary>
    public double? ImageWidth { get; set; }

    /// <summary>
    /// Image height in pixels.
    /// </summary>
    public double? ImageHeight { get; set; }

    // ────────────────────────────────────────────────
    //  DataMarker options
    // ────────────────────────────────────────────────

    /// <summary>
    /// Connector line length for data markers.
    /// </summary>
    public double ConnectorLength { get; set; } = 20;

    /// <summary>
    /// Connector line style.
    /// </summary>
    public DataMarkerConnectorStyle ConnectorStyle { get; set; } = DataMarkerConnectorStyle.Line;

    /// <summary>
    /// Text position relative to the data point for data markers.
    /// </summary>
    public AnnotationPosition TextPosition { get; set; } = AnnotationPosition.Top;

    /// <summary>
    /// Auto-rotate connector line to avoid overlap with other annotations.
    /// </summary>
    public bool AutoRotate { get; set; }

    // ────────────────────────────────────────────────
    //  DataRegion options
    // ────────────────────────────────────────────────

    /// <summary>
    /// Label for the data region.
    /// </summary>
    public string? RegionLabel { get; set; }

    // ────────────────────────────────────────────────
    //  Arc annotation options
    // ────────────────────────────────────────────────

    /// <summary>
    /// Start angle in radians for arc annotations.
    /// </summary>
    public double? StartAngle { get; set; }

    /// <summary>
    /// End angle in radians for arc annotations.
    /// </summary>
    public double? EndAngle { get; set; }

    /// <summary>
    /// Inner radius for arc annotations (0-1 fraction of outer radius).
    /// </summary>
    public double InnerRadius { get; set; }

    /// <summary>
    /// Outer radius for arc annotations (0-1 fraction of available space).
    /// </summary>
    public double OuterRadius { get; set; } = 1.0;

    // ────────────────────────────────────────────────
    //  Badge options
    // ────────────────────────────────────────────────

    /// <summary>
    /// Badge background color.
    /// </summary>
    public Avalonia.Media.IBrush? BadgeBackground { get; set; }

    /// <summary>
    /// Badge text color.
    /// </summary>
    public Avalonia.Media.IBrush? BadgeForeground { get; set; }

    /// <summary>
    /// Badge shape.
    /// </summary>
    public BadgeShape BadgeShape { get; set; } = BadgeShape.Circle;

    /// <summary>
    /// Badge size (diameter) in pixels.
    /// </summary>
    public double BadgeSize { get; set; } = 20;

    // ────────────────────────────────────────────────
    //  Z-order
    // ────────────────────────────────────────────────

    /// <summary>
    /// Z-index for layering annotations. Higher values are drawn on top.
    /// </summary>
    public int ZIndex { get; set; }

    /// <summary>
    /// Whether the annotation should be clipped to the plot area.
    /// Default true.
    /// </summary>
    public bool ClipToPlotArea { get; set; } = true;
}

/// <summary>
/// Direction of an annotation line.
/// </summary>
public enum AnnotationLineDirection
{
    Horizontal,
    Vertical,
    Diagonal
}

/// <summary>
/// Position of text relative to a point.
/// </summary>
public enum AnnotationPosition
{
    Top,
    Bottom,
    Left,
    Right,
    TopLeft,
    TopRight,
    BottomLeft,
    BottomRight,
    Center
}

/// <summary>
/// Connector line style for data markers.
/// </summary>
public enum DataMarkerConnectorStyle
{
    /// <summary>Straight line connector.</summary>
    Line,

    /// <summary>Elbow (right-angle) connector.</summary>
    Elbow,

    /// <summary>No connector (text directly at the point).</summary>
    None
}

/// <summary>
/// Badge shape.
/// </summary>
public enum BadgeShape
{
    Circle,
    Square,
    Diamond,
    Triangle
}
