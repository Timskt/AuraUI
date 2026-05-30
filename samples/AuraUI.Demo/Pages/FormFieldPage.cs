using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using AuraUI.Controls.Layout;
using AuraUI.Controls.Input;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class FormFieldPage : ComponentPageBase
{
    public override string ComponentName => "FormField";
    public override string Description => "A form field wrapper with label, helper text, error display, validation rules, and required-field indicators. Supports horizontal and vertical label placement.";
    public override string Category => "Layout";

    protected override Control BuildContent()
    {
        return new StackPanel
        {
            Spacing = 32,
            Children =
            {
                BuildBasicExample(),
                BuildPlacementExample(),
                CreateApiTable(GetApiProperties()),
                CreateGuidelines(
                    "Use FormField to wrap form inputs with labels and validation feedback. Essential for any form with multiple fields.",
                    "Always provide a label. Use helper text for format hints. Show error messages inline. Mark required fields with the IsRequired property.")
            }
        };
    }

    private Control BuildBasicExample()
    {
        var nameField = new FormField
        {
            Label = "Full Name",
            HelperText = "Enter your first and last name",
            IsRequired = true,
            Width = 350,
            Margin = new Thickness(0, 0, 0, 12),
            Content = new AuraTextBox { Watermark = "John Doe" }
        };

        var emailField = new FormField
        {
            Label = "Email",
            HelperText = "We'll never share your email",
            HasError = true,
            ErrorText = "Please enter a valid email address",
            Width = 350,
            Margin = new Thickness(0, 0, 0, 12),
            Content = new AuraTextBox { Watermark = "user@example.com" }
        };

        var panel = new StackPanel { Spacing = 8 };
        panel.Children.Add(nameField);
        panel.Children.Add(emailField);

        return CreateExampleSection("Basic Form Fields", panel,
            @"<layout:FormField Label=""Full Name"" IsRequired=""True""
    HelperText=""Enter your first and last name"">
    <TextBox Watermark=""John Doe""/>
</layout:FormField>

<layout:FormField Label=""Email"" HasError=""True""
    ErrorText=""Please enter a valid email address"">
    <TextBox Watermark=""user@example.com""/>
</layout:FormField>",
            @"// Create form fields in C#
var nameField = new FormField
{
    Label = ""Full Name"",
    HelperText = ""Enter your first and last name"",
    IsRequired = true,
    Content = new TextBox { Watermark = ""John Doe"" }
};

// Set validation error
var emailField = new FormField
{
    Label = ""Email"",
    HasError = true,
    ErrorText = ""Please enter a valid email address"",
    Content = new TextBox { Watermark = ""user@example.com"" }
};",
            @"public partial class FormViewModel : ViewModelBase
{
    private string _fullName = """";
    public string FullName
    {
        get => _fullName;
        set
        {
            SetProperty(ref _fullName, value);
            ValidateName();
        }
    }

    private string _email = """";
    public string Email
    {
        get => _email;
        set
        {
            SetProperty(ref _email, value);
            ValidateEmail();
        }
    }

    private bool _hasEmailError;
    public bool HasEmailError
    {
        get => _hasEmailError;
        set => SetProperty(ref _hasEmailError, value);
    }

    private void ValidateEmail()
    {
        HasEmailError = !Email.Contains(""@"");
    }
}");
    }

    private Control BuildPlacementExample()
    {
        var topField = new FormField
        {
            Label = "Label on Top",
            LabelPlacement = FormFieldLabelPlacement.Top,
            Width = 350,
            Margin = new Thickness(0, 0, 0, 12),
            Content = new AuraTextBox { Watermark = "Top label placement" }
        };

        var leftField = new FormField
        {
            Label = "Label on Left",
            LabelPlacement = FormFieldLabelPlacement.Left,
            LabelWidth = 120,
            Width = 400,
            Margin = new Thickness(0, 0, 0, 12),
            Content = new AuraTextBox { Watermark = "Left label placement" }
        };

        var panel = new StackPanel { Spacing = 8 };
        panel.Children.Add(topField);
        panel.Children.Add(leftField);

        return CreateExampleSection("Label Placement", panel,
            @"<layout:FormField Label=""Label on Top""
    LabelPlacement=""Top"">
    <TextBox Watermark=""Top label placement""/>
</layout:FormField>

<layout:FormField Label=""Label on Left""
    LabelPlacement=""Left"" LabelWidth=""120"">
    <TextBox Watermark=""Left label placement""/>
</layout:FormField>",
            @"// Set label placement in code
var field = new FormField
{
    Label = ""Label on Left"",
    LabelPlacement = FormFieldLabelPlacement.Left,
    LabelWidth = 120,
    Content = new TextBox { Watermark = ""Left label"" }
};");
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Label", Type = "string", Default = "null", Description = "Label text for the field" },
        new ApiProperty { PropertyName = "HelperText", Type = "string", Default = "null", Description = "Helper text below the field" },
        new ApiProperty { PropertyName = "ErrorText", Type = "string", Default = "null", Description = "Error message when validation fails" },
        new ApiProperty { PropertyName = "IsRequired", Type = "bool", Default = "false", Description = "Shows required indicator" },
        new ApiProperty { PropertyName = "HasError", Type = "bool", Default = "false", Description = "Whether the field has a validation error" },
        new ApiProperty { PropertyName = "IsValid", Type = "bool", Default = "true", Description = "Whether the field passes validation" },
        new ApiProperty { PropertyName = "LabelPlacement", Type = "FormFieldLabelPlacement", Default = "Top", Description = "Label position: Top or Left" },
        new ApiProperty { PropertyName = "LabelWidth", Type = "double", Default = "120", Description = "Width of label area when Left placement" },
        new ApiProperty { PropertyName = "ValidateOnLostFocus", Type = "bool", Default = "true", Description = "Auto-validate when field loses focus" },
    };
}
