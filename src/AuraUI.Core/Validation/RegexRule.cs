using System.Text.RegularExpressions;

namespace AuraUI.Core.Validation;

/// <summary>
/// Validates that a string value matches a regular expression pattern.
/// Null or non-string values are considered valid (use RequiredRule to enforce presence).
/// </summary>
public class RegexRule : ValidationRule
{
    /// <summary>
    /// Gets or sets the regular expression pattern to match against.
    /// </summary>
    public string Pattern { get; set; } = string.Empty;

    /// <inheritdoc/>
    public override ValidationResult Validate(object? value)
    {
        if (value is string s && !Regex.IsMatch(s, Pattern))
            return ValidationResult.Fail(ErrorMessage ?? "Invalid format");

        return ValidationResult.Success();
    }
}
