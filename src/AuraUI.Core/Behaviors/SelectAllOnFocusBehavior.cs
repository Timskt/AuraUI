using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

namespace AuraUI.Core.Behaviors;

/// <summary>
/// Behavior that selects all text in a TextBox when it receives focus.
/// </summary>
public class SelectAllOnFocusBehavior : Behavior<TextBox>
{
    #region IsEnabled

    public static readonly StyledProperty<bool> IsEnabledProperty =
        AvaloniaProperty.RegisterAttached<TextBox, bool>(
            "SelectAllOnFocusBehavior_IsEnabled", typeof(SelectAllOnFocusBehavior), true);

    /// <summary>
    /// Gets or sets whether the select-all-on-focus behavior is enabled.
    /// </summary>
    public bool IsEnabled
    {
        get => GetValue(IsEnabledProperty);
        set => SetValue(IsEnabledProperty, value);
    }

    #endregion

    protected override void OnAttached()
    {
        base.OnAttached();

        if (AssociatedObject != null)
        {
            AssociatedObject.GotFocus += OnGotFocus;
            AssociatedObject.PointerPressed += OnPointerPressed;
        }
    }

    protected override void OnDetaching()
    {
        if (AssociatedObject != null)
        {
            AssociatedObject.GotFocus -= OnGotFocus;
            AssociatedObject.PointerPressed -= OnPointerPressed;
        }

        base.OnDetaching();
    }

#if AVALONIA_12
    private void OnGotFocus(object? sender, FocusChangedEventArgs e)
#else
    private void OnGotFocus(object? sender, GotFocusEventArgs e)
#endif
    {
        if (!IsEnabled || AssociatedObject == null)
            return;

        // Only select all if focus was received via keyboard navigation or programmatic focus,
        // not via mouse click (which would position the caret at the click point)
        if (e.NavigationMethod == NavigationMethod.Directional ||
            e.NavigationMethod == NavigationMethod.Tab ||
            e.NavigationMethod == NavigationMethod.Unspecified)
        {
            AssociatedObject.SelectAll();
        }
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (!IsEnabled || AssociatedObject == null)
            return;

        // Select all on click only if the TextBox wasn't already focused
        // This allows normal cursor positioning when already focused
        if (!AssociatedObject.IsFocused)
        {
            // Use dispatcher to ensure the focus event completes first
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                AssociatedObject?.SelectAll();
            }, Avalonia.Threading.DispatcherPriority.Input);
        }
    }

    /// <summary>
    /// Static getter for AXAML usage.
    /// </summary>
    public static bool GetIsEnabled(TextBox element) => element.GetValue(IsEnabledProperty);

    /// <summary>
    /// Static setter for AXAML usage.
    /// </summary>
    public static void SetIsEnabled(TextBox element, bool value) => element.SetValue(IsEnabledProperty, value);
}
