using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Layout;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class FormGroupPage : ComponentPageBase
{
    public override string ComponentName => "FormGroup";
    public override string Description => "Form layout with label and field vertically or horizontally.";
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
                    "Use for consistent form layouts.",
                    "Mark required fields consistently.")
            }
        };
    }

    private Control BuildExample1()
    {
        return CreateExampleSection("Form Groups", BuildPreview1(),
            @"<layout:FormGroup Header=""Username"" IsRequired=""True"" HeaderPlacement=""Left"" HeaderWidth=""120"">
    <TextBox Watermark=""Enter username""/>
</layout:FormGroup>");
    }

    private Control BuildPreview1()
    {
        return new StackPanel { Spacing = 16, MaxWidth = 500, Children = { new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8, Children = { new TextBlock { Text = "Username *", Width = 100, VerticalAlignment = VerticalAlignment.Center }, new TextBox { Watermark = "Enter username", Width = 250 } } }, new StackPanel { Spacing = 4, Children = { new TextBlock { Text = "Bio" }, new TextBox { Watermark = "About you", Height = 80, AcceptsReturn = true, Width = 358 } } } } };
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Header", Type = "object?", Default = "null", Description = "Label" },
        new ApiProperty { PropertyName = "HeaderPlacement", Type = "FormGroupHeaderPlacement", Default = "Left", Description = "Left or Top" },
        new ApiProperty { PropertyName = "HeaderWidth", Type = "double", Default = "120", Description = "Header width" },
        new ApiProperty { PropertyName = "IsRequired", Type = "bool", Default = "false", Description = "Required indicator" },
    };
}
