using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AuraUI.Core.Helpers;

/// <summary>
/// Attached properties for styling Statistic controls.
/// Usage: aura:StatisticHelper.ValueFontSize="28"
/// </summary>
public static class StatisticHelper
{
    #region ValueFontSize

    public static readonly AttachedProperty<double> ValueFontSizeProperty =
        AvaloniaProperty.RegisterAttached<Control, double>("ValueFontSize", typeof(StatisticHelper), 28);

    public static double GetValueFontSize(Control element) => element.GetValue(ValueFontSizeProperty);
    public static void SetValueFontSize(Control element, double value) => element.SetValue(ValueFontSizeProperty, value);

    #endregion

    #region TitleFontSize

    public static readonly AttachedProperty<double> TitleFontSizeProperty =
        AvaloniaProperty.RegisterAttached<Control, double>("TitleFontSize", typeof(StatisticHelper), 14);

    public static double GetTitleFontSize(Control element) => element.GetValue(TitleFontSizeProperty);
    public static void SetTitleFontSize(Control element, double value) => element.SetValue(TitleFontSizeProperty, value);

    #endregion

    #region ValueColor

    public static readonly AttachedProperty<IBrush?> ValueColorProperty =
        AvaloniaProperty.RegisterAttached<Control, IBrush?>("ValueColor", typeof(StatisticHelper));

    public static IBrush? GetValueColor(Control element) => element.GetValue(ValueColorProperty);
    public static void SetValueColor(Control element, IBrush? value) => element.SetValue(ValueColorProperty, value);

    #endregion

    #region TitleColor

    public static readonly AttachedProperty<IBrush?> TitleColorProperty =
        AvaloniaProperty.RegisterAttached<Control, IBrush?>("TitleColor", typeof(StatisticHelper));

    public static IBrush? GetTitleColor(Control element) => element.GetValue(TitleColorProperty);
    public static void SetTitleColor(Control element, IBrush? value) => element.SetValue(TitleColorProperty, value);

    #endregion

    #region PrefixColor

    public static readonly AttachedProperty<IBrush?> PrefixColorProperty =
        AvaloniaProperty.RegisterAttached<Control, IBrush?>("PrefixColor", typeof(StatisticHelper));

    public static IBrush? GetPrefixColor(Control element) => element.GetValue(PrefixColorProperty);
    public static void SetPrefixColor(Control element, IBrush? value) => element.SetValue(PrefixColorProperty, value);

    #endregion

    #region SuffixColor

    public static readonly AttachedProperty<IBrush?> SuffixColorProperty =
        AvaloniaProperty.RegisterAttached<Control, IBrush?>("SuffixColor", typeof(StatisticHelper));

    public static IBrush? GetSuffixColor(Control element) => element.GetValue(SuffixColorProperty);
    public static void SetSuffixColor(Control element, IBrush? value) => element.SetValue(SuffixColorProperty, value);

    #endregion

    #region ValueFontWeight

    public static readonly AttachedProperty<Avalonia.Media.FontWeight> ValueFontWeightProperty =
        AvaloniaProperty.RegisterAttached<Control, Avalonia.Media.FontWeight>("ValueFontWeight", typeof(StatisticHelper), Avalonia.Media.FontWeight.SemiBold);

    public static Avalonia.Media.FontWeight GetValueFontWeight(Control element) => element.GetValue(ValueFontWeightProperty);
    public static void SetValueFontWeight(Control element, Avalonia.Media.FontWeight value) => element.SetValue(ValueFontWeightProperty, value);

    #endregion

    #region GroupSeparatorColor

    public static readonly AttachedProperty<IBrush?> GroupSeparatorColorProperty =
        AvaloniaProperty.RegisterAttached<Control, IBrush?>("GroupSeparatorColor", typeof(StatisticHelper));

    public static IBrush? GetGroupSeparatorColor(Control element) => element.GetValue(GroupSeparatorColorProperty);
    public static void SetGroupSeparatorColor(Control element, IBrush? value) => element.SetValue(GroupSeparatorColorProperty, value);

    #endregion
}
