using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace AuraUI.Core.Converters;

/// <summary>
/// Converts a boolean value to a visibility boolean.
/// Returns true for Visible, false for Collapsed.
/// Supports "invert" parameter to flip the result.
/// </summary>
public class BoolToVisibilityConverter : IValueConverter
{
    public static BoolToVisibilityConverter Instance { get; } = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not bool boolValue)
            return AvaloniaProperty.UnsetValue;

        bool invert = string.Equals(parameter as string, "invert", StringComparison.OrdinalIgnoreCase);
        if (invert)
            boolValue = !boolValue;

        return boolValue;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not bool boolValue)
            return AvaloniaProperty.UnsetValue;

        bool invert = string.Equals(parameter as string, "invert", StringComparison.OrdinalIgnoreCase);
        if (invert)
            boolValue = !boolValue;

        return boolValue;
    }
}
