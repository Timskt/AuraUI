using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Layout;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class SwipeActionPage : ComponentPageBase
{
    public override string ComponentName => "SwipeAction";
    public override string Description => "Swipeable content revealing action buttons on swipe.";
    public override string Category => "Layout";

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
                    "Use for mobile list items with contextual actions.",
                    "Use color-coded actions.")
            }
        };
    }

    private Control BuildExample1()
    {
        return CreateExampleSection("Swipe Actions", BuildPreview1(),
            @"<layout:SwipeAction Threshold=""80"">
    <layout:SwipeAction.LeftActions><Button Content=""Archive"" Classes=""success""/></layout:SwipeAction.LeftActions>
    <layout:SwipeAction.RightActions><Button Content=""Delete"" Classes=""destructive""/></layout:SwipeAction.RightActions>
    <Border Background=""White"" Padding=""16""><TextBlock Text=""Swipe this""/></Border>
</layout:SwipeAction>");
    }

    private Control BuildPreview1()
    {
        return new StackPanel { Spacing = 8, MaxWidth = 400, Children = { new TextBlock { Text = "Swipe left or right for actions.", TextWrapping = TextWrapping.Wrap }, new Border { Background = new SolidColorBrush(Color.Parse("#F5F5F5")), CornerRadius = new CornerRadius(8), Padding = new Thickness(16), Child = new TextBlock { Text = "Swipe me" } } } };
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "LeftActions", Type = "object?", Default = "null", Description = "Swipe right actions" },
        new ApiProperty { PropertyName = "RightActions", Type = "object?", Default = "null", Description = "Swipe left actions" },
        new ApiProperty { PropertyName = "Threshold", Type = "double", Default = "80", Description = "Min swipe distance" },
        new ApiProperty { PropertyName = "IsSwipeEnabled", Type = "bool", Default = "true", Description = "Enable swiping" },
    };
}
