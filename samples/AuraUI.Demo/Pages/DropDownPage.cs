using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Layout;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class DropDownPage : ComponentPageBase
{
    public override string ComponentName => "DropDown";
    public override string Description => "A generic dropdown container wrapping content in a popup overlay.";
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
                    "Use for custom popovers and floating panels.",
                    "Set appropriate MaxDropDownHeight.")
            }
        };
    }

    private Control BuildExample1()
    {
        return CreateExampleSection("Dropdown", BuildPreview1(),
            @"<layout:DropDown Placement=""Bottom"" MaxDropDownHeight=""300"">
    <Button Content=""Open"" Classes=""outline""/>
</layout:DropDown>",
            @"dropdown.IsOpen = !dropdown.IsOpen;");
    }

    private Control BuildPreview1()
    {
        return new Button { Content = "Open Dropdown", Classes = { "outline" } };
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "IsOpen", Type = "bool", Default = "false", Description = "Whether open" },
        new ApiProperty { PropertyName = "Placement", Type = "DropDownPlacement", Default = "Bottom", Description = "Bottom, Top, Left, Right" },
        new ApiProperty { PropertyName = "MaxDropDownHeight", Type = "double", Default = "400", Description = "Max height" },
    };
}
