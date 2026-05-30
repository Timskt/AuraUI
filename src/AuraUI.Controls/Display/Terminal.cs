using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;

namespace AuraUI.Controls.Display;

/// <summary>
/// A terminal/console control with command input, output display, and scroll history.
/// Supports command history navigation and keyboard shortcuts.
///
/// Template parts:
///   PART_OutputArea       - TextBlock or ItemsRepeater displaying terminal output
///   PART_InputBox         - TextBox for command input
///   PART_ScrollViewer     - ScrollViewer for terminal content
///
/// Pseudo-classes: :readonly, :active
/// </summary>
[TemplatePart("PART_OutputArea", typeof(ItemsControl))]
[TemplatePart("PART_InputBox", typeof(TextBox))]
[TemplatePart("PART_ScrollViewer", typeof(ScrollViewer))]
[PseudoClasses(":readonly", ":active")]
public class Terminal : TemplatedControl
{
    private ItemsControl? _outputArea;
    private TextBox? _inputBox;
    private ScrollViewer? _scrollViewer;

    private readonly ObservableCollection<string> _outputLines = new();
    private readonly List<string> _commandHistory = new();
    private int _historyIndex = -1;

    /// <summary>
    /// Defines the <see cref="Lines"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IList<string>?> LinesProperty =
        AvaloniaProperty.Register<Terminal, IList<string>?>(nameof(Lines));

    /// <summary>
    /// Defines the <see cref="Prompt"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> PromptProperty =
        AvaloniaProperty.Register<Terminal, string?>(nameof(Prompt), "> ");

    /// <summary>
    /// Defines the <see cref="InputCommand"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> InputCommandProperty =
        AvaloniaProperty.Register<Terminal, string?>(nameof(InputCommand));

    /// <summary>
    /// Defines the <see cref="MaxLines"/> styled property.
    /// </summary>
    public static readonly StyledProperty<int> MaxLinesProperty =
        AvaloniaProperty.Register<Terminal, int>(nameof(MaxLines), 10000);

    /// <summary>
    /// Defines the <see cref="IsInputEnabled"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsInputEnabledProperty =
        AvaloniaProperty.Register<Terminal, bool>(nameof(IsInputEnabled), true);

    /// <summary>
    /// Defines the <see cref="ShowPrompt"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowPromptProperty =
        AvaloniaProperty.Register<Terminal, bool>(nameof(ShowPrompt), true);

    /// <summary>
    /// Defines the <see cref="TerminalFontFamily"/> styled property.
    /// </summary>
    public static readonly StyledProperty<FontFamily> TerminalFontFamilyProperty =
        AvaloniaProperty.Register<Terminal, FontFamily>(nameof(TerminalFontFamily), new FontFamily("Cascadia Code,Consolas,Courier New,monospace"));

    /// <summary>
    /// Defines the <see cref="TerminalFontSize"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> TerminalFontSizeProperty =
        AvaloniaProperty.Register<Terminal, double>(nameof(TerminalFontSize), 13.0);

    /// <summary>
    /// Raised when a command is entered by the user.
    /// </summary>
    public event EventHandler<string>? CommandEntered;

    static Terminal()
    {
        LinesProperty.Changed.AddClassHandler<Terminal>((x, e) => x.OnLinesChanged(e));
        IsInputEnabledProperty.Changed.AddClassHandler<Terminal>((x, _) => x.UpdatePseudoClasses());
    }

    /// <summary>
    /// Gets or sets the output lines displayed in the terminal.
    /// </summary>
    public IList<string>? Lines
    {
        get => GetValue(LinesProperty);
        set => SetValue(LinesProperty, value);
    }

    /// <summary>
    /// Gets or sets the command prompt string.
    /// </summary>
    public string? Prompt
    {
        get => GetValue(PromptProperty);
        set => SetValue(PromptProperty, value);
    }

    /// <summary>
    /// Gets or sets the current input command text.
    /// </summary>
    public string? InputCommand
    {
        get => GetValue(InputCommandProperty);
        set => SetValue(InputCommandProperty, value);
    }

    /// <summary>
    /// Gets or sets the maximum number of lines to retain.
    /// </summary>
    public int MaxLines
    {
        get => GetValue(MaxLinesProperty);
        set => SetValue(MaxLinesProperty, value);
    }

    /// <summary>
    /// Gets or sets whether command input is enabled.
    /// </summary>
    public bool IsInputEnabled
    {
        get => GetValue(IsInputEnabledProperty);
        set => SetValue(IsInputEnabledProperty, value);
    }

    /// <summary>
    /// Gets or sets whether to show the prompt prefix.
    /// </summary>
    public bool ShowPrompt
    {
        get => GetValue(ShowPromptProperty);
        set => SetValue(ShowPromptProperty, value);
    }

    /// <summary>
    /// Gets or sets the terminal font family.
    /// </summary>
    public FontFamily TerminalFontFamily
    {
        get => GetValue(TerminalFontFamilyProperty);
        set => SetValue(TerminalFontFamilyProperty, value);
    }

    /// <summary>
    /// Gets or sets the terminal font size.
    /// </summary>
    public double TerminalFontSize
    {
        get => GetValue(TerminalFontSizeProperty);
        set => SetValue(TerminalFontSizeProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        if (_inputBox is not null)
        {
            _inputBox.KeyDown -= OnInputBoxKeyDown;
        }

        base.OnApplyTemplate(e);

        _outputArea = e.NameScope.Find<ItemsControl>("PART_OutputArea");
        _inputBox = e.NameScope.Find<TextBox>("PART_InputBox");
        _scrollViewer = e.NameScope.Find<ScrollViewer>("PART_ScrollViewer");

        if (_inputBox is not null)
        {
            _inputBox.KeyDown += OnInputBoxKeyDown;
#pragma warning disable CS0618 // Watermark is deprecated but PlaceholderText not available in this version
            _inputBox.Watermark = "Enter command...";
#pragma warning restore CS0618
        }

        UpdatePseudoClasses();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        if (_inputBox is not null)
        {
            _inputBox.KeyDown -= OnInputBoxKeyDown;
        }
    }

    private void OnInputBoxKeyDown(object? sender, KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.Enter:
                SubmitCommand();
                e.Handled = true;
                break;

            case Key.Up:
                NavigateHistory(-1);
                e.Handled = true;
                break;

            case Key.Down:
                NavigateHistory(1);
                e.Handled = true;
                break;

            case Key.C when e.KeyModifiers == KeyModifiers.Control:
                // Ctrl+C: Clear current input or interrupt
                if (_inputBox is not null)
                {
                    _inputBox.Text = string.Empty;
                }
                e.Handled = true;
                break;

            case Key.L when e.KeyModifiers == KeyModifiers.Control:
                // Ctrl+L: Clear terminal
                Clear();
                e.Handled = true;
                break;
        }
    }

    private void OnLinesChanged(AvaloniaPropertyChangedEventArgs e)
    {
        _outputLines.Clear();
        if (e.NewValue is IList<string> lines)
        {
            foreach (var line in lines)
            {
                _outputLines.Add(line);
            }
        }
    }

    /// <summary>
    /// Submits the current input as a command.
    /// </summary>
    public void SubmitCommand()
    {
        var command = _inputBox?.Text;
        if (string.IsNullOrWhiteSpace(command))
            return;

        // Add to history
        _commandHistory.Add(command);
        _historyIndex = _commandHistory.Count;

        // Add command echo to output
        var promptStr = ShowPrompt ? Prompt : string.Empty;
        WriteOutput($"{promptStr}{command}");

        // Raise event
        CommandEntered?.Invoke(this, command);

        // Clear input
        if (_inputBox is not null)
        {
            _inputBox.Text = string.Empty;
        }

        // Enforce max lines
        TrimOutput();
    }

    /// <summary>
    /// Writes a line of output to the terminal.
    /// </summary>
    public void WriteOutput(string line)
    {
        _outputLines.Add(line);
        ScrollToBottom();
    }

    /// <summary>
    /// Writes multiple lines of output to the terminal.
    /// </summary>
    public void WriteOutput(IEnumerable<string> lines)
    {
        foreach (var line in lines)
        {
            _outputLines.Add(line);
        }
        TrimOutput();
        ScrollToBottom();
    }

    /// <summary>
    /// Clears all terminal output.
    /// </summary>
    public void Clear()
    {
        _outputLines.Clear();
    }

    private void NavigateHistory(int direction)
    {
        if (_commandHistory.Count == 0)
            return;

        _historyIndex += direction;
        _historyIndex = Math.Clamp(_historyIndex, 0, _commandHistory.Count - 1);

        if (_inputBox is not null && _historyIndex >= 0 && _historyIndex < _commandHistory.Count)
        {
            _inputBox.Text = _commandHistory[_historyIndex];
            _inputBox.CaretIndex = _inputBox.Text.Length;
        }
    }

    private void TrimOutput()
    {
        var maxLines = MaxLines;
        while (_outputLines.Count > maxLines)
        {
            _outputLines.RemoveAt(0);
        }
    }

    private void ScrollToBottom()
    {
        Dispatcher.UIThread.Post(() =>
        {
            if (_scrollViewer is not null)
            {
                _scrollViewer.Offset = new Vector(_scrollViewer.Offset.X, _scrollViewer.Extent.Height);
            }
        }, DispatcherPriority.Render);
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":readonly", !IsInputEnabled);
        PseudoClasses.Set(":active", IsInputEnabled);
    }
}
