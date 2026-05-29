namespace AuraUI.Core.Validation;

/// <summary>
/// Validates that a numeric value falls within a specified range.
/// Supports any <see cref="IComparable"/> value type.
/// </summary>
public class RangeRule : ValidationRule
{
    /// <summary>
    /// Gets or sets the minimum allowed value. Defaults to <see cref="double.MinValue"/>.
    /// </summary>
    public double Min { get; set; } = double.MinValue;

    /// <summary>
    /// Gets or sets the maximum allowed value. Defaults to <see cref="double.MaxValue"/>.
    /// </summary>
    public double Max { get; set; } = double.MaxValue;

    /// <inheritdoc/>
    public override ValidationResult Validate(object? value)
    {
        if (value is null)
            return ValidationResult.Success();

        if (value is IComparable comparable)
        {
            try
            {
                if (comparable.CompareTo(Min) < 0 || comparable.CompareTo(Max) > 0)
                    return ValidationResult.Fail(ErrorMessage ?? $"Value must be between {Min} and {Max}");
            }
            catch (ArgumentException)
            {
                // Type mismatch in CompareTo — try converting to double
                if (value is not null && double.TryParse(value.ToString(), out var d))
                {
                    if (d < Min || d > Max)
                        return ValidationResult.Fail(ErrorMessage ?? $"Value must be between {Min} and {Max}");
                }
            }
        }

        return ValidationResult.Success();
    }
}
