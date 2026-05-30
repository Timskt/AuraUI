using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace AuraUI.Core.Validation;

/// <summary>
/// Tracks the complete state of a form including dirty/valid/submitting status,
/// per-property errors, touched fields, and supports async submission.
/// </summary>
/// <typeparam name="TModel">The model type the form operates on.</typeparam>
public class FormState<TModel> : INotifyPropertyChanged where TModel : class
{
    private readonly FormValidator _validator;
    private readonly Func<TModel, CancellationToken, Task<bool>>? _submitHandler;
    private readonly Dictionary<string, IList<string>> _errors = new();
    private readonly Dictionary<string, bool> _touched = new();
    private readonly HashSet<string> _dirtyProperties = new();
    private bool _isSubmitting;
    private bool _isValid = true;

    /// <summary>
    /// Occurs when any property on this instance changes.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Occurs when the form state changes (dirty, valid, touched, etc.).
    /// </summary>
    public event EventHandler<FormStateChangedEventArgs>? StateChanged;

    /// <summary>
    /// Gets the model instance this form state manages.
    /// </summary>
    public TModel Model { get; }

    /// <summary>
    /// Gets the underlying <see cref="FormValidator"/> used for validation.
    /// </summary>
    public FormValidator Validator => _validator;

    /// <summary>
    /// Gets whether any property has been modified since the last reset.
    /// </summary>
    public bool IsDirty => _dirtyProperties.Count > 0;

    /// <summary>
    /// Gets whether no property has been modified (the inverse of <see cref="IsDirty"/>).
    /// </summary>
    public bool IsPristine => !IsDirty;

    /// <summary>
    /// Gets whether the form is currently valid (no errors).
    /// </summary>
    public bool IsValid
    {
        get => _isValid;
        private set => SetProperty(ref _isValid, value);
    }

    /// <summary>
    /// Gets whether the form is currently being submitted.
    /// </summary>
    public bool IsSubmitting
    {
        get => _isSubmitting;
        private set => SetProperty(ref _isSubmitting, value);
    }

    /// <summary>
    /// Gets a read-only view of current validation errors, keyed by property name.
    /// </summary>
    public IReadOnlyDictionary<string, IList<string>> Errors => _errors;

    /// <summary>
    /// Gets a read-only view of which properties have been interacted with.
    /// </summary>
    public IReadOnlyDictionary<string, bool> Touched => _touched;

    /// <summary>
    /// Gets whether all required properties have been touched.
    /// </summary>
    public bool AllTouched { get; private set; }

    /// <summary>
    /// Initializes a new <see cref="FormState{TModel}"/> with a model instance.
    /// </summary>
    /// <param name="model">The model instance to manage.</param>
    /// <param name="validator">Optional validator. If null, a new one is created.</param>
    /// <param name="submitHandler">Optional async submit handler. Returns true on success.</param>
    public FormState(
        TModel model,
        FormValidator? validator = null,
        Func<TModel, CancellationToken, Task<bool>>? submitHandler = null)
    {
        Model = model ?? throw new ArgumentNullException(nameof(model));
        _validator = validator ?? new FormValidator();
        _submitHandler = submitHandler;

        // Subscribe to model property changes to track dirty state
        if (model is INotifyPropertyChanged notifyModel)
        {
            notifyModel.PropertyChanged += (_, e) =>
            {
                if (!string.IsNullOrEmpty(e.PropertyName))
                    MarkAsDirty(e.PropertyName);
            };
        }
    }

    /// <summary>
    /// Marks a property as dirty (modified).
    /// </summary>
    /// <param name="propertyName">The property name.</param>
    public void MarkAsDirty(string propertyName)
    {
        ArgumentException.ThrowIfNullOrEmpty(propertyName);

        if (_dirtyProperties.Add(propertyName))
        {
            OnPropertyChanged(nameof(IsDirty));
            OnPropertyChanged(nameof(IsPristine));
            RaiseStateChanged();
        }
    }

    /// <summary>
    /// Marks a property as touched (interacted with by the user).
    /// </summary>
    /// <param name="propertyName">The property name.</param>
    public void MarkAsTouched(string propertyName)
    {
        ArgumentException.ThrowIfNullOrEmpty(propertyName);

        _touched[propertyName] = true;
        OnPropertyChanged(nameof(Touched));
        RaiseStateChanged();
    }

    /// <summary>
    /// Resets the form to its initial state: clears dirty flags, touched state, and errors.
    /// Does not modify the model values.
    /// </summary>
    public void Reset()
    {
        _dirtyProperties.Clear();
        _touched.Clear();
        _errors.Clear();
        IsValid = true;
        _validator.ClearErrors();

        OnPropertyChanged(nameof(IsDirty));
        OnPropertyChanged(nameof(IsPristine));
        OnPropertyChanged(nameof(Errors));
        OnPropertyChanged(nameof(Touched));
        RaiseStateChanged();
    }

    /// <summary>
    /// Validates the entire model and updates error state.
    /// </summary>
    public void Validate()
    {
        var result = _validator.ValidateAll(Model);

        // Sync errors from validator into our dictionary
        _errors.Clear();
        foreach (var error in _validator.GetErrors())
        {
            if (!_errors.TryGetValue(error.PropertyName, out var list))
            {
                list = new List<string>();
                _errors[error.PropertyName] = list;
            }
            list.Add(error.ErrorMessage);
        }

        IsValid = result;
        OnPropertyChanged(nameof(IsValid));
        OnPropertyChanged(nameof(Errors));
        RaiseStateChanged();
    }

    /// <summary>
    /// Validates a single property and updates its error state.
    /// </summary>
    /// <param name="propertyName">The property to validate.</param>
    public void ValidateProperty(string propertyName)
    {
        ArgumentException.ThrowIfNullOrEmpty(propertyName);

        var prop = typeof(TModel).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        if (prop is null) return;

        var value = prop.GetValue(Model);
        _validator.ValidateProperty(propertyName, value);

        // Update errors dictionary for this property
        _errors.Remove(propertyName);
        var propertyErrors = _validator.GetErrors(propertyName);
        if (propertyErrors.Count > 0)
        {
            _errors[propertyName] = propertyErrors.Select(e => e.ErrorMessage).ToList();
        }

        IsValid = !_validator.HasErrors;
        OnPropertyChanged(nameof(IsValid));
        OnPropertyChanged(nameof(Errors));
        RaiseStateChanged();
    }

    /// <summary>
    /// Submits the form asynchronously. Validates first, then invokes the submit handler.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the submission succeeded, false otherwise.</returns>
    public async Task<bool> SubmitAsync(CancellationToken cancellationToken = default)
    {
        if (_submitHandler is null)
            throw new InvalidOperationException("No submit handler was configured. Pass a submit handler to the FormState constructor.");

        // Validate before submitting
        Validate();
        if (!IsValid)
            return false;

        IsSubmitting = true;
        OnPropertyChanged(nameof(IsSubmitting));
        RaiseStateChanged();

        try
        {
            var success = await _submitHandler(Model, cancellationToken);
            return success;
        }
        finally
        {
            IsSubmitting = false;
            OnPropertyChanged(nameof(IsSubmitting));
            RaiseStateChanged();
        }
    }

    /// <summary>
    /// Submits the form using a provided handler, overriding the configured one.
    /// </summary>
    /// <param name="handler">The submit handler.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the submission succeeded.</returns>
    public async Task<bool> SubmitAsync(Func<TModel, CancellationToken, Task<bool>> handler, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(handler);

        Validate();
        if (!IsValid)
            return false;

        IsSubmitting = true;
        try
        {
            return await handler(Model, cancellationToken);
        }
        finally
        {
            IsSubmitting = false;
        }
    }

    /// <summary>
    /// Gets whether a specific property has errors.
    /// </summary>
    public bool HasErrors(string propertyName)
    {
        return _errors.TryGetValue(propertyName, out var list) && list.Count > 0;
    }

    /// <summary>
    /// Gets the error messages for a specific property, or an empty list.
    /// </summary>
    public IList<string> GetErrors(string propertyName)
    {
        return _errors.TryGetValue(propertyName, out var list) ? list : Array.Empty<string>();
    }

    /// <summary>
    /// Manually sets an error for a property (for custom validation logic).
    /// </summary>
    public void SetError(string propertyName, string errorMessage)
    {
        ArgumentException.ThrowIfNullOrEmpty(propertyName);

        if (!_errors.TryGetValue(propertyName, out var list))
        {
            list = new List<string>();
            _errors[propertyName] = list;
        }

        if (!list.Contains(errorMessage))
            list.Add(errorMessage);

        IsValid = _errors.Values.All(e => e.Count == 0);
        OnPropertyChanged(nameof(IsValid));
        OnPropertyChanged(nameof(Errors));
        RaiseStateChanged();
    }

    /// <summary>
    /// Clears errors for a specific property.
    /// </summary>
    public void ClearErrors(string propertyName)
    {
        _errors.Remove(propertyName);
        _validator.ClearErrors(propertyName);
        IsValid = _errors.Values.All(e => e.Count == 0);
        OnPropertyChanged(nameof(IsValid));
        OnPropertyChanged(nameof(Errors));
        RaiseStateChanged();
    }

    private void SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (!EqualityComparer<T>.Default.Equals(field, value))
        {
            field = value;
            OnPropertyChanged(propertyName);
        }
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void RaiseStateChanged()
    {
        StateChanged?.Invoke(this, new FormStateChangedEventArgs
        {
            IsDirty = IsDirty,
            IsValid = IsValid,
            IsSubmitting = IsSubmitting,
        });
    }
}

/// <summary>
/// Event arguments for <see cref="FormState{TModel}.StateChanged"/> events.
/// </summary>
public class FormStateChangedEventArgs : EventArgs
{
    /// <summary>Gets whether the form has unsaved changes.</summary>
    public bool IsDirty { get; init; }

    /// <summary>Gets whether the form is valid.</summary>
    public bool IsValid { get; init; }

    /// <summary>Gets whether the form is submitting.</summary>
    public bool IsSubmitting { get; init; }
}
