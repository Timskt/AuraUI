using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using AuraUI.Controls.Feedback;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class SnackbarPage : ComponentPageBase
{
    public override string ComponentName => "Snackbar";
    public override string Description => "A brief message that appears at the bottom of the screen with optional action button.";
    public override string Category => "Feedback";

    protected override Control BuildContent()
    {
        return new StackPanel
        {
            Spacing = 32,
            Children =
            {
                BuildVariantsExample(),
                CreateApiTable(GetApiProperties()),
                CreateGuidelines(
                    "Use snackbars for brief confirmations of user actions, such as item deletion or file save. They appear at the bottom and auto-dismiss.",
                    "Include an action button for reversible operations (e.g., Undo). Keep messages under 1 line. Don't use for errors that require attention.")
            }
        };
    }

    private Control BuildVariantsExample()
    {
        var btnBasic = new Button { Content = "Show Snackbar", Classes = { "primary" }, Margin = new Avalonia.Thickness(0, 0, 8, 8) };
        btnBasic.Click += (_, _) => Snackbar.Show("Item deleted successfully.");

        var btnAction = new Button { Content = "With Action", Classes = { "outline" }, Margin = new Avalonia.Thickness(0, 0, 8, 8) };
        btnAction.Click += (_, _) => Snackbar.Show("File deleted.", "Undo", System.TimeSpan.FromSeconds(5), () => { });

        return CreateExampleSection("Snackbar Variants",
            new WrapPanel { Children = { btnBasic, btnAction } },
            @"<Button Content=""Show Snackbar"" Classes=""primary""
        Click=""ShowSnackbar""/>
<Button Content=""With Action"" Classes=""outline""
        Click=""ShowSnackbarWithAction""/>",
            @"private void ShowSnackbar(object? sender, RoutedEventArgs e)
{
    Snackbar.Show(""Item deleted successfully."");
}

private void ShowSnackbarWithAction(object? sender, RoutedEventArgs e)
{
    Snackbar.Show(""File deleted."", ""Undo"", TimeSpan.FromSeconds(5), () =>
    {
        // Undo action
    });
}");
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "message", Type = "string", Default = "", Description = "Snackbar message" },
        new ApiProperty { PropertyName = "actionText", Type = "string", Default = "null", Description = "Action button text" },
        new ApiProperty { PropertyName = "timeout", Type = "TimeSpan", Default = "3s", Description = "Auto-dismiss duration" },
        new ApiProperty { PropertyName = "action", Type = "Action", Default = "null", Description = "Action callback" },
    };
}
