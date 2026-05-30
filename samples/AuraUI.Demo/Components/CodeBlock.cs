using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;

namespace AuraUI.Demo.Components;

/// <summary>
/// A code display block with monospace font, dark background, and optional copy button.
/// Builds its own visual tree in code (no AXAML template required).
/// </summary>
public class CodeBlock : UserControl
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
    private TextBlock? _copyFeedback;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        BuildVisualTree();
        UpdateCodeDisplay();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == CodeProperty)
            UpdateCodeDisplay();
        else if (change.Property == IsCopyableProperty)
            UpdateCopyButtonVisibility();
    }

    private void BuildVisualTree()
    {
        _codeText = new TextBlock
        {
            FontFamily = new FontFamily("Consolas,Menlo,Monaco,Courier New,monospace"),
            FontSize = 13,
            Foreground = new SolidColorBrush(Color.Parse("#D4D4D4")),
            TextWrapping = TextWrapping.NoWrap,
            Text = string.Empty
        };

        _copyButton = new Button
        {
            Content = "Copy",
            FontSize = 12,
            Padding = new Thickness(8, 4),
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Top,
            Background = new SolidColorBrush(Color.Parse("#3C3C3C")),
            Foreground = new SolidColorBrush(Color.Parse("#CCCCCC")),
            BorderBrush = new SolidColorBrush(Color.Parse("#505050")),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(4),
            Cursor = new Cursor(StandardCursorType.Hand),
        };
        _copyButton.Click += OnCopyClick;

        _copyFeedback = new TextBlock
        {
            Text = "Copied!",
            FontSize = 12,
            Foreground = new SolidColorBrush(Color.Parse("#4EC9B0")),
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Top,
            IsVisible = false,
            Margin = new Thickness(0, 0, 80, 0)
        };

        var header = new DockPanel
        {
            Margin = new Thickness(12, 8, 12, 0),
            LastChildFill = true,
            Children =
            {
                new TextBlock
                {
                    Text = Language.ToUpperInvariant(),
                    FontSize = 11,
                    Foreground = new SolidColorBrush(Color.Parse("#808080")),
                    VerticalAlignment = VerticalAlignment.Center
                }
            }
        };

        if (IsCopyable)
        {
            DockPanel.SetDock(_copyButton, Dock.Right);
            header.Children.Add(_copyButton);
        }
        if (_copyFeedback != null)
        {
            DockPanel.SetDock(_copyFeedback, Dock.Right);
            header.Children.Add(_copyFeedback);
        }

        var mainStack = new StackPanel
        {
            Children =
            {
                header,
                new ScrollViewer
                {
                    HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                    VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                    MaxHeight = 400,
                    Margin = new Thickness(12, 8, 12, 12),
                    Content = _codeText
                }
            }
        };

        Content = new Border
        {
            Background = new SolidColorBrush(Color.Parse("#1E1E1E")),
            BorderBrush = new SolidColorBrush(Color.Parse("#3C3C3C")),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(8),
            Child = mainStack
        };
    }

    private void UpdateCodeDisplay()
    {
        if (_codeText != null)
        {
            _codeText.Text = Code ?? string.Empty;
        }
    }

    private void UpdateCopyButtonVisibility()
    {
        if (_copyButton != null)
        {
            _copyButton.IsVisible = IsCopyable;
        }
    }

    private async void OnCopyClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (Code == null) return;

        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel?.Clipboard != null)
        {
            await Avalonia.Input.Platform.ClipboardExtensions.SetTextAsync(topLevel.Clipboard, Code);

            if (_copyButton != null)
            {
                var origContent = _copyButton.Content;
                _copyButton.Content = "Copied!";
                _copyButton.IsEnabled = false;

                await Task.Delay(1500);

                _copyButton.Content = origContent;
                _copyButton.IsEnabled = true;
            }
        }
    }
}
