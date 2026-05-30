using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace AuraUI.Controls.Navigation;

/// <summary>
/// A dropdown menu that supports deeply nested submenus. Unlike the basic
/// DropDown which shows a flat list, this control renders
/// multi-level cascading menus that open to the side of their parent item,
/// similar to desktop application menu systems.
/// </summary>
[TemplatePart("PART_Button", typeof(Button))]
[TemplatePart("PART_Popup", typeof(Popup))]
[TemplatePart("PART_RootMenu", typeof(ItemsControl))]
[PseudoClasses(":open", ":closed")]
public class MultiLevelDropdown : TemplatedControl
{
    private Button? _button;
    private Popup? _popup;

    /// <summary>
    /// Defines the <see cref="Items"/> styled property.
    /// </summary>
    public static readonly StyledProperty<System.Collections.IEnumerable?> ItemsProperty =
        AvaloniaProperty.Register<MultiLevelDropdown, System.Collections.IEnumerable?>(nameof(Items));

    /// <summary>
    /// Defines the <see cref="IsOpen"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsOpenProperty =
        AvaloniaProperty.Register<MultiLevelDropdown, bool>(nameof(IsOpen));

    /// <summary>
    /// Defines the <see cref="Placement"/> styled property.
    /// </summary>
    public static readonly StyledProperty<PlacementMode> PlacementProperty =
        AvaloniaProperty.Register<MultiLevelDropdown, PlacementMode>(
            nameof(Placement),
            PlacementMode.BottomEdgeAlignedLeft);

    /// <summary>
    /// Defines the <see cref="MaxSubMenuDepth"/> styled property.
    /// </summary>
    public static readonly StyledProperty<int> MaxSubMenuDepthProperty =
        AvaloniaProperty.Register<MultiLevelDropdown, int>(nameof(MaxSubMenuDepth), 5);

    /// <summary>
    /// Defines the <see cref="SubMenuOpenDelay"/> styled property.
    /// Delay in milliseconds before a submenu opens on hover.
    /// </summary>
    public static readonly StyledProperty<int> SubMenuOpenDelayProperty =
        AvaloniaProperty.Register<MultiLevelDropdown, int>(nameof(SubMenuOpenDelay), 150);

    /// <summary>
    /// Defines the <see cref="SubMenuCloseDelay"/> styled property.
    /// Delay in milliseconds before a submenu closes when hover leaves.
    /// </summary>
    public static readonly StyledProperty<int> SubMenuCloseDelayProperty =
        AvaloniaProperty.Register<MultiLevelDropdown, int>(nameof(SubMenuCloseDelay), 300);

    /// <summary>
    /// Defines the <see cref="SubMenuCornerRadius"/> styled property.
    /// </summary>
    public static readonly StyledProperty<CornerRadius> SubMenuCornerRadiusProperty =
        AvaloniaProperty.Register<MultiLevelDropdown, CornerRadius>(
            nameof(SubMenuCornerRadius),
            new CornerRadius(8));

    /// <summary>
    /// Defines the <see cref="SubMenuShadow"/> styled property.
    /// </summary>
    public static readonly StyledProperty<BoxShadows> SubMenuShadowProperty =
        AvaloniaProperty.Register<MultiLevelDropdown, BoxShadows>(nameof(SubMenuShadow));

    static MultiLevelDropdown()
    {
        IsOpenProperty.Changed.AddClassHandler<MultiLevelDropdown>((x, _) => x.OnIsOpenChanged());
    }

    /// <summary>
    /// Occurs when an item is selected.
    /// </summary>
#pragma warning disable CS0067 // Event is never invoked -- public API for consumers
    public event EventHandler<MultiLevelDropdownItemSelectedEventArgs>? ItemSelected;
#pragma warning restore CS0067

    /// <summary>
    /// Gets or sets the menu items.
    /// </summary>
    public System.Collections.IEnumerable? Items
    {
        get => GetValue(ItemsProperty);
        set => SetValue(ItemsProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the dropdown is open.
    /// </summary>
    public bool IsOpen
    {
        get => GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    /// <summary>
    /// Gets or sets the popup placement mode.
    /// </summary>
    public PlacementMode Placement
    {
        get => GetValue(PlacementProperty);
        set => SetValue(PlacementProperty, value);
    }

    /// <summary>
    /// Gets or sets the maximum submenu nesting depth.
    /// </summary>
    public int MaxSubMenuDepth
    {
        get => GetValue(MaxSubMenuDepthProperty);
        set => SetValue(MaxSubMenuDepthProperty, value);
    }

    /// <summary>
    /// Gets or sets the delay before a submenu opens on hover (ms).
    /// </summary>
    public int SubMenuOpenDelay
    {
        get => GetValue(SubMenuOpenDelayProperty);
        set => SetValue(SubMenuOpenDelayProperty, value);
    }

    /// <summary>
    /// Gets or sets the delay before a submenu closes (ms).
    /// </summary>
    public int SubMenuCloseDelay
    {
        get => GetValue(SubMenuCloseDelayProperty);
        set => SetValue(SubMenuCloseDelayProperty, value);
    }

    /// <summary>
    /// Gets or sets the corner radius for submenus.
    /// </summary>
    public CornerRadius SubMenuCornerRadius
    {
        get => GetValue(SubMenuCornerRadiusProperty);
        set => SetValue(SubMenuCornerRadiusProperty, value);
    }

    /// <summary>
    /// Gets or sets the shadow for submenus.
    /// </summary>
    public BoxShadows SubMenuShadow
    {
        get => GetValue(SubMenuShadowProperty);
        set => SetValue(SubMenuShadowProperty, value);
    }

    /// <summary>
    /// Opens the dropdown.
    /// </summary>
    public void Open()
    {
        IsOpen = true;
    }

    /// <summary>
    /// Closes the dropdown.
    /// </summary>
    public void Close()
    {
        IsOpen = false;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        if (_button != null)
            _button.Click -= OnButtonClick;

        _button = e.NameScope.Find<Button>("PART_Button");
        _popup = e.NameScope.Find<Popup>("PART_Popup");

        if (_button != null)
            _button.Click += OnButtonClick;

        UpdatePseudoClasses();
    }

    private void OnButtonClick(object? sender, RoutedEventArgs e)
    {
        IsOpen = !IsOpen;
    }

    private void OnIsOpenChanged()
    {
        UpdatePseudoClasses();
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":open", IsOpen);
        PseudoClasses.Set(":closed", !IsOpen);
    }
}

/// <summary>
/// Represents a single menu item in a <see cref="MultiLevelDropdown"/>.
/// </summary>
public class MultiLevelDropdownItem
{
    /// <summary>
    /// Gets or sets the display text.
    /// </summary>
    public string? Text { get; set; }

    /// <summary>
    /// Gets or sets the icon.
    /// </summary>
    public object? Icon { get; set; }

    /// <summary>
    /// Gets or sets whether this item is enabled.
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Gets or sets whether this item is a separator.
    /// </summary>
    public bool IsSeparator { get; set; }

    /// <summary>
    /// Gets or sets the child submenu items.
    /// </summary>
    public IList<MultiLevelDropdownItem>? Children { get; set; }

    /// <summary>
    /// Gets or sets the input gesture text (e.g., "Ctrl+S").
    /// </summary>
    public string? InputGestureText { get; set; }

    /// <summary>
    /// Gets or sets the command parameter.
    /// </summary>
    public object? CommandParameter { get; set; }
}

/// <summary>
/// Event arguments for <see cref="MultiLevelDropdown.ItemSelected"/>.
/// </summary>
public class MultiLevelDropdownItemSelectedEventArgs : EventArgs
{
    public MultiLevelDropdownItem Item { get; }

    public MultiLevelDropdownItemSelectedEventArgs(MultiLevelDropdownItem item)
    {
        Item = item;
    }
}
