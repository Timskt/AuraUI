using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Layout;

namespace AuraUI.Core.Localization;

/// <summary>
/// A value converter that flips horizontal values (left/right) when the current
/// culture uses a right-to-left layout. Useful for adjusting margins, paddings,
/// icon placement, and similar directional values.
/// </summary>
/// <remarks>
/// <para>When the current culture is LTR the value is returned as-is.</para>
/// <para>
/// When the current culture is RTL the converter swaps left and right components
/// of <see cref="Thickness"/> values, or negates numeric values.
/// </para>
/// </remarks>
public class RTLConverter : IValueConverter
{
    /// <summary>
    /// Shared singleton instance for XAML bindings.
    /// </summary>
    public static RTLConverter Instance { get; } = new();

    /// <inheritdoc />
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (!FlowDirectionHelper.IsRtlCulture(LocalizationManager.Instance.CurrentCulture))
            return value;

        return value switch
        {
            Thickness t => new Thickness(t.Right, t.Top, t.Left, t.Bottom),
            double d => -d,
            float f => -f,
            int i => -i,
            _ => value
        };
    }

    /// <inheritdoc />
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // Symmetric — just call Convert again.
        return Convert(value, targetType, parameter, culture);
    }
}
