using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;

namespace AuraUI.Core.Helpers;

/// <summary>
/// Provides attached properties for responsive behavior on any <see cref="Control"/>.
/// Controls can specify minimum and maximum width constraints, or a named breakpoint,
/// and they will be shown or hidden based on the current window size.
/// </summary>
/// <example>
/// <code>
/// &lt;TextBlock Text="Only visible on wide screens"
///            ResponsiveHelper.MinWidth="600"/&gt;
/// &lt;TextBlock Text="Hidden on wide screens"
///            ResponsiveHelper.MaxWidth="599"/&gt;
/// &lt;TextBlock Text="Visible at 'tablet' breakpoint"
///            ResponsiveHelper.BreakpointName="tablet"/&gt;
/// </code>
/// </example>
public static class ResponsiveHelper
{
    /// <summary>
    /// The minimum window width at which this control becomes visible.
    /// Set to 0 (default) to ignore.
    /// </summary>
    public static readonly AttachedProperty<double> MinWidthProperty =
        AvaloniaProperty.RegisterAttached<Control, double>("MinWidth", typeof(ResponsiveHelper), 0d);

    /// <summary>
    /// The maximum window width at which this control remains visible.
    /// Set to double.MaxValue (default) to ignore.
    /// </summary>
    public static readonly AttachedProperty<double> MaxWidthProperty =
        AvaloniaProperty.RegisterAttached<Control, double>("MaxWidth", typeof(ResponsiveHelper), double.MaxValue);

    /// <summary>
    /// A named breakpoint. The control is shown when the window width falls within
    /// the registered breakpoint range. Use RegisterBreakpoint to define ranges.
    /// </summary>
    public static readonly AttachedProperty<string?> BreakpointNameProperty =
        AvaloniaProperty.RegisterAttached<Control, string?>("BreakpointName", typeof(ResponsiveHelper));

    /// <summary>
    /// When true, the control is collapsed (not just hidden) when outside the breakpoint range.
    /// Default is true (collapsed).
    /// </summary>
    public static readonly AttachedProperty<bool> CollapseWhenHiddenProperty =
        AvaloniaProperty.RegisterAttached<Control, bool>("CollapseWhenHidden", typeof(ResponsiveHelper), true);

    private static readonly Dictionary<string, (double MinWidth, double MaxWidth)> BreakpointRegistry = new();

    static ResponsiveHelper()
    {
        MinWidthProperty.Changed.AddClassHandler<Control>(OnConstraintChanged);
        MaxWidthProperty.Changed.AddClassHandler<Control>(OnConstraintChanged);
        BreakpointNameProperty.Changed.AddClassHandler<Control>(OnConstraintChanged);
    }

    public static double GetMinWidth(Control element) => element.GetValue(MinWidthProperty);
    public static void SetMinWidth(Control element, double value) => element.SetValue(MinWidthProperty, value);

    public static double GetMaxWidth(Control element) => element.GetValue(MaxWidthProperty);
    public static void SetMaxWidth(Control element, double value) => element.SetValue(MaxWidthProperty, value);

    public static string? GetBreakpointName(Control element) => element.GetValue(BreakpointNameProperty);
    public static void SetBreakpointName(Control element, string? value) => element.SetValue(BreakpointNameProperty, value);

    public static bool GetCollapseWhenHidden(Control element) => element.GetValue(CollapseWhenHiddenProperty);
    public static void SetCollapseWhenHidden(Control element, bool value) => element.SetValue(CollapseWhenHiddenProperty, value);

    /// <summary>
    /// Registers a named breakpoint range. Controls with a matching BreakpointName attached property
    /// will be visible when the window width is between <paramref name="minWidth"/> and <paramref name="maxWidth"/>.
    /// </summary>
    /// <param name="name">The breakpoint name (e.g., "mobile", "tablet", "desktop").</param>
    /// <param name="minWidth">The minimum window width.</param>
    /// <param name="maxWidth">The maximum window width.</param>
    public static void RegisterBreakpoint(string name, double minWidth, double maxWidth)
    {
        BreakpointRegistry[name] = (minWidth, maxWidth);
    }

    /// <summary>
    /// Clears a previously registered breakpoint.
    /// </summary>
    public static void UnregisterBreakpoint(string name)
    {
        BreakpointRegistry.Remove(name);
    }

    /// <summary>
    /// Gets all registered breakpoint names and their ranges.
    /// </summary>
    public static IReadOnlyDictionary<string, (double MinWidth, double MaxWidth)> GetRegisteredBreakpoints()
        => BreakpointRegistry;

    private static void OnConstraintChanged(Control control, AvaloniaPropertyChangedEventArgs e)
    {
        EnsureWindowHooked(control);
        UpdateVisibility(control);
    }

    private static void EnsureWindowHooked(Control control)
    {
        // We hook into the attached-to-visual-tree event on the control
        control.AttachedToVisualTree += OnControlAttachedToVisualTree;
        control.DetachedFromVisualTree += OnControlDetachedFromVisualTree;
    }

    private static void OnControlAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        if (sender is not Control control) return;

        var topLevel = TopLevel.GetTopLevel(control);
        if (topLevel is not null)
        {
            topLevel.PropertyChanged += OnTopLevelPropertyChanged;
        }

        UpdateVisibility(control);
    }

    private static void OnControlDetachedFromVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        if (sender is not Control control) return;

        var topLevel = TopLevel.GetTopLevel(control);
        if (topLevel is not null)
        {
            topLevel.PropertyChanged -= OnTopLevelPropertyChanged;
        }
    }

    private static void OnTopLevelPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == Visual.BoundsProperty && sender is TopLevel topLevel)
        {
            // Update all children with responsive constraints
            UpdateDescendantVisibility(topLevel);
        }
    }

    private static void UpdateDescendantVisibility(Visual root)
    {
        foreach (var child in root.GetVisualChildren())
        {
            if (child is Control control)
            {
                var hasMin = control.IsSet(MinWidthProperty) && GetMinWidth(control) > 0;
                var hasMax = control.IsSet(MaxWidthProperty) && GetMaxWidth(control) < double.MaxValue;
                var hasBreakpoint = control.IsSet(BreakpointNameProperty) && GetBreakpointName(control) is not null;

                if (hasMin || hasMax || hasBreakpoint)
                {
                    UpdateVisibility(control);
                }
            }

            UpdateDescendantVisibility(child);
        }
    }

    private static void UpdateVisibility(Control control)
    {
        var topLevel = TopLevel.GetTopLevel(control);
        if (topLevel is null) return;

        var windowWidth = topLevel.Bounds.Width;
        var isVisible = true;

        // Check min/max width constraints
        var minWidth = GetMinWidth(control);
        var maxWidth = GetMaxWidth(control);

        if (minWidth > 0 && windowWidth < minWidth)
            isVisible = false;

        if (maxWidth < double.MaxValue && windowWidth > maxWidth)
            isVisible = false;

        // Check named breakpoint
        var breakpointName = GetBreakpointName(control);
        if (breakpointName is not null && BreakpointRegistry.TryGetValue(breakpointName, out var range))
        {
            if (windowWidth < range.MinWidth || windowWidth > range.MaxWidth)
                isVisible = false;
        }

        var collapse = GetCollapseWhenHidden(control);
        control.IsVisible = isVisible;
        if (!isVisible && collapse)
        {
            // Collapsed state is handled by IsVisible in Avalonia (unlike WPF's Visibility.Collapsed vs Hidden)
            // IsVisible = false already collapses in Avalonia
        }
    }
}
