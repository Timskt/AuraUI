using Avalonia;
using Avalonia.Collections;
using Avalonia.Media;

namespace AuraUI.Controls.Charts.Series;

/// <summary>
/// A theme river chart series. Renders stacked areas centered around a horizontal axis,
/// flowing like a river. Each category is a separate colored layer.
///
/// Rendering:
///   - Data grouped by category, each category forms a "river layer"
///   - Layers are stacked above and below the center axis
///   - Each layer drawn as a filled PathGeometry
///   - Smooth interpolation (catmull-rom) for organic flow
///
/// Use case: Theme evolution over time, topic trends, budget flows.
/// </summary>
public class ThemeRiverSeries : ChartSeries
{
    /// <summary>
    /// Defines the <see cref="Smooth"/> styled property.
    /// Whether to use smooth interpolation for the river edges.
    /// </summary>
    public static readonly StyledProperty<bool> SmoothProperty =
        AvaloniaProperty.Register<ThemeRiverSeries, bool>(nameof(Smooth), true);

    /// <summary>
    /// Defines the <see cref="ShowLabels"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowLabelsProperty =
        AvaloniaProperty.Register<ThemeRiverSeries, bool>(nameof(ShowLabels), true);

    /// <summary>
    /// Defines the <see cref="LabelFontSize"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> LabelFontSizeProperty =
        AvaloniaProperty.Register<ThemeRiverSeries, double>(nameof(LabelFontSize), 10.0);

    /// <summary>
    /// Defines the LayerOpacity property.
    /// </summary>
    public static readonly StyledProperty<double> LayerOpacityProperty =
        AvaloniaProperty.Register<ThemeRiverSeries, double>(nameof(LayerOpacity), 0.8);

    /// <summary>
    /// Defines the <see cref="Categories"/> styled property.
    /// The ordered list of category names. Each name corresponds to a layer in the river.
    /// </summary>
    public static readonly StyledProperty<string[]?> CategoriesProperty =
        AvaloniaProperty.Register<ThemeRiverSeries, string[]?>(nameof(Categories));

    public bool Smooth { get => GetValue(SmoothProperty); set => SetValue(SmoothProperty, value); }
    public bool ShowLabels { get => GetValue(ShowLabelsProperty); set => SetValue(ShowLabelsProperty, value); }
    public double LabelFontSize { get => GetValue(LabelFontSizeProperty); set => SetValue(LabelFontSizeProperty, value); }
    public double LayerOpacity { get => GetValue(LayerOpacityProperty); set => SetValue(LayerOpacityProperty, value); }
    public string[]? Categories { get => GetValue(CategoriesProperty); set => SetValue(CategoriesProperty, value); }

    // ────────────────────────────────────────────────
    //  Data
    // ────────────────────────────────────────────────

    private AvaloniaList<ChartThemeRiverData>? _dataItems;

    public AvaloniaList<ChartThemeRiverData> DataItems
    {
        get => _dataItems ??= new AvaloniaList<ChartThemeRiverData>();
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

    public ThemeRiverSeries()
    {
        (_dataItems ??= new AvaloniaList<ChartThemeRiverData>()).CollectionChanged += OnDataChanged;
    }

    private void OnDataChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        RaiseDataChanged();
    }

    internal override bool UsesCategoryAxis => true;
    internal override bool RequiresAxes => false;
    internal override string RendererKey => "ThemeRiver";
}
