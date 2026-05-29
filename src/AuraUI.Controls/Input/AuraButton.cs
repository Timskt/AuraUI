using System.Windows.Input;
using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
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
[TemplatePart("PART_Border", typeof(Avalonia.Controls.Border))]
[TemplatePart("PART_IconPresenter", typeof(Avalonia.Controls.Presenters.ContentPresenter))]
[TemplatePart("PART_ContentPresenter", typeof(Avalonia.Controls.Presenters.ContentPresenter))]
[TemplatePart("PART_LoadingPresenter", typeof(Avalonia.Controls.Presenters.ContentPresenter))]
public class AuraButton : Button
{
    private ContentPresenter? _iconPresenter;
    private ContentPresenter? _loadingPresenter;

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
    /// Defines the <see cref="HoverBackground"/> property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> HoverBackgroundProperty =
        AvaloniaProperty.Register<AuraButton, IBrush?>(nameof(HoverBackground));

    /// <summary>
    /// Defines the <see cref="HoverForeground"/> property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> HoverForegroundProperty =
        AvaloniaProperty.Register<AuraButton, IBrush?>(nameof(HoverForeground));

    /// <summary>
    /// Defines the <see cref="HoverBorderBrush"/> property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> HoverBorderBrushProperty =
        AvaloniaProperty.Register<AuraButton, IBrush?>(nameof(HoverBorderBrush));

    /// <summary>
    /// Defines the <see cref="PressedBackground"/> property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> PressedBackgroundProperty =
        AvaloniaProperty.Register<AuraButton, IBrush?>(nameof(PressedBackground));

    /// <summary>
    /// Defines the <see cref="PressedForeground"/> property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> PressedForegroundProperty =
        AvaloniaProperty.Register<AuraButton, IBrush?>(nameof(PressedForeground));

    /// <summary>
    /// Defines the <see cref="PressedBorderBrush"/> property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> PressedBorderBrushProperty =
        AvaloniaProperty.Register<AuraButton, IBrush?>(nameof(PressedBorderBrush));

    /// <summary>
    /// Defines the <see cref="FocusBorderBrush"/> property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> FocusBorderBrushProperty =
        AvaloniaProperty.Register<AuraButton, IBrush?>(nameof(FocusBorderBrush));

    /// <summary>
    /// Defines the <see cref="FocusBorderThickness"/> property.
    /// </summary>
    public static readonly StyledProperty<Thickness> FocusBorderThicknessProperty =
        AvaloniaProperty.Register<AuraButton, Thickness>(nameof(FocusBorderThickness), new Thickness(2));

    /// <summary>
    /// Defines the <see cref="DisabledBackground"/> property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> DisabledBackgroundProperty =
        AvaloniaProperty.Register<AuraButton, IBrush?>(nameof(DisabledBackground));

    /// <summary>
    /// Defines the <see cref="DisabledForeground"/> property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> DisabledForegroundProperty =
        AvaloniaProperty.Register<AuraButton, IBrush?>(nameof(DisabledForeground));

    /// <summary>
    /// Defines the <see cref="DisabledOpacity"/> property.
    /// </summary>
    public static readonly StyledProperty<double> DisabledOpacityProperty =
        AvaloniaProperty.Register<AuraButton, double>(nameof(DisabledOpacity), 0.4);

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

    /// <summary>
    /// Gets or sets the background brush when the pointer hovers over the button.
    /// </summary>
    public IBrush? HoverBackground
    {
        get => GetValue(HoverBackgroundProperty);
        set => SetValue(HoverBackgroundProperty, value);
    }

    /// <summary>
    /// Gets or sets the foreground brush when the pointer hovers over the button.
    /// </summary>
    public IBrush? HoverForeground
    {
        get => GetValue(HoverForegroundProperty);
        set => SetValue(HoverForegroundProperty, value);
    }

    /// <summary>
    /// Gets or sets the border brush when the pointer hovers over the button.
    /// </summary>
    public IBrush? HoverBorderBrush
    {
        get => GetValue(HoverBorderBrushProperty);
        set => SetValue(HoverBorderBrushProperty, value);
    }

    /// <summary>
    /// Gets or sets the background brush when the button is pressed.
    /// </summary>
    public IBrush? PressedBackground
    {
        get => GetValue(PressedBackgroundProperty);
        set => SetValue(PressedBackgroundProperty, value);
    }

    /// <summary>
    /// Gets or sets the foreground brush when the button is pressed.
    /// </summary>
    public IBrush? PressedForeground
    {
        get => GetValue(PressedForegroundProperty);
        set => SetValue(PressedForegroundProperty, value);
    }

    /// <summary>
    /// Gets or sets the border brush when the button is pressed.
    /// </summary>
    public IBrush? PressedBorderBrush
    {
        get => GetValue(PressedBorderBrushProperty);
        set => SetValue(PressedBorderBrushProperty, value);
    }

    /// <summary>
    /// Gets or sets the border brush when the button has focus.
    /// </summary>
    public IBrush? FocusBorderBrush
    {
        get => GetValue(FocusBorderBrushProperty);
        set => SetValue(FocusBorderBrushProperty, value);
    }

    /// <summary>
    /// Gets or sets the border thickness when the button has focus.
    /// </summary>
    public Thickness FocusBorderThickness
    {
        get => GetValue(FocusBorderThicknessProperty);
        set => SetValue(FocusBorderThicknessProperty, value);
    }

    /// <summary>
    /// Gets or sets the background brush when the button is disabled.
    /// </summary>
    public IBrush? DisabledBackground
    {
        get => GetValue(DisabledBackgroundProperty);
        set => SetValue(DisabledBackgroundProperty, value);
    }

    /// <summary>
    /// Gets or sets the foreground brush when the button is disabled.
    /// </summary>
    public IBrush? DisabledForeground
    {
        get => GetValue(DisabledForegroundProperty);
        set => SetValue(DisabledForegroundProperty, value);
    }

    /// <summary>
    /// Gets or sets the opacity when the button is disabled.
    /// </summary>
    public double DisabledOpacity
    {
        get => GetValue(DisabledOpacityProperty);
        set => SetValue(DisabledOpacityProperty, value);
    }

    static AuraButton()
    {
        AffectsMeasure<AuraButton>(IconProperty, IconPositionProperty, IsLoadingProperty, LoadingContentProperty);
        AffectsRender<AuraButton>(IconProperty, IconPositionProperty);

        IconProperty.Changed.AddClassHandler<AuraButton>((x, e) => x.OnIconChanged(e));
        IconPositionProperty.Changed.AddClassHandler<AuraButton>((x, _) => x.UpdateIconPosition());
        IsLoadingProperty.Changed.AddClassHandler<AuraButton>((x, e) => x.OnIsLoadingChanged(e));
        LoadingContentProperty.Changed.AddClassHandler<AuraButton>((x, _) => x.UpdateLoadingState());
        ContentProperty.Changed.AddClassHandler<AuraButton>((x, _) => x.UpdateAutomationName());
    }

    protected override Type StyleKeyOverride => typeof(Button);

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _iconPresenter = e.NameScope.Find<ContentPresenter>("PART_IconPresenter");
        _loadingPresenter = e.NameScope.Find<ContentPresenter>("PART_LoadingPresenter");

        UpdateIconPosition();
        UpdateLoadingState();
        UpdateAutomationName();
    }

    private void UpdateAutomationName()
    {
        if (Content is string text && !string.IsNullOrEmpty(text))
        {
            SetValue(AutomationProperties.NameProperty, text);
        }
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
    }
}
