using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;

namespace AuraUI.Core.Behaviors;

/// <summary>
/// Behavior that manages focus state for a control.
/// Supports two-way binding of the IsFocused property and automatic focus on load.
/// </summary>
public class FocusBehavior : Behavior<Control>
{
    private bool _isUpdatingFocus;

    /// <summary>
    /// Gets or sets whether the control is focused.
    /// Supports two-way binding.
    /// </summary>
    public bool IsFocused { get; set; }

    /// <summary>
    /// When true, the control automatically receives focus when it is loaded.
    /// </summary>
    public bool FocusOnLoaded { get; set; }

    /// <summary>
    /// When true and the control is a TextBox, selects all text when focused.
    /// </summary>
    public bool SelectAllOnFocus { get; set; }

    protected override void OnAttached()
    {
        base.OnAttached();

        if (AssociatedObject == null)
            return;

        AssociatedObject.GotFocus += OnGotFocus;
        AssociatedObject.LostFocus += OnLostFocus;
        AssociatedObject.AttachedToVisualTree += OnAttachedToVisualTree;
        AssociatedObject.DetachedFromVisualTree += OnDetachedFromVisualTree;
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

    private void OnGotFocus(object? sender, FocusChangedEventArgs e)
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
            IsFocused = value;
        }
    }
}
