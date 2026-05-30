using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using AuraUI.Controls.Display;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class CodeEditorPage : ComponentPageBase
{
    public override string ComponentName => "CodeEditor";
    public override string Description => "A code editor with syntax highlighting, line numbers, and copy functionality. Supports multiple programming languages and light/dark themes.";
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
                    "Use CodeEditor for code snippets, configuration editors, documentation with code examples, and developer-facing tools.",
                    "Set the correct language for syntax highlighting. Use dark theme for code. Enable line numbers for longer snippets. Provide a copy button.")
            }
        };
    }

    private Control BuildBasicExample()
    {
        var editor = new CodeEditor
        {
            Language = CodeLanguage.CSharp,
            Theme = CodeEditorTheme.Dark,
            IsReadOnly = true,
            ShowLineNumbers = true,
            Width = 600,
            Height = 250,
            Code = @"public class HelloWorld
{
    public static void Main(string[] args)
    {
        Console.WriteLine(""Hello, AuraUI!"");

        var list = new List<int> { 1, 2, 3 };
        foreach (var item in list)
        {
            Console.WriteLine(item);
        }
    }
}"
        };

        return CreateExampleSection("C# Code Editor", editor,
            @"<display:CodeEditor Language=""CSharp"" Theme=""Dark""
    IsReadOnly=""True"" ShowLineNumbers=""True""
    Width=""600"" Height=""250""
    Code=""public class HelloWorld { ... }""/>");
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Code", Type = "string", Default = "null", Description = "Source code text" },
        new ApiProperty { PropertyName = "Language", Type = "CodeLanguage", Default = "PlainText", Description = "Language for highlighting" },
        new ApiProperty { PropertyName = "Theme", Type = "CodeEditorTheme", Default = "Light", Description = "Light or Dark theme" },
        new ApiProperty { PropertyName = "IsReadOnly", Type = "bool", Default = "false", Description = "Read-only mode" },
        new ApiProperty { PropertyName = "ShowLineNumbers", Type = "bool", Default = "true", Description = "Show line numbers" },
    };
}
