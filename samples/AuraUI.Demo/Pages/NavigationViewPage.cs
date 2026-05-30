using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
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
        // Standard Avalonia fallback for NavigationView (custom control template not yet available)
        var contentText = new TextBlock
        {
            Text = "Home - Main content area. Select a navigation item from the pane.",
            Margin = new Thickness(24),
            TextWrapping = TextWrapping.Wrap,
            FontSize = 14,
            Foreground = GetBrush("AuraForegroundBrush", "#000000")
        };

        var navItems = new ListBox
        {
            Width = 200,
            SelectedIndex = 0
        };
        foreach (var item in new[] { "Home", "Documents", "Settings", "About" })
            navItems.Items.Add(new ListBoxItem { Content = item, Padding = new Thickness(12, 8) });

        navItems.SelectionChanged += (_, _) =>
        {
            if (navItems.SelectedItem is ListBoxItem li)
                contentText.Text = $"{li.Content} - Main content area. Select a navigation item from the pane.";
        };

        var headerBorder = new Border
        {
            Padding = new Thickness(16, 12),
            Child = new TextBlock
            {
                Text = "My App",
                FontWeight = FontWeight.Bold,
                FontSize = 16,
                Foreground = GetBrush("AuraForegroundBrush", "#000000")
            }
        };
        DockPanel.SetDock(headerBorder, Dock.Top);

        var footerBorder = new Border
        {
            Padding = new Thickness(12, 8),
            Child = new TextBlock
            {
                Text = "Settings",
                FontSize = 13,
                Foreground = GetBrush("AuraForegroundSecondaryBrush", "#666666")
            }
        };
        DockPanel.SetDock(footerBorder, Dock.Bottom);

        var navView = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("Auto,*"),
            Width = 600,
            Height = 350,
            Clip = new RectangleGeometry(new Rect(0, 0, 600, 350)),
            Children =
            {
                new Border
                {
                    Background = GetBrush("AuraSurfaceBrush", "#F9F9F9"),
                    BorderBrush = GetBrush("AuraBorderBrush", "#E0E0E0"),
                    BorderThickness = new Thickness(0, 0, 1, 0),
                    Width = 200,
                    Child = new DockPanel
                    {
                        Children =
                        {
                            headerBorder,
                            footerBorder,
                            navItems
                        }
                    }
                },
                SetColumn(new Border
                {
                    Background = GetBrush("AuraCardBrush", "#FFFFFF"),
                    Child = contentText
                }, 1)
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

    private static T SetColumn<T>(T control, int column) where T : Control
    {
        Grid.SetColumn(control, column);
        return control;
    }
}
