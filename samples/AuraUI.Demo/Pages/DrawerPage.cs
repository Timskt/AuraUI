using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Layout;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class DrawerPage : ComponentPageBase
{
    public override string ComponentName => "Drawer";
    public override string Description => "A slide-in panel overlay from any edge with animation and overlay backdrop.";
    public override string Category => "Layout";

    protected override Control BuildContent()
    {
        return new StackPanel
        {
            Spacing = 32,
            Children =
            {
                BuildPlacementExample(),
                CreateApiTable(GetApiProperties()),
                CreateGuidelines(
                    "Use drawers for supplementary content like filters, settings, or details that don't need a full page. They slide in from the edge.",
                    "Use right placement for detail panels, left for navigation. Always provide a close mechanism. Use overlay for modal drawers.")
            }
        };
    }

    private Control BuildPlacementExample()
    {
        var rightDrawer = new Drawer { Placement = AuraUI.Controls.Layout.DrawerPlacement.Right, DrawerWidth = 300, Content = new StackPanel { Margin = new Thickness(20), Spacing = 12, Children = { new TextBlock { Text = "Right Drawer", FontWeight = FontWeight.SemiBold, FontSize = 18, Foreground = GetBrush("AuraForegroundBrush") }, new TextBlock { Text = "Drawer content goes here.", TextWrapping = TextWrapping.Wrap, Foreground = GetBrush("AuraForegroundSecondaryBrush", "#666666") } } } };
        var leftDrawer = new Drawer { Placement = AuraUI.Controls.Layout.DrawerPlacement.Left, DrawerWidth = 280, Content = new StackPanel { Margin = new Thickness(20), Spacing = 12, Children = { new TextBlock { Text = "Left Drawer", FontWeight = FontWeight.SemiBold, FontSize = 18, Foreground = GetBrush("AuraForegroundBrush") }, new TextBlock { Text = "Navigation or filter content.", TextWrapping = TextWrapping.Wrap, Foreground = GetBrush("AuraForegroundSecondaryBrush", "#666666") } } } };

        var btnRight = new Button { Content = "Open Right Drawer", Classes = { "primary" }, Margin = new Thickness(0, 0, 8, 8) };
        btnRight.Click += (_, _) => rightDrawer.IsOpen = true;

        var btnLeft = new Button { Content = "Open Left Drawer", Classes = { "outline" }, Margin = new Thickness(0, 0, 8, 8) };
        btnLeft.Click += (_, _) => leftDrawer.IsOpen = true;

        var panel = new Grid();
        panel.Children.Add(rightDrawer);
        panel.Children.Add(leftDrawer);
        panel.Children.Add(new WrapPanel { Children = { btnRight, btnLeft } });

        return CreateExampleSection("Drawer Placements", panel,
            @"<Button Content=""Open Right Drawer"" Classes=""primary""
        Click=""OpenRightDrawer""/>
<Button Content=""Open Left Drawer"" Classes=""outline""
        Click=""OpenLeftDrawer""/>

<layout:Drawer Placement=""Right"" DrawerWidth=""300"">
    <StackPanel Margin=""20"" Spacing=""12"">
        <TextBlock Text=""Right Drawer"" FontWeight=""SemiBold""/>
        <TextBlock Text=""Content here."" TextWrapping=""Wrap""/>
    </StackPanel>
</layout:Drawer>",
            @"rightDrawer.IsOpen = true;  // open
rightDrawer.IsOpen = false; // close");
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "IsOpen", Type = "bool", Default = "false", Description = "Whether the drawer is visible" },
        new ApiProperty { PropertyName = "Placement", Type = "DrawerPlacement", Default = "Right", Description = "Edge: Left, Right, Top, Bottom" },
        new ApiProperty { PropertyName = "DrawerWidth", Type = "double", Default = "320", Description = "Width (for Left/Right)" },
        new ApiProperty { PropertyName = "CloseOnOverlayClick", Type = "bool", Default = "true", Description = "Close on overlay click" },
        new ApiProperty { PropertyName = "AnimationDuration", Type = "TimeSpan", Default = "250ms", Description = "Slide animation duration" },
    };
}
