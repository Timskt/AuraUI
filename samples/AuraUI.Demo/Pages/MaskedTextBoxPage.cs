using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Input;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class MaskedTextBoxPage : ComponentPageBase
{
    public override string ComponentName => "MaskedTextBox";
    public override string Description => "Text box with input masking for formatted input.";
    public override string Category => "Input";

    protected override Control BuildContent()
    {
        return new StackPanel
        {
            Spacing = 32,
            Children =
            {
                BuildExample1(),
                CreateApiTable(GetApiProperties()),
                CreateGuidelines(
                    "Use for phone, credit card, date input.",
                    "Show mask format as watermark.")
            }
        };
    }

    private Control BuildExample1()
    {
        return CreateExampleSection("Input Masks", BuildPreview1(),
            @"<input:MaskedTextBox Mask=""(999) 999-9999"" PromptChar=""_""/>
<input:MaskedTextBox Mask=""9999-9999-9999-9999"" PromptChar=""_""/>");
    }

    private Control BuildPreview1()
    {
        return new StackPanel { Spacing = 16, MaxWidth = 400, Children = { new StackPanel { Spacing = 4, Children = { new TextBlock { Text = "Phone" }, new AuraUI.Controls.Input.MaskedTextBox { Mask = "(999) 999-9999", PromptChar = '_' } } }, new StackPanel { Spacing = 4, Children = { new TextBlock { Text = "Credit Card" }, new AuraUI.Controls.Input.MaskedTextBox { Mask = "9999-9999-9999-9999", PromptChar = '_' } } } } };
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Mask", Type = "string?", Default = "null", Description = "Mask pattern (9=digit, a=letter)" },
        new ApiProperty { PropertyName = "PromptChar", Type = "char", Default = "_", Description = "Prompt character" },
        new ApiProperty { PropertyName = "AllowPromptAsInput", Type = "bool", Default = "false", Description = "Allow prompt char" },
    };
}
