using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace AuraUI.Core.Converters;

/// <summary>
/// Converts a string to a visibility boolean.
/// Non-empty strings become true (Visible), null/empty/whitespace becomes false (Collapsed).
/// Supports "invert" parameter.
/// </summary>
public class EmptyStringToVisibilityConverter : IValueConverter
{
    public static EmptyStringToVisibilityConverter Instance { get; } = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        bool invert = string.Equals(parameter as string, "invert", StringComparison.OrdinalIgnoreCase);
        bool isEmpty = value is null || (value is string s && string.IsNullOrWhiteSpace(s));

        if (invert)
            return isEmpty;

        return !isEmpty;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException("EmptyStringToVisibilityConverter does not support ConvertBack.");
    }
}
