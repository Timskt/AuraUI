using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using AuraUI.Controls.Feedback;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class MessengerPage : ComponentPageBase
{
    public override string ComponentName => "AuraMessage";
    public override string Description => "A lightweight top-center notification message for brief inline feedback. Similar to Toast but positioned at the top-center of the screen.";
    public override string Category => "Feedback";

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
                    "Use AuraMessage for brief feedback messages that don't require user action, such as success confirmations, warnings, or info notices.",
                    "Keep messages short (1-2 lines). Use appropriate message type (success/warning/error/info). Set auto-dismiss duration. Support close button.")
            }
        };
    }

    private Control BuildBasicExample()
    {
        var wrap = new Avalonia.Controls.WrapPanel();

        var btnSuccess = new Avalonia.Controls.Button { Content = "Success", Classes = { "primary" }, Margin = new Thickness(0, 0, 8, 8) };
        btnSuccess.Click += (_, _) => AuraMessage.Show("Operation completed successfully!");

        var btnWarning = new Avalonia.Controls.Button { Content = "Warning", Classes = { "outline" }, Margin = new Thickness(0, 0, 8, 8) };
        btnWarning.Click += (_, _) => AuraMessage.Show("Please check your input.");

        var btnError = new Avalonia.Controls.Button { Content = "Error", Classes = { "outline" }, Margin = new Thickness(0, 0, 8, 8) };
        btnError.Click += (_, _) => AuraMessage.Show("Something went wrong.");

        var btnInfo = new Avalonia.Controls.Button { Content = "Info", Classes = { "outline" }, Margin = new Thickness(0, 0, 8, 8) };
        btnInfo.Click += (_, _) => AuraMessage.Show("Here is some information.");

        wrap.Children.Add(btnSuccess);
        wrap.Children.Add(btnWarning);
        wrap.Children.Add(btnError);
        wrap.Children.Add(btnInfo);

        return CreateExampleSection("Message Types", wrap,
            @"<!-- Static method usage -->
AuraMessage.Show(""Success!"", MessageBoxIcon.Success);
AuraMessage.Show(""Warning"", MessageBoxIcon.Warning);
AuraMessage.Show(""Error"", MessageBoxIcon.Error);
AuraMessage.Show(""Info"", MessageBoxIcon.Info);");
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Text", Type = "string", Default = "null", Description = "Message text" },
        new ApiProperty { PropertyName = "MessageType", Type = "MessageBoxIcon", Default = "Info", Description = "Success, Warning, Error, Info" },
        new ApiProperty { PropertyName = "Duration", Type = "TimeSpan", Default = "3s", Description = "Auto-dismiss duration" },
        new ApiProperty { PropertyName = "ShowClose", Type = "bool", Default = "true", Description = "Show close button" },
        new ApiProperty { PropertyName = "ShowIcon", Type = "bool", Default = "true", Description = "Show type icon" },
        new ApiProperty { PropertyName = "Center", Type = "bool", Default = "false", Description = "Center-align text" },
    };
}
