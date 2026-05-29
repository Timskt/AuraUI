using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Feedback;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class DialogPage : ComponentPageBase
{
    public override string ComponentName => "AuraDialog";
    public override string Description => "A dialog control with overlay, fade animation, modal behavior, and placement options (center, drawer).";
    public override string Category => "Feedback";

    private AuraDialog? _dialog;

    protected override Control BuildContent()
    {
        _dialog = new AuraDialog
        {
            DialogTitle = "Example Dialog",
            IsModal = true,
            CloseOnOverlay = true,
            DialogWidth = 480,
            Placement = DialogPlacement.Center,
            Content = new StackPanel
            {
                Spacing = 16,
                Margin = new Thickness(24),
                Children =
                {
                    new TextBlock { Text = "This dialog is declared inline and toggled via Show/Hide.", TextWrapping = TextWrapping.Wrap, FontSize = 14, Foreground = GetBrush("AuraForegroundBrush") },
                    new TextBox { Watermark = "Enter a value..." },
                    new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8, HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right, Children = {
                        new Button { Content = "Cancel" },
                        new Button { Content = "Submit", Classes = { "primary" } }
                    }}
                }
            }
        };

        var btnShow = new Button { Content = "Show Dialog", Classes = { "primary" }, Margin = new Thickness(0, 0, 8, 8) };
        btnShow.Click += (_, _) => _dialog?.Show();

        return new StackPanel
        {
            Spacing = 32,
            Children =
            {
                _dialog,
                CreateExampleSection("Dialog",
                    new WrapPanel { Children = { btnShow } },
                    @"<feedback:AuraDialog DialogTitle=""Example Dialog""
                       IsModal=""True""
                       CloseOnOverlay=""True""
                       DialogWidth=""480""
                       Placement=""Center"">
    <StackPanel Spacing=""16"" Margin=""24"">
        <TextBlock Text=""Dialog content here."" TextWrapping=""Wrap""/>
        <TextBox Watermark=""Enter a value...""/>
        <StackPanel Orientation=""Horizontal"" Spacing=""8""
                    HorizontalAlignment=""Right"">
            <Button Content=""Cancel""/>
            <Button Content=""Submit"" Classes=""primary""/>
        </StackPanel>
    </StackPanel>
</feedback:AuraDialog>",
                    @"dialog.Show();   // open
dialog.Hide();   // close"),
                CreateApiTable(GetApiProperties()),
                CreateGuidelines(
                    "Use dialogs for complex forms, detailed information, and actions that require user attention before proceeding.",
                    "Keep dialog content focused on a single task. Always provide a way to close. Use CloseOnOverlay for non-critical dialogs.")
            }
        };
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "DialogTitle", Type = "string", Default = "null", Description = "Dialog title text" },
        new ApiProperty { PropertyName = "IsModal", Type = "bool", Default = "true", Description = "Blocks interaction with background" },
        new ApiProperty { PropertyName = "IsOpen", Type = "bool", Default = "false", Description = "Whether the dialog is visible" },
        new ApiProperty { PropertyName = "CloseOnOverlay", Type = "bool", Default = "true", Description = "Close when clicking overlay" },
        new ApiProperty { PropertyName = "DialogWidth", Type = "double", Default = "480", Description = "Dialog width" },
        new ApiProperty { PropertyName = "Placement", Type = "DialogPlacement", Default = "Center", Description = "Placement: Center, DrawerLeft, DrawerRight, DrawerTop, DrawerBottom" },
    };
}
