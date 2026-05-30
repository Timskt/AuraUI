using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Automation;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;

namespace AuraUI.Controls.Selection;

/// <summary>
/// An enhanced combo box supporting search filtering, placeholder text, visual variants,
/// and a configurable maximum dropdown height.
/// </summary>
/// <remarks>
/// <para>
/// When <see cref="IsSearchable"/> is <c>true</c>, a search <see cref="TextBox"/> appears
/// at the top of the dropdown popup. As the user types, items are filtered in real-time
/// by matching against their string representation.
/// </para>
/// <para>
/// Template parts:
/// <list type="bullet">
///   <item><c>PART_SearchBox</c>: The search TextBox shown in the dropdown.</item>
///   <item><c>PART_DropdownBorder</c>: The dropdown border container.</item>
/// </list>
/// </para>
/// </remarks>
[TemplatePart("PART_SearchBox", typeof(TextBox))]
[TemplatePart("PART_DropdownBorder", typeof(Border))]
[PseudoClasses(":open", ":selected", ":searching", ":outlined", ":filled", ":underlined")]
public class AuraComboBox : ComboBox
{
    private TextBox? _searchBox;
    private Border? _dropdownBorder;
    private object? _originalItemsSource;
    private bool _isFiltering;

    // Cached reflection for FilterMemberPath to avoid per-item GetProperty calls
    private System.Reflection.PropertyInfo? _cachedFilterProperty;
    private string? _cachedFilterMemberPath;
    private Type? _cachedFilterPropertyType;

    /// <summary>
    /// Defines the <see cref="PlaceholderText"/> styled property.
    /// </summary>
    public new static readonly StyledProperty<string?> PlaceholderTextProperty =
        AvaloniaProperty.Register<AuraComboBox, string?>(nameof(PlaceholderText));

    /// <summary>
    /// Defines the <see cref="IsSearchable"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsSearchableProperty =
        AvaloniaProperty.Register<AuraComboBox, bool>(nameof(IsSearchable));

    /// <summary>
    /// Defines the <see cref="MaxDropdownHeight"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> MaxDropdownHeightProperty =
        AvaloniaProperty.Register<AuraComboBox, double>(
            nameof(MaxDropdownHeight),
            400.0);

    /// <summary>
    /// Defines the <see cref="Variant"/> styled property.
    /// </summary>
    public static readonly StyledProperty<ComboBoxVariant> VariantProperty =
        AvaloniaProperty.Register<AuraComboBox, ComboBoxVariant>(
            nameof(Variant),
            ComboBoxVariant.Outlined);

    /// <summary>
    /// Defines the <see cref="SearchWatermark"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> SearchWatermarkProperty =
        AvaloniaProperty.Register<AuraComboBox, string?>(
            nameof(SearchWatermark),
            "Search...");

    /// <summary>
    /// Defines the FilterMemberPath styled property for filtering.
    /// </summary>
    public static readonly StyledProperty<string?> FilterMemberPathProperty =
        AvaloniaProperty.Register<AuraComboBox, string?>(
            nameof(FilterMemberPath));

    /// <summary>
    /// Defines the <see cref="DropdownBackground"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> DropdownBackgroundProperty =
        AvaloniaProperty.Register<AuraComboBox, IBrush?>(nameof(DropdownBackground));

    /// <summary>
    /// Defines the <see cref="DropdownCornerRadius"/> styled property.
    /// </summary>
    public static readonly StyledProperty<CornerRadius> DropdownCornerRadiusProperty =
        AvaloniaProperty.Register<AuraComboBox, CornerRadius>(
            nameof(DropdownCornerRadius),
            new CornerRadius(8));

    /// <summary>
    /// Defines the <see cref="DropdownShadow"/> styled property.
    /// </summary>
    public static readonly StyledProperty<BoxShadows> DropdownShadowProperty =
        AvaloniaProperty.Register<AuraComboBox, BoxShadows>(nameof(DropdownShadow));

    /// <summary>
    /// Defines the <see cref="ItemHoverBackground"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> ItemHoverBackgroundProperty =
        AvaloniaProperty.Register<AuraComboBox, IBrush?>(nameof(ItemHoverBackground));

    protected override Type StyleKeyOverride => typeof(ComboBox);

    static AuraComboBox()
    {
        IsDropDownOpenProperty.Changed.AddClassHandler<AuraComboBox>((x, e) => x.OnIsDropDownOpenChanged(e));
        IsSearchableProperty.Changed.AddClassHandler<AuraComboBox>((x, _) => x.OnIsSearchableChanged());
        MaxDropdownHeightProperty.Changed.AddClassHandler<AuraComboBox>((x, _) => x.OnMaxDropdownHeightChanged());
        VariantProperty.Changed.AddClassHandler<AuraComboBox>((x, _) => x.UpdateVariantPseudoClasses());
        SelectedItemProperty.Changed.AddClassHandler<AuraComboBox>((x, _) => x.UpdateSelectedPseudoClass());
        PlaceholderTextProperty.Changed.AddClassHandler<AuraComboBox>((x, _) => x.UpdateAutomationName());
    }

    /// <summary>
    /// Gets or sets the placeholder text shown when no item is selected.
    /// </summary>
    public new string? PlaceholderText
    {
        get => GetValue(PlaceholderTextProperty);
        set => SetValue(PlaceholderTextProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the combo box dropdown includes a search filter.
    /// </summary>
    public bool IsSearchable
    {
        get => GetValue(IsSearchableProperty);
        set => SetValue(IsSearchableProperty, value);
    }

    /// <summary>
    /// Gets or sets the maximum height of the dropdown popup.
    /// </summary>
    public double MaxDropdownHeight
    {
        get => GetValue(MaxDropdownHeightProperty);
        set => SetValue(MaxDropdownHeightProperty, value);
    }

    /// <summary>
    /// Gets or sets the visual variant of the combo box.
    /// </summary>
    public ComboBoxVariant Variant
    {
        get => GetValue(VariantProperty);
        set => SetValue(VariantProperty, value);
    }

    /// <summary>
    /// Gets or sets the watermark text for the search box.
    /// </summary>
    public string? SearchWatermark
    {
        get => GetValue(SearchWatermarkProperty);
        set => SetValue(SearchWatermarkProperty, value);
    }

    /// <summary>
    /// Gets or sets the property path used to obtain the display string for filtering.
    /// When null, items are converted via <c>ToString()</c>.
    /// </summary>
    public string? FilterMemberPath
    {
        get => GetValue(FilterMemberPathProperty);
        set => SetValue(FilterMemberPathProperty, value);
    }

    /// <summary>
    /// Gets or sets the background brush of the dropdown popup.
    /// </summary>
    public IBrush? DropdownBackground
    {
        get => GetValue(DropdownBackgroundProperty);
        set => SetValue(DropdownBackgroundProperty, value);
    }

    /// <summary>
    /// Gets or sets the corner radius of the dropdown popup.
    /// </summary>
    public CornerRadius DropdownCornerRadius
    {
        get => GetValue(DropdownCornerRadiusProperty);
        set => SetValue(DropdownCornerRadiusProperty, value);
    }

    /// <summary>
    /// Gets or sets the box shadow of the dropdown popup.
    /// </summary>
    public BoxShadows DropdownShadow
    {
        get => GetValue(DropdownShadowProperty);
        set => SetValue(DropdownShadowProperty, value);
    }

    /// <summary>
    /// Gets or sets the background brush when hovering over a dropdown item.
    /// </summary>
    public IBrush? ItemHoverBackground
    {
        get => GetValue(ItemHoverBackgroundProperty);
        set => SetValue(ItemHoverBackgroundProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        DetachSearchBox();

        _searchBox = e.NameScope.Find<TextBox>("PART_SearchBox");
        _dropdownBorder = e.NameScope.Find<Border>("PART_DropdownBorder");

        AttachSearchBox();
        UpdateVariantPseudoClasses();
        UpdateSelectedPseudoClass();
        UpdateAutomationName();

        // Apply max dropdown height to the base.
        OnMaxDropdownHeightChanged();
    }

    /// <summary>
    /// Resets the search filter and restores the full items list.
    /// </summary>
    public void ClearSearch()
    {
        if (_searchBox != null)
        {
            _searchBox.Text = string.Empty;
        }

        RestoreOriginalItems();
        PseudoClasses.Set(":searching", false);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (e.Key == Key.Escape && IsDropDownOpen)
        {
            if (IsSearchable && _searchBox != null && !string.IsNullOrEmpty(_searchBox.Text))
            {
                ClearSearch();
                e.Handled = true;
                return;
            }
        }

        base.OnKeyDown(e);
    }

    private void OnIsDropDownOpenChanged(AvaloniaPropertyChangedEventArgs e)
    {
        var isOpen = (bool)e.NewValue!;
        PseudoClasses.Set(":open", isOpen);

        if (isOpen && IsSearchable)
        {
            // Focus the search box when the dropdown opens.
            Dispatcher.UIThread.Post(() =>
            {
                if (_searchBox != null)
                {
                    _searchBox.Focus();
                    _searchBox.SelectAll();
                }
            }, DispatcherPriority.Input);
        }
        else if (!isOpen && IsSearchable)
        {
            // Clear search when dropdown closes.
            Dispatcher.UIThread.Post(() => ClearSearch(), DispatcherPriority.Input);
        }
    }

    private void OnIsSearchableChanged()
    {
        if (!IsSearchable)
        {
            ClearSearch();
        }
    }

    private void OnMaxDropdownHeightChanged()
    {
        // Apply max height to the dropdown border if present, otherwise it will be
        // picked up by the template via a {TemplateBinding MaxDropdownHeight}.
        if (_dropdownBorder != null)
        {
            _dropdownBorder.MaxHeight = MaxDropdownHeight;
        }
    }

    private void AttachSearchBox()
    {
        if (_searchBox != null)
        {
            _searchBox.TextChanged += OnSearchTextChanged;
            _searchBox.KeyDown += OnSearchKeyDown;
        }
    }

    private void DetachSearchBox()
    {
        if (_searchBox != null)
        {
            _searchBox.TextChanged -= OnSearchTextChanged;
            _searchBox.KeyDown -= OnSearchKeyDown;
            _searchBox = null;
        }
    }

    private void OnSearchTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (!IsSearchable || _isFiltering)
            return;

        var searchText = _searchBox?.Text;

        if (string.IsNullOrWhiteSpace(searchText))
        {
            RestoreOriginalItems();
            PseudoClasses.Set(":searching", false);
        }
        else
        {
            FilterItems(searchText);
            PseudoClasses.Set(":searching", true);
        }
    }

    private void OnSearchKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            // Select the first visible item on Enter.
            var items = GetFilteredItemSource();
            if (items != null)
            {
                foreach (var item in items)
                {
                    SetCurrentValue(SelectedItemProperty, item);
                    SetCurrentValue(IsDropDownOpenProperty, false);
                    break;
                }
            }
            e.Handled = true;
        }
        else if (e.Key == Key.Escape)
        {
            ClearSearch();
            e.Handled = true;
        }
    }

    protected virtual void FilterItems(string searchText)
    {
        _isFiltering = true;
        try
        {
            // Back up original items on first filter.
            if (_originalItemsSource == null)
            {
                _originalItemsSource = ItemsSource;
            }

            var filtered = new AvaloniaList<object>();
            var source = _originalItemsSource;

            if (source is IEnumerable enumerable)
            {
                foreach (var item in enumerable)
                {
                    var displayText = GetItemDisplayText(item);
                    if (displayText != null &&
                        displayText.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                    {
                        filtered.Add(item);
                    }
                }
            }

            SetCurrentValue(ItemsSourceProperty, filtered);
        }
        finally
        {
            _isFiltering = false;
        }
    }

    private void RestoreOriginalItems()
    {
        if (_originalItemsSource != null)
        {
            _isFiltering = true;
            try
            {
                SetCurrentValue(ItemsSourceProperty, _originalItemsSource);
                _originalItemsSource = null;
            }
            finally
            {
                _isFiltering = false;
            }
        }
    }

    /// <summary>
    /// Gets the display text for an item, using FilterMemberPath if set, otherwise ToString.
    /// Caches the PropertyInfo reflection lookup to avoid repeated GetProperty calls.
    /// </summary>
    protected virtual string? GetItemDisplayText(object? item)
    {
        if (item == null)
            return null;

        var memberPath = FilterMemberPath;
        if (!string.IsNullOrEmpty(memberPath))
        {
            var itemType = item.GetType();
            // Cache the PropertyInfo if the member path or item type changed
            if (_cachedFilterMemberPath != memberPath || _cachedFilterPropertyType != itemType)
            {
                _cachedFilterMemberPath = memberPath;
                _cachedFilterPropertyType = itemType;
                _cachedFilterProperty = itemType.GetProperty(memberPath);
            }

            if (_cachedFilterProperty != null)
            {
                return _cachedFilterProperty.GetValue(item)?.ToString();
            }
        }

        return item.ToString();
    }

    /// <summary>
    /// Gets the items currently displayed (filtered or all).
    /// </summary>
    private IEnumerable? GetFilteredItemSource()
    {
        return ItemsSource as IEnumerable;
    }

    private void UpdateVariantPseudoClasses()
    {
        PseudoClasses.Set(":outlined", Variant == ComboBoxVariant.Outlined);
        PseudoClasses.Set(":filled", Variant == ComboBoxVariant.Filled);
        PseudoClasses.Set(":underlined", Variant == ComboBoxVariant.Underlined);
    }

    private void UpdateSelectedPseudoClass()
    {
        PseudoClasses.Set(":selected", SelectedItem != null);
    }

    private void UpdateAutomationName()
    {
        var placeholder = PlaceholderText;
        if (!string.IsNullOrEmpty(placeholder))
        {
            SetValue(AutomationProperties.NameProperty, placeholder);
        }
    }
}
