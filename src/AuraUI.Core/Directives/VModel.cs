using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;

namespace AuraUI.Core.Directives;

/// <summary>
/// Vue-like v-model directive implemented as attached properties.
/// Provides a shorthand for two-way data binding on common input controls.
/// </summary>
/// <example>
/// <code>
/// // In AXAML:
/// &lt;TextBox Directives:VModel.Value="{Binding UserName, Mode=TwoWay}" /&gt;
/// &lt;CheckBox Directives:VModel.Value="{Binding IsAccepted, Mode=TwoWay}" /&gt;
/// &lt;Slider Directives:VModel.Value="{Binding Volume, Mode=TwoWay}" /&gt;
///
/// // In code-behind:
/// VModel.Bind(myTextBox, viewModel, nameof(viewModel.UserName));
/// </code>
/// </example>
public static class VModel
{
    /// <summary>
    /// The two-way bound value. This is a convenience property that automatically
    /// configures the appropriate binding on the target control type.
    /// </summary>
    public static readonly AttachedProperty<object?> ValueProperty =
        AvaloniaProperty.RegisterAttached<Control, object?>("Value", typeof(VModel));

    /// <summary>
    /// When true, updates the binding source on every keystroke rather than on focus loss.
    /// Only applies to text-based controls.
    /// </summary>
    public static readonly AttachedProperty<bool> UpdateOnInputProperty =
        AvaloniaProperty.RegisterAttached<Control, bool>(
            "UpdateOnInput", typeof(VModel), true);

    /// <summary>
    /// A debounce delay in milliseconds. When set to a value greater than 0,
    /// source updates are delayed by this amount after the last input.
    /// Only applies when <see cref="UpdateOnInputProperty"/> is true.
    /// </summary>
    public static readonly AttachedProperty<int> DebounceProperty =
        AvaloniaProperty.RegisterAttached<Control, int>(
            "Debounce", typeof(VModel), 0);

    static VModel()
    {
        ValueProperty.Changed.AddClassHandler<Control>(OnValueChanged);
    }

    public static object? GetValue(Control element) => element.GetValue(ValueProperty);
    public static void SetValue(Control element, object? value) => element.SetValue(ValueProperty, value);

    public static bool GetUpdateOnInput(Control element) => element.GetValue(UpdateOnInputProperty);
    public static void SetUpdateOnInput(Control element, bool value) => element.SetValue(UpdateOnInputProperty, value);

    public static int GetDebounce(Control element) => element.GetValue(DebounceProperty);
    public static void SetDebounce(Control element, int value) => element.SetValue(DebounceProperty, value);

    /// <summary>
    /// Convenience method to create a two-way binding from code-behind.
    /// </summary>
    /// <param name="control">The target control.</param>
    /// <param name="source">The binding source (typically a ViewModel).</param>
    /// <param name="propertyName">The property name on the source.</param>
    public static void Bind(Control control, object source, string propertyName)
    {
        var binding = new Binding(propertyName)
        {
            Source = source,
            Mode = BindingMode.TwoWay
        };

        var updateOnInput = GetUpdateOnInput(control);
        if (updateOnInput)
        {
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
        }

        // Apply binding to the appropriate property based on control type
        var targetProperty = GetTargetProperty(control);
        if (targetProperty is not null)
        {
            control.Bind(targetProperty, binding);
        }
    }

    private static void OnValueChanged(Control control, AvaloniaPropertyChangedEventArgs e)
    {
        var targetProperty = GetTargetProperty(control);
        if (targetProperty is null) return;

        // Set the value directly on the target property
        control.SetValue(targetProperty, e.NewValue);
    }

    private static AvaloniaProperty? GetTargetProperty(Control control)
    {
        return control switch
        {
            TextBox => TextBox.TextProperty,
            TextBlock => TextBlock.TextProperty,
            NumericUpDown => NumericUpDown.ValueProperty,
            Slider => Slider.ValueProperty,
            CheckBox => CheckBox.IsCheckedProperty,
            RadioButton => RadioButton.IsCheckedProperty,
            ToggleButton => ToggleButton.IsCheckedProperty,
            ComboBox => ComboBox.SelectedIndexProperty,
            DatePicker => DatePicker.SelectedDateProperty,
            TimePicker => TimePicker.SelectedTimeProperty,
            _ => null
        };
    }
}
