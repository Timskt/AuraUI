using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

namespace AuraUI.Controls.Navigation;

/// <summary>
/// Specifies the position of tabs relative to content.
/// </summary>
public enum TabPosition
{
    Top,
    Left
}

/// <summary>
/// Specifies how tab widths are determined.
/// </summary>
public enum TabWidthMode
{
    Equal,
    SizeToContent,
    Fixed
}

/// <summary>
/// Event args for the <see cref="AuraTabControl.TabClosed"/> event.
/// </summary>
public class TabClosedEventArgs : RoutedEventArgs
{
    /// <summary>
    /// Gets the index of the closed tab.
    /// </summary>
    public int TabIndex { get; }

    /// <summary>
    /// Gets the header of the closed tab.
    /// </summary>
    public object? Header { get; }

    public TabClosedEventArgs(int tabIndex, object? header) : base()
    {
        TabIndex = tabIndex;
        Header = header;
    }

    public TabClosedEventArgs(RoutedEvent routedEvent, int tabIndex, object? header) : base(routedEvent)
    {
        TabIndex = tabIndex;
        Header = header;
    }
}

/// <summary>
/// Event args for the <see cref="AuraTabControl.TabCloseRequested"/> event.
/// Allows the handler to cancel the close operation.
/// </summary>
public class TabCloseRequestedEventArgs : RoutedEventArgs
{
    /// <summary>
    /// Gets the index of the tab requested to be closed.
    /// </summary>
    public int TabIndex { get; }

    /// <summary>
    /// Gets the header of the tab requested to be closed.
    /// </summary>
    public object? Header { get; }

    /// <summary>
    /// Gets or sets whether the close operation should be cancelled.
    /// </summary>
    public bool Cancel { get; set; }

    public TabCloseRequestedEventArgs(int tabIndex, object? header) : base()
    {
        TabIndex = tabIndex;
        Header = header;
    }

    public TabCloseRequestedEventArgs(RoutedEvent routedEvent, int tabIndex, object? header) : base(routedEvent)
    {
        TabIndex = tabIndex;
        Header = header;
    }
}

/// <summary>
/// Event args for the <see cref="AuraTabControl.TabReordered"/> event.
/// </summary>
public class TabReorderedEventArgs : RoutedEventArgs
{
    /// <summary>
    /// Gets the original index of the reordered tab.
    /// </summary>
    public int OldIndex { get; }

    /// <summary>
    /// Gets the new index of the reordered tab.
    /// </summary>
    public int NewIndex { get; }

    public TabReorderedEventArgs(int oldIndex, int newIndex) : base()
    {
        OldIndex = oldIndex;
        NewIndex = newIndex;
    }

    public TabReorderedEventArgs(RoutedEvent routedEvent, int oldIndex, int newIndex) : base(routedEvent)
    {
        OldIndex = oldIndex;
        NewIndex = newIndex;
    }
}

/// <summary>
/// An extended tab control supporting tab positions (top/left), closable tabs, width modes,
/// visual styles (.card, .pill), add-tab button, drag-to-reorder, and overflow scrolling.
/// </summary>
[TemplatePart("PART_HeaderItemsControl", typeof(ItemsControl))]
[TemplatePart("PART_ContentPresenter", typeof(ContentPresenter))]
[TemplatePart("PART_AddTabButton", typeof(Button))]
[TemplatePart("PART_ScrollLeftButton", typeof(Button))]
[TemplatePart("PART_ScrollRightButton", typeof(Button))]
[TemplatePart("PART_TabStripScrollViewer", typeof(ScrollViewer))]
[PseudoClasses(":top", ":left", ":card", ":pill")]
public class AuraTabControl : TabControl
{
    private Button? _addTabButton;
    private Button? _scrollLeftButton;
    private Button? _scrollRightButton;
    private ScrollViewer? _tabStripScrollViewer;
    private int _dragSourceIndex = -1;
    private bool _isDragging;

    /// <summary>
    /// Defines the <see cref="TabPosition"/> styled property.
    /// </summary>
    public static readonly StyledProperty<TabPosition> TabPositionProperty =
        AvaloniaProperty.Register<AuraTabControl, TabPosition>(nameof(TabPosition), TabPosition.Top);

    /// <summary>
    /// Defines the <see cref="IsClosable"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsClosableProperty =
        AvaloniaProperty.Register<AuraTabControl, bool>(nameof(IsClosable));

    /// <summary>
    /// Defines the <see cref="TabWidthMode"/> styled property.
    /// </summary>
    public static readonly StyledProperty<TabWidthMode> TabWidthModeProperty =
        AvaloniaProperty.Register<AuraTabControl, TabWidthMode>(nameof(TabWidthMode), TabWidthMode.Equal);

    /// <summary>
    /// Defines the <see cref="FixedTabWidth"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> FixedTabWidthProperty =
        AvaloniaProperty.Register<AuraTabControl, double>(nameof(FixedTabWidth), 120);

    /// <summary>
    /// Defines the <see cref="ShowAddTabButton"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowAddTabButtonProperty =
        AvaloniaProperty.Register<AuraTabControl, bool>(nameof(ShowAddTabButton));

    /// <summary>
    /// Defines the <see cref="IsDragReorderEnabled"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsDragReorderEnabledProperty =
        AvaloniaProperty.Register<AuraTabControl, bool>(nameof(IsDragReorderEnabled));

    /// <summary>
    /// Defines the <see cref="IsScrollable"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsScrollableProperty =
        AvaloniaProperty.Register<AuraTabControl, bool>(nameof(IsScrollable));

    /// <summary>
    /// Defines the routed event for tab closed.
    /// </summary>
    public static readonly RoutedEvent<TabClosedEventArgs> TabClosedEvent =
        RoutedEvent.Register<AuraTabControl, TabClosedEventArgs>(nameof(TabClosed), RoutingStrategies.Bubble);

    /// <summary>
    /// Defines the routed event for tab close requested (cancellable).
    /// </summary>
    public static readonly RoutedEvent<TabCloseRequestedEventArgs> TabCloseRequestedEvent =
        RoutedEvent.Register<AuraTabControl, TabCloseRequestedEventArgs>(nameof(TabCloseRequested), RoutingStrategies.Bubble);

    /// <summary>
    /// Defines the routed event for add tab button click.
    /// </summary>
    public static readonly RoutedEvent<RoutedEventArgs> AddTabEvent =
        RoutedEvent.Register<AuraTabControl, RoutedEventArgs>(nameof(AddTab), RoutingStrategies.Bubble);

    /// <summary>
    /// Defines the routed event for tab drag reorder.
    /// </summary>
    public static readonly RoutedEvent<TabReorderedEventArgs> TabReorderedEvent =
        RoutedEvent.Register<AuraTabControl, TabReorderedEventArgs>(nameof(TabReordered), RoutingStrategies.Bubble);

    static AuraTabControl()
    {
        TabPositionProperty.Changed.AddClassHandler<AuraTabControl>((x, _) => x.UpdatePseudoClasses());
        IsClosableProperty.Changed.AddClassHandler<AuraTabControl>((x, _) => x.UpdatePseudoClasses());
        TabWidthModeProperty.Changed.AddClassHandler<AuraTabControl>((x, _) => x.UpdatePseudoClasses());
        ShowAddTabButtonProperty.Changed.AddClassHandler<AuraTabControl>((x, _) => x.UpdateAddTabButtonVisibility());
        IsScrollableProperty.Changed.AddClassHandler<AuraTabControl>((x, _) => x.UpdateScrollButtons());
    }

    /// <summary>
    /// Gets or sets the tab header position.
    /// </summary>
    public TabPosition TabPosition
    {
        get => GetValue(TabPositionProperty);
        set => SetValue(TabPositionProperty, value);
    }

    /// <summary>
    /// Gets or sets whether tabs show a close button.
    /// </summary>
    public bool IsClosable
    {
        get => GetValue(IsClosableProperty);
        set => SetValue(IsClosableProperty, value);
    }

    /// <summary>
    /// Gets or sets how tab widths are calculated.
    /// </summary>
    public TabWidthMode TabWidthMode
    {
        get => GetValue(TabWidthModeProperty);
        set => SetValue(TabWidthModeProperty, value);
    }

    /// <summary>
    /// Gets or sets the fixed width of tabs when <see cref="TabWidthMode"/> is Fixed.
    /// </summary>
    public double FixedTabWidth
    {
        get => GetValue(FixedTabWidthProperty);
        set => SetValue(FixedTabWidthProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the add-tab button is shown at the end of the tab strip.
    /// </summary>
    public bool ShowAddTabButton
    {
        get => GetValue(ShowAddTabButtonProperty);
        set => SetValue(ShowAddTabButtonProperty, value);
    }

    /// <summary>
    /// Gets or sets whether tabs can be reordered by dragging.
    /// </summary>
    public bool IsDragReorderEnabled
    {
        get => GetValue(IsDragReorderEnabledProperty);
        set => SetValue(IsDragReorderEnabledProperty, value);
    }

    /// <summary>
    /// Gets or sets whether scroll buttons appear when tabs overflow.
    /// </summary>
    public bool IsScrollable
    {
        get => GetValue(IsScrollableProperty);
        set => SetValue(IsScrollableProperty, value);
    }

    /// <summary>
    /// Occurs when a tab's close button is clicked.
    /// </summary>
    public event EventHandler<TabClosedEventArgs>? TabClosed
    {
        add => AddHandler(TabClosedEvent, value);
        remove => RemoveHandler(TabClosedEvent, value);
    }

    /// <summary>
    /// Occurs when a tab close is requested. The handler can set Cancel to prevent closing.
    /// </summary>
    public event EventHandler<TabCloseRequestedEventArgs>? TabCloseRequested
    {
        add => AddHandler(TabCloseRequestedEvent, value);
        remove => RemoveHandler(TabCloseRequestedEvent, value);
    }

    /// <summary>
    /// Occurs when the add-tab button is clicked.
    /// </summary>
    public event EventHandler<RoutedEventArgs>? AddTab
    {
        add => AddHandler(AddTabEvent, value);
        remove => RemoveHandler(AddTabEvent, value);
    }

    /// <summary>
    /// Occurs when a tab is reordered via drag-and-drop.
    /// </summary>
    public event EventHandler<TabReorderedEventArgs>? TabReordered
    {
        add => AddHandler(TabReorderedEvent, value);
        remove => RemoveHandler(TabReorderedEvent, value);
    }

    /// <summary>
    /// Requests to close the tab at the specified index. Honors the cancellable TabCloseRequested event.
    /// </summary>
    public void RequestCloseTab(int index)
    {
        var items = ItemsSource;
        if (items == null) return;

        var itemList = items.Cast<object>().ToList();
        if (index < 0 || index >= itemList.Count) return;

        var header = itemList[index];

        // Raise cancellable event first
        var requestArgs = new TabCloseRequestedEventArgs(TabCloseRequestedEvent, index, header);
        RaiseEvent(requestArgs);

        if (requestArgs.Cancel) return;

        CloseTab(index);
    }

    /// <summary>
    /// Closes the tab at the specified index.
    /// </summary>
    public void CloseTab(int index)
    {
        var items = ItemsSource;
        if (items == null) return;

        var itemList = items.Cast<object>().ToList();
        if (index < 0 || index >= itemList.Count) return;

        var header = itemList[index];
        var args = new TabClosedEventArgs(TabClosedEvent, index, header);
        RaiseEvent(args);

        // Remove the item from the items source if it's a list
        if (ItemsSource is IList<object> mutableList)
        {
            mutableList.RemoveAt(index);
        }

        // Adjust selected index
        if (SelectedIndex >= itemList.Count - 1)
        {
            SelectedIndex = Math.Max(0, itemList.Count - 2);
        }
    }

    /// <summary>
    /// Reorders a tab from one position to another.
    /// </summary>
    public void ReorderTab(int oldIndex, int newIndex)
    {
        if (oldIndex == newIndex) return;
        if (ItemsSource is not IList<object> mutableList) return;

        var count = mutableList.Count;
        if (oldIndex < 0 || oldIndex >= count) return;
        if (newIndex < 0 || newIndex >= count) return;

        var item = mutableList[oldIndex];
        mutableList.RemoveAt(oldIndex);
        mutableList.Insert(newIndex, item);

        SelectedIndex = newIndex;

        var args = new TabReorderedEventArgs(TabReorderedEvent, oldIndex, newIndex);
        RaiseEvent(args);
    }

    /// <summary>
    /// Scrolls the tab strip to show the next tab.
    /// </summary>
    public void ScrollNext()
    {
        if (_tabStripScrollViewer != null)
        {
            _tabStripScrollViewer.Offset = new Vector(
                _tabStripScrollViewer.Offset.X + 100,
                _tabStripScrollViewer.Offset.Y);
        }
    }

    /// <summary>
    /// Scrolls the tab strip to show the previous tab.
    /// </summary>
    public void ScrollPrevious()
    {
        if (_tabStripScrollViewer != null)
        {
            _tabStripScrollViewer.Offset = new Vector(
                Math.Max(0, _tabStripScrollViewer.Offset.X - 100),
                _tabStripScrollViewer.Offset.Y);
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        // Unsubscribe from old parts
        if (_addTabButton != null)
            _addTabButton.Click -= OnAddTabClick;
        if (_scrollLeftButton != null)
            _scrollLeftButton.Click -= OnScrollLeftClick;
        if (_scrollRightButton != null)
            _scrollRightButton.Click -= OnScrollRightClick;
        if (_tabStripScrollViewer != null)
            _tabStripScrollViewer.ScrollChanged -= OnTabStripScrollChanged;

        // Find new template parts
        _addTabButton = e.NameScope.Find<Button>("PART_AddTabButton");
        _scrollLeftButton = e.NameScope.Find<Button>("PART_ScrollLeftButton");
        _scrollRightButton = e.NameScope.Find<Button>("PART_ScrollRightButton");
        _tabStripScrollViewer = e.NameScope.Find<ScrollViewer>("PART_TabStripScrollViewer");

        // Subscribe to new parts
        if (_addTabButton != null)
            _addTabButton.Click += OnAddTabClick;
        if (_scrollLeftButton != null)
            _scrollLeftButton.Click += OnScrollLeftClick;
        if (_scrollRightButton != null)
            _scrollRightButton.Click += OnScrollRightClick;
        if (_tabStripScrollViewer != null)
            _tabStripScrollViewer.ScrollChanged += OnTabStripScrollChanged;

        UpdatePseudoClasses();
        UpdateAddTabButtonVisibility();
        UpdateScrollButtons();
    }

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);
        if (container is TabItem tabItem)
        {
            tabItem.Classes.Add("auratab");

            // Enable drag-to-reorder if configured
            if (IsDragReorderEnabled)
            {
                tabItem.PointerPressed += OnTabPointerPressed;
                tabItem.PointerMoved += OnTabPointerMoved;
                tabItem.PointerReleased += OnTabPointerReleased;
            }
        }
    }

    protected override void ClearContainerForItemOverride(Control container)
    {
        base.ClearContainerForItemOverride(container);
        if (container is TabItem tabItem)
        {
            tabItem.PointerPressed -= OnTabPointerPressed;
            tabItem.PointerMoved -= OnTabPointerMoved;
            tabItem.PointerReleased -= OnTabPointerReleased;
        }
    }

    private void OnAddTabClick(object? sender, RoutedEventArgs e)
    {
        RaiseEvent(new RoutedEventArgs(AddTabEvent));
    }

    private void OnScrollLeftClick(object? sender, RoutedEventArgs e) => ScrollPrevious();
    private void OnScrollRightClick(object? sender, RoutedEventArgs e) => ScrollNext();

    private void OnTabStripScrollChanged(object? sender, ScrollChangedEventArgs e)
    {
        UpdateScrollButtons();
    }

    private void OnTabPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (!IsDragReorderEnabled || _isDragging || sender is not TabItem tabItem) return;

        var properties = e.GetCurrentPoint(this).Properties;
        if (!properties.IsLeftButtonPressed) return;

        _dragSourceIndex = GetTabIndex(tabItem);
        _isDragging = false;
        e.Pointer.Capture(tabItem);
    }

    private void OnTabPointerMoved(object? sender, PointerEventArgs e)
    {
        if (_dragSourceIndex < 0 || sender is not TabItem draggedTab) { _isDragging = false; return; }

        var position = e.GetPosition(this);
        var parent = draggedTab.GetVisualParent();
        if (parent == null) return;

        // Find the tab strip area
        var children = parent.GetVisualChildren().OfType<TabItem>().ToList();
        for (int i = 0; i < children.Count; i++)
        {
            var tab = children[i];
            if (tab == draggedTab) continue;

            var tabBounds = tab.Bounds;
            var tabCenter = tab.Bounds.X + tab.Bounds.Width / 2;
            var localPos = e.GetPosition(parent);

            if (localPos.X > tabBounds.X && localPos.X < tabBounds.X + tabBounds.Width)
            {
                var targetIndex = i;
                if (targetIndex != _dragSourceIndex)
                {
                    _isDragging = true;
                    ReorderTab(_dragSourceIndex, targetIndex);
                    _dragSourceIndex = targetIndex;
                }
                break;
            }
        }
    }

    private void OnTabPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        _dragSourceIndex = -1;
        _isDragging = false;
        e.Pointer.Capture(null);
    }

    private int GetTabIndex(TabItem tabItem)
    {
        if (tabItem.GetVisualParent() is ItemsControl itemsControl)
        {
            var index = 0;
            foreach (var child in itemsControl.GetVisualChildren())
            {
                if (child == tabItem) return index;
                if (child is TabItem) index++;
            }
        }
        return -1;
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":top", TabPosition == TabPosition.Top);
        PseudoClasses.Set(":left", TabPosition == TabPosition.Left);
        PseudoClasses.Set(":card", TabWidthMode == TabWidthMode.Equal);
        PseudoClasses.Set(":pill", TabWidthMode == TabWidthMode.SizeToContent);
    }

    private void UpdateAddTabButtonVisibility()
    {
        if (_addTabButton != null)
        {
            _addTabButton.IsVisible = ShowAddTabButton;
        }
    }

    private void UpdateScrollButtons()
    {
        if (!IsScrollable || _tabStripScrollViewer == null)
        {
            if (_scrollLeftButton != null) _scrollLeftButton.IsVisible = false;
            if (_scrollRightButton != null) _scrollRightButton.IsVisible = false;
            return;
        }

        var canScrollLeft = _tabStripScrollViewer.Offset.X > 0;
        var canScrollRight = _tabStripScrollViewer.Offset.X < _tabStripScrollViewer.Extent.Width - _tabStripScrollViewer.Viewport.Width;

        if (_scrollLeftButton != null) _scrollLeftButton.IsVisible = canScrollLeft;
        if (_scrollRightButton != null) _scrollRightButton.IsVisible = canScrollRight;
    }
}
