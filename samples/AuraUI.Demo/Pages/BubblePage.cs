using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Layout;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class BubblePage : ComponentPageBase
{
    public override string ComponentName => "Bubble";
    public override string Description => "A speech-bubble content control with a configurable arrow pointer.";
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
                    "Use for chat UIs and callout panels.",
                    "Point arrow toward the source.")
            }
        };
    }

    private Control BuildExample1()
    {
        return CreateExampleSection("Speech Bubbles", BuildPreview1(),
            @"<layout:Bubble ArrowPlacement=""Bottom"" Background=""#E3F2FD"">
    <TextBlock Text=""Hello!"" Margin=""12,8""/>
</layout:Bubble>");
    }

    private Control BuildPreview1()
    {
        return new StackPanel { Spacing = 16, MaxWidth = 500, Children = { new Border { Background = new SolidColorBrush(Color.Parse("#E3F2FD")), CornerRadius = new CornerRadius(8), Padding = new Thickness(16), Child = new TextBlock { Text = "Hello!" } }, new Border { Background = new SolidColorBrush(Color.Parse("#F3E5F5")), CornerRadius = new CornerRadius(8), Padding = new Thickness(16), Child = new TextBlock { Text = "Question?" } } } };
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "ArrowPlacement", Type = "ArrowPlacement", Default = "Bottom", Description = "Top, Bottom, Left, Right" },
        new ApiProperty { PropertyName = "ArrowSize", Type = "double", Default = "8.0", Description = "Arrow size" },
        new ApiProperty { PropertyName = "ArrowOffset", Type = "double", Default = "16.0", Description = "Arrow offset" },
        new ApiProperty { PropertyName = "BubbleCornerRadius", Type = "CornerRadius", Default = "8", Description = "Corner radius" },
    };
}
