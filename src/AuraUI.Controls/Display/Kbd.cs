using Avalonia;
using Avalonia.Controls;
using AuraUI.Controls.Layout;

namespace AuraUI.Controls.Display;

/// <summary>
/// A keyboard shortcut display component that renders styled keyboard key indicators.
///
/// Supports size classes: .sm, .md, .lg
/// </summary>
public class Kbd : ContentControl
{
    /// <summary>
    /// Defines the <see cref="Shortcut"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> ShortcutProperty =
        AvaloniaProperty.Register<Kbd, string?>(nameof(Shortcut));

    /// <summary>
    /// Defines the <see cref="Size"/> styled property.
    /// </summary>
    public static readonly StyledProperty<ControlSize> SizeProperty =
        AvaloniaProperty.Register<Kbd, ControlSize>(nameof(Size), ControlSize.Medium);

    static Kbd()
    {
        SizeProperty.Changed.AddClassHandler<Kbd>((x, _) => x.SyncClasses());
        ShortcutProperty.Changed.AddClassHandler<Kbd>((x, _) => x.SyncClasses());
    }

    public Kbd()
    {
        SyncClasses();
    }

    /// <summary>
    /// Gets or sets the keyboard shortcut text (e.g., "Ctrl+C", "Cmd+K").
    /// </summary>
    public string? Shortcut
    {
        get => GetValue(ShortcutProperty);
        set => SetValue(ShortcutProperty, value);
    }

    /// <summary>
    /// Gets or sets the display size.
    /// </summary>
    public ControlSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    private void SyncClasses()
    {
        Classes.Set("kbd", true);
        Classes.Set("small", Size == ControlSize.Small);
        Classes.Set("medium", Size == ControlSize.Medium);
        Classes.Set("large", Size == ControlSize.Large);
        Classes.Set("has-shortcut", !string.IsNullOrEmpty(Shortcut));
    }
}

/// <summary>
/// A container that groups multiple <see cref="Kbd"/> elements together
/// with a separator between them (e.g., "Ctrl + Shift + K").
/// </summary>
public class KbdGroup : ItemsControl
{
    /// <summary>
    /// Defines the <see cref="Size"/> styled property.
    /// </summary>
    public static readonly StyledProperty<ControlSize> SizeProperty =
        AvaloniaProperty.Register<KbdGroup, ControlSize>(nameof(Size), ControlSize.Medium);

    static KbdGroup()
    {
        SizeProperty.Changed.AddClassHandler<KbdGroup>((x, _) => x.SyncClasses());
    }

    public KbdGroup()
    {
        Focusable = false;
        ItemsView.CollectionChanged += (_, _) => SyncClasses();
        SyncClasses();
    }

    /// <summary>
    /// Gets or sets the display size for all kbd elements in the group.
    /// </summary>
    public ControlSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);
        SyncClasses();
    }

    protected override void ClearContainerForItemOverride(Control element)
    {
        base.ClearContainerForItemOverride(element);
        SyncClasses();
    }

    private void SyncClasses()
    {
        Classes.Set("kbd-group", true);
        Classes.Set("small", Size == ControlSize.Small);
        Classes.Set("medium", Size == ControlSize.Medium);
        Classes.Set("large", Size == ControlSize.Large);
        Classes.Set("has-items", ItemsView.Count > 0);
        Classes.Set("empty", ItemsView.Count == 0);
    }
}
