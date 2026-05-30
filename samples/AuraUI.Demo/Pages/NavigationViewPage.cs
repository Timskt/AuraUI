using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Navigation;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class NavigationViewPage : ComponentPageBase
{
    public override string ComponentName => "NavigationView";
    public override string Description => "A navigation view control with hamburger menu, adaptive display modes (minimal/compact/expanded), header, footer items, and a settings item.";
    public override string Category => "Navigation";

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
                    "Use NavigationView for app-level navigation with a collapsible side pane. Ideal for desktop apps with multiple sections.",
                    "Use Compact mode for default width. Provide a header with app branding. Place settings as a footer item. Support keyboard navigation.")
            }
        };
    }

    private Control BuildBasicExample()
    {
        var navView = new NavigationView
        {
            DisplayMode = NavigationViewDisplayMode.Compact,
            Header = new TextBlock
            {
                Text = "My App",
                FontWeight = FontWeight.Bold,
                FontSize = 16,
                Margin = new Thickness(16, 8)
            },
            IsSettingsVisible = true,
            IsPaneOpen = true,
            Width = 600,
            Height = 350,
            ItemsSource = new List<string> { "Home", "Documents", "Settings", "About" },
            Content = new TextBlock
            {
                Text = "Main content area - select a navigation item from the pane.",
                Margin = new Thickness(24),
                TextWrapping = TextWrapping.Wrap
            }
        };

        return CreateExampleSection("Navigation View", navView,
            @"<nav:NavigationView DisplayMode=""Compact""
    IsSettingsVisible=""True"" IsPaneOpen=""True""
    Width=""600"" Height=""350"">
    <nav:NavigationView.Header>
        <TextBlock Text=""My App"" FontWeight=""Bold"" Margin=""16,8""/>
    </nav:NavigationView.Header>
    <!-- ItemsSource bound to navigation items -->
    <TextBlock Text=""Main content area"" Margin=""24""/>
</nav:NavigationView>");
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "DisplayMode", Type = "NavigationViewDisplayMode", Default = "Compact", Description = "Minimal, Compact, or Expanded" },
        new ApiProperty { PropertyName = "IsPaneOpen", Type = "bool", Default = "true", Description = "Whether the side pane is open" },
        new ApiProperty { PropertyName = "Header", Type = "object", Default = "null", Description = "Header content above items" },
        new ApiProperty { PropertyName = "ItemsSource", Type = "IEnumerable", Default = "null", Description = "Menu items source" },
        new ApiProperty { PropertyName = "FooterItemsSource", Type = "IEnumerable", Default = "null", Description = "Footer items source" },
        new ApiProperty { PropertyName = "IsSettingsVisible", Type = "bool", Default = "true", Description = "Show settings item" },
        new ApiProperty { PropertyName = "IsBackEnabled", Type = "bool", Default = "false", Description = "Show back button" },
        new ApiProperty { PropertyName = "SelectedItem", Type = "object", Default = "null", Description = "Currently selected item" },
    };
}
