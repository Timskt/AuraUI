using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using AuraUI.Controls.Feedback;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class NotificationPage : ComponentPageBase
{
    public override string ComponentName => "Notification";
    public override string Description => "Rich notification cards with title, message, icon, actions, and auto-dismiss.";
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
                    "Use notifications for important messages that the user should notice but that don't block interaction. They support rich content and actions.",
                    "Keep titles short and descriptive. Provide action buttons for notifications that require follow-up. Use persistent notifications sparingly.")
            }
        };
    }

    private Control BuildVariantsExample()
    {
        var btnInfo = new Button { Content = "Info Notification", Classes = { "primary" }, Margin = new Thickness(0, 0, 8, 8) };
        btnInfo.Click += (_, _) => AuraNotification.Info("New Message", "You have received a new message from the team.");

        var btnPersistent = new Button { Content = "With Action", Classes = { "outline" }, Margin = new Thickness(0, 0, 8, 8) };
        btnPersistent.Click += (_, _) =>
        {
            var actions = new List<NotificationAction>
            {
                new() { Label = "View", Callback = () => { } }
            };
            AuraNotification.Show("Update Available", "A new version is available.", MessageBoxIcon.Info, actions, System.TimeSpan.FromSeconds(10));
        };

        return CreateExampleSection("Notification Variants",
            new WrapPanel { Children = { btnInfo, btnPersistent } },
            @"<Button Content=""Info Notification"" Classes=""primary""
        Click=""ShowNotification""/>
<Button Content=""With Action"" Classes=""outline""
        Click=""ShowPersistentNotification""/>",
            @"private void ShowNotification(object? sender, RoutedEventArgs e)
{
    AuraNotification.Info(""New Message"",
        ""You have received a new message."");
}

private void ShowPersistentNotification(object? sender, RoutedEventArgs e)
{
    var actions = new List<NotificationAction>
    {
        new() { Label = ""View"", Callback = () => { } }
    };
    AuraNotification.Show(""Update Available"",
        ""A new version is available."",
        MessageBoxIcon.Info, actions, TimeSpan.FromSeconds(10));
}");
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "title", Type = "string", Default = "", Description = "Notification title" },
        new ApiProperty { PropertyName = "message", Type = "string", Default = "", Description = "Notification message" },
        new ApiProperty { PropertyName = "icon", Type = "MessageBoxIcon", Default = "None", Description = "Icon type" },
        new ApiProperty { PropertyName = "actions", Type = "List<NotificationAction>", Default = "null", Description = "Action buttons" },
        new ApiProperty { PropertyName = "timeout", Type = "TimeSpan", Default = "5s", Description = "Auto-dismiss duration" },
    };
}
