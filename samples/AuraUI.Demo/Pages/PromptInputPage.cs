using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Input;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class PromptInputPage : ComponentPageBase
{
    public override string ComponentName => "PromptInput";
    public override string Description => "AI prompt input with token counting and temperature control.";
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
                    "Use for AI chat interfaces.",
                    "Show token count.")
            }
        };
    }

    private Control BuildExample1()
    {
        return CreateExampleSection("AI Prompt", BuildPreview1(),
            @"<input:PromptInput Placeholder=""Enter prompt..."" MaxTokens=""4096"" Width=""500""/>");
    }

    private Control BuildPreview1()
    {
        return new StackPanel { Spacing = 8, MaxWidth = 500, Children = { new TextBox { Watermark = "Enter prompt...", Height = 80, AcceptsReturn = true }, new StackPanel { Orientation = Orientation.Horizontal, Spacing = 16, Children = { new TextBlock { Text = "Tokens: 42 / 4096", FontSize = 12, Foreground = new SolidColorBrush(Color.Parse("#757575")), VerticalAlignment = VerticalAlignment.Center }, new Button { Content = "Submit", Classes = { "primary" } } } } } };
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Placeholder", Type = "string?", Default = "Enter prompt...", Description = "Placeholder" },
        new ApiProperty { PropertyName = "MaxTokens", Type = "int", Default = "4096", Description = "Max tokens" },
        new ApiProperty { PropertyName = "Temperature", Type = "double", Default = "0.7", Description = "Temperature" },
        new ApiProperty { PropertyName = "IsLoading", Type = "bool", Default = "false", Description = "Loading state" },
    };
}
