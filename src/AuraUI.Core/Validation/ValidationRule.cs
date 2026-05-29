namespace AuraUI.Core.Validation;

/// <summary>
/// Abstract base class for all validation rules.
/// Subclasses implement <see cref="Validate"/> to perform specific validation logic.
/// </summary>
public abstract class ValidationRule
{
    /// <summary>
    /// Gets or sets the error message to display when validation fails.
    /// If empty, a default message from the rule implementation is used.
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;

    /// <summary>
    /// Validates the given value against this rule.
    /// </summary>
    /// <param name="value">The value to validate.</param>
    /// <returns>A <see cref="ValidationResult"/> indicating success or failure.</returns>
    public abstract ValidationResult Validate(object? value);
}
