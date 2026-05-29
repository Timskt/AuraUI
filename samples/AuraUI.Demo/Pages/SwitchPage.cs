using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using AuraUI.Controls.Selection;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class SwitchPage : ComponentPageBase
{
    public override string ComponentName => "Switch";
    public override string Description => "A toggle switch control with smooth thumb transition animation. Supports small, medium, and large size variants.";
    public override string Category => "Selection";

    protected override Control BuildContent()
    {
        return new StackPanel
        {
            Spacing = 32,
            Children =
            {
                BuildBasicExample(),
                BuildSizesExample(),
                CreateApiTable(GetApiProperties()),
                CreateGuidelines(
                    "Use switches for binary on/off settings that take effect immediately. They are ideal for toggling features, modes, and preferences.",
                    "Use switches instead of checkboxes for settings that apply instantly. Place the label to the right of the switch. Use consistent sizes within a settings panel.")
            }
        };
    }

    private Control BuildBasicExample()
    {
        return CreateExampleSection("Basic Switch",
            new StackPanel
            {
                Spacing = 12,
                Children =
                {
                    CreateSwitchRow("Wi-Fi", true),
                    CreateSwitchRow("Bluetooth", false),
                    CreateSwitchRow("Airplane Mode", false),
                }
            },
            @"<StackPanel Orientation=""Horizontal"" Spacing=""12"">
    <selection:Switch IsChecked=""True""/>
    <TextBlock Text=""Wi-Fi"" VerticalAlignment=""Center""/>
</StackPanel>
<StackPanel Orientation=""Horizontal"" Spacing=""12"">
    <selection:Switch/>
    <TextBlock Text=""Bluetooth"" VerticalAlignment=""Center""/>
</StackPanel>");
    }

    private Control BuildSizesExample()
    {
        var panel = new StackPanel { Spacing = 12 };
        foreach (var size in new[] { SwitchSize.Small, SwitchSize.Medium, SwitchSize.Large })
        {
            panel.Children.Add(new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Spacing = 12,
                Children =
                {
                    new Switch { Size = size, IsChecked = true },
                    new TextBlock { Text = size.ToString(), VerticalAlignment = VerticalAlignment.Center, Foreground = GetBrush("AuraForegroundBrush") }
                }
            });
        }
        return CreateExampleSection("Sizes", panel,
            @"<selection:Switch Size=""Small"" IsChecked=""True""/>
<selection:Switch Size=""Medium"" IsChecked=""True""/>
<selection:Switch Size=""Large"" IsChecked=""True""/>");
    }

    private Control CreateSwitchRow(string label, bool isChecked)
    {
        return new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 12,
            Children =
            {
                new Switch { IsChecked = isChecked },
                new TextBlock { Text = label, VerticalAlignment = VerticalAlignment.Center, Foreground = GetBrush("AuraForegroundBrush") }
            }
        };
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "IsChecked", Type = "bool?", Default = "false", Description = "Whether the switch is on" },
        new ApiProperty { PropertyName = "Size", Type = "SwitchSize", Default = "Medium", Description = "Size variant: Small, Medium, Large" },
        new ApiProperty { PropertyName = "OnContent", Type = "string", Default = "null", Description = "Text displayed when on" },
        new ApiProperty { PropertyName = "OffContent", Type = "string", Default = "null", Description = "Text displayed when off" },
        new ApiProperty { PropertyName = "TransitionDuration", Type = "TimeSpan", Default = "200ms", Description = "Thumb animation duration" },
    };
}
