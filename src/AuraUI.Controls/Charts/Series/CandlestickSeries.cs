using Avalonia;
using Avalonia.Collections;
using Avalonia.Media;

namespace AuraUI.Controls.Charts.Series;

/// <summary>
/// A candlestick chart series for financial OHLC data (Open, High, Low, Close).
///
/// Rendering:
///   - Each candlestick is a filled rectangle (body) with vertical lines (wicks)
///   - "Up" candles (Close >= Open) use UpColor (default green)
///   - "Down" candles (Close &lt; Open) use DownColor (default red)
///   - Body spans from Open to Close; wicks span from Low to High
///
/// Performance:
///   - All candlesticks drawn in a single Render pass via DrawingContext
///   - Hit testing uses rectangle containment on the body
/// </summary>
public class CandlestickSeries : ChartSeries
{
    /// <summary>
    /// Defines the <see cref="UpColor"/> styled property.
    /// Color for bullish candles (Close >= Open).
    /// </summary>
    public static readonly StyledProperty<IBrush?> UpColorProperty =
        AvaloniaProperty.Register<CandlestickSeries, IBrush?>(nameof(UpColor));

    /// <summary>
    /// Defines the <see cref="DownColor"/> styled property.
    /// Color for bearish candles (Close &lt; Open).
    /// </summary>
    public static readonly StyledProperty<IBrush?> DownColorProperty =
        AvaloniaProperty.Register<CandlestickSeries, IBrush?>(nameof(DownColor));

    /// <summary>
    /// Defines the <see cref="WickThickness"/> styled property.
    /// Thickness of the high/low wick line in pixels.
    /// </summary>
    public static readonly StyledProperty<double> WickThicknessProperty =
        AvaloniaProperty.Register<CandlestickSeries, double>(nameof(WickThickness), 1.0);

    /// <summary>
    /// Defines the <see cref="BodyWidth"/> styled property.
    /// Width of the candle body in logical units. When 0, auto-calculated.
    /// </summary>
    public static readonly StyledProperty<double> BodyWidthProperty =
        AvaloniaProperty.Register<CandlestickSeries, double>(nameof(BodyWidth));

    /// <summary>
    /// Defines the <see cref="ShowWick"/> styled property.
    /// Whether to draw the high/low wick lines.
    /// </summary>
    public static readonly StyledProperty<bool> ShowWickProperty =
        AvaloniaProperty.Register<CandlestickSeries, bool>(nameof(ShowWick), true);

    /// <summary>
    /// Defines the <see cref="XAxisIndex"/> styled property.
    /// </summary>
    public static readonly StyledProperty<int> XAxisIndexProperty =
        AvaloniaProperty.Register<CandlestickSeries, int>(nameof(XAxisIndex));

    /// <summary>
    /// Defines the <see cref="YAxisIndex"/> styled property.
    /// </summary>
    public static readonly StyledProperty<int> YAxisIndexProperty =
        AvaloniaProperty.Register<CandlestickSeries, int>(nameof(YAxisIndex));

    public IBrush? UpColor
    {
        get => GetValue(UpColorProperty);
        set => SetValue(UpColorProperty, value);
    }

    public IBrush? DownColor
    {
        get => GetValue(DownColorProperty);
        set => SetValue(DownColorProperty, value);
    }

    public double WickThickness
    {
        get => GetValue(WickThicknessProperty);
        set => SetValue(WickThicknessProperty, value);
    }

    public double BodyWidth
    {
        get => GetValue(BodyWidthProperty);
        set => SetValue(BodyWidthProperty, value);
    }

    public bool ShowWick
    {
        get => GetValue(ShowWickProperty);
        set => SetValue(ShowWickProperty, value);
    }

    public int XAxisIndex
    {
        get => GetValue(XAxisIndexProperty);
        set => SetValue(XAxisIndexProperty, value);
    }

    public int YAxisIndex
    {
        get => GetValue(YAxisIndexProperty);
        set => SetValue(YAxisIndexProperty, value);
    }

    // ────────────────────────────────────────────────
    //  Data
    // ────────────────────────────────────────────────

    private AvaloniaList<OhlcDataPoint>? _dataPoints;

    /// <summary>
    /// The OHLC data points for this series.
    /// </summary>
    public AvaloniaList<OhlcDataPoint> DataPoints
    {
        get => _dataPoints ??= new AvaloniaList<OhlcDataPoint>();
        set
        {
            if (_dataPoints != null)
                _dataPoints.CollectionChanged -= OnDataCollectionChanged;
            _dataPoints = value;
            if (_dataPoints != null)
                _dataPoints.CollectionChanged += OnDataCollectionChanged;
            RaiseDataChanged();
        }
    }

    public CandlestickSeries()
    {
        (_dataPoints ??= new AvaloniaList<OhlcDataPoint>()).CollectionChanged += OnDataCollectionChanged;
    }

    private void OnDataCollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        RaiseDataChanged();
    }

    internal override bool UsesCategoryAxis => false;
    internal override bool RequiresAxes => true;
    internal override string RendererKey => "Candlestick";
}

/// <summary>
/// A single OHLC data point for candlestick charts.
/// </summary>
public class OhlcDataPoint
{
    /// <summary>X position (typically a date index or category index).</summary>
    public double X { get; set; }

    /// <summary>Opening price.</summary>
    public double Open { get; set; }

    /// <summary>Highest price.</summary>
    public double High { get; set; }

    /// <summary>Lowest price.</summary>
    public double Low { get; set; }

    /// <summary>Closing price.</summary>
    public double Close { get; set; }

    /// <summary>Display label (e.g., date string).</summary>
    public string? Label { get; set; }

    /// <summary>Arbitrary metadata.</summary>
    public object? Tag { get; set; }

    public OhlcDataPoint() { }

    public OhlcDataPoint(double x, double open, double high, double low, double close)
    {
        X = x;
        Open = open;
        High = high;
        Low = low;
        Close = close;
    }

    /// <summary>Whether this candle is bullish (close >= open).</summary>
    public bool IsUp => Close >= Open;
}
