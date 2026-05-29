using Avalonia;
using Avalonia.Collections;

namespace AuraUI.Controls.Charts.Series;

/// <summary>
/// A funnel chart series. Renders trapezoidal segments stacked vertically,
/// each representing a stage in a process with decreasing values.
///
/// Rendering:
///   - Each funnel segment is a trapezoid drawn via PathGeometry
///   - Labels drawn inside or beside each segment
///   - All segments in a single Render pass
/// </summary>
public class FunnelSeries : ChartSeries
{
    /// <summary>
    /// Defines the <see cref="Gap"/> styled property.
    /// Vertical gap between segments in pixels.
    /// </summary>
    public static readonly StyledProperty<double> GapProperty =
        AvaloniaProperty.Register<FunnelSeries, double>(nameof(Gap), 2.0);

    /// <summary>
    /// Defines the <see cref="ShowLabels"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowLabelsProperty =
        AvaloniaProperty.Register<FunnelSeries, bool>(nameof(ShowLabels), true);

    /// <summary>
    /// Defines the <see cref="ShowValues"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowValuesProperty =
        AvaloniaProperty.Register<FunnelSeries, bool>(nameof(ShowValues), true);

    /// <summary>
    /// Defines the <see cref="LabelPosition"/> styled property.
    /// </summary>
    public static readonly StyledProperty<FunnelLabelPosition> LabelPositionProperty =
        AvaloniaProperty.Register<FunnelSeries, FunnelLabelPosition>(nameof(LabelPosition),
            FunnelLabelPosition.Inside);

    /// <summary>
    /// Defines the <see cref="NeckRatio"/> styled property.
    /// Ratio of the narrowest point to the widest point (0 = triangle, 1 = rectangle).
    /// </summary>
    public static readonly StyledProperty<double> NeckRatioProperty =
        AvaloniaProperty.Register<FunnelSeries, double>(nameof(NeckRatio), 0.3,
            coerce: (_, v) => Math.Clamp(v, 0.0, 1.0));

    public double Gap
    {
        get => GetValue(GapProperty);
        set => SetValue(GapProperty, value);
    }

    public bool ShowLabels
    {
        get => GetValue(ShowLabelsProperty);
        set => SetValue(ShowLabelsProperty, value);
    }

    public bool ShowValues
    {
        get => GetValue(ShowValuesProperty);
        set => SetValue(ShowValuesProperty, value);
    }

    public FunnelLabelPosition LabelPosition
    {
        get => GetValue(LabelPositionProperty);
        set => SetValue(LabelPositionProperty, value);
    }

    public double NeckRatio
    {
        get => GetValue(NeckRatioProperty);
        set => SetValue(NeckRatioProperty, value);
    }

    // ────────────────────────────────────────────────
    //  Data
    // ────────────────────────────────────────────────

    private AvaloniaList<ChartSliceData>? _items;

    public AvaloniaList<ChartSliceData> Items
    {
        get => _items ??= new AvaloniaList<ChartSliceData>();
        set
        {
            if (_items != null)
                _items.CollectionChanged -= OnDataChanged;
            _items = value;
            if (_items != null)
                _items.CollectionChanged += OnDataChanged;
            RaiseDataChanged();
        }
    }

    public FunnelSeries()
    {
        (_items ??= new AvaloniaList<ChartSliceData>()).CollectionChanged += OnDataChanged;
    }

    private void OnDataChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        RaiseDataChanged();
    }

    internal override bool UsesCategoryAxis => true;
    internal override bool RequiresAxes => false;
    internal override string RendererKey => "Funnel";
}

public enum FunnelLabelPosition
{
    Inside,
    Left,
    Right
}
