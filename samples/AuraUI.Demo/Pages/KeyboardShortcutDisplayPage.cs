using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Display;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class KeyboardShortcutDisplayPage : ComponentPageBase
{
    public override string ComponentName => "KeyboardShortcutDisplay";
    public override string Description => "Styled keyboard shortcut display.";
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
                    "Use in help menus and tooltips.",
                    "Use consistent sizing.")
            }
        };
    }

    private Control BuildExample1()
    {
        return CreateExampleSection("Shortcuts", BuildPreview1(),
            @"<display:KeyboardShortcutDisplay Shortcut=""Ctrl+C"" Size=""Small""/>");
    }

    private Control BuildPreview1()
    {
        return new StackPanel { Spacing = 16, Children = { new KeyboardShortcutDisplay { Shortcut = "Ctrl+C", Size = KeyboardShortcutSize.Small }, new KeyboardShortcutDisplay { Shortcut = "Ctrl+Shift+P", Size = KeyboardShortcutSize.Medium }, new KeyboardShortcutDisplay { Shortcut = "Alt+F4", Size = KeyboardShortcutSize.Large } } };
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Shortcut", Type = "string?", Default = "null", Description = "Shortcut string" },
        new ApiProperty { PropertyName = "Size", Type = "KeyboardShortcutSize", Default = "Medium", Description = "Small, Medium, Large" },
        new ApiProperty { PropertyName = "KeyBackground", Type = "IBrush?", Default = "null", Description = "Key background" },
        new ApiProperty { PropertyName = "KeyForeground", Type = "IBrush?", Default = "null", Description = "Key foreground" },
    };
}
