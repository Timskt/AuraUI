namespace AuraUI.Core.Validation;

/// <summary>
/// Represents the outcome of a single validation rule evaluation.
/// </summary>
public class ValidationResult
{
    /// <summary>
    /// Gets or sets whether the validation passed.
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// Gets or sets the error message when validation fails.
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;

    /// <summary>
    /// Creates a successful validation result.
    /// </summary>
    public static ValidationResult Success() => new() { IsValid = true };

    /// <summary>
    /// Creates a failed validation result with the specified error message.
    /// </summary>
    public static ValidationResult Fail(string message) => new() { IsValid = false, ErrorMessage = message };
}
