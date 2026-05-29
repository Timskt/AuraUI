using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace AuraUI.Controls.Selection;

/// <summary>
/// Represents a node in a TreeSelect tree.
/// </summary>
public class TreeSelectNode : AvaloniaObject
{
    /// <summary>
    /// Defines the <see cref="Value"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> ValueProperty =
        AvaloniaProperty.Register<TreeSelectNode, string?>(nameof(Value));

    /// <summary>
    /// Defines the <see cref="Title"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<TreeSelectNode, string?>(nameof(Title));

    /// <summary>
    /// Defines the <see cref="IsDisabled"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsDisabledProperty =
        AvaloniaProperty.Register<TreeSelectNode, bool>(nameof(IsDisabled));

    /// <summary>
    /// Defines the <see cref="IsExpanded"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsExpandedProperty =
        AvaloniaProperty.Register<TreeSelectNode, bool>(nameof(IsExpanded));

    /// <summary>
    /// Defines the <see cref="IsChecked"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsCheckedProperty =
        AvaloniaProperty.Register<TreeSelectNode, bool>(nameof(IsChecked));

    /// <summary>
    /// Defines the <see cref="Icon"/> styled property.
    /// </summary>
    public static readonly StyledProperty<object?> IconProperty =
        AvaloniaProperty.Register<TreeSelectNode, object?>(nameof(Icon));

    /// <summary>
    /// Defines the <see cref="Children"/> styled property.
    /// </summary>
    public static readonly StyledProperty<ObservableCollection<TreeSelectNode>?> ChildrenProperty =
        AvaloniaProperty.Register<TreeSelectNode, ObservableCollection<TreeSelectNode>?>(nameof(Children));

    /// <summary>
    /// Gets or sets the node value.
    /// </summary>
    public string? Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    /// <summary>
    /// Gets or sets the display title.
    /// </summary>
    public string? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <summary>
    /// Gets or sets whether this node is disabled.
    /// </summary>
    public bool IsDisabled
    {
        get => GetValue(IsDisabledProperty);
        set => SetValue(IsDisabledProperty, value);
    }

    /// <summary>
    /// Gets or sets whether this node is expanded.
    /// </summary>
    public bool IsExpanded
    {
        get => GetValue(IsExpandedProperty);
        set => SetValue(IsExpandedProperty, value);
    }

    /// <summary>
    /// Gets or sets whether this node is checked (in checkable mode).
    /// </summary>
    public bool IsChecked
    {
        get => GetValue(IsCheckedProperty);
        set => SetValue(IsCheckedProperty, value);
    }

    /// <summary>
    /// Gets or sets the node icon.
    /// </summary>
    public object? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    /// <summary>
    /// Gets or sets the child nodes.
    /// </summary>
    public ObservableCollection<TreeSelectNode>? Children
    {
        get => GetValue(ChildrenProperty);
        set => SetValue(ChildrenProperty, value);
    }
}

/// <summary>
/// A tree-based dropdown selector allowing selection from hierarchical data.
/// Supports search filtering, checkable nodes, and tag display.
/// Inspired by Ant Design's TreeSelect component.
/// </summary>
[TemplatePart("PART_Dropdown", typeof(Popup))]
[TemplatePart("PART_SearchBox", typeof(TextBox))]
[TemplatePart("PART_TreeView", typeof(TreeView))]
[PseudoClasses(":open", ":closed", ":searching", ":has-value", ":multiple")]
public class TreeSelect : TemplatedControl
{
    private Popup? _dropdown;
    private TextBox? _searchBox;
    private TreeView? _treeView;

    /// <summary>
    /// Defines the <see cref="TreeData"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IList<TreeSelectNode>?> TreeDataProperty =
        AvaloniaProperty.Register<TreeSelect, IList<TreeSelectNode>?>(nameof(TreeData));

    /// <summary>
    /// Defines the <see cref="SelectedValue"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> SelectedValueProperty =
        AvaloniaProperty.Register<TreeSelect, string?>(nameof(SelectedValue));

    /// <summary>
    /// Defines the <see cref="SelectedValues"/> styled property.
    /// For multiple selection mode.
    /// </summary>
    public static readonly StyledProperty<IList<string>?> SelectedValuesProperty =
        AvaloniaProperty.Register<TreeSelect, IList<string>?>(nameof(SelectedValues));

    /// <summary>
    /// Defines the <see cref="Placeholder"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> PlaceholderProperty =
        AvaloniaProperty.Register<TreeSelect, string?>(nameof(Placeholder), "Please select");

    /// <summary>
    /// Defines the <see cref="IsSearchable"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsSearchableProperty =
        AvaloniaProperty.Register<TreeSelect, bool>(nameof(IsSearchable));

    /// <summary>
    /// Defines the <see cref="ShowLine"/> styled property.
    /// Whether to show tree connector lines.
    /// </summary>
    public static readonly StyledProperty<bool> ShowLineProperty =
        AvaloniaProperty.Register<TreeSelect, bool>(nameof(ShowLine));

    /// <summary>
    /// Defines the <see cref="Multiple"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> MultipleProperty =
        AvaloniaProperty.Register<TreeSelect, bool>(nameof(Multiple));

    /// <summary>
    /// Defines the <see cref="TreeCheckable"/> styled property.
    /// Shows checkboxes on tree nodes.
    /// </summary>
    public static readonly StyledProperty<bool> TreeCheckableProperty =
        AvaloniaProperty.Register<TreeSelect, bool>(nameof(TreeCheckable));

    /// <summary>
    /// Defines the <see cref="MaxTagCount"/> styled property.
    /// Maximum number of tags to display before showing "+N".
    /// </summary>
    public static readonly StyledProperty<int> MaxTagCountProperty =
        AvaloniaProperty.Register<TreeSelect, int>(nameof(MaxTagCount), 3);

    /// <summary>
    /// Defines the <see cref="IsDropDownOpen"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsDropDownOpenProperty =
        AvaloniaProperty.Register<TreeSelect, bool>(nameof(IsDropDownOpen));

    /// <summary>
    /// Defines the <see cref="SearchText"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> SearchTextProperty =
        AvaloniaProperty.Register<TreeSelect, string?>(nameof(SearchText));

    static TreeSelect()
    {
        IsDropDownOpenProperty.Changed.AddClassHandler<TreeSelect>((x, _) => x.UpdatePseudoClasses());
        MultipleProperty.Changed.AddClassHandler<TreeSelect>((x, _) => x.UpdatePseudoClasses());
        SelectedValueProperty.Changed.AddClassHandler<TreeSelect>((x, _) => x.UpdatePseudoClasses());
    }

    /// <summary>
    /// Gets or sets the tree data.
    /// </summary>
    public IList<TreeSelectNode>? TreeData
    {
        get => GetValue(TreeDataProperty);
        set => SetValue(TreeDataProperty, value);
    }

    /// <summary>
    /// Gets or sets the selected value (single selection).
    /// </summary>
    public string? SelectedValue
    {
        get => GetValue(SelectedValueProperty);
        set => SetValue(SelectedValueProperty, value);
    }

    /// <summary>
    /// Gets or sets the selected values (multiple selection).
    /// </summary>
    public IList<string>? SelectedValues
    {
        get => GetValue(SelectedValuesProperty);
        set => SetValue(SelectedValuesProperty, value);
    }

    /// <summary>
    /// Gets or sets the placeholder text.
    /// </summary>
    public string? Placeholder
    {
        get => GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
    }

    /// <summary>
    /// Gets or sets whether search filtering is enabled.
    /// </summary>
    public bool IsSearchable
    {
        get => GetValue(IsSearchableProperty);
        set => SetValue(IsSearchableProperty, value);
    }

    /// <summary>
    /// Gets or sets whether to show tree connector lines.
    /// </summary>
    public bool ShowLine
    {
        get => GetValue(ShowLineProperty);
        set => SetValue(ShowLineProperty, value);
    }

    /// <summary>
    /// Gets or sets whether multiple selection is enabled.
    /// </summary>
    public bool Multiple
    {
        get => GetValue(MultipleProperty);
        set => SetValue(MultipleProperty, value);
    }

    /// <summary>
    /// Gets or sets whether tree nodes show checkboxes.
    /// </summary>
    public bool TreeCheckable
    {
        get => GetValue(TreeCheckableProperty);
        set => SetValue(TreeCheckableProperty, value);
    }

    /// <summary>
    /// Gets or sets the maximum tag count before showing "+N more".
    /// </summary>
    public int MaxTagCount
    {
        get => GetValue(MaxTagCountProperty);
        set => SetValue(MaxTagCountProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the dropdown is open.
    /// </summary>
    public bool IsDropDownOpen
    {
        get => GetValue(IsDropDownOpenProperty);
        set => SetValue(IsDropDownOpenProperty, value);
    }

    /// <summary>
    /// Gets or sets the search text.
    /// </summary>
    public string? SearchText
    {
        get => GetValue(SearchTextProperty);
        set => SetValue(SearchTextProperty, value);
    }

    /// <summary>
    /// Occurs when the selection changes.
    /// </summary>
    public event EventHandler<RoutedEventArgs>? Change;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _dropdown = e.NameScope.Find<Popup>("PART_Dropdown");
        _searchBox = e.NameScope.Find<TextBox>("PART_SearchBox");
        _treeView = e.NameScope.Find<TreeView>("PART_TreeView");

        if (_searchBox != null)
        {
            _searchBox.TextChanged += (_, _) =>
            {
                SearchText = _searchBox.Text;
            };
        }

        UpdatePseudoClasses();
    }

    /// <summary>
    /// Gets the display text for the selected value(s).
    /// </summary>
    public string? GetDisplayText()
    {
        if (Multiple)
        {
            var values = SelectedValues;
            if (values == null || values.Count == 0) return null;
            var titles = values.Select(v => FindNodeTitle(TreeData, v)).Where(t => t != null);
            return string.Join(", ", titles);
        }

        var selected = SelectedValue;
        if (selected == null) return null;
        return FindNodeTitle(TreeData, selected);
    }

    /// <summary>
    /// Clears the selection.
    /// </summary>
    public void ClearSelection()
    {
        SelectedValue = null;
        SelectedValues = null;
        Change?.Invoke(this, new RoutedEventArgs());
    }

    /// <summary>
    /// Gets the number of selected items beyond MaxTagCount.
    /// </summary>
    public int OverflowCount
    {
        get
        {
            if (!Multiple) return 0;
            var count = SelectedValues?.Count ?? 0;
            return Math.Max(0, count - MaxTagCount);
        }
    }

    private static string? FindNodeTitle(IList<TreeSelectNode>? nodes, string? value)
    {
        if (nodes == null || value == null) return null;

        foreach (var node in nodes)
        {
            if (node.Value == value) return node.Title;
            var found = FindNodeTitle(node.Children, value);
            if (found != null) return found;
        }

        return null;
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":open", IsDropDownOpen);
        PseudoClasses.Set(":closed", !IsDropDownOpen);
        PseudoClasses.Set(":searching", !string.IsNullOrEmpty(SearchText));
        PseudoClasses.Set(":multiple", Multiple);

        var hasValue = Multiple
            ? SelectedValues?.Count > 0
            : SelectedValue != null;
        PseudoClasses.Set(":has-value", hasValue);
    }
}
