using System.Windows.Input;
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
public enum DropdownAlign
{
    Center,
    Start,
    End
}

/// <summary>
/// A split button that combines a primary action button with a dropdown arrow
/// that opens a secondary content area (menu, list, etc.).
///
/// Template parts: PART_PrimaryAction, PART_MenuTrigger, PART_Surface
///
/// Supports variant classes: .primary, .secondary, .outline, .ghost
/// Supports size classes: .sm, .md, .lg
/// </summary>
public class SplitButton : ContentControl
{
    private Button? _primaryAction;
    private Button? _menuTrigger;
    private Control? _surface;

    /// <summary>
    /// Defines the <see cref="DropDownContent"/> styled property.
    /// </summary>
    public static readonly StyledProperty<object?> DropDownContentProperty =
        AvaloniaProperty.Register<SplitButton, object?>(nameof(DropDownContent));

    /// <summary>
    /// Defines the <see cref="DropDownContentTemplate"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IDataTemplate?> DropDownContentTemplateProperty =
        AvaloniaProperty.Register<SplitButton, IDataTemplate?>(nameof(DropDownContentTemplate));

    /// <summary>
    /// Defines the <see cref="Placement"/> styled property.
    /// </summary>
    public static readonly StyledProperty<PlacementMode> PlacementProperty =
        AvaloniaProperty.Register<SplitButton, PlacementMode>(nameof(Placement), PlacementMode.Bottom);

    /// <summary>
    /// Defines the <see cref="Align"/> styled property.
    /// </summary>
    public static readonly StyledProperty<DropdownAlign> AlignProperty =
        AvaloniaProperty.Register<SplitButton, DropdownAlign>(nameof(Align), DropdownAlign.Center);

    /// <summary>
    /// Defines the <see cref="IsOpen"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsOpenProperty =
        AvaloniaProperty.Register<SplitButton, bool>(nameof(IsOpen));

    /// <summary>
    /// Defines the <see cref="IsLoading"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsLoadingProperty =
        AvaloniaProperty.Register<SplitButton, bool>(nameof(IsLoading));

    /// <summary>
    /// Defines the <see cref="CloseOnEscape"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> CloseOnEscapeProperty =
        AvaloniaProperty.Register<SplitButton, bool>(nameof(CloseOnEscape), true);

    /// <summary>
    /// Defines the <see cref="CloseOnItemSelected"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> CloseOnItemSelectedProperty =
        AvaloniaProperty.Register<SplitButton, bool>(nameof(CloseOnItemSelected), true);

    /// <summary>
    /// Defines the <see cref="Command"/> styled property.
    /// </summary>
    public static readonly StyledProperty<ICommand?> CommandProperty =
        AvaloniaProperty.Register<SplitButton, ICommand?>(nameof(Command));

    /// <summary>
    /// Defines the <see cref="CommandParameter"/> styled property.
    /// </summary>
    public static readonly StyledProperty<object?> CommandParameterProperty =
        AvaloniaProperty.Register<SplitButton, object?>(nameof(CommandParameter));

    /// <summary>
    /// Defines the <see cref="HasDropDownContent"/> readonly property.
    /// </summary>
    public static readonly StyledProperty<bool> HasDropDownContentProperty =
        AvaloniaProperty.Register<SplitButton, bool>(nameof(HasDropDownContent));

    /// <summary>
    /// Defines the routed event for the primary action click.
    /// </summary>
    public static readonly RoutedEvent<RoutedEventArgs> ClickEvent =
        RoutedEvent.Register<SplitButton, RoutedEventArgs>(nameof(Click), RoutingStrategies.Bubble);

    static SplitButton()
    {
        DropDownContentProperty.Changed.AddClassHandler<SplitButton>((x, _) => x.SyncSlotStates());
        IsOpenProperty.Changed.AddClassHandler<SplitButton>((x, _) => x.SyncClasses());
        IsLoadingProperty.Changed.AddClassHandler<SplitButton>((x, _) => x.SyncClasses());
        PlacementProperty.Changed.AddClassHandler<SplitButton>((x, _) => x.SyncClasses());
        AlignProperty.Changed.AddClassHandler<SplitButton>((x, _) => x.SyncClasses());
        CommandProperty.Changed.AddClassHandler<SplitButton>((x, _) => x.SyncClasses());
    }

    public SplitButton()
    {
        Focusable = false;
        SyncClasses();
        SyncSlotStates();
    }

    /// <summary>
    /// Occurs when the primary action button is clicked.
    /// </summary>
    public event EventHandler<RoutedEventArgs> Click
    {
        add => AddHandler(ClickEvent, value);
        remove => RemoveHandler(ClickEvent, value);
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
    public DropdownAlign Align
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
    /// Gets or sets whether selecting an item closes the dropdown.
    /// </summary>
    public bool CloseOnItemSelected
    {
        get => GetValue(CloseOnItemSelectedProperty);
        set => SetValue(CloseOnItemSelectedProperty, value);
    }

    /// <summary>
    /// Gets or sets the command for the primary action.
    /// </summary>
    public ICommand? Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    /// <summary>
    /// Gets or sets the command parameter.
    /// </summary>
    public object? CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    /// <summary>
    /// Gets whether dropdown content is present.
    /// </summary>
    public bool HasDropDownContent => GetValue(HasDropDownContentProperty);

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        if (_primaryAction is not null)
        {
            _primaryAction.Click -= OnPrimaryActionClick;
        }

        if (_menuTrigger is not null)
        {
            _menuTrigger.Click -= OnMenuTriggerClick;
        }

        base.OnApplyTemplate(e);

        _primaryAction = e.NameScope.Find<Button>("PART_PrimaryAction");
        _menuTrigger = e.NameScope.Find<Button>("PART_MenuTrigger");
        _surface = e.NameScope.Find<Control>("PART_Surface");

        if (_primaryAction is not null)
        {
            _primaryAction.Click += OnPrimaryActionClick;
        }

        if (_menuTrigger is not null)
        {
            _menuTrigger.Click += OnMenuTriggerClick;
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

    private void OnPrimaryActionClick(object? sender, RoutedEventArgs e)
    {
        RaiseEvent(new RoutedEventArgs(ClickEvent));
        Command?.Execute(CommandParameter);
    }

    private void OnMenuTriggerClick(object? sender, RoutedEventArgs e)
    {
        IsOpen = !IsOpen;
    }

    private void SyncClasses()
    {
        Classes.Set("open", IsOpen);
        Classes.Set("closed", !IsOpen);
        Classes.Set("loading", IsLoading);
        Classes.Set("has-command", Command is not null);
        Classes.Set("align-center", Align == DropdownAlign.Center);
        Classes.Set("align-start", Align == DropdownAlign.Start);
        Classes.Set("align-end", Align == DropdownAlign.End);
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
