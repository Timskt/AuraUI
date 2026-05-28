using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.LogicalTree;

namespace AuraUI.Controls.Input;

/// <summary>
/// Defines the visual variant for the toggle button.
/// </summary>
public enum ToggleButtonVariant
{
    Default,
    Primary,
    Success,
    Warning,
    Error
}

/// <summary>
/// Enhanced toggle button control with icon support and variant styling.
/// Supports pseudo-classes: :checked, :unchecked
/// Styled with AuraUI design tokens.
/// </summary>
public class AuraToggleButton : ToggleButton
{
    /// <summary>
    /// Defines the <see cref="Icon"/> property.
    /// </summary>
    public static readonly StyledProperty<object?> IconProperty =
        AvaloniaProperty.Register<AuraToggleButton, object?>(
            nameof(Icon));

    /// <summary>
    /// Defines the <see cref="Variant"/> property.
    /// </summary>
    public static readonly StyledProperty<ToggleButtonVariant> VariantProperty =
        AvaloniaProperty.Register<AuraToggleButton, ToggleButtonVariant>(
            nameof(Variant),
            defaultValue: ToggleButtonVariant.Default);

    /// <summary>
    /// Defines the <see cref="IconSpacing"/> property.
    /// </summary>
    public static readonly StyledProperty<double> IconSpacingProperty =
        AvaloniaProperty.Register<AuraToggleButton, double>(
            nameof(IconSpacing),
            defaultValue: 8.0);

    /// <summary>
    /// Defines the <see cref="CheckedContent"/> property.
    /// </summary>
    public static readonly StyledProperty<object?> CheckedContentProperty =
        AvaloniaProperty.Register<AuraToggleButton, object?>(
            nameof(CheckedContent));

    /// <summary>
    /// Gets or sets the icon displayed alongside the content.
    /// </summary>
    public object? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    /// <summary>
    /// Gets or sets the visual variant of the toggle button.
    /// </summary>
    public ToggleButtonVariant Variant
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

    /// <summary>
    /// Gets or sets the content displayed when the toggle is in the checked state.
    /// If null, the original Content is used for both states.
    /// </summary>
    public object? CheckedContent
    {
        get => GetValue(CheckedContentProperty);
        set => SetValue(CheckedContentProperty, value);
    }

    private object? _uncheckedContent;

    static AuraToggleButton()
    {
        AffectsMeasure<AuraToggleButton>(IconProperty, VariantProperty);

        IsCheckedProperty.Changed.AddClassHandler<AuraToggleButton>((x, e) => x.OnIsCheckedChanged(e));
        VariantProperty.Changed.AddClassHandler<AuraToggleButton>((x, _) => x.UpdateVariantClasses());
    }

    protected override Type StyleKeyOverride => typeof(ToggleButton);

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        // Initialize pseudo-class state
        UpdateCheckedState(IsChecked == true);
        UpdateVariantClasses();
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);

        // Store the unchecked content for toggling
        _uncheckedContent ??= Content;
        UpdateCheckedState(IsChecked == true);
    }

    private void OnIsCheckedChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if (e.NewValue is bool isChecked)
        {
            UpdateCheckedState(isChecked);
        }
    }

    private void UpdateCheckedState(bool isChecked)
    {
        PseudoClasses.Set("checked", isChecked);
        PseudoClasses.Set("unchecked", !isChecked);

        // Swap content based on check state if CheckedContent is set
        if (CheckedContent is not null)
        {
            if (isChecked)
            {
                // Store original content before swapping
                _uncheckedContent ??= Content;
                Content = CheckedContent;
            }
            else if (_uncheckedContent is not null)
            {
                Content = _uncheckedContent;
            }
        }
    }

    private void UpdateVariantClasses()
    {
        // Remove all variant classes
        Classes.Remove("default");
        Classes.Remove("primary");
        Classes.Remove("success");
        Classes.Remove("warning");
        Classes.Remove("error");

        // Add the current variant class
        var variantClass = Variant switch
        {
            ToggleButtonVariant.Default => "default",
            ToggleButtonVariant.Primary => "primary",
            ToggleButtonVariant.Success => "success",
            ToggleButtonVariant.Warning => "warning",
            ToggleButtonVariant.Error => "error",
            _ => "default"
        };

        Classes.Add(variantClass);
    }
}
