using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using AuraUI.Controls.Display;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class StepIndicatorPage : ComponentPageBase
{
    public override string ComponentName => "StepIndicator";
    public override string Description => "A step indicator showing progression through a multi-step process, with status-based colors, connecting lines, and icons.";
    public override string Category => "Display";

    protected override Control BuildContent()
    {
        return new StackPanel
        {
            Spacing = 32,
            Children =
            {
                BuildBasicExample(),
                CreateApiTable(GetApiProperties()),
                CreateGuidelines(
                    "Use StepIndicator for wizards, onboarding flows, checkout processes, or any multi-step workflow where users need to track progress.",
                    "Clearly label each step. Use Completed/Active/Pending states consistently. Allow navigation to completed steps. Show error state for failed steps.")
            }
        };
    }

    private Control BuildBasicExample()
    {
        var steps = new StepIndicator
        {
            CurrentStep = 1,
            Width = 500,
            ItemsSource = new List<string> { "Cart", "Shipping", "Payment", "Confirm" }
        };

        var stepsVertical = new StepIndicator
        {
            CurrentStep = 2,
            Orientation = StepOrientation.Vertical,
            Height = 200,
            Margin = new Thickness(0, 16, 0, 0),
            ItemsSource = new List<string> { "Step 1", "Step 2", "Step 3" }
        };

        var panel = new StackPanel { Spacing = 16 };
        panel.Children.Add(steps);
        panel.Children.Add(stepsVertical);

        return CreateExampleSection("Step Indicator", panel,
            @"<display:StepIndicator CurrentStep=""1"" Width=""500"">
    <display:StepIndicatorItem Title=""Cart""/>
    <display:StepIndicatorItem Title=""Shipping""/>
    <display:StepIndicatorItem Title=""Payment""/>
    <display:StepIndicatorItem Title=""Confirm""/>
</display:StepIndicator>

<display:StepIndicator CurrentStep=""2"" Orientation=""Vertical"">
    <display:StepIndicatorItem Title=""Step 1""/>
    <display:StepIndicatorItem Title=""Step 2""/>
    <display:StepIndicatorItem Title=""Step 3""/>
</display:StepIndicator>");
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "CurrentStep", Type = "int", Default = "0", Description = "Active step index" },
        new ApiProperty { PropertyName = "Orientation", Type = "StepOrientation", Default = "Horizontal", Description = "Horizontal or Vertical" },
    };
}
