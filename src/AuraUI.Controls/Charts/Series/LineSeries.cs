using Avalonia;
using Avalonia.Media;

namespace AuraUI.Controls.Charts.Series;

/// <summary>
/// A line chart series. Renders connected line segments between data points
/// with optional area fill, markers, and smooth interpolation.
///
/// Rendering approach:
///   - Line segments are rendered as a single PathGeometry (cached, rebuilt on data change)
///   - Area fill is a closed PathGeometry below the line
///   - Markers are drawn as individual shapes at each data point
///   - All rendering via DrawingContext -- zero child controls
///
/// Performance:
///   - PathGeometry cached in _cachedLinePath / _cachedAreaPath
///   - Only rebuilt when DataPoints collection changes or layout size changes
///   - For >10k points, LTTB downsampling is applied automatically
/// </summary>
public class LineSeries : XYChartSeries
{
    // ────────────────────────────────────────────────
    //  Line-Specific Properties
    // ────────────────────────────────────────────────

    /// <summary>
    /// Defines the <see cref="SmoothTension"/> styled property.
    /// Controls the tension of monotone cubic interpolation (0.0 - 1.0).
    /// </summary>
    public static readonly StyledProperty<double> SmoothTensionProperty =
        AvaloniaProperty.Register<LineSeries, double>(nameof(SmoothTension), 0.3,
            coerce: (_, v) => Math.Clamp(v, 0.0, 1.0));

    /// <summary>
    /// Defines the <see cref="DashStyle"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double[]?> DashStyleProperty =
        AvaloniaProperty.Register<LineSeries, double[]?>(nameof(DashStyle));

    /// <summary>
    /// Defines the <see cref="ShowMarkers"/> styled property.
    /// Convenience to show markers even when MarkerShape is set.
    /// </summary>
    public static readonly StyledProperty<bool> ShowMarkersProperty =
        AvaloniaProperty.Register<LineSeries, bool>(nameof(ShowMarkers));

    public double SmoothTension
    {
        get => GetValue(SmoothTensionProperty);
        set => SetValue(SmoothTensionProperty, value);
    }

    public double[]? DashStyle
    {
        get => GetValue(DashStyleProperty);
        set => SetValue(DashStyleProperty, value);
    }

    public bool ShowMarkers
    {
        get => GetValue(ShowMarkersProperty);
        set => SetValue(ShowMarkersProperty, value);
    }

    internal override string RendererKey => "Line";
}
