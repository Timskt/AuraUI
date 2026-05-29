namespace AuraUI.Core.MVVM;

/// <summary>
/// Marks a field for automatic observable property generation.
/// When a source generator is available, a field such as
/// <c>[ObservableProperty] private string _name;</c> will generate
/// a public <c>Name</c> property that raises <c>PropertyChanged</c>.
/// <para>
/// <b>NOTE:</b> This is currently a <em>marker attribute</em> only.
/// Actual code generation requires a Roslyn source generator.
/// In the interim, use <see cref="ViewModelBase.SetProperty{T}(ref T, T, string?)"/>
/// for manual notification.
/// </para>
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public sealed class ObservablePropertyAttribute : Attribute
{
}
