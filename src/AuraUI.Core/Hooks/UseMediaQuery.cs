using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;

namespace AuraUI.Core.Hooks;

/// <summary>
/// React-like hook that evaluates a media query against the current window size.
/// Listens for window resize events and exposes a <see cref="Matches"/> property
/// that indicates whether the query is currently satisfied.
/// </summary>
/// <example>
/// <code>
/// // In a ViewModel or code-behind attached to a control:
/// var hook = new UseMediaQuery(control, minWidth: 768);
/// hook.Changed += () => Console.WriteLine(hook.Matches ? "Desktop" : "Mobile");
///
/// // Or use a range:
/// var tablet = new UseMediaQuery(control, minWidth: 600, maxWidth: 1024);
/// </code>
/// </example>
public class UseMediaQuery : IDisposable
{
    private readonly Control _control;
    private readonly double _minWidth;
    private readonly double _maxWidth;
    private readonly double _minHeight;
    private readonly double _maxHeight;
    private TopLevel? _topLevel;
    private bool _disposed;
    private bool _matches;

    /// <summary>
    /// Gets whether the current window size matches the query constraints.
    /// </summary>
    public bool Matches
    {
        get => _matches;
        private set
        {
            if (_matches == value) return;
            _matches = value;
            Changed?.Invoke();
        }
    }

    /// <summary>
    /// Raised whenever <see cref="Matches"/> changes.
    /// </summary>
    public event Action? Changed;

    /// <summary>
    /// Creates a media query hook for the given control.
    /// </summary>
    /// <param name="control">The control whose window will be monitored.</param>
    /// <param name="minWidth">Minimum window width (0 = no minimum).</param>
    /// <param name="maxWidth">Maximum window width (<see cref="double.MaxValue"/> = no maximum).</param>
    /// <param name="minHeight">Minimum window height (0 = no minimum).</param>
    /// <param name="maxHeight">Maximum window height (<see cref="double.MaxValue"/> = no maximum).</param>
    public UseMediaQuery(Control control, double minWidth = 0, double maxWidth = double.MaxValue,
        double minHeight = 0, double maxHeight = double.MaxValue)
    {
        _control = control ?? throw new ArgumentNullException(nameof(control));
        _minWidth = minWidth;
        _maxWidth = maxWidth;
        _minHeight = minHeight;
        _maxHeight = maxHeight;

        _control.AttachedToVisualTree += OnAttachedToVisualTree;
        _control.DetachedFromVisualTree += OnDetachedFromVisualTree;

        // If already attached, hook immediately
        var topLevel = TopLevel.GetTopLevel(_control);
        if (topLevel is not null)
        {
            HookTopLevel(topLevel);
        }
    }

    /// <summary>
    /// Creates a media query hook for a named breakpoint.
    /// </summary>
    /// <param name="control">The control whose window will be monitored.</param>
    /// <param name="breakpointName">A registered breakpoint name from <see cref="Helpers.ResponsiveHelper"/>.</param>
    public UseMediaQuery(Control control, string breakpointName)
    {
        _control = control ?? throw new ArgumentNullException(nameof(control));

        var breakpoints = Helpers.ResponsiveHelper.GetRegisteredBreakpoints();
        if (breakpoints.TryGetValue(breakpointName, out var range))
        {
            _minWidth = range.MinWidth;
            _maxWidth = range.MaxWidth;
        }
        else
        {
            _minWidth = 0;
            _maxWidth = double.MaxValue;
        }

        _minHeight = 0;
        _maxHeight = double.MaxValue;

        _control.AttachedToVisualTree += OnAttachedToVisualTree;
        _control.DetachedFromVisualTree += OnDetachedFromVisualTree;

        var topLevel = TopLevel.GetTopLevel(_control);
        if (topLevel is not null)
        {
            HookTopLevel(topLevel);
        }
    }

    private void OnAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        if (e.Root is TopLevel topLevel)
        {
            HookTopLevel(topLevel);
        }
    }

    private void OnDetachedFromVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        UnhookTopLevel();
    }

    private void HookTopLevel(TopLevel topLevel)
    {
        UnhookTopLevel();
        _topLevel = topLevel;
        _topLevel.PropertyChanged += OnTopLevelPropertyChanged;
        Evaluate();
    }

    private void UnhookTopLevel()
    {
        if (_topLevel is not null)
        {
            _topLevel.PropertyChanged -= OnTopLevelPropertyChanged;
            _topLevel = null;
        }
    }

    private void OnTopLevelPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == Visual.BoundsProperty)
        {
            Evaluate();
        }
    }

    private void Evaluate()
    {
        if (_topLevel is null)
        {
            Matches = false;
            return;
        }

        var bounds = _topLevel.Bounds;
        var widthMatch = bounds.Width >= _minWidth && bounds.Width <= _maxWidth;
        var heightMatch = bounds.Height >= _minHeight && bounds.Height <= _maxHeight;
        Matches = widthMatch && heightMatch;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        UnhookTopLevel();
        _control.AttachedToVisualTree -= OnAttachedToVisualTree;
        _control.DetachedFromVisualTree -= OnDetachedFromVisualTree;
        GC.SuppressFinalize(this);
    }
}
