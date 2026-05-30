using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using AuraUI.Controls.Display;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class TerminalPage : ComponentPageBase
{
    public override string ComponentName => "Terminal";
    public override string Description => "A terminal/console control with command input, output display, and scroll history. Supports command history navigation and keyboard shortcuts.";
    public override string Category => "Display";

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
                    "Use Terminal for developer tools, SSH consoles, REPL interfaces, log viewers, and interactive command-line experiences.",
                    "Support command history (up/down arrows). Auto-scroll to bottom. Use monospace font. Provide a clear command. Show the prompt prominently.")
            }
        };
    }

    private Control BuildBasicExample()
    {
        var terminal = new Terminal
        {
            Prompt = "$ ",
            MaxLines = 1000,
            IsInputEnabled = true,
            Width = 600,
            Height = 300,
            Lines = new List<string>
            {
                "Welcome to AuraUI Terminal",
                "Type 'help' for available commands",
                "",
                "$ echo Hello World",
                "Hello World",
                "",
                "$ ls -la",
                "total 48",
                "drwxr-xr-x  5 user  staff  160 Jan  1 12:00 .",
                "drwxr-xr-x  3 user  staff   96 Jan  1 12:00 ..",
                "-rw-r--r--  1 user  staff  220 Jan  1 12:00 readme.md",
            }
        };

        return CreateExampleSection("Interactive Terminal", terminal,
            @"<display:Terminal Prompt=""$ "" MaxLines=""1000""
    IsReadOnly=""False"" Width=""600"" Height=""300""/>");
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Lines", Type = "IList<string>", Default = "null", Description = "Output lines" },
        new ApiProperty { PropertyName = "Prompt", Type = "string", Default = "> ", Description = "Input prompt text" },
        new ApiProperty { PropertyName = "InputCommand", Type = "string", Default = "null", Description = "Current input text" },
        new ApiProperty { PropertyName = "MaxLines", Type = "int", Default = "5000", Description = "Max output lines" },
        new ApiProperty { PropertyName = "IsReadOnly", Type = "bool", Default = "false", Description = "Disable input" },
        new ApiProperty { PropertyName = "FontFamily", Type = "FontFamily", Default = "monospace", Description = "Terminal font" },
    };
}
