using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using AuraUI.Controls.Feedback;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class LoadingOverlayPage : ComponentPageBase
{
    public override string ComponentName => "LoadingOverlay";
    public override string Description => "Provides attached properties for showing a loading overlay with spinner over any container. Supports custom messages and spinner sizes.";
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
                    "Use LoadingOverlay to indicate loading states on panels, cards, forms, or any container while async operations are in progress.",
                    "Show a message explaining what's loading. Use appropriate spinner size. Apply to the smallest container that's actually loading. Remove promptly when done.")
            }
        };
    }

    private Control BuildBasicExample()
    {
        var container = new Border
        {
            Background = GetBrush("AuraCardBrush", "#FFFFFF"),
            BorderBrush = GetBrush("AuraBorderBrush", "#E0E0E0"),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(8),
            Padding = new Thickness(20),
            Width = 400,
            Height = 200,
            Child = new Avalonia.Controls.TextBlock
            {
                Text = "This content is loaded. Click the button to show/hide the loading overlay.",
                TextWrapping = Avalonia.Media.TextWrapping.Wrap
            }
        };

        LoadingOverlay.SetIsLoading(container, true);
        LoadingOverlay.SetMessage(container, "Loading data...");

        var toggleBtn = new Avalonia.Controls.Button
        {
            Content = "Toggle Loading",
            Classes = { "primary" },
            Margin = new Thickness(0, 0, 0, 12)
        };
        toggleBtn.Click += (_, _) =>
        {
            var isLoading = LoadingOverlay.GetIsLoading(container);
            LoadingOverlay.SetIsLoading(container, !isLoading);
        };

        var panel = new StackPanel { Spacing = 12 };
        panel.Children.Add(toggleBtn);
        panel.Children.Add(container);

        return CreateExampleSection("Loading Overlay", panel,
            @"<Border x:Name=""panel"" Padding=""20"" Width=""400"" Height=""200""
    feedback:LoadingOverlay.IsLoading=""True""
    feedback:LoadingOverlay.Message=""Loading data..."">
    <TextBlock Text=""Content behind the overlay""/>
</Border>",
            @"LoadingOverlay.SetIsLoading(panel, true);  // show
LoadingOverlay.SetIsLoading(panel, false); // hide
LoadingOverlay.SetMessage(panel, ""Processing..."");");
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "IsLoading (attached)", Type = "bool", Default = "false", Description = "Show/hide the overlay" },
        new ApiProperty { PropertyName = "Message (attached)", Type = "string", Default = "null", Description = "Loading message" },
        new ApiProperty { PropertyName = "SpinnerSize (attached)", Type = "double", Default = "40", Description = "Spinner diameter" },
    };
}
