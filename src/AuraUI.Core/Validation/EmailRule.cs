namespace AuraUI.Core.Validation;

/// <summary>
/// Validates that a string value is a well-formed email address.
/// Inherits from <see cref="RegexRule"/> with a standard email pattern.
/// </summary>
public class EmailRule : RegexRule
{
    public EmailRule()
    {
        Pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        ErrorMessage = "Invalid email address";
    }
}
