using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Layout;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class TransformControlPage : ComponentPageBase
{
    public override string ComponentName => "TransformControl";
    public override string Description => "Content control with animated scale, rotation, and translation transforms.";
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
                    "Use for hover effects and animations.",
                    "Use small scale values (1.05-1.2).")
            }
        };
    }

    private Control BuildExample1()
    {
        return CreateExampleSection("Transform Effects", BuildPreview1(),
            @"<layout:TransformControl ScaleX=""1.2"" ScaleY=""1.2"">
    <Border Background=""#E3F2FD"" Width=""100"" Height=""80""><TextBlock Text=""Scaled""/></Border>
</layout:TransformControl>");
    }

    private Control BuildPreview1()
    {
        return new WrapPanel { Children = { new TransformControl { ScaleX = 1.2, ScaleY = 1.2, Content = new Border { Background = new SolidColorBrush(Color.Parse("#E3F2FD")), Width = 100, Height = 80, CornerRadius = new CornerRadius(8), Child = new TextBlock { Text = "Scaled", HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center } }, Margin = new Thickness(0, 0, 16, 16) }, new TransformControl { RotateAngle = 15, Content = new Border { Background = new SolidColorBrush(Color.Parse("#F3E5F5")), Width = 100, Height = 80, CornerRadius = new CornerRadius(8), Child = new TextBlock { Text = "Rotated", HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center } } } } };
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "ScaleX", Type = "double", Default = "1.0", Description = "H scale" },
        new ApiProperty { PropertyName = "ScaleY", Type = "double", Default = "1.0", Description = "V scale" },
        new ApiProperty { PropertyName = "RotateAngle", Type = "double", Default = "0", Description = "Rotation degrees" },
        new ApiProperty { PropertyName = "TranslateX", Type = "double", Default = "0", Description = "H translation" },
        new ApiProperty { PropertyName = "TranslateY", Type = "double", Default = "0", Description = "V translation" },
    };
}
