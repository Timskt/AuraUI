namespace AuraUI.Controls.Charts.G2;

/// <summary>
/// Scale type for data mapping, following AntV G2's Grammar of Graphics scale taxonomy.
/// Scales define how data values map to visual properties (position, color, size, etc.).
/// </summary>
public enum ScaleType
{
    /// <summary>Continuous numeric linear mapping. Default for numeric data.</summary>
    Linear,

    /// <summary>Logarithmic scale (base 10). Useful for data spanning many orders of magnitude.</summary>
    Log,

    /// <summary>Power scale with configurable exponent.</summary>
    Pow,

    /// <summary>Square root scale (exponent = 0.5). Good for area-based encodings.</summary>
    Sqrt,

    /// <summary>Temporal scale for DateTime values with automatic time interval ticks.</summary>
    Time,

    /// <summary>Discrete category scale. Maps distinct string labels to evenly spaced positions.</summary>
    Category,

    /// <summary>Band scale for bar charts. Maps categories to bands with configurable padding.</summary>
    Band,

    /// <summary>Point scale. Like Band but with zero bandwidth (for scatter-like layouts).</summary>
    Point,

    /// <summary>Ordinal scale. Maps discrete values to an arbitrary set of visual values (e.g., colors, shapes).</summary>
    Ordinal,

    /// <summary>Quantize scale. Divides continuous domain into discrete uniform bins.</summary>
    Quantize,

    /// <summary>Quantile scale. Divides continuous domain so each bin has equal number of data points.</summary>
    Quantile,

    /// <summary>Threshold scale. Maps arbitrary numeric thresholds to discrete output values.</summary>
    Threshold
}

/// <summary>
/// Configuration for a data scale. Scales define the mapping from data domain to visual range,
/// including tick generation, formatting, and domain clamping behavior.
/// </summary>
public class ScaleConfig
{
    /// <summary>The scale type (linear, log, category, etc.).</summary>
    public ScaleType Type { get; set; } = ScaleType.Linear;

    /// <summary>
    /// Explicit domain minimum. When null, auto-computed from data.
    /// For continuous scales only.
    /// </summary>
    public double? DomainMin { get; set; }

    /// <summary>
    /// Explicit domain maximum. When null, auto-computed from data.
    /// For continuous scales only.
    /// </summary>
    public double? DomainMax { get; set; }

    /// <summary>
    /// Explicit domain values for discrete scales (Category, Band, Point, Ordinal).
    /// When null, derived from unique data values.
    /// </summary>
    public IList<string>? Domain { get; set; }

    /// <summary>
    /// Explicit range minimum (visual output minimum). Default depends on encoding channel.
    /// </summary>
    public double? RangeMin { get; set; }

    /// <summary>
    /// Explicit range maximum (visual output maximum). Default depends on encoding channel.
    /// </summary>
    public double? RangeMax { get; set; }

    /// <summary>
    /// Explicit range values for ordinal/threshold scales (e.g., color palette, shape list).
    /// </summary>
    public IList<object>? Range { get; set; }

    /// <summary>
    /// Whether to round domain to "nice" numbers (0, 5, 10, 25, 50...).
    /// Improves readability of axis ticks. Default true for linear/log scales.
    /// </summary>
    public bool Nice { get; set; } = true;

    /// <summary>
    /// Explicit tick values. When set, overrides auto-generated ticks.
    /// </summary>
    public double[]? TickValues { get; set; }

    /// <summary>
    /// Approximate number of ticks to generate. Default 5.
    /// Actual count may vary to produce "nice" tick values.
    /// </summary>
    public int TickCount { get; set; } = 5;

    /// <summary>
    /// Format string for tick labels (e.g., "F2", "N0", "#,##0.00", "yyyy-MM-dd").
    /// </summary>
    public string? Format { get; set; }

    /// <summary>
    /// Custom formatter function for tick labels.
    /// Takes the raw value and returns the formatted string.
    /// </summary>
    public Func<double, string>? Formatter { get; set; }

    /// <summary>
    /// Whether to clamp domain values to the domain range.
    /// When true, values outside the domain are mapped to the nearest boundary.
    /// When false, values outside the domain are extrapolated.
    /// </summary>
    public bool Clamp { get; set; }

    /// <summary>
    /// Power scale exponent. Only used when Type is Pow.
    /// Default 2 (quadratic). Use 0.5 for square root, 3 for cubic, etc.
    /// </summary>
    public double Exponent { get; set; } = 2.0;

    /// <summary>
    /// Log scale base. Only used when Type is Log. Default 10.
    /// </summary>
    public double Base { get; set; } = 10.0;

    /// <summary>
    /// Band scale inner padding (0-1). Fraction of the step reserved for padding
    /// between bands. Default 0.1 (10% of step).
    /// </summary>
    public double PaddingInner { get; set; } = 0.1;

    /// <summary>
    /// Band scale outer padding (0-1). Fraction of the step reserved for padding
    /// before the first and after the last band. Default 0.05.
    /// </summary>
    public double PaddingOuter { get; set; } = 0.05;

    /// <summary>
    /// Alignment of the band within the step (0 = start, 0.5 = center, 1 = end).
    /// Only used when Type is Band. Default 0.5 (centered).
    /// </summary>
    public double Align { get; set; } = 0.5;

    /// <summary>
    /// Threshold breakpoints for Threshold scale. Values map to corresponding Range entries.
    /// Must be sorted in ascending order.
    /// </summary>
    public double[]? Thresholds { get; set; }

    /// <summary>
    /// Whether to include zero in the domain. Useful for bar charts
    /// where the baseline should always be visible. Default false.
    /// </summary>
    public bool Zero { get; set; }

    /// <summary>
    /// Minimum interval between ticks. Useful for time scales to force
    /// daily, weekly, or monthly ticks.
    /// </summary>
    public TimeSpan? TickInterval { get; set; }

    /// <summary>
    /// Whether to reverse the output range (high domain maps to low range).
    /// Useful for Y-axis in image coordinates. Default false.
    /// </summary>
    public bool Reverse { get; set; }

    /// <summary>
    /// Minimum allowed step between ticks. Prevents overcrowding.
    /// </summary>
    public double? MinTickStep { get; set; }

    /// <summary>
    /// Whether to hide this scale from the axis (suppress tick labels).
    /// Useful for color/size scales that have no axis. Default false.
    /// </summary>
    public bool HideAxis { get; set; }

    // ────────────────────────────────────────────────
    //  Factory methods for common configurations
    // ────────────────────────────────────────────────

    /// <summary>Create a linear scale with nice rounding and zero baseline.</summary>
    public static ScaleConfig LinearZero() => new() { Type = ScaleType.Linear, Zero = true, Nice = true };

    /// <summary>Create a logarithmic scale (base 10).</summary>
    public static ScaleConfig Logarithmic(double? domainMin = null, double? domainMax = null) =>
        new() { Type = ScaleType.Log, DomainMin = domainMin, DomainMax = domainMax, Nice = true };

    /// <summary>Create a category scale with the given domain labels.</summary>
    public static ScaleConfig Categorical(IList<string> categories) =>
        new() { Type = ScaleType.Category, Domain = categories };

    /// <summary>Create a band scale for bar charts with configurable padding.</summary>
    public static ScaleConfig Banded(IList<string> categories, double paddingInner = 0.1, double paddingOuter = 0.05) =>
        new() { Type = ScaleType.Band, Domain = categories, PaddingInner = paddingInner, PaddingOuter = paddingOuter };

    /// <summary>Create a time scale for temporal data.</summary>
    public static ScaleConfig Temporal(DateTime? min = null, DateTime? max = null) =>
        new()
        {
            Type = ScaleType.Time,
            DomainMin = min?.Ticks,
            DomainMax = max?.Ticks
        };

    /// <summary>Create a quantize scale that bins continuous values into discrete outputs.</summary>
    public static ScaleConfig Quantized(int binCount) =>
        new() { Type = ScaleType.Quantize, TickCount = binCount };

    /// <summary>Create a threshold scale with explicit breakpoints.</summary>
    public static ScaleConfig Thresholded(double[] thresholds, IList<object> outputRange) =>
        new() { Type = ScaleType.Threshold, Thresholds = thresholds, Range = outputRange };

    /// <summary>Create an ordinal scale mapping categories to a discrete visual range (e.g., colors).</summary>
    public static ScaleConfig OrdinalMapping(IList<string> domain, IList<object> range) =>
        new() { Type = ScaleType.Ordinal, Domain = domain, Range = range };
}
