using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AuraUI.Core.Helpers;

/// <summary>
/// Attached properties for styling DataGrid controls.
/// Usage: aura:DataGridHelper.AlternatingRowBackground="#F9F9F9"
/// </summary>
public static class DataGridHelper
{
    #region AlternatingRowBackground

    public static readonly AttachedProperty<IBrush?> AlternatingRowBackgroundProperty =
        AvaloniaProperty.RegisterAttached<DataGrid, IBrush?>("AlternatingRowBackground", typeof(DataGridHelper));

    public static IBrush? GetAlternatingRowBackground(DataGrid element) => element.GetValue(AlternatingRowBackgroundProperty);
    public static void SetAlternatingRowBackground(DataGrid element, IBrush? value) => element.SetValue(AlternatingRowBackgroundProperty, value);

    #endregion

    #region RowHoverBackground

    public static readonly AttachedProperty<IBrush?> RowHoverBackgroundProperty =
        AvaloniaProperty.RegisterAttached<DataGrid, IBrush?>("RowHoverBackground", typeof(DataGridHelper));

    public static IBrush? GetRowHoverBackground(DataGrid element) => element.GetValue(RowHoverBackgroundProperty);
    public static void SetRowHoverBackground(DataGrid element, IBrush? value) => element.SetValue(RowHoverBackgroundProperty, value);

    #endregion

    #region RowSelectedBackground

    public static readonly AttachedProperty<IBrush?> RowSelectedBackgroundProperty =
        AvaloniaProperty.RegisterAttached<DataGrid, IBrush?>("RowSelectedBackground", typeof(DataGridHelper));

    public static IBrush? GetRowSelectedBackground(DataGrid element) => element.GetValue(RowSelectedBackgroundProperty);
    public static void SetRowSelectedBackground(DataGrid element, IBrush? value) => element.SetValue(RowSelectedBackgroundProperty, value);

    #endregion

    #region RowSelectedForeground

    public static readonly AttachedProperty<IBrush?> RowSelectedForegroundProperty =
        AvaloniaProperty.RegisterAttached<DataGrid, IBrush?>("RowSelectedForeground", typeof(DataGridHelper));

    public static IBrush? GetRowSelectedForeground(DataGrid element) => element.GetValue(RowSelectedForegroundProperty);
    public static void SetRowSelectedForeground(DataGrid element, IBrush? value) => element.SetValue(RowSelectedForegroundProperty, value);

    #endregion

    #region HeaderBackground

    public static readonly AttachedProperty<IBrush?> HeaderBackgroundProperty =
        AvaloniaProperty.RegisterAttached<DataGrid, IBrush?>("HeaderBackground", typeof(DataGridHelper));

    public static IBrush? GetHeaderBackground(DataGrid element) => element.GetValue(HeaderBackgroundProperty);
    public static void SetHeaderBackground(DataGrid element, IBrush? value) => element.SetValue(HeaderBackgroundProperty, value);

    #endregion

    #region HeaderForeground

    public static readonly AttachedProperty<IBrush?> HeaderForegroundProperty =
        AvaloniaProperty.RegisterAttached<DataGrid, IBrush?>("HeaderForeground", typeof(DataGridHelper));

    public static IBrush? GetHeaderForeground(DataGrid element) => element.GetValue(HeaderForegroundProperty);
    public static void SetHeaderForeground(DataGrid element, IBrush? value) => element.SetValue(HeaderForegroundProperty, value);

    #endregion

    #region HeaderFontWeight

    public static readonly AttachedProperty<Avalonia.Media.FontWeight> HeaderFontWeightProperty =
        AvaloniaProperty.RegisterAttached<DataGrid, Avalonia.Media.FontWeight>("HeaderFontWeight", typeof(DataGridHelper), Avalonia.Media.FontWeight.SemiBold);

    public static Avalonia.Media.FontWeight GetHeaderFontWeight(DataGrid element) => element.GetValue(HeaderFontWeightProperty);
    public static void SetHeaderFontWeight(DataGrid element, Avalonia.Media.FontWeight value) => element.SetValue(HeaderFontWeightProperty, value);

    #endregion

    #region GridLineBrush

    public static readonly AttachedProperty<IBrush?> GridLineBrushProperty =
        AvaloniaProperty.RegisterAttached<DataGrid, IBrush?>("GridLineBrush", typeof(DataGridHelper));

    public static IBrush? GetGridLineBrush(DataGrid element) => element.GetValue(GridLineBrushProperty);
    public static void SetGridLineBrush(DataGrid element, IBrush? value) => element.SetValue(GridLineBrushProperty, value);

    #endregion

    #region RowHeight

    public static readonly AttachedProperty<double> RowHeightProperty =
        AvaloniaProperty.RegisterAttached<DataGrid, double>("RowHeight", typeof(DataGridHelper), 40);

    public static double GetRowHeight(DataGrid element) => element.GetValue(RowHeightProperty);
    public static void SetRowHeight(DataGrid element, double value) => element.SetValue(RowHeightProperty, value);

    #endregion

    #region CornerRadius

    public static readonly AttachedProperty<CornerRadius> CornerRadiusProperty =
        AvaloniaProperty.RegisterAttached<DataGrid, CornerRadius>("CornerRadius", typeof(DataGridHelper));

    public static CornerRadius GetCornerRadius(DataGrid element) => element.GetValue(CornerRadiusProperty);
    public static void SetCornerRadius(DataGrid element, CornerRadius value) => element.SetValue(CornerRadiusProperty, value);

    #endregion

    #region ShowVerticalLines

    public static readonly AttachedProperty<bool> ShowVerticalLinesProperty =
        AvaloniaProperty.RegisterAttached<DataGrid, bool>("ShowVerticalLines", typeof(DataGridHelper));

    public static bool GetShowVerticalLines(DataGrid element) => element.GetValue(ShowVerticalLinesProperty);
    public static void SetShowVerticalLines(DataGrid element, bool value) => element.SetValue(ShowVerticalLinesProperty, value);

    #endregion

    #region ShowHorizontalLines

    public static readonly AttachedProperty<bool> ShowHorizontalLinesProperty =
        AvaloniaProperty.RegisterAttached<DataGrid, bool>("ShowHorizontalLines", typeof(DataGridHelper), true);

    public static bool GetShowHorizontalLines(DataGrid element) => element.GetValue(ShowHorizontalLinesProperty);
    public static void SetShowHorizontalLines(DataGrid element, bool value) => element.SetValue(ShowHorizontalLinesProperty, value);

    #endregion

    #region HeaderHeight

    public static readonly AttachedProperty<double> HeaderHeightProperty =
        AvaloniaProperty.RegisterAttached<DataGrid, double>("HeaderHeight", typeof(DataGridHelper), 40);

    public static double GetHeaderHeight(DataGrid element) => element.GetValue(HeaderHeightProperty);
    public static void SetHeaderHeight(DataGrid element, double value) => element.SetValue(HeaderHeightProperty, value);

    #endregion
}
