using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace AuraUI.Core.Converters;

/// <summary>
/// A multi-value converter that produces a count-and-word string such as <c>"3 items"</c>.
/// Expects two bindings: the first is the numeric count, the second is the singular form
/// of the word. Pass an explicit plural form as the converter parameter to override the
/// automatic English pluralization. Use the static <see cref="Instance"/> property for
/// XAML bindings.
/// </summary>
public class PluralizeConverter : IMultiValueConverter
{
    public static PluralizeConverter Instance { get; } = new();

    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count < 2)
            return AvaloniaProperty.UnsetValue;

        int count;
        switch (values[0])
        {
            case int i:
                count = i;
                break;
            case long l:
                count = (int)l;
                break;
            case double d:
                count = (int)d;
                break;
            default:
                return AvaloniaProperty.UnsetValue;
        }

        if (values[1] is not string singular)
            return AvaloniaProperty.UnsetValue;

        string plural = parameter as string ?? DefaultPluralize(singular);

        string word = count == 1 ? singular : plural;
        return string.Format(culture, "{0} {1}", count, word);
    }

    private static string DefaultPluralize(string singular)
    {
        if (string.IsNullOrEmpty(singular))
            return singular;

        if (singular.EndsWith("y", StringComparison.OrdinalIgnoreCase) &&
            singular.Length > 1 &&
            !IsVowel(singular[^2]))
        {
            return singular[..^1] + "ies";
        }

        if (singular.EndsWith("s", StringComparison.OrdinalIgnoreCase) ||
            singular.EndsWith("sh", StringComparison.OrdinalIgnoreCase) ||
            singular.EndsWith("ch", StringComparison.OrdinalIgnoreCase) ||
            singular.EndsWith("x", StringComparison.OrdinalIgnoreCase) ||
            singular.EndsWith("z", StringComparison.OrdinalIgnoreCase))
        {
            return singular + "es";
        }

        return singular + "s";
    }

    private static bool IsVowel(char c) =>
        "aeiouAEIOU".IndexOf(c) >= 0;
}
