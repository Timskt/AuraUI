using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace AuraUI.Controls.Layout;

/// <summary>
/// Specifies the shape of the float button.
/// </summary>
public enum FloatButtonShape
{
    Circle,
    Square
}

/// <summary>
/// Specifies the position of the float button on screen.
/// </summary>
public enum FloatButtonPosition
{
    BottomRight,
    BottomLeft,
    TopRight,
    TopLeft
}

/// <summary>
/// A fixed-position floating action button with icon and tooltip support.
/// Can be placed in any corner of the container.
/// Inspired by Ant Design's FloatButton component.
/// </summary>
[TemplatePart("PART_Button", typeof(Button))]
[PseudoClasses(":circle", ":square", ":bottom-right", ":bottom-left", ":top-right", ":top-left", ":fixed")]
public class FloatButton : ContentControl
{
    private Button? _button;

    /// <summary>
    /// Defines the <see cref="Icon"/> styled property.
    /// </summary>
    public static readonly StyledProperty<object?> IconProperty =
        AvaloniaProperty.Register<FloatButton, object?>(nameof(Icon));

    /// <summary>
    /// Defines the <see cref="Shape"/> styled property.
    /// </summary>
    public static readonly StyledProperty<FloatButtonShape> ShapeProperty =
        AvaloniaProperty.Register<FloatButton, FloatButtonShape>(
            nameof(Shape), FloatButtonShape.Circle);

    /// <summary>
    /// Defines the <see cref="Position"/> styled property.
    /// </summary>
    public static readonly StyledProperty<FloatButtonPosition> PositionProperty =
        AvaloniaProperty.Register<FloatButton, FloatButtonPosition>(
            nameof(Position), FloatButtonPosition.BottomRight);

    /// <summary>
    /// Defines the <see cref="OffsetX"/> styled property.
    /// Horizontal offset from the edge in pixels.
    /// </summary>
    public static readonly StyledProperty<double> OffsetXProperty =
        AvaloniaProperty.Register<FloatButton, double>(nameof(OffsetX), 24);

    /// <summary>
    /// Defines the <see cref="OffsetY"/> styled property.
    /// Vertical offset from the edge in pixels.
    /// </summary>
    public static readonly StyledProperty<double> OffsetYProperty =
        AvaloniaProperty.Register<FloatButton, double>(nameof(OffsetY), 24);

    /// <summary>
    /// Defines the <see cref="IsFixed"/> styled property.
    /// Whether the button uses fixed positioning (stays in viewport).
    /// </summary>
    public static readonly StyledProperty<bool> IsFixedProperty =
        AvaloniaProperty.Register<FloatButton, bool>(nameof(IsFixed), true);

    /// <summary>
    /// Defines the <see cref="TooltipText"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> TooltipTextProperty =
        AvaloniaProperty.Register<FloatButton, string?>(nameof(TooltipText));

    /// <summary>
    /// Defines the <see cref="ButtonSize"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> ButtonSizeProperty =
        AvaloniaProperty.Register<FloatButton, double>(nameof(ButtonSize), 40);

    /// <summary>
    /// Defines the <see cref="Badge"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> BadgeProperty =
        AvaloniaProperty.Register<FloatButton, string?>(nameof(Badge));

    static FloatButton()
    {
        ShapeProperty.Changed.AddClassHandler<FloatButton>((x, _) => x.UpdatePseudoClasses());
        PositionProperty.Changed.AddClassHandler<FloatButton>((x, _) => x.UpdatePseudoClasses());
        IsFixedProperty.Changed.AddClassHandler<FloatButton>((x, _) => x.UpdatePseudoClasses());
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
    /// Gets or sets the button shape.
    /// </summary>
    public FloatButtonShape Shape
    {
        get => GetValue(ShapeProperty);
        set => SetValue(ShapeProperty, value);
    }

    /// <summary>
    /// Gets or sets the button position.
    /// </summary>
    public FloatButtonPosition Position
    {
        get => GetValue(PositionProperty);
        set => SetValue(PositionProperty, value);
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
    /// Gets or sets the vertical offset from the edge.
    /// </summary>
    public double OffsetY
    {
        get => GetValue(OffsetYProperty);
        set => SetValue(OffsetYProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the button uses fixed positioning.
    /// </summary>
    public bool IsFixed
    {
        get => GetValue(IsFixedProperty);
        set => SetValue(IsFixedProperty, value);
    }

    /// <summary>
    /// Gets or sets the tooltip text.
    /// </summary>
    public string? TooltipText
    {
        get => GetValue(TooltipTextProperty);
        set => SetValue(TooltipTextProperty, value);
    }

    /// <summary>
    /// Gets or sets the button size.
    /// </summary>
    public double ButtonSize
    {
        get => GetValue(ButtonSizeProperty);
        set => SetValue(ButtonSizeProperty, value);
    }

    /// <summary>
    /// Gets or sets the badge text (e.g., notification count).
    /// </summary>
    public string? Badge
    {
        get => GetValue(BadgeProperty);
        set => SetValue(BadgeProperty, value);
    }

    /// <summary>
    /// Occurs when the float button is clicked.
    /// </summary>
    public event EventHandler<RoutedEventArgs>? FloatButtonClick;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        if (_button != null)
        {
            _button.Click -= OnButtonClick;
        }

        _button = e.NameScope.Find<Button>("PART_Button");

        if (_button != null)
        {
            _button.Click += OnButtonClick;
            ToolTip.SetTip(_button, TooltipText);
        }

        UpdatePseudoClasses();
    }

    private void OnButtonClick(object? sender, RoutedEventArgs e)
    {
        FloatButtonClick?.Invoke(this, e);
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":circle", Shape == FloatButtonShape.Circle);
        PseudoClasses.Set(":square", Shape == FloatButtonShape.Square);
        PseudoClasses.Set(":bottom-right", Position == FloatButtonPosition.BottomRight);
        PseudoClasses.Set(":bottom-left", Position == FloatButtonPosition.BottomLeft);
        PseudoClasses.Set(":top-right", Position == FloatButtonPosition.TopRight);
        PseudoClasses.Set(":top-left", Position == FloatButtonPosition.TopLeft);
        PseudoClasses.Set(":fixed", IsFixed);
    }
}
