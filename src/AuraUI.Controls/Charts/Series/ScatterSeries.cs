using Avalonia;
using Avalonia.Media;

namespace AuraUI.Controls.Charts.Series;

/// <summary>
/// A scatter plot series. Renders individual markers at each data point
/// with configurable shape, size, and optional size mapping.
///
/// Rendering:
///   - Markers drawn as individual shapes via DrawingContext
///   - Supports size-mapped bubbles (SizeBinding)
///   - All markers drawn in a single Render pass
/// </summary>
public class ScatterSeries : XYChartSeries
{
    /// <summary>
    /// Defines the <see cref="MinMarkerSize"/> styled property.
    /// Minimum marker size when size-mapping is active.
    /// </summary>
    public static readonly StyledProperty<double> MinMarkerSizeProperty =
        AvaloniaProperty.Register<ScatterSeries, double>(nameof(MinMarkerSize), 4.0);

    /// <summary>
    /// Defines the <see cref="MaxMarkerSize"/> styled property.
    /// Maximum marker size when size-mapping is active.
    /// </summary>
    public static readonly StyledProperty<double> MaxMarkerSizeProperty =
        AvaloniaProperty.Register<ScatterSeries, double>(nameof(MaxMarkerSize), 30.0);

    /// <summary>
    /// Defines the <see cref="FillOpacity"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> FillOpacityProperty =
        AvaloniaProperty.Register<ScatterSeries, double>(nameof(FillOpacity), 0.7);

    /// <summary>
    /// Defines the <see cref="SizeValues"/> styled property.
    /// Per-point size values for bubble charts. When set, marker size is mapped from these values.
    /// </summary>
    public static readonly StyledProperty<double[]?> SizeValuesProperty =
        AvaloniaProperty.Register<ScatterSeries, double[]?>(nameof(SizeValues));

    /// <summary>
    /// Defines the <see cref="RegressionType"/> styled property.
    /// Type of regression line to overlay on the scatter plot.
    /// </summary>
    public static readonly StyledProperty<RegressionType> RegressionTypeProperty =
        AvaloniaProperty.Register<ScatterSeries, RegressionType>(nameof(RegressionType), RegressionType.None);

    /// <summary>
    /// Defines the <see cref="RegressionLineColor"/> styled property.
    /// Color of the regression line.
    /// </summary>
    public static readonly StyledProperty<IBrush?> RegressionLineColorProperty =
        AvaloniaProperty.Register<ScatterSeries, IBrush?>(nameof(RegressionLineColor));

    /// <summary>
    /// Defines the <see cref="ShowDensityContour"/> styled property.
    /// Whether to overlay density contour lines.
    /// </summary>
    public static readonly StyledProperty<bool> ShowDensityContourProperty =
        AvaloniaProperty.Register<ScatterSeries, bool>(nameof(ShowDensityContour));

    /// <summary>
    /// Defines the <see cref="DensityContourLevels"/> styled property.
    /// Number of contour levels for density visualization.
    /// </summary>
    public static readonly StyledProperty<int> DensityContourLevelsProperty =
        AvaloniaProperty.Register<ScatterSeries, int>(nameof(DensityContourLevels), 5);

    public double MinMarkerSize
    {
        get => GetValue(MinMarkerSizeProperty);
        set => SetValue(MinMarkerSizeProperty, value);
    }

    public double MaxMarkerSize
    {
        get => GetValue(MaxMarkerSizeProperty);
        set => SetValue(MaxMarkerSizeProperty, value);
    }

    public double FillOpacity
    {
        get => GetValue(FillOpacityProperty);
        set => SetValue(FillOpacityProperty, value);
    }

    public double[]? SizeValues
    {
        get => GetValue(SizeValuesProperty);
        set => SetValue(SizeValuesProperty, value);
    }

    public RegressionType RegressionType
    {
        get => GetValue(RegressionTypeProperty);
        set => SetValue(RegressionTypeProperty, value);
    }

    public IBrush? RegressionLineColor
    {
        get => GetValue(RegressionLineColorProperty);
        set => SetValue(RegressionLineColorProperty, value);
    }

    public bool ShowDensityContour
    {
        get => GetValue(ShowDensityContourProperty);
        set => SetValue(ShowDensityContourProperty, value);
    }

    public int DensityContourLevels
    {
        get => GetValue(DensityContourLevelsProperty);
        set => SetValue(DensityContourLevelsProperty, value);
    }

    static ScatterSeries()
    {
        // Default marker to circle for scatter
        MarkerShapeProperty.OverrideDefaultValue<ScatterSeries>(MarkerShape.Circle);
        MarkerSizeProperty.OverrideDefaultValue<ScatterSeries>(8.0);
    }

    internal override string RendererKey => "Scatter";
}
