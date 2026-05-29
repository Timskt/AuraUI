using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace AuraUI.Core.Converters;

/// <summary>
/// Maps discrete input values to output values via a dictionary.
/// Useful for mapping enum values or status strings to icons, colours, etc.
/// <para>Example: <c>"Loading" -> spinner</c>, <c>"Error" -> errorIcon</c>, <c>"Success" -> checkmark</c>.</para>
/// </summary>
public class SwitchConverter : IValueConverter
{
    /// <summary>
    /// Gets the case dictionary that maps input values to output values.
    /// </summary>
    public Dictionary<object, object?> Cases { get; } = new();

    /// <summary>
    /// Gets or sets the value returned when the input does not match any case.
    /// Defaults to <see cref="AvaloniaProperty.UnsetValue"/>.
    /// </summary>
    public object? DefaultValue { get; set; } = AvaloniaProperty.UnsetValue;

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null)
            return DefaultValue;

        if (Cases.TryGetValue(value, out var result))
            return result;

        // Try string comparison as a fallback for boxed value types.
        if (value is not string)
        {
            foreach (var kvp in Cases)
            {
                if (Equals(kvp.Key, value))
                    return kvp.Value;
            }
        }

        return DefaultValue;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // Reverse lookup: find the key whose value matches.
        foreach (var kvp in Cases)
        {
            if (Equals(kvp.Value, value))
                return kvp.Key;
        }

        return AvaloniaProperty.UnsetValue;
    }
}
