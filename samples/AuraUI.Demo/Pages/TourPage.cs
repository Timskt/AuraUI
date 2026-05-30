using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using AuraUI.Controls.Feedback;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class TourPage : ComponentPageBase
{
    public override string ComponentName => "Tour";
    public override string Description => "A guided tour component that highlights target controls with step-by-step tooltips. Used for onboarding and feature discovery.";
    public override string Category => "Feedback";

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
                    "Use Tour for new user onboarding, feature announcements, and guiding users through complex workflows step by step.",
                    "Keep steps to 3-5 maximum. Use clear, concise descriptions. Highlight the most important features first. Allow users to skip or restart the tour.")
            }
        };
    }

    private Control BuildBasicExample()
    {
        var targetBtn = new Avalonia.Controls.Button
        {
            Content = "Featured Button",
            Classes = { "primary" },
            Margin = new Thickness(0, 0, 0, 12)
        };

        var steps = new Avalonia.Collections.AvaloniaList<TourStep>
        {
            new TourStep { Title = "Welcome!", Description = "This is a guided tour of the Tour component.", Placement = PlacementMode.Bottom },
            new TourStep { Title = "Step 2", Description = "Each step highlights a target element with an overlay." },
            new TourStep { Title = "All Done!", Description = "You've completed the tour. Use this for onboarding flows." },
        };

        var panel = new StackPanel { Spacing = 12 };
        panel.Children.Add(targetBtn);
        panel.Children.Add(new Avalonia.Controls.TextBlock
        {
            Text = "The Tour component creates step-by-step guided experiences with highlighted targets and tooltip descriptions.",
            TextWrapping = Avalonia.Media.TextWrapping.Wrap,
            Foreground = GetBrush("AuraForegroundSecondaryBrush", "#666666")
        });

        return CreateExampleSection("Guided Tour", panel,
            @"<feedback:Tour>
    <feedback:TourStep Title=""Welcome!""
        Description=""This is a guided tour.""
        Placement=""Bottom""/>
    <feedback:TourStep Title=""Step 2""
        Description=""Each step highlights a target."" />
    <feedback:TourStep Title=""All Done!""
        Description=""Tour complete."" />
</feedback:Tour>");
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Steps", Type = "AvaloniaList<TourStep>", Default = "null", Description = "Tour steps" },
        new ApiProperty { PropertyName = "CurrentStep", Type = "int", Default = "0", Description = "Current step index" },
        new ApiProperty { PropertyName = "ShowArrow", Type = "bool", Default = "true", Description = "Show tooltip arrow" },
        new ApiProperty { PropertyName = "Mask", Type = "bool", Default = "true", Description = "Show overlay mask" },
        new ApiProperty { PropertyName = "Closeable", Type = "bool", Default = "true", Description = "Allow closing the tour" },
    };
}
