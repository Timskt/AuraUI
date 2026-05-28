using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace AuraUI.Core.Extensions;

/// <summary>
/// Extension methods for Avalonia controls.
/// </summary>
public static class ControlExtensions
{
    /// <summary>
    /// Set the IsVisible property.
    /// </summary>
    public static T Show<T>(this T control) where T : Control
    {
        control.IsVisible = true;
        return control;
    }

    /// <summary>
    /// Set the IsVisible property to false.
    /// </summary>
    public static T Hide<T>(this T control) where T : Control
    {
        control.IsVisible = false;
        return control;
    }

    /// <summary>
    /// Toggle the IsVisible property.
    /// </summary>
    public static T ToggleVisibility<T>(this T control) where T : Control
    {
        control.IsVisible = !control.IsVisible;
        return control;
    }

    /// <summary>
    /// Set the IsEnabled property.
    /// </summary>
    public static T Enable<T>(this T control) where T : Control
    {
        control.IsEnabled = true;
        return control;
    }

    /// <summary>
    /// Set the IsEnabled property to false.
    /// </summary>
    public static T Disable<T>(this T control) where T : Control
    {
        control.IsEnabled = false;
        return control;
    }

    /// <summary>
    /// Add a CSS-like class to the control.
    /// </summary>
    public static T WithClass<T>(this T control, string className) where T : Control
    {
        control.Classes.Add(className);
        return control;
    }

    /// <summary>
    /// Remove a CSS-like class from the control.
    /// </summary>
    public static T WithoutClass<T>(this T control, string className) where T : Control
    {
        control.Classes.Remove(className);
        return control;
    }

    /// <summary>
    /// Toggle a CSS-like class on the control.
    /// </summary>
    public static T ToggleClass<T>(this T control, string className) where T : Control
    {
        if (control.Classes.Contains(className))
            control.Classes.Remove(className);
        else
            control.Classes.Add(className);
        return control;
    }

    /// <summary>
    /// Set the ToolTip.
    /// </summary>
    public static T WithToolTip<T>(this T control, string? tip) where T : Control
    {
        ToolTip.SetTip(control, tip);
        return control;
    }

    /// <summary>
    /// Set the Margin.
    /// </summary>
    public static T WithMargin<T>(this T control, Thickness margin) where T : Control
    {
        control.Margin = margin;
        return control;
    }

    /// <summary>
    /// Set the Margin with uniform value.
    /// </summary>
    public static T WithMargin<T>(this T control, double uniform) where T : Control
    {
        control.Margin = new Thickness(uniform);
        return control;
    }

    /// <summary>
    /// Set the Margin with horizontal and vertical values.
    /// </summary>
    public static T WithMargin<T>(this T control, double horizontal, double vertical) where T : Control
    {
        control.Margin = new Thickness(horizontal, vertical);
        return control;
    }

    /// <summary>
    /// Set the Padding (for controls that support it).
    /// </summary>
    public static T WithPadding<T>(this T control, Thickness padding) where T : Control
    {
        if (control is Border border)
            border.Padding = padding;
        else if (control is ContentControl cc)
            cc.Padding = padding;
        return control;
    }

    /// <summary>
    /// Set the Width and Height.
    /// </summary>
    public static T WithSize<T>(this T control, double width, double height) where T : Control
    {
        control.Width = width;
        control.Height = height;
        return control;
    }

    /// <summary>
    /// Set the MinWidth and MinHeight.
    /// </summary>
    public static T WithMinSize<T>(this T control, double width, double height) where T : Control
    {
        control.MinWidth = width;
        control.MinHeight = height;
        return control;
    }

    /// <summary>
    /// Set the MaxWidth and MaxHeight.
    /// </summary>
    public static T WithMaxSize<T>(this T control, double width, double height) where T : Control
    {
        control.MaxWidth = width;
        control.MaxHeight = height;
        return control;
    }

    /// <summary>
    /// Set the HorizontalAlignment and VerticalAlignment.
    /// </summary>
    public static T WithAlignment<T>(this T control, HorizontalAlignment horizontal, VerticalAlignment vertical) where T : Control
    {
        control.HorizontalAlignment = horizontal;
        control.VerticalAlignment = vertical;
        return control;
    }

    /// <summary>
    /// Set the HorizontalAlignment to Stretch.
    /// </summary>
    public static T Stretch<T>(this T control) where T : Control
    {
        control.HorizontalAlignment = HorizontalAlignment.Stretch;
        return control;
    }

    /// <summary>
    /// Set the HorizontalAlignment to Center.
    /// </summary>
    public static T Center<T>(this T control) where T : Control
    {
        control.HorizontalAlignment = HorizontalAlignment.Center;
        return control;
    }
}
