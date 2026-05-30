using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Layout;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class FloatButtonPage : ComponentPageBase
{
    public override string ComponentName => "FloatButton";
    public override string Description => "A fixed-position floating action button with icon and tooltip support. Can be placed in any corner of the container.";
    public override string Category => "Layout";

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
                    "Use FloatButton for primary actions that should always be accessible, such as 'Add', 'Compose', or 'Help'.",
                    "Place in the bottom-right corner by default. Use a recognizable icon. Keep tooltip text short. Avoid multiple float buttons on one screen.")
            }
        };
    }

    private Control BuildBasicExample()
    {
        var container = new Grid { Width = 500, Height = 250, ClipToBounds = true };
        container.Children.Add(new Border
        {
            Background = GetBrush("AuraMutedBrush", "#F5F5F5"),
            CornerRadius = new CornerRadius(8),
            Child = new TextBlock
            {
                Text = "Container area - Float buttons are positioned relative to this container",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(20)
            }
        });

        var fb1 = new FloatButton
        {
            Shape = FloatButtonShape.Circle,
            Position = FloatButtonPosition.BottomRight,
            TooltipText = "Scroll to top",
            Content = new TextBlock { Text = "^", FontSize = 20, FontWeight = FontWeight.Bold, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center }
        };

        var fb2 = new FloatButton
        {
            Shape = FloatButtonShape.Square,
            Position = FloatButtonPosition.BottomLeft,
            TooltipText = "Help",
            Content = new TextBlock { Text = "?", FontSize = 18, FontWeight = FontWeight.Bold, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center }
        };

        container.Children.Add(fb1);
        container.Children.Add(fb2);

        return CreateExampleSection("Float Button Shapes", container,
            @"<layout:FloatButton Shape=""Circle"" Position=""BottomRight""
    ToolTip=""Scroll to top"">
    <TextBlock Text=""^"" FontSize=""20""/>
</layout:FloatButton>

<layout:FloatButton Shape=""Square"" Position=""BottomLeft""
    ToolTip=""Help"">
    <TextBlock Text=""?"" FontSize=""18""/>
</layout:FloatButton>");
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Shape", Type = "FloatButtonShape", Default = "Circle", Description = "Circle or Square" },
        new ApiProperty { PropertyName = "Position", Type = "FloatButtonPosition", Default = "BottomRight", Description = "Screen corner position" },
        new ApiProperty { PropertyName = "Icon", Type = "object", Default = "null", Description = "Icon content" },
        new ApiProperty { PropertyName = "ToolTip", Type = "string", Default = "null", Description = "Tooltip text" },
        new ApiProperty { PropertyName = "Badge", Type = "string", Default = "null", Description = "Badge count or text" },
    };
}
