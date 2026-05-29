using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace AuraUI.Core.Converters;

/// <summary>
/// Converts a <see cref="Color"/> or color string (e.g. <c>"#FF0000"</c>) to a
/// <see cref="SolidColorBrush"/> and back. Use the static <see cref="Instance"/>
/// property for XAML bindings.
/// </summary>
public class ColorToBrushConverter : IValueConverter
{
    public static ColorToBrushConverter Instance { get; } = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is Color color)
            return new SolidColorBrush(color);

        if (value is string colorString)
        {
            try
            {
                var parsed = Color.Parse(colorString);
                return new SolidColorBrush(parsed);
            }
            catch
            {
                return AvaloniaProperty.UnsetValue;
            }
        }

        return AvaloniaProperty.UnsetValue;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is SolidColorBrush brush)
            return brush.Color;

        return AvaloniaProperty.UnsetValue;
    }
}
