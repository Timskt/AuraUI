using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using AuraUI.Controls.Display;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class AIChatBoxPage : ComponentPageBase
{
    public override string ComponentName => "AIChatBox";
    public override string Description => "A chat interface for AI conversations with user/assistant/system message roles, streaming support, and input area with send button.";
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
                    "Use AIChatBox for AI assistants, chatbots, conversational interfaces, and any back-and-forth messaging UI.",
                    "Show role avatars. Support streaming for long responses. Provide a clear input area. Auto-scroll to latest message. Allow message copying.")
            }
        };
    }

    private Control BuildBasicExample()
    {
        var chatBox = new AIChatBox
        {
            Width = 500,
            Height = 400,
            Messages = new ObservableCollection<ChatMessage>
            {
                new ChatMessage { Role = ChatRole.User, Content = "What is AuraUI?", Timestamp = DateTime.Now.AddMinutes(-5) },
                new ChatMessage { Role = ChatRole.Assistant, Content = "AuraUI is a comprehensive UI component framework for Avalonia desktop applications. It provides 35+ controls including charts, data grids, form inputs, navigation components, and more.", Timestamp = DateTime.Now.AddMinutes(-4) },
                new ChatMessage { Role = ChatRole.User, Content = "What controls does it include?", Timestamp = DateTime.Now.AddMinutes(-2) },
                new ChatMessage { Role = ChatRole.Assistant, Content = "AuraUI includes controls in categories like Layout (Card, Drawer, Space), Input (TextBox, Upload, ChipInput), Selection (ComboBox, TreeSelect, Cascader), Display (Charts, Calendar, Terminal), Navigation (NavigationView, FileExplorer), and Feedback (Toast, Dialog, Tour).", Timestamp = DateTime.Now.AddMinutes(-1) },
            }
        };

        return CreateExampleSection("AI Chat", chatBox,
            @"<display:AIChatBox Width=""500"" Height=""400"">
    <display:ChatMessage Role=""User"" Content=""What is AuraUI?""/>
    <display:ChatMessage Role=""Assistant""
        Content=""AuraUI is a comprehensive UI framework...""/>
</display:AIChatBox>");
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Messages", Type = "ObservableCollection<ChatMessage>", Default = "null", Description = "Chat messages" },
        new ApiProperty { PropertyName = "Placeholder", Type = "string", Default = "Type a message...", Description = "Input placeholder" },
        new ApiProperty { PropertyName = "IsStreaming", Type = "bool", Default = "false", Description = "Whether assistant is streaming" },
        new ApiProperty { PropertyName = "ShowAvatar", Type = "bool", Default = "true", Description = "Show message avatars" },
        new ApiProperty { PropertyName = "ShowTimestamp", Type = "bool", Default = "true", Description = "Show message timestamps" },
        new ApiProperty { PropertyName = "MaxMessages", Type = "int", Default = "100", Description = "Max messages before pruning" },
    };
}
