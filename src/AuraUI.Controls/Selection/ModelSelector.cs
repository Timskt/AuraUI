using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace AuraUI.Controls.Selection;

/// <summary>
/// Represents information about an AI model.
/// </summary>
public class ModelInfo : AvaloniaObject
{
    /// <summary>
    /// Defines the <see cref="Name"/> property.
    /// </summary>
    public static readonly StyledProperty<string?> NameProperty =
        AvaloniaProperty.Register<ModelInfo, string?>(nameof(Name));

    /// <summary>
    /// Defines the <see cref="Provider"/> property.
    /// </summary>
    public static readonly StyledProperty<string?> ProviderProperty =
        AvaloniaProperty.Register<ModelInfo, string?>(nameof(Provider));

    /// <summary>
    /// Defines the <see cref="ContextLength"/> property.
    /// </summary>
    public static readonly StyledProperty<int> ContextLengthProperty =
        AvaloniaProperty.Register<ModelInfo, int>(nameof(ContextLength));

    /// <summary>
    /// Defines the <see cref="Pricing"/> property.
    /// </summary>
    public static readonly StyledProperty<string?> PricingProperty =
        AvaloniaProperty.Register<ModelInfo, string?>(nameof(Pricing));

    /// <summary>
    /// Defines the <see cref="Description"/> property.
    /// </summary>
    public static readonly StyledProperty<string?> DescriptionProperty =
        AvaloniaProperty.Register<ModelInfo, string?>(nameof(Description));

    /// <summary>
    /// Gets or sets the model name.
    /// </summary>
    public string? Name
    {
        get => GetValue(NameProperty);
        set => SetValue(NameProperty, value);
    }

    /// <summary>
    /// Gets or sets the model provider (e.g., OpenAI, Anthropic, Google).
    /// </summary>
    public string? Provider
    {
        get => GetValue(ProviderProperty);
        set => SetValue(ProviderProperty, value);
    }

    /// <summary>
    /// Gets or sets the maximum context length in tokens.
    /// </summary>
    public int ContextLength
    {
        get => GetValue(ContextLengthProperty);
        set => SetValue(ContextLengthProperty, value);
    }

    /// <summary>
    /// Gets or sets the pricing string (e.g., "$0.01 / 1K tokens").
    /// </summary>
    public string? Pricing
    {
        get => GetValue(PricingProperty);
        set => SetValue(PricingProperty, value);
    }

    /// <summary>
    /// Gets or sets a short description of the model.
    /// </summary>
    public string? Description
    {
        get => GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }
}

/// <summary>
/// A dropdown selector for AI models that displays model details including
/// provider, context length, pricing, and description.
///
/// Template parts:
///   PART_DropdownToggle   - Button that opens the dropdown
///   PART_DropdownPopup    - Popup containing the model list
///   PART_ModelList        - ItemsControl listing available models
///   PART_SelectedDisplay  - TextBlock showing the selected model name
///
/// Pseudo-classes: :open, :selected, :has-pricing, :has-context
/// </summary>
[TemplatePart("PART_DropdownToggle", typeof(Button))]
[TemplatePart("PART_DropdownPopup", typeof(Popup))]
[TemplatePart("PART_ModelList", typeof(ItemsControl))]
[TemplatePart("PART_SelectedDisplay", typeof(TextBlock))]
[PseudoClasses(":open", ":selected", ":has-pricing", ":has-context")]
public class ModelSelector : TemplatedControl
{
    private Button? _dropdownToggle;
    private Popup? _dropdownPopup;
    private ItemsControl? _modelList;
    private TextBlock? _selectedDisplay;

    /// <summary>
    /// Defines the <see cref="Models"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IList<ModelInfo>?> ModelsProperty =
        AvaloniaProperty.Register<ModelSelector, IList<ModelInfo>?>(nameof(Models));

    /// <summary>
    /// Defines the <see cref="SelectedModel"/> styled property.
    /// </summary>
    public static readonly StyledProperty<ModelInfo?> SelectedModelProperty =
        AvaloniaProperty.Register<ModelSelector, ModelInfo?>(nameof(SelectedModel));

    /// <summary>
    /// Defines the <see cref="ShowPricing"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowPricingProperty =
        AvaloniaProperty.Register<ModelSelector, bool>(nameof(ShowPricing), true);

    /// <summary>
    /// Defines the <see cref="ShowContextLength"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowContextLengthProperty =
        AvaloniaProperty.Register<ModelSelector, bool>(nameof(ShowContextLength), true);

    /// <summary>
    /// Defines the <see cref="PlaceholderText"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> PlaceholderTextProperty =
        AvaloniaProperty.Register<ModelSelector, string?>(nameof(PlaceholderText), "Select a model...");

    /// <summary>
    /// Defines the <see cref="IsDropDownOpen"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsDropDownOpenProperty =
        AvaloniaProperty.Register<ModelSelector, bool>(nameof(IsDropDownOpen));

    /// <summary>
    /// Raised when the selected model changes.
    /// </summary>
    public event EventHandler<ModelInfo?>? SelectionChanged;

    static ModelSelector()
    {
        SelectedModelProperty.Changed.AddClassHandler<ModelSelector>((x, _) => x.OnSelectedModelChanged());
        IsDropDownOpenProperty.Changed.AddClassHandler<ModelSelector>((x, _) => x.OnDropDownOpenChanged());
    }

    /// <summary>
    /// Gets or sets the collection of available models.
    /// </summary>
    public IList<ModelInfo>? Models
    {
        get => GetValue(ModelsProperty);
        set => SetValue(ModelsProperty, value);
    }

    /// <summary>
    /// Gets or sets the currently selected model.
    /// </summary>
    public ModelInfo? SelectedModel
    {
        get => GetValue(SelectedModelProperty);
        set => SetValue(SelectedModelProperty, value);
    }

    /// <summary>
    /// Gets or sets whether to display model pricing information.
    /// </summary>
    public bool ShowPricing
    {
        get => GetValue(ShowPricingProperty);
        set => SetValue(ShowPricingProperty, value);
    }

    /// <summary>
    /// Gets or sets whether to display context length information.
    /// </summary>
    public bool ShowContextLength
    {
        get => GetValue(ShowContextLengthProperty);
        set => SetValue(ShowContextLengthProperty, value);
    }

    /// <summary>
    /// Gets or sets the placeholder text when no model is selected.
    /// </summary>
    public string? PlaceholderText
    {
        get => GetValue(PlaceholderTextProperty);
        set => SetValue(PlaceholderTextProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the dropdown popup is open.
    /// </summary>
    public bool IsDropDownOpen
    {
        get => GetValue(IsDropDownOpenProperty);
        set => SetValue(IsDropDownOpenProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        if (_dropdownToggle is not null)
            _dropdownToggle.Click -= OnDropdownToggleClick;

        base.OnApplyTemplate(e);

        _dropdownToggle = e.NameScope.Find<Button>("PART_DropdownToggle");
        _dropdownPopup = e.NameScope.Find<Popup>("PART_DropdownPopup");
        _modelList = e.NameScope.Find<ItemsControl>("PART_ModelList");
        _selectedDisplay = e.NameScope.Find<TextBlock>("PART_SelectedDisplay");

        if (_dropdownToggle is not null)
        {
            _dropdownToggle.Click += OnDropdownToggleClick;
        }

        UpdateSelectedDisplay();
        UpdatePseudoClasses();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        if (_dropdownToggle is not null)
            _dropdownToggle.Click -= OnDropdownToggleClick;
    }

    private void OnDropdownToggleClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        IsDropDownOpen = !IsDropDownOpen;
    }

    private void OnSelectedModelChanged()
    {
        UpdateSelectedDisplay();
        UpdatePseudoClasses();
        SelectionChanged?.Invoke(this, SelectedModel);
    }

    private void OnDropDownOpenChanged()
    {
        PseudoClasses.Set(":open", IsDropDownOpen);
        if (_dropdownPopup is not null)
        {
            _dropdownPopup.IsOpen = IsDropDownOpen;
        }
    }

    /// <summary>
    /// Selects a model and closes the dropdown.
    /// </summary>
    public void SelectModel(ModelInfo model)
    {
        SelectedModel = model;
        IsDropDownOpen = false;
    }

    private void UpdateSelectedDisplay()
    {
        if (_selectedDisplay is not null)
        {
            if (SelectedModel is not null)
            {
                _selectedDisplay.Text = SelectedModel.Name;
            }
            else
            {
                _selectedDisplay.Text = PlaceholderText;
            }
        }

        PseudoClasses.Set(":selected", SelectedModel is not null);
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":selected", SelectedModel is not null);
        PseudoClasses.Set(":has-pricing", ShowPricing);
        PseudoClasses.Set(":has-context", ShowContextLength);
    }
}
