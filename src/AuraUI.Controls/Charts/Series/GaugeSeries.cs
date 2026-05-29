using Avalonia;
using Avalonia.Media;

namespace AuraUI.Controls.Charts.Series;

/// <summary>
/// A gauge/dial chart. Renders a single value as an arc on a radial scale.
///
/// Rendering:
///   - Track arc (background) + value arc (foreground) via PathGeometry
///   - Optional needle, tick marks, and center text
///   - All drawn via DrawingContext in a single Render pass
/// </summary>
public class GaugeSeries : ChartSeries
{
    /// <summary>
    /// Defines the <see cref="Value"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> ValueProperty =
        AvaloniaProperty.Register<GaugeSeries, double>(nameof(Value));

    /// <summary>
    /// Defines the <see cref="Minimum"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> MinimumProperty =
        AvaloniaProperty.Register<GaugeSeries, double>(nameof(Minimum));

    /// <summary>
    /// Defines the <see cref="Maximum"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> MaximumProperty =
        AvaloniaProperty.Register<GaugeSeries, double>(nameof(Maximum), 100.0);

    /// <summary>
    /// Defines the <see cref="Mode"/> styled property.
    /// </summary>
    public static readonly StyledProperty<GaugeMode> ModeProperty =
        AvaloniaProperty.Register<GaugeSeries, GaugeMode>(nameof(Mode), GaugeMode.ThreeQuarter);

    /// <summary>
    /// Defines the <see cref="StrokeWidth"/> styled property.
    /// Thickness of the gauge arc.
    /// </summary>
    public static readonly StyledProperty<double> StrokeWidthProperty =
        AvaloniaProperty.Register<GaugeSeries, double>(nameof(StrokeWidth), 12.0);

    /// <summary>
    /// Defines the <see cref="TrackColor"/> styled property.
    /// Color of the background track arc.
    /// </summary>
    public static readonly StyledProperty<IBrush?> TrackColorProperty =
        AvaloniaProperty.Register<GaugeSeries, IBrush?>(nameof(TrackColor));

    /// <summary>
    /// Defines the <see cref="ShowNeedle"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowNeedleProperty =
        AvaloniaProperty.Register<GaugeSeries, bool>(nameof(ShowNeedle));

    /// <summary>
    /// Defines the <see cref="ShowTickMarks"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowTickMarksProperty =
        AvaloniaProperty.Register<GaugeSeries, bool>(nameof(ShowTickMarks), true);

    /// <summary>
    /// Defines the <see cref="TickCount"/> styled property.
    /// </summary>
    public static readonly StyledProperty<int> TickCountProperty =
        AvaloniaProperty.Register<GaugeSeries, int>(nameof(TickCount), 10);

    /// <summary>
    /// Defines the <see cref="ShowCenterLabel"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowCenterLabelProperty =
        AvaloniaProperty.Register<GaugeSeries, bool>(nameof(ShowCenterLabel), true);

    /// <summary>
    /// Defines the <see cref="LabelFormat"/> styled property.
    /// Format string for the center label (e.g. "{0:F1}%").
    /// </summary>
    public static readonly StyledProperty<string?> LabelFormatProperty =
        AvaloniaProperty.Register<GaugeSeries, string?>(nameof(LabelFormat), "{0:F0}");

    /// <summary>
    /// Defines the <see cref="Segments"/> styled property.
    /// Optional color segments (value thresholds with colors).
    /// </summary>
    public static readonly StyledProperty<GaugeSegment[]?> SegmentsProperty =
        AvaloniaProperty.Register<GaugeSeries, GaugeSegment[]?>(nameof(Segments));

    public double Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public double Minimum
    {
        get => GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }

    public double Maximum
    {
        get => GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    public GaugeMode Mode
    {
        get => GetValue(ModeProperty);
        set => SetValue(ModeProperty, value);
    }

    public double StrokeWidth
    {
        get => GetValue(StrokeWidthProperty);
        set => SetValue(StrokeWidthProperty, value);
    }

    public IBrush? TrackColor
    {
        get => GetValue(TrackColorProperty);
        set => SetValue(TrackColorProperty, value);
    }

    public bool ShowNeedle
    {
        get => GetValue(ShowNeedleProperty);
        set => SetValue(ShowNeedleProperty, value);
    }

    public bool ShowTickMarks
    {
        get => GetValue(ShowTickMarksProperty);
        set => SetValue(ShowTickMarksProperty, value);
    }

    public int TickCount
    {
        get => GetValue(TickCountProperty);
        set => SetValue(TickCountProperty, value);
    }

    public bool ShowCenterLabel
    {
        get => GetValue(ShowCenterLabelProperty);
        set => SetValue(ShowCenterLabelProperty, value);
    }

    public string? LabelFormat
    {
        get => GetValue(LabelFormatProperty);
        set => SetValue(LabelFormatProperty, value);
    }

    public GaugeSegment[]? Segments
    {
        get => GetValue(SegmentsProperty);
        set => SetValue(SegmentsProperty, value);
    }

    internal override bool UsesCategoryAxis => false;
    internal override bool RequiresAxes => false;
    internal override string RendererKey => "Gauge";
}

/// <summary>
/// Defines a color segment on a gauge arc. Used for multi-color gauge tracks
/// (e.g., green for 0-60, yellow for 60-80, red for 80-100).
/// </summary>
public class GaugeSegment
{
    public double From { get; set; }
    public double To { get; set; }
    public IBrush? Color { get; set; }
}
