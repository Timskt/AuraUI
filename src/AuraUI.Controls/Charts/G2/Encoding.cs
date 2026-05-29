namespace AuraUI.Controls.Charts.G2;

/// <summary>
/// Type of encoding channel. Each channel maps a data field to a visual property.
/// </summary>
public enum EncodingChannel
{
    /// <summary>X position encoding (horizontal axis).</summary>
    X,

    /// <summary>Y position encoding (vertical axis).</summary>
    Y,

    /// <summary>
    /// Color encoding. Maps data values to colors.
    /// Supports discrete (categorical) and continuous (gradient) color mapping.
    /// </summary>
    Color,

    /// <summary>
    /// Size encoding. Maps data values to visual size (marker radius, bar width, etc.).
    /// Used for bubble charts and variable-width bars.
    /// </summary>
    Size,

    /// <summary>
    /// Shape encoding. Maps data values to different marker shapes.
    /// Discrete only (circle, square, triangle, diamond, cross, etc.).
    /// </summary>
    Shape,

    /// <summary>
    /// Opacity encoding. Maps data values to opacity (0.0 - 1.0).
    /// </summary>
    Opacity,

    /// <summary>
    /// Text encoding. Maps data values to text labels displayed on the chart.
    /// </summary>
    Text,

    /// <summary>
    /// Tooltip encoding. Specifies custom content for the tooltip display.
    /// Allows showing additional fields beyond X/Y in the tooltip.
    /// </summary>
    Tooltip,

    /// <summary>
    /// Angle encoding for polar/theta coordinates.
    /// Maps data values to angular position.
    /// </summary>
    Angle,

    /// <summary>
    /// Radius encoding for polar coordinates.
    /// Maps data values to radial distance from center.
    /// </summary>
    Radius,

    /// <summary>
    /// X2 encoding for ranges (e.g., interval bars, range areas).
    /// Specifies the end X position.
    /// </summary>
    X2,

    /// <summary>
    /// Y2 encoding for ranges (e.g., stacked bars, range areas).
    /// Specifies the end Y position.
    /// </summary>
    Y2,

    /// <summary>
    /// Key encoding for data join identity (used for stable animations across data updates).
    /// </summary>
    Key,

    /// <summary>
    /// Group encoding. Groups data points for grouped/stacked layouts.
    /// </summary>
    Group,

    /// <summary>
    /// Sort encoding. Determines sort order for stacking or layering.
    /// </summary>
    Sort,

    /// <summary>
    /// Detail encoding. Additional grouping that does not map to a visual channel
    /// but separates data into distinct series.
    /// </summary>
    Detail
}

/// <summary>
/// Represents a single encoding specification that maps a data field to a visual channel.
/// Encodings are the core of the Grammar of Graphics approach: they define HOW data
/// is represented visually.
/// </summary>
public class EncodingSpec
{
    /// <summary>The data field name to encode (e.g., "month", "sales", "category").</summary>
    public string Field { get; set; } = string.Empty;

    /// <summary>
    /// Optional aggregate function to apply before encoding.
    /// Useful for summarizing data (sum, mean, count, etc.).
    /// </summary>
    public AggregateType? Aggregate { get; set; }

    /// <summary>
    /// Optional sort order for this encoding channel.
    /// </summary>
    public SortOrder? Sort { get; set; }

    /// <summary>
    /// Optional bin configuration for histogram-like binning on this channel.
    /// </summary>
    public int? BinCount { get; set; }

    /// <summary>
    /// Optional time unit for temporal fields (year, month, day, hour, etc.).
    /// Used to extract a component of a date/time value.
    /// </summary>
    public TimeUnit? TimeUnit { get; set; }

    /// <summary>
    /// Whether to stack values on this channel (for stacked bars/areas).
    /// </summary>
    public bool Stack { get; set; }

    /// <summary>
    /// Whether to normalize stacked values to 0-1 (percent stacking).
    /// Only meaningful when Stack is true.
    /// </summary>
    public bool Normalize { get; set; }

    /// <summary>
    /// Whether to sort stacked values by size (for stream graphs).
    /// </summary>
    public bool SortBySize { get; set; }

    /// <summary>
    /// Proportion threshold below which slices are grouped into "Other".
    /// Value between 0 and 1 (e.g., 0.05 = group slices under 5%).
    /// </summary>
    public double? Threshold { get; set; }

    /// <summary>
    /// Custom formatter for displayed values on this channel.
    /// </summary>
    public Func<object, string>? Formatter { get; set; }

    /// <summary>
    /// Format string for values on this channel.
    /// </summary>
    public string? Format { get; set; }

    /// <summary>
    /// Legend configuration for this encoding channel.
    /// When null, no legend is shown for this channel.
    /// </summary>
    public bool ShowLegend { get; set; } = true;

    /// <summary>
    /// Scale configuration override for this channel.
    /// When null, the scale is auto-determined from data type.
    /// </summary>
    public ScaleConfig? Scale { get; set; }
}

/// <summary>
/// Time unit extraction for temporal fields.
/// </summary>
public enum TimeUnit
{
    Year,
    Quarter,
    Month,
    Week,
    Day,
    DayOfWeek,
    Hour,
    Minute,
    Second
}

/// <summary>
/// Sort direction.
/// </summary>
public enum SortOrder
{
    /// <summary>Ascending order (A-Z, 0-9).</summary>
    Ascending,

    /// <summary>Descending order (Z-A, 9-0).</summary>
    Descending,

    /// <summary>Sort by the data field value.</summary>
    ByValue,

    /// <summary>Sort by the encoding channel value (e.g., by Y for X sorting).</summary>
    ByChannel
}

/// <summary>
/// Aggregate function type for data summarization.
/// </summary>
public enum AggregateType
{
    /// <summary>Sum of values.</summary>
    Sum,

    /// <summary>Mean (average) of values.</summary>
    Mean,

    /// <summary>Median (50th percentile) of values.</summary>
    Median,

    /// <summary>Count of data points.</summary>
    Count,

    /// <summary>Minimum value.</summary>
    Min,

    /// <summary>Maximum value.</summary>
    Max,

    /// <summary>Standard deviation.</summary>
    StdDev,

    /// <summary>Variance.</summary>
    Variance,

    /// <summary>First value in the group.</summary>
    First,

    /// <summary>Last value in the group.</summary>
    Last,

    /// <summary>Distinct count of unique values.</summary>
    Distinct
}

/// <summary>
/// Shape type for the Shape encoding channel.
/// </summary>
public enum PointShape
{
    Circle,
    Square,
    Triangle,
    Diamond,
    Cross,
    Hexagon,
    Bowtie,
    Pentagon,
    Star,
    Wye,
    TriangleDown
}

/// <summary>
/// Collection of encoding specifications for a chart mark.
/// </summary>
public class EncodingCollection
{
    private readonly Dictionary<EncodingChannel, EncodingSpec> _encodings = new();

    /// <summary>
    /// Get or set an encoding for a specific channel.
    /// </summary>
    public EncodingSpec? this[EncodingChannel channel]
    {
        get => _encodings.TryGetValue(channel, out var spec) ? spec : null;
        set
        {
            if (value != null)
                _encodings[channel] = value;
            else
                _encodings.Remove(channel);
        }
    }

    /// <summary>
    /// Add an encoding that maps a data field to a channel.
    /// </summary>
    public EncodingCollection Set(EncodingChannel channel, string field, Action<EncodingSpec>? configure = null)
    {
        var spec = new EncodingSpec { Field = field };
        configure?.Invoke(spec);
        _encodings[channel] = spec;
        return this;
    }

    /// <summary>
    /// Get all configured encodings.
    /// </summary>
    public IReadOnlyDictionary<EncodingChannel, EncodingSpec> All => _encodings;

    /// <summary>
    /// Whether this collection has an encoding for the given channel.
    /// </summary>
    public bool Has(EncodingChannel channel) => _encodings.ContainsKey(channel);

    /// <summary>
    /// Remove an encoding for the given channel.
    /// </summary>
    public bool Remove(EncodingChannel channel) => _encodings.Remove(channel);
}
