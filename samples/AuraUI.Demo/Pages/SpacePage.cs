using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Layout;
using AuraUI.Controls.Input;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class SpacePage : ComponentPageBase
{
    public override string ComponentName => "Space";
    public override string Description => "A flex spacing container that arranges child elements in a single direction with uniform spacing, optional wrapping, and configurable alignment.";
    public override string Category => "Layout";

    protected override Control BuildContent()
    {
        return new StackPanel
        {
            Spacing = 32,
            Children =
            {
                BuildHorizontalExample(),
                BuildVerticalExample(),
                CreateApiTable(GetApiProperties()),
                CreateGuidelines(
                    "Use Space to add consistent spacing between inline elements without manual margin management. Great for button groups, tag lists, and toolbar items.",
                    "Choose the appropriate direction and alignment. Use Wrap for responsive layouts. Prefer Space over manual margins for consistent spacing.")
            }
        };
    }

    private Control BuildHorizontalExample()
    {
        var space = new Space
        {
            Spacing = 12,
            Direction = SpaceDirection.Horizontal,
            Alignment = SpaceAlign.Center
        };
        space.Children.Add(new AuraButton { Content = "Button 1", Classes = { "primary" } });
        space.Children.Add(new AuraButton { Content = "Button 2", Classes = { "outline" } });
        space.Children.Add(new AuraButton { Content = "Button 3", Classes = { "outline" } });

        return CreateExampleSection("Horizontal Space", space,
            @"<layout:Space Spacing=""12"" Direction=""Horizontal"" Align=""Center"">
    <Button Content=""Button 1"" Classes=""primary""/>
    <Button Content=""Button 2"" Classes=""outline""/>
    <Button Content=""Button 3"" Classes=""outline""/>
</layout:Space>");
    }

    private Control BuildVerticalExample()
    {
        var space = new Space
        {
            Spacing = 16,
            Direction = SpaceDirection.Vertical,
            MaxWidth = 300
        };
        for (int i = 1; i <= 4; i++)
        {
            space.Children.Add(new Border
            {
                Background = GetBrush("AuraMutedBrush", "#F0F0F0"),
                CornerRadius = new CornerRadius(6),
                Padding = new Thickness(16),
                Child = new Avalonia.Controls.TextBlock { Text = $"Vertical item {i}" }
            });
        }

        return CreateExampleSection("Vertical Space", space,
            @"<layout:Space Spacing=""16"" Direction=""Vertical"">
    <Border Background=""#F0F0F0"" Padding=""16"">
        <TextBlock Text=""Item 1""/>
    </Border>
    <Border Background=""#F0F0F0"" Padding=""16"">
        <TextBlock Text=""Item 2""/>
    </Border>
</layout:Space>");
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Spacing", Type = "double", Default = "8", Description = "Uniform spacing between items" },
        new ApiProperty { PropertyName = "Direction", Type = "SpaceDirection", Default = "Horizontal", Description = "Layout direction" },
        new ApiProperty { PropertyName = "Align", Type = "SpaceAlign", Default = "Stretch", Description = "Cross-axis alignment" },
        new ApiProperty { PropertyName = "Wrap", Type = "bool", Default = "false", Description = "Allow items to wrap" },
        new ApiProperty { PropertyName = "Split", Type = "object", Default = "null", Description = "Separator between items" },
    };
}
