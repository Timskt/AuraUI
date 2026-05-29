using Avalonia;

namespace AuraUI.Controls.Charts.Series;

/// <summary>
/// An area chart series. Identical to LineSeries with ShowArea=true by default.
/// Provided as a distinct type for semantic clarity and AXAML readability.
///
/// Rendering: same PathGeometry approach as LineSeries, with the area fill
/// always enabled and configurable opacity.
/// </summary>
public class AreaSeries : XYChartSeries
{
    /// <summary>
    /// Defines the <see cref="SmoothTension"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> SmoothTensionProperty =
        AvaloniaProperty.Register<AreaSeries, double>(nameof(SmoothTension), 0.3,
            coerce: (_, v) => Math.Clamp(v, 0.0, 1.0));

    /// <summary>
    /// Defines the <see cref="BaselineY"/> styled property.
    /// The Y value from which the area is filled (default 0).
    /// </summary>
    public static readonly StyledProperty<double> BaselineYProperty =
        AvaloniaProperty.Register<AreaSeries, double>(nameof(BaselineY));

    public double SmoothTension
    {
        get => GetValue(SmoothTensionProperty);
        set => SetValue(SmoothTensionProperty, value);
    }

    public double BaselineY
    {
        get => GetValue(BaselineYProperty);
        set => SetValue(BaselineYProperty, value);
    }

    public AreaSeries()
    {
        // Area series always shows area fill
        ShowArea = true;
        AreaOpacity = 0.25;
    }

    internal override string RendererKey => "Area";
}
