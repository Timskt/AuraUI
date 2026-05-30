using Avalonia;
using Avalonia.Controls;

namespace AuraUI.Controls.Feedback;

/// <summary>
/// Alert variant that determines the color scheme and icon.
/// </summary>
public enum AlertVariant
{
    Info,
    Success,
    Warning,
    Error
}

/// <summary>
/// An alert component for displaying important messages with variant styling,
/// icon, title, description, and optional close button.
///
/// Supports variant classes: .info, .success, .warning, .error
/// Supports size classes: .sm, .md, .lg
/// </summary>
public class Alert : ContentControl
{
    /// <summary>
    /// Defines the <see cref="Title"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<Alert, string?>(nameof(Title));

    /// <summary>
    /// Defines the <see cref="Description"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> DescriptionProperty =
        AvaloniaProperty.Register<Alert, string?>(nameof(Description));

    /// <summary>
    /// Defines the <see cref="Icon"/> styled property.
    /// </summary>
    public static readonly StyledProperty<object?> IconProperty =
        AvaloniaProperty.Register<Alert, object?>(nameof(Icon));

    /// <summary>
    /// Defines the <see cref="Variant"/> styled property.
    /// </summary>
    public static readonly StyledProperty<AlertVariant> VariantProperty =
        AvaloniaProperty.Register<Alert, AlertVariant>(nameof(Variant), AlertVariant.Info);

    /// <summary>
    /// Defines the <see cref="IsClosable"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsClosableProperty =
        AvaloniaProperty.Register<Alert, bool>(nameof(IsClosable));

    /// <summary>
    /// Defines the <see cref="ShowIcon"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowIconProperty =
        AvaloniaProperty.Register<Alert, bool>(nameof(ShowIcon), true);

    /// <summary>
    /// Defines the <see cref="IsOpen"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsOpenProperty =
        AvaloniaProperty.Register<Alert, bool>(nameof(IsOpen), true);

    /// <summary>
    /// Defines the <see cref="HasTitle"/> readonly property.
    /// </summary>
    public static readonly StyledProperty<bool> HasTitleProperty =
        AvaloniaProperty.Register<Alert, bool>(nameof(HasTitle));

    /// <summary>
    /// Defines the <see cref="HasDescription"/> readonly property.
    /// </summary>
    public static readonly StyledProperty<bool> HasDescriptionProperty =
        AvaloniaProperty.Register<Alert, bool>(nameof(HasDescription));

    /// <summary>
    /// Defines the <see cref="HasIcon"/> readonly property.
    /// </summary>
    public static readonly StyledProperty<bool> HasIconProperty =
        AvaloniaProperty.Register<Alert, bool>(nameof(HasIcon));

    static Alert()
    {
        VariantProperty.Changed.AddClassHandler<Alert>((x, _) => x.SyncClasses());
        TitleProperty.Changed.AddClassHandler<Alert>((x, _) => x.SyncSlotStates());
        DescriptionProperty.Changed.AddClassHandler<Alert>((x, _) => x.SyncSlotStates());
        IconProperty.Changed.AddClassHandler<Alert>((x, _) => x.SyncSlotStates());
        IsOpenProperty.Changed.AddClassHandler<Alert>((x, _) => x.SyncClasses());
        ContentControl.ContentProperty.Changed.AddClassHandler<Alert>((x, _) => x.SyncSlotStates());
    }

    public Alert()
    {
        SyncClasses();
        SyncSlotStates();
    }

    /// <summary>
    /// Gets or sets the alert title text.
    /// </summary>
    public string? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <summary>
    /// Gets or sets the alert description text.
    /// </summary>
    public string? Description
    {
        get => GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    /// <summary>
    /// Gets or sets the icon displayed in the alert.
    /// </summary>
    public object? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    /// <summary>
    /// Gets or sets the alert variant (color scheme).
    /// </summary>
    public AlertVariant Variant
    {
        get => GetValue(VariantProperty);
        set => SetValue(VariantProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the close button is shown.
    /// </summary>
    public bool IsClosable
    {
        get => GetValue(IsClosableProperty);
        set => SetValue(IsClosableProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the variant icon is shown.
    /// </summary>
    public bool ShowIcon
    {
        get => GetValue(ShowIconProperty);
        set => SetValue(ShowIconProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the alert is visible.
    /// </summary>
    public bool IsOpen
    {
        get => GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    /// <summary>
    /// Gets whether a title is present.
    /// </summary>
    public bool HasTitle => GetValue(HasTitleProperty);

    /// <summary>
    /// Gets whether a description is present.
    /// </summary>
    public bool HasDescription => GetValue(HasDescriptionProperty);

    /// <summary>
    /// Gets whether an icon is present.
    /// </summary>
    public bool HasIcon => GetValue(HasIconProperty);

    private void SyncClasses()
    {
        Classes.Set("info", Variant == AlertVariant.Info);
        Classes.Set("success", Variant == AlertVariant.Success);
        Classes.Set("warning", Variant == AlertVariant.Warning);
        Classes.Set("error", Variant == AlertVariant.Error);
        Classes.Set("open", IsOpen);
        Classes.Set("closed", !IsOpen);
    }

    private void SyncSlotStates()
    {
        var hasTitle = HasValue(Title);
        var hasDescription = HasValue(Description);
        var hasIcon = HasValue(Icon);

        SetValue(HasTitleProperty, hasTitle);
        SetValue(HasDescriptionProperty, hasDescription);
        SetValue(HasIconProperty, hasIcon);

        Classes.Set("has-title", hasTitle);
        Classes.Set("has-description", hasDescription);
        Classes.Set("has-icon", hasIcon);
        Classes.Set("has-content", HasValue(Content));
    }

    private static bool HasValue(object? value)
    {
        return value is string text ? !string.IsNullOrWhiteSpace(text) : value is not null;
    }
}
