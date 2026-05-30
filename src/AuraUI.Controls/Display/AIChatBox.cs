using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;

namespace AuraUI.Controls.Display;

/// <summary>
/// Defines the role of a chat message.
/// </summary>
public enum ChatRole
{
    User,
    Assistant,
    System
}

/// <summary>
/// Represents a single chat message in the conversation.
/// </summary>
public class ChatMessage : AvaloniaObject
{
    /// <summary>
    /// Defines the <see cref="Role"/> property.
    /// </summary>
    public static readonly StyledProperty<ChatRole> RoleProperty =
        AvaloniaProperty.Register<ChatMessage, ChatRole>(nameof(Role));

    /// <summary>
    /// Defines the <see cref="Content"/> property.
    /// </summary>
    public static readonly StyledProperty<string?> ContentProperty =
        AvaloniaProperty.Register<ChatMessage, string?>(nameof(Content));

    /// <summary>
    /// Defines the <see cref="Timestamp"/> property.
    /// </summary>
    public static readonly StyledProperty<DateTime> TimestampProperty =
        AvaloniaProperty.Register<ChatMessage, DateTime>(nameof(Timestamp));

    /// <summary>
    /// Defines the <see cref="Avatar"/> property.
    /// </summary>
    public static readonly StyledProperty<IImage?> AvatarProperty =
        AvaloniaProperty.Register<ChatMessage, IImage?>(nameof(Avatar));

    /// <summary>
    /// Defines the <see cref="IsStreaming"/> property.
    /// </summary>
    public static readonly StyledProperty<bool> IsStreamingProperty =
        AvaloniaProperty.Register<ChatMessage, bool>(nameof(IsStreaming));

    /// <summary>
    /// Gets or sets the role of the message sender.
    /// </summary>
    public ChatRole Role
    {
        get => GetValue(RoleProperty);
        set => SetValue(RoleProperty, value);
    }

    /// <summary>
    /// Gets or sets the content text of the message.
    /// </summary>
    public string? Content
    {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    /// <summary>
    /// Gets or sets the timestamp of the message.
    /// </summary>
    public DateTime Timestamp
    {
        get => GetValue(TimestampProperty);
        set => SetValue(TimestampProperty, value);
    }

    /// <summary>
    /// Gets or sets the avatar image for the message sender.
    /// </summary>
    public IImage? Avatar
    {
        get => GetValue(AvatarProperty);
        set => SetValue(AvatarProperty, value);
    }

    /// <summary>
    /// Gets or sets whether this message is currently being streamed.
    /// </summary>
    public bool IsStreaming
    {
        get => GetValue(IsStreamingProperty);
        set => SetValue(IsStreamingProperty, value);
    }
}

/// <summary>
/// An AI chat interface control that displays a conversation between a user and an AI assistant.
/// Supports streaming responses, markdown rendering, code block display, and keyboard shortcuts.
///
/// Template parts:
///   PART_MessagesList     - ItemsRepeater or ListBox displaying the messages
///   PART_InputBox         - TextBox for composing messages
///   PART_SendButton       - Button to send the message
///   PART_ScrollViewer     - ScrollViewer for auto-scroll behavior
///
/// Pseudo-classes: :streaming, :loading, :empty
/// </summary>
[TemplatePart("PART_MessagesList", typeof(ItemsControl))]
[TemplatePart("PART_InputBox", typeof(TextBox))]
[TemplatePart("PART_SendButton", typeof(Button))]
[TemplatePart("PART_ScrollViewer", typeof(ScrollViewer))]
[PseudoClasses(":streaming", ":loading", ":empty")]
public class AIChatBox : TemplatedControl
{
    private ItemsControl? _messagesList;
    private TextBox? _inputBox;
    private Button? _sendButton;
    private ScrollViewer? _scrollViewer;

    /// <summary>
    /// Defines the <see cref="Messages"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IList<ChatMessage>?> MessagesProperty =
        AvaloniaProperty.Register<AIChatBox, IList<ChatMessage>?>(nameof(Messages));

    /// <summary>
    /// Defines the <see cref="IsStreaming"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsStreamingProperty =
        AvaloniaProperty.Register<AIChatBox, bool>(nameof(IsStreaming));

    /// <summary>
    /// Defines the <see cref="CurrentResponse"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> CurrentResponseProperty =
        AvaloniaProperty.Register<AIChatBox, string?>(nameof(CurrentResponse));

    /// <summary>
    /// Defines the <see cref="Placeholder"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> PlaceholderProperty =
        AvaloniaProperty.Register<AIChatBox, string?>(nameof(Placeholder), "Type a message...");

    /// <summary>
    /// Defines the <see cref="SendCommand"/> styled property.
    /// </summary>
    public static readonly StyledProperty<ICommand?> SendCommandProperty =
        AvaloniaProperty.Register<AIChatBox, ICommand?>(nameof(SendCommand));

    /// <summary>
    /// Defines the <see cref="ModelName"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> ModelNameProperty =
        AvaloniaProperty.Register<AIChatBox, string?>(nameof(ModelName));

    /// <summary>
    /// Defines the <see cref="ShowAvatar"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowAvatarProperty =
        AvaloniaProperty.Register<AIChatBox, bool>(nameof(ShowAvatar), true);

    /// <summary>
    /// Defines the <see cref="ShowTimestamp"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowTimestampProperty =
        AvaloniaProperty.Register<AIChatBox, bool>(nameof(ShowTimestamp));

    /// <summary>
    /// Defines the <see cref="IsLoading"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsLoadingProperty =
        AvaloniaProperty.Register<AIChatBox, bool>(nameof(IsLoading));

    /// <summary>
    /// Raised when a message is sent by the user.
    /// </summary>
    public event EventHandler<string>? MessageSent;

    static AIChatBox()
    {
        IsStreamingProperty.Changed.AddClassHandler<AIChatBox>((x, _) => x.UpdatePseudoClasses());
        IsLoadingProperty.Changed.AddClassHandler<AIChatBox>((x, _) => x.UpdatePseudoClasses());
        MessagesProperty.Changed.AddClassHandler<AIChatBox>((x, _) => x.OnMessagesChanged());
    }

    /// <summary>
    /// Gets or sets the collection of chat messages.
    /// </summary>
    public IList<ChatMessage>? Messages
    {
        get => GetValue(MessagesProperty);
        set => SetValue(MessagesProperty, value);
    }

    /// <summary>
    /// Gets or sets whether a response is currently being streamed.
    /// </summary>
    public bool IsStreaming
    {
        get => GetValue(IsStreamingProperty);
        set => SetValue(IsStreamingProperty, value);
    }

    /// <summary>
    /// Gets or sets the current streaming response text.
    /// </summary>
    public string? CurrentResponse
    {
        get => GetValue(CurrentResponseProperty);
        set => SetValue(CurrentResponseProperty, value);
    }

    /// <summary>
    /// Gets or sets the placeholder text for the input box.
    /// </summary>
    public string? Placeholder
    {
        get => GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
    }

    /// <summary>
    /// Gets or sets the command invoked when the user sends a message.
    /// The command parameter is the message text.
    /// </summary>
    public ICommand? SendCommand
    {
        get => GetValue(SendCommandProperty);
        set => SetValue(SendCommandProperty, value);
    }

    /// <summary>
    /// Gets or sets the name of the AI model being used.
    /// </summary>
    public string? ModelName
    {
        get => GetValue(ModelNameProperty);
        set => SetValue(ModelNameProperty, value);
    }

    /// <summary>
    /// Gets or sets whether to show avatars next to messages.
    /// </summary>
    public bool ShowAvatar
    {
        get => GetValue(ShowAvatarProperty);
        set => SetValue(ShowAvatarProperty, value);
    }

    /// <summary>
    /// Gets or sets whether to show timestamps on messages.
    /// </summary>
    public bool ShowTimestamp
    {
        get => GetValue(ShowTimestampProperty);
        set => SetValue(ShowTimestampProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the control is in a loading state.
    /// </summary>
    public bool IsLoading
    {
        get => GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        // Detach old parts
        if (_sendButton is not null)
            _sendButton.Click -= OnSendButtonClick;
        if (_inputBox is not null)
            _inputBox.KeyDown -= OnInputBoxKeyDown;

        base.OnApplyTemplate(e);

        _messagesList = e.NameScope.Find<ItemsControl>("PART_MessagesList");
        _inputBox = e.NameScope.Find<TextBox>("PART_InputBox");
        _sendButton = e.NameScope.Find<Button>("PART_SendButton");
        _scrollViewer = e.NameScope.Find<ScrollViewer>("PART_ScrollViewer");

        if (_sendButton is not null)
        {
            _sendButton.Click += OnSendButtonClick;
            _sendButton.SetValue(AutomationProperties.NameProperty, "Send");
        }

        if (_inputBox is not null)
        {
            _inputBox.KeyDown += OnInputBoxKeyDown;
#pragma warning disable CS0618 // Watermark is deprecated but PlaceholderText not available in this version
            _inputBox.Watermark = Placeholder;
#pragma warning restore CS0618
        }

        UpdatePseudoClasses();
        UpdateEmptyState();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        if (_sendButton is not null)
            _sendButton.Click -= OnSendButtonClick;
        if (_inputBox is not null)
            _inputBox.KeyDown -= OnInputBoxKeyDown;
    }

    private void OnInputBoxKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter && e.KeyModifiers != KeyModifiers.Shift)
        {
            SendMessage();
            e.Handled = true;
        }
    }

    private void OnSendButtonClick(object? sender, RoutedEventArgs e)
    {
        SendMessage();
    }

    /// <summary>
    /// Sends the current input text as a user message.
    /// </summary>
    public void SendMessage()
    {
        var text = _inputBox?.Text;
        if (string.IsNullOrWhiteSpace(text))
            return;

        MessageSent?.Invoke(this, text);

        if (SendCommand?.CanExecute(text) == true)
        {
            SendCommand.Execute(text);
        }

        if (_inputBox is not null)
        {
            _inputBox.Text = string.Empty;
        }
    }

    /// <summary>
    /// Scrolls the message list to the bottom.
    /// </summary>
    public void ScrollToBottom()
    {
        Dispatcher.UIThread.Post(() =>
        {
            if (_scrollViewer is not null)
            {
                _scrollViewer.Offset = new Vector(_scrollViewer.Offset.X, _scrollViewer.Extent.Height);
            }
        }, DispatcherPriority.Render);
    }

    private void OnMessagesChanged()
    {
        UpdateEmptyState();
        ScrollToBottom();
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":streaming", IsStreaming);
        PseudoClasses.Set(":loading", IsLoading);
    }

    private void UpdateEmptyState()
    {
        PseudoClasses.Set(":empty", Messages is null or { Count: 0 });
    }
}
