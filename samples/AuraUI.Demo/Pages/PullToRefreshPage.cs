using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using AuraUI.Controls.Layout;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class PullToRefreshPage : ComponentPageBase
{
    public override string ComponentName => "PullToRefresh";
    public override string Description => "A mobile-style pull-to-refresh container. Pull down past a threshold to trigger a refresh with a loading indicator.";
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
                    "Use PullToRefresh for mobile list views where users expect to pull down to reload content, such as feeds, lists, and dashboards.",
                    "Set an appropriate PullThreshold (60-100px). Show a clear loading indicator during refresh. Call CompleteRefresh when data is loaded.")
            }
        };
    }

    private Control BuildBasicExample()
    {
        var refreshContent = new StackPanel
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            Spacing = 4,
            Children =
            {
                new ProgressBar { IsIndeterminate = true, Width = 40, Height = 4 },
                new Avalonia.Controls.TextBlock { Text = "Refreshing...", FontSize = 12 }
            }
        };

        var listContent = new StackPanel { Spacing = 8, Margin = new Thickness(16) };
        for (int i = 1; i <= 8; i++)
        {
            listContent.Children.Add(new Border
            {
                Background = GetBrush("AuraMutedBrush", "#F0F0F0"),
                CornerRadius = new CornerRadius(6),
                Padding = new Thickness(12),
                Child = new Avalonia.Controls.TextBlock { Text = $"Item {i}" }
            });
        }

        var ptr = new PullToRefresh
        {
            PullThreshold = 80,
            RefreshContent = refreshContent,
            Content = new ScrollViewer
            {
                MaxWidth = 400,
                MaxHeight = 300,
                Content = listContent
            }
        };

        ptr.RefreshRequested += (_, _) =>
        {
            // Simulate async refresh
            Avalonia.Threading.Dispatcher.UIThread.Post(async () =>
            {
                await System.Threading.Tasks.Task.Delay(1500);
                ptr.CompleteRefresh();
            });
        };

        return CreateExampleSection("Pull to Refresh", ptr,
            @"<layout:PullToRefresh PullThreshold=""80"" RefreshRequested=""OnRefresh"">
    <layout:PullToRefresh.RefreshContent>
        <ProgressBar IsIndeterminate=""True"" Width=""40""/>
    </layout:PullToRefresh.RefreshContent>
    <ScrollViewer>
        <StackPanel Spacing=""8"">
            <!-- List items -->
        </StackPanel>
    </ScrollViewer>
</layout:PullToRefresh>",
            @"ptr.RefreshRequested += (_, _) => {
    // Load data...
    ptr.CompleteRefresh();
};");
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "IsRefreshing", Type = "bool", Default = "false", Description = "Whether refresh is in progress" },
        new ApiProperty { PropertyName = "PullThreshold", Type = "double", Default = "80", Description = "Pull distance to trigger refresh" },
        new ApiProperty { PropertyName = "RefreshContent", Type = "object", Default = "null", Description = "Content shown during pull/refresh" },
        new ApiProperty { PropertyName = "RefreshCommand", Type = "ICommand", Default = "null", Description = "Command executed on refresh" },
        new ApiProperty { PropertyName = "IsPullEnabled", Type = "bool", Default = "true", Description = "Whether pull gestures are enabled" },
        new ApiProperty { PropertyName = "CompletedMessage", Type = "string", Default = "null", Description = "Message after refresh completes" },
    };
}
