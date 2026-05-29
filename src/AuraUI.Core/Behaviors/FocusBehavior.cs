using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;

namespace AuraUI.Core.Behaviors;

/// <summary>
/// Behavior that manages focus state for a control.
/// Supports two-way binding of the IsFocused property and automatic focus on load.
/// </summary>
public class FocusBehavior : Behavior<Control>
{
    private bool _isUpdatingFocus;

    #region IsFocused

    public static readonly StyledProperty<bool> IsFocusedProperty =
        AvaloniaProperty.RegisterAttached<Control, bool>(
            "FocusBehavior_IsFocused", typeof(FocusBehavior));

    /// <summary>
    /// Gets or sets whether the control is focused.
    /// Supports two-way binding.
    /// </summary>
    public bool IsFocused
    {
        get => GetValue(IsFocusedProperty);
        set => SetValue(IsFocusedProperty, value);
    }

    #endregion

    #region FocusOnLoaded

    public static readonly StyledProperty<bool> FocusOnLoadedProperty =
        AvaloniaProperty.RegisterAttached<Control, bool>(
            "FocusBehavior_FocusOnLoaded", typeof(FocusBehavior));

    /// <summary>
    /// When true, the control automatically receives focus when it is loaded.
    /// </summary>
    public bool FocusOnLoaded
    {
        get => GetValue(FocusOnLoadedProperty);
        set => SetValue(FocusOnLoadedProperty, value);
    }

    #endregion

    #region SelectAllOnFocus

    public static readonly StyledProperty<bool> SelectAllOnFocusProperty =
        AvaloniaProperty.RegisterAttached<Control, bool>(
            "FocusBehavior_SelectAllOnFocus", typeof(FocusBehavior));

    /// <summary>
    /// When true and the control is a TextBox, selects all text when focused.
    /// </summary>
    public bool SelectAllOnFocus
    {
        get => GetValue(SelectAllOnFocusProperty);
        set => SetValue(SelectAllOnFocusProperty, value);
    }

    #endregion

    protected override void OnAttached()
    {
        base.OnAttached();

        if (AssociatedObject == null)
            return;

        AssociatedObject.GotFocus += OnGotFocus;
        AssociatedObject.LostFocus += OnLostFocus;
        AssociatedObject.AttachedToVisualTree += OnAttachedToVisualTree;
        AssociatedObject.DetachedFromVisualTree += OnDetachedFromVisualTree;

        // React to property changes
        IsFocusedProperty.Changed.AddClassHandler<Control>((c, e) =>
        {
            // Only react on the actual associated object
        });
    }

    protected override void OnDetaching()
    {
        if (AssociatedObject != null)
        {
            AssociatedObject.GotFocus -= OnGotFocus;
            AssociatedObject.LostFocus -= OnLostFocus;
            AssociatedObject.AttachedToVisualTree -= OnAttachedToVisualTree;
            AssociatedObject.DetachedFromVisualTree -= OnDetachedFromVisualTree;
        }

        base.OnDetaching();
    }

    private void OnAttachedToVisualTree(object? sender, Avalonia.VisualTreeAttachmentEventArgs e)
    {
        if (FocusOnLoaded && AssociatedObject != null)
        {
            Dispatcher.UIThread.Post(() =>
            {
                AssociatedObject?.Focus();
            }, DispatcherPriority.Loaded);
        }
    }

    private void OnDetachedFromVisualTree(object? sender, Avalonia.VisualTreeAttachmentEventArgs e)
    {
        // Update IsFocused when control leaves the tree
        _isUpdatingFocus = true;
        try
        {
            SetIsFocusedValue(false);
        }
        finally
        {
            _isUpdatingFocus = false;
        }
    }

    private void OnGotFocus(object? sender, GotFocusEventArgs e)
    {
        if (_isUpdatingFocus)
            return;

        _isUpdatingFocus = true;
        try
        {
            SetIsFocusedValue(true);

            // Select all text in TextBox if configured
            if (SelectAllOnFocus && AssociatedObject is TextBox textBox)
            {
                textBox.SelectAll();
            }
        }
        finally
        {
            _isUpdatingFocus = false;
        }
    }

    private void OnLostFocus(object? sender, RoutedEventArgs e)
    {
        if (_isUpdatingFocus)
            return;

        _isUpdatingFocus = true;
        try
        {
            SetIsFocusedValue(false);
        }
        finally
        {
            _isUpdatingFocus = false;
        }
    }

    private void SetIsFocusedValue(bool value)
    {
        if (AssociatedObject != null)
        {
            // This allows two-way binding by setting the attached property on the target control
            AssociatedObject.SetValue(IsFocusedProperty, value);
        }
    }

    /// <summary>
    /// Static getter for AXAML usage.
    /// </summary>
    public static bool GetIsFocused(Control element) => element.GetValue(IsFocusedProperty);

    /// <summary>
    /// Static setter for AXAML usage.
    /// </summary>
    public static void SetIsFocused(Control element, bool value) => element.SetValue(IsFocusedProperty, value);

    /// <summary>
    /// Static getter for AXAML usage.
    /// </summary>
    public static bool GetFocusOnLoaded(Control element) => element.GetValue(FocusOnLoadedProperty);

    /// <summary>
    /// Static setter for AXAML usage.
    /// </summary>
    public static void SetFocusOnLoaded(Control element, bool value) => element.SetValue(FocusOnLoadedProperty, value);
}
