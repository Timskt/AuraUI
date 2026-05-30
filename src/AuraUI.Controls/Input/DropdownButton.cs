using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace AuraUI.Controls.Input;

/// <summary>
/// Specifies the dropdown alignment relative to the button.
/// </summary>
public enum DropdownButtonAlign
{
    Center,
    Start,
    End
}

/// <summary>
/// A button that opens a dropdown panel with custom content when clicked.
///
/// Template parts: PART_Trigger, PART_Surface
///
/// Supports size classes: .sm, .md, .lg
/// </summary>
public class DropdownButton : ContentControl
{
    private Button? _trigger;

    /// <summary>
    /// Defines the <see cref="DropDownContent"/> styled property.
    /// </summary>
    public static readonly StyledProperty<object?> DropDownContentProperty =
        AvaloniaProperty.Register<DropdownButton, object?>(nameof(DropDownContent));

    /// <summary>
    /// Defines the <see cref="DropDownContentTemplate"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IDataTemplate?> DropDownContentTemplateProperty =
        AvaloniaProperty.Register<DropdownButton, IDataTemplate?>(nameof(DropDownContentTemplate));

    /// <summary>
    /// Defines the <see cref="Placement"/> styled property.
    /// </summary>
    public static readonly StyledProperty<PlacementMode> PlacementProperty =
        AvaloniaProperty.Register<DropdownButton, PlacementMode>(nameof(Placement), PlacementMode.Bottom);

    /// <summary>
    /// Defines the <see cref="Align"/> styled property.
    /// </summary>
    public static readonly StyledProperty<DropdownButtonAlign> AlignProperty =
        AvaloniaProperty.Register<DropdownButton, DropdownButtonAlign>(nameof(Align), DropdownButtonAlign.Center);

    /// <summary>
    /// Defines the <see cref="IsOpen"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsOpenProperty =
        AvaloniaProperty.Register<DropdownButton, bool>(nameof(IsOpen));

    /// <summary>
    /// Defines the <see cref="IsLoading"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsLoadingProperty =
        AvaloniaProperty.Register<DropdownButton, bool>(nameof(IsLoading));

    /// <summary>
    /// Defines the <see cref="CloseOnEscape"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> CloseOnEscapeProperty =
        AvaloniaProperty.Register<DropdownButton, bool>(nameof(CloseOnEscape), true);

    /// <summary>
    /// Defines the <see cref="HasDropDownContent"/> readonly property.
    /// </summary>
    public static readonly StyledProperty<bool> HasDropDownContentProperty =
        AvaloniaProperty.Register<DropdownButton, bool>(nameof(HasDropDownContent));

    static DropdownButton()
    {
        DropDownContentProperty.Changed.AddClassHandler<DropdownButton>((x, _) => x.SyncSlotStates());
        IsOpenProperty.Changed.AddClassHandler<DropdownButton>((x, _) => x.SyncClasses());
        IsLoadingProperty.Changed.AddClassHandler<DropdownButton>((x, _) => x.SyncClasses());
        PlacementProperty.Changed.AddClassHandler<DropdownButton>((x, _) => x.SyncClasses());
        AlignProperty.Changed.AddClassHandler<DropdownButton>((x, _) => x.SyncClasses());
    }

    public DropdownButton()
    {
        Focusable = false;
        SyncClasses();
        SyncSlotStates();
    }

    /// <summary>
    /// Gets or sets the dropdown content.
    /// </summary>
    public object? DropDownContent
    {
        get => GetValue(DropDownContentProperty);
        set => SetValue(DropDownContentProperty, value);
    }

    /// <summary>
    /// Gets or sets the template for the dropdown content.
    /// </summary>
    public IDataTemplate? DropDownContentTemplate
    {
        get => GetValue(DropDownContentTemplateProperty);
        set => SetValue(DropDownContentTemplateProperty, value);
    }

    /// <summary>
    /// Gets or sets the dropdown placement relative to the button.
    /// </summary>
    public PlacementMode Placement
    {
        get => GetValue(PlacementProperty);
        set => SetValue(PlacementProperty, value);
    }

    /// <summary>
    /// Gets or sets the dropdown alignment.
    /// </summary>
    public DropdownButtonAlign Align
    {
        get => GetValue(AlignProperty);
        set => SetValue(AlignProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the dropdown is open.
    /// </summary>
    public bool IsOpen
    {
        get => GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the button is in a loading state.
    /// </summary>
    public bool IsLoading
    {
        get => GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }

    /// <summary>
    /// Gets or sets whether pressing Escape closes the dropdown.
    /// </summary>
    public bool CloseOnEscape
    {
        get => GetValue(CloseOnEscapeProperty);
        set => SetValue(CloseOnEscapeProperty, value);
    }

    /// <summary>
    /// Gets whether dropdown content is present.
    /// </summary>
    public bool HasDropDownContent => GetValue(HasDropDownContentProperty);

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        if (_trigger is not null)
        {
            _trigger.Click -= OnTriggerClick;
        }

        base.OnApplyTemplate(e);

        _trigger = e.NameScope.Find<Button>("PART_Trigger");

        if (_trigger is not null)
        {
            _trigger.Click += OnTriggerClick;
        }
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (e.Key == Key.Escape && CloseOnEscape && IsOpen)
        {
            IsOpen = false;
            e.Handled = true;
            return;
        }

        base.OnKeyDown(e);
    }

    private void OnTriggerClick(object? sender, RoutedEventArgs e)
    {
        IsOpen = !IsOpen;
    }

    private void SyncClasses()
    {
        Classes.Set("open", IsOpen);
        Classes.Set("closed", !IsOpen);
        Classes.Set("loading", IsLoading);
        Classes.Set("align-center", Align == DropdownButtonAlign.Center);
        Classes.Set("align-start", Align == DropdownButtonAlign.Start);
        Classes.Set("align-end", Align == DropdownButtonAlign.End);
    }

    private void SyncSlotStates()
    {
        SetValue(HasDropDownContentProperty, HasValue(DropDownContent));
        Classes.Set("has-dropdown-content", HasDropDownContent);
    }

    private static bool HasValue(object? value)
    {
        return value is string text ? !string.IsNullOrWhiteSpace(text) : value is not null;
    }
}
