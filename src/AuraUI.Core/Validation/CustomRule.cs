namespace AuraUI.Core.Validation;

/// <summary>
/// A validation rule that accepts a custom delegate for ad-hoc validation logic.
/// </summary>
public class CustomRule : ValidationRule
{
    /// <summary>
    /// Gets or sets the custom validation function.
    /// Returns true if the value is valid, false otherwise.
    /// </summary>
    public Func<object?, bool>? Validator { get; set; }

    /// <inheritdoc/>
    public override ValidationResult Validate(object? value)
    {
        if (Validator is not null && !Validator(value))
            return ValidationResult.Fail(ErrorMessage ?? "Validation failed");

        return ValidationResult.Success();
    }
}
