using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using AuraUI.Controls.Layout;

namespace AuraUI.Controls.Display;

/// <summary>
/// A form label control with required indicator, click-to-focus support,
/// and accessibility integration.
///
/// Supports size classes: .sm, .md, .lg
/// </summary>
public class AuraLabel : Label
{
    /// <summary>
    /// Defines the <see cref="IsRequired"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsRequiredProperty =
        AvaloniaProperty.Register<AuraLabel, bool>(nameof(IsRequired));

    /// <summary>
    /// Defines the <see cref="Description"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> DescriptionProperty =
        AvaloniaProperty.Register<AuraLabel, string?>(nameof(Description));

    /// <summary>
    /// Defines the <see cref="Size"/> styled property.
    /// </summary>
    public static readonly StyledProperty<ControlSize> SizeProperty =
        AvaloniaProperty.Register<AuraLabel, ControlSize>(nameof(Size), ControlSize.Medium);

    static AuraLabel()
    {
        IsRequiredProperty.Changed.AddClassHandler<AuraLabel>((x, _) => x.SyncClasses());
        DescriptionProperty.Changed.AddClassHandler<AuraLabel>((x, _) => x.SyncClasses());
        SizeProperty.Changed.AddClassHandler<AuraLabel>((x, _) => x.SyncClasses());
    }

    public AuraLabel()
    {
        SyncClasses();
    }

    /// <summary>
    /// Gets or sets whether the field is required (shows a required indicator).
    /// </summary>
    public bool IsRequired
    {
        get => GetValue(IsRequiredProperty);
        set => SetValue(IsRequiredProperty, value);
    }

    /// <summary>
    /// Gets or sets a description text shown below the label.
    /// </summary>
    public string? Description
    {
        get => GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    /// <summary>
    /// Gets or sets the label size.
    /// </summary>
    public ControlSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == TargetProperty || change.Property == IsEnabledProperty)
        {
            SyncClasses();
        }
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        SyncClasses();
        base.OnPointerPressed(e);
    }

    private void SyncClasses()
    {
        Classes.Set("aura-label", true);
        Classes.Set("small", Size == ControlSize.Small);
        Classes.Set("medium", Size == ControlSize.Medium);
        Classes.Set("large", Size == ControlSize.Large);
        Classes.Set("required", IsRequired);
        Classes.Set("has-target", Target is not null);
        Classes.Set("has-description", !string.IsNullOrEmpty(Description));
        Classes.Set("target-disabled", Target is InputElement { IsEnabled: false });
    }
}
