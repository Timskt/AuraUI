using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;

namespace AuraUI.Demo.Components;

/// <summary>
/// A code display block with monospace font, dark background, and optional copy button.
/// </summary>
public class CodeBlock : TemplatedControl
{
    public static readonly StyledProperty<string?> CodeProperty =
        AvaloniaProperty.Register<CodeBlock, string?>(nameof(Code));

    public static readonly StyledProperty<string> LanguageProperty =
        AvaloniaProperty.Register<CodeBlock, string>(nameof(Language), "axaml");

    public static readonly StyledProperty<bool> IsCopyableProperty =
        AvaloniaProperty.Register<CodeBlock, bool>(nameof(IsCopyable), true);

    public string? Code
    {
        get => GetValue(CodeProperty);
        set => SetValue(CodeProperty, value);
    }

    public string Language
    {
        get => GetValue(LanguageProperty);
        set => SetValue(LanguageProperty, value);
    }

    public bool IsCopyable
    {
        get => GetValue(IsCopyableProperty);
        set => SetValue(IsCopyableProperty, value);
    }

    private Button? _copyButton;
    private TextBlock? _codeText;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        if (_copyButton != null)
            _copyButton.Click -= OnCopyClick;

        _copyButton = e.NameScope.Find<Button>("PART_CopyButton");
        _codeText = e.NameScope.Find<TextBlock>("PART_CodeText");

        if (_copyButton != null)
            _copyButton.Click += OnCopyClick;

        UpdateCodeDisplay();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == CodeProperty)
            UpdateCodeDisplay();
    }

    private void UpdateCodeDisplay()
    {
        // TextBlock is bound via template binding; no manual update needed
    }

    private async void OnCopyClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (Code != null)
        {
            var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
            if (clipboard != null)
            {
                var dataTransfer = new DataTransfer();
                dataTransfer.Add(DataTransferItem.CreateText(Code));
                await clipboard.SetDataAsync(dataTransfer);
                if (_copyButton != null)
                {
                    var orig = _copyButton.Content;
                    _copyButton.Content = "Copied!";
                    await System.Threading.Tasks.Task.Delay(1500);
                    _copyButton.Content = orig;
                }
            }
        }
    }
}
