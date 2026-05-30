using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Threading;

namespace AuraUI.Controls.Layout;

/// <summary>
/// Specifies when the scrollbar is visible.
/// </summary>
public enum ScrollAreaType
{
    Auto,
    Always,
    Hover,
    Scroll
}

/// <summary>
/// A custom scroll container with styled scrollbars, configurable visibility modes,
/// and scroll position tracking.
///
/// Template parts: PART_Viewport (ScrollViewer)
///
/// Supports size classes: .sm, .md, .lg
/// </summary>
public class ScrollArea : ContentControl
{
    private static readonly TimeSpan ScrollIdleDelay = TimeSpan.FromMilliseconds(650);

    private ScrollViewer? _viewport;
    private readonly DispatcherTimer _scrollIdleTimer;

    /// <summary>
    /// Defines the <see cref="Type"/> styled property.
    /// </summary>
    public static readonly StyledProperty<ScrollAreaType> TypeProperty =
        AvaloniaProperty.Register<ScrollArea, ScrollAreaType>(nameof(Type), ScrollAreaType.Auto);

    /// <summary>
    /// Defines the <see cref="HorizontalScrollBarVisibility"/> styled property.
    /// </summary>
    public static readonly StyledProperty<ScrollBarVisibility> HorizontalScrollBarVisibilityProperty =
        AvaloniaProperty.Register<ScrollArea, ScrollBarVisibility>(nameof(HorizontalScrollBarVisibility), ScrollBarVisibility.Disabled);

    /// <summary>
    /// Defines the <see cref="VerticalScrollBarVisibility"/> styled property.
    /// </summary>
    public static readonly StyledProperty<ScrollBarVisibility> VerticalScrollBarVisibilityProperty =
        AvaloniaProperty.Register<ScrollArea, ScrollBarVisibility>(nameof(VerticalScrollBarVisibility), ScrollBarVisibility.Auto);

    /// <summary>
    /// Defines the <see cref="IsInsetContent"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsInsetContentProperty =
        AvaloniaProperty.Register<ScrollArea, bool>(nameof(IsInsetContent));

    /// <summary>
    /// Defines the <see cref="Offset"/> styled property.
    /// </summary>
    public static readonly StyledProperty<Vector> OffsetProperty =
        AvaloniaProperty.Register<ScrollArea, Vector>(nameof(Offset));

    /// <summary>
    /// Defines the <see cref="Extent"/> styled property.
    /// </summary>
    public static readonly StyledProperty<Size> ExtentProperty =
        AvaloniaProperty.Register<ScrollArea, Size>(nameof(Extent));

    /// <summary>
    /// Defines the <see cref="Viewport"/> styled property.
    /// </summary>
    public static readonly StyledProperty<Size> ViewportProperty =
        AvaloniaProperty.Register<ScrollArea, Size>(nameof(Viewport));

    /// <summary>
    /// Defines the <see cref="IsScrolling"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsScrollingProperty =
        AvaloniaProperty.Register<ScrollArea, bool>(nameof(IsScrolling));

    /// <summary>
    /// Defines the <see cref="CanScrollHorizontally"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> CanScrollHorizontallyProperty =
        AvaloniaProperty.Register<ScrollArea, bool>(nameof(CanScrollHorizontally));

    /// <summary>
    /// Defines the <see cref="CanScrollVertically"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> CanScrollVerticallyProperty =
        AvaloniaProperty.Register<ScrollArea, bool>(nameof(CanScrollVertically));

    /// <summary>
    /// Defines the <see cref="IsAtTop"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsAtTopProperty =
        AvaloniaProperty.Register<ScrollArea, bool>(nameof(IsAtTop), true);

    /// <summary>
    /// Defines the <see cref="IsAtBottom"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsAtBottomProperty =
        AvaloniaProperty.Register<ScrollArea, bool>(nameof(IsAtBottom), true);

    static ScrollArea()
    {
        TypeProperty.Changed.AddClassHandler<ScrollArea>((x, _) => x.SyncClasses());
        HorizontalScrollBarVisibilityProperty.Changed.AddClassHandler<ScrollArea>((x, _) => x.SyncClasses());
        VerticalScrollBarVisibilityProperty.Changed.AddClassHandler<ScrollArea>((x, _) => x.SyncClasses());
        IsInsetContentProperty.Changed.AddClassHandler<ScrollArea>((x, _) => x.SyncClasses());
        IsScrollingProperty.Changed.AddClassHandler<ScrollArea>((x, _) => x.SyncClasses());
        CanScrollHorizontallyProperty.Changed.AddClassHandler<ScrollArea>((x, _) => x.SyncClasses());
        CanScrollVerticallyProperty.Changed.AddClassHandler<ScrollArea>((x, _) => x.SyncClasses());
        IsAtTopProperty.Changed.AddClassHandler<ScrollArea>((x, _) => x.SyncClasses());
        IsAtBottomProperty.Changed.AddClassHandler<ScrollArea>((x, _) => x.SyncClasses());
    }

    public ScrollArea()
    {
        ClipToBounds = true;
        _scrollIdleTimer = new DispatcherTimer { Interval = ScrollIdleDelay };
        _scrollIdleTimer.Tick += OnScrollIdleTimerTick;
        SyncClasses();
    }

    /// <summary>
    /// Occurs when the scroll position changes.
    /// </summary>
    public event EventHandler<ScrollChangedEventArgs>? ScrollChanged;

    /// <summary>
    /// Gets or sets the scrollbar visibility type.
    /// </summary>
    public ScrollAreaType Type
    {
        get => GetValue(TypeProperty);
        set => SetValue(TypeProperty, value);
    }

    /// <summary>
    /// Gets or sets the horizontal scrollbar visibility.
    /// </summary>
    public ScrollBarVisibility HorizontalScrollBarVisibility
    {
        get => GetValue(HorizontalScrollBarVisibilityProperty);
        set => SetValue(HorizontalScrollBarVisibilityProperty, value);
    }

    /// <summary>
    /// Gets or sets the vertical scrollbar visibility.
    /// </summary>
    public ScrollBarVisibility VerticalScrollBarVisibility
    {
        get => GetValue(VerticalScrollBarVisibilityProperty);
        set => SetValue(VerticalScrollBarVisibilityProperty, value);
    }

    /// <summary>
    /// Gets or sets whether content is inset from the scrollbar.
    /// </summary>
    public bool IsInsetContent
    {
        get => GetValue(IsInsetContentProperty);
        set => SetValue(IsInsetContentProperty, value);
    }

    /// <summary>
    /// Gets the current scroll offset.
    /// </summary>
    public Vector Offset => GetValue(OffsetProperty);

    /// <summary>
    /// Gets the total extent of the scrollable content.
    /// </summary>
    public Size Extent => GetValue(ExtentProperty);

    /// <summary>
    /// Gets the size of the visible viewport.
    /// </summary>
    public Size Viewport => GetValue(ViewportProperty);

    /// <summary>
    /// Gets whether the user is currently scrolling.
    /// </summary>
    public bool IsScrolling => GetValue(IsScrollingProperty);

    /// <summary>
    /// Gets whether horizontal scrolling is possible.
    /// </summary>
    public bool CanScrollHorizontally => GetValue(CanScrollHorizontallyProperty);

    /// <summary>
    /// Gets whether vertical scrolling is possible.
    /// </summary>
    public bool CanScrollVertically => GetValue(CanScrollVerticallyProperty);

    /// <summary>
    /// Gets whether the scroll position is at the top.
    /// </summary>
    public bool IsAtTop => GetValue(IsAtTopProperty);

    /// <summary>
    /// Gets whether the scroll position is at the bottom.
    /// </summary>
    public bool IsAtBottom => GetValue(IsAtBottomProperty);

    /// <summary>
    /// Scrolls to the top of the content.
    /// </summary>
    public bool ScrollToTop()
    {
        return SetOffset(new Vector(Offset.X, 0));
    }

    /// <summary>
    /// Scrolls to the bottom of the content.
    /// </summary>
    public bool ScrollToBottom()
    {
        return SetOffset(new Vector(Offset.X, Math.Max(0, Extent.Height - Viewport.Height)));
    }

    /// <summary>
    /// Sets the scroll offset directly.
    /// </summary>
    public bool SetOffset(Vector offset)
    {
        if (_viewport is null) return false;

        var maxX = Math.Max(0, _viewport.Extent.Width - _viewport.Viewport.Width);
        var maxY = Math.Max(0, _viewport.Extent.Height - _viewport.Viewport.Height);
        _viewport.Offset = new Vector(Math.Clamp(offset.X, 0, maxX), Math.Clamp(offset.Y, 0, maxY));
        SyncScrollMetrics();
        return true;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        if (_viewport is not null)
        {
            _viewport.ScrollChanged -= OnViewportScrollChanged;
        }

        base.OnApplyTemplate(e);

        _viewport = e.NameScope.Find<ScrollViewer>("PART_Viewport");

        if (_viewport is not null)
        {
            _viewport.ScrollChanged += OnViewportScrollChanged;
            SyncScrollMetrics();
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        _scrollIdleTimer.Stop();
        base.OnDetachedFromVisualTree(e);
    }

    private void OnViewportScrollChanged(object? sender, ScrollChangedEventArgs e)
    {
        SetValue(IsScrollingProperty, true);
        _scrollIdleTimer.Stop();
        _scrollIdleTimer.Start();
        SyncScrollMetrics();
        ScrollChanged?.Invoke(this, e);
    }

    private void OnScrollIdleTimerTick(object? sender, EventArgs e)
    {
        _scrollIdleTimer.Stop();
        SetValue(IsScrollingProperty, false);
    }

    private void SyncScrollMetrics()
    {
        if (_viewport is null) return;

        SetValue(OffsetProperty, _viewport.Offset);
        SetValue(ExtentProperty, _viewport.Extent);
        SetValue(ViewportProperty, _viewport.Viewport);

        var offset = _viewport.Offset;
        var extent = _viewport.Extent;
        var viewport = _viewport.Viewport;
        var maxX = Math.Max(0, extent.Width - viewport.Width);
        var maxY = Math.Max(0, extent.Height - viewport.Height);

        SetValue(CanScrollHorizontallyProperty, maxX > 0.5);
        SetValue(CanScrollVerticallyProperty, maxY > 0.5);
        SetValue(IsAtTopProperty, offset.Y <= 0.5);
        SetValue(IsAtBottomProperty, offset.Y >= maxY - 0.5);
        SyncClasses();
    }

    private void SyncClasses()
    {
        Classes.Set("type-auto", Type == ScrollAreaType.Auto);
        Classes.Set("type-always", Type == ScrollAreaType.Always);
        Classes.Set("type-hover", Type == ScrollAreaType.Hover);
        Classes.Set("type-scroll", Type == ScrollAreaType.Scroll);
        Classes.Set("horizontal-disabled", HorizontalScrollBarVisibility == ScrollBarVisibility.Disabled);
        Classes.Set("horizontal-auto", HorizontalScrollBarVisibility == ScrollBarVisibility.Auto);
        Classes.Set("horizontal-visible", HorizontalScrollBarVisibility == ScrollBarVisibility.Visible);
        Classes.Set("vertical-disabled", VerticalScrollBarVisibility == ScrollBarVisibility.Disabled);
        Classes.Set("vertical-auto", VerticalScrollBarVisibility == ScrollBarVisibility.Auto);
        Classes.Set("vertical-visible", VerticalScrollBarVisibility == ScrollBarVisibility.Visible);
        Classes.Set("inset-content", IsInsetContent);
        Classes.Set("scrolling", IsScrolling);
        Classes.Set("can-scroll-x", CanScrollHorizontally);
        Classes.Set("can-scroll-y", CanScrollVertically);
        Classes.Set("at-top", IsAtTop);
        Classes.Set("at-bottom", IsAtBottom);
    }
}
