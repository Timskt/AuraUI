using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace AuraUI.Core.Converters;

/// <summary>
/// Negates a <see cref="bool"/> value: <c>true</c> becomes <c>false</c> and vice versa.
/// Supports both forward and backward conversion. Use the static <see cref="Instance"/>
/// property for XAML bindings.
/// </summary>
public class InverseBoolConverter : IValueConverter
{
    public static InverseBoolConverter Instance { get; } = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
            return !boolValue;

        return AvaloniaProperty.UnsetValue;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
            return !boolValue;

        return AvaloniaProperty.UnsetValue;
    }
}
