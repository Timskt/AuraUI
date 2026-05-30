using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Navigation;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class FramePage : ComponentPageBase
{
    public override string ComponentName => "Frame";
    public override string Description => "Content frame with page transition animations.";
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
                    "Use for MVVM navigation.",
                    "CrossFade for general nav, Slide for hierarchical.")
            }
        };
    }

    private Control BuildExample1()
    {
        return CreateExampleSection("Navigation Frame", BuildPreview1(),
            @"<Navigation:Frame Router=""{Binding Router}"" TransitionType=""CrossFade""/>",
            @"router.NavigateTo<HomeViewModel>();");
    }

    private Control BuildPreview1()
    {
        return new StackPanel { Spacing = 8, Children = { new TextBlock { Text = "Frame with transition animations.", TextWrapping = TextWrapping.Wrap }, new Border { BorderBrush = new SolidColorBrush(Color.Parse("#E0E0E0")), BorderThickness = new Thickness(1), CornerRadius = new CornerRadius(8), Height = 200, Child = new TextBlock { Text = "Frame content", HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center } } } };
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Router", Type = "Router?", Default = "null", Description = "Router" },
        new ApiProperty { PropertyName = "TransitionType", Type = "PageTransitionType", Default = "CrossFade", Description = "Transition type" },
        new ApiProperty { PropertyName = "TransitionDuration", Type = "TimeSpan", Default = "300ms", Description = "Duration" },
    };
}
