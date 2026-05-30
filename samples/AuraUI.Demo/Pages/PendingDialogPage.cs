using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using AuraUI.Controls.Feedback;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class PendingDialogPage : ComponentPageBase
{
    public override string ComponentName => "PendingDialog";
    public override string Description => "A pending/progress dialog that shows a spinner with a message and optional cancel button. Supports async patterns with automatic close.";
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
                    "Use PendingDialog for long-running operations like file uploads, data processing, API calls, or any task where the user should wait.",
                    "Always provide a clear message. Support cancellation for long operations. Close automatically when work completes. Use ShowWhileAsync for clean patterns.")
            }
        };
    }

    private Control BuildBasicExample()
    {
        var btn = new Avalonia.Controls.Button
        {
            Content = "Start Operation",
            Classes = { "primary" },
            Margin = new Thickness(0, 0, 0, 12)
        };

        var statusText = new Avalonia.Controls.TextBlock
        {
            Text = "Click the button to simulate a pending operation",
            TextWrapping = Avalonia.Media.TextWrapping.Wrap
        };

        btn.Click += async (_, _) =>
        {
            statusText.Text = "Operation started...";
            var workTask = System.Threading.Tasks.Task.Delay(3000);
            await PendingDialog.ShowWhileAsync("Processing your request...", workTask, cancellable: true);
            statusText.Text = "Operation completed!";
        };

        var panel = new StackPanel { Spacing = 12 };
        panel.Children.Add(btn);
        panel.Children.Add(statusText);

        return CreateExampleSection("Pending Dialog", panel,
            @"<Button Content=""Start Operation"" Classes=""primary""
        Click=""StartOperation""/>",
            @"// Auto-close when work completes
await PendingDialog.ShowWhileAsync(
    ""Processing your request..."",
    workTask,
    cancellable: true);

// Manual usage
await PendingDialog.ShowAsync(
    ""Please wait..."",
    cancellationToken);");
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "PendingMessage", Type = "string", Default = "null", Description = "Message with spinner" },
        new ApiProperty { PropertyName = "IsCancellable", Type = "bool", Default = "false", Description = "Show cancel button" },
        new ApiProperty { PropertyName = "CancellationToken", Type = "CancellationToken", Default = "None", Description = "Cancellation token" },
    };
}
