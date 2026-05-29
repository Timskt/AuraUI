using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace AuraUI.Controls.Navigation;

/// <summary>
/// A mobile-style top app bar with back button, title, and action buttons.
/// Follows Material Design top app bar patterns.
/// </summary>
[PseudoClasses(":elevated", ":flat", ":centered-title")]
public class MobileAppBar : TemplatedControl
{
    /// <summary>
    /// Defines the <see cref="Title"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<MobileAppBar, string?>(nameof(Title));

    /// <summary>
    /// Defines the <see cref="LeftAction"/> styled property.
    /// Content for the left action area (typically a back button).
    /// </summary>
    public static readonly StyledProperty<object?> LeftActionProperty =
        AvaloniaProperty.Register<MobileAppBar, object?>(nameof(LeftAction));

    /// <summary>
    /// Defines the <see cref="RightActions"/> styled property.
    /// Content for the right action area (typically icon buttons).
    /// </summary>
    public static readonly StyledProperty<object?> RightActionsProperty =
        AvaloniaProperty.Register<MobileAppBar, object?>(nameof(RightActions));

    /// <summary>
    /// Defines the <see cref="IsElevated"/> styled property.
    /// Whether the app bar has a shadow/elevation.
    /// </summary>
    public static readonly StyledProperty<bool> IsElevatedProperty =
        AvaloniaProperty.Register<MobileAppBar, bool>(nameof(IsElevated), true);

    /// <summary>
    /// Defines the <see cref="AppBarBackground"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> AppBarBackgroundProperty =
        AvaloniaProperty.Register<MobileAppBar, IBrush?>(nameof(AppBarBackground));

    /// <summary>
    /// Defines the <see cref="AppBarForeground"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> AppBarForegroundProperty =
        AvaloniaProperty.Register<MobileAppBar, IBrush?>(nameof(AppBarForeground));

    /// <summary>
    /// Defines the <see cref="ShowBackButton"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowBackButtonProperty =
        AvaloniaProperty.Register<MobileAppBar, bool>(nameof(ShowBackButton));

    /// <summary>
    /// Defines the <see cref="BarHeight"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> BarHeightProperty =
        AvaloniaProperty.Register<MobileAppBar, double>(nameof(BarHeight), 56);

    /// <summary>
    /// Defines the <see cref="IsCenterTitle"/> styled property.
    /// Whether the title is centered (Material style) or left-aligned.
    /// </summary>
    public static readonly StyledProperty<bool> IsCenterTitleProperty =
        AvaloniaProperty.Register<MobileAppBar, bool>(nameof(IsCenterTitle));

    /// <summary>
    /// Defines the <see cref="Elevation"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> ElevationProperty =
        AvaloniaProperty.Register<MobileAppBar, double>(nameof(Elevation), 4);

    static MobileAppBar()
    {
        IsElevatedProperty.Changed.AddClassHandler<MobileAppBar>((x, _) => x.UpdatePseudoClasses());
        IsCenterTitleProperty.Changed.AddClassHandler<MobileAppBar>((x, _) => x.UpdatePseudoClasses());
    }

    /// <summary>
    /// Gets or sets the title text.
    /// </summary>
    public string? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <summary>
    /// Gets or sets the left action content.
    /// </summary>
    public object? LeftAction
    {
        get => GetValue(LeftActionProperty);
        set => SetValue(LeftActionProperty, value);
    }

    /// <summary>
    /// Gets or sets the right action content.
    /// </summary>
    public object? RightActions
    {
        get => GetValue(RightActionsProperty);
        set => SetValue(RightActionsProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the app bar has elevation shadow.
    /// </summary>
    public bool IsElevated
    {
        get => GetValue(IsElevatedProperty);
        set => SetValue(IsElevatedProperty, value);
    }

    /// <summary>
    /// Gets or sets the background brush.
    /// </summary>
    public IBrush? AppBarBackground
    {
        get => GetValue(AppBarBackgroundProperty);
        set => SetValue(AppBarBackgroundProperty, value);
    }

    /// <summary>
    /// Gets or sets the foreground brush for title and icons.
    /// </summary>
    public IBrush? AppBarForeground
    {
        get => GetValue(AppBarForegroundProperty);
        set => SetValue(AppBarForegroundProperty, value);
    }

    /// <summary>
    /// Gets or sets whether to show a back button in the left action area.
    /// </summary>
    public bool ShowBackButton
    {
        get => GetValue(ShowBackButtonProperty);
        set => SetValue(ShowBackButtonProperty, value);
    }

    /// <summary>
    /// Gets or sets the height of the app bar.
    /// </summary>
    public double BarHeight
    {
        get => GetValue(BarHeightProperty);
        set => SetValue(BarHeightProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the title is centered.
    /// </summary>
    public bool IsCenterTitle
    {
        get => GetValue(IsCenterTitleProperty);
        set => SetValue(IsCenterTitleProperty, value);
    }

    /// <summary>
    /// Gets or sets the elevation in device-independent pixels.
    /// </summary>
    public double Elevation
    {
        get => GetValue(ElevationProperty);
        set => SetValue(ElevationProperty, value);
    }

    /// <summary>
    /// Occurs when the back button is clicked.
    /// </summary>
    public event EventHandler<RoutedEventArgs>? BackButtonClick;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        UpdatePseudoClasses();
    }

    /// <summary>
    /// Invokes the back button click event. Called from template.
    /// </summary>
    public void OnBackButtonClicked()
    {
        BackButtonClick?.Invoke(this, new RoutedEventArgs());
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":elevated", IsElevated);
        PseudoClasses.Set(":flat", !IsElevated);
        PseudoClasses.Set(":centered-title", IsCenterTitle);
    }
}
