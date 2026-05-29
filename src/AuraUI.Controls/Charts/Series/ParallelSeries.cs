using Avalonia;
using Avalonia.Collections;
using Avalonia.Media;

namespace AuraUI.Controls.Charts.Series;

/// <summary>
/// A parallel coordinates chart series. Renders multivariate data as
/// polylines crossing parallel vertical axes.
///
/// Rendering:
///   - Each axis is a vertical line
///   - Each data item is a polyline connecting its values across axes
///   - Lines drawn with configurable opacity for overplotting
/// </summary>
public class ParallelSeries : ChartSeries
{
    /// <summary>Labels for each parallel axis.</summary>
    public static readonly StyledProperty<string[]?> AxisLabelsProperty =
        AvaloniaProperty.Register<ParallelSeries, string[]?>(nameof(AxisLabels));

    /// <summary>Minimum values for each axis (auto-calculated if null).</summary>
    public static readonly StyledProperty<double[]?> AxisMinValuesProperty =
        AvaloniaProperty.Register<ParallelSeries, double[]?>(nameof(AxisMinValues));

    /// <summary>Maximum values for each axis (auto-calculated if null).</summary>
    public static readonly StyledProperty<double[]?> AxisMaxValuesProperty =
        AvaloniaProperty.Register<ParallelSeries, double[]?>(nameof(AxisMaxValues));

    /// <summary>Line thickness for each data polyline.</summary>
    public static readonly StyledProperty<double> LineThicknessProperty =
        AvaloniaProperty.Register<ParallelSeries, double>(nameof(LineThickness), 1.5);

    /// <summary>Opacity of each polyline (for overplotting).</summary>
    public static readonly StyledProperty<double> LineOpacityProperty =
        AvaloniaProperty.Register<ParallelSeries, double>(nameof(LineOpacity), 0.3);

    /// <summary>Whether to show axis labels.</summary>
    public static readonly StyledProperty<bool> ShowAxisLabelsProperty =
        AvaloniaProperty.Register<ParallelSeries, bool>(nameof(ShowAxisLabels), true);

    public string[]? AxisLabels { get => GetValue(AxisLabelsProperty); set => SetValue(AxisLabelsProperty, value); }
    public double[]? AxisMinValues { get => GetValue(AxisMinValuesProperty); set => SetValue(AxisMinValuesProperty, value); }
    public double[]? AxisMaxValues { get => GetValue(AxisMaxValuesProperty); set => SetValue(AxisMaxValuesProperty, value); }
    public double LineThickness { get => GetValue(LineThicknessProperty); set => SetValue(LineThicknessProperty, value); }
    public double LineOpacity { get => GetValue(LineOpacityProperty); set => SetValue(LineOpacityProperty, value); }
    public bool ShowAxisLabels { get => GetValue(ShowAxisLabelsProperty); set => SetValue(ShowAxisLabelsProperty, value); }

    private AvaloniaList<ChartParallelData>? _dataItems;

    public AvaloniaList<ChartParallelData> DataItems
    {
        get => _dataItems ??= new AvaloniaList<ChartParallelData>();
        set
        {
            if (_dataItems != null)
                _dataItems.CollectionChanged -= OnDataChanged;
            _dataItems = value;
            if (_dataItems != null)
                _dataItems.CollectionChanged += OnDataChanged;
            RaiseDataChanged();
        }
    }

    public ParallelSeries()
    {
        (_dataItems ??= new AvaloniaList<ChartParallelData>()).CollectionChanged += OnDataChanged;
    }

    private void OnDataChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        RaiseDataChanged();
    }

    internal override bool UsesCategoryAxis => false;
    internal override bool RequiresAxes => false;
    internal override string RendererKey => "Parallel";
}
