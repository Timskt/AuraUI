using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Layout;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class AppShellPage : ComponentPageBase
{
    public override string ComponentName => "AppShell";
    public override string Description => "A standard app layout shell with header, sidebar, content area, and footer.";
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
                    "Use for application layouts with consistent header, sidebar, and footer.",
                    "Support sidebar collapse for responsive layouts.")
            }
        };
    }

    private Control BuildExample1()
    {
        return CreateExampleSection("App Shell", BuildPreview1(),
            @"<layout:AppShell SidebarWidth=""240"">
    <layout:AppShell.Header><Border Background=""#1976D2"" Padding=""16""><TextBlock Text=""Header"" Foreground=""White""/></Border></layout:AppShell.Header>
    <layout:AppShell.Sidebar><TextBlock Text=""Sidebar""/></layout:AppShell.Sidebar>
    <TextBlock Text=""Main content""/>
</layout:AppShell>");
    }

    private Control BuildPreview1()
    {
        return new StackPanel { Spacing = 16, MaxWidth = 500, Children = { new TextBlock { Text = "AppShell provides header, sidebar, content, and footer.", TextWrapping = TextWrapping.Wrap }, new Border { BorderBrush = new SolidColorBrush(Color.Parse("#E0E0E0")), BorderThickness = new Thickness(1), CornerRadius = new CornerRadius(8), Height = 250, Child = new TextBlock { Text = "Layout preview", HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center } } } };
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Header", Type = "object?", Default = "null", Description = "Header content" },
        new ApiProperty { PropertyName = "Sidebar", Type = "object?", Default = "null", Description = "Sidebar content" },
        new ApiProperty { PropertyName = "Footer", Type = "object?", Default = "null", Description = "Footer content" },
        new ApiProperty { PropertyName = "IsSidebarCollapsed", Type = "bool", Default = "false", Description = "Sidebar collapsed" },
        new ApiProperty { PropertyName = "SidebarWidth", Type = "double", Default = "240", Description = "Sidebar width" },
    };
}
