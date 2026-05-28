using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace AuraUI.Controls.Input;

/// <summary>
/// Enhanced repeat button styled with AuraUI design tokens.
/// Supports the same variant classes as AuraButton:
/// .primary, .secondary, .destructive, .outline, .ghost, .link
/// Supports size classes: .sm, .md, .lg
/// </summary>
public class AuraRepeatButton : RepeatButton
{
    /// <summary>
    /// Defines the <see cref="Icon"/> property.
    /// </summary>
    public static readonly StyledProperty<object?> IconProperty =
        AvaloniaProperty.Register<AuraRepeatButton, object?>(
            nameof(Icon));

    /// <summary>
    /// Defines the <see cref="IconPosition"/> property.
    /// </summary>
    public static readonly StyledProperty<IconPosition> IconPositionProperty =
        AvaloniaProperty.Register<AuraRepeatButton, IconPosition>(
            nameof(IconPosition),
            defaultValue: IconPosition.Left);

    /// <summary>
    /// Defines the <see cref="Variant"/> property.
    /// </summary>
    public static readonly StyledProperty<ButtonVariant> VariantProperty =
        AvaloniaProperty.Register<AuraRepeatButton, ButtonVariant>(
            nameof(Variant),
            defaultValue: ButtonVariant.Default);

    /// <summary>
    /// Defines the <see cref="IconSpacing"/> property.
    /// </summary>
    public static readonly StyledProperty<double> IconSpacingProperty =
        AvaloniaProperty.Register<AuraRepeatButton, double>(
            nameof(IconSpacing),
            defaultValue: 8.0);

    /// <summary>
    /// Gets or sets the icon displayed alongside the content.
    /// </summary>
    public object? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    /// <summary>
    /// Gets or sets the position of the icon relative to the content.
    /// </summary>
    public IconPosition IconPosition
    {
        get => GetValue(IconPositionProperty);
        set => SetValue(IconPositionProperty, value);
    }

    /// <summary>
    /// Gets or sets the visual variant of the repeat button.
    /// </summary>
    public ButtonVariant Variant
    {
        get => GetValue(VariantProperty);
        set => SetValue(VariantProperty, value);
    }

    /// <summary>
    /// Gets or sets the spacing between the icon and the content.
    /// </summary>
    public double IconSpacing
    {
        get => GetValue(IconSpacingProperty);
        set => SetValue(IconSpacingProperty, value);
    }

    static AuraRepeatButton()
    {
        AffectsMeasure<AuraRepeatButton>(IconProperty, IconPositionProperty, VariantProperty);

        VariantProperty.Changed.AddClassHandler<AuraRepeatButton>((x, _) => x.UpdateVariantClasses());
    }

    protected override Type StyleKeyOverride => typeof(RepeatButton);

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        UpdateVariantClasses();
    }

    private void UpdateVariantClasses()
    {
        // Remove all variant classes
        Classes.Remove("default");
        Classes.Remove("primary");
        Classes.Remove("secondary");
        Classes.Remove("destructive");
        Classes.Remove("outline");
        Classes.Remove("ghost");
        Classes.Remove("link");

        // Add the current variant class
        var variantClass = Variant switch
        {
            ButtonVariant.Default => "default",
            ButtonVariant.Primary => "primary",
            ButtonVariant.Secondary => "secondary",
            ButtonVariant.Destructive => "destructive",
            ButtonVariant.Outline => "outline",
            ButtonVariant.Ghost => "ghost",
            ButtonVariant.Link => "link",
            _ => "default"
        };

        Classes.Add(variantClass);
    }
}

/// <summary>
/// Defines visual button variants shared across AuraUI button controls.
/// </summary>
public enum ButtonVariant
{
    Default,
    Primary,
    Secondary,
    Destructive,
    Outline,
    Ghost,
    Link
}
