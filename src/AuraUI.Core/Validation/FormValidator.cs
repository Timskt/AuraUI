using System.Reflection;

namespace AuraUI.Core.Validation;

/// <summary>
/// A service that manages validation rules for properties and validates objects.
/// Can be used standalone or integrated with MVVM ViewModels.
/// </summary>
public class FormValidator
{
    private readonly Dictionary<string, IList<ValidationRule>> _rules = new();
    private readonly Dictionary<string, List<ValidationError>> _errors = new();

    /// <summary>
    /// Occurs when validation state changes for any property.
    /// </summary>
    public event EventHandler<ValidationChangedEventArgs>? ValidationChanged;

    /// <summary>
    /// Gets whether any validation errors currently exist.
    /// </summary>
    public bool HasErrors => _errors.Values.Any(list => list.Count > 0);

    /// <summary>
    /// Adds a single validation rule for the specified property.
    /// </summary>
    /// <param name="propertyName">The name of the property to validate.</param>
    /// <param name="rule">The validation rule to add.</param>
    public void AddRule(string propertyName, ValidationRule rule)
    {
        ArgumentException.ThrowIfNullOrEmpty(propertyName);
        ArgumentNullException.ThrowIfNull(rule);

        if (!_rules.TryGetValue(propertyName, out var list))
        {
            list = new List<ValidationRule>();
            _rules[propertyName] = list;
        }

        list.Add(rule);
    }

    /// <summary>
    /// Adds multiple validation rules for the specified property.
    /// </summary>
    /// <param name="propertyName">The name of the property to validate.</param>
    /// <param name="rules">The validation rules to add.</param>
    public void AddRules(string propertyName, IEnumerable<ValidationRule> rules)
    {
        foreach (var rule in rules)
            AddRule(propertyName, rule);
    }

    /// <summary>
    /// Validates a single property against all its registered rules.
    /// </summary>
    /// <param name="propertyName">The property to validate.</param>
    /// <param name="value">The current value of the property.</param>
    /// <returns>True if the property is valid; false otherwise.</returns>
    public bool ValidateProperty(string propertyName, object? value)
    {
        if (!_rules.TryGetValue(propertyName, out var rules))
            return true;

        var errors = new List<ValidationError>();

        foreach (var rule in rules)
        {
            var result = rule.Validate(value);
            if (!result.IsValid)
            {
                errors.Add(new ValidationError
                {
                    PropertyName = propertyName,
                    ErrorMessage = result.ErrorMessage,
                    Rule = rule
                });
            }
        }

        _errors[propertyName] = errors;

        ValidationChanged?.Invoke(this,
            new ValidationChangedEventArgs(propertyName, errors, errors.Count == 0));

        return errors.Count == 0;
    }

    /// <summary>
    /// Validates all properties of the given model by reading property values via reflection.
    /// </summary>
    /// <param name="model">The model object to validate.</param>
    /// <returns>True if the model is valid; false otherwise.</returns>
    public bool ValidateAll(object model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var type = model.GetType();
        var allValid = true;

        foreach (var (propertyName, rules) in _rules)
        {
            var property = type.GetProperty(propertyName,
                BindingFlags.Public | BindingFlags.Instance);

            var value = property?.GetValue(model);
            if (!ValidateProperty(propertyName, value))
                allValid = false;
        }

        return allValid;
    }

    /// <summary>
    /// Gets all current validation errors across all properties.
    /// </summary>
    public IList<ValidationError> GetErrors()
    {
        return _errors.Values.SelectMany(e => e).ToList();
    }

    /// <summary>
    /// Gets validation errors for a specific property.
    /// </summary>
    /// <param name="propertyName">The property name.</param>
    public IList<ValidationError> GetErrors(string propertyName)
    {
        return _errors.TryGetValue(propertyName, out var errors)
            ? errors
            : Array.Empty<ValidationError>();
    }

    /// <summary>
    /// Clears all validation errors and resets state.
    /// </summary>
    public void ClearErrors()
    {
        _errors.Clear();
    }

    /// <summary>
    /// Clears validation errors for a specific property.
    /// </summary>
    /// <param name="propertyName">The property name.</param>
    public void ClearErrors(string propertyName)
    {
        _errors.Remove(propertyName);
    }

    /// <summary>
    /// Gets whether a specific property has validation errors.
    /// </summary>
    /// <param name="propertyName">The property name.</param>
    public bool HasErrorsFor(string propertyName)
    {
        return _errors.TryGetValue(propertyName, out var errors) && errors.Count > 0;
    }

    /// <summary>
    /// Builds a <see cref="FormValidationResult"/> from the current state.
    /// </summary>
    public FormValidationResult ToResult()
    {
        var errors = GetErrors();
        return new FormValidationResult
        {
            IsValid = errors.Count == 0,
            Errors = errors
        };
    }
}
