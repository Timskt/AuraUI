using AuraUI.Core.Validation;
using Xunit;

namespace AuraUI.Tests;

public class ValidationTests
{
    #region RequiredRule

    [Fact]
    public void RequiredRule_Null_Fails()
    {
        var rule = new RequiredRule();
        var result = rule.Validate(null);
        Assert.False(result.IsValid);
    }

    [Fact]
    public void RequiredRule_Empty_Fails()
    {
        var rule = new RequiredRule();
        var result = rule.Validate("");
        Assert.False(result.IsValid);
    }

    [Fact]
    public void RequiredRule_Whitespace_Fails()
    {
        var rule = new RequiredRule();
        var result = rule.Validate("   ");
        Assert.False(result.IsValid);
    }

    [Fact]
    public void RequiredRule_NonEmpty_Passes()
    {
        var rule = new RequiredRule();
        var result = rule.Validate("hello");
        Assert.True(result.IsValid);
    }

    [Fact]
    public void RequiredRule_NonStringValue_Passes()
    {
        var rule = new RequiredRule();
        var result = rule.Validate(42);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void RequiredRule_CustomErrorMessage()
    {
        var rule = new RequiredRule { ErrorMessage = "Name is required" };
        var result = rule.Validate(null);
        Assert.False(result.IsValid);
        Assert.Equal("Name is required", result.ErrorMessage);
    }

    [Fact]
    public void RequiredRule_DefaultErrorMessage()
    {
        var rule = new RequiredRule();
        var result = rule.Validate(null);
        Assert.Equal("This field is required", result.ErrorMessage);
    }

    #endregion

    #region MinLengthRule

    [Fact]
    public void MinLength_BelowMin_Fails()
    {
        var rule = new MinLengthRule { MinLength = 3 };
        var result = rule.Validate("ab");
        Assert.False(result.IsValid);
    }

    [Fact]
    public void MinLength_AtMin_Passes()
    {
        var rule = new MinLengthRule { MinLength = 3 };
        var result = rule.Validate("abc");
        Assert.True(result.IsValid);
    }

    [Fact]
    public void MinLength_AboveMin_Passes()
    {
        var rule = new MinLengthRule { MinLength = 3 };
        var result = rule.Validate("abcdef");
        Assert.True(result.IsValid);
    }

    [Fact]
    public void MinLength_Null_Passes()
    {
        // MinLengthRule treats null as valid (use RequiredRule for presence)
        var rule = new MinLengthRule { MinLength = 3 };
        var result = rule.Validate(null);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void MinLength_NonString_Passes()
    {
        var rule = new MinLengthRule { MinLength = 3 };
        var result = rule.Validate(42);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void MinLength_CustomErrorMessage()
    {
        var rule = new MinLengthRule { MinLength = 5, ErrorMessage = "Too short" };
        var result = rule.Validate("hi");
        Assert.False(result.IsValid);
        Assert.Equal("Too short", result.ErrorMessage);
    }

    #endregion

    #region MaxLengthRule

    [Fact]
    public void MaxLength_AboveMax_Fails()
    {
        var rule = new MaxLengthRule { MaxLength = 3 };
        var result = rule.Validate("abcd");
        Assert.False(result.IsValid);
    }

    [Fact]
    public void MaxLength_AtMax_Passes()
    {
        var rule = new MaxLengthRule { MaxLength = 3 };
        var result = rule.Validate("abc");
        Assert.True(result.IsValid);
    }

    [Fact]
    public void MaxLength_BelowMax_Passes()
    {
        var rule = new MaxLengthRule { MaxLength = 3 };
        var result = rule.Validate("ab");
        Assert.True(result.IsValid);
    }

    [Fact]
    public void MaxLength_Null_Passes()
    {
        var rule = new MaxLengthRule { MaxLength = 3 };
        var result = rule.Validate(null);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void MaxLength_NonString_Passes()
    {
        var rule = new MaxLengthRule { MaxLength = 3 };
        var result = rule.Validate(12345);
        Assert.True(result.IsValid);
    }

    #endregion

    #region RegexRule

    [Fact]
    public void Regex_MatchingPattern_Passes()
    {
        var rule = new RegexRule { Pattern = @"^\d+$" };
        var result = rule.Validate("12345");
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Regex_NonMatchingPattern_Fails()
    {
        var rule = new RegexRule { Pattern = @"^\d+$" };
        var result = rule.Validate("abc");
        Assert.False(result.IsValid);
    }

    [Fact]
    public void Regex_EmailPattern()
    {
        var rule = new RegexRule { Pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$" };
        Assert.True(rule.Validate("user@example.com").IsValid);
        Assert.False(rule.Validate("invalid").IsValid);
        Assert.False(rule.Validate("user@").IsValid);
    }

    [Fact]
    public void Regex_Null_Passes()
    {
        var rule = new RegexRule { Pattern = @"^\d+$" };
        var result = rule.Validate(null);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Regex_NonString_Passes()
    {
        var rule = new RegexRule { Pattern = @"^\d+$" };
        var result = rule.Validate(42);
        Assert.True(result.IsValid);
    }

    #endregion

    #region RangeRule

    [Fact]
    public void Range_InRange_Passes()
    {
        var rule = new RangeRule { Min = 1, Max = 10 };
        var result = rule.Validate(5);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Range_AtMin_Passes()
    {
        var rule = new RangeRule { Min = 1, Max = 10 };
        var result = rule.Validate(1);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Range_AtMax_Passes()
    {
        var rule = new RangeRule { Min = 1, Max = 10 };
        var result = rule.Validate(10);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Range_BelowMin_Fails()
    {
        var rule = new RangeRule { Min = 1, Max = 10 };
        var result = rule.Validate(0);
        Assert.False(result.IsValid);
    }

    [Fact]
    public void Range_AboveMax_Fails()
    {
        var rule = new RangeRule { Min = 1, Max = 10 };
        var result = rule.Validate(11);
        Assert.False(result.IsValid);
    }

    [Fact]
    public void Range_Null_Passes()
    {
        var rule = new RangeRule { Min = 1, Max = 10 };
        var result = rule.Validate(null);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Range_DoubleValues()
    {
        var rule = new RangeRule { Min = 0.0, Max = 1.0 };
        Assert.True(rule.Validate(0.5).IsValid);
        Assert.False(rule.Validate(1.5).IsValid);
    }

    [Fact]
    public void Range_StringNumeric_FallsBackToParse()
    {
        var rule = new RangeRule { Min = 1, Max = 10 };
        // strings implement IComparable against double will throw, falls back to parse
        var result = rule.Validate("5");
        Assert.True(result.IsValid);
    }

    #endregion

    #region EmailRule

    [Fact]
    public void Email_ValidEmail_Passes()
    {
        var rule = new EmailRule();
        Assert.True(rule.Validate("user@example.com").IsValid);
    }

    [Fact]
    public void Email_WithSubdomain_Passes()
    {
        var rule = new EmailRule();
        Assert.True(rule.Validate("user@mail.example.com").IsValid);
    }

    [Fact]
    public void Email_WithPlus_Passes()
    {
        var rule = new EmailRule();
        Assert.True(rule.Validate("user+tag@example.com").IsValid);
    }

    [Fact]
    public void Email_Invalid_NoDomain_Fails()
    {
        var rule = new EmailRule();
        Assert.False(rule.Validate("user@").IsValid);
    }

    [Fact]
    public void Email_Invalid_NoAt_Fails()
    {
        var rule = new EmailRule();
        Assert.False(rule.Validate("userexample.com").IsValid);
    }

    [Fact]
    public void Email_Invalid_Empty_Fails()
    {
        var rule = new EmailRule();
        Assert.False(rule.Validate("").IsValid);
    }

    [Fact]
    public void Email_Invalid_JustAt_Fails()
    {
        var rule = new EmailRule();
        Assert.False(rule.Validate("@").IsValid);
    }

    [Fact]
    public void Email_Null_Passes()
    {
        // Inherits from RegexRule, null passes
        var rule = new EmailRule();
        Assert.True(rule.Validate(null).IsValid);
    }

    #endregion

    #region CompareRule

    [Fact]
    public void Compare_Matching_Passes()
    {
        var rule = new CompareRule
        {
            OtherValueGetter = () => "password123"
        };
        var result = rule.Validate("password123");
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Compare_NotMatching_Fails()
    {
        var rule = new CompareRule
        {
            OtherValueGetter = () => "password123"
        };
        var result = rule.Validate("different");
        Assert.False(result.IsValid);
    }

    [Fact]
    public void Compare_BothNull_Passes()
    {
        var rule = new CompareRule
        {
            OtherValueGetter = () => null
        };
        var result = rule.Validate(null);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Compare_NullVsValue_Fails()
    {
        var rule = new CompareRule
        {
            OtherValueGetter = () => "value"
        };
        var result = rule.Validate(null);
        Assert.False(result.IsValid);
    }

    [Fact]
    public void Compare_CustomErrorMessage()
    {
        var rule = new CompareRule
        {
            OtherValueGetter = () => "abc",
            ErrorMessage = "Passwords must match"
        };
        var result = rule.Validate("xyz");
        Assert.False(result.IsValid);
        Assert.Equal("Passwords must match", result.ErrorMessage);
    }

    #endregion

    #region CustomRule

    [Fact]
    public void CustomRule_PredicateTrue_Passes()
    {
        var rule = new CustomRule
        {
            Validator = value => value is int i && i > 0
        };
        Assert.True(rule.Validate(5).IsValid);
    }

    [Fact]
    public void CustomRule_PredicateFalse_Fails()
    {
        var rule = new CustomRule
        {
            Validator = value => value is int i && i > 0
        };
        Assert.False(rule.Validate(-1).IsValid);
    }

    [Fact]
    public void CustomRule_NullValidator_Passes()
    {
        var rule = new CustomRule();
        Assert.True(rule.Validate("anything").IsValid);
    }

    #endregion

    #region ValidationResult

    [Fact]
    public void ValidationResult_Success_IsValid()
    {
        var result = ValidationResult.Success();
        Assert.True(result.IsValid);
        Assert.Equal(string.Empty, result.ErrorMessage);
    }

    [Fact]
    public void ValidationResult_Fail_IsNotValid()
    {
        var result = ValidationResult.Fail("Error message");
        Assert.False(result.IsValid);
        Assert.Equal("Error message", result.ErrorMessage);
    }

    #endregion

    #region FormValidator

    [Fact]
    public void FormValidator_AddRule_ValidProperty()
    {
        var validator = new FormValidator();
        validator.AddRule("Name", new RequiredRule());
        Assert.True(validator.ValidateProperty("Name", "hello"));
    }

    [Fact]
    public void FormValidator_AddRule_InvalidProperty()
    {
        var validator = new FormValidator();
        validator.AddRule("Name", new RequiredRule());
        Assert.False(validator.ValidateProperty("Name", null));
    }

    [Fact]
    public void FormValidator_MultipleRules_AllPass()
    {
        var validator = new FormValidator();
        validator.AddRule("Name", new RequiredRule());
        validator.AddRule("Name", new MinLengthRule { MinLength = 2 });
        Assert.True(validator.ValidateProperty("Name", "hello"));
    }

    [Fact]
    public void FormValidator_MultipleRules_OneFails()
    {
        var validator = new FormValidator();
        validator.AddRule("Name", new RequiredRule());
        validator.AddRule("Name", new MinLengthRule { MinLength = 10 });
        Assert.False(validator.ValidateProperty("Name", "hi"));
    }

    [Fact]
    public void FormValidator_ValidateAll_ReflectsModel()
    {
        var validator = new FormValidator();
        validator.AddRule("Name", new RequiredRule());
        validator.AddRule("Email", new RequiredRule());

        var model = new TestModel { Name = "John", Email = "" };
        Assert.False(validator.ValidateAll(model));
    }

    [Fact]
    public void FormValidator_ValidateAll_AllValid()
    {
        var validator = new FormValidator();
        validator.AddRule("Name", new RequiredRule());
        validator.AddRule("Email", new RequiredRule());

        var model = new TestModel { Name = "John", Email = "john@example.com" };
        Assert.True(validator.ValidateAll(model));
    }

    [Fact]
    public void FormValidator_HasErrors_TrueWhenInvalid()
    {
        var validator = new FormValidator();
        validator.AddRule("Name", new RequiredRule());
        validator.ValidateProperty("Name", null);
        Assert.True(validator.HasErrors);
    }

    [Fact]
    public void FormValidator_HasErrors_FalseWhenValid()
    {
        var validator = new FormValidator();
        validator.AddRule("Name", new RequiredRule());
        validator.ValidateProperty("Name", "hello");
        Assert.False(validator.HasErrors);
    }

    [Fact]
    public void FormValidator_GetErrors_ReturnsCorrectErrors()
    {
        var validator = new FormValidator();
        validator.AddRule("Name", new RequiredRule());
        validator.ValidateProperty("Name", null);
        var errors = validator.GetErrors("Name");
        Assert.Single(errors);
    }

    [Fact]
    public void FormValidator_ClearErrors()
    {
        var validator = new FormValidator();
        validator.AddRule("Name", new RequiredRule());
        validator.ValidateProperty("Name", null);
        validator.ClearErrors();
        Assert.False(validator.HasErrors);
    }

    [Fact]
    public void FormValidator_ClearErrorsForProperty()
    {
        var validator = new FormValidator();
        validator.AddRule("Name", new RequiredRule());
        validator.ValidateProperty("Name", null);
        validator.ClearErrors("Name");
        Assert.False(validator.HasErrorsFor("Name"));
    }

    [Fact]
    public void FormValidator_NoRulesForProperty_ReturnsTrue()
    {
        var validator = new FormValidator();
        Assert.True(validator.ValidateProperty("Unknown", null));
    }

    [Fact]
    public void FormValidator_ValidationChanged_Fires()
    {
        var validator = new FormValidator();
        validator.AddRule("Name", new RequiredRule());

        ValidationChangedEventArgs? capturedArgs = null;
        validator.ValidationChanged += (_, args) => capturedArgs = args;

        validator.ValidateProperty("Name", null);

        Assert.NotNull(capturedArgs);
        Assert.Equal("Name", capturedArgs!.PropertyName);
        Assert.False(capturedArgs.IsValid);
    }

    [Fact]
    public void FormValidator_ToResult()
    {
        var validator = new FormValidator();
        validator.AddRule("Name", new RequiredRule());
        validator.ValidateProperty("Name", null);

        var result = validator.ToResult();
        Assert.False(result.IsValid);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public void FormValidator_AddRules_Batch()
    {
        var validator = new FormValidator();
        validator.AddRules("Name", new ValidationRule[]
        {
            new RequiredRule(),
            new MinLengthRule { MinLength = 2 },
            new MaxLengthRule { MaxLength = 50 }
        });

        Assert.True(validator.ValidateProperty("Name", "John"));
        Assert.False(validator.ValidateProperty("Name", "J"));
    }

    private class TestModel
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public int Age { get; set; }
    }

    #endregion

    #region FluentValidator

    [Fact]
    public void FluentValidator_Required_Passes()
    {
        var validator = new FluentValidator<TestModel>()
            .Required(m => m.Name!);

        var result = validator.Validate(new TestModel { Name = "John" });
        Assert.True(result.IsValid);
    }

    [Fact]
    public void FluentValidator_Required_Fails()
    {
        var validator = new FluentValidator<TestModel>()
            .Required(m => m.Name!);

        var result = validator.Validate(new TestModel { Name = null });
        Assert.False(result.IsValid);
    }

    [Fact]
    public void FluentValidator_MinLength()
    {
        var validator = new FluentValidator<TestModel>()
            .MinLength(m => m.Name, 3);

        Assert.True(validator.Validate(new TestModel { Name = "John" }).IsValid);
        Assert.False(validator.Validate(new TestModel { Name = "Jo" }).IsValid);
    }

    [Fact]
    public void FluentValidator_MaxLength()
    {
        var validator = new FluentValidator<TestModel>()
            .MaxLength(m => m.Name, 5);

        Assert.True(validator.Validate(new TestModel { Name = "John" }).IsValid);
        Assert.False(validator.Validate(new TestModel { Name = "TooLongName" }).IsValid);
    }

    [Fact]
    public void FluentValidator_Matches()
    {
        var validator = new FluentValidator<TestModel>()
            .Matches(m => m.Name, @"^[A-Z]");

        Assert.True(validator.Validate(new TestModel { Name = "John" }).IsValid);
        Assert.False(validator.Validate(new TestModel { Name = "john" }).IsValid);
    }

    [Fact]
    public void FluentValidator_Email()
    {
        var validator = new FluentValidator<TestModel>()
            .Email(m => m.Email);

        Assert.True(validator.Validate(new TestModel { Email = "user@example.com" }).IsValid);
        Assert.False(validator.Validate(new TestModel { Email = "invalid" }).IsValid);
    }

    [Fact]
    public void FluentValidator_InRange()
    {
        var validator = new FluentValidator<TestModel>()
            .InRange(m => m.Age, 18, 120);

        Assert.True(validator.Validate(new TestModel { Age = 25 }).IsValid);
        Assert.False(validator.Validate(new TestModel { Age = 10 }).IsValid);
        Assert.False(validator.Validate(new TestModel { Age = 150 }).IsValid);
    }

    [Fact]
    public void FluentValidator_Must()
    {
        var validator = new FluentValidator<TestModel>()
            .Must(m => m.Name?.Length > 0, "Name cannot be empty");

        Assert.True(validator.Validate(new TestModel { Name = "John" }).IsValid);
        Assert.False(validator.Validate(new TestModel { Name = "" }).IsValid);
    }

    [Fact]
    public void FluentValidator_Chaining()
    {
        var validator = new FluentValidator<TestModel>()
            .Required(m => m.Name!)
            .MinLength(m => m.Name, 2)
            .MaxLength(m => m.Name, 50)
            .Email(m => m.Email);

        var result = validator.Validate(new TestModel { Name = "John", Email = "john@example.com" });
        Assert.True(result.IsValid);
    }

    [Fact]
    public void FluentValidator_MultipleFailures()
    {
        var validator = new FluentValidator<TestModel>()
            .Required(m => m.Name!)
            .MinLength(m => m.Name, 10, "Too short");

        var result = validator.Validate(new TestModel { Name = null });
        // Required fails, MinLength also fails on null (null string has 0 < 10 length... but null is not string)
        // Actually null is not string, so MinLengthRule passes for null
        Assert.False(result.IsValid);
        Assert.True(result.Errors.Count >= 1);
    }

    [Fact]
    public void FluentValidator_CustomMessage()
    {
        var validator = new FluentValidator<TestModel>()
            .Required(m => m.Name!, "Name is required!");

        var result = validator.Validate(new TestModel { Name = null });
        Assert.Contains("Name is required!", result.Errors.Select(e => e.ErrorMessage));
    }

    [Fact]
    public void FluentValidator_ValidateAsync()
    {
        var validator = new FluentValidator<TestModel>()
            .Required(m => m.Name!);

        var result = validator.ValidateAsync(new TestModel { Name = "John" }).Result;
        Assert.True(result.IsValid);
    }

    [Fact]
    public void FluentValidator_EmptyModel()
    {
        var validator = new FluentValidator<TestModel>();
        var result = validator.Validate(new TestModel());
        Assert.True(result.IsValid);
    }

    #endregion

    #region FormValidationResult

    [Fact]
    public void FormValidationResult_Success()
    {
        var result = FormValidationResult.Success();
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void FormValidationResult_Fail()
    {
        var errors = new List<ValidationError>
        {
            new() { PropertyName = "Name", ErrorMessage = "Required" }
        };
        var result = FormValidationResult.Fail(errors);
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
    }

    [Fact]
    public void FormValidationResult_ErrorsByProperty()
    {
        var result = new FormValidationResult
        {
            Errors = new List<ValidationError>
            {
                new() { PropertyName = "Name", ErrorMessage = "Required" },
                new() { PropertyName = "Email", ErrorMessage = "Invalid" },
                new() { PropertyName = "Name", ErrorMessage = "Too short" }
            }
        };

        var grouped = result.ErrorsByProperty;
        Assert.Equal(2, grouped.Count);
        Assert.Equal(2, grouped["Name"].Count);
        Assert.Single(grouped["Email"]);
    }

    #endregion
}
