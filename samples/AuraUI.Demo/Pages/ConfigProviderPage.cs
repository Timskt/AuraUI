using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Layout;
using AuraUI.Controls.Input;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class ConfigProviderPage : ComponentPageBase
{
    public override string ComponentName => "ConfigProvider";
    public override string Description => "Provides global configuration for all AuraUI controls within its scope. Set theme, locale, primary color, and control size to cascade to descendants.";
    public override string Category => "Layout";

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
                    "Use ConfigProvider at the application root or section level to apply consistent theming and configuration across multiple controls.",
                    "Place at the outermost level for global config. Nest for section-specific overrides. Use for locale and accessibility settings.")
            }
        };
    }

    private Control BuildBasicExample()
    {
        var defaultProvider = new ConfigProvider
        {
            Content = new StackPanel
            {
                Spacing = 8,
                Children =
                {
                    new Avalonia.Controls.TextBlock { Text = "Default Config", FontWeight = FontWeight.SemiBold },
                    new AuraButton { Content = "Default Button", Classes = { "primary" } },
                    new AuraTextBox { PlaceholderText = "Default size input" }
                }
            }
        };

        var smallProvider = new ConfigProvider
        {
            DefaultControlSize = ControlSize.Small,
            Content = new StackPanel
            {
                Spacing = 8,
                Children =
                {
                    new Avalonia.Controls.TextBlock { Text = "Small Config", FontWeight = FontWeight.SemiBold },
                    new AuraButton { Content = "Small Button", Classes = { "primary" } },
                    new AuraTextBox { PlaceholderText = "Small input" }
                }
            }
        };

        var wrap = new WrapPanel();
        wrap.Children.Add(new Border
        {
            BorderBrush = GetBrush("AuraBorderBrush", "#E0E0E0"),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(8),
            Padding = new Thickness(16),
            Margin = new Thickness(0, 0, 12, 12),
            Child = defaultProvider
        });
        wrap.Children.Add(new Border
        {
            BorderBrush = GetBrush("AuraBorderBrush", "#E0E0E0"),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(8),
            Padding = new Thickness(16),
            Margin = new Thickness(0, 0, 12, 12),
            Child = smallProvider
        });

        return CreateExampleSection("Config Provider", wrap,
            @"<layout:ConfigProvider>
    <StackPanel Spacing=""8"">
        <TextBlock Text=""Default Config""/>
        <Button Content=""Default Button"" Classes=""primary""/>
    </StackPanel>
</layout:ConfigProvider>

<layout:ConfigProvider DefaultControlSize=""Small"">
    <StackPanel Spacing=""8"">
        <TextBlock Text=""Small Config""/>
        <Button Content=""Small Button"" Classes=""primary""/>
    </StackPanel>
</layout:ConfigProvider>");
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Theme", Type = "ConfigProviderTheme", Default = "Auto", Description = "Theme: Light, Dark, or Auto" },
        new ApiProperty { PropertyName = "Locale", Type = "string", Default = "en-US", Description = "Locale for i18n" },
        new ApiProperty { PropertyName = "GlobalPrefix", Type = "string", Default = "aura", Description = "CSS class prefix" },
        new ApiProperty { PropertyName = "DefaultControlSize", Type = "ControlSize", Default = "Medium", Description = "Default size for controls" },
        new ApiProperty { PropertyName = "PrimaryColor", Type = "IBrush", Default = "null", Description = "Override primary accent color" },
        new ApiProperty { PropertyName = "DisabledGlobal", Type = "bool", Default = "false", Description = "Disable all descendant controls" },
    };
}
