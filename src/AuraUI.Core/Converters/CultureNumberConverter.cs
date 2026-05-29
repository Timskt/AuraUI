using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using AuraUI.Core.Localization;

namespace AuraUI.Core.Converters;

/// <summary>
/// Formats numeric values using the <see cref="LocalizationManager"/>'s current culture.
/// Supports <see cref="int"/>, <see cref="long"/>, <see cref="double"/>,
/// <see cref="float"/>, and <see cref="decimal"/>.
/// </summary>
/// <example>
/// <code>
/// &lt;!-- Two decimal places --&gt;
/// &lt;TextBlock Text="{Binding Price, Converter={x:Static converters:CultureNumberConverter.Instance},
///                              ConverterParameter=N2}"/&gt;
/// </code>
/// </example>
public class CultureNumberConverter : IValueConverter
{
    /// <summary>
    /// Shared singleton instance for XAML bindings.
    /// </summary>
    public static CultureNumberConverter Instance { get; } = new();

    /// <inheritdoc />
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var format = parameter as string;
        var targetCulture = LocalizationManager.Instance.CurrentCulture;

        return value switch
        {
            int i => i.ToString(format, targetCulture),
            long l => l.ToString(format, targetCulture),
            double d => d.ToString(format, targetCulture),
            float f => f.ToString(format, targetCulture),
            decimal m => m.ToString(format, targetCulture),
            short s => s.ToString(format, targetCulture),
            _ => AvaloniaProperty.UnsetValue
        };
    }

    /// <inheritdoc />
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string text)
            return AvaloniaProperty.UnsetValue;

        var targetCulture = LocalizationManager.Instance.CurrentCulture;

        if (targetType == typeof(int) || targetType == typeof(int?))
            return int.TryParse(text, NumberStyles.Any, targetCulture, out var i) ? i : AvaloniaProperty.UnsetValue;
        if (targetType == typeof(long) || targetType == typeof(long?))
            return long.TryParse(text, NumberStyles.Any, targetCulture, out var l) ? l : AvaloniaProperty.UnsetValue;
        if (targetType == typeof(double) || targetType == typeof(double?))
            return double.TryParse(text, NumberStyles.Any, targetCulture, out var d) ? d : AvaloniaProperty.UnsetValue;
        if (targetType == typeof(float) || targetType == typeof(float?))
            return float.TryParse(text, NumberStyles.Any, targetCulture, out var f) ? f : AvaloniaProperty.UnsetValue;
        if (targetType == typeof(decimal) || targetType == typeof(decimal?))
            return decimal.TryParse(text, NumberStyles.Any, targetCulture, out var m) ? m : AvaloniaProperty.UnsetValue;

        return AvaloniaProperty.UnsetValue;
    }
}
