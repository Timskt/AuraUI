using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using AuraUI.Controls.Feedback;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class MessageBoxPage : ComponentPageBase
{
    public override string ComponentName => "MessageBox";
    public override string Description => "A modal dialog for displaying messages, confirmations, and alerts with icon support.";
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
                    "Use MessageBox for important messages that require user acknowledgment or confirmation of destructive actions.",
                    "Use clear, concise titles and messages. Use appropriate icons for the message type. For confirmations, make the safe action the default.")
            }
        };
    }

    private Control BuildVariantsExample()
    {
        var btnInfo = new Button { Content = "Info MessageBox", Classes = { "primary" }, Margin = new Thickness(0, 0, 8, 8) };
        btnInfo.Click += async (_, _) =>
        {
            var owner = this.VisualRoot as Window;
            if (owner == null) return;
            await AuraMessageBox.ShowAsync(owner, "Information", "This is an AuraUI message box dialog.", MessageBoxButtons.OK, MessageBoxIcon.Info);
        };

        var btnConfirm = new Button { Content = "Confirmation", Classes = { "outline" }, Margin = new Thickness(0, 0, 8, 8) };
        btnConfirm.Click += async (_, _) =>
        {
            var owner = this.VisualRoot as Window;
            if (owner == null) return;
            var result = await AuraMessageBox.ShowAsync(owner, "Confirm Action", "Are you sure you want to proceed?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        };

        return CreateExampleSection("MessageBox Variants",
            new WrapPanel { Children = { btnInfo, btnConfirm } },
            @"<Button Content=""Info MessageBox"" Classes=""primary""
        Click=""ShowInfo""/>
<Button Content=""Confirmation"" Classes=""outline""
        Click=""ShowConfirm""/>",
            @"private async void ShowInfo(object? sender, RoutedEventArgs e)
{
    await AuraMessageBox.ShowAsync(
        this, ""Information"",
        ""This is an AuraUI message box dialog."",
        MessageBoxButtons.OK, MessageBoxIcon.Info);
}

private async void ShowConfirm(object? sender, RoutedEventArgs e)
{
    var result = await AuraMessageBox.ShowAsync(
        this, ""Confirm Action"",
        ""Are you sure you want to proceed?"",
        MessageBoxButtons.YesNo, MessageBoxIcon.Question);
}");
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "title", Type = "string", Default = "", Description = "Dialog title" },
        new ApiProperty { PropertyName = "message", Type = "string", Default = "", Description = "Dialog message" },
        new ApiProperty { PropertyName = "buttons", Type = "MessageBoxButtons", Default = "OK", Description = "Button configuration" },
        new ApiProperty { PropertyName = "icon", Type = "MessageBoxIcon", Default = "None", Description = "Icon type: Info, Warning, Error, Question" },
    };
}
