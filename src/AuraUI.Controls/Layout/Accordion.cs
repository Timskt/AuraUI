using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;

namespace AuraUI.Controls.Layout;

/// <summary>
/// Represents a single collapsible panel within an <see cref="Accordion"/>.
/// Unlike <see cref="Expander"/>, multiple <see cref="AccordionItem"/> instances
/// can be expanded simultaneously within their parent <see cref="Accordion"/>.
/// </summary>
[PseudoClasses(":expanded", ":collapsed")]
public class AccordionItem : HeaderedContentControl
{
    /// <summary>
    /// Defines the <see cref="IsExpanded"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsExpandedProperty =
        AvaloniaProperty.Register<AccordionItem, bool>(nameof(IsExpanded));

    /// <summary>
    /// Defines the <see cref="ExpandAnimationDuration"/> styled property.
    /// </summary>
    public static readonly StyledProperty<TimeSpan> ExpandAnimationDurationProperty =
        AvaloniaProperty.Register<AccordionItem, TimeSpan>(
            nameof(ExpandAnimationDuration),
            TimeSpan.FromMilliseconds(200));

    /// <summary>
    /// Defines the <see cref="Icon"/> styled property.
    /// </summary>
    public static readonly StyledProperty<object?> IconProperty =
        AvaloniaProperty.Register<AccordionItem, object?>(nameof(Icon));

    static AccordionItem()
    {
        IsExpandedProperty.Changed.AddClassHandler<AccordionItem>((x, _) => x.OnIsExpandedChanged());
    }

    /// <summary>
    /// Gets or sets whether this accordion item is currently expanded.
    /// </summary>
    public bool IsExpanded
    {
        get => GetValue(IsExpandedProperty);
        set => SetValue(IsExpandedProperty, value);
    }

    /// <summary>
    /// Gets or sets the duration of the expand/collapse animation.
    /// </summary>
    public TimeSpan ExpandAnimationDuration
    {
        get => GetValue(ExpandAnimationDurationProperty);
        set => SetValue(ExpandAnimationDurationProperty, value);
    }

    /// <summary>
    /// Gets or sets an optional icon displayed alongside the header.
    /// </summary>
    public object? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    /// <summary>
    /// Occurs when the item is expanded.
    /// </summary>
    public event EventHandler? Expanded;

    /// <summary>
    /// Occurs when the item is collapsed.
    /// </summary>
    public event EventHandler? Collapsed;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        UpdatePseudoClasses();
    }

    private void OnIsExpandedChanged()
    {
        UpdatePseudoClasses();

        if (IsExpanded)
            Expanded?.Invoke(this, EventArgs.Empty);
        else
            Collapsed?.Invoke(this, EventArgs.Empty);
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":expanded", IsExpanded);
        PseudoClasses.Set(":collapsed", !IsExpanded);
    }
}

/// <summary>
/// A multi-expand accordion container that hosts multiple <see cref="AccordionItem"/>
/// children. All items can be expanded simultaneously (unlike <see cref="Expander"/>
/// which is designed for single-panel collapse/expand).
/// </summary>
[PseudoClasses(":vertical", ":horizontal")]
public class Accordion : ItemsControl
{
    /// <summary>
    /// Defines the <see cref="Orientation"/> styled property.
    /// </summary>
    public static readonly StyledProperty<Orientation> OrientationProperty =
        AvaloniaProperty.Register<Accordion, Orientation>(
            nameof(Orientation),
            Orientation.Vertical);

    /// <summary>
    /// Defines the <see cref="ExpandMultiple"/> styled property.
    /// When false, expanding one item collapses all others (accordion behavior).
    /// When true, multiple items can be expanded simultaneously.
    /// </summary>
    public static readonly StyledProperty<bool> ExpandMultipleProperty =
        AvaloniaProperty.Register<Accordion, bool>(nameof(ExpandMultiple), true);

    static Accordion()
    {
        OrientationProperty.Changed.AddClassHandler<Accordion>((x, _) => x.UpdatePseudoClasses());
    }

    /// <summary>
    /// Gets or sets the layout orientation.
    /// </summary>
    public Orientation Orientation
    {
        get => GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    /// <summary>
    /// Gets or sets whether multiple items can be expanded at once.
    /// When false, expanding one item collapses all others.
    /// </summary>
    public bool ExpandMultiple
    {
        get => GetValue(ExpandMultipleProperty);
        set => SetValue(ExpandMultipleProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        UpdatePseudoClasses();
    }

    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        return new AccordionItem();
    }

    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        return NeedsContainer<AccordionItem>(item, out recycleKey);
    }

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);

        if (container is AccordionItem accordionItem)
        {
            accordionItem.Expanded += OnItemExpanded;
        }
    }

    private void OnItemExpanded(object? sender, EventArgs e)
    {
        if (ExpandMultiple || sender is not AccordionItem expandedItem)
            return;

        // Collapse all other items
        var items = Items;
        if (items is not null)
        {
            foreach (var itemObj in items)
            {
                if (itemObj is null) continue;
                var container = ContainerFromItem(itemObj);
                if (container is AccordionItem item && item != expandedItem)
                {
                    item.IsExpanded = false;
                }
            }
        }
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":vertical", Orientation == Orientation.Vertical);
        PseudoClasses.Set(":horizontal", Orientation == Orientation.Horizontal);
    }
}
