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

    /// <summary>
    /// Gets or sets whether auto-scrolling is enabled.
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the threshold (in pixels) from the bottom to consider "at bottom".
    /// When the user is within this distance from the bottom, auto-scrolling will be active.
    /// </summary>
    public double Threshold { get; set; } = 50.0;

    protected override void OnAttached()
    {
        base.OnAttached();

        if (AssociatedObject == null)
            return;

        AssociatedObject.AttachedToVisualTree += OnAttachedToVisualTree;
        AssociatedObject.DetachedFromVisualTree += OnDetachedFromVisualTree;

        // If already in tree, subscribe via AttachedToVisualTree handler
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

        double distanceFromBottom = extent.Height - offset.Y - viewport.Height;

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
        }, Avalonia.Threading.DispatcherPriority.Render);
    }

}
