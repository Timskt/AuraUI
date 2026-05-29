using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using AuraUI.Controls.Layout;
using AuraUI.Demo.Models;
using Expander = AuraUI.Controls.Layout.Expander;

namespace AuraUI.Demo.Pages;

public class ExpanderPage : ComponentPageBase
{
    public override string ComponentName => "Expander";
    public override string Description => "A collapsible panel that shows/hides content with smooth animation.";
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
                    "Use expanders to hide secondary content and reduce visual clutter. They work well for FAQ sections, advanced settings, and detail views.",
                    "Use clear headers that indicate what will be revealed. Avoid nesting expanders deeply. Default to collapsed unless the content is commonly needed.")
            }
        };
    }

    private Control BuildBasicExample()
    {
        return CreateExampleSection("Expander",
            new StackPanel
            {
                Spacing = 8, MaxWidth = 500,
                Children =
                {
                    new Expander { Header = "Click to expand", IsExpanded = false, Content = new TextBlock { Text = "This content expands and collapses with smooth animation.", TextWrapping = TextWrapping.Wrap, Margin = new Thickness(16, 8), Foreground = GetBrush("AuraForegroundBrush") } },
                    new Expander { Header = "Expanded by default", IsExpanded = true, Content = new TextBlock { Text = "Use IsExpanded to control initial visibility.", TextWrapping = TextWrapping.Wrap, Margin = new Thickness(16, 8), Foreground = GetBrush("AuraForegroundBrush") } },
                }
            },
            @"<layout:Expander Header=""Click to expand"" IsExpanded=""False"">
    <TextBlock Text=""This content expands and collapses.""
               TextWrapping=""Wrap"" Margin=""16,8""/>
</layout:Expander>
<layout:Expander Header=""Expanded by default"" IsExpanded=""True"">
    <TextBlock Text=""Use IsExpanded to control initial visibility.""
               TextWrapping=""Wrap"" Margin=""16,8""/>
</layout:Expander>");
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Header", Type = "object", Default = "null", Description = "Content displayed in the header area" },
        new ApiProperty { PropertyName = "IsExpanded", Type = "bool", Default = "false", Description = "Whether the content is visible" },
        new ApiProperty { PropertyName = "Content", Type = "object", Default = "null", Description = "The expandable content" },
    };
}
