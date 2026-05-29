using System.ComponentModel.DataAnnotations;

namespace AuraUI.Core.Validation;

/// <summary>
/// Bridges a <see cref="ValidationAttribute"/> from System.ComponentModel.DataAnnotations
/// into the AuraUI validation framework.
/// </summary>
public class DataAnnotationRule : ValidationRule
{
    /// <summary>
    /// Gets or sets the DataAnnotations validation attribute to apply.
    /// </summary>
    public required ValidationAttribute Attribute { get; set; }

    /// <inheritdoc/>
    public override ValidationResult Validate(object? value)
    {
        var context = new ValidationContext(value ?? new object());
        var result = Attribute.GetValidationResult(value, context);

        if (result != System.ComponentModel.DataAnnotations.ValidationResult.Success)
            return ValidationResult.Fail(ErrorMessage ?? result?.ErrorMessage ?? "Validation failed");

        return ValidationResult.Success();
    }
}
