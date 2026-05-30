using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Display;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class BulletChartPage : ComponentPageBase
{
    public override string ComponentName => "BulletChart";
    public override string Description => "KPI bullet chart with ranges, value, and target.";
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
                    "Use for KPI dashboards.",
                    "Use 3-4 ranges.")
            }
        };
    }

    private Control BuildExample1()
    {
        return CreateExampleSection("Bullet Chart", BuildPreview1(),
            @"<display:BulletChart Value=""75"" Target=""80"" MinValue=""0"" MaxValue=""100"" Width=""400"" Height=""60""/>");
    }

    private Control BuildPreview1()
    {
        return new BulletChart { Width = 400, Height = 60, Value = 75, Target = 80, MinValue = 0, MaxValue = 100, Ranges = new List<BulletRange> { new() { Start = 0, End = 40, Color = new SolidColorBrush(Color.Parse("#FFCDD2")) }, new() { Start = 40, End = 70, Color = new SolidColorBrush(Color.Parse("#FFF9C4")) }, new() { Start = 70, End = 100, Color = new SolidColorBrush(Color.Parse("#C8E6C9")) } } };
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Value", Type = "double", Default = "0", Description = "Value marker" },
        new ApiProperty { PropertyName = "Target", Type = "double", Default = "0", Description = "Target line" },
        new ApiProperty { PropertyName = "MinValue", Type = "double", Default = "0", Description = "Min scale" },
        new ApiProperty { PropertyName = "MaxValue", Type = "double", Default = "100", Description = "Max scale" },
        new ApiProperty { PropertyName = "Ranges", Type = "IList<BulletRange>?", Default = "null", Description = "Background ranges" },
    };
}
