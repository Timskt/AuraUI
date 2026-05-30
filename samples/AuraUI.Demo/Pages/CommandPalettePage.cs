using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Navigation;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class CommandPalettePage : ComponentPageBase
{
    public override string ComponentName => "CommandPalette";
    public override string Description => "Modal command palette with fuzzy search.";
    public override string Category => "Navigation";

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
                    "Use for power-user features and app-wide search.",
                    "Organize by category.")
            }
        };
    }

    private Control BuildExample1()
    {
        return CreateExampleSection("Command Palette", BuildPreview1(),
            @"<Navigation:CommandPalette IsOpen=""{Binding Open}"" Commands=""{Binding Commands}""/>",
            @"var cmds = new List<CommandItem> { new() { Name = ""Toggle Dark"", Category = ""View"" } };");
    }

    private Control BuildPreview1()
    {
        return new StackPanel { Spacing = 16, Children = { new TextBlock { Text = "Press Ctrl+Shift+P to open.", TextWrapping = TextWrapping.Wrap }, new Button { Content = "Open Palette", Classes = { "primary" }, Width = 200 } } };
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "IsOpen", Type = "bool", Default = "false", Description = "Is open" },
        new ApiProperty { PropertyName = "Commands", Type = "IList<CommandItem>?", Default = "null", Description = "Commands" },
        new ApiProperty { PropertyName = "SearchText", Type = "string?", Default = "null", Description = "Search query" },
        new ApiProperty { PropertyName = "Placeholder", Type = "string", Default = "Type a command...", Description = "Placeholder" },
    };
}
