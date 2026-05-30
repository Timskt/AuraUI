using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using AuraUI.Controls.Display;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class MarkdownViewerPage : ComponentPageBase
{
    public override string ComponentName => "MarkdownViewer";
    public override string Description => "Renders Markdown text as formatted visual elements. Supports headings, bold, italic, code blocks, lists, links, tables, and blockquotes.";
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
                    "Use MarkdownViewer for documentation, README files, chat message formatting, and any content authored in Markdown.",
                    "Use dark theme for code-heavy content. Set appropriate base font size. Support click-to-copy for code blocks. Render tables responsively.")
            }
        };
    }

    private Control BuildBasicExample()
    {
        var viewer = new MarkdownViewer
        {
            Markdown = @"# Welcome to AuraUI

## Getting Started

AuraUI provides **35+ controls** for building modern desktop apps.

### Features

- Fluent & Material themes
- Dark mode support
- 70+ design tokens
- Responsive layouts

### Code Example

```csharp
var button = new AuraButton {
    Content = ""Click Me"",
    Classes = { ""primary"" }
};
```

> AuraUI makes desktop development productive and beautiful.

| Control | Category |
|---------|----------|
| Card | Layout |
| Button | Input |
| ComboBox | Selection |",
            Width = 500,
            Height = 400
        };

        return CreateExampleSection("Markdown Rendering", viewer,
            @"<display:MarkdownViewer Width=""500"" Height=""400""
    Markdown=""# Hello World
## Subtitle
**Bold** and *italic* text
- List item 1
- List item 2

> Blockquote text""/>");
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Markdown", Type = "string", Default = "null", Description = "Markdown source text" },
        new ApiProperty { PropertyName = "ViewerTheme", Type = "MarkdownTheme", Default = "Light", Description = "Light or Dark theme" },
        new ApiProperty { PropertyName = "BaseFontSize", Type = "double", Default = "14", Description = "Base font size" },
        new ApiProperty { PropertyName = "CodeFontFamily", Type = "FontFamily", Default = "Cascadia Code", Description = "Code block font" },
    };
}
