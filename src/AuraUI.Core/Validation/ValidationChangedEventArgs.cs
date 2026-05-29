namespace AuraUI.Core.Validation;

/// <summary>
/// Event arguments raised when validation state changes.
/// </summary>
public class ValidationChangedEventArgs : EventArgs
{
    /// <summary>
    /// Gets the name of the property whose validation state changed.
    /// </summary>
    public string PropertyName { get; }

    /// <summary>
    /// Gets the list of current validation errors for the property.
    /// </summary>
    public IList<ValidationError> Errors { get; }

    /// <summary>
    /// Gets whether the property is currently valid.
    /// </summary>
    public bool IsValid { get; }

    public ValidationChangedEventArgs(string propertyName, IList<ValidationError> errors, bool isValid)
    {
        PropertyName = propertyName;
        Errors = errors;
        IsValid = isValid;
    }
}
