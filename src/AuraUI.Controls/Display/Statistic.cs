using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Media;

namespace AuraUI.Controls.Display;

/// <summary>
/// Displays a prominent numerical value with optional title, prefix, suffix,
/// and value style, inspired by Ant Design's Statistic component.
/// </summary>
public class Statistic : TemplatedControl
{
    /// <summary>
    /// Defines the <see cref="Title"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<Statistic, string?>(nameof(Title));

    /// <summary>
    /// Defines the <see cref="Value"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> ValueProperty =
        AvaloniaProperty.Register<Statistic, double>(nameof(Value));

    /// <summary>
    /// Defines the <see cref="ValueText"/> styled property.
    /// Alternative string value (overrides numeric Value when set).
    /// </summary>
    public static readonly StyledProperty<string?> ValueTextProperty =
        AvaloniaProperty.Register<Statistic, string?>(nameof(ValueText));

    /// <summary>
    /// Defines the <see cref="Prefix"/> styled property.
    /// Content displayed before the value.
    /// </summary>
    public static readonly StyledProperty<object?> PrefixProperty =
        AvaloniaProperty.Register<Statistic, object?>(nameof(Prefix));

    /// <summary>
    /// Defines the <see cref="Suffix"/> styled property.
    /// Content displayed after the value.
    /// </summary>
    public static readonly StyledProperty<object?> SuffixProperty =
        AvaloniaProperty.Register<Statistic, object?>(nameof(Suffix));

    /// <summary>
    /// Defines the <see cref="ValueStyle"/> styled property.
    /// </summary>
    public static readonly StyledProperty<StatisticValueStyle> ValueStyleProperty =
        AvaloniaProperty.Register<Statistic, StatisticValueStyle>(nameof(ValueStyle), StatisticValueStyle.Default);

    /// <summary>
    /// Defines the <see cref="DecimalSeparator"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string> DecimalSeparatorProperty =
        AvaloniaProperty.Register<Statistic, string>(nameof(DecimalSeparator), ".");

    /// <summary>
    /// Defines the <see cref="GroupSeparator"/> styled property.
    /// The thousands separator character.
    /// </summary>
    public static readonly StyledProperty<string> GroupSeparatorProperty =
        AvaloniaProperty.Register<Statistic, string>(nameof(GroupSeparator), ",");

    /// <summary>
    /// Defines the <see cref="Precision"/> styled property.
    /// Number of decimal places.
    /// </summary>
    public static readonly StyledProperty<int> PrecisionProperty =
        AvaloniaProperty.Register<Statistic, int>(nameof(Precision), -1);

    /// <summary>
    /// Defines the <see cref="PrefixTemplate"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IDataTemplate?> PrefixTemplateProperty =
        AvaloniaProperty.Register<Statistic, IDataTemplate?>(nameof(PrefixTemplate));

    /// <summary>
    /// Defines the <see cref="SuffixTemplate"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IDataTemplate?> SuffixTemplateProperty =
        AvaloniaProperty.Register<Statistic, IDataTemplate?>(nameof(SuffixTemplate));

    /// <summary>
    /// Gets or sets the title text.
    /// </summary>
    public string? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <summary>
    /// Gets or sets the numeric value.
    /// </summary>
    public double Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    /// <summary>
    /// Gets or sets an alternative string representation of the value.
    /// When set, this takes precedence over the numeric <see cref="Value"/>.
    /// </summary>
    public string? ValueText
    {
        get => GetValue(ValueTextProperty);
        set => SetValue(ValueTextProperty, value);
    }

    /// <summary>
    /// Gets or sets the prefix content (displayed before the value).
    /// </summary>
    public object? Prefix
    {
        get => GetValue(PrefixProperty);
        set => SetValue(PrefixProperty, value);
    }

    /// <summary>
    /// Gets or sets the suffix content (displayed after the value).
    /// </summary>
    public object? Suffix
    {
        get => GetValue(SuffixProperty);
        set => SetValue(SuffixProperty, value);
    }

    /// <summary>
    /// Gets or sets the visual style of the value.
    /// </summary>
    public StatisticValueStyle ValueStyle
    {
        get => GetValue(ValueStyleProperty);
        set => SetValue(ValueStyleProperty, value);
    }

    /// <summary>
    /// Gets or sets the decimal separator character.
    /// </summary>
    public string DecimalSeparator
    {
        get => GetValue(DecimalSeparatorProperty);
        set => SetValue(DecimalSeparatorProperty, value);
    }

    /// <summary>
    /// Gets or sets the group (thousands) separator.
    /// </summary>
    public string GroupSeparator
    {
        get => GetValue(GroupSeparatorProperty);
        set => SetValue(GroupSeparatorProperty, value);
    }

    /// <summary>
    /// Gets or sets the number of decimal places. -1 means auto.
    /// </summary>
    public int Precision
    {
        get => GetValue(PrecisionProperty);
        set => SetValue(PrecisionProperty, value);
    }

    /// <summary>
    /// Gets or sets the data template for the prefix.
    /// </summary>
    public IDataTemplate? PrefixTemplate
    {
        get => GetValue(PrefixTemplateProperty);
        set => SetValue(PrefixTemplateProperty, value);
    }

    /// <summary>
    /// Gets or sets the data template for the suffix.
    /// </summary>
    public IDataTemplate? SuffixTemplate
    {
        get => GetValue(SuffixTemplateProperty);
        set => SetValue(SuffixTemplateProperty, value);
    }

    /// <summary>
    /// Gets the formatted display string.
    /// </summary>
    public string FormattedValue
    {
        get
        {
            if (ValueText != null)
                return ValueText;

            var precision = Precision;
            var format = precision >= 0 ? $"F{precision}" : "G";
            var raw = Value.ToString(format, System.Globalization.CultureInfo.InvariantCulture);

            // Apply custom separators
            var parts = raw.Split('.');
            var intPart = parts[0];

            // Add group separators
            if (GroupSeparator.Length > 0 && intPart.Length > 3)
            {
                var negative = intPart.StartsWith('-');
                if (negative) intPart = intPart[1..];

                var result = new System.Text.StringBuilder();
                var count = 0;
                for (var i = intPart.Length - 1; i >= 0; i--)
                {
                    if (count > 0 && count % 3 == 0)
                        result.Insert(0, GroupSeparator);
                    result.Insert(0, intPart[i]);
                    count++;
                }
                intPart = (negative ? "-" : "") + result.ToString();
            }

            if (parts.Length > 1)
                return intPart + DecimalSeparator + parts[1];
            return intPart;
        }
    }
}

/// <summary>
/// Visual style variants for the statistic value.
/// </summary>
public enum StatisticValueStyle
{
    /// <summary>Default styling.</summary>
    Default,
    /// <summary>Large display value.</summary>
    Large,
    /// <summary>Small caption value.</summary>
    Small
}
