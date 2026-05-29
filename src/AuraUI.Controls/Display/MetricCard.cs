using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace AuraUI.Controls.Display;

/// <summary>
/// Specifies the trend direction for a metric.
/// </summary>
public enum TrendDirection
{
    Up,
    Down,
    Neutral
}

/// <summary>
/// A KPI display card with title, value, unit, trend indicator, sparkline, and status.
/// Designed for dashboards and monitoring views.
/// </summary>
public class MetricCard : TemplatedControl
{
    /// <summary>
    /// Defines the <see cref="CardTitle"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> CardTitleProperty =
        AvaloniaProperty.Register<MetricCard, string?>(nameof(CardTitle));

    /// <summary>
    /// Defines the <see cref="Value"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> ValueProperty =
        AvaloniaProperty.Register<MetricCard, string?>(nameof(Value));

    /// <summary>
    /// Defines the <see cref="Unit"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> UnitProperty =
        AvaloniaProperty.Register<MetricCard, string?>(nameof(Unit));

    /// <summary>
    /// Defines the <see cref="Trend"/> styled property.
    /// </summary>
    public static readonly StyledProperty<TrendDirection> TrendProperty =
        AvaloniaProperty.Register<MetricCard, TrendDirection>(nameof(Trend), TrendDirection.Neutral);

    /// <summary>
    /// Defines the <see cref="TrendValue"/> styled property.
    /// The percentage or absolute change value.
    /// </summary>
    public static readonly StyledProperty<string?> TrendValueProperty =
        AvaloniaProperty.Register<MetricCard, string?>(nameof(TrendValue));

    /// <summary>
    /// Defines the <see cref="SparklineData"/> styled property.
    /// Collection of data points for the sparkline chart.
    /// </summary>
    public static readonly StyledProperty<AvaloniaList<double>?> SparklineDataProperty =
        AvaloniaProperty.Register<MetricCard, AvaloniaList<double>?>(nameof(SparklineData));

    /// <summary>
    /// Defines the <see cref="Icon"/> styled property.
    /// </summary>
    public static readonly StyledProperty<object?> IconProperty =
        AvaloniaProperty.Register<MetricCard, object?>(nameof(Icon));

    /// <summary>
    /// Defines the <see cref="CardStatus"/> styled property.
    /// </summary>
    public static readonly StyledProperty<ServiceStatus> CardStatusProperty =
        AvaloniaProperty.Register<MetricCard, ServiceStatus>(nameof(CardStatus), ServiceStatus.Online);

    /// <summary>
    /// Defines the <see cref="TrendColor"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> TrendColorProperty =
        AvaloniaProperty.Register<MetricCard, IBrush?>(nameof(TrendColor));

    /// <summary>
    /// Defines the <see cref="SparklineStroke"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> SparklineStrokeProperty =
        AvaloniaProperty.Register<MetricCard, IBrush?>(nameof(SparklineStroke));

    /// <summary>
    /// Defines the <see cref="SparklineFill"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> SparklineFillProperty =
        AvaloniaProperty.Register<MetricCard, IBrush?>(nameof(SparklineFill));

    /// <summary>
    /// Gets or sets the title text.
    /// </summary>
    public string? CardTitle
    {
        get => GetValue(CardTitleProperty);
        set => SetValue(CardTitleProperty, value);
    }

    /// <summary>
    /// Gets or sets the main value display string.
    /// </summary>
    public string? Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    /// <summary>
    /// Gets or sets the unit text (e.g., "ms", "%", "req/s").
    /// </summary>
    public string? Unit
    {
        get => GetValue(UnitProperty);
        set => SetValue(UnitProperty, value);
    }

    /// <summary>
    /// Gets or sets the trend direction.
    /// </summary>
    public TrendDirection Trend
    {
        get => GetValue(TrendProperty);
        set => SetValue(TrendProperty, value);
    }

    /// <summary>
    /// Gets or sets the trend value text (e.g., "+12.5%", "-3").
    /// </summary>
    public string? TrendValue
    {
        get => GetValue(TrendValueProperty);
        set => SetValue(TrendValueProperty, value);
    }

    /// <summary>
    /// Gets or sets the sparkline data points.
    /// </summary>
    public AvaloniaList<double>? SparklineData
    {
        get => GetValue(SparklineDataProperty);
        set => SetValue(SparklineDataProperty, value);
    }

    /// <summary>
    /// Gets or sets the icon content.
    /// </summary>
    public object? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    /// <summary>
    /// Gets or sets the status indicator.
    /// </summary>
    public ServiceStatus CardStatus
    {
        get => GetValue(CardStatusProperty);
        set => SetValue(CardStatusProperty, value);
    }

    /// <summary>
    /// Gets or sets the trend indicator color.
    /// </summary>
    public IBrush? TrendColor
    {
        get => GetValue(TrendColorProperty);
        set => SetValue(TrendColorProperty, value);
    }

    /// <summary>
    /// Gets or sets the sparkline stroke brush.
    /// </summary>
    public IBrush? SparklineStroke
    {
        get => GetValue(SparklineStrokeProperty);
        set => SetValue(SparklineStrokeProperty, value);
    }

    /// <summary>
    /// Gets or sets the sparkline area fill brush.
    /// </summary>
    public IBrush? SparklineFill
    {
        get => GetValue(SparklineFillProperty);
        set => SetValue(SparklineFillProperty, value);
    }

    /// <summary>
    /// Gets the trend arrow character.
    /// </summary>
    public string TrendArrow => Trend switch
    {
        TrendDirection.Up => "▲",
        TrendDirection.Down => "▼",
        _ => "•"
    };

    /// <summary>
    /// Gets the computed trend color based on direction when TrendColor is not explicitly set.
    /// </summary>
    public IBrush ComputedTrendColor => TrendColor ?? (Trend switch
    {
        TrendDirection.Up => new SolidColorBrush(Color.Parse("#4CAF50")),
        TrendDirection.Down => new SolidColorBrush(Color.Parse("#F44336")),
        _ => new SolidColorBrush(Color.Parse("#9E9E9E"))
    });
}
