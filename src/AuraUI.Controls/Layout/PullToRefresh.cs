using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace AuraUI.Controls.Layout;

/// <summary>
/// A mobile-style pull-to-refresh container. When the user pulls down on the content
/// past a threshold, a refresh indicator appears and the RefreshCommand is executed.
/// </summary>
[PseudoClasses(":pulling", ":refreshing", ":completed")]
public class PullToRefresh : ContentControl
{
    private Point _pullStart;
    private bool _isPulling;

    /// <summary>
    /// Defines the <see cref="IsRefreshing"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsRefreshingProperty =
        AvaloniaProperty.Register<PullToRefresh, bool>(nameof(IsRefreshing));

    /// <summary>
    /// Defines the <see cref="PullThreshold"/> styled property.
    /// The minimum pull distance before refresh triggers.
    /// </summary>
    public static readonly StyledProperty<double> PullThresholdProperty =
        AvaloniaProperty.Register<PullToRefresh, double>(nameof(PullThreshold), 80);

    /// <summary>
    /// Defines the <see cref="RefreshContent"/> styled property.
    /// Content displayed during the pull/refresh (e.g., a loading spinner).
    /// </summary>
    public static readonly StyledProperty<object?> RefreshContentProperty =
        AvaloniaProperty.Register<PullToRefresh, object?>(nameof(RefreshContent));

    /// <summary>
    /// Defines the <see cref="RefreshCommand"/> styled property.
    /// Command executed when pull-to-refresh is triggered.
    /// </summary>
    public static readonly StyledProperty<ICommand?> RefreshCommandProperty =
        AvaloniaProperty.Register<PullToRefresh, ICommand?>(nameof(RefreshCommand));

    /// <summary>
    /// Defines the <see cref="PullDistance"/> styled property.
    /// The current pull distance (0 = resting, positive = pulling down).
    /// </summary>
    public static readonly StyledProperty<double> PullDistanceProperty =
        AvaloniaProperty.Register<PullToRefresh, double>(nameof(PullDistance));

    /// <summary>
    /// Defines the <see cref="IsPullEnabled"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsPullEnabledProperty =
        AvaloniaProperty.Register<PullToRefresh, bool>(nameof(IsPullEnabled), true);

    /// <summary>
    /// Defines the <see cref="CompletedMessage"/> styled property.
    /// Brief message shown after refresh completes.
    /// </summary>
    public static readonly StyledProperty<string?> CompletedMessageProperty =
        AvaloniaProperty.Register<PullToRefresh, string?>(nameof(CompletedMessage));

    /// <summary>
    /// Defines the <see cref="ShowRefreshContent"/> styled property.
    /// Controls visibility of the refresh indicator area.
    /// </summary>
    public static readonly StyledProperty<bool> ShowRefreshContentProperty =
        AvaloniaProperty.Register<PullToRefresh, bool>(nameof(ShowRefreshContent));

    static PullToRefresh()
    {
        IsRefreshingProperty.Changed.AddClassHandler<PullToRefresh>((x, _) => x.UpdatePseudoClasses());
        PullDistanceProperty.Changed.AddClassHandler<PullToRefresh>((x, _) => x.UpdatePseudoClasses());
    }

    /// <summary>
    /// Gets or sets whether a refresh is currently in progress.
    /// </summary>
    public bool IsRefreshing
    {
        get => GetValue(IsRefreshingProperty);
        set => SetValue(IsRefreshingProperty, value);
    }

    /// <summary>
    /// Gets or sets the pull distance threshold to trigger refresh.
    /// </summary>
    public double PullThreshold
    {
        get => GetValue(PullThresholdProperty);
        set => SetValue(PullThresholdProperty, value);
    }

    /// <summary>
    /// Gets or sets the content displayed during pull/refresh (e.g., spinner).
    /// </summary>
    public object? RefreshContent
    {
        get => GetValue(RefreshContentProperty);
        set => SetValue(RefreshContentProperty, value);
    }

    /// <summary>
    /// Gets or sets the command to execute on refresh.
    /// </summary>
    public ICommand? RefreshCommand
    {
        get => GetValue(RefreshCommandProperty);
        set => SetValue(RefreshCommandProperty, value);
    }

    /// <summary>
    /// Gets or sets the current pull distance.
    /// </summary>
    public double PullDistance
    {
        get => GetValue(PullDistanceProperty);
        set => SetValue(PullDistanceProperty, value);
    }

    /// <summary>
    /// Gets or sets whether pull gestures are enabled.
    /// </summary>
    public bool IsPullEnabled
    {
        get => GetValue(IsPullEnabledProperty);
        set => SetValue(IsPullEnabledProperty, value);
    }

    /// <summary>
    /// Gets or sets the message shown after a completed refresh.
    /// </summary>
    public string? CompletedMessage
    {
        get => GetValue(CompletedMessageProperty);
        set => SetValue(CompletedMessageProperty, value);
    }

    /// <summary>
    /// Gets or sets whether to show the refresh content area.
    /// </summary>
    public bool ShowRefreshContent
    {
        get => GetValue(ShowRefreshContentProperty);
        set => SetValue(ShowRefreshContentProperty, value);
    }

    /// <summary>
    /// Occurs when a pull-to-refresh is triggered.
    /// </summary>
    public event EventHandler<RoutedEventArgs>? RefreshRequested;

    /// <summary>
    /// Gets the normalized pull progress (0.0 to 1.0 based on threshold).
    /// </summary>
    public double PullProgress => Math.Min(1.0, PullDistance / PullThreshold);

    /// <summary>
    /// Manually completes the refresh and resets the pull state.
    /// </summary>
    public void CompleteRefresh()
    {
        IsRefreshing = false;
        PullDistance = 0;
        ShowRefreshContent = false;
        PseudoClasses.Set(":completed", true);
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        if (!IsPullEnabled || IsRefreshing) return;

        _isPulling = true;
        _pullStart = e.GetPosition(this);
        e.Handled = true;
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);
        if (!_isPulling || !IsPullEnabled || IsRefreshing) return;

        var current = e.GetPosition(this);
        var deltaY = current.Y - _pullStart.Y;

        if (deltaY > 0)
        {
            // Apply resistance as user pulls further
            var resistance = 0.5;
            PullDistance = deltaY * resistance;
            ShowRefreshContent = PullDistance > 10;
        }

        e.Handled = true;
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        if (!_isPulling) return;
        _isPulling = false;

        if (PullDistance >= PullThreshold)
        {
            // Trigger refresh
            IsRefreshing = true;
            RefreshRequested?.Invoke(this, new RoutedEventArgs());

            if (RefreshCommand?.CanExecute(null) == true)
            {
                RefreshCommand.Execute(null);
            }
        }
        else
        {
            // Snap back
            PullDistance = 0;
            ShowRefreshContent = false;
        }

        e.Handled = true;
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":pulling", _isPulling && PullDistance > 0);
        PseudoClasses.Set(":refreshing", IsRefreshing);
        PseudoClasses.Set(":completed", !IsRefreshing && PullDistance == 0);
    }
}
