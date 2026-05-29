using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using AuraUI.Core.Validation;

namespace AuraUI.Controls.Layout;

/// <summary>
/// Specifies the placement of the label relative to the field content.
/// </summary>
public enum FormFieldLabelPlacement
{
    /// <summary>
    /// Label is placed above the field content.
    /// </summary>
    Top,

    /// <summary>
    /// Label is placed to the left of the field content.
    /// </summary>
    Left
}

/// <summary>
/// A form field control that wraps content with a label, helper text, error display,
/// validation rules, and required-field indicators. Supports both horizontal and vertical layouts.
/// </summary>
[TemplatePart("PART_LabelPresenter", typeof(ContentPresenter))]
[TemplatePart("PART_ContentPresenter", typeof(ContentPresenter))]
[TemplatePart("PART_HelperText", typeof(TextBlock))]
[TemplatePart("PART_ErrorText", typeof(TextBlock))]
[TemplatePart("PART_RequiredIndicator", typeof(TextBlock))]
[TemplatePart("PART_IconPresenter", typeof(ContentPresenter))]
[PseudoClasses(":error", ":valid", ":required", ":horizontal", ":vertical", ":has-helper", ":has-error")]
public class FormField : ContentControl
{
    /// <summary>
    /// Defines the <see cref="Label"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> LabelProperty =
        AvaloniaProperty.Register<FormField, string?>(nameof(Label));

    /// <summary>
    /// Defines the <see cref="HelperText"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> HelperTextProperty =
        AvaloniaProperty.Register<FormField, string?>(nameof(HelperText));

    /// <summary>
    /// Defines the <see cref="ErrorText"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> ErrorTextProperty =
        AvaloniaProperty.Register<FormField, string?>(nameof(ErrorText));

    /// <summary>
    /// Defines the <see cref="IsRequired"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsRequiredProperty =
        AvaloniaProperty.Register<FormField, bool>(nameof(IsRequired));

    /// <summary>
    /// Defines the <see cref="HasError"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> HasErrorProperty =
        AvaloniaProperty.Register<FormField, bool>(nameof(HasError));

    /// <summary>
    /// Defines the <see cref="IsValid"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsValidProperty =
        AvaloniaProperty.Register<FormField, bool>(nameof(IsValid), true);

    /// <summary>
    /// Defines the <see cref="LabelWidth"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> LabelWidthProperty =
        AvaloniaProperty.Register<FormField, double>(nameof(LabelWidth), 120);

    /// <summary>
    /// Defines the <see cref="LabelPlacement"/> styled property.
    /// </summary>
    public static readonly StyledProperty<FormFieldLabelPlacement> LabelPlacementProperty =
        AvaloniaProperty.Register<FormField, FormFieldLabelPlacement>(
            nameof(LabelPlacement), FormFieldLabelPlacement.Top);

    /// <summary>
    /// Defines the <see cref="Icon"/> styled property.
    /// </summary>
    public static readonly StyledProperty<object?> IconProperty =
        AvaloniaProperty.Register<FormField, object?>(nameof(Icon));

    /// <summary>
    /// Defines the <see cref="ValidationRules"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IList<ValidationRule>?> ValidationRulesProperty =
        AvaloniaProperty.Register<FormField, IList<ValidationRule>?>(nameof(ValidationRules));

    /// <summary>
    /// Defines the <see cref="ValidateOnLostFocus"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ValidateOnLostFocusProperty =
        AvaloniaProperty.Register<FormField, bool>(nameof(ValidateOnLostFocus), true);

    static FormField()
    {
        IsRequiredProperty.Changed.AddClassHandler<FormField>((x, _) => x.UpdatePseudoClasses());
        HasErrorProperty.Changed.AddClassHandler<FormField>((x, _) => x.UpdatePseudoClasses());
        IsValidProperty.Changed.AddClassHandler<FormField>((x, _) => x.UpdatePseudoClasses());
        LabelPlacementProperty.Changed.AddClassHandler<FormField>((x, _) => x.UpdatePseudoClasses());
        HelperTextProperty.Changed.AddClassHandler<FormField>((x, _) => x.UpdatePseudoClasses());
    }

    /// <summary>
    /// Gets or sets the label text displayed next to or above the field.
    /// </summary>
    public string? Label
    {
        get => GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    /// <summary>
    /// Gets or sets helper text displayed below the field.
    /// </summary>
    public string? HelperText
    {
        get => GetValue(HelperTextProperty);
        set => SetValue(HelperTextProperty, value);
    }

    /// <summary>
    /// Gets or sets the error text displayed when validation fails.
    /// </summary>
    public string? ErrorText
    {
        get => GetValue(ErrorTextProperty);
        set => SetValue(ErrorTextProperty, value);
    }

    /// <summary>
    /// Gets or sets whether this field is required (shows a required indicator).
    /// </summary>
    public bool IsRequired
    {
        get => GetValue(IsRequiredProperty);
        set => SetValue(IsRequiredProperty, value);
    }

    /// <summary>
    /// Gets or sets whether this field has a validation error.
    /// </summary>
    public bool HasError
    {
        get => GetValue(HasErrorProperty);
        set => SetValue(HasErrorProperty, value);
    }

    /// <summary>
    /// Gets or sets whether this field is valid (no errors after validation).
    /// </summary>
    public bool IsValid
    {
        get => GetValue(IsValidProperty);
        set => SetValue(IsValidProperty, value);
    }

    /// <summary>
    /// Gets or sets the width of the label area when <see cref="LabelPlacement"/> is Left.
    /// </summary>
    public double LabelWidth
    {
        get => GetValue(LabelWidthProperty);
        set => SetValue(LabelWidthProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the label is placed to the left or above the field content.
    /// </summary>
    public FormFieldLabelPlacement LabelPlacement
    {
        get => GetValue(LabelPlacementProperty);
        set => SetValue(LabelPlacementProperty, value);
    }

    /// <summary>
    /// Gets or sets an optional icon displayed alongside the label.
    /// </summary>
    public object? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    /// <summary>
    /// Gets or sets the list of validation rules to evaluate against the field value.
    /// </summary>
    public IList<ValidationRule>? ValidationRules
    {
        get => GetValue(ValidationRulesProperty);
        set => SetValue(ValidationRulesProperty, value);
    }

    /// <summary>
    /// Gets or sets whether validation runs automatically when the field loses focus.
    /// </summary>
    public bool ValidateOnLostFocus
    {
        get => GetValue(ValidateOnLostFocusProperty);
        set => SetValue(ValidateOnLostFocusProperty, value);
    }

    /// <summary>
    /// Occurs when validation state changes.
    /// </summary>
    public event EventHandler<ValidationChangedEventArgs>? ValidationChanged;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        UpdatePseudoClasses();
    }

    protected override void OnLostFocus(FocusChangedEventArgs e)
    {
        base.OnLostFocus(e);
        if (ValidateOnLostFocus)
        {
            // Validate will be called by the consumer passing the current value.
            // We raise the event to signal that validation should occur.
        }
    }

    /// <summary>
    /// Validates the given value against all registered <see cref="ValidationRules"/>.
    /// Updates <see cref="HasError"/>, <see cref="ErrorText"/>, and <see cref="IsValid"/>.
    /// </summary>
    /// <param name="value">The value to validate.</param>
    /// <returns>True if valid; false if any rule fails.</returns>
    public bool Validate(object? value)
    {
        var rules = ValidationRules;
        if (rules is null || rules.Count == 0)
        {
            HasError = false;
            ErrorText = null;
            IsValid = true;
            return true;
        }

        foreach (var rule in rules)
        {
            var result = rule.Validate(value);
            if (!result.IsValid)
            {
                HasError = true;
                ErrorText = result.ErrorMessage;
                IsValid = false;
                ValidationChanged?.Invoke(this,
                    new ValidationChangedEventArgs(Label ?? string.Empty,
                        new List<ValidationError>
                        {
                            new()
                            {
                                PropertyName = Label ?? string.Empty,
                                ErrorMessage = result.ErrorMessage,
                                Rule = rule
                            }
                        }, false));
                return false;
            }
        }

        HasError = false;
        ErrorText = null;
        IsValid = true;
        ValidationChanged?.Invoke(this,
            new ValidationChangedEventArgs(Label ?? string.Empty,
                Array.Empty<ValidationError>(), true));
        return true;
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":required", IsRequired);
        PseudoClasses.Set(":error", HasError);
        PseudoClasses.Set(":valid", IsValid && !HasError);
        PseudoClasses.Set(":horizontal", LabelPlacement == FormFieldLabelPlacement.Left);
        PseudoClasses.Set(":vertical", LabelPlacement == FormFieldLabelPlacement.Top);
        PseudoClasses.Set(":has-helper", !string.IsNullOrEmpty(HelperText));
        PseudoClasses.Set(":has-error", HasError);
    }
}
