using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;

namespace AuraUI.Controls.Selection;

/// <summary>
/// An enhanced list box control with configurable item spacing and compact/comfortable density modes.
/// Supports custom scroll bar styling.
/// </summary>
[PseudoClasses(":compact", ":comfortable")]
public class AuraListBox : ListBox
{
    /// <summary>
    /// Defines the <see cref="ItemSpacing"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> ItemSpacingProperty =
        AvaloniaProperty.Register<AuraListBox, double>(nameof(ItemSpacing), 4.0);

    /// <summary>
    /// Defines the <see cref="Density"/> styled property.
    /// </summary>
    public static readonly StyledProperty<ListBoxDensity> DensityProperty =
        AvaloniaProperty.Register<AuraListBox, ListBoxDensity>(
            nameof(Density),
            ListBoxDensity.Comfortable);

    /// <summary>
    /// Defines the <see cref="ScrollBarVisibility"/> styled property for the vertical scroll bar.
    /// </summary>
    public static readonly StyledProperty<ScrollBarVisibility> VerticalScrollBarVisibilityProperty =
        AvaloniaProperty.Register<AuraListBox, ScrollBarVisibility>(
            nameof(VerticalScrollBarVisibility),
            ScrollBarVisibility.Auto);

    /// <summary>
    /// Defines the <see cref="ScrollBarVisibility"/> styled property for the horizontal scroll bar.
    /// </summary>
    public static readonly StyledProperty<ScrollBarVisibility> HorizontalScrollBarVisibilityProperty =
        AvaloniaProperty.Register<AuraListBox, ScrollBarVisibility>(
            nameof(HorizontalScrollBarVisibility),
            ScrollBarVisibility.Disabled);

    /// <summary>
    /// Defines the <see cref="ScrollViewerTheme"/> styled property for custom scroll bar styling.
    /// </summary>
    public static readonly StyledProperty<string?> ScrollViewerThemeProperty =
        AvaloniaProperty.Register<AuraListBox, string?>(nameof(ScrollViewerTheme));

    static AuraListBox()
    {
        DensityProperty.Changed.AddClassHandler<AuraListBox>((x, _) => x.UpdatePseudoClasses());
        SelectionChangedEvent.AddClassHandler<AuraListBox>((x, e) => x.OnSelectionChangedHandler(e));
    }

    /// <summary>
    /// Gets or sets the spacing between list items.
    /// </summary>
    public double ItemSpacing
    {
        get => GetValue(ItemSpacingProperty);
        set => SetValue(ItemSpacingProperty, value);
    }

    /// <summary>
    /// Gets or sets the density mode affecting padding and sizing of items.
    /// </summary>
    public ListBoxDensity Density
    {
        get => GetValue(DensityProperty);
        set => SetValue(DensityProperty, value);
    }

    /// <summary>
    /// Gets or sets the vertical scroll bar visibility.
    /// </summary>
    public ScrollBarVisibility VerticalScrollBarVisibility
    {
        get => GetValue(VerticalScrollBarVisibilityProperty);
        set => SetValue(VerticalScrollBarVisibilityProperty, value);
    }

    /// <summary>
    /// Gets or sets the horizontal scroll bar visibility.
    /// </summary>
    public ScrollBarVisibility HorizontalScrollBarVisibility
    {
        get => GetValue(HorizontalScrollBarVisibilityProperty);
        set => SetValue(HorizontalScrollBarVisibilityProperty, value);
    }

    /// <summary>
    /// Gets or sets a resource key used to style the inner ScrollViewer for custom scroll bars.
    /// </summary>
    public string? ScrollViewerTheme
    {
        get => GetValue(ScrollViewerThemeProperty);
        set => SetValue(ScrollViewerThemeProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        UpdatePseudoClasses();
    }

    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        return new AuraListBoxItem();
    }

    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        return NeedsContainer<AuraListBoxItem>(item, out recycleKey);
    }

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);

        if (container is AuraListBoxItem listBoxItem)
        {
            listBoxItem.UpdateSelectedPseudoClass(SelectedItems.Contains(item));
        }
    }

    private void OnSelectionChangedHandler(SelectionChangedEventArgs e)
    {

        // Remove :selected from deselected items.
        foreach (var removed in e.RemovedItems)
        {
            var index = IndexOf(removed);
            if (index >= 0 && ContainerFromIndex(index) is AuraListBoxItem deselected)
            {
                deselected.UpdateSelectedPseudoClass(false);
            }
        }

        // Add :selected to newly selected items.
        foreach (var added in e.AddedItems)
        {
            var index = IndexOf(added);
            if (index >= 0 && ContainerFromIndex(index) is AuraListBoxItem selected)
            {
                selected.UpdateSelectedPseudoClass(true);
            }
        }
    }

    private int IndexOf(object? item)
    {
        if (item == null || ItemsSource is not System.Collections.IList items)
            return -1;

        return items.IndexOf(item);
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":compact", Density == ListBoxDensity.Compact);
        PseudoClasses.Set(":comfortable", Density == ListBoxDensity.Comfortable);
    }
}

/// <summary>
/// Density modes for <see cref="AuraListBox"/>.
/// </summary>
public enum ListBoxDensity
{
    /// <summary>
    /// Compact density with reduced padding.
    /// </summary>
    Compact,

    /// <summary>
    /// Comfortable density with standard padding.
    /// </summary>
    Comfortable
}

/// <summary>
/// A list box item container for <see cref="AuraListBox"/> that supports the :selected pseudo-class.
/// </summary>
[PseudoClasses(":selected", ":pointerover")]
public class AuraListBoxItem : ListBoxItem
{
    internal void UpdateSelectedPseudoClass(bool isSelected)
    {
        PseudoClasses.Set(":selected", isSelected);
    }

    protected override void OnPointerEntered(Avalonia.Input.PointerEventArgs e)
    {
        base.OnPointerEntered(e);
        PseudoClasses.Set(":pointerover", true);
    }

    protected override void OnPointerExited(Avalonia.Input.PointerEventArgs e)
    {
        base.OnPointerExited(e);
        PseudoClasses.Set(":pointerover", false);
    }
}
