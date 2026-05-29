using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace AuraUI.Core.Behaviors;

/// <summary>
/// Behavior that shows a watermark (placeholder) text on a TextBox when it is empty and not focused.
/// Uses a TextBlock overlay via AdornerLayer to display the watermark.
/// </summary>
public class WatermarkBehavior : Behavior<TextBox>
{
    private TextBlock? _watermarkTextBlock;
    private bool _isWatermarkVisible;

    /// <summary>
    /// Gets or sets the watermark text to display when the TextBox is empty and unfocused.
    /// </summary>
    public string? WatermarkText { get; set; }

    /// <summary>
    /// Gets or sets the brush used to render the watermark text.
    /// </summary>
    public IBrush? WatermarkForeground { get; set; }

    /// <summary>
    /// Gets or sets the font style for the watermark text.
    /// </summary>
    public FontStyle WatermarkFontStyle { get; set; } = FontStyle.Italic;

    protected override void OnAttached()
    {
        base.OnAttached();

        if (AssociatedObject == null)
            return;

        AssociatedObject.TextChanged += OnTextChanged;
        AssociatedObject.GotFocus += OnGotFocus;
        AssociatedObject.LostFocus += OnLostFocus;
        AssociatedObject.AttachedToVisualTree += OnAttachedToVisualTree;
        AssociatedObject.DetachedFromVisualTree += OnDetachedFromVisualTree;

        // Watermark will be created when AttachedToVisualTree fires
    }

    protected override void OnDetaching()
    {
        if (AssociatedObject != null)
        {
            AssociatedObject.TextChanged -= OnTextChanged;
            AssociatedObject.GotFocus -= OnGotFocus;
            AssociatedObject.LostFocus -= OnLostFocus;
            AssociatedObject.AttachedToVisualTree -= OnAttachedToVisualTree;
            AssociatedObject.DetachedFromVisualTree -= OnDetachedFromVisualTree;
        }

        RemoveWatermark();
        base.OnDetaching();
    }

    private void OnAttachedToVisualTree(object? sender, Avalonia.VisualTreeAttachmentEventArgs e)
    {
        CreateWatermark();
        UpdateWatermarkVisibility();
    }

    private void OnDetachedFromVisualTree(object? sender, Avalonia.VisualTreeAttachmentEventArgs e)
    {
        RemoveWatermark();
    }

    private void OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        UpdateWatermarkVisibility();
    }

    private void OnGotFocus(object? sender, Avalonia.Input.FocusChangedEventArgs e)
    {
        UpdateWatermarkVisibility();
    }

    private void OnLostFocus(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        UpdateWatermarkVisibility();
    }

    private void CreateWatermark()
    {
        if (_watermarkTextBlock != null || AssociatedObject == null)
            return;

        _watermarkTextBlock = new TextBlock
        {
            Text = WatermarkText ?? string.Empty,
            Foreground = WatermarkForeground ?? new SolidColorBrush(Colors.Gray) { Opacity = 0.5 },
            FontStyle = WatermarkFontStyle,
            IsHitTestVisible = false,
            Margin = new Thickness(
                AssociatedObject.Padding.Left + 4,
                AssociatedObject.Padding.Top + 2,
                AssociatedObject.Padding.Right,
                AssociatedObject.Padding.Bottom),
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left,
            Opacity = 0,
        };

        // Add as an overlay inside the TextBox's parent (a common pattern for watermark placement)
        // We use the adorner layer or add directly to the control's logical tree
        if (AssociatedObject.Parent is Panel panel)
        {
            // Use a Canvas overlay approach - add watermark as sibling in a Grid
            // For simpler implementation, we add the watermark as a child overlay
            var parent = AssociatedObject.Parent;

            if (parent is Grid grid)
            {
                Grid.SetRow(_watermarkTextBlock, Grid.GetRow(AssociatedObject));
                Grid.SetColumn(_watermarkTextBlock, Grid.GetColumn(AssociatedObject));
                grid.Children.Add(_watermarkTextBlock);
            }
            else if (parent is Panel hostPanel)
            {
                hostPanel.Children.Add(_watermarkTextBlock);
            }
        }
    }

    private void RemoveWatermark()
    {
        if (_watermarkTextBlock == null)
            return;

        var parent = _watermarkTextBlock.Parent;
        if (parent is Panel panel)
        {
            panel.Children.Remove(_watermarkTextBlock);
        }

        _watermarkTextBlock = null;
        _isWatermarkVisible = false;
    }

    private void UpdateWatermarkVisibility()
    {
        if (_watermarkTextBlock == null || AssociatedObject == null)
            return;

        bool isEmpty = string.IsNullOrEmpty(AssociatedObject.Text);
        bool isFocused = AssociatedObject.IsFocused;

        bool shouldShow = isEmpty && !isFocused;

        if (shouldShow != _isWatermarkVisible)
        {
            _isWatermarkVisible = shouldShow;
            _watermarkTextBlock.Opacity = shouldShow ? 1.0 : 0.0;
        }

        // Update text if it changed
        if (_watermarkTextBlock.Text != (WatermarkText ?? string.Empty))
        {
            _watermarkTextBlock.Text = WatermarkText ?? string.Empty;
        }

        // Update foreground if it changed
        if (WatermarkForeground != null && _watermarkTextBlock.Foreground != WatermarkForeground)
        {
            _watermarkTextBlock.Foreground = WatermarkForeground;
        }
    }

}
