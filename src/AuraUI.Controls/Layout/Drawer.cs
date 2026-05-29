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
/// Specifies which edge the drawer slides in from.
/// </summary>
public enum DrawerPlacement
{
    Left,
    Right,
    Top,
    Bottom
}

/// <summary>
/// A slide-in panel overlay that appears from the edge of its container.
/// Supports four placement directions, overlay backdrop, and animation.
/// </summary>
[TemplatePart("PART_Overlay", typeof(Border))]
[TemplatePart("PART_DrawerPanel", typeof(Border))]
[PseudoClasses(":open", ":closed", ":left", ":right", ":top", ":bottom")]
public class Drawer : ContentControl
{
    /// <summary>
    /// Defines the <see cref="IsOpen"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsOpenProperty =
        AvaloniaProperty.Register<Drawer, bool>(nameof(IsOpen));

    /// <summary>
    /// Defines the <see cref="Placement"/> styled property.
    /// </summary>
    public static readonly StyledProperty<DrawerPlacement> PlacementProperty =
        AvaloniaProperty.Register<Drawer, DrawerPlacement>(nameof(Placement), DrawerPlacement.Right);

    /// <summary>
    /// Defines the <see cref="OverlayBrush"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> OverlayBrushProperty =
        AvaloniaProperty.Register<Drawer, IBrush?>(nameof(OverlayBrush));

    /// <summary>
    /// Defines the <see cref="CloseOnOverlayClick"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> CloseOnOverlayClickProperty =
        AvaloniaProperty.Register<Drawer, bool>(nameof(CloseOnOverlayClick), true);

    /// <summary>
    /// Defines the <see cref="AnimationDuration"/> styled property.
    /// </summary>
    public static readonly StyledProperty<TimeSpan> AnimationDurationProperty =
        AvaloniaProperty.Register<Drawer, TimeSpan>(
            nameof(AnimationDuration),
            TimeSpan.FromMilliseconds(250));

    /// <summary>
    /// Defines the <see cref="DrawerCornerRadius"/> styled property.
    /// </summary>
    public static readonly StyledProperty<CornerRadius> DrawerCornerRadiusProperty =
        AvaloniaProperty.Register<Drawer, CornerRadius>(nameof(DrawerCornerRadius));

    /// <summary>
    /// Defines the <see cref="DrawerShadow"/> styled property.
    /// </summary>
    public static readonly StyledProperty<BoxShadows> DrawerShadowProperty =
        AvaloniaProperty.Register<Drawer, BoxShadows>(nameof(DrawerShadow));

    /// <summary>
    /// Defines the <see cref="DrawerWidth"/> styled property (used for Left/Right placement).
    /// </summary>
    public static readonly StyledProperty<double> DrawerWidthProperty =
        AvaloniaProperty.Register<Drawer, double>(nameof(DrawerWidth), 320);

    /// <summary>
    /// Defines the <see cref="DrawerHeight"/> styled property (used for Top/Bottom placement).
    /// </summary>
    public static readonly StyledProperty<double> DrawerHeightProperty =
        AvaloniaProperty.Register<Drawer, double>(nameof(DrawerHeight), 320);

    static Drawer()
    {
        IsOpenProperty.Changed.AddClassHandler<Drawer>((x, e) => x.OnIsOpenChanged(e));
        PlacementProperty.Changed.AddClassHandler<Drawer>((x, _) => x.UpdatePlacementPseudoClasses());
    }

    /// <summary>
    /// Gets or sets whether the drawer is currently visible.
    /// </summary>
    public bool IsOpen
    {
        get => GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    /// <summary>
    /// Gets or sets which edge the drawer slides in from.
    /// </summary>
    public DrawerPlacement Placement
    {
        get => GetValue(PlacementProperty);
        set => SetValue(PlacementProperty, value);
    }

    /// <summary>
    /// Gets or sets the overlay background brush shown behind the drawer.
    /// </summary>
    public IBrush? OverlayBrush
    {
        get => GetValue(OverlayBrushProperty);
        set => SetValue(OverlayBrushProperty, value);
    }

    /// <summary>
    /// Gets or sets whether clicking the overlay closes the drawer.
    /// </summary>
    public bool CloseOnOverlayClick
    {
        get => GetValue(CloseOnOverlayClickProperty);
        set => SetValue(CloseOnOverlayClickProperty, value);
    }

    /// <summary>
    /// Gets or sets the duration of the slide animation.
    /// </summary>
    public TimeSpan AnimationDuration
    {
        get => GetValue(AnimationDurationProperty);
        set => SetValue(AnimationDurationProperty, value);
    }

    /// <summary>
    /// Gets or sets the corner radius of the drawer panel.
    /// </summary>
    public CornerRadius DrawerCornerRadius
    {
        get => GetValue(DrawerCornerRadiusProperty);
        set => SetValue(DrawerCornerRadiusProperty, value);
    }

    /// <summary>
    /// Gets or sets the box shadow of the drawer panel.
    /// </summary>
    public BoxShadows DrawerShadow
    {
        get => GetValue(DrawerShadowProperty);
        set => SetValue(DrawerShadowProperty, value);
    }

    /// <summary>
    /// Gets or sets the width of the drawer (for Left/Right placement).
    /// </summary>
    public double DrawerWidth
    {
        get => GetValue(DrawerWidthProperty);
        set => SetValue(DrawerWidthProperty, value);
    }

    /// <summary>
    /// Gets or sets the height of the drawer (for Top/Bottom placement).
    /// </summary>
    public double DrawerHeight
    {
        get => GetValue(DrawerHeightProperty);
        set => SetValue(DrawerHeightProperty, value);
    }

    /// <summary>
    /// Occurs when the drawer is opened.
    /// </summary>
    public event EventHandler<RoutedEventArgs>? Opened;

    /// <summary>
    /// Occurs when the drawer is closed.
    /// </summary>
    public event EventHandler<RoutedEventArgs>? Closed;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        var overlay = e.NameScope.Find<Border>("PART_Overlay");
        if (overlay != null)
        {
            overlay.PointerPressed += OnOverlayPointerPressed;
        }

        UpdatePlacementPseudoClasses();
        UpdateOpenPseudoClasses();
    }

    private void OnIsOpenChanged(AvaloniaPropertyChangedEventArgs e)
    {
        UpdateOpenPseudoClasses();

        if ((bool)e.NewValue!)
        {
            IsVisible = true;
            Opened?.Invoke(this, new RoutedEventArgs());
        }
        else
        {
            Closed?.Invoke(this, new RoutedEventArgs());
        }
    }

    private void OnOverlayPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (CloseOnOverlayClick)
        {
            SetCurrentValue(IsOpenProperty, false);
        }
    }

    private void UpdateOpenPseudoClasses()
    {
        PseudoClasses.Set(":open", IsOpen);
        PseudoClasses.Set(":closed", !IsOpen);
    }

    private void UpdatePlacementPseudoClasses()
    {
        PseudoClasses.Set(":left", Placement == DrawerPlacement.Left);
        PseudoClasses.Set(":right", Placement == DrawerPlacement.Right);
        PseudoClasses.Set(":top", Placement == DrawerPlacement.Top);
        PseudoClasses.Set(":bottom", Placement == DrawerPlacement.Bottom);
    }
}
