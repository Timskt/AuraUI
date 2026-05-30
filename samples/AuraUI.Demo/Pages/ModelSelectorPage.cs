using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Selection;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class ModelSelectorPage : ComponentPageBase
{
    public override string ComponentName => "ModelSelector";
    public override string Description => "AI model selector with provider and context info.";
    public override string Category => "Selection";

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
                    "Use for AI-powered applications.",
                    "Display provider and context length.")
            }
        };
    }

    private Control BuildExample1()
    {
        return CreateExampleSection("Model Selector", BuildPreview1(),
            @"<Selection:ModelSelector SelectedModel=""{Binding Model}"" Models=""{Binding Models}""/>");
    }

    private Control BuildPreview1()
    {
        return new StackPanel { Spacing = 16, MaxWidth = 400, Children = { new TextBlock { Text = "Select an AI model:", TextWrapping = TextWrapping.Wrap }, new Border { BorderBrush = new SolidColorBrush(Color.Parse("#E0E0E0")), BorderThickness = new Thickness(1), CornerRadius = new CornerRadius(8), Padding = new Thickness(12), Child = new StackPanel { Spacing = 4, Children = { new TextBlock { Text = "GPT-4o", FontWeight = FontWeight.SemiBold }, new TextBlock { Text = "OpenAI - 128K context", FontSize = 12, Foreground = new SolidColorBrush(Color.Parse("#757575")) } } } } } };
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "SelectedModel", Type = "ModelInfo?", Default = "null", Description = "Selected model" },
        new ApiProperty { PropertyName = "Models", Type = "IList<ModelInfo>?", Default = "null", Description = "Available models" },
    };
}
