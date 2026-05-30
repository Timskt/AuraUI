using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Display;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class RichTextBlockPage : ComponentPageBase
{
    public override string ComponentName => "RichTextBlock";
    public override string Description => "Rich text with bold, italic, hyperlinks, and color.";
    public override string Category => "Display";

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
                    "Use for formatted text display.",
                    "Use markup for simple formatting.")
            }
        };
    }

    private Control BuildExample1()
    {
        return CreateExampleSection("Rich Text", BuildPreview1(),
            @"<display:RichTextBlock Text=""This is **bold** and *italic*.""/>");
    }

    private Control BuildPreview1()
    {
        return new StackPanel { Spacing = 16, MaxWidth = 500, Children = { new RichTextBlock { Text = "This is **bold** and *italic*." } } };
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Text", Type = "string?", Default = "null", Description = "Inline markup" },
        new ApiProperty { PropertyName = "Segments", Type = "IList<RichTextSegment>?", Default = "null", Description = "Structured segments" },
        new ApiProperty { PropertyName = "LineSpacing", Type = "double", Default = "4.0", Description = "Line spacing" },
    };
}
