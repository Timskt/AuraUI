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
/// Represents an option in a Cascader, which can have children for hierarchical navigation.
/// </summary>
public class CascaderOption : AvaloniaObject
{
    /// <summary>
    /// Defines the <see cref="Value"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> ValueProperty =
        AvaloniaProperty.Register<CascaderOption, string?>(nameof(Value));

    /// <summary>
    /// Defines the <see cref="Label"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> LabelProperty =
        AvaloniaProperty.Register<CascaderOption, string?>(nameof(Label));

    /// <summary>
    /// Defines the <see cref="IsDisabled"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsDisabledProperty =
        AvaloniaProperty.Register<CascaderOption, bool>(nameof(IsDisabled));

    /// <summary>
    /// Defines the <see cref="IsLeaf"/> styled property.
    /// Whether this option has no children.
    /// </summary>
    public static readonly StyledProperty<bool> IsLeafProperty =
        AvaloniaProperty.Register<CascaderOption, bool>(nameof(IsLeaf));

    /// <summary>
    /// Defines the <see cref="Children"/> styled property.
    /// </summary>
    public static readonly StyledProperty<ObservableCollection<CascaderOption>?> ChildrenProperty =
        AvaloniaProperty.Register<CascaderOption, ObservableCollection<CascaderOption>?>(nameof(Children));

    /// <summary>
    /// Gets or sets the option value.
    /// </summary>
    public string? Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    /// <summary>
    /// Gets or sets the display label.
    /// </summary>
    public string? Label
    {
        get => GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    /// <summary>
    /// Gets or sets whether this option is disabled.
    /// </summary>
    public bool IsDisabled
    {
        get => GetValue(IsDisabledProperty);
        set => SetValue(IsDisabledProperty, value);
    }

    /// <summary>
    /// Gets or sets whether this is a leaf node (no children).
    /// </summary>
    public bool IsLeaf
    {
        get => GetValue(IsLeafProperty);
        set => SetValue(IsLeafProperty, value);
    }

    /// <summary>
    /// Gets or sets the child options.
    /// </summary>
    public ObservableCollection<CascaderOption>? Children
    {
        get => GetValue(ChildrenProperty);
        set => SetValue(ChildrenProperty, value);
    }
}

/// <summary>
/// Specifies what triggers the expansion of cascader submenus.
/// </summary>
public enum CascaderExpandTrigger
{
    /// <summary>Expand on click.</summary>
    Click,
    /// <summary>Expand on hover.</summary>
    Hover
}

/// <summary>
/// A cascading dropdown selector for hierarchical data. Each selection reveals
/// the next level of options in a cascading panel layout.
/// Inspired by Ant Design's Cascader component.
/// </summary>
[TemplatePart("PART_Dropdown", typeof(Popup))]
[TemplatePart("PART_Input", typeof(TextBox))]
[PseudoClasses(":open", ":closed", ":searching", ":has-value")]
public class Cascader : TemplatedControl
{
    private Popup? _dropdown;
    private TextBox? _input;

    /// <summary>
    /// Defines the <see cref="Options"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IList<CascaderOption>?> OptionsProperty =
        AvaloniaProperty.Register<Cascader, IList<CascaderOption>?>(nameof(Options));

    /// <summary>
    /// Defines the <see cref="Value"/> styled property.
    /// The selected value path (array of values from root to leaf).
    /// </summary>
    public static readonly StyledProperty<IList<string>?> ValueProperty =
        AvaloniaProperty.Register<Cascader, IList<string>?>(nameof(Value));

    /// <summary>
    /// Defines the <see cref="Placeholder"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> PlaceholderProperty =
        AvaloniaProperty.Register<Cascader, string?>(nameof(Placeholder), "Please select");

    /// <summary>
    /// Defines the <see cref="IsSearchable"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsSearchableProperty =
        AvaloniaProperty.Register<Cascader, bool>(nameof(IsSearchable));

    /// <summary>
    /// Defines the <see cref="ShowAllLevels"/> styled property.
    /// Whether to show the full path or just the selected label.
    /// </summary>
    public static readonly StyledProperty<bool> ShowAllLevelsProperty =
        AvaloniaProperty.Register<Cascader, bool>(nameof(ShowAllLevels), true);

    /// <summary>
    /// Defines the <see cref="Separator"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string> SeparatorProperty =
        AvaloniaProperty.Register<Cascader, string>(nameof(Separator), " / ");

    /// <summary>
    /// Defines the <see cref="ExpandTrigger"/> styled property.
    /// </summary>
    public static readonly StyledProperty<CascaderExpandTrigger> ExpandTriggerProperty =
        AvaloniaProperty.Register<Cascader, CascaderExpandTrigger>(
            nameof(ExpandTrigger), CascaderExpandTrigger.Click);

    /// <summary>
    /// Defines the <see cref="MaxLevel"/> styled property.
    /// Maximum depth of the cascade. 0 = unlimited.
    /// </summary>
    public static readonly StyledProperty<int> MaxLevelProperty =
        AvaloniaProperty.Register<Cascader, int>(nameof(MaxLevel));

    /// <summary>
    /// Defines the <see cref="ChangeOnSelect"/> styled property.
    /// Whether selecting a parent node triggers a change event (without requiring a leaf).
    /// </summary>
    public static readonly StyledProperty<bool> ChangeOnSelectProperty =
        AvaloniaProperty.Register<Cascader, bool>(nameof(ChangeOnSelect));

    /// <summary>
    /// Defines the <see cref="IsDropDownOpen"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsDropDownOpenProperty =
        AvaloniaProperty.Register<Cascader, bool>(nameof(IsDropDownOpen));

    /// <summary>
    /// Defines the <see cref="SearchText"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> SearchTextProperty =
        AvaloniaProperty.Register<Cascader, string?>(nameof(SearchText));

    static Cascader()
    {
        IsDropDownOpenProperty.Changed.AddClassHandler<Cascader>((x, _) => x.UpdatePseudoClasses());
        ValueProperty.Changed.AddClassHandler<Cascader>((x, _) => x.UpdateDisplayText());
    }

    /// <summary>
    /// Gets or sets the root-level options.
    /// </summary>
    public IList<CascaderOption>? Options
    {
        get => GetValue(OptionsProperty);
        set => SetValue(OptionsProperty, value);
    }

    /// <summary>
    /// Gets or sets the selected value path.
    /// </summary>
    public IList<string>? Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
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
    /// Gets or sets whether to show the full path in the display.
    /// </summary>
    public bool ShowAllLevels
    {
        get => GetValue(ShowAllLevelsProperty);
        set => SetValue(ShowAllLevelsProperty, value);
    }

    /// <summary>
    /// Gets or sets the path separator.
    /// </summary>
    public string Separator
    {
        get => GetValue(SeparatorProperty);
        set => SetValue(SeparatorProperty, value);
    }

    /// <summary>
    /// Gets or sets what triggers submenu expansion.
    /// </summary>
    public CascaderExpandTrigger ExpandTrigger
    {
        get => GetValue(ExpandTriggerProperty);
        set => SetValue(ExpandTriggerProperty, value);
    }

    /// <summary>
    /// Gets or sets the maximum cascade depth.
    /// </summary>
    public int MaxLevel
    {
        get => GetValue(MaxLevelProperty);
        set => SetValue(MaxLevelProperty, value);
    }

    /// <summary>
    /// Gets or sets whether selecting a parent triggers a change.
    /// </summary>
    public bool ChangeOnSelect
    {
        get => GetValue(ChangeOnSelectProperty);
        set => SetValue(ChangeOnSelectProperty, value);
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
    /// Gets the display text for the current selection.
    /// </summary>
    public string? DisplayText { get; private set; }

    /// <summary>
    /// Occurs when the selection changes.
    /// </summary>
    public event EventHandler<RoutedEventArgs>? Change;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _dropdown = e.NameScope.Find<Popup>("PART_Dropdown");
        _input = e.NameScope.Find<TextBox>("PART_Input");
        UpdatePseudoClasses();
    }

    /// <summary>
    /// Selects an option at the specified level.
    /// </summary>
    public void SelectOption(CascaderOption option, int level)
    {
        if (option.IsDisabled) return;

        var currentValue = Value?.ToList() ?? new List<string>();

        // Trim to current level and add new value
        while (currentValue.Count > level)
        {
            currentValue.RemoveAt(currentValue.Count - 1);
        }

        if (option.Value != null)
        {
            currentValue.Add(option.Value);
        }

        Value = currentValue;

        // If leaf or ChangeOnSelect, trigger change
        if (option.IsLeaf || ChangeOnSelect)
        {
            Change?.Invoke(this, new RoutedEventArgs());
            if (option.IsLeaf || option.Children == null || option.Children.Count == 0)
            {
                IsDropDownOpen = false;
            }
        }
    }

    /// <summary>
    /// Clears the selection.
    /// </summary>
    public void ClearSelection()
    {
        Value = null;
        DisplayText = null;
        Change?.Invoke(this, new RoutedEventArgs());
    }

    /// <summary>
    /// Gets the label path for a given value path.
    /// </summary>
    public IList<string> GetLabelPath(IList<string> valuePath)
    {
        var labels = new List<string>();
        if (valuePath == null || Options == null) return labels;

        var currentOptions = Options;
        foreach (var value in valuePath)
        {
            var option = currentOptions?.FirstOrDefault(o => o.Value == value);
            if (option == null) break;
            labels.Add(option.Label ?? option.Value ?? "");
            currentOptions = option.Children;
        }

        return labels;
    }

    private void UpdateDisplayText()
    {
        var value = Value;
        if (value == null || value.Count == 0)
        {
            DisplayText = null;
            return;
        }

        if (ShowAllLevels)
        {
            var labels = GetLabelPath(value);
            DisplayText = string.Join(Separator, labels);
        }
        else
        {
            var labels = GetLabelPath(value);
            DisplayText = labels.LastOrDefault();
        }
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":open", IsDropDownOpen);
        PseudoClasses.Set(":closed", !IsDropDownOpen);
        PseudoClasses.Set(":searching", !string.IsNullOrEmpty(SearchText));
        PseudoClasses.Set(":has-value", Value?.Count > 0);
    }
}
