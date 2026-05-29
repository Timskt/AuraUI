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
    StepAfter
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
    Inside
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
