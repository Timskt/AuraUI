using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Layout;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace AuraUI.Controls.Layout;

/// <summary>
/// A scroll-to-top button that appears when the user scrolls down,
/// inspired by Ant Design's BackTop component.
/// </summary>
[PseudoClasses(":visible", ":hidden")]
public class BackTop : ContentControl
{
    private ScrollViewer? _scrollViewer;

    /// <summary>
    /// Defines the <see cref="VisibilityHeight"/> styled property.
    /// The scroll offset (in pixels) at which the button becomes visible.
    /// </summary>
    public static readonly StyledProperty<double> VisibilityHeightProperty =
        AvaloniaProperty.Register<BackTop, double>(nameof(VisibilityHeight), 400);

    /// <summary>
    /// Defines the <see cref="Target"/> styled property.
    /// The scroll container to listen to.
    /// </summary>
    public static readonly StyledProperty<ScrollViewer?> TargetProperty =
        AvaloniaProperty.Register<BackTop, ScrollViewer?>(nameof(Target));

    /// <summary>
    /// Defines the <see cref="Duration"/> styled property.
    /// The duration of the smooth scroll animation.
    /// </summary>
    public static readonly StyledProperty<TimeSpan> DurationProperty =
        AvaloniaProperty.Register<BackTop, TimeSpan>(nameof(Duration), TimeSpan.FromMilliseconds(300));

    /// <summary>
    /// Defines the <see cref="Right"/> styled property.
    /// Distance from the right edge.
    /// </summary>
    public static readonly StyledProperty<double> RightProperty =
        AvaloniaProperty.Register<BackTop, double>(nameof(Right), 40);

    /// <summary>
    /// Defines the <see cref="Bottom"/> styled property.
    /// Distance from the bottom edge.
    /// </summary>
    public static readonly StyledProperty<double> BottomProperty =
        AvaloniaProperty.Register<BackTop, double>(nameof(Bottom), 40);

    /// <summary>
    /// Occurs when the back-to-top button is clicked.
    /// </summary>
    public event EventHandler? Click;

    static BackTop()
    {
        AffectsArrange<BackTop>(RightProperty, BottomProperty);
    }

    /// <summary>
    /// Gets or sets the scroll offset at which the button becomes visible.
    /// </summary>
    public double VisibilityHeight
    {
        get => GetValue(VisibilityHeightProperty);
        set => SetValue(VisibilityHeightProperty, value);
    }

    /// <summary>
    /// Gets or sets the target scroll container.
    /// </summary>
    public ScrollViewer? Target
    {
        get => GetValue(TargetProperty);
        set => SetValue(TargetProperty, value);
    }

    /// <summary>
    /// Gets or sets the smooth scroll duration.
    /// </summary>
    public TimeSpan Duration
    {
        get => GetValue(DurationProperty);
        set => SetValue(DurationProperty, value);
    }

    /// <summary>
    /// Gets or sets the distance from the right edge of the container.
    /// </summary>
    public double Right
    {
        get => GetValue(RightProperty);
        set => SetValue(RightProperty, value);
    }

    /// <summary>
    /// Gets or sets the distance from the bottom edge of the container.
    /// </summary>
    public double Bottom
    {
        get => GetValue(BottomProperty);
        set => SetValue(BottomProperty, value);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        FindAndSubscribeScrollViewer();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        UnsubscribeScrollViewer();
    }

    protected override void OnPointerPressed(Avalonia.Input.PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        ScrollToTop();
        Click?.Invoke(this, EventArgs.Empty);
    }

    private void FindAndSubscribeScrollViewer()
    {
        UnsubscribeScrollViewer();
        _scrollViewer = Target ?? this.FindAncestorOfType<ScrollViewer>();
        if (_scrollViewer != null)
        {
            _scrollViewer.ScrollChanged += OnScrollChanged;
            UpdateVisibility();
        }
    }

    private void UnsubscribeScrollViewer()
    {
        if (_scrollViewer != null)
        {
            _scrollViewer.ScrollChanged -= OnScrollChanged;
            _scrollViewer = null;
        }
    }

    private void OnScrollChanged(object? sender, ScrollChangedEventArgs e)
    {
        UpdateVisibility();
    }

    private void UpdateVisibility()
    {
        if (_scrollViewer == null) return;

        var isVisible = _scrollViewer.Offset.Y >= VisibilityHeight;
        PseudoClasses.Set(":visible", isVisible);
        PseudoClasses.Set(":hidden", !isVisible);
        IsVisible = isVisible;
    }

    private void ScrollToTop()
    {
        if (_scrollViewer == null) return;

        _scrollViewer.Offset = new Vector(_scrollViewer.Offset.X, 0);
    }
}
