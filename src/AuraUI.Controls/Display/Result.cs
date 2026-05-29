using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;

namespace AuraUI.Controls.Display;

/// <summary>
/// The type of result to display.
/// </summary>
public enum ResultStatus
{
    /// <summary>Success result.</summary>
    Success,
    /// <summary>Error result.</summary>
    Error,
    /// <summary>Information result.</summary>
    Info,
    /// <summary>Warning result.</summary>
    Warning,
    /// <summary>403 Forbidden result.</summary>
    Forbidden,
    /// <summary>404 Not Found result.</summary>
    NotFound,
    /// <summary>500 Server Error result.</summary>
    ServerError
}

/// <summary>
/// Displays the result of an operation with a status icon, title, subtitle,
/// and optional extra content/buttons, inspired by Ant Design's Result component.
/// </summary>
[TemplatePart("PART_ExtraPresenter", typeof(ContentPresenter))]
[PseudoClasses(":success", ":error", ":info", ":warning", ":forbidden", ":notfound", ":servererror")]
public class Result : ContentControl
{
    /// <summary>
    /// Defines the <see cref="Status"/> styled property.
    /// </summary>
    public static readonly StyledProperty<ResultStatus> StatusProperty =
        AvaloniaProperty.Register<Result, ResultStatus>(nameof(Status), ResultStatus.Info);

    /// <summary>
    /// Defines the <see cref="Title"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<Result, string?>(nameof(Title));

    /// <summary>
    /// Defines the <see cref="SubTitle"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> SubTitleProperty =
        AvaloniaProperty.Register<Result, string?>(nameof(SubTitle));

    /// <summary>
    /// Defines the <see cref="Extra"/> styled property.
    /// Extra content (typically action buttons) displayed below the subtitle.
    /// </summary>
    public static readonly StyledProperty<object?> ExtraProperty =
        AvaloniaProperty.Register<Result, object?>(nameof(Extra));

    /// <summary>
    /// Defines the <see cref="Icon"/> styled property.
    /// Custom icon content, overriding the default status icon.
    /// </summary>
    public static readonly StyledProperty<object?> IconProperty =
        AvaloniaProperty.Register<Result, object?>(nameof(Icon));

    static Result()
    {
        StatusProperty.Changed.AddClassHandler<Result>((x, _) => x.UpdatePseudoClasses());
    }

    /// <summary>
    /// Gets or sets the result status type.
    /// </summary>
    public ResultStatus Status
    {
        get => GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
    }

    /// <summary>
    /// Gets or sets the result title.
    /// </summary>
    public string? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <summary>
    /// Gets or sets the result subtitle (descriptive text).
    /// </summary>
    public string? SubTitle
    {
        get => GetValue(SubTitleProperty);
        set => SetValue(SubTitleProperty, value);
    }

    /// <summary>
    /// Gets or sets the extra content (action buttons).
    /// </summary>
    public object? Extra
    {
        get => GetValue(ExtraProperty);
        set => SetValue(ExtraProperty, value);
    }

    /// <summary>
    /// Gets or sets a custom icon that overrides the default status icon.
    /// </summary>
    public object? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        UpdatePseudoClasses();
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":success", Status == ResultStatus.Success);
        PseudoClasses.Set(":error", Status == ResultStatus.Error);
        PseudoClasses.Set(":info", Status == ResultStatus.Info);
        PseudoClasses.Set(":warning", Status == ResultStatus.Warning);
        PseudoClasses.Set(":forbidden", Status == ResultStatus.Forbidden);
        PseudoClasses.Set(":notfound", Status == ResultStatus.NotFound);
        PseudoClasses.Set(":servererror", Status == ResultStatus.ServerError);
    }
}
