using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace AuraUI.Controls.Display;

/// <summary>
/// Specifies when a cell enters edit mode.
/// </summary>
public enum EditTrigger
{
    /// <summary>Cell enters edit mode on double-click.</summary>
    DoubleClick,
    /// <summary>Cell enters edit mode on single click.</summary>
    SingleClick,
    /// <summary>Cell enters edit mode programmatically only.</summary>
    Programmatic
}

/// <summary>
/// A data grid that supports inline cell editing. Click (or double-click,
/// depending on <see cref="EditTrigger"/>) on a cell to enter edit mode.
/// Press Enter to commit, Escape to cancel, and Tab to move to the next cell.
/// </summary>
[PseudoClasses(":editing", ":readonly")]
public class EditableDataGrid : TemplatedControl
{
    /// <summary>
    /// Defines the <see cref="ItemsSource"/> styled property.
    /// </summary>
    public static readonly StyledProperty<System.Collections.IEnumerable?> ItemsSourceProperty =
        AvaloniaProperty.Register<EditableDataGrid, System.Collections.IEnumerable?>(nameof(ItemsSource));

    /// <summary>
    /// Defines the <see cref="IsReadOnly"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsReadOnlyProperty =
        AvaloniaProperty.Register<EditableDataGrid, bool>(nameof(IsReadOnly));

    /// <summary>
    /// Defines the <see cref="EditTrigger"/> styled property.
    /// </summary>
    public static readonly StyledProperty<EditTrigger> EditTriggerProperty =
        AvaloniaProperty.Register<EditableDataGrid, EditTrigger>(
            nameof(EditTrigger),
            EditTrigger.DoubleClick);

    /// <summary>
    /// Defines the <see cref="SelectedIndex"/> styled property.
    /// </summary>
    public static readonly StyledProperty<int> SelectedIndexProperty =
        AvaloniaProperty.Register<EditableDataGrid, int>(nameof(SelectedIndex), -1);

    /// <summary>
    /// Defines the <see cref="AlternatingRowBackground"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> AlternatingRowBackgroundProperty =
        AvaloniaProperty.Register<EditableDataGrid, IBrush?>(nameof(AlternatingRowBackground));

    /// <summary>
    /// Defines the <see cref="RowHoverBackground"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> RowHoverBackgroundProperty =
        AvaloniaProperty.Register<EditableDataGrid, IBrush?>(nameof(RowHoverBackground));

    /// <summary>
    /// Defines the <see cref="EditCellBackground"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> EditCellBackgroundProperty =
        AvaloniaProperty.Register<EditableDataGrid, IBrush?>(nameof(EditCellBackground));

    /// <summary>
    /// Defines the <see cref="GridLineBrush"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> GridLineBrushProperty =
        AvaloniaProperty.Register<EditableDataGrid, IBrush?>(nameof(GridLineBrush));

    /// <summary>
    /// Defines the <see cref="CellPadding"/> styled property.
    /// </summary>
    public static readonly StyledProperty<Thickness> CellPaddingProperty =
        AvaloniaProperty.Register<EditableDataGrid, Thickness>(
            nameof(CellPadding),
            new Thickness(8, 4));

    /// <summary>
    /// Defines the <see cref="RowHeight"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> RowHeightProperty =
        AvaloniaProperty.Register<EditableDataGrid, double>(nameof(RowHeight), 36);

    static EditableDataGrid()
    {
        IsReadOnlyProperty.Changed.AddClassHandler<EditableDataGrid>((x, _) => x.UpdatePseudoClasses());
        ItemsSourceProperty.Changed.AddClassHandler<EditableDataGrid>((x, _) => x.OnItemsSourceChanged());
    }

    /// <summary>
    /// Occurs when a cell value is committed.
    /// </summary>
#pragma warning disable CS0067 // Event is never invoked -- public API for consumers
    public event EventHandler<CellEditCommittedEventArgs>? CellEditCommitted;

    /// <summary>
    /// Occurs when a cell edit is cancelled.
    /// </summary>
    public event EventHandler<CellEditCancelledEventArgs>? CellEditCancelled;

    /// <summary>
    /// Occurs when a cell enters edit mode.
    /// </summary>
    public event EventHandler<CellEditStartingEventArgs>? CellEditStarting;
#pragma warning restore CS0067

    /// <summary>
    /// Gets or sets the data source.
    /// </summary>
    public System.Collections.IEnumerable? ItemsSource
    {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the grid is read-only.
    /// </summary>
    public bool IsReadOnly
    {
        get => GetValue(IsReadOnlyProperty);
        set => SetValue(IsReadOnlyProperty, value);
    }

    /// <summary>
    /// Gets or sets what triggers cell edit mode.
    /// </summary>
    public EditTrigger EditTrigger
    {
        get => GetValue(EditTriggerProperty);
        set => SetValue(EditTriggerProperty, value);
    }

    /// <summary>
    /// Gets or sets the selected row index.
    /// </summary>
    public int SelectedIndex
    {
        get => GetValue(SelectedIndexProperty);
        set => SetValue(SelectedIndexProperty, value);
    }

    /// <summary>
    /// Gets or sets the alternating row background brush.
    /// </summary>
    public IBrush? AlternatingRowBackground
    {
        get => GetValue(AlternatingRowBackgroundProperty);
        set => SetValue(AlternatingRowBackgroundProperty, value);
    }

    /// <summary>
    /// Gets or sets the row hover background brush.
    /// </summary>
    public IBrush? RowHoverBackground
    {
        get => GetValue(RowHoverBackgroundProperty);
        set => SetValue(RowHoverBackgroundProperty, value);
    }

    /// <summary>
    /// Gets or sets the background brush for cells being edited.
    /// </summary>
    public IBrush? EditCellBackground
    {
        get => GetValue(EditCellBackgroundProperty);
        set => SetValue(EditCellBackgroundProperty, value);
    }

    /// <summary>
    /// Gets or sets the grid line brush.
    /// </summary>
    public IBrush? GridLineBrush
    {
        get => GetValue(GridLineBrushProperty);
        set => SetValue(GridLineBrushProperty, value);
    }

    /// <summary>
    /// Gets or sets the padding inside each cell.
    /// </summary>
    public Thickness CellPadding
    {
        get => GetValue(CellPaddingProperty);
        set => SetValue(CellPaddingProperty, value);
    }

    /// <summary>
    /// Gets or sets the row height.
    /// </summary>
    public double RowHeight
    {
        get => GetValue(RowHeightProperty);
        set => SetValue(RowHeightProperty, value);
    }

    /// <summary>
    /// Commits the current cell edit.
    /// </summary>
    public void CommitEdit()
    {
        PseudoClasses.Set(":editing", false);
    }

    /// <summary>
    /// Cancels the current cell edit.
    /// </summary>
    public void CancelEdit()
    {
        PseudoClasses.Set(":editing", false);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        UpdatePseudoClasses();
    }

    private void OnItemsSourceChanged()
    {
        SelectedIndex = -1;
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":readonly", IsReadOnly);
    }
}

/// <summary>
/// Event arguments for cell edit committed events.
/// </summary>
public class CellEditCommittedEventArgs : EventArgs
{
    public int RowIndex { get; }
    public int ColumnIndex { get; }
    public object? OldValue { get; }
    public object? NewValue { get; }

    public CellEditCommittedEventArgs(int rowIndex, int columnIndex, object? oldValue, object? newValue)
    {
        RowIndex = rowIndex;
        ColumnIndex = columnIndex;
        OldValue = oldValue;
        NewValue = newValue;
    }
}

/// <summary>
/// Event arguments for cell edit cancelled events.
/// </summary>
public class CellEditCancelledEventArgs : EventArgs
{
    public int RowIndex { get; }
    public int ColumnIndex { get; }

    public CellEditCancelledEventArgs(int rowIndex, int columnIndex)
    {
        RowIndex = rowIndex;
        ColumnIndex = columnIndex;
    }
}

/// <summary>
/// Event arguments for cell edit starting events.
/// </summary>
public class CellEditStartingEventArgs : EventArgs
{
    public int RowIndex { get; }
    public int ColumnIndex { get; }
    public object? CurrentValue { get; }
    public bool Cancel { get; set; }

    public CellEditStartingEventArgs(int rowIndex, int columnIndex, object? currentValue)
    {
        RowIndex = rowIndex;
        ColumnIndex = columnIndex;
        CurrentValue = currentValue;
    }
}
