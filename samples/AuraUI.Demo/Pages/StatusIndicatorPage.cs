using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using AuraUI.Controls.Display;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class StatusIndicatorPage : ComponentPageBase
{
    public override string ComponentName => "StatusIndicator";
    public override string Description => "A colored status dot with optional pulse animation and label text. Used to display the status of services, servers, or resources.";
    public override string Category => "Display";

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
                    "Use StatusIndicator to show the current state of services, connections, users, or any resource with distinct states.",
                    "Use consistent colors across the app. Enable pulse animation for active/attention states. Show labels when context isn't clear from color alone.")
            }
        };
    }

    private Control BuildBasicExample()
    {
        var panel = new StackPanel { Spacing = 12 };
        var statuses = new[] { ServiceStatus.Online, ServiceStatus.Offline, ServiceStatus.Warning, ServiceStatus.Error, ServiceStatus.Maintenance };
        var labels = new[] { "API Gateway", "Database", "Cache", "Auth Service", "CDN" };

        for (int i = 0; i < statuses.Length; i++)
        {
            var indicator = new StatusIndicator
            {
                Status = statuses[i],
                ShowLabel = true,
                Label = labels[i],
                PulseAnimation = statuses[i] == ServiceStatus.Online || statuses[i] == ServiceStatus.Warning,
                IndicatorSize = 10,
                Margin = new Thickness(0, 0, 0, 8)
            };
            panel.Children.Add(indicator);
        }

        return CreateExampleSection("Service Status", panel,
            @"<display:StatusIndicator Status=""Online"" ShowLabel=""True""
    Label=""API Gateway"" PulseAnimation=""True""/>
<display:StatusIndicator Status=""Offline"" ShowLabel=""True""
    Label=""Database""/>
<display:StatusIndicator Status=""Warning"" ShowLabel=""True""
    Label=""Cache"" PulseAnimation=""True""/>
<display:StatusIndicator Status=""Error"" ShowLabel=""True""
    Label=""Auth Service""/>
<display:StatusIndicator Status=""Maintenance"" ShowLabel=""True""
    Label=""CDN""/>");
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Status", Type = "ServiceStatus", Default = "Online", Description = "Online, Offline, Warning, Error, Maintenance" },
        new ApiProperty { PropertyName = "IndicatorSize", Type = "double", Default = "10", Description = "Dot diameter in pixels" },
        new ApiProperty { PropertyName = "ShowLabel", Type = "bool", Default = "false", Description = "Show text label" },
        new ApiProperty { PropertyName = "Label", Type = "string", Default = "null", Description = "Label text (defaults to status name)" },
        new ApiProperty { PropertyName = "PulseAnimation", Type = "bool", Default = "false", Description = "Animate opacity for active states" },
    };
}
