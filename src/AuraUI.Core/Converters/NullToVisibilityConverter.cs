using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace AuraUI.Core.Converters;

/// <summary>
/// Converts a value to a visibility boolean.
/// Non-null values become true (Visible), null becomes false (Collapsed).
/// Supports "invert" parameter.
/// </summary>
public class NullToVisibilityConverter : IValueConverter
{
    public static NullToVisibilityConverter Instance { get; } = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        bool invert = string.Equals(parameter as string, "invert", StringComparison.OrdinalIgnoreCase);
        bool isNull = value is null;

        if (invert)
            return isNull;

        return !isNull;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException("NullToVisibilityConverter does not support ConvertBack.");
    }
}
