using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using AuraUI.Core.Localization;

namespace AuraUI.Core.Converters;

/// <summary>
/// Converts <see cref="DateTime"/>, <see cref="DateTimeOffset"/>,
/// <see cref="DateOnly"/>, and <see cref="TimeOnly"/> values to formatted strings
/// using the <see cref="LocalizationManager"/>'s current culture.
/// </summary>
/// <remarks>
/// Unlike <see cref="DateTimeFormatConverter"/> which uses the culture from the
/// binding pipeline, this converter always uses the explicitly set current culture
/// from <see cref="LocalizationManager"/>.
/// </remarks>
public class CultureDateTimeConverter : IValueConverter
{
    /// <summary>
    /// Shared singleton instance for XAML bindings.
    /// </summary>
    public static CultureDateTimeConverter Instance { get; } = new();

    private const string DefaultFormat = "g";

    /// <inheritdoc />
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var fmt = parameter as string ?? DefaultFormat;
        var targetCulture = LocalizationManager.Instance.CurrentCulture;

        return value switch
        {
            DateTime dt => dt.ToString(fmt, targetCulture),
            DateTimeOffset dto => dto.ToString(fmt, targetCulture),
            DateOnly d => d.ToString(fmt, targetCulture),
            TimeOnly t => t.ToString(fmt, targetCulture),
            _ => AvaloniaProperty.UnsetValue
        };
    }

    /// <inheritdoc />
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string text)
            return AvaloniaProperty.UnsetValue;

        var fmt = parameter as string ?? DefaultFormat;
        var targetCulture = LocalizationManager.Instance.CurrentCulture;

        if (targetType == typeof(DateTime) || targetType == typeof(DateTime?))
        {
            if (DateTime.TryParseExact(text, fmt, targetCulture, DateTimeStyles.None, out var dt))
                return dt;
            if (DateTime.TryParse(text, targetCulture, DateTimeStyles.None, out dt))
                return dt;
        }

        if (targetType == typeof(DateTimeOffset) || targetType == typeof(DateTimeOffset?))
        {
            if (DateTimeOffset.TryParseExact(text, fmt, targetCulture, DateTimeStyles.None, out var dto))
                return dto;
            if (DateTimeOffset.TryParse(text, targetCulture, DateTimeStyles.None, out dto))
                return dto;
        }

        return AvaloniaProperty.UnsetValue;
    }
}
