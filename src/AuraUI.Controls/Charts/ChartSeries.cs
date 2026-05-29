using Avalonia;
using Avalonia.Collections;
using Avalonia.Media;

namespace AuraUI.Controls.Charts;

/// <summary>
/// Base class for all chart series. A series holds data and rendering configuration.
/// The actual rendering is delegated to a corresponding <see cref="Rendering.IChartRenderer"/>.
///
/// Series does NOT derive from Control -- it is a lightweight data/config object.
/// This avoids the overhead of a control tree per series (the critical perf difference
/// from naive chart implementations).
/// </summary>
public abstract class ChartSeries : AvaloniaObject
{
    // ────────────────────────────────────────────────
    //  Avalonia Properties
    // ────────────────────────────────────────────────

    /// <summary>
    /// Defines the <see cref="Title"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<ChartSeries, string?>(nameof(Title));

    /// <summary>
    /// Defines the <see cref="Color"/> styled property.
    /// When null, a color is auto-assigned from the chart's palette.
    /// </summary>
    public static readonly StyledProperty<IBrush?> ColorProperty =
        AvaloniaProperty.Register<ChartSeries, IBrush?>(nameof(Color));

    /// <summary>
    /// Defines the <see cref="StrokeThickness"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> StrokeThicknessProperty =
        AvaloniaProperty.Register<ChartSeries, double>(nameof(StrokeThickness), 2.0);

    /// <summary>
    /// Defines the <see cref="Opacity"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> OpacityProperty =
        AvaloniaProperty.Register<ChartSeries, double>(nameof(Opacity), 1.0);

    /// <summary>
    /// Defines the <see cref="IsVisible"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsVisibleProperty =
        AvaloniaProperty.Register<ChartSeries, bool>(nameof(IsVisible), true);

    /// <summary>
    /// Defines the <see cref="MarkerShape"/> styled property.
    /// </summary>
    public static readonly StyledProperty<MarkerShape> MarkerShapeProperty =
        AvaloniaProperty.Register<ChartSeries, MarkerShape>(nameof(MarkerShape), MarkerShape.None);

    /// <summary>
    /// Defines the <see cref="MarkerSize"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> MarkerSizeProperty =
        AvaloniaProperty.Register<ChartSeries, double>(nameof(MarkerSize), 6.0);

    /// <summary>
    /// Defines the <see cref="StackGroup"/> styled property.
    /// Series with the same stack group are stacked together.
    /// </summary>
    public static readonly StyledProperty<string?> StackGroupProperty =
        AvaloniaProperty.Register<ChartSeries, string?>(nameof(StackGroup));

    /// <summary>
    /// Defines the <see cref="StackMode"/> styled property.
    /// </summary>
    public static readonly StyledProperty<StackMode> StackModeProperty =
        AvaloniaProperty.Register<ChartSeries, StackMode>(nameof(StackMode), StackMode.None);

    /// <summary>
    /// Defines the <see cref="AnimationDuration"/> styled property.
    /// </summary>
    public static readonly StyledProperty<TimeSpan> AnimationDurationProperty =
        AvaloniaProperty.Register<ChartSeries, TimeSpan>(nameof(AnimationDuration),
            TimeSpan.FromMilliseconds(400));

    // ────────────────────────────────────────────────
    //  CLR Property Wrappers
    // ────────────────────────────────────────────────

    public string? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public IBrush? Color
    {
        get => GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }

    public double StrokeThickness
    {
        get => GetValue(StrokeThicknessProperty);
        set => SetValue(StrokeThicknessProperty, value);
    }

    public double Opacity
    {
        get => GetValue(OpacityProperty);
        set => SetValue(OpacityProperty, value);
    }

    public bool IsVisible
    {
        get => GetValue(IsVisibleProperty);
        set => SetValue(IsVisibleProperty, value);
    }

    public MarkerShape MarkerShape
    {
        get => GetValue(MarkerShapeProperty);
        set => SetValue(MarkerShapeProperty, value);
    }

    public double MarkerSize
    {
        get => GetValue(MarkerSizeProperty);
        set => SetValue(MarkerSizeProperty, value);
    }

    public string? StackGroup
    {
        get => GetValue(StackGroupProperty);
        set => SetValue(StackGroupProperty, value);
    }

    public StackMode StackMode
    {
        get => GetValue(StackModeProperty);
        set => SetValue(StackModeProperty, value);
    }

    public TimeSpan AnimationDuration
    {
        get => GetValue(AnimationDurationProperty);
        set => SetValue(AnimationDurationProperty, value);
    }

    // ────────────────────────────────────────────────
    //  Index in parent chart (set by Chart on attach)
    // ────────────────────────────────────────────────

    /// <summary>
    /// Index of this series within the parent chart's series collection.
    /// Used for auto-color assignment from the palette.
    /// </summary>
    internal int SeriesIndex { get; set; }

    /// <summary>
    /// Whether this series uses a category axis (pie, radar, funnel) vs. XY axes.
    /// </summary>
    internal abstract bool UsesCategoryAxis { get; }

    /// <summary>
    /// Whether this series requires two axes (line, bar, area, scatter) vs. none (pie, gauge, funnel).
    /// </summary>
    internal abstract bool RequiresAxes { get; }

    /// <summary>
    /// Called by the parent chart when any data or configuration changes.
    /// Returns the renderer key that maps to an <see cref="Rendering.IChartRenderer"/>.
    /// </summary>
    internal abstract string RendererKey { get; }

    /// <summary>
    /// Raised when the series data changes, prompting the chart to re-render.
    /// </summary>
    internal event EventHandler? DataChanged;

    /// <summary>
    /// Notify the parent chart that data has changed and a re-render is needed.
    /// </summary>
    protected void RaiseDataChanged()
    {
        DataChanged?.Invoke(this, EventArgs.Empty);
    }
}
