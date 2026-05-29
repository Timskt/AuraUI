using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using AuraUI.Controls.Selection;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class RangeSliderPage : ComponentPageBase
{
    public override string ComponentName => "RangeSlider";
    public override string Description => "A dual-handle slider for selecting a range of values with configurable min/max and step.";
    public override string Category => "Selection";

    protected override Control BuildContent()
    {
        return new StackPanel
        {
            Spacing = 32,
            Children =
            {
                BuildBasicExample(),
                CreateApiTable(GetApiProperties()),
                CreateGuidelines(
                    "Use range sliders for filtering by price, date ranges, and any scenario where users need to select a minimum and maximum value.",
                    "Show the current range values as text. Set appropriate min/max bounds. Use step for discrete values.")
            }
        };
    }

    private Control BuildBasicExample()
    {
        var slider = new RangeSlider { Minimum = 0, Maximum = 100, LowerValue = 20, UpperValue = 80, Width = 400 };
        return CreateExampleSection("Range Selection", slider,
            @"<selection:RangeSlider Minimum=""0"" Maximum=""100""
                       LowerValue=""20"" UpperValue=""80""
                       Width=""400""/>");
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Minimum", Type = "double", Default = "0", Description = "Minimum value" },
        new ApiProperty { PropertyName = "Maximum", Type = "double", Default = "100", Description = "Maximum value" },
        new ApiProperty { PropertyName = "LowerValue", Type = "double", Default = "0", Description = "Lower handle value" },
        new ApiProperty { PropertyName = "UpperValue", Type = "double", Default = "100", Description = "Upper handle value" },
        new ApiProperty { PropertyName = "Step", Type = "double", Default = "1", Description = "Step increment" },
    };
}
