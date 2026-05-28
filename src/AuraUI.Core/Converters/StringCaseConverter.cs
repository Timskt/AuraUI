using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace AuraUI.Core.Converters;

public class StringCaseConverter : IValueConverter
{
    public static StringCaseConverter Instance { get; } = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string text)
            return AvaloniaProperty.UnsetValue;

        string mode = (parameter as string ?? "lower").ToLowerInvariant();

        return mode switch
        {
            "upper" => text.ToUpper(culture),
            "lower" => text.ToLower(culture),
            "title" => ToTitleCase(text, culture),
            _ => text
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException("StringCaseConverter does not support ConvertBack.");
    }

    private static string ToTitleCase(string text, CultureInfo culture)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        TextInfo textInfo = culture.TextInfo;
        return textInfo.ToTitleCase(text.ToLower(culture));
    }
}
