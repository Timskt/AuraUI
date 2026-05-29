using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AuraUI.Core.MVVM;

/// <summary>
/// Base class for all ViewModels. Implements INotifyPropertyChanged with helper methods.
/// </summary>
public abstract class ViewModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    protected bool SetProperty<T>(ref T field, T value, Action? onChanged, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;
        OnPropertyChanged(propertyName);
        onChanged?.Invoke();
        return true;
    }

    protected void OnPropertyChanged(params string[] propertyNames)
    {
        foreach (var name in propertyNames)
            OnPropertyChanged(name);
    }

    /// <summary>
    /// Sets a property value and raises PropertyChanged for the property and any dependent properties.
    /// </summary>
    protected bool SetProperty<T>(ref T field, T value, params string[] alsoNotify)
    {
        if (SetProperty(ref field, value))
        {
            OnPropertyChanged(alsoNotify);
            return true;
        }
        return false;
    }
}
