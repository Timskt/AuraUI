using System.Linq.Expressions;
using System.Reflection;

namespace AuraUI.Core.Validation;

/// <summary>
/// Fluent API entry point for building form state with validation rules.
/// </summary>
/// <example>
/// <code>
/// var form = FormBuilder.Create&lt;MyModel&gt;()
///     .Field(m => m.Name, rules => rules.Required().MinLength(2).MaxLength(50))
///     .Field(m => m.Email, rules => rules.Required().Email())
///     .Field(m => m.Age, rules => rules.Range(0, 150))
///     .Build();
/// </code>
/// </example>
public static class FormBuilder
{
    /// <summary>
    /// Creates a new <see cref="FormBuilder{TModel}"/> for the specified model type.
    /// </summary>
    /// <typeparam name="TModel">The model type.</typeparam>
    /// <returns>A new form builder.</returns>
    public static FormBuilder<TModel> Create<TModel>() where TModel : class, new()
    {
        return new FormBuilder<TModel>();
    }

    /// <summary>
    /// Creates a new <see cref="FormBuilder{TModel}"/> with an existing model instance.
    /// </summary>
    /// <typeparam name="TModel">The model type.</typeparam>
    /// <param name="model">The model instance.</param>
    /// <returns>A new form builder.</returns>
    public static FormBuilder<TModel> Create<TModel>(TModel model) where TModel : class, new()
    {
        return new FormBuilder<TModel>(model);
    }
}

/// <summary>
/// Fluent builder for configuring form fields and their validation rules.
/// </summary>
/// <typeparam name="TModel">The model type.</typeparam>
public class FormBuilder<TModel> where TModel : class, new()
{
    private readonly TModel _model;
    private readonly FormValidator _validator = new();
    private readonly List<string> _requiredProperties = new();
    private Func<TModel, CancellationToken, Task<bool>>? _submitHandler;

    internal FormBuilder()
    {
        _model = new TModel();
    }

    internal FormBuilder(TModel model)
    {
        _model = model ?? throw new ArgumentNullException(nameof(model));
    }

    /// <summary>
    /// Configures a field with validation rules.
    /// </summary>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="propertySelector">Expression selecting the property.</param>
    /// <param name="rules">Action to configure validation rules using the fluent rule builder.</param>
    /// <returns>This builder for chaining.</returns>
    public FormBuilder<TModel> Field<TProperty>(
        Expression<Func<TModel, TProperty>> propertySelector,
        Action<FieldRuleBuilder<TModel, TProperty>> rules)
    {
        ArgumentNullException.ThrowIfNull(propertySelector);
        ArgumentNullException.ThrowIfNull(rules);

        var propertyName = GetPropertyName(propertySelector);
        var ruleBuilder = new FieldRuleBuilder<TModel, TProperty>(propertyName, _validator);
        rules(ruleBuilder);

        if (ruleBuilder.IsRequired)
            _requiredProperties.Add(propertyName);

        return this;
    }

    /// <summary>
    /// Configures a field with explicit validation rules (raw rules list).
    /// </summary>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="propertySelector">Expression selecting the property.</param>
    /// <param name="rules">The validation rules to apply.</param>
    /// <returns>This builder for chaining.</returns>
    public FormBuilder<TModel> Field<TProperty>(
        Expression<Func<TModel, TProperty>> propertySelector,
        params ValidationRule[] rules)
    {
        var propertyName = GetPropertyName(propertySelector);
        _validator.AddRules(propertyName, rules);
        return this;
    }

    /// <summary>
    /// Sets the async submit handler for the form.
    /// </summary>
    /// <param name="handler">A function that processes the model on submit; returns true on success.</param>
    /// <returns>This builder for chaining.</returns>
    public FormBuilder<TModel> OnSubmit(Func<TModel, CancellationToken, Task<bool>> handler)
    {
        _submitHandler = handler ?? throw new ArgumentNullException(nameof(handler));
        return this;
    }

    /// <summary>
    /// Sets the async submit handler (without cancellation token).
    /// </summary>
    /// <param name="handler">A function that processes the model; returns true on success.</param>
    /// <returns>This builder for chaining.</returns>
    public FormBuilder<TModel> OnSubmit(Func<TModel, Task<bool>> handler)
    {
        ArgumentNullException.ThrowIfNull(handler);
        _submitHandler = (model, _) => handler(model);
        return this;
    }

    /// <summary>
    /// Sets a synchronous submit handler.
    /// </summary>
    /// <param name="handler">A function that processes the model; returns true on success.</param>
    /// <returns>This builder for chaining.</returns>
    public FormBuilder<TModel> OnSubmit(Func<TModel, bool> handler)
    {
        ArgumentNullException.ThrowIfNull(handler);
        _submitHandler = (model, _) => Task.FromResult(handler(model));
        return this;
    }

    /// <summary>
    /// Builds the <see cref="FormState{TModel}"/> with all configured rules.
    /// </summary>
    /// <returns>A fully configured form state instance.</returns>
    public FormState<TModel> Build()
    {
        return new FormState<TModel>(_model, _validator, _submitHandler);
    }

    private static string GetPropertyName<TProperty>(Expression<Func<TModel, TProperty>> expression)
    {
        return expression.Body switch
        {
            MemberExpression member => member.Member.Name,
            UnaryExpression { Operand: MemberExpression innerMember } => innerMember.Member.Name,
            _ => throw new ArgumentException("Expression must be a property access.", nameof(expression))
        };
    }
}

/// <summary>
/// Fluent builder for configuring validation rules on a single field.
/// </summary>
/// <typeparam name="TModel">The model type.</typeparam>
/// <typeparam name="TProperty">The property type.</typeparam>
public class FieldRuleBuilder<TModel, TProperty> where TModel : class
{
    private readonly string _propertyName;
    private readonly FormValidator _validator;

    /// <summary>
    /// Gets whether a Required rule has been added.
    /// </summary>
    internal bool IsRequired { get; private set; }

    internal FieldRuleBuilder(string propertyName, FormValidator validator)
    {
        _propertyName = propertyName;
        _validator = validator;
    }

    /// <summary>
    /// Adds a required (not null, not empty, not whitespace) rule.
    /// </summary>
    /// <param name="message">Optional custom error message.</param>
    /// <returns>This builder for chaining.</returns>
    public FieldRuleBuilder<TModel, TProperty> Required(string? message = null)
    {
        var rule = new RequiredRule();
        if (message is not null) rule.ErrorMessage = message;
        _validator.AddRule(_propertyName, rule);
        IsRequired = true;
        return this;
    }

    /// <summary>
    /// Adds a minimum length rule (for string properties).
    /// </summary>
    /// <param name="min">The minimum length.</param>
    /// <param name="message">Optional custom error message.</param>
    /// <returns>This builder for chaining.</returns>
    public FieldRuleBuilder<TModel, TProperty> MinLength(int min, string? message = null)
    {
        var rule = new MinLengthRule { MinLength = min };
        if (message is not null) rule.ErrorMessage = message;
        _validator.AddRule(_propertyName, rule);
        return this;
    }

    /// <summary>
    /// Adds a maximum length rule (for string properties).
    /// </summary>
    /// <param name="max">The maximum length.</param>
    /// <param name="message">Optional custom error message.</param>
    /// <returns>This builder for chaining.</returns>
    public FieldRuleBuilder<TModel, TProperty> MaxLength(int max, string? message = null)
    {
        var rule = new MaxLengthRule { MaxLength = max };
        if (message is not null) rule.ErrorMessage = message;
        _validator.AddRule(_propertyName, rule);
        return this;
    }

    /// <summary>
    /// Adds a range validation rule (for numeric properties).
    /// </summary>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    /// <param name="message">Optional custom error message.</param>
    /// <returns>This builder for chaining.</returns>
    public FieldRuleBuilder<TModel, TProperty> Range(double min, double max, string? message = null)
    {
        var rule = new RangeRule { Min = min, Max = max };
        if (message is not null) rule.ErrorMessage = message;
        _validator.AddRule(_propertyName, rule);
        return this;
    }

    /// <summary>
    /// Adds an email validation rule.
    /// </summary>
    /// <param name="message">Optional custom error message.</param>
    /// <returns>This builder for chaining.</returns>
    public FieldRuleBuilder<TModel, TProperty> Email(string? message = null)
    {
        var rule = new EmailRule();
        if (message is not null) rule.ErrorMessage = message;
        _validator.AddRule(_propertyName, rule);
        return this;
    }

    /// <summary>
    /// Adds a regex pattern matching rule.
    /// </summary>
    /// <param name="pattern">The regex pattern.</param>
    /// <param name="message">Optional custom error message.</param>
    /// <returns>This builder for chaining.</returns>
    public FieldRuleBuilder<TModel, TProperty> Matches(string pattern, string? message = null)
    {
        var rule = new RegexRule { Pattern = pattern };
        if (message is not null) rule.ErrorMessage = message;
        _validator.AddRule(_propertyName, rule);
        return this;
    }

    /// <summary>
    /// Adds a custom predicate validation rule.
    /// </summary>
    /// <param name="predicate">A function that returns true when the value is valid.</param>
    /// <param name="message">The error message when the predicate returns false.</param>
    /// <returns>This builder for chaining.</returns>
    public FieldRuleBuilder<TModel, TProperty> Must(Func<TProperty?, bool> predicate, string message)
    {
        var rule = new CustomRule
        {
            Validator = value => value is TProperty typed ? predicate(typed) : predicate(default),
            ErrorMessage = message
        };
        _validator.AddRule(_propertyName, rule);
        return this;
    }

    /// <summary>
    /// Adds a compare rule to validate this field matches another property's value.
    /// </summary>
    /// <param name="otherPropertySelector">Expression selecting the other property.</param>
    /// <param name="message">Optional custom error message.</param>
    /// <returns>This builder for chaining.</returns>
    public FieldRuleBuilder<TModel, TProperty> CompareTo(
        Expression<Func<TModel, TProperty>> otherPropertySelector,
        string? message = null)
    {
        var otherPropName = GetOtherPropertyName(otherPropertySelector);
        var rule = new CompareRule
        {
            OtherPropertyName = otherPropName,
            OtherValueGetter = () =>
            {
                var prop = typeof(TModel).GetProperty(otherPropName, BindingFlags.Public | BindingFlags.Instance);
                // We don't have a model instance here, so we use the rule differently
                return null;
            }
        };
        if (message is not null) rule.ErrorMessage = message;
        _validator.AddRule(_propertyName, rule);
        return this;
    }

    /// <summary>
    /// Adds a DataAnnotation-style validation attribute as a rule.
    /// </summary>
    /// <param name="attribute">The validation attribute.</param>
    /// <param name="message">Optional custom error message.</param>
    /// <returns>This builder for chaining.</returns>
    public FieldRuleBuilder<TModel, TProperty> Attribute(
        System.ComponentModel.DataAnnotations.ValidationAttribute attribute,
        string? message = null)
    {
        var rule = new DataAnnotationRule { Attribute = attribute };
        if (message is not null) rule.ErrorMessage = message;
        _validator.AddRule(_propertyName, rule);
        return this;
    }

    private static string GetOtherPropertyName(Expression<Func<TModel, TProperty>> expression)
    {
        return expression.Body switch
        {
            MemberExpression member => member.Member.Name,
            UnaryExpression { Operand: MemberExpression innerMember } => innerMember.Member.Name,
            _ => throw new ArgumentException("Expression must be a property access.", nameof(expression))
        };
    }
}
