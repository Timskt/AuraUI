using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AuraUI.Core.Helpers;

/// <summary>
/// Attached properties for styling Result controls.
/// Usage: aura:ResultHelper.IconSize="64"
/// </summary>
public static class ResultHelper
{
    #region IconSize

    public static readonly AttachedProperty<double> IconSizeProperty =
        AvaloniaProperty.RegisterAttached<Control, double>("IconSize", typeof(ResultHelper), 64);

    public static double GetIconSize(Control element) => element.GetValue(IconSizeProperty);
    public static void SetIconSize(Control element, double value) => element.SetValue(IconSizeProperty, value);

    #endregion

    #region TitleFontSize

    public static readonly AttachedProperty<double> TitleFontSizeProperty =
        AvaloniaProperty.RegisterAttached<Control, double>("TitleFontSize", typeof(ResultHelper), 24);

    public static double GetTitleFontSize(Control element) => element.GetValue(TitleFontSizeProperty);
    public static void SetTitleFontSize(Control element, double value) => element.SetValue(TitleFontSizeProperty, value);

    #endregion

    #region SubTitleFontSize

    public static readonly AttachedProperty<double> SubTitleFontSizeProperty =
        AvaloniaProperty.RegisterAttached<Control, double>("SubTitleFontSize", typeof(ResultHelper), 14);

    public static double GetSubTitleFontSize(Control element) => element.GetValue(SubTitleFontSizeProperty);
    public static void SetSubTitleFontSize(Control element, double value) => element.SetValue(SubTitleFontSizeProperty, value);

    #endregion

    #region TitleColor

    public static readonly AttachedProperty<IBrush?> TitleColorProperty =
        AvaloniaProperty.RegisterAttached<Control, IBrush?>("TitleColor", typeof(ResultHelper));

    public static IBrush? GetTitleColor(Control element) => element.GetValue(TitleColorProperty);
    public static void SetTitleColor(Control element, IBrush? value) => element.SetValue(TitleColorProperty, value);

    #endregion

    #region SubTitleColor

    public static readonly AttachedProperty<IBrush?> SubTitleColorProperty =
        AvaloniaProperty.RegisterAttached<Control, IBrush?>("SubTitleColor", typeof(ResultHelper));

    public static IBrush? GetSubTitleColor(Control element) => element.GetValue(SubTitleColorProperty);
    public static void SetSubTitleColor(Control element, IBrush? value) => element.SetValue(SubTitleColorProperty, value);

    #endregion

    #region IconColor

    public static readonly AttachedProperty<IBrush?> IconColorProperty =
        AvaloniaProperty.RegisterAttached<Control, IBrush?>("IconColor", typeof(ResultHelper));

    public static IBrush? GetIconColor(Control element) => element.GetValue(IconColorProperty);
    public static void SetIconColor(Control element, IBrush? value) => element.SetValue(IconColorProperty, value);

    #endregion

    #region TitleFontWeight

    public static readonly AttachedProperty<Avalonia.Media.FontWeight> TitleFontWeightProperty =
        AvaloniaProperty.RegisterAttached<Control, Avalonia.Media.FontWeight>("TitleFontWeight", typeof(ResultHelper), Avalonia.Media.FontWeight.SemiBold);

    public static Avalonia.Media.FontWeight GetTitleFontWeight(Control element) => element.GetValue(TitleFontWeightProperty);
    public static void SetTitleFontWeight(Control element, Avalonia.Media.FontWeight value) => element.SetValue(TitleFontWeightProperty, value);

    #endregion
}
