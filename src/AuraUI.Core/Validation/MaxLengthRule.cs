namespace AuraUI.Core.Validation;

/// <summary>
/// Validates that a string value does not exceed a maximum length.
/// Null or non-string values are considered valid (use RequiredRule to enforce presence).
/// </summary>
public class MaxLengthRule : ValidationRule
{
    /// <summary>
    /// Gets or sets the maximum allowed string length.
    /// </summary>
    public int MaxLength { get; set; }

    /// <inheritdoc/>
    public override ValidationResult Validate(object? value)
    {
        if (value is string s && s.Length > MaxLength)
            return ValidationResult.Fail(ErrorMessage ?? $"Maximum length is {MaxLength}");

        return ValidationResult.Success();
    }
}
