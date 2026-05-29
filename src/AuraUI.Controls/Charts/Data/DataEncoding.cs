using Avalonia.Media;

namespace AuraUI.Controls.Charts.Data;

/// <summary>
/// Maps data fields to visual properties (channels) for chart rendering.
/// Supports X, Y, Color, Size, Shape, Opacity, Text, and conditional encodings.
///
/// Usage:
/// <code>
/// var encoding = new DataEncoding();
/// encoding.X("month");
/// encoding.Y("sales");
/// encoding.Color("region");
/// encoding.Size("profit", range: (2, 20));
/// </code>
/// </summary>
public class DataEncoding
{
    private readonly Dictionary<VisualChannel, ChannelEncoding> _encodings = new();

    // ────────────────────────────────────────────────
    //  Fluent configuration methods
    // ────────────────────────────────────────────────

    /// <summary>
    /// Map a field to the X position channel.
    /// </summary>
    public DataEncoding X(string field, Action<ChannelEncoding>? configure = null)
    {
        return Set(VisualChannel.X, field, configure);
    }

    /// <summary>
    /// Map a field to the Y position channel.
    /// </summary>
    public DataEncoding Y(string field, Action<ChannelEncoding>? configure = null)
    {
        return Set(VisualChannel.Y, field, configure);
    }

    /// <summary>
    /// Map a field to the color channel.
    /// </summary>
    public DataEncoding Color(string field, Action<ChannelEncoding>? configure = null)
    {
        return Set(VisualChannel.Color, field, configure);
    }

    /// <summary>
    /// Map a field to the size channel.
    /// </summary>
    /// <param name="field">The data field to map.</param>
    /// <param name="min">Minimum visual size (pixels).</param>
    /// <param name="max">Maximum visual size (pixels).</param>
    public DataEncoding Size(string field, double min = 4, double max = 40, Action<ChannelEncoding>? configure = null)
    {
        var enc = Set(VisualChannel.Size, field, configure);
        var channel = _encodings[VisualChannel.Size];
        channel.RangeMin = min;
        channel.RangeMax = max;
        return enc;
    }

    /// <summary>
    /// Map a field to the shape channel (for scatter/markers).
    /// </summary>
    public DataEncoding Shape(string field, Action<ChannelEncoding>? configure = null)
    {
        return Set(VisualChannel.Shape, field, configure);
    }

    /// <summary>
    /// Map a field to the opacity channel.
    /// </summary>
    /// <param name="field">The data field to map.</param>
    /// <param name="min">Minimum opacity (0-1).</param>
    /// <param name="max">Maximum opacity (0-1).</param>
    public DataEncoding Opacity(string field, double min = 0.2, double max = 1.0, Action<ChannelEncoding>? configure = null)
    {
        var enc = Set(VisualChannel.Opacity, field, configure);
        var channel = _encodings[VisualChannel.Opacity];
        channel.RangeMin = min;
        channel.RangeMax = max;
        return enc;
    }

    /// <summary>
    /// Map a field to the text label channel.
    /// </summary>
    public DataEncoding Text(string field, Action<ChannelEncoding>? configure = null)
    {
        return Set(VisualChannel.Text, field, configure);
    }

    /// <summary>
    /// Map a field to the tooltip channel (additional tooltip info).
    /// </summary>
    public DataEncoding Tooltip(string field, Action<ChannelEncoding>? configure = null)
    {
        return Set(VisualChannel.Tooltip, field, configure);
    }

    /// <summary>
    /// Map a field to the angle channel (for polar/theta charts).
    /// </summary>
    public DataEncoding Angle(string field, Action<ChannelEncoding>? configure = null)
    {
        return Set(VisualChannel.Angle, field, configure);
    }

    /// <summary>
    /// Map a field to the radius channel (for polar charts).
    /// </summary>
    public DataEncoding Radius(string field, Action<ChannelEncoding>? configure = null)
    {
        return Set(VisualChannel.Radius, field, configure);
    }

    // ────────────────────────────────────────────────
    //  Core access
    // ────────────────────────────────────────────────

    /// <summary>
    /// Get or set encoding for a visual channel.
    /// </summary>
    public ChannelEncoding? this[VisualChannel channel]
    {
        get => _encodings.TryGetValue(channel, out var enc) ? enc : null;
        set
        {
            if (value != null)
                _encodings[channel] = value;
            else
                _encodings.Remove(channel);
        }
    }

    /// <summary>
    /// All configured encodings.
    /// </summary>
    public IReadOnlyDictionary<VisualChannel, ChannelEncoding> All => _encodings;

    /// <summary>
    /// Whether an encoding is configured for the given channel.
    /// </summary>
    public bool Has(VisualChannel channel) => _encodings.ContainsKey(channel);

    /// <summary>
    /// Get the field name for a channel.
    /// </summary>
    public string? GetField(VisualChannel channel)
    {
        return _encodings.TryGetValue(channel, out var enc) ? enc.Field : null;
    }

    // ────────────────────────────────────────────────
    //  Value resolution
    // ────────────────────────────────────────────────

    /// <summary>
    /// Resolve the X value for a data row.
    /// </summary>
    public double ResolveX(DataRow row)
    {
        var enc = this[VisualChannel.X];
        return enc != null ? row.GetDouble(enc.Field) : 0;
    }

    /// <summary>
    /// Resolve the Y value for a data row.
    /// </summary>
    public double ResolveY(DataRow row)
    {
        var enc = this[VisualChannel.Y];
        return enc != null ? row.GetDouble(enc.Field) : 0;
    }

    /// <summary>
    /// Resolve the color for a data row.
    /// </summary>
    public IBrush? ResolveColor(DataRow row, IBrush[]? palette = null)
    {
        var enc = this[VisualChannel.Color];
        if (enc == null) return null;

        // Check conditional encodings first
        if (enc.Conditionals.Count > 0)
        {
            var val = row.GetDouble(enc.Field);
            foreach (var cond in enc.Conditionals)
            {
                if (cond.Matches(val))
                    return cond.Brush;
            }
        }

        // Check discrete color mapping
        if (enc.DiscreteColors != null)
        {
            var key = row.GetString(enc.Field);
            if (enc.DiscreteColors.TryGetValue(key, out var brush))
                return brush;
        }

        // Auto-assign from palette based on category index
        if (palette != null && palette.Length > 0)
        {
            var key = row.GetString(enc.Field);
            var index = Math.Abs(key.GetHashCode()) % palette.Length;
            return palette[index];
        }

        return null;
    }

    /// <summary>
    /// Resolve the size for a data row.
    /// </summary>
    public double ResolveSize(DataRow row, double defaultSize = 6)
    {
        var enc = this[VisualChannel.Size];
        if (enc == null) return defaultSize;

        var val = row.GetDouble(enc.Field);
        var min = enc.RangeMin ?? 4;
        var max = enc.RangeMax ?? 40;

        if (enc.DomainMin.HasValue && enc.DomainMax.HasValue)
        {
            var range = enc.DomainMax.Value - enc.DomainMin.Value;
            if (Math.Abs(range) > 1e-10)
            {
                var t = (val - enc.DomainMin.Value) / range;
                return min + t * (max - min);
            }
        }

        return defaultSize;
    }

    /// <summary>
    /// Resolve the opacity for a data row.
    /// </summary>
    public double ResolveOpacity(DataRow row, double defaultOpacity = 1.0)
    {
        var enc = this[VisualChannel.Opacity];
        if (enc == null) return defaultOpacity;

        var val = row.GetDouble(enc.Field);
        var min = enc.RangeMin ?? 0.2;
        var max = enc.RangeMax ?? 1.0;

        if (enc.DomainMin.HasValue && enc.DomainMax.HasValue)
        {
            var range = enc.DomainMax.Value - enc.DomainMin.Value;
            if (Math.Abs(range) > 1e-10)
            {
                var t = (val - enc.DomainMin.Value) / range;
                return min + t * (max - min);
            }
        }

        return defaultOpacity;
    }

    /// <summary>
    /// Resolve the text label for a data row.
    /// </summary>
    public string? ResolveText(DataRow row)
    {
        var enc = this[VisualChannel.Text];
        if (enc == null) return null;

        var val = row[enc.Field];
        if (enc.Formatter != null)
            return enc.Formatter(val);
        return val?.ToString();
    }

    /// <summary>
    /// Resolve the shape for a data row.
    /// </summary>
    public string? ResolveShape(DataRow row)
    {
        var enc = this[VisualChannel.Shape];
        if (enc == null) return null;

        if (enc.DiscreteShapes != null)
        {
            var key = row.GetString(enc.Field);
            return enc.DiscreteShapes.TryGetValue(key, out var shape) ? shape : enc.DefaultShape;
        }

        return enc.DefaultShape;
    }

    // ────────────────────────────────────────────────
    //  Private helpers
    // ────────────────────────────────────────────────

    private DataEncoding Set(VisualChannel channel, string field, Action<ChannelEncoding>? configure)
    {
        var enc = new ChannelEncoding { Field = field };
        configure?.Invoke(enc);
        _encodings[channel] = enc;
        return this;
    }
}

/// <summary>
/// Visual channels that data can be mapped to.
/// </summary>
public enum VisualChannel
{
    /// <summary>X position (horizontal axis).</summary>
    X,

    /// <summary>Y position (vertical axis).</summary>
    Y,

    /// <summary>Color encoding.</summary>
    Color,

    /// <summary>Size encoding (marker radius, bar width, etc.).</summary>
    Size,

    /// <summary>Shape encoding (marker shape).</summary>
    Shape,

    /// <summary>Opacity encoding.</summary>
    Opacity,

    /// <summary>Text label encoding.</summary>
    Text,

    /// <summary>Tooltip content encoding.</summary>
    Tooltip,

    /// <summary>Angle encoding (for polar charts).</summary>
    Angle,

    /// <summary>Radius encoding (for polar charts).</summary>
    Radius
}

/// <summary>
/// Configuration for a single visual channel encoding.
/// </summary>
public class ChannelEncoding
{
    /// <summary>The data field to map.</summary>
    public string Field { get; set; } = string.Empty;

    /// <summary>Display title for this channel.</summary>
    public string? Title { get; set; }

    /// <summary>Format string for values.</summary>
    public string? Format { get; set; }

    /// <summary>Custom formatter function.</summary>
    public Func<object?, string>? Formatter { get; set; }

    /// <summary>Domain minimum for continuous mapping. When null, auto-computed.</summary>
    public double? DomainMin { get; set; }

    /// <summary>Domain maximum for continuous mapping. When null, auto-computed.</summary>
    public double? DomainMax { get; set; }

    /// <summary>Range minimum for continuous mapping (e.g., min size, min opacity).</summary>
    public double? RangeMin { get; set; }

    /// <summary>Range maximum for continuous mapping.</summary>
    public double? RangeMax { get; set; }

    /// <summary>
    /// Discrete color mapping: maps category values to specific brushes.
    /// </summary>
    public Dictionary<string, IBrush>? DiscreteColors { get; set; }

    /// <summary>
    /// Discrete shape mapping: maps category values to shape names.
    /// </summary>
    public Dictionary<string, string>? DiscreteShapes { get; set; }

    /// <summary>
    /// Default shape when no discrete mapping matches.
    /// </summary>
    public string? DefaultShape { get; set; }

    /// <summary>
    /// Palette for auto-assigning colors to categories.
    /// </summary>
    public IBrush[]? Palette { get; set; }

    /// <summary>
    /// Conditional encodings: apply different styles based on value ranges.
    /// </summary>
    public List<ConditionalEncoding> Conditionals { get; } = new();

    /// <summary>
    /// Whether to stack values on this channel.
    /// </summary>
    public bool Stack { get; set; }

    /// <summary>
    /// Whether to normalize stacked values.
    /// </summary>
    public bool Normalize { get; set; }

    /// <summary>
    /// Add a conditional encoding for a value range.
    /// </summary>
    public ChannelEncoding When(double min, double max, IBrush brush)
    {
        Conditionals.Add(new ConditionalEncoding { Min = min, Max = max, Brush = brush });
        return this;
    }

    /// <summary>
    /// Add a conditional encoding for a value threshold.
    /// </summary>
    public ChannelEncoding WhenAbove(double threshold, IBrush brush)
    {
        Conditionals.Add(new ConditionalEncoding { Min = threshold, Max = double.MaxValue, Brush = brush });
        return this;
    }

    /// <summary>
    /// Add a conditional encoding for values below a threshold.
    /// </summary>
    public ChannelEncoding WhenBelow(double threshold, IBrush brush)
    {
        Conditionals.Add(new ConditionalEncoding { Min = double.MinValue, Max = threshold, Brush = brush });
        return this;
    }

    /// <summary>
    /// Set a discrete color for a category value.
    /// </summary>
    public ChannelEncoding MapColor(string category, IBrush brush)
    {
        DiscreteColors ??= new Dictionary<string, IBrush>(StringComparer.OrdinalIgnoreCase);
        DiscreteColors[category] = brush;
        return this;
    }

    /// <summary>
    /// Set a discrete shape for a category value.
    /// </summary>
    public ChannelEncoding MapShape(string category, string shape)
    {
        DiscreteShapes ??= new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        DiscreteShapes[category] = shape;
        return this;
    }
}

/// <summary>
/// A conditional encoding that applies a style when a value falls within a range.
/// </summary>
public class ConditionalEncoding
{
    /// <summary>Minimum value (inclusive) for this condition.</summary>
    public double Min { get; set; } = double.MinValue;

    /// <summary>Maximum value (inclusive) for this condition.</summary>
    public double Max { get; set; } = double.MaxValue;

    /// <summary>Brush to apply when the condition matches.</summary>
    public IBrush Brush { get; set; } = Brushes.Transparent;

    /// <summary>Shape to apply when the condition matches (optional).</summary>
    public string? Shape { get; set; }

    /// <summary>Whether the given value falls within this condition's range.</summary>
    public bool Matches(double value) => value >= Min && value <= Max;
}
