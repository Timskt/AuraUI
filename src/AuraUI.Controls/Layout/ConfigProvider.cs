using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AuraUI.Controls.Layout;

/// <summary>
/// Provides global configuration for all AuraUI controls within its scope,
/// inspired by Ant Design's ConfigProvider and Element Plus patterns.
/// Set properties on this control to cascade configuration to descendant controls.
/// </summary>
public class ConfigProvider : ContentControl
{
    /// <summary>
    /// Defines the <see cref="GlobalPrefix"/> styled property.
    /// The CSS prefix for class names.
    /// </summary>
    public static readonly StyledProperty<string> GlobalPrefixProperty =
        AvaloniaProperty.Register<ConfigProvider, string>(nameof(GlobalPrefix), "aura");

    /// <summary>
    /// Defines the <see cref="Theme"/> styled property.
    /// </summary>
    public static new readonly StyledProperty<ConfigProviderTheme> ThemeProperty =
        AvaloniaProperty.Register<ConfigProvider, ConfigProviderTheme>(nameof(Theme), ConfigProviderTheme.Auto);

    /// <summary>
    /// Defines the <see cref="Locale"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string> LocaleProperty =
        AvaloniaProperty.Register<ConfigProvider, string>(nameof(Locale), "en-US");

    /// <summary>
    /// Defines the <see cref="DefaultControlSize"/> styled property.
    /// </summary>
    public static readonly StyledProperty<ControlSize> DefaultControlSizeProperty =
        AvaloniaProperty.Register<ConfigProvider, ControlSize>(nameof(DefaultControlSize), ControlSize.Medium);

    /// <summary>
    /// Defines the <see cref="DisabledGlobal"/> styled property.
    /// When true, disables all descendant controls.
    /// </summary>
    public static readonly StyledProperty<bool> DisabledGlobalProperty =
        AvaloniaProperty.Register<ConfigProvider, bool>(nameof(DisabledGlobal));

    /// <summary>
    /// Defines the <see cref="PrimaryColor"/> styled property.
    /// Overrides the primary accent color for all descendants.
    /// </summary>
    public static readonly StyledProperty<IBrush?> PrimaryColorProperty =
        AvaloniaProperty.Register<ConfigProvider, IBrush?>(nameof(PrimaryColor));

    /// <summary>
    /// Defines the <see cref="BorderRadius"/> styled property.
    /// Default corner radius for all descendant controls.
    /// </summary>
    public static readonly StyledProperty<CornerRadius> BorderRadiusProperty =
        AvaloniaProperty.Register<ConfigProvider, CornerRadius>(nameof(BorderRadius), new CornerRadius(4));

    /// <summary>
    /// Defines the <see cref="FontFamily"/> styled property.
    /// Default font family for all descendant controls.
    /// </summary>
    public static new readonly StyledProperty<FontFamily?> FontFamilyProperty =
        AvaloniaProperty.Register<ConfigProvider, FontFamily?>(nameof(FontFamily));

    /// <summary>
    /// Defines the <see cref="AnimationDisabled"/> styled property.
    /// When true, disables animations for all descendants.
    /// </summary>
    public static readonly StyledProperty<bool> AnimationDisabledProperty =
        AvaloniaProperty.Register<ConfigProvider, bool>(nameof(AnimationDisabled));

    /// <summary>
    /// Defines the <see cref="SpaceSize"/> styled property.
    /// Default spacing size for Space components.
    /// </summary>
    public static readonly StyledProperty<double> SpaceSizeProperty =
        AvaloniaProperty.Register<ConfigProvider, double>(nameof(SpaceSize), 8.0);

    /// <summary>
    /// Defines the <see cref="InputSize"/> styled property.
    /// </summary>
    public static readonly StyledProperty<ControlSize> InputSizeProperty =
        AvaloniaProperty.Register<ConfigProvider, ControlSize>(nameof(InputSize), ControlSize.Medium);

    /// <summary>
    /// Gets or sets the global prefix.
    /// </summary>
    public string GlobalPrefix
    {
        get => GetValue(GlobalPrefixProperty);
        set => SetValue(GlobalPrefixProperty, value);
    }

    /// <summary>
    /// Gets or sets the theme mode.
    /// </summary>
    public new ConfigProviderTheme Theme
    {
        get => GetValue(ThemeProperty);
        set => SetValue(ThemeProperty, value);
    }

    /// <summary>
    /// Gets or sets the locale string (e.g. "en-US", "zh-CN").
    /// </summary>
    public string Locale
    {
        get => GetValue(LocaleProperty);
        set => SetValue(LocaleProperty, value);
    }

    /// <summary>
    /// Gets or sets the default control size.
    /// </summary>
    public ControlSize DefaultControlSize
    {
        get => GetValue(DefaultControlSizeProperty);
        set => SetValue(DefaultControlSizeProperty, value);
    }

    /// <summary>
    /// Gets or sets whether all descendant controls are disabled.
    /// </summary>
    public bool DisabledGlobal
    {
        get => GetValue(DisabledGlobalProperty);
        set => SetValue(DisabledGlobalProperty, value);
    }

    /// <summary>
    /// Gets or sets the primary color override.
    /// </summary>
    public IBrush? PrimaryColor
    {
        get => GetValue(PrimaryColorProperty);
        set => SetValue(PrimaryColorProperty, value);
    }

    /// <summary>
    /// Gets or sets the default border radius.
    /// </summary>
    public CornerRadius BorderRadius
    {
        get => GetValue(BorderRadiusProperty);
        set => SetValue(BorderRadiusProperty, value);
    }

    /// <summary>
    /// Gets or sets the default font family.
    /// </summary>
    public new FontFamily? FontFamily
    {
        get => GetValue(FontFamilyProperty);
        set => SetValue(FontFamilyProperty, value);
    }

    /// <summary>
    /// Gets or sets whether animations are disabled.
    /// </summary>
    public bool AnimationDisabled
    {
        get => GetValue(AnimationDisabledProperty);
        set => SetValue(AnimationDisabledProperty, value);
    }

    /// <summary>
    /// Gets or sets the default spacing for Space components.
    /// </summary>
    public double SpaceSize
    {
        get => GetValue(SpaceSizeProperty);
        set => SetValue(SpaceSizeProperty, value);
    }

    /// <summary>
    /// Gets or sets the default input control size.
    /// </summary>
    public ControlSize InputSize
    {
        get => GetValue(InputSizeProperty);
        set => SetValue(InputSizeProperty, value);
    }
}

/// <summary>
/// Theme mode for ConfigProvider.
/// </summary>
public enum ConfigProviderTheme
{
    /// <summary>Follow the system theme.</summary>
    Auto,
    /// <summary>Force light theme.</summary>
    Light,
    /// <summary>Force dark theme.</summary>
    Dark
}

/// <summary>
/// Standard control sizes.
/// </summary>
public enum ControlSize
{
    /// <summary>Small controls.</summary>
    Small,
    /// <summary>Medium (default) controls.</summary>
    Medium,
    /// <summary>Large controls.</summary>
    Large
}
