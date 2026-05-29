using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using AuraUI.Core.Localization;

namespace AuraUI.Core.Converters;

/// <summary>
/// A value converter that selects a pluralized word form based on the current culture's
/// plural rules and a numeric count.
/// </summary>
/// <remarks>
/// <para><b>ConverterParameter</b> format: <c>singular|plural</c> (for simple two-form languages)
/// or <c>zero|one|two|few|many|other</c> (for languages with complex rules like Arabic).</para>
/// <para>
/// Examples:
/// <code>
/// &lt;!-- English: "item" / "items" --&gt;
/// &lt;TextBlock Text="{Binding Count, Converter={x:Static converters:PluralizationConverter.Instance},
///                              ConverterParameter=item|items}"/&gt;
///
/// &lt;!-- Arabic: full 6-form --&gt;
/// &lt;TextBlock Text="{Binding Count, Converter={x:Static converters:PluralizationConverter.Instance},
///                              ConverterParameter=عنصر|عنصر|عنصران|...}"/&gt;
/// </code>
/// </para>
/// </remarks>
public class PluralizationConverter : IValueConverter
{
    /// <summary>
    /// Shared singleton instance for XAML bindings.
    /// </summary>
    public static PluralizationConverter Instance { get; } = new();

    /// <inheritdoc />
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null)
            return AvaloniaProperty.UnsetValue;

        int count;
        switch (value)
        {
            case int i: count = i; break;
            case long l: count = (int)l; break;
            case double d: count = (int)d; break;
            case float f: count = (int)f; break;
            default: return AvaloniaProperty.UnsetValue;
        }

        var param = parameter as string;
        if (string.IsNullOrEmpty(param))
            return AvaloniaProperty.UnsetValue;

        var parts = param.Split('|');
        var targetCulture = LocalizationManager.Instance.CurrentCulture;

        // Two forms: singular|plural
        if (parts.Length == 2)
        {
            return PluralRules.Pluralize(parts[0], parts[1], count, targetCulture);
        }

        // Full CLDR forms: zero|one|two|few|many|other
        if (parts.Length >= 6)
        {
            return PluralRules.Select(
                parts[0], parts[1], parts[2], parts[3], parts[4], parts[5],
                count, targetCulture);
        }

        return AvaloniaProperty.UnsetValue;
    }

    /// <inheritdoc />
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return AvaloniaProperty.UnsetValue;
    }
}
