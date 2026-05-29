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

    /// <summary>Tooltip appears on hover over the axis.</summary>
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
