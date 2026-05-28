using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;

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
/// An extended tab control supporting tab positions (top/left), closable tabs, width modes,
/// and visual styles (.card, .pill).
/// </summary>
[TemplatePart("PART_HeaderItemsControl", typeof(ItemsControl))]
[TemplatePart("PART_ContentPresenter", typeof(ContentPresenter))]
[PseudoClasses(":top", ":left", ":card", ":pill")]
public class AuraTabControl : TabControl
{
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
    /// Defines the routed event for tab closed.
    /// </summary>
    public static readonly RoutedEvent<TabClosedEventArgs> TabClosedEvent =
        RoutedEvent.Register<AuraTabControl, TabClosedEventArgs>(nameof(TabClosed), RoutingStrategies.Bubble);

    static AuraTabControl()
    {
        TabPositionProperty.Changed.AddClassHandler<AuraTabControl>((x, _) => x.UpdatePseudoClasses());
        IsClosableProperty.Changed.AddClassHandler<AuraTabControl>((x, _) => x.UpdatePseudoClasses());
        TabWidthModeProperty.Changed.AddClassHandler<AuraTabControl>((x, _) => x.UpdatePseudoClasses());
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
    /// Occurs when a tab's close button is clicked.
    /// </summary>
    public event EventHandler<TabClosedEventArgs>? TabClosed
    {
        add => AddHandler(TabClosedEvent, value);
        remove => RemoveHandler(TabClosedEvent, value);
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

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        UpdatePseudoClasses();
    }

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);
        if (container is TabItem tabItem)
        {
            tabItem.Classes.Add("auratab");
        }
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":top", TabPosition == TabPosition.Top);
        PseudoClasses.Set(":left", TabPosition == TabPosition.Left);
        PseudoClasses.Set(":card", TabWidthMode == TabWidthMode.Equal);
        PseudoClasses.Set(":pill", TabWidthMode == TabWidthMode.SizeToContent);
    }
}
