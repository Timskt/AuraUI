# Validation Guide

AuraUI provides a comprehensive validation system with 10+ built-in rules, a `FormValidator` for imperative validation, a `FluentValidator<T>` for declarative validation, and `FormField` integration for displaying errors in the UI.

---

## Built-in Rules

| Rule | Class | Description | Properties |
|------|-------|-------------|------------|
| Required | `RequiredRule` | Value must not be null, empty, or whitespace | -- |
| MinLength | `MinLengthRule` | String must be at least N characters | `MinLength` |
| MaxLength | `MaxLengthRule` | String must be at most N characters | `MaxLength` |
| Email | `EmailRule` | Must be a valid email format | -- |
| Regex | `RegexRule` | Must match a regular expression pattern | `Pattern` |
| Range | `RangeRule` | Numeric value must be within a range | `Min`, `Max` |
| Compare | `CompareRule` | Two values must be equal (e.g., password confirm) | `OtherPropertyName` |
| Custom | `CustomRule` | User-defined predicate validation | `Validator` (Func<object?, bool>) |
| DataAnnotation | `DataAnnotationRule` | Uses `System.ComponentModel.DataAnnotations` | -- |

All rules inherit from `ValidationRule` and implement:

```csharp
public abstract ValidationResult Validate(object? value);
```

---

## FormValidator (Imperative)

Use `FormValidator` when you want to define rules imperatively and validate properties individually or all at once.

### Defining Rules

```csharp
var validator = new FormValidator();

validator.AddRule("Name", new RequiredRule());
validator.AddRule("Name", new MinLengthRule { MinLength = 2 });
validator.AddRule("Name", new MaxLengthRule { MaxLength = 50 });

validator.AddRule("Email", new RequiredRule());
validator.AddRule("Email", new EmailRule());

validator.AddRule("Age", new RangeRule { Min = 0, Max = 150 });

validator.AddRule("Password", new RequiredRule());
validator.AddRule("Password", new MinLengthRule { MinLength = 8 });
validator.AddRule("Password", new RegexRule
{
    Pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$",
    ErrorMessage = "Password must contain uppercase, lowercase, and digit"
});

validator.AddRule("ConfirmPassword", new CompareRule { OtherPropertyName = "Password" });
```

### Validating

```csharp
// Validate a single property
bool isNameValid = validator.ValidateProperty("Name", userName);

// Validate all properties (reads values via reflection)
bool isFormValid = validator.ValidateAll(model);

// Get errors
var errors = validator.GetErrors();
var nameErrors = validator.GetErrors("Name");

// Check for errors
if (validator.HasErrors) { /* show errors */ }
if (validator.HasErrorsFor("Email")) { /* highlight email field */ }

// Build a result object
FormValidationResult result = validator.ToResult();
```

### Listening for Changes

```csharp
validator.ValidationChanged += (s, e) =>
{
    if (e.IsValid)
    {
        // Clear error display for this property
    }
    else
    {
        // Show error messages
        foreach (var error in e.Errors)
        {
            Console.WriteLine($"{error.PropertyName}: {error.ErrorMessage}");
        }
    }
};
```

---

## FluentValidator<T> (Declarative)

Use `FluentValidator<T>` for a concise, chainable syntax when validating model objects.

### Defining a Validator

```csharp
var validator = new FluentValidator<UserModel>()
    .Required(u => u.Name, "Name is required")
    .MinLength(u => u.Name, 2, "Name must be at least 2 characters")
    .MaxLength(u => u.Name, 50)
    .Required(u => u.Email, "Email is required")
    .Email(u => u.Email, "Invalid email format")
    .InRange(u => u.Age, 0, 150, "Age must be between 0 and 150")
    .Required(u => u.Password, "Password is required")
    .MinLength(u => u.Password, 8, "Password must be at least 8 characters")
    .Matches(u => u.Password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$",
             "Password must contain uppercase, lowercase, and digit")
    .Must(u => u.Password == u.ConfirmPassword, "Passwords do not match");
```

### Validating

```csharp
var result = validator.Validate(user);

if (!result.IsValid)
{
    foreach (var error in result.Errors)
    {
        Console.WriteLine(error.ErrorMessage);
    }
}

// Async version (currently synchronous, provided for API compatibility)
var result = await validator.ValidateAsync(user);
```

---

## FormField Integration

The `FormField` control wraps content with a label, helper text, error display, and required-field indicator. It integrates with the validation system for automatic error display.

```xml
<layout:FormField Label="Email Address"
                  HelperText="We'll never share your email"
                  IsRequired="True"
                  ErrorText="{Binding EmailError}">
    <TextBox Text="{Binding Email, Mode=TwoWay}"
             Watermark="Enter your email"/>
</layout:FormField>
```

### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Label` | `string?` | Field label text |
| `HelperText` | `string?` | Help text shown below the field |
| `ErrorText` | `string?` | Error message (shown in error state) |
| `IsRequired` | `bool` | Shows a required-field indicator |
| `LabelPlacement` | `FormFieldLabelPlacement` | `Top` or `Left` |

### Pseudo-Classes

FormField uses pseudo-classes for styling:

- `:error` -- when `ErrorText` is set
- `:valid` -- when no error text is set
- `:required` -- when `IsRequired` is true
- `:has-helper` -- when `HelperText` is set
- `:has-error` -- when `ErrorText` is set

---

## Integrating with ViewModel

```csharp
public class RegistrationViewModel : ViewModelBase
{
    private readonly FormValidator _validator = new();
    private string _email = "";
    private string _emailError = "";

    public string Email
    {
        get => _email;
        set
        {
            if (SetProperty(ref _email, value))
            {
                ValidateEmail();
            }
        }
    }

    public string EmailError
    {
        get => _emailError;
        set => SetProperty(ref _emailError, value);
    }

    public RegistrationViewModel()
    {
        _validator.AddRule("Email", new RequiredRule());
        _validator.AddRule("Email", new EmailRule());
    }

    private void ValidateEmail()
    {
        var isValid = _validator.ValidateProperty("Email", Email);
        EmailError = isValid ? "" : _validator.GetErrors("Email").FirstOrDefault()?.ErrorMessage ?? "";
    }
}
```

```xml
<StackPanel Spacing="16">
    <layout:FormField Label="Email" IsRequired="True" ErrorText="{Binding EmailError}">
        <TextBox Text="{Binding Email, Mode=TwoWay}" Watermark="you@example.com"/>
    </layout:FormField>

    <input:AuraButton Content="Register" Command="{Binding RegisterCommand}"/>
</StackPanel>
```

---

## Custom Validation Rules

Create custom rules by extending `ValidationRule`:

```csharp
public class UsernameRule : ValidationRule
{
    public override ValidationResult Validate(object? value)
    {
        if (value is not string username || string.IsNullOrWhiteSpace(username))
            return ValidationResult.Fail("Username is required");

        if (username.Length < 3)
            return ValidationResult.Fail("Username must be at least 3 characters");

        if (!char.IsLetter(username[0]))
            return ValidationResult.Fail("Username must start with a letter");

        if (!username.All(c => char.IsLetterOrDigit(c) || c == '_'))
            return ValidationResult.Fail("Username can only contain letters, digits, and underscores");

        return ValidationResult.Success();
    }
}

// Usage
validator.AddRule("Username", new UsernameRule());
```

---

## Data Annotations Support

Use `DataAnnotationRule` to leverage `System.ComponentModel.DataAnnotations`:

```csharp
public class UserModel
{
    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string Name { get; set; } = "";

    [Required]
    [EmailAddress]
    public string Email { get; set; } = "";

    [Range(0, 150)]
    public int Age { get; set; }
}

// Usage with DataAnnotationRule
validator.AddRule("Name", new DataAnnotationRule());
validator.AddRule("Email", new DataAnnotationRule());
validator.AddRule("Age", new DataAnnotationRule());
```
