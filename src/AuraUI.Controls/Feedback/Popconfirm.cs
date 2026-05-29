using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;

namespace AuraUI.Controls.Feedback;

/// <summary>
/// A confirmation popover with OK/Cancel buttons that appears on click.
/// Inspired by Ant Design's Popconfirm component.
/// </summary>
[TemplatePart("PART_OKButton", typeof(Button))]
[TemplatePart("PART_CancelButton", typeof(Button))]
[PseudoClasses(":open", ":closed")]
public class Popconfirm : ContentControl
{
    private Button? _okButton;
    private Button? _cancelButton;

    /// <summary>
    /// Defines the <see cref="Title"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<Popconfirm, string?>(nameof(Title));

    /// <summary>
    /// Defines the <see cref="Description"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> DescriptionProperty =
        AvaloniaProperty.Register<Popconfirm, string?>(nameof(Description));

    /// <summary>
    /// Defines the <see cref="Icon"/> styled property.
    /// </summary>
    public static readonly StyledProperty<object?> IconProperty =
        AvaloniaProperty.Register<Popconfirm, object?>(nameof(Icon));

    /// <summary>
    /// Defines the <see cref="OkText"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> OkTextProperty =
        AvaloniaProperty.Register<Popconfirm, string?>(nameof(OkText), "OK");

    /// <summary>
    /// Defines the <see cref="CancelText"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> CancelTextProperty =
        AvaloniaProperty.Register<Popconfirm, string?>(nameof(CancelText), "Cancel");

    /// <summary>
    /// Defines the <see cref="OkButtonTheme"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> OkButtonThemeProperty =
        AvaloniaProperty.Register<Popconfirm, string?>(nameof(OkButtonTheme), "primary");

    /// <summary>
    /// Defines the <see cref="CancelButtonTheme"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> CancelButtonThemeProperty =
        AvaloniaProperty.Register<Popconfirm, string?>(nameof(CancelButtonTheme));

    /// <summary>
    /// Defines the <see cref="Placement"/> styled property.
    /// </summary>
    public static readonly StyledProperty<PlacementMode> PlacementProperty =
        AvaloniaProperty.Register<Popconfirm, PlacementMode>(
            nameof(Placement), PlacementMode.BottomEdgeAlignedLeft);

    /// <summary>
    /// Defines the <see cref="IsOpen"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsOpenProperty =
        AvaloniaProperty.Register<Popconfirm, bool>(nameof(IsOpen));

    /// <summary>
    /// Defines the <see cref="ShowArrow"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowArrowProperty =
        AvaloniaProperty.Register<Popconfirm, bool>(nameof(ShowArrow), true);

    static Popconfirm()
    {
        IsOpenProperty.Changed.AddClassHandler<Popconfirm>((x, _) => x.UpdatePseudoClasses());
    }

    /// <summary>
    /// Gets or sets the confirmation title.
    /// </summary>
    public string? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <summary>
    /// Gets or sets the confirmation description.
    /// </summary>
    public string? Description
    {
        get => GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
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
    /// Gets or sets the OK button text.
    /// </summary>
    public string? OkText
    {
        get => GetValue(OkTextProperty);
        set => SetValue(OkTextProperty, value);
    }

    /// <summary>
    /// Gets or sets the Cancel button text.
    /// </summary>
    public string? CancelText
    {
        get => GetValue(CancelTextProperty);
        set => SetValue(CancelTextProperty, value);
    }

    /// <summary>
    /// Gets or sets the OK button theme class.
    /// </summary>
    public string? OkButtonTheme
    {
        get => GetValue(OkButtonThemeProperty);
        set => SetValue(OkButtonThemeProperty, value);
    }

    /// <summary>
    /// Gets or sets the Cancel button theme class.
    /// </summary>
    public string? CancelButtonTheme
    {
        get => GetValue(CancelButtonThemeProperty);
        set => SetValue(CancelButtonThemeProperty, value);
    }

    /// <summary>
    /// Gets or sets the popover placement.
    /// </summary>
    public PlacementMode Placement
    {
        get => GetValue(PlacementProperty);
        set => SetValue(PlacementProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the popconfirm is open.
    /// </summary>
    public bool IsOpen
    {
        get => GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    /// <summary>
    /// Gets or sets whether to show the arrow.
    /// </summary>
    public bool ShowArrow
    {
        get => GetValue(ShowArrowProperty);
        set => SetValue(ShowArrowProperty, value);
    }

    /// <summary>
    /// Occurs when the user confirms.
    /// </summary>
    public event EventHandler<RoutedEventArgs>? Confirm;

    /// <summary>
    /// Occurs when the user cancels.
    /// </summary>
    public event EventHandler<RoutedEventArgs>? Cancel;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        if (_okButton != null)
        {
            _okButton.Click -= OnOkClick;
        }
        if (_cancelButton != null)
        {
            _cancelButton.Click -= OnCancelClick;
        }

        _okButton = e.NameScope.Find<Button>("PART_OKButton");
        _cancelButton = e.NameScope.Find<Button>("PART_CancelButton");

        if (_okButton != null)
        {
            _okButton.Click += OnOkClick;
        }
        if (_cancelButton != null)
        {
            _cancelButton.Click += OnCancelClick;
        }

        UpdatePseudoClasses();
    }

    /// <summary>
    /// Opens the popconfirm.
    /// </summary>
    public void Open()
    {
        IsOpen = true;
    }

    /// <summary>
    /// Closes the popconfirm.
    /// </summary>
    public void Close()
    {
        IsOpen = false;
    }

    private void OnOkClick(object? sender, RoutedEventArgs e)
    {
        Confirm?.Invoke(this, new RoutedEventArgs());
        IsOpen = false;
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e)
    {
        Cancel?.Invoke(this, new RoutedEventArgs());
        IsOpen = false;
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":open", IsOpen);
        PseudoClasses.Set(":closed", !IsOpen);
    }
}
