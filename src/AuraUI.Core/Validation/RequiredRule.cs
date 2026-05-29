namespace AuraUI.Core.Validation;

/// <summary>
/// Validates that a value is not null, not empty, and not whitespace.
/// </summary>
public class RequiredRule : ValidationRule
{
    /// <inheritdoc/>
    public override ValidationResult Validate(object? value)
    {
        if (value is null)
            return ValidationResult.Fail(ErrorMessage ?? "This field is required");

        if (value is string s && string.IsNullOrWhiteSpace(s))
            return ValidationResult.Fail(ErrorMessage ?? "This field is required");

        return ValidationResult.Success();
    }
}
