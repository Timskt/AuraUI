using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace AuraUI.Core.Converters;

/// <summary>
/// A multi-value converter that performs arithmetic operations (+, -, *, /, %)
/// on two numeric values. The operation is specified via the converter parameter.
/// </summary>
/// <example>
/// <code>
/// &lt;MultiBinding Converter="{x:Static converters:MathConverter.Instance}" ConverterParameter="+"&gt;
///     &lt;Binding Path="Width"/&gt;
///     &lt;Binding Path="Offset"/&gt;
/// &lt;/MultiBinding&gt;
/// </code>
/// </example>
public class MathConverter : IMultiValueConverter
{
    /// <summary>
    /// Singleton instance of the <see cref="MathConverter"/>.
    /// </summary>
    public static MathConverter Instance { get; } = new();

    /// <summary>
    /// Converts two bound values using the specified arithmetic operation.
    /// </summary>
    /// <param name="values">The bound values (at least two numeric values expected).</param>
    /// <param name="targetType">The target type for the result.</param>
    /// <param name="parameter">The arithmetic operation as a string: "+", "-", "*", "/", or "%".</param>
    /// <param name="culture">The culture info.</param>
    /// <returns>The computed result, or <see cref="AvaloniaProperty.UnsetValue"/> on failure.</returns>
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count < 2)
            return AvaloniaProperty.UnsetValue;

        if (!TryGetDouble(values[0], out double left) || !TryGetDouble(values[1], out double right))
            return AvaloniaProperty.UnsetValue;

        string operation = (parameter as string ?? "+").Trim();

        double result = operation switch
        {
            "+" => left + right,
            "-" => left - right,
            "*" => left * right,
            "/" => right != 0 ? left / right : double.NaN,
            "%" => right != 0 ? left % right : double.NaN,
            _ => AvaloniaProperty.UnsetValue is double d ? d : double.NaN
        };

        if (double.IsNaN(result))
            return AvaloniaProperty.UnsetValue;

        if (targetType == typeof(int) || targetType == typeof(int?))
            return (int)Math.Round(result);

        if (targetType == typeof(float) || targetType == typeof(float?))
            return (float)result;

        if (targetType == typeof(decimal) || targetType == typeof(decimal?))
            return (decimal)result;

        return result;
    }

    private static bool TryGetDouble(object? value, out double result)
    {
        switch (value)
        {
            case double d:
                result = d;
                return true;
            case float f:
                result = f;
                return true;
            case int i:
                result = i;
                return true;
            case long l:
                result = l;
                return true;
            case decimal dec:
                result = (double)dec;
                return true;
            case string s when double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsed):
                result = parsed;
                return true;
            default:
                result = 0;
                return false;
        }
    }
}
