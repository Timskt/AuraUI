namespace AuraUI.Core.Validation;

/// <summary>
/// Validates that a string value meets a minimum length requirement.
/// Null or non-string values are considered valid (use RequiredRule to enforce presence).
/// </summary>
public class MinLengthRule : ValidationRule
{
    /// <summary>
    /// Gets or sets the minimum allowed string length.
    /// </summary>
    public int MinLength { get; set; }

    /// <inheritdoc/>
    public override ValidationResult Validate(object? value)
    {
        if (value is string s && s.Length < MinLength)
            return ValidationResult.Fail(ErrorMessage ?? $"Minimum length is {MinLength}");

        return ValidationResult.Success();
    }
}
