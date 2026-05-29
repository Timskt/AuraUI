using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace AuraUI.Demo.Converters;

public class BoolToStringConverter : IValueConverter
{
    public string TrueValue { get; set; } = "Yes";
    public string FalseValue { get; set; } = "No";

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is bool b ? (b ? TrueValue : FalseValue) : FalseValue;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value?.ToString() == TrueValue;
    }
}
