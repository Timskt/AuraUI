namespace AuraUI.Core.Validation;

/// <summary>
/// Represents the aggregate result of validating an entire form or model.
/// </summary>
public class FormValidationResult
{
    /// <summary>
    /// Gets or sets whether the entire form is valid (no errors).
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// Gets or sets all validation errors across all properties.
    /// </summary>
    public IList<ValidationError> Errors { get; set; } = new List<ValidationError>();

    /// <summary>
    /// Gets errors grouped by property name.
    /// </summary>
    public IDictionary<string, IList<ValidationError>> ErrorsByProperty =>
        Errors.GroupBy(e => e.PropertyName)
              .ToDictionary(g => g.Key, g => (IList<ValidationError>)g.ToList());

    /// <summary>
    /// Creates a successful form validation result.
    /// </summary>
    public static FormValidationResult Success() => new() { IsValid = true };

    /// <summary>
    /// Creates a failed form validation result with the given errors.
    /// </summary>
    public static FormValidationResult Fail(IList<ValidationError> errors) =>
        new() { IsValid = false, Errors = errors };
}
