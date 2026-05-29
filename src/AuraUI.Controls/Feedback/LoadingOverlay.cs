using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;

namespace AuraUI.Controls.Feedback;

/// <summary>
/// Provides attached properties for showing a loading overlay with spinner over any container.
/// </summary>
[PseudoClasses(":loading")]
public class LoadingOverlay : AvaloniaObject
{
    /// <summary>
    /// Defines the IsLoading attached property.
    /// </summary>
    public static readonly AttachedProperty<bool> IsLoadingProperty =
        AvaloniaProperty.RegisterAttached<LoadingOverlay, Control, bool>("IsLoading");

    /// <summary>
    /// Defines the Message attached property.
    /// </summary>
    public static readonly AttachedProperty<string?> MessageProperty =
        AvaloniaProperty.RegisterAttached<LoadingOverlay, Control, string?>("Message");

    /// <summary>
    /// Defines the SpinnerSize attached property.
    /// </summary>
    public static readonly AttachedProperty<double> SpinnerSizeProperty =
        AvaloniaProperty.RegisterAttached<LoadingOverlay, Control, double>("SpinnerSize", 40.0);

    private static readonly AttachedProperty<Border?> OverlayBorderProperty =
        AvaloniaProperty.RegisterAttached<LoadingOverlay, Control, Border?>("OverlayBorder");

    private static readonly AttachedProperty<object?> OriginalContentProperty =
        AvaloniaProperty.RegisterAttached<LoadingOverlay, Control, object?>("OriginalContent");

    static LoadingOverlay()
    {
        IsLoadingProperty.Changed.AddClassHandler<Control>(OnIsLoadingChanged);
        MessageProperty.Changed.AddClassHandler<Control>(OnMessageChanged);
    }

    /// <summary>
    /// Gets the IsLoading value for the specified control.
    /// </summary>
    public static bool GetIsLoading(Control control) => control.GetValue(IsLoadingProperty);

    /// <summary>
    /// Sets the IsLoading value for the specified control.
    /// </summary>
    public static void SetIsLoading(Control control, bool value) => control.SetValue(IsLoadingProperty, value);

    /// <summary>
    /// Gets the loading message for the specified control.
    /// </summary>
    public static string? GetMessage(Control control) => control.GetValue(MessageProperty);

    /// <summary>
    /// Sets the loading message for the specified control.
    /// </summary>
    public static void SetMessage(Control control, string? value) => control.SetValue(MessageProperty, value);

    /// <summary>
    /// Gets the spinner size for the specified control.
    /// </summary>
    public static double GetSpinnerSize(Control control) => control.GetValue(SpinnerSizeProperty);

    /// <summary>
    /// Sets the spinner size for the specified control.
    /// </summary>
    public static void SetSpinnerSize(Control control, double value) => control.SetValue(SpinnerSizeProperty, value);

    private static void OnIsLoadingChanged(Control control, AvaloniaPropertyChangedEventArgs e)
    {
        var isLoading = (bool)e.NewValue!;
        if (isLoading)
            control.Classes.Add(":loading");
        else
            control.Classes.Remove(":loading");

        if (isLoading)
        {
            ShowOverlay(control);
        }
        else
        {
            HideOverlay(control);
        }
    }

    private static void OnMessageChanged(Control control, AvaloniaPropertyChangedEventArgs e)
    {
        var overlay = control.GetValue(OverlayBorderProperty);
        if (overlay?.Child is StackPanel panel)
        {
            foreach (var child in panel.Children)
            {
                if (child is TextBlock textBlock)
                {
                    textBlock.Text = (string?)e.NewValue;
                    textBlock.IsVisible = !string.IsNullOrEmpty(textBlock.Text);
                    break;
                }
            }
        }
    }

    private static void ShowOverlay(Control control)
    {
        if (control.GetValue(OverlayBorderProperty) != null) return;

        var message = GetMessage(control);
        var spinnerSize = GetSpinnerSize(control);

        var spinner = new ProgressBar
        {
            IsIndeterminate = true,
            Width = spinnerSize,
            Height = spinnerSize / 8,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            MinWidth = 40
        };

        var textBlock = new TextBlock
        {
            Text = message,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(0, 12, 0, 0),
            IsVisible = !string.IsNullOrEmpty(message),
            Foreground = Brushes.White,
            FontSize = 14
        };

        var stackPanel = new StackPanel
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Spacing = 8
        };
        stackPanel.Children.Add(spinner);
        stackPanel.Children.Add(textBlock);

        var overlayBorder = new Border
        {
            Background = new SolidColorBrush(Color.FromArgb(128, 0, 0, 0)),
            Child = stackPanel,
            ZIndex = 10000,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch
        };

        control.SetValue(OverlayBorderProperty, overlayBorder);

        if (control is Panel panel)
        {
            panel.Children.Add(overlayBorder);
        }
        else if (control is ContentControl contentControl)
        {
            // Store original content so we can restore it when hiding
            control.SetValue(OriginalContentProperty, contentControl.Content);

            // Wrap existing content
            var existingContent = contentControl.Content;
            var grid = new Grid();
            if (existingContent is Control existingControl)
            {
                grid.Children.Add(existingControl);
            }
            else if (existingContent != null)
            {
                var presenter = new ContentPresenter { Content = existingContent };
                grid.Children.Add(presenter);
            }
            grid.Children.Add(overlayBorder);
            contentControl.Content = grid;
        }
    }

    private static void HideOverlay(Control control)
    {
        var overlay = control.GetValue(OverlayBorderProperty);
        if (overlay == null) return;

        control.ClearValue(OverlayBorderProperty);

        if (control is Panel panel)
        {
            panel.Children.Remove(overlay);
        }
        else if (control is ContentControl contentControl)
        {
            // Restore original content
            var originalContent = control.GetValue(OriginalContentProperty);
            contentControl.Content = originalContent;
            control.ClearValue(OriginalContentProperty);
        }
    }
}
