using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using AuraUI.Controls.Layout;

namespace AuraUI.Controls.Input;

/// <summary>
/// A multi-line text input control extending TextBox with auto-resize capability,
/// row limits, and character count display.
///
/// Supports size classes: .sm, .md, .lg
/// Supports variant classes: .filled, .outlined, .underlined
/// </summary>
public class Textarea : TextBox
{
    /// <summary>
    /// Defines the <see cref="AutoResize"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> AutoResizeProperty =
        AvaloniaProperty.Register<Textarea, bool>(nameof(AutoResize), true);

    /// <summary>
    /// Defines the <see cref="MinRows"/> styled property.
    /// </summary>
    public static readonly StyledProperty<int> MinRowsProperty =
        AvaloniaProperty.Register<Textarea, int>(nameof(MinRows), 3);

    /// <summary>
    /// Defines the <see cref="MaxRows"/> styled property.
    /// </summary>
    public static readonly StyledProperty<int> MaxRowsProperty =
        AvaloniaProperty.Register<Textarea, int>(nameof(MaxRows), 10);

    /// <summary>
    /// Defines the <see cref="ShowCount"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowCountProperty =
        AvaloniaProperty.Register<Textarea, bool>(nameof(ShowCount));

    /// <summary>
    /// Defines the <see cref="CharacterCount"/> styled property.
    /// </summary>
    public static readonly StyledProperty<int> CharacterCountProperty =
        AvaloniaProperty.Register<Textarea, int>(nameof(CharacterCount));

    static Textarea()
    {
        AutoResizeProperty.Changed.AddClassHandler<Textarea>((x, _) => x.SyncTextareaClasses());
        MinRowsProperty.Changed.AddClassHandler<Textarea>((x, _) =>
        {
            x.SyncTextareaClasses();
            x.UpdateMinHeight();
        });
        MaxRowsProperty.Changed.AddClassHandler<Textarea>((x, _) => x.UpdateMaxHeight());
        TextProperty.Changed.AddClassHandler<Textarea>((x, _) =>
        {
            x.UpdateCharacterCount();
            if (x.AutoResize) x.UpdateHeight();
        });
    }

    public Textarea()
    {
        AcceptsReturn = true;
        TextWrapping = TextWrapping.Wrap;
        ScrollViewer.SetHorizontalScrollBarVisibility(this, ScrollBarVisibility.Disabled);
        ScrollViewer.SetVerticalScrollBarVisibility(this, ScrollBarVisibility.Auto);
        SyncTextareaClasses();
    }

    /// <summary>
    /// Gets or sets whether the textarea automatically resizes based on content.
    /// </summary>
    public bool AutoResize
    {
        get => GetValue(AutoResizeProperty);
        set => SetValue(AutoResizeProperty, value);
    }

    /// <summary>
    /// Gets or sets the minimum number of visible rows.
    /// </summary>
    public int MinRows
    {
        get => GetValue(MinRowsProperty);
        set => SetValue(MinRowsProperty, value);
    }

    /// <summary>
    /// Gets or sets the maximum number of visible rows before scrolling.
    /// </summary>
    public int MaxRows
    {
        get => GetValue(MaxRowsProperty);
        set => SetValue(MaxRowsProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the character count is displayed.
    /// </summary>
    public bool ShowCount
    {
        get => GetValue(ShowCountProperty);
        set => SetValue(ShowCountProperty, value);
    }

    /// <summary>
    /// Gets the current character count.
    /// </summary>
    public int CharacterCount => GetValue(CharacterCountProperty);

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        UpdateMinHeight();
        UpdateMaxHeight();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == MaxLengthProperty)
        {
            UpdateCharacterCount();
        }
    }

    private void SyncTextareaClasses()
    {
        Classes.Set("textarea", true);
        Classes.Set("auto-resize", AutoResize);
        Classes.Set("textarea-tall", MinRows >= 5);
    }

    private void UpdateCharacterCount()
    {
        SetValue(CharacterCountProperty, Text?.Length ?? 0);
    }

    private void UpdateMinHeight()
    {
        // Approximate: each row is ~20px tall
        var lineHeight = FontSize * 1.4;
        MinHeight = MinRows * lineHeight + Padding.Top + Padding.Bottom;
    }

    private void UpdateMaxHeight()
    {
        if (MaxRows <= 0) return;
        var lineHeight = FontSize * 1.4;
        MaxHeight = MaxRows * lineHeight + Padding.Top + Padding.Bottom;
    }

    private void UpdateHeight()
    {
        if (!AutoResize) return;

        // Let the layout system handle this via MinHeight/MaxHeight constraints
        InvalidateMeasure();
    }
}
