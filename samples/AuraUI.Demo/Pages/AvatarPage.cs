using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using AuraUI.Controls.Layout;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class AvatarPage : ComponentPageBase
{
    public override string ComponentName => "Avatar";
    public override string Description => "A versatile avatar control that can display an image, icon, or text fallback with configurable size and shape.";
    public override string Category => "Layout";

    protected override Control BuildContent()
    {
        return new StackPanel
        {
            Spacing = 32,
            Children =
            {
                BuildSizesExample(),
                BuildShapesExample(),
                CreateApiTable(GetApiProperties()),
                CreateGuidelines(
                    "Use avatars to represent users, organizations, or entities. They work well in lists, comments, headers, and user profiles.",
                    "Use consistent sizes within the same context. Provide meaningful fallback initials (1-2 characters). Use circle shape for people and square for organizations.")
            }
        };
    }

    private Control BuildSizesExample()
    {
        var panel = new WrapPanel { VerticalAlignment = VerticalAlignment.Center };
        foreach (var size in new[] { AvatarSize.Small, AvatarSize.Medium, AvatarSize.Large, AvatarSize.XL })
        {
            panel.Children.Add(new Avatar
            {
                FallbackText = size.ToString()[..1],
                Shape = AvatarShape.Circle,
                Size = size,
                FallbackBackground = GetBrush("AuraPrimaryBrush", "#0078D4"),
                Margin = new Thickness(0, 0, 8, 8)
            });
        }
        return CreateExampleSection("Sizes", panel,
            @"<layout:Avatar FallbackText=""S"" Shape=""Circle"" Size=""Small""
         FallbackBackground=""{DynamicResource AuraPrimaryBrush}""/>
<layout:Avatar FallbackText=""M"" Shape=""Circle"" Size=""Medium""
         FallbackBackground=""{DynamicResource AuraPrimaryBrush}""/>
<layout:Avatar FallbackText=""L"" Shape=""Circle"" Size=""Large""
         FallbackBackground=""{DynamicResource AuraPrimaryBrush}""/>
<layout:Avatar FallbackText=""X"" Shape=""Circle"" Size=""XL""
         FallbackBackground=""{DynamicResource AuraPrimaryBrush}""/>");
    }

    private Control BuildShapesExample()
    {
        var panel = new WrapPanel { VerticalAlignment = VerticalAlignment.Center };
        var shapes = new[] {
            (AvatarShape.Circle, "JD", "#0078D4"),
            (AvatarShape.Square, "AB", "#107C10"),
        };
        foreach (var (shape, text, color) in shapes)
        {
            panel.Children.Add(new Avatar
            {
                FallbackText = text,
                Shape = shape,
                Size = AvatarSize.Large,
                FallbackBackground = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse(color)),
                Margin = new Thickness(0, 0, 8, 8)
            });
        }
        return CreateExampleSection("Shapes", panel,
            @"<layout:Avatar FallbackText=""JD"" Shape=""Circle"" Size=""Large""
         FallbackBackground=""{DynamicResource AuraPrimaryBrush}""/>
<layout:Avatar FallbackText=""AB"" Shape=""Square"" Size=""Large""
         FallbackBackground=""{DynamicResource AuraSuccessBrush}""/>");
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Source", Type = "IImage", Default = "null", Description = "Image source for the avatar" },
        new ApiProperty { PropertyName = "Size", Type = "AvatarSize", Default = "Medium", Description = "Size variant: Small, Medium, Large, XL" },
        new ApiProperty { PropertyName = "Shape", Type = "AvatarShape", Default = "Circle", Description = "Shape: Circle or Square" },
        new ApiProperty { PropertyName = "FallbackText", Type = "string", Default = "null", Description = "Text shown when no image (typically 1-2 initials)" },
        new ApiProperty { PropertyName = "FallbackBackground", Type = "IBrush", Default = "null", Description = "Background brush for fallback text" },
    };
}
