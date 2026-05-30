using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace AuraUI.Controls.Display;

/// <summary>
/// A tree view item with a three-state checkbox for hierarchical selection.
/// Extends <see cref="AuraTreeViewItem"/> with checkbox support, enabling
/// parent-child checkbox propagation (check parent -> check all children,
/// uncheck any child -> parent becomes indeterminate).
/// </summary>
[PseudoClasses(":checked", ":unchecked", ":indeterminate")]
public class CheckedTreeViewItem : AuraTreeViewItem
{
    /// <summary>
    /// Defines the <see cref="IsChecked"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool?> IsCheckedProperty =
        AvaloniaProperty.Register<CheckedTreeViewItem, bool?>(nameof(IsChecked), false);

    /// <summary>
    /// Defines the <see cref="IsThreeState"/> styled property.
    /// When true, the checkbox cycles through Checked, Indeterminate, and Unchecked.
    /// </summary>
    public static readonly StyledProperty<bool> IsThreeStateProperty =
        AvaloniaProperty.Register<CheckedTreeViewItem, bool>(nameof(IsThreeState), true);

    /// <summary>
    /// Defines the <see cref="CheckBoxMargin"/> styled property.
    /// </summary>
    public static readonly StyledProperty<Thickness> CheckBoxMarginProperty =
        AvaloniaProperty.Register<CheckedTreeViewItem, Thickness>(
            nameof(CheckBoxMargin),
            new Thickness(0, 0, 8, 0));

    static CheckedTreeViewItem()
    {
        IsCheckedProperty.Changed.AddClassHandler<CheckedTreeViewItem>((x, _) => x.OnIsCheckedChanged());
    }

    /// <summary>
    /// Occurs when the checked state changes.
    /// </summary>
    public event EventHandler<CheckedChangedEventArgs>? CheckedChanged;

    /// <summary>
    /// Gets or sets the checked state. Null represents the indeterminate state.
    /// </summary>
    public bool? IsChecked
    {
        get => GetValue(IsCheckedProperty);
        set => SetValue(IsCheckedProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the checkbox supports three states.
    /// </summary>
    public bool IsThreeState
    {
        get => GetValue(IsThreeStateProperty);
        set => SetValue(IsThreeStateProperty, value);
    }

    /// <summary>
    /// Gets or sets the margin around the checkbox.
    /// </summary>
    public Thickness CheckBoxMargin
    {
        get => GetValue(CheckBoxMarginProperty);
        set => SetValue(CheckBoxMarginProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        UpdatePseudoClasses();
    }

    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        return new CheckedTreeViewItem();
    }

    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        return NeedsContainer<CheckedTreeViewItem>(item, out recycleKey);
    }

    private void OnIsCheckedChanged()
    {
        UpdatePseudoClasses();
        CheckedChanged?.Invoke(this, new CheckedChangedEventArgs(IsChecked));

        // Propagate to children
        if (IsChecked.HasValue)
        {
            PropagateToChildren(IsChecked.Value);
        }
    }

    /// <summary>
    /// Propagates the checked state to all child items.
    /// </summary>
    private void PropagateToChildren(bool isChecked)
    {
        var items = Items;
        if (items is not null)
        {
            foreach (var itemObj in items)
            {
                if (itemObj is null) continue;
                var container = ContainerFromItem(itemObj);
                if (container is CheckedTreeViewItem childItem)
                {
                    childItem.IsChecked = isChecked;
                }
            }
        }
    }

    /// <summary>
    /// Recalculates the parent's checked state based on children.
    /// Should be called by the parent <see cref="CheckedTreeView"/>.
    /// </summary>
    internal void RecalculateFromChildren()
    {
        bool? anyChecked = null;
        bool allChecked = true;
        bool anyChildChecked = false;

        var items = Items;
        if (items is not null)
        {
            foreach (var itemObj in items)
            {
                if (itemObj is null) continue;
                var container = ContainerFromItem(itemObj);
                if (container is CheckedTreeViewItem childItem)
                {
                    if (childItem.IsChecked == true)
                    {
                        anyChildChecked = true;
                    }
                    else
                    {
                        allChecked = false;
                    }

                    if (childItem.IsChecked == null)
                    {
                        anyChecked = null;
                        allChecked = false;
                    }
                }
            }
        }

        if (allChecked && anyChildChecked)
            IsChecked = true;
        else if (anyChildChecked || anyChecked == null)
            IsChecked = null; // Indeterminate
        else
            IsChecked = false;
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":checked", IsChecked == true);
        PseudoClasses.Set(":unchecked", IsChecked == false);
        PseudoClasses.Set(":indeterminate", IsChecked == null);
    }
}

/// <summary>
/// A tree view with checkboxes on each node for hierarchical selection.
/// Extends <see cref="AuraTreeView"/> with automatic parent-child checkbox
/// propagation and three-state checkbox support.
/// </summary>
public class CheckedTreeView : AuraTreeView
{
    /// <summary>
    /// Defines the <see cref="ShowCheckBoxes"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowCheckBoxesProperty =
        AvaloniaProperty.Register<CheckedTreeView, bool>(nameof(ShowCheckBoxes), true);

    /// <summary>
    /// Defines the <see cref="CheckBoxBrush"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> CheckBoxBrushProperty =
        AvaloniaProperty.Register<CheckedTreeView, IBrush?>(nameof(CheckBoxBrush));

    static CheckedTreeView()
    {
        ShowCheckBoxesProperty.Changed.AddClassHandler<CheckedTreeView>((x, _) => x.InvalidateMeasure());
    }

    /// <summary>
    /// Occurs when the checked items collection changes.
    /// </summary>
#pragma warning disable CS0067 // Event is never invoked -- public API for consumers
    public event EventHandler? CheckedItemsChanged;
#pragma warning restore CS0067

    /// <summary>
    /// Gets or sets whether checkboxes are visible.
    /// </summary>
    public bool ShowCheckBoxes
    {
        get => GetValue(ShowCheckBoxesProperty);
        set => SetValue(ShowCheckBoxesProperty, value);
    }

    /// <summary>
    /// Gets or sets the brush used for checkbox marks.
    /// </summary>
    public IBrush? CheckBoxBrush
    {
        get => GetValue(CheckBoxBrushProperty);
        set => SetValue(CheckBoxBrushProperty, value);
    }

    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        return new CheckedTreeViewItem();
    }

    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        return NeedsContainer<CheckedTreeViewItem>(item, out recycleKey);
    }

    /// <summary>
    /// Gets all checked items in the tree.
    /// </summary>
    public IEnumerable<CheckedTreeViewItem> GetCheckedItems()
    {
        return GetCheckedItemsRecursive(this);
    }

    private IEnumerable<CheckedTreeViewItem> GetCheckedItemsRecursive(ItemsControl parent)
    {
        var items = parent.Items;
        if (items is null) yield break;
        foreach (var itemObj in items)
        {
            if (itemObj is null) continue;
            var container = parent.ContainerFromItem(itemObj);
            if (container is CheckedTreeViewItem item)
            {
                if (item.IsChecked == true)
                    yield return item;

                foreach (var child in GetCheckedItemsRecursive(item))
                    yield return child;
            }
        }
    }
}

/// <summary>
/// Event arguments for <see cref="CheckedTreeViewItem.CheckedChanged"/>.
/// </summary>
public class CheckedChangedEventArgs : EventArgs
{
    public bool? IsChecked { get; }

    public CheckedChangedEventArgs(bool? isChecked)
    {
        IsChecked = isChecked;
    }
}
