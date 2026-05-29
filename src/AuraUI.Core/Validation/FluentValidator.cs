namespace AuraUI.Core.Validation;

/// <summary>
/// A fluent API for building validation rules against a model type.
/// Provides a concise, chainable syntax for defining validation.
/// </summary>
/// <typeparam name="T">The type of model to validate.</typeparam>
public class FluentValidator<T>
{
    private readonly List<(Func<T, object?> Selector, ValidationRule Rule)> _rules = new();

    /// <summary>
    /// Adds a validation rule with a property selector.
    /// </summary>
    /// <param name="selector">Function to extract the value from the model.</param>
    /// <param name="rule">The validation rule to apply.</param>
    /// <returns>This validator instance for chaining.</returns>
    public FluentValidator<T> RuleFor(Func<T, object?> selector, ValidationRule rule)
    {
        _rules.Add((selector, rule));
        return this;
    }

    /// <summary>
    /// Adds a required-field validation rule.
    /// </summary>
    /// <param name="selector">Function to extract the value from the model.</param>
    /// <param name="message">Optional custom error message.</param>
    /// <returns>This validator instance for chaining.</returns>
    public FluentValidator<T> Required(Func<T, object?> selector, string? message = null)
    {
        var rule = new RequiredRule();
        if (message is not null) rule.ErrorMessage = message;
        _rules.Add((selector, rule));
        return this;
    }

    /// <summary>
    /// Adds a minimum length validation rule.
    /// </summary>
    /// <param name="selector">Function to extract the string value from the model.</param>
    /// <param name="min">Minimum length.</param>
    /// <param name="message">Optional custom error message.</param>
    /// <returns>This validator instance for chaining.</returns>
    public FluentValidator<T> MinLength(Func<T, string?> selector, int min, string? message = null)
    {
        var rule = new MinLengthRule { MinLength = min };
        if (message is not null) rule.ErrorMessage = message;
        _rules.Add((m => selector(m), rule));
        return this;
    }

    /// <summary>
    /// Adds a maximum length validation rule.
    /// </summary>
    /// <param name="selector">Function to extract the string value from the model.</param>
    /// <param name="max">Maximum length.</param>
    /// <param name="message">Optional custom error message.</param>
    /// <returns>This validator instance for chaining.</returns>
    public FluentValidator<T> MaxLength(Func<T, string?> selector, int max, string? message = null)
    {
        var rule = new MaxLengthRule { MaxLength = max };
        if (message is not null) rule.ErrorMessage = message;
        _rules.Add((m => selector(m), rule));
        return this;
    }

    /// <summary>
    /// Adds a regex pattern matching validation rule.
    /// </summary>
    /// <param name="selector">Function to extract the string value from the model.</param>
    /// <param name="pattern">The regular expression pattern.</param>
    /// <param name="message">Optional custom error message.</param>
    /// <returns>This validator instance for chaining.</returns>
    public FluentValidator<T> Matches(Func<T, string?> selector, string pattern, string? message = null)
    {
        var rule = new RegexRule { Pattern = pattern };
        if (message is not null) rule.ErrorMessage = message;
        _rules.Add((m => selector(m), rule));
        return this;
    }

    /// <summary>
    /// Adds an email validation rule.
    /// </summary>
    /// <param name="selector">Function to extract the string value from the model.</param>
    /// <param name="message">Optional custom error message.</param>
    /// <returns>This validator instance for chaining.</returns>
    public FluentValidator<T> Email(Func<T, string?> selector, string? message = null)
    {
        var rule = new EmailRule();
        if (message is not null) rule.ErrorMessage = message;
        _rules.Add((m => selector(m), rule));
        return this;
    }

    /// <summary>
    /// Adds a range validation rule.
    /// </summary>
    /// <param name="selector">Function to extract the value from the model.</param>
    /// <param name="min">Minimum value.</param>
    /// <param name="max">Maximum value.</param>
    /// <param name="message">Optional custom error message.</param>
    /// <returns>This validator instance for chaining.</returns>
    public FluentValidator<T> InRange(Func<T, object?> selector, double min, double max, string? message = null)
    {
        var rule = new RangeRule { Min = min, Max = max };
        if (message is not null) rule.ErrorMessage = message;
        _rules.Add((selector, rule));
        return this;
    }

    /// <summary>
    /// Adds a custom predicate validation rule.
    /// </summary>
    /// <param name="predicate">A function that returns true when the model is valid.</param>
    /// <param name="message">The error message when the predicate returns false.</param>
    /// <returns>This validator instance for chaining.</returns>
    public FluentValidator<T> Must(Func<T, bool> predicate, string message)
    {
        var rule = new CustomRule
        {
            Validator = value => value is T model && predicate(model),
            ErrorMessage = message
        };
        _rules.Add((m => m!, rule));
        return this;
    }

    /// <summary>
    /// Validates the model against all registered rules.
    /// </summary>
    /// <param name="model">The model instance to validate.</param>
    /// <returns>A <see cref="FormValidationResult"/> containing all validation errors.</returns>
    public FormValidationResult Validate(T model)
    {
        var errors = new List<ValidationError>();

        foreach (var (selector, rule) in _rules)
        {
            var value = selector(model);
            var result = rule.Validate(value);
            if (!result.IsValid)
            {
                errors.Add(new ValidationError
                {
                    ErrorMessage = result.ErrorMessage,
                    Rule = rule
                });
            }
        }

        return new FormValidationResult
        {
            IsValid = errors.Count == 0,
            Errors = errors
        };
    }

    /// <summary>
    /// Asynchronously validates the model (runs synchronously, provided for API compatibility).
    /// </summary>
    /// <param name="model">The model instance to validate.</param>
    /// <returns>A task containing the <see cref="FormValidationResult"/>.</returns>
    public Task<FormValidationResult> ValidateAsync(T model)
    {
        return Task.FromResult(Validate(model));
    }
}
