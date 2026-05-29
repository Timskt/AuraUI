using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Layout;

namespace AuraUI.Controls.Layout;

/// <summary>
/// Specifies icon placement relative to the content.
/// </summary>
public enum IconPlacement
{
    Left,
    Right,
    Top,
    Bottom
}

/// <summary>
/// An enhanced <see cref="ContentControl"/> with built-in icon support.
/// Supports icon placement on any side of the content with configurable size and margin.
/// </summary>
[TemplatePart("PART_IconPresenter", typeof(ContentPresenter))]
[TemplatePart("PART_ContentPresenter", typeof(ContentPresenter))]
[PseudoClasses(":has-icon", ":no-icon", ":icon-left", ":icon-right", ":icon-top", ":icon-bottom")]
public class ContentControlX : ContentControl
{
    /// <summary>
    /// Defines the <see cref="Icon"/> styled property.
    /// </summary>
    public static readonly StyledProperty<object?> IconProperty =
        AvaloniaProperty.Register<ContentControlX, object?>(nameof(Icon));

    /// <summary>
    /// Defines the <see cref="IconPlacement"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IconPlacement> IconPlacementProperty =
        AvaloniaProperty.Register<ContentControlX, IconPlacement>(
            nameof(IconPlacement),
            IconPlacement.Left);

    /// <summary>
    /// Defines the <see cref="IconSize"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> IconSizeProperty =
        AvaloniaProperty.Register<ContentControlX, double>(
            nameof(IconSize),
            16.0);

    /// <summary>
    /// Defines the <see cref="IconMargin"/> styled property.
    /// </summary>
    public static readonly StyledProperty<Thickness> IconMarginProperty =
        AvaloniaProperty.Register<ContentControlX, Thickness>(
            nameof(IconMargin),
            new Thickness(0, 0, 8, 0));

    /// <summary>
    /// Defines the <see cref="IconTemplate"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IDataTemplate?> IconTemplateProperty =
        AvaloniaProperty.Register<ContentControlX, IDataTemplate?>(nameof(IconTemplate));

    static ContentControlX()
    {
        IconProperty.Changed.AddClassHandler<ContentControlX>((x, _) => x.UpdatePseudoClasses());
        IconPlacementProperty.Changed.AddClassHandler<ContentControlX>((x, _) => x.UpdatePseudoClasses());
    }

    /// <summary>
    /// Gets or sets the icon content.
    /// </summary>
    public object? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    /// <summary>
    /// Gets or sets where the icon is placed relative to the content.
    /// </summary>
    public IconPlacement IconPlacement
    {
        get => GetValue(IconPlacementProperty);
        set => SetValue(IconPlacementProperty, value);
    }

    /// <summary>
    /// Gets or sets the size of the icon in device-independent pixels.
    /// </summary>
    public double IconSize
    {
        get => GetValue(IconSizeProperty);
        set => SetValue(IconSizeProperty, value);
    }

    /// <summary>
    /// Gets or sets the margin around the icon.
    /// </summary>
    public Thickness IconMargin
    {
        get => GetValue(IconMarginProperty);
        set => SetValue(IconMarginProperty, value);
    }

    /// <summary>
    /// Gets or sets the data template used to render the icon.
    /// </summary>
    public IDataTemplate? IconTemplate
    {
        get => GetValue(IconTemplateProperty);
        set => SetValue(IconTemplateProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        UpdatePseudoClasses();
    }

    private void UpdatePseudoClasses()
    {
        var hasIcon = Icon != null;
        PseudoClasses.Set(":has-icon", hasIcon);
        PseudoClasses.Set(":no-icon", !hasIcon);

        PseudoClasses.Set(":icon-left", IconPlacement == IconPlacement.Left);
        PseudoClasses.Set(":icon-right", IconPlacement == IconPlacement.Right);
        PseudoClasses.Set(":icon-top", IconPlacement == IconPlacement.Top);
        PseudoClasses.Set(":icon-bottom", IconPlacement == IconPlacement.Bottom);
    }
}
