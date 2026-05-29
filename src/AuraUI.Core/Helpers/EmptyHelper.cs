using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AuraUI.Core.Helpers;

/// <summary>
/// Attached properties for styling Empty state controls.
/// Usage: aura:EmptyHelper.IconSize="64"
/// </summary>
public static class EmptyHelper
{
    #region IconSize

    public static readonly AttachedProperty<double> IconSizeProperty =
        AvaloniaProperty.RegisterAttached<Control, double>("IconSize", typeof(EmptyHelper), 64);

    public static double GetIconSize(Control element) => element.GetValue(IconSizeProperty);
    public static void SetIconSize(Control element, double value) => element.SetValue(IconSizeProperty, value);

    #endregion

    #region DescriptionColor

    public static readonly AttachedProperty<IBrush?> DescriptionColorProperty =
        AvaloniaProperty.RegisterAttached<Control, IBrush?>("DescriptionColor", typeof(EmptyHelper));

    public static IBrush? GetDescriptionColor(Control element) => element.GetValue(DescriptionColorProperty);
    public static void SetDescriptionColor(Control element, IBrush? value) => element.SetValue(DescriptionColorProperty, value);

    #endregion

    #region TitleColor

    public static readonly AttachedProperty<IBrush?> TitleColorProperty =
        AvaloniaProperty.RegisterAttached<Control, IBrush?>("TitleColor", typeof(EmptyHelper));

    public static IBrush? GetTitleColor(Control element) => element.GetValue(TitleColorProperty);
    public static void SetTitleColor(Control element, IBrush? value) => element.SetValue(TitleColorProperty, value);

    #endregion

    #region IconColor

    public static readonly AttachedProperty<IBrush?> IconColorProperty =
        AvaloniaProperty.RegisterAttached<Control, IBrush?>("IconColor", typeof(EmptyHelper));

    public static IBrush? GetIconColor(Control element) => element.GetValue(IconColorProperty);
    public static void SetIconColor(Control element, IBrush? value) => element.SetValue(IconColorProperty, value);

    #endregion

    #region TitleFontSize

    public static readonly AttachedProperty<double> TitleFontSizeProperty =
        AvaloniaProperty.RegisterAttached<Control, double>("TitleFontSize", typeof(EmptyHelper), 16);

    public static double GetTitleFontSize(Control element) => element.GetValue(TitleFontSizeProperty);
    public static void SetTitleFontSize(Control element, double value) => element.SetValue(TitleFontSizeProperty, value);

    #endregion

    #region DescriptionFontSize

    public static readonly AttachedProperty<double> DescriptionFontSizeProperty =
        AvaloniaProperty.RegisterAttached<Control, double>("DescriptionFontSize", typeof(EmptyHelper), 14);

    public static double GetDescriptionFontSize(Control element) => element.GetValue(DescriptionFontSizeProperty);
    public static void SetDescriptionFontSize(Control element, double value) => element.SetValue(DescriptionFontSizeProperty, value);

    #endregion
}
