using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using AuraUI.Core.Localization;

namespace AuraUI.Core.Converters;

/// <summary>
/// Formats numeric values as currency using the <see cref="LocalizationManager"/>'s
/// current culture. The currency symbol, position, and grouping are determined
/// by <see cref="NumberFormatInfo.CurrencySymbol"/> and related properties.
/// </summary>
/// <example>
/// <code>
/// &lt;!-- Default 2 decimal places --&gt;
/// &lt;TextBlock Text="{Binding Total, Converter={x:Static converters:CultureCurrencyConverter.Instance}}"/&gt;
///
/// &lt;!-- Custom decimal places --&gt;
/// &lt;TextBlock Text="{Binding Total, Converter={x:Static converters:CultureCurrencyConverter.Instance},
///                              ConverterParameter=0}"/&gt;
/// </code>
/// </example>
public class CultureCurrencyConverter : IValueConverter
{
    /// <summary>
    /// Shared singleton instance for XAML bindings.
    /// </summary>
    public static CultureCurrencyConverter Instance { get; } = new();

    /// <inheritdoc />
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var targetCulture = LocalizationManager.Instance.CurrentCulture;
        var decimalPlaces = 2;

        if (parameter is string dpStr && int.TryParse(dpStr, out var dp))
            decimalPlaces = dp;
        else if (parameter is int dpInt)
            decimalPlaces = dpInt;

        var format = $"C{decimalPlaces}";

        return value switch
        {
            int i => i.ToString(format, targetCulture),
            long l => l.ToString(format, targetCulture),
            double d => d.ToString(format, targetCulture),
            float f => f.ToString(format, targetCulture),
            decimal m => m.ToString(format, targetCulture),
            _ => AvaloniaProperty.UnsetValue
        };
    }

    /// <inheritdoc />
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string text)
            return AvaloniaProperty.UnsetValue;

        var targetCulture = LocalizationManager.Instance.CurrentCulture;

        if (decimal.TryParse(text, NumberStyles.Currency, targetCulture, out var result))
        {
            if (targetType == typeof(int) || targetType == typeof(int?)) return (int)result;
            if (targetType == typeof(long) || targetType == typeof(long?)) return (long)result;
            if (targetType == typeof(double) || targetType == typeof(double?)) return (double)result;
            if (targetType == typeof(float) || targetType == typeof(float?)) return (float)result;
            if (targetType == typeof(decimal) || targetType == typeof(decimal?)) return result;
        }

        return AvaloniaProperty.UnsetValue;
    }
}
