using Avalonia;
using Avalonia.Collections;
using Avalonia.Media;

namespace AuraUI.Controls.Charts.Series;

/// <summary>
/// A box-and-whisker plot series for statistical data visualization.
/// Each box shows the distribution of a dataset: min, Q1, median, Q3, max.
///
/// Rendering:
///   - Box from Q1 to Q3 (interquartile range)
///   - Median line inside the box
///   - Whisker lines from box to min and max
///   - Outlier dots beyond whiskers
/// </summary>
public class BoxplotSeries : XYChartSeries
{
    /// <summary>Width of each box as a fraction of available space (0-1).</summary>
    public static readonly StyledProperty<double> BoxWidthProperty =
        AvaloniaProperty.Register<BoxplotSeries, double>(nameof(BoxWidth), 0.5,
            coerce: (_, v) => Math.Clamp(v, 0.1, 1.0));

    /// <summary>Whisker line thickness.</summary>
    public static readonly StyledProperty<double> WhiskerWidthProperty =
        AvaloniaProperty.Register<BoxplotSeries, double>(nameof(WhiskerWidth), 1.0);

    /// <summary>Size of outlier dots.</summary>
    public static readonly StyledProperty<double> OutlierSizeProperty =
        AvaloniaProperty.Register<BoxplotSeries, double>(nameof(OutlierSize), 4.0);

    /// <summary>Fill opacity of the box.</summary>
    public static readonly StyledProperty<double> FillOpacityProperty =
        AvaloniaProperty.Register<BoxplotSeries, double>(nameof(FillOpacity), 0.4);

    public double BoxWidth { get => GetValue(BoxWidthProperty); set => SetValue(BoxWidthProperty, value); }
    public double WhiskerWidth { get => GetValue(WhiskerWidthProperty); set => SetValue(WhiskerWidthProperty, value); }
    public double OutlierSize { get => GetValue(OutlierSizeProperty); set => SetValue(OutlierSizeProperty, value); }
    public double FillOpacity { get => GetValue(FillOpacityProperty); set => SetValue(FillOpacityProperty, value); }

    private AvaloniaList<ChartBoxplotData>? _boxData;

    public AvaloniaList<ChartBoxplotData> BoxData
    {
        get => _boxData ??= new AvaloniaList<ChartBoxplotData>();
        set
        {
            if (_boxData != null)
                _boxData.CollectionChanged -= OnDataChanged;
            _boxData = value;
            if (_boxData != null)
                _boxData.CollectionChanged += OnDataChanged;
            RaiseDataChanged();
        }
    }

    public BoxplotSeries()
    {
        (_boxData ??= new AvaloniaList<ChartBoxplotData>()).CollectionChanged += OnDataChanged;
    }

    private void OnDataChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        RaiseDataChanged();
    }

    internal override string RendererKey => "Boxplot";
}
