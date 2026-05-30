using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Display;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class TreemapControlPage : ComponentPageBase
{
    public override string ComponentName => "TreemapControl";
    public override string Description => "Squarified treemap for hierarchical data.";
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
                    "Use for disk usage and market share visualization.",
                    "Use distinct colors.")
            }
        };
    }

    private Control BuildExample1()
    {
        return CreateExampleSection("Treemap", BuildPreview1(),
            @"<display:TreemapControl Items=""{Binding Data}"" Width=""600"" Height=""300""/>");
    }

    private Control BuildPreview1()
    {
        return new TreemapControl { Width = 400, Height = 200, Items = new List<TreemapItem> { new() { Name = "Docs", Value = 40, Color = new SolidColorBrush(Color.Parse("#42A5F5")) }, new() { Name = "Images", Value = 30, Color = new SolidColorBrush(Color.Parse("#66BB6A")) }, new() { Name = "Videos", Value = 20, Color = new SolidColorBrush(Color.Parse("#FFA726")) }, new() { Name = "Other", Value = 10, Color = new SolidColorBrush(Color.Parse("#EF5350")) } } };
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Items", Type = "IList<TreemapItem>?", Default = "null", Description = "Data items" },
        new ApiProperty { PropertyName = "ColorPalette", Type = "IList<IBrush>?", Default = "null", Description = "Color palette" },
    };
}
