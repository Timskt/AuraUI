using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using AuraUI.Controls.Layout;

namespace AuraUI.Controls.Selection;

/// <summary>
/// A native-feeling select dropdown that extends ComboBox with additional
/// properties for validation, size, and intent styling.
///
/// Supports size classes: .sm, .md, .lg
/// Supports intent classes: .default, .error, .warning
/// </summary>
[PseudoClasses(":focus-visible")]
public class NativeSelect : ComboBox
{
    /// <summary>
    /// Defines the <see cref="Size"/> styled property.
    /// </summary>
    public static readonly StyledProperty<ControlSize> SizeProperty =
        AvaloniaProperty.Register<NativeSelect, ControlSize>(nameof(Size), ControlSize.Medium);

    /// <summary>
    /// Defines the <see cref="IsInvalid"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsInvalidProperty =
        AvaloniaProperty.Register<NativeSelect, bool>(nameof(IsInvalid));

    // PlaceholderText is inherited from ComboBox; no need to redeclare.

    static NativeSelect()
    {
        SizeProperty.Changed.AddClassHandler<NativeSelect>((x, _) => x.SyncClasses());
        IsInvalidProperty.Changed.AddClassHandler<NativeSelect>((x, _) => x.SyncClasses());
        SelectedItemProperty.Changed.AddClassHandler<NativeSelect>((x, _) => x.SyncSelectionClasses());
        SelectedIndexProperty.Changed.AddClassHandler<NativeSelect>((x, _) => x.SyncSelectionClasses());
        IsDropDownOpenProperty.Changed.AddClassHandler<NativeSelect>((x, _) => x.SyncPopupClasses());
    }

    public NativeSelect()
    {
        SyncClasses();
    }

    /// <summary>
    /// Gets or sets the size of the select control.
    /// </summary>
    public ControlSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the control is in an invalid state.
    /// </summary>
    public bool IsInvalid
    {
        get => GetValue(IsInvalidProperty);
        set => SetValue(IsInvalidProperty, value);
    }

    protected override void OnGotFocus(GotFocusEventArgs e)
    {
        base.OnGotFocus(e);
        PseudoClasses.Set(":focus-visible", e.NavigationMethod == NavigationMethod.Directional);
    }

    protected override void OnLostFocus(RoutedEventArgs e)
    {
        base.OnLostFocus(e);
        PseudoClasses.Set(":focus-visible", false);
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        PseudoClasses.Set(":focus-visible", false);
        base.OnPointerPressed(e);
    }

    private void SyncClasses()
    {
        Classes.Set("small", Size == ControlSize.Small);
        Classes.Set("medium", Size == ControlSize.Medium);
        Classes.Set("large", Size == ControlSize.Large);
        Classes.Set("native-select", true);
        Classes.Set("invalid", IsInvalid);
        SyncSelectionClasses();
    }

    private void SyncSelectionClasses()
    {
        var hasSelection = SelectedItem is not null && SelectedIndex >= 0;
        Classes.Set("has-selection", hasSelection);
        Classes.Set("placeholder-visible", !hasSelection);
    }

    private void SyncPopupClasses()
    {
        Classes.Set("popup-open", false);

        if (!IsDropDownOpen) return;

        Dispatcher.UIThread.Post(() =>
        {
            if (IsDropDownOpen)
            {
                Classes.Set("popup-open", true);
            }
        });
    }
}

/// <summary>
/// An option item for use within a <see cref="NativeSelect"/>.
/// </summary>
public class NativeSelectOption : ComboBoxItem
{
    /// <summary>
    /// Defines the <see cref="Value"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> ValueProperty =
        AvaloniaProperty.Register<NativeSelectOption, string?>(nameof(Value));

    static NativeSelectOption()
    {
        ValueProperty.Changed.AddClassHandler<NativeSelectOption>((x, _) => x.SyncClasses());
        IsEnabledProperty.Changed.AddClassHandler<NativeSelectOption>((x, _) => x.SyncClasses());
    }

    public NativeSelectOption()
    {
        SyncClasses();
    }

    /// <summary>
    /// Gets or sets the value associated with this option.
    /// </summary>
    public string? Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == ContentProperty)
        {
            SyncClasses();
        }
    }

    private void SyncClasses()
    {
        Classes.Set("native-select-option", true);
        Classes.Set("has-value", !string.IsNullOrEmpty(Value));
        Classes.Set("option-disabled", !IsEnabled);
    }
}
