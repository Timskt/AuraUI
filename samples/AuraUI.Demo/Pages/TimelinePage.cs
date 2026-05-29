using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using AuraUI.Controls.Display;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class TimelinePage : ComponentPageBase
{
    public override string ComponentName => "Timeline";
    public override string Description => "A vertical timeline for displaying events in chronological order with icons and color indicators.";
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
                    "Use timelines to show a sequence of events, activity logs, or process steps. They provide clear visual hierarchy for chronological data.",
                    "Use consistent timestamp formatting. Use DotColor for status indication. Limit to recent/important events.")
            }
        };
    }

    private Control BuildBasicExample()
    {
        var timeline = new AuraTimeline { MaxWidth = 500 };
        timeline.Items.Add(new AuraTimelineItem { Content = "Order Placed - Your order #1234 has been placed successfully.", Timestamp = "2024-01-15 10:30 AM", DotColor = new SolidColorBrush(Color.Parse("#107C10")) });
        timeline.Items.Add(new AuraTimelineItem { Content = "Payment Confirmed - Payment of $99.99 processed.", Timestamp = "2024-01-15 10:31 AM", DotColor = new SolidColorBrush(Color.Parse("#107C10")) });
        timeline.Items.Add(new AuraTimelineItem { Content = "Shipped - Package shipped via FedEx.", Timestamp = "2024-01-16 2:15 PM", DotColor = new SolidColorBrush(Color.Parse("#0078D4")) });
        timeline.Items.Add(new AuraTimelineItem { Content = "In Transit - Expected delivery tomorrow.", Timestamp = "2024-01-17 8:00 AM", DotColor = new SolidColorBrush(Color.Parse("#FFB900")) });

        return CreateExampleSection("Order Timeline", timeline,
            @"<display:AuraTimeline MaxWidth=""500"">
    <display:AuraTimelineItem Content=""Order Placed""
        Timestamp=""2024-01-15 10:30 AM""
        DotColor=""{DynamicResource AuraSuccessBrush}""/>
    <display:AuraTimelineItem Content=""Payment Confirmed""
        Timestamp=""2024-01-15 10:31 AM""
        DotColor=""{DynamicResource AuraSuccessBrush}""/>
    <display:AuraTimelineItem Content=""Shipped""
        Timestamp=""2024-01-16 2:15 PM""
        DotColor=""{DynamicResource AuraPrimaryBrush}""/>
</display:AuraTimeline>");
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Content", Type = "object", Default = "null", Description = "Timeline item content" },
        new ApiProperty { PropertyName = "Timestamp", Type = "string", Default = "null", Description = "Time string" },
        new ApiProperty { PropertyName = "DotColor", Type = "IBrush", Default = "null", Description = "Color of the timeline dot" },
        new ApiProperty { PropertyName = "Icon", Type = "object", Default = "null", Description = "Custom icon in the dot" },
    };
}
