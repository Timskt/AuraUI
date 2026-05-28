using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;

namespace AuraUI.Controls.Layout;

/// <summary>
/// A small status descriptor for UI elements. Supports dot, standard (count),
/// and custom content modes.
/// </summary>
[TemplatePart("PART_BadgeContainer", typeof(Border))]
[TemplatePart("PART_BadgeContentPresenter", typeof(ContentPresenter))]
[PseudoClasses(":dot", ":standard", ":primary", ":success", ":warning", ":error", ":default")]
public class Badge : ContentControl
{
    /// <summary>
    /// Defines the <see cref="BadgeContent"/> styled property.
    /// </summary>
    public static readonly StyledProperty<object?> BadgeContentProperty =
        AvaloniaProperty.Register<Badge, object?>(nameof(BadgeContent));

    /// <summary>
    /// Defines the <see cref="Value"/> styled property.
    /// </summary>
    public static readonly StyledProperty<int?> ValueProperty =
        AvaloniaProperty.Register<Badge, int?>(nameof(Value));

    /// <summary>
    /// Defines the <see cref="MaxValue"/> styled property.
    /// </summary>
    public static readonly StyledProperty<int> MaxValueProperty =
        AvaloniaProperty.Register<Badge, int>(nameof(MaxValue), 99);

    /// <summary>
    /// Defines the <see cref="Variant"/> styled property.
    /// </summary>
    public static readonly StyledProperty<BadgeVariant> VariantProperty =
        AvaloniaProperty.Register<Badge, BadgeVariant>(
            nameof(Variant),
            BadgeVariant.Default);

    /// <summary>
    /// Defines the <see cref="IsDot"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsDotProperty =
        AvaloniaProperty.Register<Badge, bool>(nameof(IsDot));

    /// <summary>
    /// Defines the <see cref="BadgeContentTemplate"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IDataTemplate?> BadgeContentTemplateProperty =
        AvaloniaProperty.Register<Badge, IDataTemplate?>(nameof(BadgeContentTemplate));

    private Border? _badgeContainer;
    private ContentPresenter? _badgeContentPresenter;

    static Badge()
    {
        IsDotProperty.Changed.AddClassHandler<Badge>((x, _) => x.UpdatePseudoClasses());
        VariantProperty.Changed.AddClassHandler<Badge>((x, _) => x.UpdatePseudoClasses());
        ValueProperty.Changed.AddClassHandler<Badge>((x, _) => x.UpdateDisplayedContent());
        BadgeContentProperty.Changed.AddClassHandler<Badge>((x, _) => x.UpdateDisplayedContent());
        MaxValueProperty.Changed.AddClassHandler<Badge>((x, _) => x.UpdateDisplayedContent());
    }

    /// <summary>
    /// Gets or sets custom badge content. When set, this takes precedence over <see cref="Value"/>.
    /// </summary>
    public object? BadgeContent
    {
        get => GetValue(BadgeContentProperty);
        set => SetValue(BadgeContentProperty, value);
    }

    /// <summary>
    /// Gets or sets the numeric value displayed by the badge.
    /// Values above <see cref="MaxValue"/> are displayed as "{MaxValue}+".
    /// </summary>
    public int? Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    /// <summary>
    /// Gets or sets the maximum value before overflow text is shown.
    /// Default is 99 (so 100+ displays as "99+").
    /// </summary>
    public int MaxValue
    {
        get => GetValue(MaxValueProperty);
        set => SetValue(MaxValueProperty, value);
    }

    /// <summary>
    /// Gets or sets the visual variant.
    /// </summary>
    public BadgeVariant Variant
    {
        get => GetValue(VariantProperty);
        set => SetValue(VariantProperty, value);
    }

    /// <summary>
    /// Gets or sets whether to display a small dot instead of content.
    /// </summary>
    public bool IsDot
    {
        get => GetValue(IsDotProperty);
        set => SetValue(IsDotProperty, value);
    }

    /// <summary>
    /// Gets or sets the data template for the badge content.
    /// </summary>
    public IDataTemplate? BadgeContentTemplate
    {
        get => GetValue(BadgeContentTemplateProperty);
        set => SetValue(BadgeContentTemplateProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _badgeContainer = e.NameScope.Find<Border>("PART_BadgeContainer");
        _badgeContentPresenter = e.NameScope.Find<ContentPresenter>("PART_BadgeContentPresenter");

        UpdatePseudoClasses();
        UpdateDisplayedContent();
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":dot", IsDot);
        PseudoClasses.Set(":standard", !IsDot);

        PseudoClasses.Set(":default", Variant == BadgeVariant.Default);
        PseudoClasses.Set(":primary", Variant == BadgeVariant.Primary);
        PseudoClasses.Set(":success", Variant == BadgeVariant.Success);
        PseudoClasses.Set(":warning", Variant == BadgeVariant.Warning);
        PseudoClasses.Set(":error", Variant == BadgeVariant.Error);
    }

    private void UpdateDisplayedContent()
    {
        if (IsDot) return;

        // BadgeContent takes precedence over Value.
        if (BadgeContent != null)
        {
            if (_badgeContentPresenter != null)
            {
                _badgeContentPresenter.Content = BadgeContent;
            }
            return;
        }

        // Format the numeric value.
        if (Value.HasValue)
        {
            var maxValue = MaxValue;
            var displayValue = Value.Value;

            string text;
            if (displayValue > maxValue)
            {
                text = $"{maxValue}+";
            }
            else if (displayValue < 0)
            {
                text = "0";
            }
            else
            {
                text = displayValue.ToString();
            }

            if (_badgeContentPresenter != null)
            {
                _badgeContentPresenter.Content = text;
            }
        }
        else
        {
            if (_badgeContentPresenter != null)
            {
                _badgeContentPresenter.Content = null;
            }
        }
    }
}
