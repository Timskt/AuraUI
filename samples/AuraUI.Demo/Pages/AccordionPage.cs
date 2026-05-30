using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Layout;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class AccordionPage : ComponentPageBase
{
    public override string ComponentName => "Accordion";
    public override string Description => "Multi-expand accordion panels where multiple items can be expanded simultaneously.";
    public override string Category => "Layout";

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
                    "Use accordion panels for FAQ sections and progressive disclosure.",
                    "Keep header text short. Default to collapsed.")
            }
        };
    }

    private Control BuildExample1()
    {
        return CreateExampleSection("Basic Accordion", BuildPreview1(),
            @"<StackPanel Spacing=""4"" MaxWidth=""500"">
    <layout:AccordionItem Header=""Getting Started"" IsExpanded=""True"">
        <TextBlock Text=""Welcome!"" TextWrapping=""Wrap"" Margin=""16,8""/>
    </layout:AccordionItem>
    <layout:AccordionItem Header=""Installation"">
        <TextBlock Text=""Install via NuGet."" TextWrapping=""Wrap"" Margin=""16,8""/>
    </layout:AccordionItem>
</StackPanel>",
            @"var item = new AccordionItem { Header = ""Dynamic"", IsExpanded = true };
item.Expanded += (s, e) => Console.WriteLine(""Expanded!"");");
    }

    private Control BuildPreview1()
    {
        return new StackPanel { Spacing = 4, MaxWidth = 500, Children = { new Avalonia.Controls.Expander { Header = "Getting Started", IsExpanded = true, Content = new TextBlock { Text = "Welcome to AuraUI!", TextWrapping = TextWrapping.Wrap, Margin = new Thickness(16, 8) } }, new Avalonia.Controls.Expander { Header = "Installation", IsExpanded = false, Content = new TextBlock { Text = "Install via NuGet.", TextWrapping = TextWrapping.Wrap, Margin = new Thickness(16, 8) } } } };
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Header", Type = "object", Default = "null", Description = "Header content" },
        new ApiProperty { PropertyName = "IsExpanded", Type = "bool", Default = "false", Description = "Whether expanded" },
        new ApiProperty { PropertyName = "Icon", Type = "object", Default = "null", Description = "Optional icon" },
        new ApiProperty { PropertyName = "ExpandAnimationDuration", Type = "TimeSpan", Default = "200ms", Description = "Animation duration" },
    };
}
