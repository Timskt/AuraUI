using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Layout;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class StackPanelResponsivePage : ComponentPageBase
{
    public override string ComponentName => "StackPanelResponsive";
    public override string Description => "StackPanel that switches orientation at a width breakpoint.";
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
                    "Use for layouts that adapt orientation to screen width.",
                    "Set BreakpointWidth based on content.")
            }
        };
    }

    private Control BuildExample1()
    {
        return CreateExampleSection("Responsive Stack", BuildPreview1(),
            @"<layout:StackPanelResponsive BreakpointWidth=""600""
    DefaultOrientation=""Horizontal"" OrientationAtBreakpoint=""Vertical"" Spacing=""8""/>");
    }

    private Control BuildPreview1()
    {
        return new StackPanelResponsive { Width = 500, BreakpointWidth = 600, DefaultOrientation = Orientation.Horizontal, OrientationAtBreakpoint = Orientation.Vertical, Spacing = 8, Children = { new Border { Background = new SolidColorBrush(Color.Parse("#E3F2FD")), Width = 150, Height = 60 }, new Border { Background = new SolidColorBrush(Color.Parse("#F3E5F5")), Width = 150, Height = 60 } } };
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "BreakpointWidth", Type = "double", Default = "600", Description = "Width threshold" },
        new ApiProperty { PropertyName = "DefaultOrientation", Type = "Orientation", Default = "Horizontal", Description = "Above breakpoint" },
        new ApiProperty { PropertyName = "OrientationAtBreakpoint", Type = "Orientation", Default = "Vertical", Description = "Below breakpoint" },
    };
}
