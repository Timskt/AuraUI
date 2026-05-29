using Avalonia;
using Avalonia.Collections;

namespace AuraUI.Controls.Charts.Series;

/// <summary>
/// A pie/donut chart series. Each data slice is a <see cref="ChartSliceData"/>.
///
/// Rendering:
///   - Slices are rendered as arc PathGeometry segments via DrawingContext
///   - Supports donut (inner radius > 0), exploded slices, and label lines
///   - All slices drawn in a single Render pass
///
/// Performance:
///   - Arc geometries cached; rebuilt only on data change or resize
///   - Hit testing uses angle-in-sector math (no per-slice controls)
/// </summary>
public class PieSeries : ChartSeries
{
    /// <summary>
    /// Defines the <see cref="InnerRadius"/> styled property.
    /// Ratio of inner radius to outer radius (0 = full pie, 0.5 = donut).
    /// </summary>
    public static readonly StyledProperty<double> InnerRadiusProperty =
        AvaloniaProperty.Register<PieSeries, double>(nameof(InnerRadius),
            coerce: (_, v) => Math.Clamp(v, 0.0, 0.95));

    /// <summary>
    /// Defines the <see cref="StartAngle"/> styled property.
    /// Starting angle in degrees (0 = 12 o'clock, increases clockwise).
    /// </summary>
    public static readonly StyledProperty<double> StartAngleProperty =
        AvaloniaProperty.Register<PieSeries, double>(nameof(StartAngle), -90);

    /// <summary>
    /// Defines the <see cref="EndAngle"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> EndAngleProperty =
        AvaloniaProperty.Register<PieSeries, double>(nameof(EndAngle), 270);

    /// <summary>
    /// Defines the <see cref="PadAngle"/> styled property.
    /// Gap angle between slices in degrees.
    /// </summary>
    public static readonly StyledProperty<double> PadAngleProperty =
        AvaloniaProperty.Register<PieSeries, double>(nameof(PadAngle), 1.0);

    /// <summary>
    /// Defines the <see cref="ExplodeDistance"/> styled property.
    /// How far exploded slices are offset from center (in pixels).
    /// </summary>
    public static readonly StyledProperty<double> ExplodeDistanceProperty =
        AvaloniaProperty.Register<PieSeries, double>(nameof(ExplodeDistance), 10.0);

    /// <summary>
    /// Defines the <see cref="ShowLabels"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowLabelsProperty =
        AvaloniaProperty.Register<PieSeries, bool>(nameof(ShowLabels), true);

    /// <summary>
    /// Defines the <see cref="ShowPercentage"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowPercentageProperty =
        AvaloniaProperty.Register<PieSeries, bool>(nameof(ShowPercentage), true);

    /// <summary>
    /// Defines the <see cref="LabelLineLength"/> styled property.
    /// Length of the leader line from slice to label.
    /// </summary>
    public static readonly StyledProperty<double> LabelLineLengthProperty =
        AvaloniaProperty.Register<PieSeries, double>(nameof(LabelLineLength), 20.0);

    public double InnerRadius
    {
        get => GetValue(InnerRadiusProperty);
        set => SetValue(InnerRadiusProperty, value);
    }

    public double StartAngle
    {
        get => GetValue(StartAngleProperty);
        set => SetValue(StartAngleProperty, value);
    }

    public double EndAngle
    {
        get => GetValue(EndAngleProperty);
        set => SetValue(EndAngleProperty, value);
    }

    public double PadAngle
    {
        get => GetValue(PadAngleProperty);
        set => SetValue(PadAngleProperty, value);
    }

    public double ExplodeDistance
    {
        get => GetValue(ExplodeDistanceProperty);
        set => SetValue(ExplodeDistanceProperty, value);
    }

    public bool ShowLabels
    {
        get => GetValue(ShowLabelsProperty);
        set => SetValue(ShowLabelsProperty, value);
    }

    public bool ShowPercentage
    {
        get => GetValue(ShowPercentageProperty);
        set => SetValue(ShowPercentageProperty, value);
    }

    public double LabelLineLength
    {
        get => GetValue(LabelLineLengthProperty);
        set => SetValue(LabelLineLengthProperty, value);
    }

    // ────────────────────────────────────────────────
    //  Data
    // ────────────────────────────────────────────────

    private AvaloniaList<ChartSliceData>? _slices;

    public AvaloniaList<ChartSliceData> Slices
    {
        get => _slices ??= new AvaloniaList<ChartSliceData>();
        set
        {
            if (_slices != null)
                _slices.CollectionChanged -= OnSlicesChanged;
            _slices = value;
            if (_slices != null)
                _slices.CollectionChanged += OnSlicesChanged;
            RaiseDataChanged();
        }
    }

    /// <summary>
    /// Index of each slice that should be exploded (offset from center).
    /// </summary>
    public AvaloniaList<int> ExplodedIndices { get; } = new();

    public PieSeries()
    {
        (_slices ??= new AvaloniaList<ChartSliceData>()).CollectionChanged += OnSlicesChanged;
        ExplodedIndices.CollectionChanged += (_, _) => RaiseDataChanged();
    }

    private void OnSlicesChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        RaiseDataChanged();
    }

    internal override bool UsesCategoryAxis => true;
    internal override bool RequiresAxes => false;
    internal override string RendererKey => "Pie";
}
