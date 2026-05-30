using System;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Display;

namespace AuraUI.Demo.Pages;

public class ChatDemoPage : ComponentPageBase
{
    public override string ComponentName => "Chat Interface";
    public override string Description => "A chat interface scenario demonstrating message bubbles, typing indicators, timestamps, and copy-to-clipboard functionality using AuraUI controls.";
    public override string Category => "Scenarios";

    protected override Control BuildContent()
    {
        return new StackPanel
        {
            Spacing = 32,
            Children =
            {
                BuildChatExample(),
                BuildGuidelinesSection()
            }
        };
    }

    private Control BuildChatExample()
    {
        var messages = new ObservableCollection<ChatMessage>
        {
            new ChatMessage
            {
                Role = ChatRole.User,
                Content = "How do I create a new project with AuraUI?",
                Timestamp = DateTime.Now.AddMinutes(-10)
            },
            new ChatMessage
            {
                Role = ChatRole.Assistant,
                Content = "To create a new project with AuraUI, follow these steps:\n\n1. Create a new Avalonia project: `dotnet new avalonia -n MyApp`\n2. Add the AuraUI NuGet package: `dotnet add package AuraUI.Controls`\n3. Add the theme package: `dotnet add package AuraUI.Themes.Fluent`\n4. Update App.axaml to include the AuraUI theme.\n\nWould you like me to show the detailed configuration?",
                Timestamp = DateTime.Now.AddMinutes(-9)
            },
            new ChatMessage
            {
                Role = ChatRole.User,
                Content = "Yes, please show the App.axaml configuration.",
                Timestamp = DateTime.Now.AddMinutes(-7)
            },
            new ChatMessage
            {
                Role = ChatRole.Assistant,
                Content = "Here's the App.axaml configuration:\n\n```xml\n<Application xmlns=\"https://github.com/avaloniaui\"\n    RequestedThemeVariant=\"Light\">\n    <Application.Styles>\n        <FluentTheme />\n        <StyleInclude Source=\"avares://AuraUI.Themes.Fluent/Styles.axaml\"/>\n    </Application.Styles>\n</Application>\n```\n\nThis sets up the Fluent theme with all AuraUI design tokens and component styles.",
                Timestamp = DateTime.Now.AddMinutes(-6)
            },
            new ChatMessage
            {
                Role = ChatRole.User,
                Content = "What components are available?",
                Timestamp = DateTime.Now.AddMinutes(-3)
            },
            new ChatMessage
            {
                Role = ChatRole.Assistant,
                Content = "AuraUI includes 35+ components across these categories:\n\n- Layout: Card, Drawer, Space, Avatar, Badge\n- Input: Button, TextBox, SearchBox, Upload\n- Selection: ComboBox, Switch, RateControl, Cascader\n- Display: Charts (Line, Bar, Pie, Area), MetricCard, Timeline\n- Navigation: TabControl, Pagination, NavigationView\n- Feedback: Toast, Dialog, Notification, Tour\n- Windowing: WindowX\n\nEach component supports light/dark themes and is fully customizable.",
                Timestamp = DateTime.Now.AddMinutes(-2)
            }
        };

        var chatBox = new AIChatBox
        {
            Width = 550,
            Height = 420,
            Messages = messages,
            Placeholder = "Type your question...",
            ShowAvatar = true,
            ShowTimestamp = true
        };

        // Typing indicator demo
        var typingIndicator = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 6,
            Margin = new Thickness(16, 8),
            Children =
            {
                new TextBlock
                {
                    Text = "Assistant is typing",
                    FontSize = 12,
                    Foreground = GetBrush("AuraForegroundSecondaryBrush", "#666666"),
                    FontStyle = FontStyle.Italic,
                    VerticalAlignment = VerticalAlignment.Center
                },
                new ProgressRing
                {
                    Width = 14,
                    Height = 14,
                    IsIndeterminate = true,
                    Foreground = GetBrush("AuraPrimaryBrush", "#0078D4")
                }
            }
        };

        var headerBar = new Border
        {
            Background = GetBrush("AuraMutedBrush", "#F5F5F5"),
            Padding = new Thickness(16, 10),
            Child = new TextBlock
            {
                Text = "AuraUI Assistant",
                FontWeight = FontWeight.SemiBold,
                FontSize = 14,
                Foreground = GetBrush("AuraForegroundBrush", "#000000")
            }
        };
        DockPanel.SetDock(headerBar, Dock.Top);

        var chatContainer = new Border
        {
            Background = GetBrush("AuraCardBrush", "#FFFFFF"),
            CornerRadius = new CornerRadius(8),
            BorderBrush = GetBrush("AuraBorderBrush", "#E0E0E0"),
            BorderThickness = new Thickness(1),
            Child = new DockPanel
            {
                Children =
                {
                    headerBar,
                    chatBox
                }
            }
        };

        // Copy button demo
        var copyDemo = new StackPanel
        {
            Spacing = 8,
            Children =
            {
                new TextBlock
                {
                    Text = "Message Actions",
                    FontSize = 14,
                    FontWeight = FontWeight.SemiBold,
                    Foreground = GetBrush("AuraForegroundBrush", "#000000")
                },
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 8,
                    Children =
                    {
                        new Button
                        {
                            Content = "Copy Last Message",
                            Classes = { "outline" },
                            Padding = new Thickness(12, 6)
                        },
                        new Button
                        {
                            Content = "Regenerate Response",
                            Classes = { "outline" },
                            Padding = new Thickness(12, 6)
                        },
                        new Button
                        {
                            Content = "Clear Chat",
                            Classes = { "outline" },
                            Padding = new Thickness(12, 6)
                        }
                    }
                }
            }
        };

        var result = new StackPanel
        {
            Spacing = 16,
            MaxWidth = 600,
            Children = { chatContainer, copyDemo }
        };

        return CreateExampleSection("AI Chat Interface", result,
            @"<display:AIChatBox Width=""550"" Height=""420""
    Placeholder=""Type your question...""
    ShowAvatar=""True"" ShowTimestamp=""True"">
    <display:ChatMessage Role=""User""
        Content=""How do I create a new project?""/>
    <display:ChatMessage Role=""Assistant""
        Content=""Follow these steps: ...""/>
</display:AIChatBox>

<!-- Typing Indicator -->
<StackPanel Orientation=""Horizontal"" Spacing=""6"">
    <TextBlock Text=""Assistant is typing"" FontStyle=""Italic""/>
    <ProgressRing Width=""14"" Height=""14"" IsIndeterminate=""True""/>
</StackPanel>",
            @"var chatBox = new AIChatBox
{
    Width = 550,
    Height = 420,
    Placeholder = ""Type your question..."",
    ShowAvatar = true,
    ShowTimestamp = true,
    Messages = new ObservableCollection<ChatMessage>
    {
        new ChatMessage
        {
            Role = ChatRole.User,
            Content = ""Hello!"",
            Timestamp = DateTime.Now
        },
        new ChatMessage
        {
            Role = ChatRole.Assistant,
            Content = ""Hi! How can I help?"",
            Timestamp = DateTime.Now
        }
    }
};");
    }

    private Control BuildGuidelinesSection()
    {
        return CreateGuidelines(
            "Use chat interfaces for AI assistants, customer support, messaging apps, and conversational UIs. They provide a familiar interaction pattern for text-based communication.",
            "Show message timestamps. Use distinct styling for user vs assistant messages. Include a typing indicator for AI responses. Auto-scroll to the latest message. Provide message actions like copy and regenerate. Support markdown in assistant messages.");
    }
}
