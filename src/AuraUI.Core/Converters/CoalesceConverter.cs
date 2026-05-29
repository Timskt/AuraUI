using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace AuraUI.Core.Converters;

/// <summary>
/// A multi-value converter that returns the <b>first non-null</b> value
/// from the input bindings, similar to the SQL <c>COALESCE</c> function.
/// <para>
/// Values equal to <see cref="AvaloniaProperty.UnsetValue"/> are also
/// treated as null and skipped.
/// </para>
/// </summary>
public class CoalesceConverter : IMultiValueConverter
{
    public static CoalesceConverter Instance { get; } = new();

    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values is null)
            return AvaloniaProperty.UnsetValue;

        foreach (var value in values)
        {
            if (value is not null && value != AvaloniaProperty.UnsetValue)
                return value;
        }

        return AvaloniaProperty.UnsetValue;
    }
}
