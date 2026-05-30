using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AuraUI.Controls.Display;

/// <summary>
/// Represents a qualitative range in a bullet chart.
/// </summary>
public class BulletRange
{
    /// <summary>
    /// Gets or sets the start value of the range.
    /// </summary>
    public double Start { get; set; }

    /// <summary>
    /// Gets or sets the end value of the range.
    /// </summary>
    public double End { get; set; }

    /// <summary>
    /// Gets or sets the fill color for this range.
    /// </summary>
    public IBrush? Color { get; set; }
}

/// <summary>
/// A bullet chart control for KPI dashboards. Displays a horizontal bar with
/// qualitative range bands, a value marker, and a target line.
/// </summary>
public class BulletChart : Control
{
    /// <summary>
    /// Defines the <see cref="Value"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> ValueProperty =
        AvaloniaProperty.Register<BulletChart, double>(nameof(Value));

    /// <summary>
    /// Defines the <see cref="Target"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> TargetProperty =
        AvaloniaProperty.Register<BulletChart, double>(nameof(Target));

    /// <summary>
    /// Defines the <see cref="Ranges"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IList<BulletRange>?> RangesProperty =
        AvaloniaProperty.Register<BulletChart, IList<BulletRange>?>(nameof(Ranges));

    /// <summary>
    /// Defines the <see cref="Title"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<BulletChart, string?>(nameof(Title));

    /// <summary>
    /// Defines the <see cref="Unit"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> UnitProperty =
        AvaloniaProperty.Register<BulletChart, string?>(nameof(Unit));

    /// <summary>
    /// Defines the <see cref="Format"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> FormatProperty =
        AvaloniaProperty.Register<BulletChart, string?>(nameof(Format));

    /// <summary>
    /// Defines the <see cref="ValueBrush"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> ValueBrushProperty =
        AvaloniaProperty.Register<BulletChart, IBrush?>(nameof(ValueBrush));

    /// <summary>
    /// Defines the <see cref="TargetBrush"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> TargetBrushProperty =
        AvaloniaProperty.Register<BulletChart, IBrush?>(nameof(TargetBrush));

    /// <summary>
    /// Defines the <see cref="MinValue"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> MinValueProperty =
        AvaloniaProperty.Register<BulletChart, double>(nameof(MinValue), 0.0);

    /// <summary>
    /// Defines the <see cref="MaxValue"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> MaxValueProperty =
        AvaloniaProperty.Register<BulletChart, double>(nameof(MaxValue), 100.0);

    /// <summary>
    /// Defines the <see cref="BarHeightRatio"/> styled property.
    /// Fraction of available height for the bar (0..1).
    /// </summary>
    public static readonly StyledProperty<double> BarHeightRatioProperty =
        AvaloniaProperty.Register<BulletChart, double>(nameof(BarHeightRatio), 0.6);

    // Cached brushes
    private static readonly SolidColorBrush s_defaultRangeBrush1 = new(Color.FromArgb(50, 158, 158, 158));
    private static readonly SolidColorBrush s_defaultRangeBrush2 = new(Color.FromArgb(35, 158, 158, 158));
    private static readonly SolidColorBrush s_defaultRangeBrush3 = new(Color.FromArgb(20, 158, 158, 158));
    private static readonly SolidColorBrush s_defaultValueBrush = new(Color.Parse("#212121"));
    private static readonly SolidColorBrush s_defaultTargetBrush = new(Color.Parse("#F44336"));
    private static readonly Typeface s_labelTypeface = new("Segoe UI", FontStyle.Normal, FontWeight.Normal);

    static BulletChart()
    {
        ValueProperty.Changed.AddClassHandler<BulletChart>((x, _) => x.InvalidateVisual());
        TargetProperty.Changed.AddClassHandler<BulletChart>((x, _) => x.InvalidateVisual());
        RangesProperty.Changed.AddClassHandler<BulletChart>((x, _) => x.InvalidateVisual());
        MinValueProperty.Changed.AddClassHandler<BulletChart>((x, _) => x.InvalidateVisual());
        MaxValueProperty.Changed.AddClassHandler<BulletChart>((x, _) => x.InvalidateVisual());
        TitleProperty.Changed.AddClassHandler<BulletChart>((x, _) => x.InvalidateVisual());
        UnitProperty.Changed.AddClassHandler<BulletChart>((x, _) => x.InvalidateVisual());
        FormatProperty.Changed.AddClassHandler<BulletChart>((x, _) => x.InvalidateVisual());
    }

    /// <summary>
    /// Gets or sets the current value.
    /// </summary>
    public double Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    /// <summary>
    /// Gets or sets the target value.
    /// </summary>
    public double Target
    {
        get => GetValue(TargetProperty);
        set => SetValue(TargetProperty, value);
    }

    /// <summary>
    /// Gets or sets the qualitative range bands.
    /// </summary>
    public IList<BulletRange>? Ranges
    {
        get => GetValue(RangesProperty);
        set => SetValue(RangesProperty, value);
    }

    /// <summary>
    /// Gets or sets the title text.
    /// </summary>
    public string? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <summary>
    /// Gets or sets the unit text (e.g., "%", "ms").
    /// </summary>
    public string? Unit
    {
        get => GetValue(UnitProperty);
        set => SetValue(UnitProperty, value);
    }

    /// <summary>
    /// Gets or sets the value format string.
    /// </summary>
    public string? Format
    {
        get => GetValue(FormatProperty);
        set => SetValue(FormatProperty, value);
    }

    /// <summary>
    /// Gets or sets the value bar brush.
    /// </summary>
    public IBrush? ValueBrush
    {
        get => GetValue(ValueBrushProperty);
        set => SetValue(ValueBrushProperty, value);
    }

    /// <summary>
    /// Gets or sets the target line brush.
    /// </summary>
    public IBrush? TargetBrush
    {
        get => GetValue(TargetBrushProperty);
        set => SetValue(TargetBrushProperty, value);
    }

    /// <summary>
    /// Gets or sets the minimum scale value.
    /// </summary>
    public double MinValue
    {
        get => GetValue(MinValueProperty);
        set => SetValue(MinValueProperty, value);
    }

    /// <summary>
    /// Gets or sets the maximum scale value.
    /// </summary>
    public double MaxValue
    {
        get => GetValue(MaxValueProperty);
        set => SetValue(MaxValueProperty, value);
    }

    /// <summary>
    /// Gets or sets the fraction of available height for the bar (0..1).
    /// </summary>
    public double BarHeightRatio
    {
        get => GetValue(BarHeightRatioProperty);
        set => SetValue(BarHeightRatioProperty, value);
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        var bounds = new Rect(Bounds.Size);
        if (bounds.Width <= 0 || bounds.Height <= 0) return;

        var min = MinValue;
        var max = MaxValue;
        var range = max - min;
        if (range <= 0) return;

        // Layout: [Title] | [Chart Area]
        var titleWidth = 0.0;
        var title = Title;
        if (!string.IsNullOrEmpty(title))
        {
            titleWidth = 80;
        }

        var chartLeft = bounds.X + titleWidth;
        var chartRight = bounds.X + bounds.Width;
        var chartWidth = chartRight - chartLeft;
        if (chartWidth <= 0) return;

        // Draw title
        if (!string.IsNullOrEmpty(title) && titleWidth > 0)
        {
            var labelBrush = Brushes.Gray;
            var formatted = new FormattedText(title, CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                s_labelTypeface, 12, labelBrush);
            var labelY = bounds.Y + (bounds.Height - formatted.Height) / 2;
            context.DrawText(formatted, new Point(bounds.X + 4, labelY));
        }

        // Bar dimensions
        var barHeight = Math.Max(4, bounds.Height * BarHeightRatio);
        var barTop = bounds.Y + (bounds.Height - barHeight) / 2;

        // Draw range bands (background)
        var ranges = Ranges;
        if (ranges != null && ranges.Count > 0)
        {
            for (var i = 0; i < ranges.Count; i++)
            {
                var r = ranges[i];
                var rStart = Math.Max(r.Start, min);
                var rEnd = Math.Min(r.End, max);
                if (rEnd <= rStart) continue;

                var x1 = chartLeft + ((rStart - min) / range) * chartWidth;
                var x2 = chartLeft + ((rEnd - min) / range) * chartWidth;
                var rangeBrush = r.Color ?? GetDefaultRangeBrush(i);
                context.DrawRectangle(rangeBrush, null, new Rect(x1, barTop, x2 - x1, barHeight));
            }
        }
        else
        {
            // Default: three grey bands
            var bandBrushes = new[] { s_defaultRangeBrush1, s_defaultRangeBrush2, s_defaultRangeBrush3 };
            var third = range / 3;
            for (var i = 0; i < 3; i++)
            {
                var x1 = chartLeft + (i * third / range) * chartWidth;
                var x2 = chartLeft + ((i + 1) * third / range) * chartWidth;
                context.DrawRectangle(bandBrushes[i], null, new Rect(x1, barTop, x2 - x1, barHeight));
            }
        }

        // Draw value bar
        var valueNormalized = Math.Clamp((Value - min) / range, 0, 1);
        var valueWidth = valueNormalized * chartWidth;
        var valueBrush = ValueBrush ?? s_defaultValueBrush;
        context.DrawRectangle(valueBrush, null, new Rect(chartLeft, barTop + barHeight * 0.15, valueWidth, barHeight * 0.7));

        // Draw target line
        var targetNormalized = Math.Clamp((Target - min) / range, 0, 1);
        var targetX = chartLeft + targetNormalized * chartWidth;
        var targetBrush = TargetBrush ?? s_defaultTargetBrush;
        var targetPen = new Pen(targetBrush, 2.5);
        context.DrawLine(targetPen,
            new Point(targetX, barTop - 4),
            new Point(targetX, barTop + barHeight + 4));

        // Draw value label
        var format = Format ?? "G";
        var unit = Unit ?? string.Empty;
        var valueText = Value.ToString(format, CultureInfo.InvariantCulture) + unit;
        var valueLabelBrush = Brushes.White;
        var valueFormatted = new FormattedText(valueText, CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
            s_labelTypeface, 10, valueLabelBrush);
        var labelX = chartLeft + valueWidth + 4;
        var labelYPos = barTop + (barHeight - valueFormatted.Height) / 2;
        if (labelX + valueFormatted.Width < chartRight)
        {
            context.DrawText(valueFormatted, new Point(labelX, labelYPos));
        }
    }

    private static IBrush GetDefaultRangeBrush(int index)
    {
        return (index % 3) switch
        {
            0 => s_defaultRangeBrush1,
            1 => s_defaultRangeBrush2,
            _ => s_defaultRangeBrush3
        };
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var height = 40.0;
        var width = double.IsInfinity(availableSize.Width) ? 200 : Math.Min(availableSize.Width, 200);
        return new Size(width, height);
    }
}
