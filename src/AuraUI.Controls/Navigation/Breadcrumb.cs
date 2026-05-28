using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;

namespace AuraUI.Controls.Navigation;

/// <summary>
/// Event args for breadcrumb item click.
/// </summary>
public class BreadcrumbItemClickEventArgs : RoutedEventArgs
{
    /// <summary>
    /// Gets the clicked item.
    /// </summary>
    public object? Item { get; }

    /// <summary>
    /// Gets the index of the clicked item.
    /// </summary>
    public int Index { get; }

    public BreadcrumbItemClickEventArgs(object? item, int index)
    {
        Item = item;
        Index = index;
    }

    public BreadcrumbItemClickEventArgs(RoutedEvent routedEvent, object? item, int index) : base(routedEvent)
    {
        Item = item;
        Index = index;
    }
}

/// <summary>
/// A breadcrumb navigation control with separator, overflow collapsing,
/// and clickable items for hierarchical navigation.
/// </summary>
[PseudoClasses(":overflow")]
public class Breadcrumb : ItemsControl
{
    /// <summary>
    /// Defines the <see cref="Separator"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string> SeparatorProperty =
        AvaloniaProperty.Register<Breadcrumb, string>(nameof(Separator), "/");

    /// <summary>
    /// Defines the <see cref="MaxItems"/> styled property.
    /// </summary>
    public static readonly StyledProperty<int> MaxItemsProperty =
        AvaloniaProperty.Register<Breadcrumb, int>(nameof(MaxItems), 0);

    /// <summary>
    /// Defines the routed event for item click.
    /// </summary>
    public static readonly RoutedEvent<BreadcrumbItemClickEventArgs> ItemClickEvent =
        RoutedEvent.Register<Breadcrumb, BreadcrumbItemClickEventArgs>(nameof(ItemClick), RoutingStrategies.Bubble);

    /// <summary>
    /// Defines the <see cref="DisplayMemberPath"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> DisplayMemberPathProperty =
        AvaloniaProperty.Register<Breadcrumb, string?>(nameof(DisplayMemberPath));

    static Breadcrumb()
    {
        SeparatorProperty.Changed.AddClassHandler<Breadcrumb>((x, _) => x.InvalidateMeasure());
        MaxItemsProperty.Changed.AddClassHandler<Breadcrumb>((x, _) => x.InvalidateMeasure());
    }

    /// <summary>
    /// Gets or sets the separator string between breadcrumb items.
    /// </summary>
    public string Separator
    {
        get => GetValue(SeparatorProperty);
        set => SetValue(SeparatorProperty, value);
    }

    /// <summary>
    /// Gets or sets the maximum number of visible items. Items beyond this are collapsed with "...".
    /// Set to 0 for unlimited.
    /// </summary>
    public int MaxItems
    {
        get => GetValue(MaxItemsProperty);
        set => SetValue(MaxItemsProperty, value);
    }

    /// <summary>
    /// Gets or sets the property path to display for each item.
    /// </summary>
    public string? DisplayMemberPath
    {
        get => GetValue(DisplayMemberPathProperty);
        set => SetValue(DisplayMemberPathProperty, value);
    }

    /// <summary>
    /// Occurs when a breadcrumb item is clicked.
    /// </summary>
    public event EventHandler<BreadcrumbItemClickEventArgs>? ItemClick
    {
        add => AddHandler(ItemClickEvent, value);
        remove => RemoveHandler(ItemClickEvent, value);
    }

    /// <summary>
    /// Gets the effective items to display, applying overflow logic.
    /// </summary>
    public IEnumerable<object> GetDisplayItems()
    {
        var allItems = Items.Cast<object>().ToList();
        var max = MaxItems;

        if (max <= 0 || allItems.Count <= max)
        {
            PseudoClasses.Set(":overflow", false);
            return allItems;
        }

        PseudoClasses.Set(":overflow", true);

        // Show first item, "...", and last (max - 2) items
        var result = new List<object> { allItems[0] };
        result.Add("..."); // Overflow indicator
        var tailCount = max - 2;
        if (tailCount < 1) tailCount = 1;
        for (int i = allItems.Count - tailCount; i < allItems.Count; i++)
        {
            result.Add(allItems[i]);
        }
        return result;
    }

    /// <summary>
    /// Raises the ItemClick event for a breadcrumb item.
    /// </summary>
    public void RaiseItemClick(object? item, int index)
    {
        var args = new BreadcrumbItemClickEventArgs(ItemClickEvent, item, index);
        RaiseEvent(args);
    }

    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        return new BreadcrumbItem();
    }

    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        return NeedsContainer<BreadcrumbItem>(item, out recycleKey);
    }

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);
        if (container is BreadcrumbItem breadcrumbItem)
        {
            breadcrumbItem.BreadcrumbParent = this;
            breadcrumbItem.ItemIndex = index;
            breadcrumbItem.SeparatorText = index < ItemCount - 1 ? Separator : string.Empty;
            breadcrumbItem.IsLast = index == ItemCount - 1;
            breadcrumbItem.IsOverflow = item?.ToString() == "...";
        }
    }
}

/// <summary>
/// Represents a single item within a <see cref="Breadcrumb"/>.
/// </summary>
[PseudoClasses(":last", ":overflow", ":clickable")]
public class BreadcrumbItem : ContentControl
{
    /// <summary>
    /// Defines the <see cref="SeparatorText"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> SeparatorTextProperty =
        AvaloniaProperty.Register<BreadcrumbItem, string?>(nameof(SeparatorText));

    /// <summary>
    /// Defines the <see cref="ItemIndex"/> styled property.
    /// </summary>
    public static readonly StyledProperty<int> ItemIndexProperty =
        AvaloniaProperty.Register<BreadcrumbItem, int>(nameof(ItemIndex));

    /// <summary>
    /// Defines the <see cref="IsLast"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsLastProperty =
        AvaloniaProperty.Register<BreadcrumbItem, bool>(nameof(IsLast));

    /// <summary>
    /// Defines the <see cref="IsOverflow"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsOverflowProperty =
        AvaloniaProperty.Register<BreadcrumbItem, bool>(nameof(IsOverflow));

    static BreadcrumbItem()
    {
        IsLastProperty.Changed.AddClassHandler<BreadcrumbItem>((x, _) => x.UpdatePseudoClasses());
        IsOverflowProperty.Changed.AddClassHandler<BreadcrumbItem>((x, _) => x.UpdatePseudoClasses());
    }

    /// <summary>
    /// Gets or sets the separator text shown after this item.
    /// </summary>
    public string? SeparatorText
    {
        get => GetValue(SeparatorTextProperty);
        set => SetValue(SeparatorTextProperty, value);
    }

    /// <summary>
    /// Gets or sets the index of this item in the breadcrumb.
    /// </summary>
    public int ItemIndex
    {
        get => GetValue(ItemIndexProperty);
        set => SetValue(ItemIndexProperty, value);
    }

    /// <summary>
    /// Gets or sets whether this is the last breadcrumb item.
    /// </summary>
    public bool IsLast
    {
        get => GetValue(IsLastProperty);
        set => SetValue(IsLastProperty, value);
    }

    /// <summary>
    /// Gets or sets whether this item represents the overflow indicator.
    /// </summary>
    public bool IsOverflow
    {
        get => GetValue(IsOverflowProperty);
        set => SetValue(IsOverflowProperty, value);
    }

    /// <summary>
    /// Gets or sets the parent breadcrumb control.
    /// </summary>
    internal Breadcrumb? BreadcrumbParent { get; set; }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        UpdatePseudoClasses();
    }

    protected override void OnPointerReleased(Avalonia.Input.PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        if (!IsOverflow && BreadcrumbParent != null)
        {
            BreadcrumbParent.RaiseItemClick(DataContext, ItemIndex);
        }
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":last", IsLast);
        PseudoClasses.Set(":overflow", IsOverflow);
        PseudoClasses.Set(":clickable", !IsOverflow);
    }
}
