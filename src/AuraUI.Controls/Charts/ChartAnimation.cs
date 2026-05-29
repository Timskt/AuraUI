using Avalonia.Animation.Easings;
using Avalonia.Threading;

namespace AuraUI.Controls.Charts;

/// <summary>
/// Manages frame-based animation for chart transitions. When data changes,
/// the chart interpolates from old values to new values over a configurable
/// duration using easing functions.
///
/// Architecture:
///   - Uses a DispatcherTimer at 60fps (16ms interval) to drive the animation clock
///   - Each frame, the chart calls <see cref="GetCurrentProgress"/> to get the
///     interpolation factor (0.0 = old state, 1.0 = new state)
///   - The chart's Render method uses this factor to lerp between old and new positions
///   - When progress reaches 1.0, the timer stops and the final frame is rendered
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
        _isAnimating = true;

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

    /// <summary>
    /// Complete the animation immediately (jump to end state).
    /// </summary>
    public void Complete()
    {
        Stop();
        _currentProgress = 1.0;
        Completed?.Invoke();
    }

    /// <summary>
    /// Cancel the animation without raising Completed.
    /// </summary>
    public void Cancel()
    {
        Stop();
        _currentProgress = 1.0;
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

    private void OnTimerTick(object? sender, EventArgs e)
    {
        var elapsed = DateTime.UtcNow - _startTime;
        var rawProgress = _duration.TotalMilliseconds > 0
            ? elapsed.TotalMilliseconds / _duration.TotalMilliseconds
            : 1.0;

        _currentProgress = Math.Clamp(rawProgress, 0.0, 1.0);

        if (_currentProgress >= 1.0)
        {
            _currentProgress = 1.0;
            Stop();
            FrameTick?.Invoke(1.0);
            Completed?.Invoke();
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
