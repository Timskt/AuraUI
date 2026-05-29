namespace AuraUI.Core.Validation;

/// <summary>
/// Represents a single validation error associated with a property and rule.
/// </summary>
public class ValidationError
{
    /// <summary>
    /// Gets or sets the name of the property that failed validation.
    /// </summary>
    public string PropertyName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the error message for this validation failure.
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the validation rule that produced this error.
    /// </summary>
    public ValidationRule Rule { get; set; } = default!;
}
