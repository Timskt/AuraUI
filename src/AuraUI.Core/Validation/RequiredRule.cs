namespace AuraUI.Core.Validation;

/// <summary>
/// Validates that a value is not null, not empty, and not whitespace.
/// </summary>
public class RequiredRule : ValidationRule
{
    /// <inheritdoc/>
    public override ValidationResult Validate(object? value)
    {
        var message = string.IsNullOrEmpty(ErrorMessage) ? "This field is required" : ErrorMessage;

        if (value is null)
            return ValidationResult.Fail(message);

        if (value is string s && string.IsNullOrWhiteSpace(s))
            return ValidationResult.Fail(message);

        return ValidationResult.Success();
    }
}
