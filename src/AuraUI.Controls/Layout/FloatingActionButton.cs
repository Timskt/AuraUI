using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace AuraUI.Controls.Layout;

/// <summary>
/// Specifies the position of the FAB within its container.
/// </summary>
public enum FabPosition
{
    BottomRight,
    BottomLeft
}

/// <summary>
/// Specifies the size variant of the FAB.
/// </summary>
public enum FabSize
{
    Mini,
    Regular,
    Large
}

/// <summary>
/// A Material Design Floating Action Button (FAB) with icon, optional label,
/// size variants (Mini/Regular/Large), and extended mode.
/// </summary>
[PseudoClasses(":mini", ":regular", ":large", ":extended", ":bottom-right", ":bottom-left")]
public class FloatingActionButton : ContentControl
{
    /// <summary>
    /// Defines the <see cref="Icon"/> styled property.
    /// </summary>
    public static readonly StyledProperty<object?> IconProperty =
        AvaloniaProperty.Register<FloatingActionButton, object?>(nameof(Icon));

    /// <summary>
    /// Defines the <see cref="Label"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> LabelProperty =
        AvaloniaProperty.Register<FloatingActionButton, string?>(nameof(Label));

    /// <summary>
    /// Defines the <see cref="Position"/> styled property.
    /// </summary>
    public static readonly StyledProperty<FabPosition> PositionProperty =
        AvaloniaProperty.Register<FloatingActionButton, FabPosition>(nameof(Position), FabPosition.BottomRight);

    /// <summary>
    /// Defines the <see cref="ButtonSize"/> styled property.
    /// </summary>
    public static readonly StyledProperty<FabSize> ButtonSizeProperty =
        AvaloniaProperty.Register<FloatingActionButton, FabSize>(nameof(ButtonSize), FabSize.Regular);

    /// <summary>
    /// Defines the <see cref="IsExtended"/> styled property.
    /// When true, the FAB expands to show the label alongside the icon.
    /// </summary>
    public static readonly StyledProperty<bool> IsExtendedProperty =
        AvaloniaProperty.Register<FloatingActionButton, bool>(nameof(IsExtended));

    /// <summary>
    /// Defines the <see cref="FabColor"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> FabColorProperty =
        AvaloniaProperty.Register<FloatingActionButton, IBrush?>(nameof(FabColor));

    /// <summary>
    /// Defines the <see cref="Elevation"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> ElevationProperty =
        AvaloniaProperty.Register<FloatingActionButton, double>(nameof(Elevation), 6);

    /// <summary>
    /// Defines the <see cref="OffsetX"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> OffsetXProperty =
        AvaloniaProperty.Register<FloatingActionButton, double>(nameof(OffsetX), 16);

    /// <summary>
    /// Defines the <see cref="OffsetY"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> OffsetYProperty =
        AvaloniaProperty.Register<FloatingActionButton, double>(nameof(OffsetY), 16);

    static FloatingActionButton()
    {
        ButtonSizeProperty.Changed.AddClassHandler<FloatingActionButton>((x, _) => x.UpdatePseudoClasses());
        IsExtendedProperty.Changed.AddClassHandler<FloatingActionButton>((x, _) => x.UpdatePseudoClasses());
        PositionProperty.Changed.AddClassHandler<FloatingActionButton>((x, _) => x.UpdatePseudoClasses());
    }

    /// <summary>
    /// Gets or sets the icon content.
    /// </summary>
    public object? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    /// <summary>
    /// Gets or sets the label text (shown in extended mode).
    /// </summary>
    public string? Label
    {
        get => GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    /// <summary>
    /// Gets or sets the screen position of the FAB.
    /// </summary>
    public FabPosition Position
    {
        get => GetValue(PositionProperty);
        set => SetValue(PositionProperty, value);
    }

    /// <summary>
    /// Gets or sets the size variant.
    /// </summary>
    public FabSize ButtonSize
    {
        get => GetValue(ButtonSizeProperty);
        set => SetValue(ButtonSizeProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the FAB is in extended mode (shows label).
    /// </summary>
    public bool IsExtended
    {
        get => GetValue(IsExtendedProperty);
        set => SetValue(IsExtendedProperty, value);
    }

    /// <summary>
    /// Gets or sets the FAB background color.
    /// </summary>
    public IBrush? FabColor
    {
        get => GetValue(FabColorProperty);
        set => SetValue(FabColorProperty, value);
    }

    /// <summary>
    /// Gets or sets the elevation in device-independent pixels.
    /// </summary>
    public double Elevation
    {
        get => GetValue(ElevationProperty);
        set => SetValue(ElevationProperty, value);
    }

    /// <summary>
    /// Gets or sets the horizontal offset from the edge.
    /// </summary>
    public double OffsetX
    {
        get => GetValue(OffsetXProperty);
        set => SetValue(OffsetXProperty, value);
    }

    /// <summary>
    /// Gets or sets the vertical offset from the bottom.
    /// </summary>
    public double OffsetY
    {
        get => GetValue(OffsetYProperty);
        set => SetValue(OffsetYProperty, value);
    }

    /// <summary>
    /// Occurs when the FAB is clicked.
    /// </summary>
    public event EventHandler<RoutedEventArgs>? FabClick;

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        UpdatePseudoClasses();
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        FabClick?.Invoke(this, new RoutedEventArgs());
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":mini", ButtonSize == FabSize.Mini);
        PseudoClasses.Set(":regular", ButtonSize == FabSize.Regular);
        PseudoClasses.Set(":large", ButtonSize == FabSize.Large);
        PseudoClasses.Set(":extended", IsExtended);
        PseudoClasses.Set(":bottom-right", Position == FabPosition.BottomRight);
        PseudoClasses.Set(":bottom-left", Position == FabPosition.BottomLeft);
    }
}
