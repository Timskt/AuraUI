namespace AuraUI.Controls.Charts;

/// <summary>
/// The type of interpolation between data points in line/area series.
/// </summary>
public enum ChartInterpolation
{
    /// <summary>Straight line segments between points.</summary>
    Linear,

    /// <summary>Smooth cubic Bezier curves (monotone).</summary>
    MonotoneCubic,

    /// <summary>Step function (horizontal then vertical).</summary>
    StepBefore,

    /// <summary>Step function (vertical then horizontal).</summary>
    StepAfter,

    /// <summary>Catmull-Rom spline interpolation (passes through all points).</summary>
    CatmullRom
}

/// <summary>
/// Position of an axis relative to the chart area.
/// </summary>
public enum AxisPosition
{
    Left,
    Right,
    Top,
    Bottom
}

/// <summary>
/// The type of scale used on an axis.
/// </summary>
public enum AxisScale
{
    /// <summary>Numeric linear scale.</summary>
    Linear,

    /// <summary>Numeric logarithmic scale.</summary>
    Logarithmic,

    /// <summary>Discrete category labels.</summary>
    Category,

    /// <summary>Date/time values.</summary>
    DateTime
}

/// <summary>
/// Position of the chart legend.
/// </summary>
public enum LegendPosition
{
    Top,
    Bottom,
    Left,
    Right,
    None
}

/// <summary>
/// Tooltip trigger mode.
/// </summary>
public enum TooltipTrigger
{
    /// <summary>Tooltip appears on hover over individual data items.</summary>
    Item,

    /// <summary>Tooltip appears on hover over the axis, showing all series at that X position.</summary>
    Axis,

    /// <summary>No tooltip.</summary>
    None
}

/// <summary>
/// Shape of data point markers in line/area/scatter series.
/// </summary>
public enum MarkerShape
{
    Circle,
    Square,
    Diamond,
    Triangle,
    Cross,
    None
}

/// <summary>
/// Stack mode for bar/area series.
/// </summary>
public enum StackMode
{
    None,
    Normal,
    Percent
}

/// <summary>
/// Gauge display mode.
/// </summary>
public enum GaugeMode
{
    /// <summary>180-degree half-circle gauge.</summary>
    Half,

    /// <summary>270-degree three-quarter gauge.</summary>
    ThreeQuarter,

    /// <summary>360-degree full-circle gauge.</summary>
    Full
}

/// <summary>
/// Chart theme preset.
/// </summary>
public enum ChartTheme
{
    /// <summary>Light theme (white backgrounds, dark text).</summary>
    Light,

    /// <summary>Dark theme (dark backgrounds, light text).</summary>
    Dark,

    /// <summary>Custom theme (user-defined colors).</summary>
    Custom
}

/// <summary>
/// Position of the tooltip relative to the data point.
/// </summary>
public enum TooltipPosition
{
    /// <summary>Tooltip appears above the data point.</summary>
    Top,

    /// <summary>Tooltip appears below the data point.</summary>
    Bottom,

    /// <summary>Tooltip appears to the left of the data point.</summary>
    Left,

    /// <summary>Tooltip appears to the right of the data point.</summary>
    Right,

    /// <summary>Tooltip appears inside the chart area near the data point.</summary>
    Inside
}

/// <summary>
/// Orientation of the legend items layout.
/// </summary>
public enum LegendOrientation
{
    /// <summary>Items arranged horizontally.</summary>
    Horizontal,

    /// <summary>Items arranged vertically.</summary>
    Vertical
}

/// <summary>
/// Type of chart scale interpolation for data zoom and axis mapping.
/// </summary>
public enum ChartScaleType
{
    /// <summary>Numeric linear scale.</summary>
    Linear,

    /// <summary>Numeric logarithmic scale (base 10).</summary>
    Logarithmic,

    /// <summary>Date/time scale.</summary>
    Time,

    /// <summary>Ordinal/category scale.</summary>
    Category
}

/// <summary>
/// Type of curve interpolation for smoothing.
/// </summary>
public enum ChartCurveType
{
    /// <summary>No smoothing — straight line segments.</summary>
    Linear,

    /// <summary>Catmull-Rom spline interpolation.</summary>
    CatmullRom,

    /// <summary>Cubic Bezier interpolation.</summary>
    Bezier,

    /// <summary>Monotone cubic interpolation (preserves monotonicity).</summary>
    MonotoneCubic
}

/// <summary>
/// DataZoom display mode.
/// </summary>
public enum DataZoomType
{
    /// <summary>Slider below the chart showing the full data range (separate from chart).</summary>
    Slider,

    /// <summary>Overlay on the chart area (transparent mask outside selection).</summary>
    Inside,

    /// <summary>Both slider and inside zoom controls active simultaneously.</summary>
    Both
}

/// <summary>
/// Type of brush/selection interaction.
/// </summary>
public enum BrushType
{
    /// <summary>Vertical line selection (select by X range).</summary>
    LineX,

    /// <summary>Horizontal line selection (select by Y range).</summary>
    LineY,

    /// <summary>Rectangular area selection.</summary>
    Rect,

    /// <summary>Polygon area selection.</summary>
    Polygon
}

/// <summary>
/// Bar grouping mode when multiple bar series are present.
/// </summary>
public enum BarMode
{
    /// <summary>Bars are placed side by side (default grouped behavior).</summary>
    Grouped,

    /// <summary>Bars are stacked on top of each other.</summary>
    Stacked,

    /// <summary>Bars are stacked and normalized to 100%.</summary>
    StackedPercent
}

/// <summary>
/// Mode for pie chart rendering.
/// </summary>
public enum PieMode
{
    /// <summary>Standard pie or donut chart.</summary>
    Standard,

    /// <summary>Nightingale rose chart — all slices have equal angle, radius varies by value.</summary>
    Rose,

    /// <summary>Nightingale area mode — angle varies by value, but radius also scales.</summary>
    RoseArea
}

/// <summary>
/// Tree chart layout direction.
/// </summary>
public enum TreeLayout
{
    /// <summary>Left-to-right orthogonal layout.</summary>
    LR,

    /// <summary>Right-to-left orthogonal layout.</summary>
    RL,

    /// <summary>Top-to-bottom orthogonal layout.</summary>
    TB,

    /// <summary>Bottom-to-top orthogonal layout.</summary>
    BT
}

/// <summary>
/// Tree chart rendering mode.
/// </summary>
public enum TreeMode
{
    /// <summary>Orthogonal (right-angle) branches.</summary>
    Orthogonal,

    /// <summary>Radial layout (branches radiate from center).</summary>
    Radial
}

/// <summary>
/// Gauge rendering type for multi-ring and special gauges.
/// </summary>
public enum GaugeType
{
    /// <summary>Standard single-ring gauge.</summary>
    Standard,

    /// <summary>Progress gauge (filled arc showing percentage).</summary>
    Progress,

    /// <summary>Temperature gauge with colored segments.</summary>
    Temperature,

    /// <summary>Clock gauge showing time.</summary>
    Clock,

    /// <summary>Multi-ring gauge with concentric arcs.</summary>
    MultiRing
}

/// <summary>
/// Step line position mode.
/// </summary>
public enum StepPosition
{
    /// <summary>Step before the data point.</summary>
    Before,

    /// <summary>Step after the data point.</summary>
    After,

    /// <summary>Step at the midpoint between data points.</summary>
    Middle
}

/// <summary>
/// Regression line type for scatter charts.
/// </summary>
public enum RegressionType
{
    /// <summary>No regression line.</summary>
    None,

    /// <summary>Linear regression (least squares fit).</summary>
    Linear,

    /// <summary>Polynomial regression (quadratic).</summary>
    Polynomial,

    /// <summary>Exponential regression.</summary>
    Exponential,

    /// <summary>Logarithmic regression.</summary>
    Logarithmic
}

/// <summary>
/// Axis pointer mode for crosshair interaction.
/// </summary>
public enum AxisPointerMode
{
    /// <summary>Line mode (thin crosshair line).</summary>
    Line,

    /// <summary>Shadow mode (shaded area at the axis position).</summary>
    Shadow,

    /// <summary>Cross mode (both X and Y crosshair lines).</summary>
    Cross
}

/// <summary>
/// Visual map type for color/size/opacity mapping.
/// </summary>
public enum VisualMapType
{
    /// <summary>Continuous gradient mapping.</summary>
    Continuous,

    /// <summary>Piecewise (discrete ranges) mapping.</summary>
    Piecewise
}

/// <summary>
/// Mark type for annotations.
/// </summary>
public enum MarkType
{
    /// <summary>Maximum value in the series.</summary>
    Max,

    /// <summary>Minimum value in the series.</summary>
    Min,

    /// <summary>Average value in the series.</summary>
    Average
}

/// <summary>
/// Shape of radar grid lines.
/// </summary>
public enum RadarShape
{
    /// <summary>Polygon (straight edges between axes).</summary>
    Polygon,

    /// <summary>Circle (curved grid lines).</summary>
    Circle
}

/// <summary>
/// Sort order for pie chart slices.
/// </summary>
public enum PieSortOrder
{
    /// <summary>No sorting — use data insertion order.</summary>
    None,

    /// <summary>Sort slices by value, smallest first.</summary>
    Ascending,

    /// <summary>Sort slices by value, largest first.</summary>
    Descending
}

/// <summary>
/// Export format for chart export operations.
/// </summary>
public enum ChartExportFormat
{
    Png,
    Svg,
    Clipboard
}

/// <summary>
/// Chart animation mode.
/// </summary>
public enum ChartAnimationMode
{
    /// <summary>No animation.</summary>
    None,

    /// <summary>Entry animation only (data grows from zero).</summary>
    Entry,

    /// <summary>Update animation only (smooth transition when data changes).</summary>
    Update,

    /// <summary>Exit animation only (data shrinks to zero).</summary>
    Exit,

    /// <summary>Full lifecycle animation (entry + update + exit).</summary>
    All
}

/// <summary>
/// Easing preset for chart animations.
/// </summary>
public enum ChartEasingPreset
{
    /// <summary>Cubic ease-out (default, decelerating).</summary>
    CubicEaseOut,

    /// <summary>Cubic ease-in (accelerating).</summary>
    CubicEaseIn,

    /// <summary>Cubic ease-in-out (accelerate then decelerate).</summary>
    CubicEaseInOut,

    /// <summary>Linear (no easing).</summary>
    Linear,

    /// <summary>Elastic ease-out (bouncy).</summary>
    ElasticEaseOut,

    /// <summary>Back ease-out (overshoot then settle).</summary>
    BackEaseOut
}

/// <summary>
/// Icon shape for legend items (ECharts legend.icon).
/// </summary>
public enum LegendIcon
{
    /// <summary>Circle icon.</summary>
    Circle,

    /// <summary>Rectangle icon.</summary>
    Rect,

    /// <summary>Rounded rectangle icon.</summary>
    RoundRect,

    /// <summary>Triangle icon.</summary>
    Triangle,

    /// <summary>Diamond icon.</summary>
    Diamond,

    /// <summary>Pin (teardrop) icon.</summary>
    Pin,

    /// <summary>No icon (text only).</summary>
    None,

    /// <summary>Use a custom path for the icon.</summary>
    Custom
}

/// <summary>
/// Image format for save-as-image toolbox feature.
/// </summary>
public enum SaveAsImageFormat
{
    /// <summary>PNG format (lossless, supports transparency).</summary>
    Png,

    /// <summary>JPEG format (lossy, smaller file size).</summary>
    Jpeg,

    /// <summary>SVG format (vector, scalable).</summary>
    Svg
}

/// <summary>
/// Location of the axis name label.
/// </summary>
public enum AxisNameLocation
{
    /// <summary>Name at the start of the axis.</summary>
    Start,

    /// <summary>Name at the center of the axis.</summary>
    Middle,

    /// <summary>Name at the end of the axis.</summary>
    End
}

/// <summary>
/// Render mode for the tooltip.
/// </summary>
public enum TooltipRenderMode
{
    /// <summary>Render tooltip using DrawingContext calls (native).</summary>
    Canvas,

    /// <summary>Render tooltip as styled text (lightweight).</summary>
    Html
}

/// <summary>
/// Type of axis pointer crosshair indicator.
/// </summary>
public enum AxisPointerType
{
    /// <summary>Thin line at the cursor position.</summary>
    Line,

    /// <summary>Shaded band at the nearest category/interval.</summary>
    Shadow,

    /// <summary>Cross-shaped cursor (both X and Y lines).</summary>
    Cross
}

/// <summary>
/// Magic type for toolbox series type switching.
/// </summary>
public enum MagicChartType
{
    /// <summary>Line chart.</summary>
    Line,

    /// <summary>Bar chart.</summary>
    Bar,

    /// <summary>Stacked mode (combined with Line or Bar).</summary>
    Stack
}
