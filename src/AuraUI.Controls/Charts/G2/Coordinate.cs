namespace AuraUI.Controls.Charts.G2;

/// <summary>
/// Type of coordinate system used for chart rendering.
/// </summary>
public enum CoordinateType
{
    /// <summary>
    /// Standard Cartesian (rectangular) coordinate system.
    /// X-axis horizontal, Y-axis vertical. Used for line, bar, area, scatter charts.
    /// </summary>
    Cartesian,

    /// <summary>
    /// Polar coordinate system with radial and angular axes.
    /// Used for pie charts, radar charts, and polar bar charts.
    /// </summary>
    Polar,

    /// <summary>
    /// Transposed Cartesian coordinate system (X and Y axes swapped).
    /// Used for horizontal bar charts.
    /// </summary>
    Transpose,

    /// <summary>
    /// Parallel coordinate system where each variable has its own vertical axis
    /// and data points are represented as polylines across axes.
    /// </summary>
    Parallel,

    /// <summary>
    /// Theta (angular) coordinate system for nightingale rose charts.
    /// Each category has equal angle, and the radius encodes the value.
    /// </summary>
    Theta
}

/// <summary>
/// Configuration for the chart coordinate system. The coordinate system determines
/// how data positions are mapped to visual positions on the canvas.
///
/// AntV G2 supports composable coordinate transformations:
///   Cartesian = rect()
///   Polar = polar()
///   Horizontal bar = rect() + transpose()
///   Nightingale rose = theta()
/// </summary>
public class CoordinateConfig
{
    /// <summary>The base coordinate type.</summary>
    public CoordinateType Type { get; set; } = CoordinateType.Cartesian;

    /// <summary>
    /// Start angle for polar coordinates, in radians.
    /// 0 = 3 o'clock, PI/2 = 12 o'clock (top), PI = 9 o'clock.
    /// Default -PI/2 (start at top, 12 o'clock position).
    /// </summary>
    public double StartAngle { get; set; } = -Math.PI / 2;

    /// <summary>
    /// End angle for polar coordinates, in radians.
    /// Default 3*PI/2 (full circle from -PI/2).
    /// </summary>
    public double EndAngle { get; set; } = 3 * Math.PI / 2;

    /// <summary>
    /// Inner radius ratio for polar coordinates (0-1).
    /// 0 = full pie, 0.5 = donut with 50% inner radius.
    /// Only applicable for Polar and Theta coordinate types.
    /// </summary>
    public double InnerRadius { get; set; }

    /// <summary>
    /// Outer radius ratio for polar coordinates (0-1).
    /// 1.0 = fill the entire available area. 0.8 = 80% of available area.
    /// Default 1.0.
    /// </summary>
    public double OuterRadius { get; set; } = 1.0;

    /// <summary>
    /// Whether the coordinate is transposed (X and Y axes swapped).
    /// Can be applied to Cartesian to create horizontal bar charts.
    /// </summary>
    public bool IsTransposed { get; set; }

    /// <summary>
    /// Whether the coordinate is reflected (mirrored) across the horizontal axis.
    /// Useful for butterfly charts or symmetric layouts.
    /// </summary>
    public bool IsReflected { get; set; }

    /// <summary>
    /// Whether the coordinate is reflected (mirrored) across the vertical axis.
    /// </summary>
    public bool IsReflectedY { get; set; }

    /// <summary>
    /// Number of axes for Parallel coordinates.
    /// Each axis represents a different variable/dimension.
    /// </summary>
    public int ParallelAxisCount { get; set; }

    /// <summary>
    /// Labels for each parallel axis. Length should match ParallelAxisCount.
    /// </summary>
    public string[]? ParallelAxisLabels { get; set; }

    /// <summary>
    /// Spacing between parallel axes as a fraction of total width.
    /// Default 0.05 (5% of width between each axis).
    /// </summary>
    public double ParallelAxisSpacing { get; set; } = 0.05;

    // ────────────────────────────────────────────────
    //  Factory methods
    // ────────────────────────────────────────────────

    /// <summary>Create a standard Cartesian (rectangular) coordinate system.</summary>
    public static CoordinateConfig Rect() => new() { Type = CoordinateType.Cartesian };

    /// <summary>Create a polar coordinate system for pie/radar charts.</summary>
    public static CoordinateConfig Polar(double innerRadius = 0, double outerRadius = 1.0) =>
        new() { Type = CoordinateType.Polar, InnerRadius = innerRadius, OuterRadius = outerRadius };

    /// <summary>Create a transposed (horizontal) Cartesian coordinate for horizontal bars.</summary>
    public static CoordinateConfig TransposedRect() =>
        new() { Type = CoordinateType.Cartesian, IsTransposed = true };

    /// <summary>Create a theta (nightingale rose) coordinate system.</summary>
    public static CoordinateConfig Theta(double innerRadius = 0, double outerRadius = 1.0) =>
        new() { Type = CoordinateType.Theta, InnerRadius = innerRadius, OuterRadius = outerRadius };

    /// <summary>Create a parallel coordinate system for multivariate data.</summary>
    public static CoordinateConfig Parallel(int axisCount, string[]? labels = null) =>
        new() { Type = CoordinateType.Parallel, ParallelAxisCount = axisCount, ParallelAxisLabels = labels };

    /// <summary>
    /// Whether this coordinate system uses polar (radial) mapping.
    /// </summary>
    public bool IsPolar => Type is CoordinateType.Polar or CoordinateType.Theta;

    /// <summary>
    /// The total sweep angle in radians for polar coordinates.
    /// </summary>
    public double SweepAngle => EndAngle - StartAngle;
}
