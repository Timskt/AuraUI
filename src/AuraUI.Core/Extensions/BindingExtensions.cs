using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;

namespace AuraUI.Core.Extensions;

/// <summary>
/// Fluent extension methods for creating data bindings on Avalonia controls.
/// </summary>
public static class BindingExtensions
{
    /// <summary>
    /// Creates a one-way binding from <paramref name="control"/> to
    /// <paramref name="source"/> along the given property <paramref name="path"/>.
    /// Returns the control for chaining.
    /// </summary>
    /// <typeparam name="T">The control type.</typeparam>
    /// <param name="control">The target control.</param>
    /// <param name="property">The Avalonia property to bind.</param>
    /// <param name="source">The binding source (typically a ViewModel).</param>
    /// <param name="path">The property path on the source (e.g. <c>"UserName"</c>).</param>
    public static T Bind<T>(this T control, AvaloniaProperty property, object source, string path) where T : Control
    {
        control[property] = new Binding
        {
            Source = source,
            Path = path,
            Mode = BindingMode.OneWay
        };
        return control;
    }

    /// <summary>
    /// Creates a two-way binding from <paramref name="control"/> to
    /// <paramref name="source"/> along the given property <paramref name="path"/>.
    /// Returns the control for chaining.
    /// </summary>
    /// <typeparam name="T">The control type.</typeparam>
    /// <param name="control">The target control.</param>
    /// <param name="property">The Avalonia property to bind.</param>
    /// <param name="source">The binding source (typically a ViewModel).</param>
    /// <param name="path">The property path on the source.</param>
    public static T BindTwoWay<T>(this T control, AvaloniaProperty property, object source, string path) where T : Control
    {
        control[property] = new Binding
        {
            Source = source,
            Path = path,
            Mode = BindingMode.TwoWay
        };
        return control;
    }

    /// <summary>
    /// Binds an <see cref="ICommand"/> to a control. For <see cref="Button"/>
    /// this binds <see cref="Button.Command"/>. Returns the control for chaining.
    /// </summary>
    /// <typeparam name="T">The control type.</typeparam>
    /// <param name="control">The target control.</param>
    /// <param name="command">The command to bind.</param>
    /// <param name="parameter">An optional command parameter.</param>
    public static T BindCommand<T>(this T control, ICommand command, object? parameter = null) where T : Control
    {
        if (control is Avalonia.Controls.Button button)
        {
            button.Command = command;
            button.CommandParameter = parameter;
        }

        return control;
    }
}
