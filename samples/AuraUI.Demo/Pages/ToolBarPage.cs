using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Navigation;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class ToolBarPage : ComponentPageBase
{
    public override string ComponentName => "ToolBar";
    public override string Description => "Toolbar with uniform button sizing.";
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
                    "Use for grouped action buttons.",
                    "Use separators between groups.")
            }
        };
    }

    private Control BuildExample1()
    {
        return CreateExampleSection("Toolbar", BuildPreview1(),
            @"<Navigation:ToolBar ItemSpacing=""4"" BarPadding=""4"">
    <Button Content=""New"" Classes=""ghost""/>
    <Button Content=""Open"" Classes=""ghost""/>
    <Button Content=""Save"" Classes=""ghost""/>
</Navigation:ToolBar>");
    }

    private Control BuildPreview1()
    {
        return new ToolBar { Width = 500, BarPadding = new Thickness(4), ItemSpacing = 4, Children = { new Button { Content = "New", Classes = { "ghost" } }, new Button { Content = "Open", Classes = { "ghost" } }, new Button { Content = "Save", Classes = { "ghost" } } } };
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "ToolBarOrientation", Type = "Orientation", Default = "Horizontal", Description = "Orientation" },
        new ApiProperty { PropertyName = "ButtonSize", Type = "Size", Default = "32x32", Description = "Button size" },
        new ApiProperty { PropertyName = "ItemSpacing", Type = "double", Default = "4", Description = "Item spacing" },
        new ApiProperty { PropertyName = "BarPadding", Type = "Thickness", Default = "4", Description = "Padding" },
    };
}
