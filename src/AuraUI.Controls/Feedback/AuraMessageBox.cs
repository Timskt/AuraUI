using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;

namespace AuraUI.Controls.Feedback;

/// <summary>
/// Specifies the icon type for a message box.
/// </summary>
public enum MessageBoxIcon
{
    None,
    Info,
    Warning,
    Error,
    Success,
    Question
}

/// <summary>
/// Specifies the buttons displayed in a message box.
/// </summary>
public enum MessageBoxButtons
{
    OK,
    OKCancel,
    YesNo,
    YesNoCancel
}

/// <summary>
/// Specifies which button is focused by default.
/// </summary>
public enum MessageBoxDefaultButton
{
    Button1,
    Button2,
    Button3
}

/// <summary>
/// Specifies the result returned from a message box.
/// </summary>
public enum MessageBoxResult
{
    None,
    OK,
    Cancel,
    Yes,
    No
}

/// <summary>
/// A message box dialog with static convenience methods for showing alerts,
/// confirmations, and custom button configurations.
/// </summary>
[TemplatePart("PART_OKButton", typeof(Button))]
[TemplatePart("PART_CancelButton", typeof(Button))]
[TemplatePart("PART_YesButton", typeof(Button))]
[TemplatePart("PART_NoButton", typeof(Button))]
[PseudoClasses(":info", ":warning", ":error", ":success", ":question")]
public class AuraMessageBox : Window
{
    private Button? _okButton;
    private Button? _cancelButton;
    private Button? _yesButton;
    private Button? _noButton;

    /// <summary>
    /// Defines the <see cref="Message"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> MessageProperty =
        AvaloniaProperty.Register<AuraMessageBox, string?>(nameof(Message));

    /// <summary>
    /// Defines the <see cref="Icon"/> styled property.
    /// </summary>
    public new static readonly StyledProperty<MessageBoxIcon> IconProperty =
        AvaloniaProperty.Register<AuraMessageBox, MessageBoxIcon>(nameof(Icon));

    /// <summary>
    /// Defines the <see cref="Buttons"/> styled property.
    /// </summary>
    public static readonly StyledProperty<MessageBoxButtons> ButtonsProperty =
        AvaloniaProperty.Register<AuraMessageBox, MessageBoxButtons>(nameof(Buttons), MessageBoxButtons.OK);

    /// <summary>
    /// Defines the <see cref="DefaultButton"/> styled property.
    /// </summary>
    public static readonly StyledProperty<MessageBoxDefaultButton> DefaultButtonProperty =
        AvaloniaProperty.Register<AuraMessageBox, MessageBoxDefaultButton>(nameof(DefaultButton));

    /// <summary>
    /// Defines the <see cref="Result"/> styled property.
    /// </summary>
    public static readonly StyledProperty<MessageBoxResult> ResultProperty =
        AvaloniaProperty.Register<AuraMessageBox, MessageBoxResult>(nameof(Result));

    static AuraMessageBox()
    {
        IconProperty.Changed.AddClassHandler<AuraMessageBox>((x, _) => x.UpdatePseudoClasses());
    }

    /// <summary>
    /// Gets or sets the message text.
    /// </summary>
    public string? Message
    {
        get => GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }

    /// <summary>
    /// Gets or sets the icon type.
    /// </summary>
    public new MessageBoxIcon Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    /// <summary>
    /// Gets or sets which buttons to show.
    /// </summary>
    public MessageBoxButtons Buttons
    {
        get => GetValue(ButtonsProperty);
        set => SetValue(ButtonsProperty, value);
    }

    /// <summary>
    /// Gets or sets which button is focused by default.
    /// </summary>
    public MessageBoxDefaultButton DefaultButton
    {
        get => GetValue(DefaultButtonProperty);
        set => SetValue(DefaultButtonProperty, value);
    }

    /// <summary>
    /// Gets the result after the dialog closes.
    /// </summary>
    public MessageBoxResult Result
    {
        get => GetValue(ResultProperty);
        set => SetValue(ResultProperty, value);
    }

    /// <summary>
    /// Shows a message box with the specified parameters (synchronous, blocks the owner).
    /// </summary>
    /// <remarks>
    /// This method is obsolete. Use <see cref="ShowAsync"/> instead to avoid potential deadlocks.
    /// </remarks>
    [Obsolete("Use ShowAsync instead. The synchronous Show method risks deadlocks on the UI thread.", false)]
    public static MessageBoxResult Show(Window? owner, string title, string message,
        MessageBoxButtons buttons = MessageBoxButtons.OK,
        MessageBoxIcon icon = MessageBoxIcon.None,
        MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Button1)
    {
        var messageBox = new AuraMessageBox
        {
            Title = title,
            Message = message,
            Buttons = buttons,
            Icon = icon,
            DefaultButton = defaultButton,
            Width = 420,
            Height = 200,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            CanResize = false,
            ShowInTaskbar = false
        };

        if (owner != null)
        {
            messageBox.ShowDialog(owner).GetAwaiter().GetResult();
        }
        else
        {
            messageBox.Show();
        }

        return messageBox.Result;
    }

    /// <summary>
    /// Shows a message box asynchronously.
    /// </summary>
    public static async Task<MessageBoxResult> ShowAsync(Window? owner, string title, string message,
        MessageBoxButtons buttons = MessageBoxButtons.OK,
        MessageBoxIcon icon = MessageBoxIcon.None,
        MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Button1)
    {
        var messageBox = new AuraMessageBox
        {
            Title = title,
            Message = message,
            Buttons = buttons,
            Icon = icon,
            DefaultButton = defaultButton,
            Width = 420,
            Height = 200,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            CanResize = false,
            ShowInTaskbar = false
        };

        if (owner != null)
        {
            await messageBox.ShowDialog(owner);
        }

        return messageBox.Result;
    }

    /// <summary>
    /// Shows a confirmation dialog (synchronous).
    /// </summary>
    /// <remarks>
    /// This method is obsolete. Use <see cref="ConfirmAsync"/> instead to avoid potential deadlocks.
    /// </remarks>
    [Obsolete("Use ConfirmAsync instead. The synchronous Confirm method risks deadlocks on the UI thread.", false)]
    public static bool Confirm(Window? owner, string title, string message,
        MessageBoxIcon icon = MessageBoxIcon.Question)
    {
        return Show(owner, title, message, MessageBoxButtons.YesNo, icon) == MessageBoxResult.Yes;
    }

    /// <summary>
    /// Shows a confirmation dialog asynchronously.
    /// </summary>
    public static async Task<bool> ConfirmAsync(Window? owner, string title, string message,
        MessageBoxIcon icon = MessageBoxIcon.Question)
    {
        var result = await ShowAsync(owner, title, message, MessageBoxButtons.YesNo, icon);
        return result == MessageBoxResult.Yes;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        UnsubscribeButtons();

        _okButton = e.NameScope.Find<Button>("PART_OKButton");
        _cancelButton = e.NameScope.Find<Button>("PART_CancelButton");
        _yesButton = e.NameScope.Find<Button>("PART_YesButton");
        _noButton = e.NameScope.Find<Button>("PART_NoButton");

        SubscribeButtons();
        ConfigureButtons();
        UpdatePseudoClasses();
    }

    private void UnsubscribeButtons()
    {
        if (_okButton != null) _okButton.Click -= OnOkClick;
        if (_cancelButton != null) _cancelButton.Click -= OnCancelClick;
        if (_yesButton != null) _yesButton.Click -= OnYesClick;
        if (_noButton != null) _noButton.Click -= OnNoClick;
    }

    private void SubscribeButtons()
    {
        if (_okButton != null) _okButton.Click += OnOkClick;
        if (_cancelButton != null) _cancelButton.Click += OnCancelClick;
        if (_yesButton != null) _yesButton.Click += OnYesClick;
        if (_noButton != null) _noButton.Click += OnNoClick;
    }

    private void ConfigureButtons()
    {
        var showOk = false;
        var showCancel = false;
        var showYes = false;
        var showNo = false;

        switch (Buttons)
        {
            case MessageBoxButtons.OK:
                showOk = true;
                break;
            case MessageBoxButtons.OKCancel:
                showOk = true;
                showCancel = true;
                break;
            case MessageBoxButtons.YesNo:
                showYes = true;
                showNo = true;
                break;
            case MessageBoxButtons.YesNoCancel:
                showYes = true;
                showNo = true;
                showCancel = true;
                break;
        }

        if (_okButton != null) _okButton.IsVisible = showOk;
        if (_cancelButton != null) _cancelButton.IsVisible = showCancel;
        if (_yesButton != null) _yesButton.IsVisible = showYes;
        if (_noButton != null) _noButton.IsVisible = showNo;

        // Focus default button
        var focusButton = DefaultButton switch
        {
            MessageBoxDefaultButton.Button1 when showOk => _okButton,
            MessageBoxDefaultButton.Button1 when showYes => _yesButton,
            MessageBoxDefaultButton.Button2 when showCancel => _cancelButton,
            MessageBoxDefaultButton.Button2 when showNo => _noButton,
            _ => _okButton ?? (Control?)_yesButton
        };

        if (focusButton != null)
        {
            Dispatcher.UIThread.Post(() => focusButton.Focus(), DispatcherPriority.Loaded);
        }
    }

    private void CloseWithResult(MessageBoxResult result)
    {
        Result = result;
        Close(result);
    }

    private void OnOkClick(object? sender, RoutedEventArgs e) => CloseWithResult(MessageBoxResult.OK);
    private void OnCancelClick(object? sender, RoutedEventArgs e) => CloseWithResult(MessageBoxResult.Cancel);
    private void OnYesClick(object? sender, RoutedEventArgs e) => CloseWithResult(MessageBoxResult.Yes);
    private void OnNoClick(object? sender, RoutedEventArgs e) => CloseWithResult(MessageBoxResult.No);

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":info", Icon == MessageBoxIcon.Info);
        PseudoClasses.Set(":warning", Icon == MessageBoxIcon.Warning);
        PseudoClasses.Set(":error", Icon == MessageBoxIcon.Error);
        PseudoClasses.Set(":success", Icon == MessageBoxIcon.Success);
        PseudoClasses.Set(":question", Icon == MessageBoxIcon.Question);
    }
}
