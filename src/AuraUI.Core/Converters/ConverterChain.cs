using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace AuraUI.Core.Converters;

/// <summary>
/// Chains multiple <see cref="IValueConverter"/> instances together.
/// The output of each converter is fed as the input to the next:
/// <c>value -> converter1 -> converter2 -> ... -> result</c>.
/// </summary>
public class ConverterChain : IValueConverter
{
    /// <summary>
    /// Gets the ordered list of converters to apply.
    /// </summary>
    public IList<IValueConverter> Converters { get; } = new List<IValueConverter>();

    /// <summary>
    /// Applies each converter in sequence. If any converter returns
    /// <see cref="AvaloniaProperty.UnsetValue"/>, the chain short-circuits.
    /// </summary>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var result = value;
        foreach (var converter in Converters)
        {
            result = converter.Convert(result, targetType, parameter, culture);
            if (result == AvaloniaProperty.UnsetValue)
                return AvaloniaProperty.UnsetValue;
        }
        return result;
    }

    /// <summary>
    /// Not supported for chained converters.
    /// </summary>
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return AvaloniaProperty.UnsetValue;
    }
}
