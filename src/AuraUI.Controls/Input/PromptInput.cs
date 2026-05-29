using System;
using System.Windows.Input;
using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace AuraUI.Controls.Input;

/// <summary>
/// An AI prompt input control with token counting, model selection, and temperature control.
/// Designed for submitting prompts to large language models.
///
/// Template parts:
///   PART_InputBox         - TextBox for composing the prompt
///   PART_SubmitButton     - Button to submit the prompt
///   PART_TokenCount       - TextBlock displaying token/character count
///   PART_ModelSelector    - ComboBox for selecting the model
///   PART_TemperatureSlider - Slider for temperature setting
///
/// Pseudo-classes: :loading, :empty, :focus
/// </summary>
[TemplatePart("PART_InputBox", typeof(TextBox))]
[TemplatePart("PART_SubmitButton", typeof(Button))]
[TemplatePart("PART_TokenCount", typeof(TextBlock))]
[TemplatePart("PART_ModelSelector", typeof(ComboBox))]
[TemplatePart("PART_TemperatureSlider", typeof(Slider))]
[PseudoClasses(":loading", ":empty", ":focus")]
public class PromptInput : TemplatedControl
{
    private TextBox? _inputBox;
    private Button? _submitButton;
    private TextBlock? _tokenCount;
    private ComboBox? _modelSelector;
    private Slider? _temperatureSlider;

    /// <summary>
    /// Defines the <see cref="Placeholder"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> PlaceholderProperty =
        AvaloniaProperty.Register<PromptInput, string?>(nameof(Placeholder), "Enter your prompt...");

    /// <summary>
    /// Defines the <see cref="MaxTokens"/> styled property.
    /// </summary>
    public static readonly StyledProperty<int> MaxTokensProperty =
        AvaloniaProperty.Register<PromptInput, int>(nameof(MaxTokens), 4096);

    /// <summary>
    /// Defines the <see cref="ShowTokenCount"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowTokenCountProperty =
        AvaloniaProperty.Register<PromptInput, bool>(nameof(ShowTokenCount), true);

    /// <summary>
    /// Defines the <see cref="IsLoading"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsLoadingProperty =
        AvaloniaProperty.Register<PromptInput, bool>(nameof(IsLoading));

    /// <summary>
    /// Defines the <see cref="ModelSelector"/> styled property.
    /// When set, overrides the built-in model selector with a custom items source.
    /// </summary>
    public static readonly StyledProperty<object?> ModelSelectorProperty =
        AvaloniaProperty.Register<PromptInput, object?>(nameof(ModelSelector));

    /// <summary>
    /// Defines the <see cref="SelectedModel"/> styled property.
    /// </summary>
    public static readonly StyledProperty<object?> SelectedModelProperty =
        AvaloniaProperty.Register<PromptInput, object?>(nameof(SelectedModel));

    /// <summary>
    /// Defines the <see cref="Temperature"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> TemperatureProperty =
        AvaloniaProperty.Register<PromptInput, double>(nameof(Temperature), 0.7);

    /// <summary>
    /// Defines the <see cref="SubmitCommand"/> styled property.
    /// </summary>
    public static readonly StyledProperty<ICommand?> SubmitCommandProperty =
        AvaloniaProperty.Register<PromptInput, ICommand?>(nameof(SubmitCommand));

    /// <summary>
    /// Defines the <see cref="ShowModelSelector"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowModelSelectorProperty =
        AvaloniaProperty.Register<PromptInput, bool>(nameof(ShowModelSelector), true);

    /// <summary>
    /// Defines the <see cref="ShowTemperature"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowTemperatureProperty =
        AvaloniaProperty.Register<PromptInput, bool>(nameof(ShowTemperature));

    /// <summary>
    /// Raised when the prompt is submitted.
    /// </summary>
    public event EventHandler<string>? PromptSubmitted;

    static PromptInput()
    {
        IsLoadingProperty.Changed.AddClassHandler<PromptInput>((x, _) => x.UpdatePseudoClasses());
    }

    /// <summary>
    /// Gets or sets the placeholder text.
    /// </summary>
    public string? Placeholder
    {
        get => GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
    }

    /// <summary>
    /// Gets or sets the maximum token limit for the prompt.
    /// </summary>
    public int MaxTokens
    {
        get => GetValue(MaxTokensProperty);
        set => SetValue(MaxTokensProperty, value);
    }

    /// <summary>
    /// Gets or sets whether to display a token/character count.
    /// </summary>
    public bool ShowTokenCount
    {
        get => GetValue(ShowTokenCountProperty);
        set => SetValue(ShowTokenCountProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the control is in a loading state.
    /// </summary>
    public bool IsLoading
    {
        get => GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }

    /// <summary>
    /// Gets or sets the items source for the model selector dropdown.
    /// </summary>
    public object? ModelSelector
    {
        get => GetValue(ModelSelectorProperty);
        set => SetValue(ModelSelectorProperty, value);
    }

    /// <summary>
    /// Gets or sets the currently selected model.
    /// </summary>
    public object? SelectedModel
    {
        get => GetValue(SelectedModelProperty);
        set => SetValue(SelectedModelProperty, value);
    }

    /// <summary>
    /// Gets or sets the temperature (0.0 - 2.0) for generation.
    /// </summary>
    public double Temperature
    {
        get => GetValue(TemperatureProperty);
        set => SetValue(TemperatureProperty, value);
    }

    /// <summary>
    /// Gets or sets the command invoked when the prompt is submitted.
    /// </summary>
    public ICommand? SubmitCommand
    {
        get => GetValue(SubmitCommandProperty);
        set => SetValue(SubmitCommandProperty, value);
    }

    /// <summary>
    /// Gets or sets whether to show the model selector dropdown.
    /// </summary>
    public bool ShowModelSelector
    {
        get => GetValue(ShowModelSelectorProperty);
        set => SetValue(ShowModelSelectorProperty, value);
    }

    /// <summary>
    /// Gets or sets whether to show the temperature slider.
    /// </summary>
    public bool ShowTemperature
    {
        get => GetValue(ShowTemperatureProperty);
        set => SetValue(ShowTemperatureProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        // Detach old parts
        if (_submitButton is not null)
            _submitButton.Click -= OnSubmitButtonClick;
        if (_inputBox is not null)
        {
            _inputBox.KeyDown -= OnInputBoxKeyDown;
            _inputBox.TextChanged -= OnInputBoxTextChanged;
        }

        base.OnApplyTemplate(e);

        _inputBox = e.NameScope.Find<TextBox>("PART_InputBox");
        _submitButton = e.NameScope.Find<Button>("PART_SubmitButton");
        _tokenCount = e.NameScope.Find<TextBlock>("PART_TokenCount");
        _modelSelector = e.NameScope.Find<ComboBox>("PART_ModelSelector");
        _temperatureSlider = e.NameScope.Find<Slider>("PART_TemperatureSlider");

        if (_submitButton is not null)
        {
            _submitButton.Click += OnSubmitButtonClick;
            _submitButton.SetValue(AutomationProperties.NameProperty, "Submit prompt");
        }

        if (_inputBox is not null)
        {
            _inputBox.KeyDown += OnInputBoxKeyDown;
            _inputBox.TextChanged += OnInputBoxTextChanged;
            _inputBox.Watermark = Placeholder;
        }

        UpdatePseudoClasses();
        UpdateTokenCount();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        if (_submitButton is not null)
            _submitButton.Click -= OnSubmitButtonClick;
        if (_inputBox is not null)
        {
            _inputBox.KeyDown -= OnInputBoxKeyDown;
            _inputBox.TextChanged -= OnInputBoxTextChanged;
        }
    }

    private void OnInputBoxKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            SubmitPrompt();
            e.Handled = true;
        }
    }

    private void OnInputBoxTextChanged(object? sender, TextChangedEventArgs e)
    {
        UpdateTokenCount();
        PseudoClasses.Set(":empty", string.IsNullOrEmpty(_inputBox?.Text));
    }

    private void OnSubmitButtonClick(object? sender, RoutedEventArgs e)
    {
        SubmitPrompt();
    }

    /// <summary>
    /// Submits the current prompt text.
    /// </summary>
    public void SubmitPrompt()
    {
        var text = _inputBox?.Text;
        if (string.IsNullOrWhiteSpace(text))
            return;

        PromptSubmitted?.Invoke(this, text);

        if (SubmitCommand?.CanExecute(text) == true)
        {
            SubmitCommand.Execute(text);
        }
    }

    private void UpdateTokenCount()
    {
        if (_tokenCount is not null && ShowTokenCount)
        {
            var charCount = _inputBox?.Text?.Length ?? 0;
            // Rough token estimate: ~4 characters per token for English text
            var estimatedTokens = charCount / 4;
            _tokenCount.Text = $"{estimatedTokens} tokens ({charCount} chars)";
            _tokenCount.IsVisible = true;
        }
        else if (_tokenCount is not null)
        {
            _tokenCount.IsVisible = false;
        }
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":loading", IsLoading);
    }
}
