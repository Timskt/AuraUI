using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Navigation;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class MobileStepperPage : ComponentPageBase
{
    public override string ComponentName => "MobileStepper";
    public override string Description => "Mobile stepper with dots, progress, or numbers.";
    public override string Category => "Navigation";

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
                    "Use for multi-step workflows.",
                    "Show labels for context.")
            }
        };
    }

    private Control BuildExample1()
    {
        return CreateExampleSection("Stepper", BuildPreview1(),
            @"<Navigation:MobileStepper CurrentStep=""{Binding Step}"" ShowLabels=""True""/>");
    }

    private Control BuildPreview1()
    {
        return new StackPanel { Spacing = 24, MaxWidth = 400, Children = { new TextBlock { Text = "Mobile stepper for multi-step workflows.", TextWrapping = TextWrapping.Wrap }, new Border { BorderBrush = new SolidColorBrush(Color.Parse("#E0E0E0")), BorderThickness = new Thickness(1), CornerRadius = new CornerRadius(8), Padding = new Thickness(16), Child = new TextBlock { Text = "Step 2 of 4", HorizontalAlignment = HorizontalAlignment.Center } } } };
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Steps", Type = "ObservableCollection<string>?", Default = "null", Description = "Step labels" },
        new ApiProperty { PropertyName = "CurrentStep", Type = "int", Default = "0", Description = "Current step" },
        new ApiProperty { PropertyName = "ShowLabels", Type = "bool", Default = "false", Description = "Show labels" },
    };
}
