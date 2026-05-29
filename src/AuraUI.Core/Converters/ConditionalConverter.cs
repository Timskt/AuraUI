using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace AuraUI.Core.Converters;

/// <summary>
/// A multi-value converter that returns <c>true</c> only when <b>all</b>
/// bound values are <c>true</c>. Useful for enabling a button only when
/// every field in a form is valid.
/// <para>
/// Use with <c>Bind(MultiBinding)</c> and pass multiple boolean bindings.
/// </para>
/// </summary>
public class ConditionalConverter : IMultiValueConverter
{
    public static ConditionalConverter Instance { get; } = new();

    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values is null || values.Count == 0)
            return false;

        foreach (var value in values)
        {
            if (value is bool b)
            {
                if (!b)
                    return false;
            }
            else if (value is null || value == AvaloniaProperty.UnsetValue)
            {
                return false;
            }
            else
            {
                // Non-boolean truthy check: non-null objects are considered true.
            }
        }

        return true;
    }
}
