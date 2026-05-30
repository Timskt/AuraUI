using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class TextBoxPage : ComponentPageBase
{
    public override string ComponentName => "TextBox";
    public override string Description => "A text input control with watermark placeholder, multi-line support, and AuraUI styling.";
    public override string Category => "Input";

    protected override Control BuildContent()
    {
        return new StackPanel
        {
            Spacing = 32,
            Children =
            {
                BuildBasicExample(),
                BuildStylesExample(),
                BuildStatesExample(),
                CreateApiTable(GetApiProperties()),
                CreateGuidelines(
                    "Use text boxes for free-form text input such as names, addresses, and descriptions. Use watermark text to hint at the expected input format.",
                    "Always provide watermark/placeholder text. Use IsReadOnly for display-only fields. Set appropriate MaxLength for constrained inputs.")
            }
        };
    }

    private Control BuildBasicExample()
    {
        return CreateExampleSection("Basic TextBox",
            new StackPanel
            {
                Spacing = 8, MaxWidth = 400,
                Children =
                {
                    new TextBox { Watermark = "Enter your name..." },
                    new TextBox { Watermark = "With default value", Text = "Hello World" },
                    new TextBox { Watermark = "Multi-line", AcceptsReturn = true, TextWrapping = TextWrapping.Wrap, MinHeight = 80 },
                }
            },
            @"<TextBox Watermark=""Enter your name...""/>
<TextBox Watermark=""With default value"" Text=""Hello World""/>
<TextBox Watermark=""Multi-line""
         AcceptsReturn=""True""
         TextWrapping=""Wrap""
         MinHeight=""80""/>",
            @"// Access text value in C#
private void OnTextChanged(object? sender, TextChangedEventArgs e)
{
    if (sender is TextBox textBox)
    {
        var value = textBox.Text;
        Console.WriteLine($""Text changed: {value}"");
    }
}

// Programmatically set text
textBox.Text = ""New value"";",
            @"public partial class MyViewModel : ViewModelBase
{
    private string _name = """";
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }
}");
    }

    private Control BuildStylesExample()
    {
        return CreateExampleSection("Styles",
            new StackPanel
            {
                Spacing = 8, MaxWidth = 400,
                Children =
                {
                    new TextBox { Watermark = "Default style" },
                    new TextBox { Watermark = "Underline style", Classes = { "underline" } },
                }
            },
            @"<TextBox Watermark=""Default style""/>
<TextBox Watermark=""Underline style"" Classes=""underline""/>",
            @"// Apply style classes in code
var textBox = new TextBox { Watermark = ""Styled"" };
textBox.Classes.Add(""underline"");");
    }

    private Control BuildStatesExample()
    {
        return CreateExampleSection("States",
            new StackPanel
            {
                Spacing = 8, MaxWidth = 400,
                Children =
                {
                    new TextBox { Watermark = "Normal" },
                    new TextBox { Watermark = "Read-only", Text = "Read-only content", IsReadOnly = true },
                    new TextBox { Watermark = "Disabled", IsEnabled = false },
                    new TextBox { Watermark = "Password", PasswordChar = '*', MaxLength = 32 },
                }
            },
            @"<TextBox Watermark=""Normal""/>
<TextBox Watermark=""Read-only"" Text=""Read-only content"" IsReadOnly=""True""/>
<TextBox Watermark=""Disabled"" IsEnabled=""False""/>
<TextBox Watermark=""Password"" PasswordChar=""*"" MaxLength=""32""/>",
            @"// Toggle states in code
textBox.IsReadOnly = true;    // make read-only
textBox.IsEnabled = false;    // disable
textBox.PasswordChar = '*';   // mask input
textBox.MaxLength = 32;       // limit characters");
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Text", Type = "string", Default = "null", Description = "The text content" },
        new ApiProperty { PropertyName = "Watermark", Type = "string", Default = "null", Description = "Placeholder text shown when empty" },
        new ApiProperty { PropertyName = "IsReadOnly", Type = "bool", Default = "false", Description = "Prevents user editing" },
        new ApiProperty { PropertyName = "AcceptsReturn", Type = "bool", Default = "false", Description = "Enables multi-line input" },
        new ApiProperty { PropertyName = "TextWrapping", Type = "TextWrapping", Default = "NoWrap", Description = "Text wrapping mode" },
        new ApiProperty { PropertyName = "MaxLength", Type = "int", Default = "0", Description = "Maximum character count (0 = unlimited)" },
        new ApiProperty { PropertyName = "PasswordChar", Type = "char", Default = "\\0", Description = "Masking character for password input" },
    };
}
