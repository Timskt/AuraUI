using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AuraUI.Core.Helpers;

/// <summary>
/// Attached properties for styling TextBlock elements.
/// Usage: aura:TextBlockHelper.Foreground="#666666"
/// </summary>
public static class TextBlockHelper
{
    #region IsTrimmed

    public static readonly AttachedProperty<bool> IsTrimmedProperty =
        AvaloniaProperty.RegisterAttached<TextBlock, bool>("IsTrimmed", typeof(TextBlockHelper));

    public static bool GetIsTrimmed(TextBlock element) => element.GetValue(IsTrimmedProperty);
    public static void SetIsTrimmed(TextBlock element, bool value) => element.SetValue(IsTrimmedProperty, value);

    #endregion

    #region MaxLines

    public static readonly AttachedProperty<int> MaxLinesProperty =
        AvaloniaProperty.RegisterAttached<TextBlock, int>("MaxLines", typeof(TextBlockHelper));

    public static int GetMaxLines(TextBlock element) => element.GetValue(MaxLinesProperty);
    public static void SetMaxLines(TextBlock element, int value) => element.SetValue(MaxLinesProperty, value);

    #endregion

    #region IsSelectable

    public static readonly AttachedProperty<bool> IsSelectableProperty =
        AvaloniaProperty.RegisterAttached<TextBlock, bool>("IsSelectable", typeof(TextBlockHelper));

    public static bool GetIsSelectable(TextBlock element) => element.GetValue(IsSelectableProperty);
    public static void SetIsSelectable(TextBlock element, bool value) => element.SetValue(IsSelectableProperty, value);

    #endregion

    #region TextDecorations

    public static readonly AttachedProperty<TextDecorationCollection?> TextDecorationsProperty =
        AvaloniaProperty.RegisterAttached<TextBlock, TextDecorationCollection?>("TextDecorations", typeof(TextBlockHelper));

    public static TextDecorationCollection? GetTextDecorations(TextBlock element) => element.GetValue(TextDecorationsProperty);
    public static void SetTextDecorations(TextBlock element, TextDecorationCollection? value) => element.SetValue(TextDecorationsProperty, value);

    #endregion

    #region HoverForeground

    public static readonly AttachedProperty<IBrush?> HoverForegroundProperty =
        AvaloniaProperty.RegisterAttached<TextBlock, IBrush?>("HoverForeground", typeof(TextBlockHelper));

    public static IBrush? GetHoverForeground(TextBlock element) => element.GetValue(HoverForegroundProperty);
    public static void SetHoverForeground(TextBlock element, IBrush? value) => element.SetValue(HoverForegroundProperty, value);

    #endregion
}
