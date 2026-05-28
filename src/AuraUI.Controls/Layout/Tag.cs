using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Controls.Templates;

namespace AuraUI.Controls.Layout;

/// <summary>
/// A tag component for categorizing or marking items with optional icon and close button.
/// </summary>
[TemplatePart("PART_CloseButton", typeof(Button))]
[TemplatePart("PART_IconPresenter", typeof(ContentPresenter))]
[PseudoClasses(":closable", ":primary", ":success", ":warning", ":error", ":default", ":outlined")]
public class Tag : ContentControl
{
    /// <summary>
    /// Defines the <see cref="IsClosable"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsClosableProperty =
        AvaloniaProperty.Register<Tag, bool>(nameof(IsClosable));

    /// <summary>
    /// Defines the <see cref="Icon"/> styled property.
    /// </summary>
    public static readonly StyledProperty<object?> IconProperty =
        AvaloniaProperty.Register<Tag, object?>(nameof(Icon));

    /// <summary>
    /// Defines the <see cref="IconTemplate"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IDataTemplate?> IconTemplateProperty =
        AvaloniaProperty.Register<Tag, IDataTemplate?>(nameof(IconTemplate));

    /// <summary>
    /// Defines the <see cref="Variant"/> styled property.
    /// </summary>
    public static readonly StyledProperty<TagVariant> VariantProperty =
        AvaloniaProperty.Register<Tag, TagVariant>(
            nameof(Variant),
            TagVariant.Default);

    /// <summary>
    /// Defines the <see cref="CloseButtonContent"/> styled property.
    /// </summary>
    public static readonly StyledProperty<object?> CloseButtonContentProperty =
        AvaloniaProperty.Register<Tag, object?>(nameof(CloseButtonContent));

    private Button? _closeButton;

    static Tag()
    {
        IsClosableProperty.Changed.AddClassHandler<Tag>((x, _) => x.UpdatePseudoClasses());
        VariantProperty.Changed.AddClassHandler<Tag>((x, _) => x.UpdatePseudoClasses());
    }

    /// <summary>
    /// Gets or sets whether a close button is shown.
    /// </summary>
    public bool IsClosable
    {
        get => GetValue(IsClosableProperty);
        set => SetValue(IsClosableProperty, value);
    }

    /// <summary>
    /// Gets or sets the icon content displayed before the tag text.
    /// </summary>
    public object? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    /// <summary>
    /// Gets or sets the data template for the icon.
    /// </summary>
    public IDataTemplate? IconTemplate
    {
        get => GetValue(IconTemplateProperty);
        set => SetValue(IconTemplateProperty, value);
    }

    /// <summary>
    /// Gets or sets the visual variant.
    /// </summary>
    public TagVariant Variant
    {
        get => GetValue(VariantProperty);
        set => SetValue(VariantProperty, value);
    }

    /// <summary>
    /// Gets or sets the content of the close button. Defaults to an "x" glyph.
    /// </summary>
    public object? CloseButtonContent
    {
        get => GetValue(CloseButtonContentProperty);
        set => SetValue(CloseButtonContentProperty, value);
    }

    /// <summary>
    /// Occurs when the user clicks the close button.
    /// </summary>
    public event EventHandler<RoutedEventArgs>? Closed;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        if (_closeButton != null)
        {
            _closeButton.Click -= OnCloseButtonClick;
        }

        _closeButton = e.NameScope.Find<Button>("PART_CloseButton");

        if (_closeButton != null)
        {
            _closeButton.Click += OnCloseButtonClick;
        }

        UpdatePseudoClasses();
    }

    private void OnCloseButtonClick(object? sender, RoutedEventArgs e)
    {
        e.Handled = true;
        Closed?.Invoke(this, e);
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":closable", IsClosable);

        PseudoClasses.Set(":default", Variant == TagVariant.Default);
        PseudoClasses.Set(":primary", Variant == TagVariant.Primary);
        PseudoClasses.Set(":success", Variant == TagVariant.Success);
        PseudoClasses.Set(":warning", Variant == TagVariant.Warning);
        PseudoClasses.Set(":error", Variant == TagVariant.Error);
        PseudoClasses.Set(":outlined", Variant == TagVariant.Outlined);
    }
}
