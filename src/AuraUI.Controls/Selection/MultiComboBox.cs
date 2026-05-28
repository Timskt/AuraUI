using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;

using AuraUI.Controls.Layout;

namespace AuraUI.Controls.Selection;

/// <summary>
/// A multi-selection combo box that displays selected items as <see cref="Tag"/> controls
/// and provides checkboxes within each dropdown item.
/// </summary>
/// <remarks>
/// <para>
/// Selected items are displayed as tags above the dropdown. Each dropdown item shows a
/// check box indicating its selection state. Supports Select All / Clear All actions
/// and limits via <see cref="MaxSelections"/>.
/// </para>
/// <para>
/// Template parts:
/// <list type="bullet">
///   <item><c>PART_TagContainer</c>: An <see cref="ItemsControl"/> or panel where tags are rendered.</item>
///   <item><c>PART_SelectAllButton</c>: A button to select all items.</item>
///   <item><c>PART_ClearAllButton</c>: A button to clear all selections.</item>
///   <item><c>PART_DropdownItemsList</c>: The <see cref="ListBox"/> used in the dropdown for items.</item>
/// </list>
/// </para>
/// </remarks>
[TemplatePart("PART_TagContainer", typeof(ItemsControl))]
[TemplatePart("PART_SelectAllButton", typeof(Button))]
[TemplatePart("PART_ClearAllButton", typeof(Button))]
[TemplatePart("PART_DropdownItemsList", typeof(ListBox))]
[PseudoClasses(":has-selection", ":max-reached")]
public class MultiComboBox : AuraComboBox
{
    private ItemsControl? _tagContainer;
    private Button? _selectAllButton;
    private Button? _clearAllButton;
    private ListBox? _dropdownItemsList;

    private readonly AvaloniaList<object> _selectedItems = new();
    private bool _isSyncingSelection;

    /// <summary>
    /// Defines the <see cref="SelectedItems"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IList?> SelectedItemsProperty =
        AvaloniaProperty.Register<MultiComboBox, IList?>(nameof(SelectedItems));

    /// <summary>
    /// Defines the <see cref="MaxSelections"/> styled property.
    /// A value of 0 means no limit.
    /// </summary>
    public static readonly StyledProperty<int> MaxSelectionsProperty =
        AvaloniaProperty.Register<MultiComboBox, int>(nameof(MaxSelections));

    /// <summary>
    /// Defines the <see cref="ShowSelectAll"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowSelectAllProperty =
        AvaloniaProperty.Register<MultiComboBox, bool>(nameof(ShowSelectAll));

    /// <summary>
    /// Defines the <see cref="TagVariant"/> styled property.
    /// </summary>
    public static readonly StyledProperty<Layout.TagVariant> TagVariantProperty =
        AvaloniaProperty.Register<MultiComboBox, Layout.TagVariant>(
            nameof(TagVariant),
            Layout.TagVariant.Default);

    /// <summary>
    /// Defines the <see cref="SelectionDisplayLimit"/> styled property.
    /// Controls the maximum number of tags shown before collapsing to a count.
    /// A value of 0 means show all.
    /// </summary>
    public static readonly StyledProperty<int> SelectionDisplayLimitProperty =
        AvaloniaProperty.Register<MultiComboBox, int>(nameof(SelectionDisplayLimit));

    /// <summary>
    /// Defines the <see cref="TagMemberPath"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> TagMemberPathProperty =
        AvaloniaProperty.Register<MultiComboBox, string?>(
            nameof(TagMemberPath));

    static MultiComboBox()
    {
        SelectedItemsProperty.Changed.AddClassHandler<MultiComboBox>((x, e) => x.OnSelectedItemsChanged(e));
        MaxSelectionsProperty.Changed.AddClassHandler<MultiComboBox>((x, _) => x.UpdateSelectionPseudoClasses());
        IsDropDownOpenProperty.Changed.AddClassHandler<MultiComboBox>((x, e) =>
        {
            if (e.NewValue is false)
                x.SyncExternalSelectedItems();
        });
    }

    /// <summary>
    /// Occurs when the selected items collection changes.
    /// </summary>
    public event EventHandler<SelectedItemsChangedEventArgs>? SelectedItemsChanged;

    /// <summary>
    /// Gets or sets the list of currently selected items.
    /// </summary>
    public IList? SelectedItems
    {
        get => GetValue(SelectedItemsProperty);
        set => SetValue(SelectedItemsProperty, value);
    }

    /// <summary>
    /// Gets or sets the maximum number of items that can be selected.
    /// A value of 0 means no limit.
    /// </summary>
    public int MaxSelections
    {
        get => GetValue(MaxSelectionsProperty);
        set => SetValue(MaxSelectionsProperty, value);
    }

    /// <summary>
    /// Gets or sets whether Select All / Clear All buttons are shown.
    /// </summary>
    public bool ShowSelectAll
    {
        get => GetValue(ShowSelectAllProperty);
        set => SetValue(ShowSelectAllProperty, value);
    }

    /// <summary>
    /// Gets or sets the variant used for the selection tags.
    /// </summary>
    public Layout.TagVariant TagVariant
    {
        get => GetValue(TagVariantProperty);
        set => SetValue(TagVariantProperty, value);
    }

    /// <summary>
    /// Gets or sets the maximum number of tags to display before showing a count badge.
    /// A value of 0 means show all tags.
    /// </summary>
    public int SelectionDisplayLimit
    {
        get => GetValue(SelectionDisplayLimitProperty);
        set => SetValue(SelectionDisplayLimitProperty, value);
    }

    /// <summary>
    /// Gets or sets the property path used to obtain the tag display text for an item.
    /// </summary>
    public string? TagMemberPath
    {
        get => GetValue(TagMemberPathProperty);
        set => SetValue(TagMemberPathProperty, value);
    }

    /// <summary>
    /// Gets the internal observable collection of selected items.
    /// </summary>
    public AvaloniaList<object> InternalSelectedItems => _selectedItems;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        DetachTemplateParts();

        _tagContainer = e.NameScope.Find<ItemsControl>("PART_TagContainer");
        _selectAllButton = e.NameScope.Find<Button>("PART_SelectAllButton");
        _clearAllButton = e.NameScope.Find<Button>("PART_ClearAllButton");
        _dropdownItemsList = e.NameScope.Find<ListBox>("PART_DropdownItemsList");

        AttachTemplateParts();
        RefreshTagDisplay();
        UpdateSelectionPseudoClasses();
    }

    /// <summary>
    /// Selects the specified item if it is not already selected and the max selection limit
    /// has not been reached.
    /// </summary>
    /// <param name="item">The item to select.</param>
    /// <returns><c>true</c> if the item was added; <c>false</c> otherwise.</returns>
    public bool SelectItem(object item)
    {
        if (_selectedItems.Contains(item))
            return false;

        var max = MaxSelections;
        if (max > 0 && _selectedItems.Count >= max)
            return false;

        _selectedItems.Add(item);
        OnInternalSelectionChanged();
        return true;
    }

    /// <summary>
    /// Deselects the specified item.
    /// </summary>
    /// <param name="item">The item to deselect.</param>
    /// <returns><c>true</c> if the item was removed; <c>false</c> otherwise.</returns>
    public bool DeselectItem(object item)
    {
        var removed = _selectedItems.Remove(item);
        if (removed)
        {
            OnInternalSelectionChanged();
        }
        return removed;
    }

    /// <summary>
    /// Toggles the selection state of the specified item.
    /// </summary>
    /// <param name="item">The item to toggle.</param>
    public void ToggleItem(object item)
    {
        if (_selectedItems.Contains(item))
        {
            DeselectItem(item);
        }
        else
        {
            SelectItem(item);
        }
    }

    /// <summary>
    /// Selects all items currently in the items source.
    /// </summary>
    public void SelectAll()
    {
        var max = MaxSelections;
        var items = GetItemEnumerable();

        if (items == null)
            return;

        _selectedItems.Clear();

        foreach (var item in items)
        {
            if (max > 0 && _selectedItems.Count >= max)
                break;

            _selectedItems.Add(item);
        }

        OnInternalSelectionChanged();
    }

    /// <summary>
    /// Clears all selections.
    /// </summary>
    public void ClearAll()
    {
        if (_selectedItems.Count == 0)
            return;

        _selectedItems.Clear();
        OnInternalSelectionChanged();
    }

    private void AttachTemplateParts()
    {
        if (_selectAllButton != null)
            _selectAllButton.Click += OnSelectAllClick;

        if (_clearAllButton != null)
            _clearAllButton.Click += OnClearAllClick;

        if (_dropdownItemsList != null)
            _dropdownItemsList.SelectionChanged += OnDropdownSelectionChanged;
    }

    private void DetachTemplateParts()
    {
        if (_selectAllButton != null)
            _selectAllButton.Click -= OnSelectAllClick;

        if (_clearAllButton != null)
            _clearAllButton.Click -= OnClearAllClick;

        if (_dropdownItemsList != null)
            _dropdownItemsList.SelectionChanged -= OnDropdownSelectionChanged;
    }

    private void OnSelectAllClick(object? sender, RoutedEventArgs e)
    {
        SelectAll();
    }

    private void OnClearAllClick(object? sender, RoutedEventArgs e)
    {
        ClearAll();
    }

    private void OnDropdownSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_isSyncingSelection)
            return;

        // When items are selected in the dropdown, toggle them in our multi-select collection.
        foreach (var added in e.AddedItems)
        {
            if (added != null)
                SelectItem(added);
        }

        foreach (var removed in e.RemovedItems)
        {
            if (removed != null)
                DeselectItem(removed);
        }
    }

    private void OnSelectedItemsChanged(AvaloniaPropertyChangedEventArgs e)
    {
        // When the external SelectedItems property changes, sync the internal list.
        if (_isSyncingSelection)
            return;

        var newList = e.NewValue as IList;
        _selectedItems.Clear();

        if (newList != null)
        {
            foreach (var item in newList)
            {
                if (item != null)
                    _selectedItems.Add(item);
            }
        }

        OnInternalSelectionChanged();
    }

    private void OnInternalSelectionChanged()
    {
        RefreshTagDisplay();
        UpdateDropdownCheckStates();
        UpdateSelectionPseudoClasses();
    }

    private void RefreshTagDisplay()
    {
        if (_tagContainer == null)
            return;

        var displayLimit = SelectionDisplayLimit;
        var tagDataList = new List<object>();
        var selected = _selectedItems.ToList();

        if (displayLimit > 0 && selected.Count > displayLimit)
        {
            // Show limited tags plus a "+N more" indicator.
            for (var i = 0; i < displayLimit; i++)
            {
                tagDataList.Add(new TagData
                {
                    Content = GetTagDisplayText(selected[i]),
                    Item = selected[i],
                    IsClosable = true,
                    Variant = TagVariant
                });
            }

            tagDataList.Add(new TagData
                {
                    Content = $"+{selected.Count - displayLimit} more",
                    Item = null,
                    IsClosable = false,
                    Variant = Layout.TagVariant.Outlined
                });
        }
        else
        {
            foreach (var item in selected)
            {
                tagDataList.Add(new TagData
                {
                    Content = GetTagDisplayText(item),
                    Item = item,
                    IsClosable = true,
                    Variant = TagVariant
                });
            }
        }

        _tagContainer.ItemsSource = tagDataList;
    }

    private void UpdateDropdownCheckStates()
    {
        if (_dropdownItemsList == null)
            return;

        _isSyncingSelection = true;
        try
        {
            _dropdownItemsList.SelectedItems.Clear();
            foreach (var item in _selectedItems)
            {
                _dropdownItemsList.SelectedItems.Add(item);
            }
        }
        finally
        {
            _isSyncingSelection = false;
        }
    }

    private void SyncExternalSelectedItems()
    {
        var external = SelectedItems;
        if (external == null)
            return;

        _isSyncingSelection = true;
        try
        {
            external.Clear();
            foreach (var item in _selectedItems)
            {
                external.Add(item);
            }
        }
        finally
        {
            _isSyncingSelection = false;
        }

        SelectedItemsChanged?.Invoke(this, new SelectedItemsChangedEventArgs(
            Array.Empty<object>(),
            _selectedItems.ToList()));
    }

    private string GetTagDisplayText(object item)
    {
        var memberPath = TagMemberPath;
        if (!string.IsNullOrEmpty(memberPath))
        {
            var prop = item.GetType().GetProperty(memberPath);
            if (prop != null)
            {
                return prop.GetValue(item)?.ToString() ?? string.Empty;
            }
        }

        return item.ToString() ?? string.Empty;
    }

    private IEnumerable? GetItemEnumerable()
    {
        return ItemsSource as IEnumerable ?? Items;
    }

    private void UpdateSelectionPseudoClasses()
    {
        PseudoClasses.Set(":has-selection", _selectedItems.Count > 0);

        var max = MaxSelections;
        PseudoClasses.Set(":max-reached", max > 0 && _selectedItems.Count >= max);
    }

    /// <summary>
    /// Represents the data for a tag rendered in the tag container.
    /// </summary>
    public class TagData
    {
        /// <summary>
        /// Gets or sets the display content for the tag.
        /// </summary>
        public string? Content { get; set; }

        /// <summary>
        /// Gets or sets the original item this tag represents.
        /// Null for informational tags (e.g., "+N more").
        /// </summary>
        public object? Item { get; set; }

        /// <summary>
        /// Gets or sets whether the tag has a close button.
        /// </summary>
        public bool IsClosable { get; set; }

        /// <summary>
        /// Gets or sets the visual variant for this tag.
        /// </summary>
        public Layout.TagVariant Variant { get; set; }
    }
}

/// <summary>
/// Event arguments for the <see cref="MultiComboBox.SelectedItemsChanged"/> event.
/// </summary>
public class SelectedItemsChangedEventArgs : EventArgs
{
    /// <summary>
    /// Gets the items that were added to the selection.
    /// </summary>
    public IList AddedItems { get; }

    /// <summary>
    /// Gets the items that were removed from the selection.
    /// </summary>
    public IList RemovedItems { get; }

    public SelectedItemsChangedEventArgs(IList addedItems, IList removedItems)
    {
        AddedItems = addedItems;
        RemovedItems = removedItems;
    }
}
