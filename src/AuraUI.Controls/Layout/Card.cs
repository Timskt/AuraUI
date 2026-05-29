using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Controls.Templates;


namespace AuraUI.Controls.Layout;

/// <summary>
/// A container control with distinct header, content, and footer areas,
/// supporting elevation shadow and hover effects.
/// </summary>
[TemplatePart("PART_HeaderPresenter", typeof(ContentPresenter))]
[TemplatePart("PART_ContentPresenter", typeof(ContentPresenter))]
[TemplatePart("PART_FooterPresenter", typeof(ContentPresenter))]
[PseudoClasses(":pointerover")]
public class Card : ContentControl
{
    /// <summary>
    /// Defines the <see cref="Header"/> styled property.
    /// </summary>
    public static readonly StyledProperty<object?> HeaderProperty =
        AvaloniaProperty.Register<Card, object?>(nameof(Header));

    /// <summary>
    /// Defines the <see cref="Footer"/> styled property.
    /// </summary>
    public static readonly StyledProperty<object?> FooterProperty =
        AvaloniaProperty.Register<Card, object?>(nameof(Footer));

    /// <summary>
    /// Defines the <see cref="IsHoverable"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsHoverableProperty =
        AvaloniaProperty.Register<Card, bool>(nameof(IsHoverable));

    /// <summary>
    /// Defines the <see cref="Elevation"/> styled property.
    /// </summary>
    public static readonly StyledProperty<int> ElevationProperty =
        AvaloniaProperty.Register<Card, int>(nameof(Elevation), 1, coerce: (o, v) => CoerceElevation((Card)o, v));

    /// <summary>
    /// Defines the <see cref="BoxShadow"/> styled property.
    /// </summary>
    public static readonly StyledProperty<BoxShadows> BoxShadowProperty =
        AvaloniaProperty.Register<Card, BoxShadows>(nameof(BoxShadow));

    /// <summary>
    /// Defines the <see cref="HeaderTemplate"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IDataTemplate?> HeaderTemplateProperty =
        AvaloniaProperty.Register<Card, IDataTemplate?>(nameof(HeaderTemplate));

    /// <summary>
    /// Defines the <see cref="FooterTemplate"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IDataTemplate?> FooterTemplateProperty =
        AvaloniaProperty.Register<Card, IDataTemplate?>(nameof(FooterTemplate));

    /// <summary>
    /// Defines the <see cref="HeaderBackground"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> HeaderBackgroundProperty =
        AvaloniaProperty.Register<Card, IBrush?>(nameof(HeaderBackground));

    /// <summary>
    /// Defines the <see cref="FooterBackground"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> FooterBackgroundProperty =
        AvaloniaProperty.Register<Card, IBrush?>(nameof(FooterBackground));

    static Card()
    {
        IsHoverableProperty.Changed.AddClassHandler<Card>((x, _) => x.UpdatePseudoClasses());
        ElevationProperty.Changed.AddClassHandler<Card>((x, e) =>
        {
            if (e.OldValue != e.NewValue)
            {
                x.InvalidateVisual();
            }
        });
    }

    /// <summary>
    /// Gets or sets the header content.
    /// </summary>
    public object? Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    /// <summary>
    /// Gets or sets the footer content.
    /// </summary>
    public object? Footer
    {
        get => GetValue(FooterProperty);
        set => SetValue(FooterProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the card shows a hover effect when the pointer enters.
    /// </summary>
    public bool IsHoverable
    {
        get => GetValue(IsHoverableProperty);
        set => SetValue(IsHoverableProperty, value);
    }

    /// <summary>
    /// Gets or sets the elevation level (0-3) controlling shadow depth.
    /// </summary>
    public int Elevation
    {
        get => GetValue(ElevationProperty);
        set => SetValue(ElevationProperty, value);
    }

    /// <summary>
    /// Gets or sets the box shadow effect.
    /// </summary>
    public BoxShadows BoxShadow
    {
        get => GetValue(BoxShadowProperty);
        set => SetValue(BoxShadowProperty, value);
    }

    /// <summary>
    /// Gets or sets the data template for the header.
    /// </summary>
    public IDataTemplate? HeaderTemplate
    {
        get => GetValue(HeaderTemplateProperty);
        set => SetValue(HeaderTemplateProperty, value);
    }

    /// <summary>
    /// Gets or sets the data template for the footer.
    /// </summary>
    public IDataTemplate? FooterTemplate
    {
        get => GetValue(FooterTemplateProperty);
        set => SetValue(FooterTemplateProperty, value);
    }

    /// <summary>
    /// Gets or sets the background brush for the header area.
    /// </summary>
    public IBrush? HeaderBackground
    {
        get => GetValue(HeaderBackgroundProperty);
        set => SetValue(HeaderBackgroundProperty, value);
    }

    /// <summary>
    /// Gets or sets the background brush for the footer area.
    /// </summary>
    public IBrush? FooterBackground
    {
        get => GetValue(FooterBackgroundProperty);
        set => SetValue(FooterBackgroundProperty, value);
    }

    private static int CoerceElevation(Card sender, int value)
    {
        return Math.Clamp(value, 0, 3);
    }

    private void UpdatePseudoClasses()
    {
        // When IsHoverable changes, clear the pointer-over state if now disabled.
        if (!IsHoverable)
        {
            PseudoClasses.Set(":pointerover", false);
        }
    }

    protected override void OnPointerEntered(PointerEventArgs e)
    {
        base.OnPointerEntered(e);
        if (IsHoverable)
        {
            PseudoClasses.Set(":pointerover", true);
        }
    }

    protected override void OnPointerExited(PointerEventArgs e)
    {
        base.OnPointerExited(e);
        if (IsHoverable)
        {
            PseudoClasses.Set(":pointerover", false);
        }
    }
}
