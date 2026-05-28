using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;

namespace AuraUI.Controls.Selection;

/// <summary>
/// An enhanced radio button control supporting default, card, and button visual variants.
/// </summary>
/// <remarks>
/// <para>
/// Supports three visual modes via the <see cref="Variant"/> property:
/// <list type="bullet">
///   <item><see cref="RadioButtonVariant.Default"/>: Standard radio button appearance.</item>
///   <item><see cref="RadioButtonVariant.Card"/>: Full clickable card with check indicator.</item>
///   <item><see cref="RadioButtonVariant.Button"/>: Toggle-button style within a radio group.</item>
/// </list>
/// </para>
/// </remarks>
[PseudoClasses(":checked", ":unchecked", ":pointerover", ":pressed", ":default", ":card", ":button")]
public class AuraRadioButton : RadioButton
{
    /// <summary>
    /// Defines the <see cref="Variant"/> styled property.
    /// </summary>
    public static readonly StyledProperty<RadioButtonVariant> VariantProperty =
        AvaloniaProperty.Register<AuraRadioButton, RadioButtonVariant>(
            nameof(Variant),
            RadioButtonVariant.Default);

    /// <summary>
    /// Defines the <see cref="CardIcon"/> styled property.
    /// </summary>
    public static readonly StyledProperty<object?> CardIconProperty =
        AvaloniaProperty.Register<AuraRadioButton, object?>(nameof(CardIcon));

    /// <summary>
    /// Defines the <see cref="CardDescription"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> CardDescriptionProperty =
        AvaloniaProperty.Register<AuraRadioButton, string?>(nameof(CardDescription));

    static AuraRadioButton()
    {
        VariantProperty.Changed.AddClassHandler<AuraRadioButton>((x, _) => x.UpdatePseudoClasses());
        IsCheckedProperty.Changed.AddClassHandler<AuraRadioButton>((x, _) => x.UpdatePseudoClasses());
    }

    /// <summary>
    /// Gets or sets the visual variant of the radio button.
    /// </summary>
    public RadioButtonVariant Variant
    {
        get => GetValue(VariantProperty);
        set => SetValue(VariantProperty, value);
    }

    /// <summary>
    /// Gets or sets the icon displayed in card variant.
    /// </summary>
    public object? CardIcon
    {
        get => GetValue(CardIconProperty);
        set => SetValue(CardIconProperty, value);
    }

    /// <summary>
    /// Gets or sets a description shown below the header in card variant.
    /// </summary>
    public string? CardDescription
    {
        get => GetValue(CardDescriptionProperty);
        set => SetValue(CardDescriptionProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        UpdatePseudoClasses();
    }

    protected override void OnPointerEntered(PointerEventArgs e)
    {
        base.OnPointerEntered(e);
        PseudoClasses.Set(":pointerover", true);
    }

    protected override void OnPointerExited(PointerEventArgs e)
    {
        base.OnPointerExited(e);
        PseudoClasses.Set(":pointerover", false);
        PseudoClasses.Set(":pressed", false);
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            PseudoClasses.Set(":pressed", true);
        }
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        PseudoClasses.Set(":pressed", false);
    }

    private void UpdatePseudoClasses()
    {
        var isChecked = IsChecked == true;

        PseudoClasses.Set(":checked", isChecked);
        PseudoClasses.Set(":unchecked", !isChecked);

        PseudoClasses.Set(":default", Variant == RadioButtonVariant.Default);
        PseudoClasses.Set(":card", Variant == RadioButtonVariant.Card);
        PseudoClasses.Set(":button", Variant == RadioButtonVariant.Button);
    }
}
