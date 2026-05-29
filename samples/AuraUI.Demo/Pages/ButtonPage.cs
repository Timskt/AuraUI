using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class ButtonPage : ComponentPageBase
{
    public override string ComponentName => "Button";
    public override string Description => "A clickable button with multiple variants, sizes, icon support, and loading state.";
    public override string Category => "Input";

    protected override Control BuildContent()
    {
        return new StackPanel
        {
            Spacing = 32,
            Children =
            {
                BuildVariantsExample(),
                BuildSizesExample(),
                BuildStatesExample(),
                BuildLoadingExample(),
                CreateApiTable(GetApiProperties()),
                CreateGuidelines(
                    "Use buttons for primary actions in forms, dialogs, and toolbars. Use primary variant for the main action and outline/ghost for secondary actions.",
                    "Keep button text concise (1-3 words). Use consistent button styles across the application. Avoid using too many primary buttons in one view.")
            }
        };
    }

    private Control BuildVariantsExample()
    {
        return CreateExampleSection("Variants",
            new WrapPanel
            {
                Children =
                {
                    new Button { Content = "Default", Margin = new Thickness(0, 0, 8, 8) },
                    new Button { Content = "Primary", Classes = { "primary" }, Margin = new Thickness(0, 0, 8, 8) },
                    new Button { Content = "Accent", Classes = { "accent" }, Margin = new Thickness(0, 0, 8, 8) },
                    new Button { Content = "Outline", Classes = { "outline" }, Margin = new Thickness(0, 0, 8, 8) },
                    new Button { Content = "Ghost", Classes = { "ghost" }, Margin = new Thickness(0, 0, 8, 8) },
                    new Button { Content = "Destructive", Classes = { "destructive" }, Margin = new Thickness(0, 0, 8, 8) },
                    new Button { Content = "Success", Classes = { "success" }, Margin = new Thickness(0, 0, 8, 8) },
                    new Button { Content = "Warning", Classes = { "warning" }, Margin = new Thickness(0, 0, 8, 8) },
                    new Button { Content = "Link", Classes = { "link" }, Margin = new Thickness(0, 0, 8, 8) },
                }
            },
            @"<Button Content=""Default""/>
<Button Content=""Primary"" Classes=""primary""/>
<Button Content=""Accent"" Classes=""accent""/>
<Button Content=""Outline"" Classes=""outline""/>
<Button Content=""Ghost"" Classes=""ghost""/>
<Button Content=""Destructive"" Classes=""destructive""/>
<Button Content=""Success"" Classes=""success""/>
<Button Content=""Warning"" Classes=""warning""/>
<Button Content=""Link"" Classes=""link""/>");
    }

    private Control BuildSizesExample()
    {
        return CreateExampleSection("Sizes",
            new WrapPanel
            {
                VerticalAlignment = VerticalAlignment.Center,
                Children =
                {
                    new Button { Content = "Small", Classes = { "primary", "sm" }, Margin = new Thickness(0, 0, 8, 8) },
                    new Button { Content = "Medium", Classes = { "primary" }, Margin = new Thickness(0, 0, 8, 8) },
                    new Button { Content = "Large", Classes = { "primary", "lg" }, Margin = new Thickness(0, 0, 8, 8) },
                    new Button { Content = "Pill", Classes = { "primary", "rounded" }, Margin = new Thickness(0, 0, 8, 8) },
                }
            },
            @"<Button Content=""Small"" Classes=""primary sm""/>
<Button Content=""Medium"" Classes=""primary""/>
<Button Content=""Large"" Classes=""primary lg""/>
<Button Content=""Pill"" Classes=""primary rounded""/>");
    }

    private Control BuildStatesExample()
    {
        return CreateExampleSection("States",
            new WrapPanel
            {
                Children =
                {
                    new Button { Content = "Normal", Classes = { "primary" }, Margin = new Thickness(0, 0, 8, 8) },
                    new Button { Content = "Disabled", Classes = { "primary" }, IsEnabled = false, Margin = new Thickness(0, 0, 8, 8) },
                    new Button { Content = "Disabled", IsEnabled = false, Margin = new Thickness(0, 0, 8, 8) },
                }
            },
            @"<Button Content=""Normal"" Classes=""primary""/>
<Button Content=""Disabled"" Classes=""primary"" IsEnabled=""False""/>
<Button Content=""Disabled"" IsEnabled=""False""/>");
    }

    private Control BuildLoadingExample()
    {
        var btn = new Avalonia.Controls.Button { Content = "Click to Load", Classes = { "primary" } };
        btn.Click += async (_, _) =>
        {
            btn.Classes.Add("loading");
            btn.Content = "Loading...";
            btn.IsEnabled = false;
            await System.Threading.Tasks.Task.Delay(2000);
            btn.Classes.Remove("loading");
            btn.Content = "Click to Load";
            btn.IsEnabled = true;
        };

        return CreateExampleSection("Loading State", btn,
            @"<Button Content=""Click to Load""
        Classes=""primary""
        Click=""LoadingButton_Click""/>",
            @"private async void LoadingButton_Click(object? sender, RoutedEventArgs e)
{
    if (sender is Button button)
    {
        button.Classes.Add(""loading"");
        button.Content = ""Loading..."";
        await Task.Delay(2000);
        button.Classes.Remove(""loading"");
        button.Content = ""Click to Load"";
    }
}");
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Content", Type = "object", Default = "null", Description = "The button content (text or control)" },
        new ApiProperty { PropertyName = "IsEnabled", Type = "bool", Default = "true", Description = "Whether the button is interactive" },
        new ApiProperty { PropertyName = "Classes", Type = "Classes", Default = "", Description = "Style classes: primary, accent, outline, ghost, destructive, success, warning, link, sm, lg, rounded, loading" },
    };
}
