using Avalonia.Collections;

namespace AuraUI.Controls.Charts;

/// <summary>
/// Provides real-time data streaming capabilities for charts.
/// Supports appending new data points with auto-scroll, updating points in place,
/// and clearing data. Designed for live dashboards, monitoring, and streaming scenarios.
///
/// Usage:
///   var rt = new ChartRealTime(chart);
///   rt.MaxVisiblePoints = 100;
///   rt.AppendData(0, new ChartDataPoint(time, value));
///   // Chart auto-scrolls to show latest data
///
/// Features:
///   - AppendData: adds a point and scrolls the view window
///   - UpdateData: updates a point in place (e.g., latest candlestick)
///   - ClearData: clears all data for a series
///   - Auto-scroll to latest data (configurable)
///   - Configurable max visible points (window size)
///   - Automatic axis range management
/// </summary>
public class ChartRealTime
{
    private readonly Chart _chart;

    /// <summary>
    /// Maximum number of visible points in the chart window.
    /// When exceeded, the oldest points scroll off the left edge.
    /// Set to 0 for unlimited (no windowing).
    /// </summary>
    public int MaxVisiblePoints { get; set; } = 200;

    /// <summary>
    /// Whether to automatically scroll the X axis to show the latest data.
    /// </summary>
    public bool AutoScroll { get; set; } = true;

    /// <summary>
    /// Whether to automatically adjust the Y axis range to fit visible data.
    /// </summary>
    public bool AutoFitY { get; set; } = true;

    /// <summary>
    /// Padding ratio for Y axis auto-fit (0.1 = 10% padding above and below).
    /// </summary>
    public double YPaddingRatio { get; set; } = 0.1;

    /// <summary>
    /// Whether to use smooth animation when scrolling the view window.
    /// When true, axis ranges transition smoothly instead of jumping.
    /// </summary>
    public bool SmoothScroll { get; set; } = true;

    /// <summary>
    /// Duration of the smooth scroll animation in milliseconds.
    /// </summary>
    public int SmoothScrollDurationMs { get; set; } = 150;

    // Smooth scroll state
    private double _targetXMin, _targetXMax, _targetYMin, _targetYMax;
    private double _sourceXMin, _sourceXMax, _sourceYMin, _sourceYMax;
    private DateTime _scrollAnimStart;
    private bool _isScrollAnimating;

    /// <summary>
    /// Event raised when new data is appended.
    /// </summary>
    public event EventHandler<DataAppendedEventArgs>? DataAppended;

    /// <summary>
    /// Event raised when the visible window scrolls.
    /// </summary>
#pragma warning disable CS0067 // Event is never used — public API for consumers
    public event EventHandler? WindowScrolled;
#pragma warning restore CS0067

    public ChartRealTime(Chart chart)
    {
        _chart = chart ?? throw new ArgumentNullException(nameof(chart));
    }

    /// <summary>
    /// Append a data point to a series and auto-scroll if needed.
    /// </summary>
    /// <param name="seriesIndex">Index of the series in the chart's Series collection.</param>
    /// <param name="dataPoint">The data point to append.</param>
    public void AppendData(int seriesIndex, ChartDataPoint dataPoint)
    {
        if (seriesIndex < 0 || seriesIndex >= _chart.Series.Count)
            throw new ArgumentOutOfRangeException(nameof(seriesIndex));

        var series = _chart.Series[seriesIndex];
        if (series is not XYChartSeries xy)
            throw new InvalidOperationException("AppendData only works with XY-based series.");

        xy.DataPoints.Add(dataPoint);

        // Enforce max visible points window
        if (MaxVisiblePoints > 0 && xy.DataPoints.Count > MaxVisiblePoints * 2)
        {
            // Remove old points beyond the window (keep 2x for smooth scrolling)
            var excess = xy.DataPoints.Count - MaxVisiblePoints * 2;
            for (int i = 0; i < excess; i++)
            {
                xy.DataPoints.RemoveAt(0);
            }
        }

        // Auto-scroll X axis to show latest data
        if (AutoScroll)
        {
            var xRange = _chart.XAxis.EffectiveMax - _chart.XAxis.EffectiveMin;
            if (MaxVisiblePoints > 0 && xRange > 0)
            {
                var allXValues = xy.DataPoints.Select(p => p.X).ToArray();
                if (allXValues.Length > 0)
                {
                    var latestX = allXValues[^1];
                    var windowStart = latestX - xRange;

                    if (SmoothScroll)
                    {
                        // Start smooth scroll animation
                        _sourceXMin = _chart.XAxis.EffectiveMin;
                        _sourceXMax = _chart.XAxis.EffectiveMax;
                        _targetXMin = windowStart;
                        _targetXMax = latestX;
                        _scrollAnimStart = DateTime.UtcNow;
                        _isScrollAnimating = true;

                        // Start a timer to drive the animation
                        StartScrollAnimationTimer();
                    }
                    else
                    {
                        _chart.XAxis.EffectiveMin = windowStart;
                        _chart.XAxis.EffectiveMax = latestX;
                    }
                }
            }
        }

        // Auto-fit Y axis to visible data
        if (AutoFitY)
        {
            if (SmoothScroll)
            {
                ComputeYTargets();
                if (!_isScrollAnimating)
                {
                    _scrollAnimStart = DateTime.UtcNow;
                    _isScrollAnimating = true;
                    StartScrollAnimationTimer();
                }
            }
            else
            {
                FitYAxisToVisibleData();
            }
        }

        DataAppended?.Invoke(this, new DataAppendedEventArgs(seriesIndex, dataPoint));
        _chart.InvalidateVisual();
    }

    /// <summary>
    /// Update a data point at the specified index.
    /// Useful for updating the latest candlestick or live value.
    /// </summary>
    /// <param name="seriesIndex">Index of the series.</param>
    /// <param name="dataIndex">Index of the data point to update.</param>
    /// <param name="newValue">The new Y value (X remains unchanged).</param>
    public void UpdateData(int seriesIndex, int dataIndex, double newValue)
    {
        if (seriesIndex < 0 || seriesIndex >= _chart.Series.Count)
            throw new ArgumentOutOfRangeException(nameof(seriesIndex));

        var series = _chart.Series[seriesIndex];
        if (series is not XYChartSeries xy)
            throw new InvalidOperationException("UpdateData only works with XY-based series.");

        if (dataIndex < 0 || dataIndex >= xy.DataPoints.Count)
            throw new ArgumentOutOfRangeException(nameof(dataIndex));

        xy.DataPoints[dataIndex].Y = newValue;

        if (AutoFitY)
        {
            FitYAxisToVisibleData();
        }

        _chart.InvalidateVisual();
    }

    /// <summary>
    /// Update a data point with a new ChartDataPoint (replaces both X and Y).
    /// </summary>
    public void UpdateData(int seriesIndex, int dataIndex, ChartDataPoint newDataPoint)
    {
        if (seriesIndex < 0 || seriesIndex >= _chart.Series.Count)
            throw new ArgumentOutOfRangeException(nameof(seriesIndex));

        var series = _chart.Series[seriesIndex];
        if (series is not XYChartSeries xy)
            throw new InvalidOperationException("UpdateData only works with XY-based series.");

        if (dataIndex < 0 || dataIndex >= xy.DataPoints.Count)
            throw new ArgumentOutOfRangeException(nameof(dataIndex));

        xy.DataPoints[dataIndex] = newDataPoint;

        if (AutoFitY)
        {
            FitYAxisToVisibleData();
        }

        _chart.InvalidateVisual();
    }

    /// <summary>
    /// Clear all data for a series.
    /// </summary>
    /// <param name="seriesIndex">Index of the series.</param>
    public void ClearData(int seriesIndex)
    {
        if (seriesIndex < 0 || seriesIndex >= _chart.Series.Count)
            throw new ArgumentOutOfRangeException(nameof(seriesIndex));

        var series = _chart.Series[seriesIndex];
        if (series is XYChartSeries xy)
        {
            xy.DataPoints.Clear();
        }
        else if (series is Series.PieSeries pie)
        {
            pie.Slices.Clear();
        }
        else if (series is Series.FunnelSeries funnel)
        {
            funnel.Items.Clear();
        }

        _chart.InvalidateVisual();
    }

    /// <summary>
    /// Clear all data for all series.
    /// </summary>
    public void ClearAllData()
    {
        for (int i = 0; i < _chart.Series.Count; i++)
        {
            ClearData(i);
        }
    }

    /// <summary>
    /// Append multiple data points at once (batch update).
    /// More efficient than calling AppendData in a loop.
    /// </summary>
    /// <param name="seriesIndex">Index of the series.</param>
    /// <param name="dataPoints">The data points to append.</param>
    public void AppendDataRange(int seriesIndex, IEnumerable<ChartDataPoint> dataPoints)
    {
        if (seriesIndex < 0 || seriesIndex >= _chart.Series.Count)
            throw new ArgumentOutOfRangeException(nameof(seriesIndex));

        var series = _chart.Series[seriesIndex];
        if (series is not XYChartSeries xy)
            throw new InvalidOperationException("AppendDataRange only works with XY-based series.");

        foreach (var dp in dataPoints)
            xy.DataPoints.Add(dp);

        // Enforce max visible points window
        if (MaxVisiblePoints > 0 && xy.DataPoints.Count > MaxVisiblePoints * 2)
        {
            var excess = xy.DataPoints.Count - MaxVisiblePoints * 2;
            for (int i = 0; i < excess; i++)
                xy.DataPoints.RemoveAt(0);
        }

        // Auto-scroll and auto-fit (without smooth animation for batch)
        if (AutoScroll)
        {
            var xRange = _chart.XAxis.EffectiveMax - _chart.XAxis.EffectiveMin;
            if (MaxVisiblePoints > 0 && xRange > 0 && xy.DataPoints.Count > 0)
            {
                var latestX = xy.DataPoints[^1].X;
                _chart.XAxis.EffectiveMin = latestX - xRange;
                _chart.XAxis.EffectiveMax = latestX;
            }
        }

        if (AutoFitY)
            FitYAxisToVisibleData();

        _chart.InvalidateVisual();
    }

    /// <summary>
    /// Release resources held by this instance (animation timers).
    /// </summary>
    public void Dispose()
    {
        _scrollTimer?.Dispose();
        _scrollTimer = null;
    }

    /// <summary>
    /// Get the current number of data points in a series.
    /// </summary>
    public int GetDataCount(int seriesIndex)
    {
        if (seriesIndex < 0 || seriesIndex >= _chart.Series.Count)
            return 0;

        var series = _chart.Series[seriesIndex];
        if (series is XYChartSeries xy)
            return xy.DataPoints.Count;

        return 0;
    }

    /// <summary>
    /// Compute target Y axis values for smooth scroll.
    /// </summary>
    private void ComputeYTargets()
    {
        double yMin = double.MaxValue;
        double yMax = double.MinValue;

        var xMin = _targetXMin;
        var xMax = _targetXMax;

        foreach (var s in _chart.Series)
        {
            if (s is not XYChartSeries xy || !s.IsVisible) continue;
            foreach (var pt in xy.DataPoints)
            {
                if (!double.IsNaN(xMin) && pt.X < xMin) continue;
                if (!double.IsNaN(xMax) && pt.X > xMax) continue;
                if (pt.Y < yMin) yMin = pt.Y;
                if (pt.Y > yMax) yMax = pt.Y;
            }
        }

        if (yMin == double.MaxValue || yMax == double.MinValue)
        {
            _targetYMin = _chart.YAxis.EffectiveMin;
            _targetYMax = _chart.YAxis.EffectiveMax;
            return;
        }

        var yRange = yMax - yMin;
        if (yRange < 1e-10) yRange = 1.0;
        var padding = yRange * YPaddingRatio;

        _sourceYMin = _chart.YAxis.EffectiveMin;
        _sourceYMax = _chart.YAxis.EffectiveMax;
        _targetYMin = yMin - padding;
        _targetYMax = yMax + padding;
    }

    private System.Threading.Timer? _scrollTimer;

    private void StartScrollAnimationTimer()
    {
        _scrollTimer?.Dispose();
        _scrollTimer = new System.Threading.Timer(_ =>
        {
            Avalonia.Threading.Dispatcher.UIThread.Post(AnimateScrollStep);
        }, null, 0, 16); // ~60fps
    }

    private void AnimateScrollStep()
    {
        if (!_isScrollAnimating)
        {
            _scrollTimer?.Dispose();
            _scrollTimer = null;
            return;
        }

        var elapsed = (DateTime.UtcNow - _scrollAnimStart).TotalMilliseconds;
        var duration = Math.Max(SmoothScrollDurationMs, 1);
        var t = Math.Clamp(elapsed / duration, 0, 1);

        // Ease-out cubic
        var easedT = 1 - Math.Pow(1 - t, 3);

        _chart.XAxis.EffectiveMin = _sourceXMin + (_targetXMin - _sourceXMin) * easedT;
        _chart.XAxis.EffectiveMax = _sourceXMax + (_targetXMax - _sourceXMax) * easedT;

        if (AutoFitY)
        {
            _chart.YAxis.EffectiveMin = _sourceYMin + (_targetYMin - _sourceYMin) * easedT;
            _chart.YAxis.EffectiveMax = _sourceYMax + (_targetYMax - _sourceYMax) * easedT;
        }

        _chart.InvalidateVisual();

        if (t >= 1.0)
        {
            _isScrollAnimating = false;
            _scrollTimer?.Dispose();
            _scrollTimer = null;
        }
    }

    /// <summary>
    /// Fit the Y axis range to the currently visible data points.
    /// </summary>
    private void FitYAxisToVisibleData()
    {
        double yMin = double.MaxValue;
        double yMax = double.MinValue;

        var xMin = _chart.XAxis.EffectiveMin;
        var xMax = _chart.XAxis.EffectiveMax;

        foreach (var s in _chart.Series)
        {
            if (s is not XYChartSeries xy || !s.IsVisible) continue;

            foreach (var pt in xy.DataPoints)
            {
                // Only consider points within the visible X range
                if (!double.IsNaN(xMin) && pt.X < xMin) continue;
                if (!double.IsNaN(xMax) && pt.X > xMax) continue;

                if (pt.Y < yMin) yMin = pt.Y;
                if (pt.Y > yMax) yMax = pt.Y;
            }
        }

        if (yMin == double.MaxValue || yMax == double.MinValue)
            return;

        // Add padding
        var yRange = yMax - yMin;
        if (yRange < 1e-10) yRange = 1.0;
        var padding = yRange * YPaddingRatio;

        _chart.YAxis.EffectiveMin = yMin - padding;
        _chart.YAxis.EffectiveMax = yMax + padding;
    }
}

/// <summary>
/// Event args for data appended events.
/// </summary>
public class DataAppendedEventArgs : EventArgs
{
    public int SeriesIndex { get; }
    public ChartDataPoint DataPoint { get; }

    public DataAppendedEventArgs(int seriesIndex, ChartDataPoint dataPoint)
    {
        SeriesIndex = seriesIndex;
        DataPoint = dataPoint;
    }
}
