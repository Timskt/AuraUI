using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace AuraUI.Core.Converters;

public class MathConverter : IMultiValueConverter
{
    public static MathConverter Instance { get; } = new();

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
