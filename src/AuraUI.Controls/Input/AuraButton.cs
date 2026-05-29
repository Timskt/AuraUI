using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Threading;

namespace AuraUI.Controls.Input;

/// <summary>
/// Defines the position of an icon relative to the button content.
/// </summary>
public enum IconPosition
{
    Left,
    Right,
    Top,
    Bottom
}

/// <summary>
/// Enhanced button control with icon support, loading state, and AuraUI design token integration.
/// Supports variant classes: .primary, .secondary, .destructive, .outline, .ghost, .link
/// Supports size classes: .sm, .md, .lg
/// </summary>
public class AuraButton : Button
{
    private ContentPresenter? _iconPresenter;
    private ContentPresenter? _loadingPresenter;
    private IDisposable? _loadingCancellation;

    /// <summary>
    /// Defines the <see cref="Icon"/> property.
    /// </summary>
    public static readonly StyledProperty<object?> IconProperty =
        AvaloniaProperty.Register<AuraButton, object?>(
            nameof(Icon));

    /// <summary>
    /// Defines the <see cref="IconPosition"/> property.
    /// </summary>
    public static readonly StyledProperty<IconPosition> IconPositionProperty =
        AvaloniaProperty.Register<AuraButton, IconPosition>(
            nameof(IconPosition),
            defaultValue: IconPosition.Left);

    /// <summary>
    /// Defines the <see cref="IsLoading"/> property.
    /// </summary>
    public static readonly StyledProperty<bool> IsLoadingProperty =
        AvaloniaProperty.Register<AuraButton, bool>(
            nameof(IsLoading));

    /// <summary>
    /// Defines the <see cref="LoadingContent"/> property.
    /// </summary>
    public static readonly StyledProperty<object?> LoadingContentProperty =
        AvaloniaProperty.Register<AuraButton, object?>(
            nameof(LoadingContent));

    /// <summary>
    /// Defines the <see cref="IconSpacing"/> property.
    /// </summary>
    public static readonly StyledProperty<double> IconSpacingProperty =
        AvaloniaProperty.Register<AuraButton, double>(
            nameof(IconSpacing),
            defaultValue: 8.0);

    /// <summary>
    /// Gets or sets the icon content displayed alongside the button content.
    /// </summary>
    public object? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    /// <summary>
    /// Gets or sets the position of the icon relative to the button content.
    /// </summary>
    public IconPosition IconPosition
    {
        get => GetValue(IconPositionProperty);
        set => SetValue(IconPositionProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the button is in a loading state.
    /// When true, the button displays a loading indicator and becomes non-interactive.
    /// </summary>
    public bool IsLoading
    {
        get => GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }

    /// <summary>
    /// Gets or sets the content displayed during loading state.
    /// If null, the original content is retained.
    /// </summary>
    public object? LoadingContent
    {
        get => GetValue(LoadingContentProperty);
        set => SetValue(LoadingContentProperty, value);
    }

    /// <summary>
    /// Gets or sets the spacing between the icon and the content.
    /// </summary>
    public double IconSpacing
    {
        get => GetValue(IconSpacingProperty);
        set => SetValue(IconSpacingProperty, value);
    }

    static AuraButton()
    {
        AffectsMeasure<AuraButton>(IconProperty, IconPositionProperty, IsLoadingProperty, LoadingContentProperty);
        AffectsRender<AuraButton>(IconProperty, IconPositionProperty);

        IconProperty.Changed.AddClassHandler<AuraButton>((x, e) => x.OnIconChanged(e));
        IconPositionProperty.Changed.AddClassHandler<AuraButton>((x, _) => x.UpdateIconPosition());
        IsLoadingProperty.Changed.AddClassHandler<AuraButton>((x, e) => x.OnIsLoadingChanged(e));
        LoadingContentProperty.Changed.AddClassHandler<AuraButton>((x, _) => x.UpdateLoadingState());
    }

    protected override Type StyleKeyOverride => typeof(Button);

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _iconPresenter = e.NameScope.Find<ContentPresenter>("PART_IconPresenter");
        _loadingPresenter = e.NameScope.Find<ContentPresenter>("PART_LoadingPresenter");

        UpdateIconPosition();
        UpdateLoadingState();
    }

    protected virtual void OnIconChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if (_iconPresenter is not null)
        {
            _iconPresenter.Content = e.NewValue;
        }
    }

    private void OnIsLoadingChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if (e.NewValue is bool isLoading)
        {
            PseudoClasses.Set("loading", isLoading);
            IsEnabled = !isLoading;
            UpdateLoadingState();
        }
    }

    protected virtual void UpdateIconPosition()
    {
        if (_iconPresenter is null)
            return;

        var pos = IconPosition;
        _iconPresenter.SetValue(IsVisibleProperty, Icon is not null);

        // Update layout direction via logical classes; the template should use
        // ItemsControl or a Panel that responds to the IconPosition value.
        // Here we update margin to provide spacing between icon and content.
        var spacing = IconSpacing;
        _iconPresenter.Margin = pos switch
        {
            IconPosition.Left => new Thickness(0, 0, spacing, 0),
            IconPosition.Right => new Thickness(spacing, 0, 0, 0),
            IconPosition.Top => new Thickness(0, 0, 0, spacing),
            IconPosition.Bottom => new Thickness(0, spacing, 0, 0),
            _ => new Thickness(0, 0, spacing, 0)
        };
    }

    protected virtual void UpdateLoadingState()
    {
        var isLoading = IsLoading;

        if (_loadingPresenter is not null)
        {
            _loadingPresenter.IsVisible = isLoading;
        }

        if (_iconPresenter is not null)
        {
            // Hide icon when loading
            _iconPresenter.IsVisible = !isLoading && Icon is not null;
        }
    }

    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromLogicalTree(e);
        _loadingCancellation?.Dispose();
        _loadingCancellation = null;
    }
}
