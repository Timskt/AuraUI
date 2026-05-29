using Avalonia;
using Avalonia.Controls;

namespace AuraUI.Controls.Layout;

/// <summary>
/// A responsive <see cref="Grid"/> that switches between different column definitions
/// based on the available width. Useful for form layouts that reorganize at different
/// screen sizes.
/// </summary>
/// <example>
/// <code>
/// &lt;layout:ResponsiveGrid ColumnSpacing="8" RowSpacing="8"&gt;
///     &lt;layout:ResponsiveGrid.Breakpoints&gt;
///         &lt;ColumnDefinitions x:Key="0"&gt;*&lt;/ColumnDefinitions&gt;
///         &lt;ColumnDefinitions x:Key="600"&gt;*,*&lt;/ColumnDefinitions&gt;
///         &lt;ColumnDefinitions x:Key="900"&gt;*,*,*&lt;/ColumnDefinitions&gt;
///     &lt;/layout:ResponsiveGrid.Breakpoints&gt;
/// &lt;/layout:ResponsiveGrid&gt;
/// </code>
/// </example>
public class ResponsiveGrid : Grid
{
    /// <summary>
    /// Defines the <see cref="Breakpoints"/> styled property.
    /// Maps minimum widths (keys) to column definitions (values).
    /// The largest breakpoint that fits the available width wins.
    /// </summary>
    public static readonly StyledProperty<Dictionary<int, ColumnDefinitions>?> BreakpointsProperty =
        AvaloniaProperty.Register<ResponsiveGrid, Dictionary<int, ColumnDefinitions>?>(nameof(Breakpoints));

    /// <summary>
    /// Defines the <see cref="ColumnSpacing"/> styled property.
    /// Uniform spacing between columns.
    /// </summary>
    public static new readonly StyledProperty<double> ColumnSpacingProperty =
        AvaloniaProperty.Register<ResponsiveGrid, double>(nameof(ColumnSpacing), 0.0);

    /// <summary>
    /// Defines the <see cref="RowSpacing"/> styled property.
    /// Uniform spacing between rows.
    /// </summary>
    public static new readonly StyledProperty<double> RowSpacingProperty =
        AvaloniaProperty.Register<ResponsiveGrid, double>(nameof(RowSpacing), 0.0);

    /// <summary>
    /// Defines the <see cref="BreakpointColumnSpanProperty"/> attached property.
    /// Allows setting a column span for a specific breakpoint width on a child.
    /// </summary>
    public static readonly AttachedProperty<int> BreakpointColumnSpanProperty =
        AvaloniaProperty.RegisterAttached<ResponsiveGrid, Control, int>("BreakpointColumnSpan", 1);

    /// <summary>
    /// Defines the <see cref="BreakpointRowSpanProperty"/> attached property.
    /// Allows setting a row span for a specific breakpoint width on a child.
    /// </summary>
    public static readonly AttachedProperty<int> BreakpointRowSpanProperty =
        AvaloniaProperty.RegisterAttached<ResponsiveGrid, Control, int>("BreakpointRowSpan", 1);

    private int _activeBreakpoint = -1;

    static ResponsiveGrid()
    {
        BreakpointsProperty.Changed.AddClassHandler<ResponsiveGrid>((x, _) => x.OnBreakpointsChanged());
    }

    /// <summary>
    /// Gets or sets the breakpoint-to-column-definition mapping.
    /// </summary>
    public Dictionary<int, ColumnDefinitions>? Breakpoints
    {
        get => GetValue(BreakpointsProperty);
        set => SetValue(BreakpointsProperty, value);
    }

    /// <summary>
    /// Gets or sets uniform spacing between columns.
    /// </summary>
    public new double ColumnSpacing
    {
        get => GetValue(ColumnSpacingProperty);
        set => SetValue(ColumnSpacingProperty, value);
    }

    /// <summary>
    /// Gets or sets uniform spacing between rows.
    /// </summary>
    public new double RowSpacing
    {
        get => GetValue(RowSpacingProperty);
        set => SetValue(RowSpacingProperty, value);
    }

    public static int GetBreakpointColumnSpan(Control element) => element.GetValue(BreakpointColumnSpanProperty);
    public static void SetBreakpointColumnSpan(Control element, int value) => element.SetValue(BreakpointColumnSpanProperty, value);

    public static int GetBreakpointRowSpan(Control element) => element.GetValue(BreakpointRowSpanProperty);
    public static void SetBreakpointRowSpan(Control element, int value) => element.SetValue(BreakpointRowSpanProperty, value);

    protected override Size MeasureOverride(Size availableSize)
    {
        ApplyBreakpoint(availableSize.Width);
        return base.MeasureOverride(availableSize);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        ApplyBreakpoint(finalSize.Width);
        return base.ArrangeOverride(finalSize);
    }

    private void ApplyBreakpoint(double width)
    {
        var breakpoints = Breakpoints;
        if (breakpoints is not { Count: > 0 })
            return;

        if (double.IsInfinity(width))
            return;

        // Find the best matching breakpoint
        int bestKey = -1;
        ColumnDefinitions? bestDefs = null;

        foreach (var kvp in breakpoints)
        {
            if (width >= kvp.Key && kvp.Key > bestKey)
            {
                bestKey = kvp.Key;
                bestDefs = kvp.Value;
            }
        }

        if (bestDefs is null || bestKey == _activeBreakpoint)
            return;

        _activeBreakpoint = bestKey;

        // Apply the column definitions, preserving user-set row definitions
        var currentRows = RowDefinitions.Count > 0
            ? new RowDefinitions()
            : null;
        if (currentRows is not null)
        {
            foreach (var rd in RowDefinitions)
                currentRows.Add(new RowDefinition(rd.Height));
        }

        ColumnDefinitions.Clear();
        foreach (var cd in bestDefs)
        {
            ColumnDefinitions.Add(new ColumnDefinition(cd.Width));
        }

        if (currentRows is not null)
        {
            RowDefinitions.Clear();
            foreach (var rd in currentRows)
                RowDefinitions.Add(rd);
        }
    }

    private void OnBreakpointsChanged()
    {
        _activeBreakpoint = -1;
        InvalidateMeasure();
    }
}
