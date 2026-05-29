using Avalonia;
using Avalonia.Media;

namespace AuraUI.Controls.Charts.Interaction;

/// <summary>
/// Visual map provides data-driven visual encoding: mapping data values to colors,
/// sizes, or opacities. Supports both continuous gradient and piecewise (discrete) mapping.
///
/// Types:
///   - Continuous: smooth gradient from MinColor to MaxColor based on value
///   - Piecewise: discrete color ranges with labels
///
/// Usage:
///   var visualMap = new VisualMap();
///   visualMap.Type = VisualMapType.Continuous;
///   visualMap.MinValue = 0;
///   visualMap.MaxValue = 100;
///   visualMap.MinColor = Brushes.Blue;
///   visualMap.MaxColor = Brushes.Red;
///   chart.VisualMap = visualMap;
/// </summary>
public class VisualMap : AvaloniaObject
{
    /// <summary>
    /// Defines the <see cref="Type"/> styled property.
    /// </summary>
    public static readonly StyledProperty<VisualMapType> TypeProperty =
        AvaloniaProperty.Register<VisualMap, VisualMapType>(nameof(Type), VisualMapType.Continuous);

    /// <summary>
    /// Defines the <see cref="IsVisible"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsVisibleProperty =
        AvaloniaProperty.Register<VisualMap, bool>(nameof(IsVisible), true);

    /// <summary>
    /// Defines the <see cref="MinValue"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> MinValueProperty =
        AvaloniaProperty.Register<VisualMap, double>(nameof(MinValue));

    /// <summary>
    /// Defines the <see cref="MaxValue"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> MaxValueProperty =
        AvaloniaProperty.Register<VisualMap, double>(nameof(MaxValue), 100.0);

    /// <summary>
    /// Defines the <see cref="MinColor"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> MinColorProperty =
        AvaloniaProperty.Register<VisualMap, IBrush?>(nameof(MinColor));

    /// <summary>
    /// Defines the <see cref="MaxColor"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> MaxColorProperty =
        AvaloniaProperty.Register<VisualMap, IBrush?>(nameof(MaxColor));

    /// <summary>
    /// Defines the <see cref="Position"/> styled property.
    /// Position of the visual map control (slider/legend).
    /// </summary>
    public static readonly StyledProperty<LegendPosition> PositionProperty =
        AvaloniaProperty.Register<VisualMap, LegendPosition>(nameof(Position), LegendPosition.Right);

    /// <summary>
    /// Defines the <see cref="ShowSlider"/> styled property.
    /// Whether to show the interactive slider for adjusting the range.
    /// </summary>
    public static readonly StyledProperty<bool> ShowSliderProperty =
        AvaloniaProperty.Register<VisualMap, bool>(nameof(ShowSlider), true);

    /// <summary>
    /// Defines the <see cref="ShowLabels"/> styled property.
    /// Whether to show value labels on the slider.
    /// </summary>
    public static readonly StyledProperty<bool> ShowLabelsProperty =
        AvaloniaProperty.Register<VisualMap, bool>(nameof(ShowLabels), true);

    /// <summary>
    /// Defines the <see cref="Dimension"/> styled property.
    /// Which visual dimension to encode: "color", "size", or "opacity".
    /// </summary>
    public static readonly StyledProperty<string> DimensionProperty =
        AvaloniaProperty.Register<VisualMap, string>(nameof(Dimension), "color");

    /// <summary>
    /// Defines the <see cref="SliderMin"/> styled property.
    /// Current minimum of the slider range (user-adjustable).
    /// </summary>
    public static readonly StyledProperty<double> SliderMinProperty =
        AvaloniaProperty.Register<VisualMap, double>(nameof(SliderMin), double.NaN);

    /// <summary>
    /// Defines the <see cref="SliderMax"/> styled property.
    /// Current maximum of the slider range (user-adjustable).
    /// </summary>
    public static readonly StyledProperty<double> SliderMaxProperty =
        AvaloniaProperty.Register<VisualMap, double>(nameof(SliderMax), double.NaN);

    /// <summary>
    /// Defines the <see cref="ItemWidth"/> styled property.
    /// Width of the visual map control in pixels.
    /// </summary>
    public static readonly StyledProperty<double> ItemWidthProperty =
        AvaloniaProperty.Register<VisualMap, double>(nameof(ItemWidth), 20.0);

    /// <summary>
    /// Defines the <see cref="ItemHeight"/> styled property.
    /// Height of the visual map control in pixels.
    /// </summary>
    public static readonly StyledProperty<double> ItemHeightProperty =
        AvaloniaProperty.Register<VisualMap, double>(nameof(ItemHeight), 140.0);

    public VisualMapType Type { get => GetValue(TypeProperty); set => SetValue(TypeProperty, value); }
    public bool IsVisible { get => GetValue(IsVisibleProperty); set => SetValue(IsVisibleProperty, value); }
    public double MinValue { get => GetValue(MinValueProperty); set => SetValue(MinValueProperty, value); }
    public double MaxValue { get => GetValue(MaxValueProperty); set => SetValue(MaxValueProperty, value); }
    public IBrush? MinColor { get => GetValue(MinColorProperty); set => SetValue(MinColorProperty, value); }
    public IBrush? MaxColor { get => GetValue(MaxColorProperty); set => SetValue(MaxColorProperty, value); }
    public LegendPosition Position { get => GetValue(PositionProperty); set => SetValue(PositionProperty, value); }
    public bool ShowSlider { get => GetValue(ShowSliderProperty); set => SetValue(ShowSliderProperty, value); }
    public bool ShowLabels { get => GetValue(ShowLabelsProperty); set => SetValue(ShowLabelsProperty, value); }
    public string Dimension { get => GetValue(DimensionProperty); set => SetValue(DimensionProperty, value); }
    public double SliderMin { get => GetValue(SliderMinProperty); set => SetValue(SliderMinProperty, value); }
    public double SliderMax { get => GetValue(SliderMaxProperty); set => SetValue(SliderMaxProperty, value); }
    public double ItemWidth { get => GetValue(ItemWidthProperty); set => SetValue(ItemWidthProperty, value); }
    public double ItemHeight { get => GetValue(ItemHeightProperty); set => SetValue(ItemHeightProperty, value); }

    /// <summary>
    /// Piecewise color definitions (used when Type is Piecewise).
    /// </summary>
    public List<VisualMapPiece> Pieces { get; } = new();

    /// <summary>
    /// Get the mapped color for a given data value.
    /// </summary>
    public IBrush GetColor(double value)
    {
        if (Type == VisualMapType.Piecewise)
        {
            return GetPiecewiseColor(value);
        }
        return GetContinuousColor(value);
    }

    /// <summary>
    /// Get the mapped size for a given data value (for size encoding).
    /// </summary>
    public double GetSize(double value, double minSize, double maxSize)
    {
        var t = NormalizeValue(value);
        return minSize + t * (maxSize - minSize);
    }

    /// <summary>
    /// Get the mapped opacity for a given data value.
    /// </summary>
    public double GetOpacity(double value, double minOpacity, double maxOpacity)
    {
        var t = NormalizeValue(value);
        return minOpacity + t * (maxOpacity - minOpacity);
    }

    /// <summary>
    /// Check if a value is within the current slider range.
    /// </summary>
    public bool IsInRange(double value)
    {
        var effectiveMin = double.IsNaN(SliderMin) ? MinValue : SliderMin;
        var effectiveMax = double.IsNaN(SliderMax) ? MaxValue : SliderMax;
        return value >= effectiveMin && value <= effectiveMax;
    }

    /// <summary>
    /// Render the visual map control (gradient bar + slider).
    /// </summary>
    public void Render(DrawingContext context, Rect controlArea)
    {
        if (!IsVisible) return;

        var minColor = (MinColor as SolidColorBrush)?.Color ?? Avalonia.Media.Colors.Blue;
        var maxColor = (MaxColor as SolidColorBrush)?.Color ?? Avalonia.Media.Colors.Red;

        if (Type == VisualMapType.Continuous)
        {
            RenderContinuousBar(context, controlArea, minColor, maxColor);
        }
        else
        {
            RenderPiecewiseBar(context, controlArea);
        }

        // Labels
        if (ShowLabels)
        {
            RenderLabels(context, controlArea);
        }
    }

    private double NormalizeValue(double value)
    {
        var range = MaxValue - MinValue;
        if (Math.Abs(range) < 1e-10) return 0;
        return Math.Clamp((value - MinValue) / range, 0, 1);
    }

    private IBrush GetContinuousColor(double value)
    {
        var t = NormalizeValue(value);
        var minColor = (MinColor as SolidColorBrush)?.Color ?? Avalonia.Media.Colors.Blue;
        var maxColor = (MaxColor as SolidColorBrush)?.Color ?? Avalonia.Media.Colors.Red;

        var r = (byte)(minColor.R + t * (maxColor.R - minColor.R));
        var g = (byte)(minColor.G + t * (maxColor.G - minColor.G));
        var b = (byte)(minColor.B + t * (maxColor.B - minColor.B));

        return new SolidColorBrush(Avalonia.Media.Color.FromRgb(r, g, b));
    }

    private IBrush GetPiecewiseColor(double value)
    {
        foreach (var piece in Pieces)
        {
            if (value >= piece.Min && value < piece.Max)
                return piece.Color ?? Brushes.Gray;
        }

        // Default: use continuous mapping
        return GetContinuousColor(value);
    }

    private void RenderContinuousBar(DrawingContext context, Rect area,
        Avalonia.Media.Color minColor, Avalonia.Media.Color maxColor)
    {
        // Draw gradient bar using horizontal strips
        var barRect = new Rect(area.X + area.Width - ItemWidth, area.Y, ItemWidth, area.Height);
        var stripCount = Math.Max(1, (int)(area.Height / 2));

        for (int i = 0; i < stripCount; i++)
        {
            var t = (double)i / stripCount;
            var r = (byte)(minColor.R + t * (maxColor.R - minColor.R));
            var g = (byte)(minColor.G + t * (maxColor.G - minColor.G));
            var b = (byte)(minColor.B + t * (maxColor.B - minColor.B));

            var stripBrush = new SolidColorBrush(Avalonia.Media.Color.FromRgb(r, g, b));
            var stripRect = new Rect(barRect.X, barRect.Y + t * barRect.Height,
                barRect.Width, barRect.Height / stripCount + 1);

            context.DrawRectangle(stripBrush, null, stripRect);
        }

        // Border
        context.DrawRectangle(null, new Pen(Brushes.Gray, 1), barRect);
    }

    private void RenderPiecewiseBar(DrawingContext context, Rect area)
    {
        var barRect = new Rect(area.X + area.Width - ItemWidth, area.Y, ItemWidth, area.Height);
        var pieceHeight = barRect.Height / Math.Max(1, Pieces.Count);

        for (int i = 0; i < Pieces.Count; i++)
        {
            var piece = Pieces[i];
            var pieceBrush = piece.Color ?? Brushes.Gray;
            var pieceRect = new Rect(barRect.X, barRect.Y + i * pieceHeight,
                barRect.Width, pieceHeight);

            context.DrawRectangle(pieceBrush, null, pieceRect);
        }

        // Border
        context.DrawRectangle(null, new Pen(Brushes.Gray, 1), barRect);
    }

    private void RenderLabels(DrawingContext context, Rect area)
    {
        var barX = area.X + area.Width - ItemWidth;

        // Min label
        var minLabel = new FormattedText(MinValue.ToString("F0"),
            System.Globalization.CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight,
            new Typeface("Segoe UI", FontStyle.Normal, FontWeight.Normal),
            9.0,
            Brushes.Gray);

        context.DrawText(minLabel, new Point(barX - minLabel.Width - 4, area.Y));

        // Max label
        var maxLabel = new FormattedText(MaxValue.ToString("F0"),
            System.Globalization.CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight,
            new Typeface("Segoe UI", FontStyle.Normal, FontWeight.Normal),
            9.0,
            Brushes.Gray);

        context.DrawText(maxLabel, new Point(barX - maxLabel.Width - 4, area.Bottom - maxLabel.Height));
    }
}
