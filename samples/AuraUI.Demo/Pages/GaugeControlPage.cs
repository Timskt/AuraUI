using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Display;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class GaugeControlPage : ComponentPageBase
{
    public override string ComponentName => "GaugeControl";
    public override string Description => "Circular gauge with pointer and color ranges.";
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
                    "Use for dashboards and KPIs.",
                    "Use color ranges for zones.")
            }
        };
    }

    private Control BuildExample1()
    {
        return CreateExampleSection("Gauge", BuildPreview1(),
            @"<display:GaugeControl Value=""72"" Min=""0"" Max=""100"" Width=""200"" Height=""200""/>");
    }

    private Control BuildPreview1()
    {
        return new GaugeControl { Width = 200, Height = 200, Value = 72, Min = 0, Max = 100, Ranges = new List<GaugeRange> { new() { Start = 0, End = 30, Color = new SolidColorBrush(Color.Parse("#F44336")) }, new() { Start = 30, End = 70, Color = new SolidColorBrush(Color.Parse("#FFC107")) }, new() { Start = 70, End = 100, Color = new SolidColorBrush(Color.Parse("#4CAF50")) } } };
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Value", Type = "double", Default = "0", Description = "Current value" },
        new ApiProperty { PropertyName = "Min", Type = "double", Default = "0", Description = "Min scale" },
        new ApiProperty { PropertyName = "Max", Type = "double", Default = "100", Description = "Max scale" },
        new ApiProperty { PropertyName = "Ranges", Type = "IList<GaugeRange>?", Default = "null", Description = "Color ranges" },
    };
}
