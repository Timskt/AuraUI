using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;

namespace AuraUI.Core.MVVM;

/// <summary>
/// React-like render prop control that delegates its visual representation to a function.
/// Instead of defining a fixed template, the control's content is produced by
/// <see cref="RenderContent"/>, which receives the current <see cref="Data"/> value
/// and returns an object (typically a <see cref="Control"/>) to display.
/// </summary>
/// <typeparam name="T">The type of data passed to the render function.</typeparam>
/// <example>
/// <code>
/// // In AXAML (with code-behind or compiled bindings):
/// &lt;m:RenderPropControl x:TypeArguments="x:String"
///     Data="{Binding UserName}"
///     RenderContent="{Binding RenderGreeting}" /&gt;
///
/// // In ViewModel:
/// public object RenderGreeting(string name)
/// {
///     return new TextBlock { Text = $"Hello, {name}!", FontSize = 24 };
/// }
///
/// // In code-behind:
/// var control = new RenderPropControl&lt;ItemViewModel&gt;
/// {
///     Data = currentItem,
///     RenderContent = item =&gt; new StackPanel
///     {
///         Children =
///         {
///             new TextBlock { Text = item.Title },
///             new TextBlock { Text = item.Description }
///         }
///     }
/// };
/// </code>
/// </example>
public class RenderPropControl<T> : ContentControl
{
    /// <summary>
    /// Defines the <see cref="Data"/> property.
    /// </summary>
    public static readonly StyledProperty<T?> DataProperty =
        AvaloniaProperty.Register<RenderPropControl<T>, T?>(nameof(Data));

    /// <summary>
    /// Defines the <see cref="RenderContent"/> property.
    /// </summary>
    public static readonly StyledProperty<Func<T, object?>?> RenderContentProperty =
        AvaloniaProperty.Register<RenderPropControl<T>, Func<T, object?>?>(nameof(RenderContent));

    /// <summary>
    /// Defines the <see cref="EmptyContent"/> property.
    /// </summary>
    public static readonly StyledProperty<object?> EmptyContentProperty =
        AvaloniaProperty.Register<RenderPropControl<T>, object?>(nameof(EmptyContent));

    /// <summary>
    /// Gets or sets the data to pass to the render function.
    /// When this changes, the control re-renders.
    /// </summary>
    public T? Data
    {
        get => GetValue(DataProperty);
        set => SetValue(DataProperty, value);
    }

    /// <summary>
    /// Gets or sets the render function that produces content from data.
    /// The function receives the current <see cref="Data"/> and should return
    /// a <see cref="Control"/>, a string, or any object suitable for
    /// <see cref="ContentControl.Content"/>.
    /// </summary>
    public Func<T, object?>? RenderContent
    {
        get => GetValue(RenderContentProperty);
        set => SetValue(RenderContentProperty, value);
    }

    /// <summary>
    /// Gets or sets the content to display when <see cref="Data"/> is null or default
    /// and no render function is set.
    /// </summary>
    public object? EmptyContent
    {
        get => GetValue(EmptyContentProperty);
        set => SetValue(EmptyContentProperty, value);
    }

    static RenderPropControl()
    {
        DataProperty.Changed.AddClassHandler<RenderPropControl<T>>(OnDataChanged);
        RenderContentProperty.Changed.AddClassHandler<RenderPropControl<T>>(OnRenderContentChanged);
    }

    private static void OnDataChanged(RenderPropControl<T> control, AvaloniaPropertyChangedEventArgs e)
    {
        control.ReRender();
    }

    private static void OnRenderContentChanged(RenderPropControl<T> control, AvaloniaPropertyChangedEventArgs e)
    {
        control.ReRender();
    }

    private void ReRender()
    {
        var renderFunc = RenderContent;
        var data = Data;

        if (renderFunc is not null && data is not null)
        {
            Content = renderFunc(data);
        }
        else
        {
            Content = EmptyContent;
        }
    }
}

/// <summary>
/// Non-generic base for render prop controls that can be used in XAML
/// without specifying a type parameter.
/// </summary>
public class RenderPropControl : ContentControl
{
    /// <summary>
    /// Defines the <see cref="Data"/> property.
    /// </summary>
    public static readonly StyledProperty<object?> DataProperty =
        AvaloniaProperty.Register<RenderPropControl, object?>(nameof(Data));

    /// <summary>
    /// Defines the <see cref="RenderContent"/> property.
    /// </summary>
    public static readonly StyledProperty<Func<object, object?>?> RenderContentProperty =
        AvaloniaProperty.Register<RenderPropControl, Func<object, object?>?>(nameof(RenderContent));

    /// <summary>
    /// Gets or sets the data to pass to the render function.
    /// </summary>
    public object? Data
    {
        get => GetValue(DataProperty);
        set => SetValue(DataProperty, value);
    }

    /// <summary>
    /// Gets or sets the render function that produces content from data.
    /// </summary>
    public Func<object, object?>? RenderContent
    {
        get => GetValue(RenderContentProperty);
        set => SetValue(RenderContentProperty, value);
    }

    static RenderPropControl()
    {
        DataProperty.Changed.AddClassHandler<RenderPropControl>(OnDataChanged);
        RenderContentProperty.Changed.AddClassHandler<RenderPropControl>(OnRenderContentChanged);
    }

    private static void OnDataChanged(RenderPropControl control, AvaloniaPropertyChangedEventArgs e)
    {
        control.ReRender();
    }

    private static void OnRenderContentChanged(RenderPropControl control, AvaloniaPropertyChangedEventArgs e)
    {
        control.ReRender();
    }

    private void ReRender()
    {
        var renderFunc = RenderContent;
        var data = Data;

        if (renderFunc is not null && data is not null)
        {
            Content = renderFunc(data);
        }
    }
}
