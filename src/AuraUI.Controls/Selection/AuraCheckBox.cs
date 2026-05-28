using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;

namespace AuraUI.Controls.Selection;

/// <summary>
/// An enhanced check box control supporting default and card visual variants
/// with custom check mark animation support.
/// </summary>
[PseudoClasses(":checked", ":unchecked", ":indeterminate", ":pointerover", ":pressed", ":default", ":card")]
public class AuraCheckBox : CheckBox
{
    /// <summary>
    /// Defines the <see cref="Variant"/> styled property.
    /// </summary>
    public static readonly StyledProperty<CheckBoxVariant> VariantProperty =
        AvaloniaProperty.Register<AuraCheckBox, CheckBoxVariant>(
            nameof(Variant),
            CheckBoxVariant.Default);

    /// <summary>
    /// Defines the <see cref="CardIcon"/> styled property.
    /// </summary>
    public static readonly StyledProperty<object?> CardIconProperty =
        AvaloniaProperty.Register<AuraCheckBox, object?>(nameof(CardIcon));

    /// <summary>
    /// Defines the <see cref="CardDescription"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> CardDescriptionProperty =
        AvaloniaProperty.Register<AuraCheckBox, string?>(nameof(CardDescription));

    /// <summary>
    /// Defines the <see cref="CheckMarkBrush"/> styled property.
    /// </summary>
    public static readonly StyledProperty<Avalonia.Media.IBrush?> CheckMarkBrushProperty =
        AvaloniaProperty.Register<AuraCheckBox, Avalonia.Media.IBrush?>(nameof(CheckMarkBrush));

    /// <summary>
    /// Defines the <see cref="CheckMarkAnimationDuration"/> styled property.
    /// </summary>
    public static readonly StyledProperty<TimeSpan> CheckMarkAnimationDurationProperty =
        AvaloniaProperty.Register<AuraCheckBox, TimeSpan>(
            nameof(CheckMarkAnimationDuration),
            TimeSpan.FromMilliseconds(200));

    static AuraCheckBox()
    {
        VariantProperty.Changed.AddClassHandler<AuraCheckBox>((x, _) => x.UpdatePseudoClasses());
        IsCheckedProperty.Changed.AddClassHandler<AuraCheckBox>((x, _) => x.UpdatePseudoClasses());
    }

    /// <summary>
    /// Gets or sets the visual variant of the check box.
    /// </summary>
    public CheckBoxVariant Variant
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

    /// <summary>
    /// Gets or sets the brush used for the check mark glyph.
    /// </summary>
    public Avalonia.Media.IBrush? CheckMarkBrush
    {
        get => GetValue(CheckMarkBrushProperty);
        set => SetValue(CheckMarkBrushProperty, value);
    }

    /// <summary>
    /// Gets or sets the duration of the check mark animation.
    /// </summary>
    public TimeSpan CheckMarkAnimationDuration
    {
        get => GetValue(CheckMarkAnimationDurationProperty);
        set => SetValue(CheckMarkAnimationDurationProperty, value);
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
        var isChecked = IsChecked;

        PseudoClasses.Set(":checked", isChecked == true);
        PseudoClasses.Set(":unchecked", isChecked == false);
        PseudoClasses.Set(":indeterminate", isChecked == null);

        PseudoClasses.Set(":default", Variant == CheckBoxVariant.Default);
        PseudoClasses.Set(":card", Variant == CheckBoxVariant.Card);
    }
}
