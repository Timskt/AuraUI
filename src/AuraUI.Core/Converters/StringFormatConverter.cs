using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace AuraUI.Core.Converters;

/// <summary>
/// Formats a value using <see cref="string.Format(string, object?)"/>.
/// Pass the format string as the converter parameter or via <see cref="Format"/>.
/// <para>Example: <c>StringFormat="{}{0:C2}"</c> for currency.</para>
/// </summary>
public class StringFormatConverter : IValueConverter
{
    public static StringFormatConverter Instance { get; } = new();

    /// <summary>
    /// Gets or sets an optional format string that is used when the
    /// converter parameter is <c>null</c>.
    /// </summary>
    public string? Format { get; set; }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var fmt = parameter as string ?? Format;
        if (string.IsNullOrEmpty(fmt))
            return value?.ToString() ?? string.Empty;

        try
        {
            return string.Format(culture, fmt, value);
        }
        catch (FormatException)
        {
            return value?.ToString() ?? string.Empty;
        }
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return AvaloniaProperty.UnsetValue;
    }
}
