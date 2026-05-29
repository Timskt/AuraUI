using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Media;

namespace AuraUI.Core.Configuration;

/// <summary>
/// Global configuration settings for the AuraUI framework.
/// Access the shared instance via <see cref="Default"/> and override
/// individual properties before creating UI elements.
/// </summary>
public class AuraUIGlobalSettings
{
    /// <summary>
    /// Gets the shared default settings instance.
    /// </summary>
    public static AuraUIGlobalSettings Default { get; } = new();

    /// <summary>
    /// Gets or sets the opacity applied to disabled controls.
    /// </summary>
    public double DisabledOpacity { get; set; } = 0.4;

    /// <summary>
    /// Gets or sets the default font family for AuraUI controls.
    /// When <c>null</c>, the theme default is used.
    /// </summary>
    public FontFamily? FontFamily { get; set; }

    /// <summary>
    /// Gets or sets the default font size for AuraUI controls.
    /// When <c>null</c>, the theme default is used.
    /// </summary>
    public double? FontSize { get; set; }

    /// <summary>
    /// Gets or sets the default duration for control animations.
    /// </summary>
    public TimeSpan AnimationDuration { get; set; } = TimeSpan.FromMilliseconds(200);

    /// <summary>
    /// Gets or sets the default easing function for control animations.
    /// When <c>null</c>, a linear easing is used.
    /// </summary>
    public Easing? AnimationEasing { get; set; }

    /// <summary>
    /// Gets or sets the default corner radius applied to controls such as buttons and text boxes.
    /// </summary>
    public CornerRadius DefaultCornerRadius { get; set; } = new(4);

    /// <summary>
    /// Gets or sets the default height for standard-sized controls.
    /// </summary>
    public double DefaultControlHeight { get; set; } = 32;
}
