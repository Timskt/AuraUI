using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using AuraUI.Core.Validation;

namespace AuraUI.Core.Helpers;

/// <summary>
/// Attached properties for adding validation support to any Avalonia control.
/// Usage: aura:ValidationHelper.Rules="{StaticResource MyRules}"
/// </summary>
public static class ValidationHelper
{
    #region Rules

    public static readonly AttachedProperty<IList<ValidationRule>?> RulesProperty =
        AvaloniaProperty.RegisterAttached<Control, IList<ValidationRule>?>("Rules", typeof(ValidationHelper));

    public static IList<ValidationRule>? GetRules(Control element) => element.GetValue(RulesProperty);
    public static void SetRules(Control element, IList<ValidationRule>? value) => element.SetValue(RulesProperty, value);

    #endregion

    #region HasError

    public static readonly AttachedProperty<bool> HasErrorProperty =
        AvaloniaProperty.RegisterAttached<Control, bool>("HasError", typeof(ValidationHelper));

    public static bool GetHasError(Control element) => element.GetValue(HasErrorProperty);
    public static void SetHasError(Control element, bool value) => element.SetValue(HasErrorProperty, value);

    #endregion

    #region ErrorText

    public static readonly AttachedProperty<string?> ErrorTextProperty =
        AvaloniaProperty.RegisterAttached<Control, string?>("ErrorText", typeof(ValidationHelper));

    public static string? GetErrorText(Control element) => element.GetValue(ErrorTextProperty);
    public static void SetErrorText(Control element, string? value) => element.SetValue(ErrorTextProperty, value);

    #endregion

    #region ErrorBrush

    public static readonly AttachedProperty<IBrush?> ErrorBrushProperty =
        AvaloniaProperty.RegisterAttached<Control, IBrush?>("ErrorBrush", typeof(ValidationHelper));

    public static IBrush? GetErrorBrush(Control element) => element.GetValue(ErrorBrushProperty);
    public static void SetErrorBrush(Control element, IBrush? value) => element.SetValue(ErrorBrushProperty, value);

    #endregion

    #region ValidateOnPropertyChanged

    public static readonly AttachedProperty<bool> ValidateOnPropertyChangedProperty =
        AvaloniaProperty.RegisterAttached<Control, bool>("ValidateOnPropertyChanged", typeof(ValidationHelper), true);

    public static bool GetValidateOnPropertyChanged(Control element) => element.GetValue(ValidateOnPropertyChangedProperty);
    public static void SetValidateOnPropertyChanged(Control element, bool value) => element.SetValue(ValidateOnPropertyChangedProperty, value);

    #endregion

    #region IsValid

    public static readonly AttachedProperty<bool> IsValidProperty =
        AvaloniaProperty.RegisterAttached<Control, bool>("IsValid", typeof(ValidationHelper));

    public static bool GetIsValid(Control element) => element.GetValue(IsValidProperty);
    public static void SetIsValid(Control element, bool value) => element.SetValue(IsValidProperty, value);

    #endregion

    /// <summary>
    /// Validates a value against all rules attached to the given control.
    /// Sets HasError and ErrorText attached properties on the control.
    /// </summary>
    /// <param name="element">The control to validate.</param>
    /// <param name="value">The value to validate.</param>
    /// <returns>The validation result, or null if no rules are attached.</returns>
    public static ValidationResult? Validate(Control element, object? value)
    {
        var rules = GetRules(element);
        if (rules is null || rules.Count == 0)
        {
            SetHasError(element, false);
            SetErrorText(element, null);
            SetIsValid(element, true);
            return null;
        }

        foreach (var rule in rules)
        {
            var result = rule.Validate(value);
            if (!result.IsValid)
            {
                SetHasError(element, true);
                SetErrorText(element, result.ErrorMessage);
                SetIsValid(element, false);
                return result;
            }
        }

        SetHasError(element, false);
        SetErrorText(element, null);
        SetIsValid(element, true);
        return ValidationResult.Success();
    }
}
