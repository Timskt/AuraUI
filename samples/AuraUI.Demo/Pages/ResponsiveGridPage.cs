using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Layout;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class ResponsiveGridPage : ComponentPageBase
{
    public override string ComponentName => "ResponsiveGrid";
    public override string Description => "Responsive Grid switching columns based on width breakpoints.";
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
                    "Use for form layouts that reorganize at different sizes.",
                    "Define breakpoints for mobile/tablet/desktop.")
            }
        };
    }

    private Control BuildExample1()
    {
        return CreateExampleSection("Responsive Grid", BuildPreview1(),
            @"<layout:ResponsiveGrid ColumnSpacing=""8"" RowSpacing=""8""/>");
    }

    private Control BuildPreview1()
    {
        return new StackPanel { Spacing = 16, MaxWidth = 500, Children = { new TextBlock { Text = "Switches columns based on width.", TextWrapping = TextWrapping.Wrap }, new Border { BorderBrush = new SolidColorBrush(Color.Parse("#E0E0E0")), BorderThickness = new Thickness(1), CornerRadius = new CornerRadius(4), Height = 150, Child = new TextBlock { Text = "Responsive grid", HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center } } } };
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Breakpoints", Type = "Dictionary<int, ColumnDefinitions>?", Default = "null", Description = "Width-to-columns" },
        new ApiProperty { PropertyName = "ColumnSpacing", Type = "double", Default = "0", Description = "Column spacing" },
        new ApiProperty { PropertyName = "RowSpacing", Type = "double", Default = "0", Description = "Row spacing" },
    };
}
