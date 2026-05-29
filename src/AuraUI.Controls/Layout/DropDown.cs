using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace AuraUI.Controls.Layout;

/// <summary>
/// Specifies the placement of the dropdown relative to its anchor.
/// </summary>
public enum DropDownPlacement
{
    Bottom,
    Top,
    Left,
    Right
}

/// <summary>
/// A generic dropdown container that wraps arbitrary content in a popup overlay.
/// Can be used to build custom dropdown menus, popovers, or floating panels.
/// </summary>
[TemplatePart("PART_Popup", typeof(Popup))]
[TemplatePart("PART_Overlay", typeof(Border))]
[PseudoClasses(":open", ":closed")]
public class DropDown : ContentControl
{
    private Popup? _popup;

    /// <summary>
    /// Defines the <see cref="IsOpen"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsOpenProperty =
        AvaloniaProperty.Register<DropDown, bool>(nameof(IsOpen));

    /// <summary>
    /// Defines the <see cref="Placement"/> styled property.
    /// </summary>
    public static readonly StyledProperty<DropDownPlacement> PlacementProperty =
        AvaloniaProperty.Register<DropDown, DropDownPlacement>(nameof(Placement), DropDownPlacement.Bottom);

    /// <summary>
    /// Defines the <see cref="MaxDropDownHeight"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> MaxDropDownHeightProperty =
        AvaloniaProperty.Register<DropDown, double>(
            nameof(MaxDropDownHeight),
            400);

    /// <summary>
    /// Defines the <see cref="StaysOpen"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> StaysOpenProperty =
        AvaloniaProperty.Register<DropDown, bool>(nameof(StaysOpen), false);

    /// <summary>
    /// Defines the <see cref="OverlayBrush"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> OverlayBrushProperty =
        AvaloniaProperty.Register<DropDown, IBrush?>(nameof(OverlayBrush));

    /// <summary>
    /// Defines the <see cref="DropDownContent"/> styled property.
    /// </summary>
    public static readonly StyledProperty<object?> DropDownContentProperty =
        AvaloniaProperty.Register<DropDown, object?>(nameof(DropDownContent));

    /// <summary>
    /// Defines the <see cref="DropDownCornerRadius"/> styled property.
    /// </summary>
    public static readonly StyledProperty<CornerRadius> DropDownCornerRadiusProperty =
        AvaloniaProperty.Register<DropDown, CornerRadius>(nameof(DropDownCornerRadius));

    /// <summary>
    /// Defines the <see cref="DropDownShadow"/> styled property.
    /// </summary>
    public static readonly StyledProperty<BoxShadows> DropDownShadowProperty =
        AvaloniaProperty.Register<DropDown, BoxShadows>(nameof(DropDownShadow));

    static DropDown()
    {
        IsOpenProperty.Changed.AddClassHandler<DropDown>((x, e) => x.OnIsOpenChanged(e));
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
    /// Gets or sets the placement of the dropdown.
    /// </summary>
    public DropDownPlacement Placement
    {
        get => GetValue(PlacementProperty);
        set => SetValue(PlacementProperty, value);
    }

    /// <summary>
    /// Gets or sets the maximum height of the dropdown.
    /// </summary>
    public double MaxDropDownHeight
    {
        get => GetValue(MaxDropDownHeightProperty);
        set => SetValue(MaxDropDownHeightProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the dropdown stays open when clicking outside.
    /// </summary>
    public bool StaysOpen
    {
        get => GetValue(StaysOpenProperty);
        set => SetValue(StaysOpenProperty, value);
    }

    /// <summary>
    /// Gets or sets the overlay background brush.
    /// </summary>
    public IBrush? OverlayBrush
    {
        get => GetValue(OverlayBrushProperty);
        set => SetValue(OverlayBrushProperty, value);
    }

    /// <summary>
    /// Gets or sets the content displayed in the dropdown popup.
    /// </summary>
    public object? DropDownContent
    {
        get => GetValue(DropDownContentProperty);
        set => SetValue(DropDownContentProperty, value);
    }

    /// <summary>
    /// Gets or sets the corner radius of the dropdown panel.
    /// </summary>
    public CornerRadius DropDownCornerRadius
    {
        get => GetValue(DropDownCornerRadiusProperty);
        set => SetValue(DropDownCornerRadiusProperty, value);
    }

    /// <summary>
    /// Gets or sets the box shadow of the dropdown panel.
    /// </summary>
    public BoxShadows DropDownShadow
    {
        get => GetValue(DropDownShadowProperty);
        set => SetValue(DropDownShadowProperty, value);
    }

    /// <summary>
    /// Occurs when the dropdown is opened.
    /// </summary>
    public event EventHandler<RoutedEventArgs>? Opened;

    /// <summary>
    /// Occurs when the dropdown is closed.
    /// </summary>
    public event EventHandler<RoutedEventArgs>? Closed;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _popup = e.NameScope.Find<Popup>("PART_Popup");
        UpdatePseudoClasses();
    }

    protected override void OnLostFocus(FocusChangedEventArgs e)
    {
        base.OnLostFocus(e);
        if (!StaysOpen)
        {
            SetCurrentValue(IsOpenProperty, false);
        }
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        // Toggle open when the anchor area is clicked.
        if (!e.Handled && !IsOpen)
        {
            SetCurrentValue(IsOpenProperty, true);
        }
    }

    private void OnIsOpenChanged(AvaloniaPropertyChangedEventArgs e)
    {
        var isOpen = (bool)e.NewValue!;
        UpdatePseudoClasses();

        if (isOpen)
        {
            Opened?.Invoke(this, new RoutedEventArgs());
        }
        else
        {
            Closed?.Invoke(this, new RoutedEventArgs());
        }
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":open", IsOpen);
        PseudoClasses.Set(":closed", !IsOpen);
    }
}
