using Avalonia.Animation.Easings;
using Avalonia.Threading;

namespace AuraUI.Controls.Charts;

/// <summary>
/// Manages frame-based animation for chart transitions. Supports:
///   - Entry animation: data points grow from zero
///   - Update animation: smooth transition when data changes
///   - Exit animation: data points shrink to zero
///   - Configurable duration and easing per animation mode
///   - Per-series animation delay (staggered entry)
///
/// Architecture:
///   - Uses a DispatcherTimer at 60fps (16ms interval) to drive the animation clock
///   - Each frame, the chart calls <see cref="GetEasedProgress"/> to get the
///     interpolation factor (0.0 = old state, 1.0 = new state)
///   - The chart's Render method uses this factor to lerp between old and new positions
///   - When progress reaches 1.0, the timer stops and the final frame is rendered
///   - Per-series stagger is computed as: delay = seriesIndex * StaggerDelay
///
/// This approach is chosen over Avalonia's built-in Animation system because:
///   1. Chart animations need per-frame control of the render pass
///   2. The built-in animation system operates on AvaloniaProperties, not DrawingContext calls
///   3. A single timer drives all active series animations simultaneously
/// </summary>
internal sealed class ChartAnimation : IDisposable
{
    private DispatcherTimer? _timer;
    private DateTime _startTime;
    private TimeSpan _duration;
    private Easing _easing;
    private double _currentProgress = 1.0;
    private bool _isAnimating;
    private ChartAnimationMode _currentMode = ChartAnimationMode.None;

    // Stagger support
    private int _seriesCount;
    private TimeSpan _staggerDelay;

    /// <summary>
    /// Called each animation frame. The parameter is the interpolated progress (0..1).
    /// The subscriber should call InvalidateVisual() to trigger a re-render.
    /// </summary>
    public event Action<double>? FrameTick;

    /// <summary>
    /// Called when the animation completes (progress reaches 1.0).
    /// </summary>
    public event Action? Completed;

    public bool IsAnimating => _isAnimating;
    public double CurrentProgress => _currentProgress;

    /// <summary>
    /// The current animation mode (entry, update, exit, or none).
    /// </summary>
    public ChartAnimationMode CurrentMode => _currentMode;

    /// <summary>
    /// Per-series stagger delay. When > 0, each series starts its animation
    /// after a delay proportional to its index.
    /// </summary>
    public TimeSpan StaggerDelay
    {
        get => _staggerDelay;
        set => _staggerDelay = value;
    }

    /// <summary>
    /// Number of series (used for stagger calculation).
    /// </summary>
    public int SeriesCount
    {
        get => _seriesCount;
        set => _seriesCount = value;
    }

    public ChartAnimation()
    {
        _easing = new CubicEaseOut();
        _duration = TimeSpan.FromMilliseconds(400);
    }

    /// <summary>
    /// Start a new animation. Cancels any in-progress animation.
    /// </summary>
    public void Start(TimeSpan? duration = null, Easing? easing = null)
    {
        _duration = duration ?? TimeSpan.FromMilliseconds(400);
        _easing = easing ?? new CubicEaseOut();
        _startTime = DateTime.UtcNow;
        _currentProgress = 0.0;
        _currentMode = ChartAnimationMode.Update;
        _isAnimating = true;

        EnsureTimerRunning();
    }

    /// <summary>
    /// Start an entry animation (data points grow from zero).
    /// </summary>
    public void StartEntry(TimeSpan? duration = null, Easing? easing = null, int seriesCount = 1)
    {
        _duration = duration ?? TimeSpan.FromMilliseconds(600);
        _easing = easing ?? new CubicEaseOut();
        _seriesCount = seriesCount;
        _startTime = DateTime.UtcNow;
        _currentProgress = 0.0;
        _currentMode = ChartAnimationMode.Entry;
        _isAnimating = true;

        EnsureTimerRunning();
    }

    /// <summary>
    /// Start an exit animation (data points shrink to zero).
    /// </summary>
    public void StartExit(TimeSpan? duration = null, Easing? easing = null, int seriesCount = 1)
    {
        _duration = duration ?? TimeSpan.FromMilliseconds(300);
        _easing = easing ?? new CubicEaseIn();
        _seriesCount = seriesCount;
        _startTime = DateTime.UtcNow;
        _currentProgress = 0.0;
        _currentMode = ChartAnimationMode.Exit;
        _isAnimating = true;

        EnsureTimerRunning();
    }

    /// <summary>
    /// Get the eased progress for a specific series index (accounts for stagger delay).
    /// Returns a value between 0 and 1.
    /// </summary>
    public double GetEasedProgressForSeries(int seriesIndex)
    {
        if (!_isAnimating) return _currentMode == ChartAnimationMode.Exit ? 0.0 : 1.0;

        var elapsed = DateTime.UtcNow - _startTime;
        var stagger = TimeSpan.FromTicks(_staggerDelay.Ticks * seriesIndex);
        var adjustedElapsed = elapsed - stagger;

        if (adjustedElapsed.TotalMilliseconds <= 0)
            return _currentMode == ChartAnimationMode.Exit ? 1.0 : 0.0;

        var rawProgress = _duration.TotalMilliseconds > 0
            ? adjustedElapsed.TotalMilliseconds / _duration.TotalMilliseconds
            : 1.0;

        var clamped = Math.Clamp(rawProgress, 0.0, 1.0);
        return _easing.Ease(clamped);
    }

    /// <summary>
    /// Complete the animation immediately (jump to end state).
    /// </summary>
    public void Complete()
    {
        Stop();
        _currentProgress = 1.0;
        _currentMode = ChartAnimationMode.None;
        Completed?.Invoke();
    }

    /// <summary>
    /// Cancel the animation without raising Completed.
    /// </summary>
    public void Cancel()
    {
        Stop();
        _currentProgress = 1.0;
        _currentMode = ChartAnimationMode.None;
    }

    /// <summary>
    /// Get the eased progress value for the current frame.
    /// Returns 1.0 when not animating.
    /// </summary>
    public double GetEasedProgress()
    {
        if (!_isAnimating) return 1.0;
        return _easing.Ease(_currentProgress);
    }

    /// <summary>
    /// Get the easing preset for the given configuration.
    /// </summary>
    public static Easing GetEasing(ChartEasingPreset preset)
    {
        return preset switch
        {
            ChartEasingPreset.CubicEaseOut => new CubicEaseOut(),
            ChartEasingPreset.CubicEaseIn => new CubicEaseIn(),
            ChartEasingPreset.CubicEaseInOut => new CubicEaseInOut(),
            ChartEasingPreset.Linear => new LinearEasing(),
            ChartEasingPreset.ElasticEaseOut => new ElasticEaseOut(),
            ChartEasingPreset.BackEaseOut => new BackEaseOut(),
            _ => new CubicEaseOut()
        };
    }

    private void EnsureTimerRunning()
    {
        if (_timer == null)
        {
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(16) // ~60fps
            };
            _timer.Tick += OnTimerTick;
        }

        _timer.Start();
    }

    private void OnTimerTick(object? sender, EventArgs e)
    {
        var elapsed = DateTime.UtcNow - _startTime;

        // For staggered animations, we need to extend the total duration
        var totalDuration = _duration;
        if (_staggerDelay.TotalMilliseconds > 0 && _seriesCount > 1)
        {
            totalDuration = TimeSpan.FromMilliseconds(
                _duration.TotalMilliseconds + _staggerDelay.TotalMilliseconds * (_seriesCount - 1));
        }

        var rawProgress = totalDuration.TotalMilliseconds > 0
            ? elapsed.TotalMilliseconds / totalDuration.TotalMilliseconds
            : 1.0;

        _currentProgress = Math.Clamp(rawProgress, 0.0, 1.0);

        if (_currentProgress >= 1.0)
        {
            _currentProgress = 1.0;
            Stop();
            FrameTick?.Invoke(1.0);
            var mode = _currentMode;
            _currentMode = ChartAnimationMode.None;
            Completed?.Invoke();

            // If this was an entry animation, we might want to chain into update mode
            // (the chart handles this via the Completed event)
        }
        else
        {
            FrameTick?.Invoke(_easing.Ease(_currentProgress));
        }
    }

    private void Stop()
    {
        _isAnimating = false;
        _timer?.Stop();
    }

    public void Dispose()
    {
        _timer?.Stop();
        if (_timer != null)
            _timer.Tick -= OnTimerTick;
        _timer = null;
    }
}
