using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;

namespace AuraUI.Controls.Layout;

/// <summary>
/// Specifies where the header label is placed relative to the field content.
/// </summary>
public enum FormGroupHeaderPlacement
{
    Left,
    Top
}

/// <summary>
/// A form layout control that arranges a label and field vertically or horizontally.
/// Supports required field indicators and helper text.
/// </summary>
[TemplatePart("PART_HeaderPresenter", typeof(ContentPresenter))]
[TemplatePart("PART_ContentPresenter", typeof(ContentPresenter))]
[TemplatePart("PART_HelperTextPresenter", typeof(ContentPresenter))]
[PseudoClasses(":required", ":left", ":top", ":has-helper")]
public class FormGroup : ContentControl
{
    /// <summary>
    /// Defines the <see cref="Header"/> styled property.
    /// </summary>
    public static readonly StyledProperty<object?> HeaderProperty =
        AvaloniaProperty.Register<FormGroup, object?>(nameof(Header));

    /// <summary>
    /// Defines the <see cref="HeaderTemplate"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IDataTemplate?> HeaderTemplateProperty =
        AvaloniaProperty.Register<FormGroup, IDataTemplate?>(nameof(HeaderTemplate));

    /// <summary>
    /// Defines the <see cref="HeaderWidth"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> HeaderWidthProperty =
        AvaloniaProperty.Register<FormGroup, double>(nameof(HeaderWidth), 120);

    /// <summary>
    /// Defines the <see cref="HeaderPlacement"/> styled property.
    /// </summary>
    public static readonly StyledProperty<FormGroupHeaderPlacement> HeaderPlacementProperty =
        AvaloniaProperty.Register<FormGroup, FormGroupHeaderPlacement>(
            nameof(HeaderPlacement),
            FormGroupHeaderPlacement.Left);

    /// <summary>
    /// Defines the <see cref="IsRequired"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsRequiredProperty =
        AvaloniaProperty.Register<FormGroup, bool>(nameof(IsRequired), false);

    /// <summary>
    /// Defines the <see cref="HelperText"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> HelperTextProperty =
        AvaloniaProperty.Register<FormGroup, string?>(nameof(HelperText));

    /// <summary>
    /// Defines the <see cref="ErrorText"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> ErrorTextProperty =
        AvaloniaProperty.Register<FormGroup, string?>(nameof(ErrorText));

    static FormGroup()
    {
        IsRequiredProperty.Changed.AddClassHandler<FormGroup>((x, _) => x.UpdatePseudoClasses());
        HeaderPlacementProperty.Changed.AddClassHandler<FormGroup>((x, _) => x.UpdatePseudoClasses());
        HelperTextProperty.Changed.AddClassHandler<FormGroup>((x, _) => x.UpdatePseudoClasses());
        ErrorTextProperty.Changed.AddClassHandler<FormGroup>((x, _) => x.UpdatePseudoClasses());
    }

    /// <summary>
    /// Gets or sets the header content (typically a text label).
    /// </summary>
    public object? Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
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
    /// Gets or sets the width allocated for the header when <see cref="HeaderPlacement"/> is Left.
    /// </summary>
    public double HeaderWidth
    {
        get => GetValue(HeaderWidthProperty);
        set => SetValue(HeaderWidthProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the header is placed to the left or above the content.
    /// </summary>
    public FormGroupHeaderPlacement HeaderPlacement
    {
        get => GetValue(HeaderPlacementProperty);
        set => SetValue(HeaderPlacementProperty, value);
    }

    /// <summary>
    /// Gets or sets whether this form field is required (shows an indicator).
    /// </summary>
    public bool IsRequired
    {
        get => GetValue(IsRequiredProperty);
        set => SetValue(IsRequiredProperty, value);
    }

    /// <summary>
    /// Gets or sets optional helper text shown below the field.
    /// </summary>
    public string? HelperText
    {
        get => GetValue(HelperTextProperty);
        set => SetValue(HelperTextProperty, value);
    }

    /// <summary>
    /// Gets or sets error text shown below the field (replaces helper text).
    /// </summary>
    public string? ErrorText
    {
        get => GetValue(ErrorTextProperty);
        set => SetValue(ErrorTextProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        UpdatePseudoClasses();
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":required", IsRequired);
        PseudoClasses.Set(":left", HeaderPlacement == FormGroupHeaderPlacement.Left);
        PseudoClasses.Set(":top", HeaderPlacement == FormGroupHeaderPlacement.Top);
        PseudoClasses.Set(":has-helper", !string.IsNullOrEmpty(HelperText) || !string.IsNullOrEmpty(ErrorText));
    }
}
