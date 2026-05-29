namespace AuraUI.Core.Validation;

/// <summary>
/// Validates that a value equals another value (e.g., password confirmation fields).
/// </summary>
public class CompareRule : ValidationRule
{
    /// <summary>
    /// Gets or sets the name of the other property to compare against.
    /// Used for display purposes.
    /// </summary>
    public string OtherPropertyName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a function that retrieves the value of the other property.
    /// </summary>
    public Func<object?>? OtherValueGetter { get; set; }

    /// <inheritdoc/>
    public override ValidationResult Validate(object? value)
    {
        var other = OtherValueGetter?.Invoke();

        if (!Equals(value, other))
            return ValidationResult.Fail(ErrorMessage ?? "Values do not match");

        return ValidationResult.Success();
    }
}
