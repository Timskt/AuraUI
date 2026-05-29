using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace AuraUI.Core.Converters;

/// <summary>
/// Converts <see cref="DateTime"/>, <see cref="DateTimeOffset"/>, <see cref="DateOnly"/>,
/// and <see cref="TimeOnly"/> values to formatted strings. Pass a format string as the
/// converter parameter; defaults to <c>"g"</c> (general short). Use the static
/// <see cref="Instance"/> property for XAML bindings.
/// </summary>
public class DateTimeFormatConverter : IValueConverter
{
    public static DateTimeFormatConverter Instance { get; } = new();

    private const string DefaultFormat = "g";

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        string format = parameter as string ?? DefaultFormat;

        switch (value)
        {
            case DateTime dt:
                return dt.ToString(format, culture);
            case DateTimeOffset dto:
                return dto.ToString(format, culture);
            case DateOnly d:
                return d.ToString(format, culture);
            case TimeOnly t:
                return t.ToString(format, culture);
            default:
                return AvaloniaProperty.UnsetValue;
        }
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string text)
            return AvaloniaProperty.UnsetValue;

        string format = parameter as string ?? DefaultFormat;

        if (targetType == typeof(DateTime) || targetType == typeof(DateTime?))
        {
            if (DateTime.TryParseExact(text, format, culture, DateTimeStyles.None, out var dt))
                return dt;
            if (DateTime.TryParse(text, culture, DateTimeStyles.None, out dt))
                return dt;
        }

        if (targetType == typeof(DateTimeOffset) || targetType == typeof(DateTimeOffset?))
        {
            if (DateTimeOffset.TryParseExact(text, format, culture, DateTimeStyles.None, out var dto))
                return dto;
            if (DateTimeOffset.TryParse(text, culture, DateTimeStyles.None, out dto))
                return dto;
        }

        return AvaloniaProperty.UnsetValue;
    }
}
