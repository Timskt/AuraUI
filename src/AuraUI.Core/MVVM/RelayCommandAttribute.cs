namespace AuraUI.Core.MVVM;

/// <summary>
/// Marks a method for automatic <see cref="RelayCommand"/> generation.
/// When a source generator is available, a method such as
/// <c>[RelayCommand] private void Save() { ... }</c> will generate
/// a public <c>SaveCommand</c> property of type <see cref="RelayCommand"/>.
/// <para>
/// <b>NOTE:</b> This is currently a <em>marker attribute</em> only.
/// Actual code generation requires a Roslyn source generator.
/// In the interim, create commands manually with <see cref="RelayCommand"/>.
/// </para>
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public sealed class RelayCommandAttribute : Attribute
{
    /// <summary>
    /// Gets or sets the name of a <see cref="bool"/> property or method
    /// that determines whether the command can execute.
    /// </summary>
    public string? CanExecute { get; set; }

    /// <summary>
    /// Gets or sets whether the generated command should be an
    /// <see cref="AsyncRelayCommand"/> instead of a <see cref="RelayCommand"/>.
    /// </summary>
    public bool IsAsync { get; set; }
}
