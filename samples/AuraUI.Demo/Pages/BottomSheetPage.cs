using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Layout;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class BottomSheetPage : ComponentPageBase
{
    public override string ComponentName => "BottomSheet";
    public override string Description => "A mobile-style bottom sheet that slides up from the bottom with snap points, drag handle, and swipe-to-dismiss.";
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
                    "Use BottomSheet for mobile-friendly actions, filters, or detail views that don't require a full page navigation.",
                    "Provide snap points for partial and full expansion. Always show a drag handle. Support swipe-to-dismiss for natural mobile interaction.")
            }
        };
    }

    private Control BuildBasicExample()
    {
        var sheet = new BottomSheet
        {
            ShowHandle = true,
            CloseOnSwipeDown = true,
            Content = new StackPanel
            {
                Margin = new Thickness(20),
                Spacing = 16,
                Children =
                {
                    new TextBlock { Text = "Bottom Sheet Content", FontWeight = FontWeight.SemiBold, FontSize = 18 },
                    new TextBlock { Text = "This slides up from the bottom. Drag the handle to resize or swipe down to dismiss.", TextWrapping = TextWrapping.Wrap },
                    new Button { Content = "Close", Classes = { "primary" }, HorizontalAlignment = HorizontalAlignment.Left }
                }
            }
        };

        var btn = new Button { Content = "Open Bottom Sheet", Classes = { "primary" }, Margin = new Thickness(0, 0, 8, 8) };
        btn.Click += (_, _) => sheet.IsOpen = true;

        var grid = new Grid();
        grid.Children.Add(sheet);
        grid.Children.Add(btn);

        return CreateExampleSection("Bottom Sheet", grid,
            @"<Button Content=""Open Bottom Sheet"" Classes=""primary""
        Click=""OpenSheet""/>

<layout:BottomSheet x:Name=""sheet"" ShowHandle=""True""
    CloseOnSwipeDown=""True"">
    <StackPanel Margin=""20"" Spacing=""16"">
        <TextBlock Text=""Bottom Sheet Content"" FontWeight=""SemiBold""/>
        <TextBlock Text=""Drag handle to resize."" TextWrapping=""Wrap""/>
    </StackPanel>
</layout:BottomSheet>",
            @"sheet.IsOpen = true;  // open
sheet.IsOpen = false; // close");
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "IsOpen", Type = "bool", Default = "false", Description = "Whether the sheet is visible" },
        new ApiProperty { PropertyName = "SnapPoints", Type = "AvaloniaList<double>", Default = "null", Description = "Heights the sheet can snap to" },
        new ApiProperty { PropertyName = "CurrentSnapPoint", Type = "int", Default = "0", Description = "Current snap point index" },
        new ApiProperty { PropertyName = "ShowHandle", Type = "bool", Default = "true", Description = "Show drag handle bar" },
        new ApiProperty { PropertyName = "CloseOnSwipeDown", Type = "bool", Default = "true", Description = "Swipe down past minimum to close" },
        new ApiProperty { PropertyName = "OverlayBrush", Type = "IBrush", Default = "null", Description = "Background overlay brush" },
    };
}
