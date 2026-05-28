using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;

namespace AuraUI.Core.Helpers;

public static class LoadingHelper
{
    public static readonly AttachedProperty<bool> IsLoadingProperty =
        AvaloniaProperty.RegisterAttached<Control, bool>("IsLoading", typeof(LoadingHelper), defaultValue: false);

    private static readonly AttachedProperty<Panel?> OverlayPanelProperty =
        AvaloniaProperty.RegisterAttached<Control, Panel?>("OverlayPanel", typeof(LoadingHelper));

    private static readonly AttachedProperty<Border?> OverlayBorderProperty =
        AvaloniaProperty.RegisterAttached<Control, Border?>("OverlayBorder", typeof(LoadingHelper));

    private static readonly AttachedProperty<bool> IsHandlerAttachedProperty =
        AvaloniaProperty.RegisterAttached<Control, bool>("IsHandlerAttached", typeof(LoadingHelper));

    static LoadingHelper()
    {
        IsLoadingProperty.Changed.AddClassHandler<Control>(OnIsLoadingChanged);
    }

    public static bool GetIsLoading(Control element) => element.GetValue(IsLoadingProperty);
    public static void SetIsLoading(Control element, bool value) => element.SetValue(IsLoadingProperty, value);

    private static void OnIsLoadingChanged(Control control, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.NewValue is not bool isLoading)
            return;

        if (isLoading)
        {
            ShowOverlay(control);
        }
        else
        {
            HideOverlay(control);
        }
    }

    private static void ShowOverlay(Control host)
    {
        // If the host is a Panel, we can add an overlay directly
        if (host is Panel panel)
        {
            AddOverlayToPanel(panel);
            return;
        }

        // If the host has a Content property (ContentControl, Window, etc.)
        if (host is ContentControl contentControl)
        {
            if (contentControl.Content is Panel existingPanel)
            {
                AddOverlayToPanel(existingPanel);
            }
            else
            {
                // Wrap existing content in a Grid with overlay
                var grid = new Grid();
                var originalContent = contentControl.Content;

                if (originalContent is Control originalControl)
                {
                    contentControl.Content = null;
                    grid.Children.Add(originalControl);
                }

                var overlay = CreateOverlay(host);
                grid.Children.Add(overlay);
                contentControl.Content = grid;

                host.SetValue(OverlayBorderProperty, overlay);
            }
            return;
        }

        // For Border controls, add overlay as a child if it has a Panel child
        if (host is Border border)
        {
            if (border.Child is Panel childPanel)
            {
                AddOverlayToPanel(childPanel);
            }
            else if (border.Child is not null)
            {
                var grid = new Grid();
                var originalChild = border.Child;
                border.Child = null;
                grid.Children.Add(originalChild);

                var overlay = CreateOverlay(host);
                grid.Children.Add(overlay);
                border.Child = grid;

                host.SetValue(OverlayBorderProperty, overlay);
            }
        }
    }

    private static void AddOverlayToPanel(Panel panel)
    {
        var overlay = CreateOverlay(panel);
        panel.Children.Add(overlay);
        panel.SetValue(OverlayBorderProperty, overlay);
    }

    private static Border CreateOverlay(Control host)
    {
        var spinner = new ProgressBar
        {
            IsIndeterminate = true,
            Height = 4,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
            MaxWidth = 120,
            Classes = { "loading-overlay-progress" }
        };

        var overlayContent = new StackPanel
        {
            Orientation = Avalonia.Layout.Orientation.Vertical,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
            Spacing = 8,
            Children = { spinner }
        };

        var overlay = new Border
        {
            Background = new SolidColorBrush(Color.FromArgb(128, 255, 255, 255)),
            Child = overlayContent,
            ZIndex = int.MaxValue,
            IsHitTestVisible = true,
            Classes = { "loading-overlay" }
        };

        return overlay;
    }

    private static void HideOverlay(Control host)
    {
        // Try direct overlay removal
        var overlay = host.GetValue(OverlayBorderProperty);
        if (overlay is not null)
        {
            // If overlay is in a Panel, remove it
            if (overlay.Parent is Panel panel)
            {
                panel.Children.Remove(overlay);
            }
            // If overlay replaced content, restore
            else if (overlay.Parent is Grid grid)
            {
                if (grid.Parent is ContentControl contentControl)
                {
                    // Restore first child as content
                    if (grid.Children.Count > 0 && grid.Children[0] is Control firstChild)
                    {
                        grid.Children.Remove(firstChild);
                        contentControl.Content = firstChild;
                    }
                }
                else if (grid.Parent is Border border)
                {
                    if (grid.Children.Count > 0 && grid.Children[0] is Control firstChild)
                    {
                        grid.Children.Remove(firstChild);
                        border.Child = firstChild;
                    }
                }
            }

            host.ClearValue(OverlayBorderProperty);
        }
    }
}
