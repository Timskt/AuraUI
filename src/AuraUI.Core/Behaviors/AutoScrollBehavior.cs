using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace AuraUI.Core.Behaviors;

/// <summary>
/// Behavior that automatically scrolls a ScrollViewer to the bottom when new content is added.
/// Stops auto-scrolling if the user manually scrolls up, and resumes when scrolled back to the bottom.
/// </summary>
public class AutoScrollBehavior : Behavior<ScrollViewer>
{
    private bool _isAutoScrollEnabled = true;
    private bool _isSubscribed;

    #region IsEnabled

    public static readonly StyledProperty<bool> IsEnabledProperty =
        AvaloniaProperty.RegisterAttached<ScrollViewer, bool>(
            "AutoScrollBehavior_IsEnabled", typeof(AutoScrollBehavior), true);

    /// <summary>
    /// Gets or sets whether auto-scrolling is enabled.
    /// </summary>
    public bool IsEnabled
    {
        get => GetValue(IsEnabledProperty);
        set => SetValue(IsEnabledProperty, value);
    }

    #endregion

    #region Threshold

    public static readonly StyledProperty<double> ThresholdProperty =
        AvaloniaProperty.RegisterAttached<ScrollViewer, double>(
            "AutoScrollBehavior_Threshold", typeof(AutoScrollBehavior), 50.0);

    /// <summary>
    /// Gets or sets the threshold (in pixels) from the bottom to consider "at bottom".
    /// When the user is within this distance from the bottom, auto-scrolling will be active.
    /// </summary>
    public double Threshold
    {
        get => GetValue(ThresholdProperty);
        set => SetValue(ThresholdProperty, value);
    }

    #endregion

    protected override void OnAttached()
    {
        base.OnAttached();

        if (AssociatedObject == null)
            return;

        AssociatedObject.AttachedToVisualTree += OnAttachedToVisualTree;
        AssociatedObject.DetachedFromVisualTree += OnDetachedFromVisualTree;

        // If already in tree, subscribe now
        if (AssociatedObject.IsAttachedToVisualTree)
        {
            Subscribe();
        }
    }

    protected override void OnDetaching()
    {
        Unsubscribe();

        if (AssociatedObject != null)
        {
            AssociatedObject.AttachedToVisualTree -= OnAttachedToVisualTree;
            AssociatedObject.DetachedFromVisualTree -= OnDetachedFromVisualTree;
        }

        base.OnDetaching();
    }

    private void OnAttachedToVisualTree(object? sender, Avalonia.VisualTreeAttachmentEventArgs e)
    {
        Subscribe();
    }

    private void OnDetachedFromVisualTree(object? sender, Avalonia.VisualTreeAttachmentEventArgs e)
    {
        Unsubscribe();
    }

    private void Subscribe()
    {
        if (_isSubscribed || AssociatedObject == null)
            return;

        AssociatedObject.ScrollChanged += OnScrollChanged;
        _isSubscribed = true;

        // Initial scroll to bottom
        if (IsEnabled)
        {
            ScrollToBottom();
        }
    }

    private void Unsubscribe()
    {
        if (!_isSubscribed || AssociatedObject == null)
            return;

        AssociatedObject.ScrollChanged -= OnScrollChanged;
        _isSubscribed = false;
    }

    private void OnScrollChanged(object? sender, ScrollChangedEventArgs e)
    {
        if (!IsEnabled || AssociatedObject == null)
            return;

        // Check if the user is near the bottom
        var offset = AssociatedObject.Offset;
        var extent = AssociatedObject.Extent;
        var viewport = AssociatedObject.Viewport;

        double distanceFromBottom = extent.Y - offset.Y - viewport.Y;

        // If the extent changed (content was added), scroll to bottom if auto-scroll is active
        if (e.ExtentDelta.Y > 0)
        {
            if (_isAutoScrollEnabled)
            {
                ScrollToBottom();
            }
        }

        // Determine if user scrolled to the bottom area
        _isAutoScrollEnabled = distanceFromBottom <= Threshold;
    }

    private void ScrollToBottom()
    {
        if (AssociatedObject == null)
            return;

        // Use dispatcher to ensure layout is up to date
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            if (AssociatedObject != null)
            {
                AssociatedObject.ScrollToEnd();
            }
        }, Avalonia.Threading.DispatcherPriority.Layout);
    }

    /// <summary>
    /// Static getter for AXAML usage.
    /// </summary>
    public static bool GetIsEnabled(ScrollViewer element) => element.GetValue(IsEnabledProperty);

    /// <summary>
    /// Static setter for AXAML usage.
    /// </summary>
    public static void SetIsEnabled(ScrollViewer element, bool value) => element.SetValue(IsEnabledProperty, value);

    /// <summary>
    /// Static getter for AXAML usage.
    /// </summary>
    public static double GetThreshold(ScrollViewer element) => element.GetValue(ThresholdProperty);

    /// <summary>
    /// Static setter for AXAML usage.
    /// </summary>
    public static void SetThreshold(ScrollViewer element, double value) => element.SetValue(ThresholdProperty, value);
}
