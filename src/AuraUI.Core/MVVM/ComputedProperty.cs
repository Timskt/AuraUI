using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace AuraUI.Core.MVVM;

/// <summary>
/// Vue-like computed property that automatically recalculates when its dependencies change.
/// Uses expression tree analysis to discover and subscribe to <see cref="INotifyPropertyChanged"/>
/// sources, then re-evaluates the selector when any dependency raises <c>PropertyChanged</c>.
/// </summary>
/// <typeparam name="T">The type of the computed value.</typeparam>
/// <example>
/// <code>
/// // In a ViewModel:
/// public class OrderViewModel : ViewModelBase
/// {
///     private decimal _price;
///     public decimal Price { get => _price; set => SetProperty(ref _price, value); }
///
///     private int _quantity;
///     public int Quantity { get => _quantity; set => SetProperty(ref _quantity, value); }
///
///     // Auto-recalculates when Price or Quantity changes
///     public ComputedProperty&lt;decimal&gt; Total { get; }
///
///     public OrderViewModel()
///     {
///         Total = new ComputedProperty&lt;decimal&gt;(() => Price * Quantity, this);
///     }
/// }
/// </code>
/// </example>
public class ComputedProperty<T> : INotifyPropertyChanged, IDisposable
{
    private readonly Func<T> _selector;
    private readonly List<(INotifyPropertyChanged Source, string PropertyName)> _subscriptions = new();
    private readonly object _lock = new();
    private T _value;
    private bool _disposed;
    private bool _isDirty = true;

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Gets the current computed value. Recalculates lazily if dependencies have changed.
    /// </summary>
    public T Value
    {
        get
        {
            if (_isDirty)
            {
                Recalculate();
            }
            return _value;
        }
    }

    /// <summary>
    /// Creates a new computed property from a selector expression.
    /// </summary>
    /// <param name="selector">
    /// A lambda expression that computes the value.
    /// Property accesses on <see cref="INotifyPropertyChanged"/> sources are automatically tracked.
    /// </param>
    /// <param name="owner">
    /// An optional owner object. If it implements <see cref="INotifyPropertyChanged"/>,
    /// all property accesses on it within the selector are tracked.
    /// </param>
    public ComputedProperty(Expression<Func<T>> selector, INotifyPropertyChanged? owner = null)
    {
        ArgumentNullException.ThrowIfNull(selector);

        _selector = selector.Compile();
        AnalyzeDependencies(selector.Body);

        // If an owner was provided and accesses its properties, track it
        if (owner is not null)
        {
            SubscribeToOwner(owner);
        }

        // Initial calculation
        Recalculate();
    }

    /// <summary>
    /// Creates a new computed property from a simple function.
    /// Dependencies must be manually specified.
    /// </summary>
    /// <param name="selector">The computation function.</param>
    /// <param name="dependencies">
    /// The <see cref="INotifyPropertyChanged"/> sources and property names this computed depends on.
    /// </param>
    public ComputedProperty(Func<T> selector, params (INotifyPropertyChanged Source, string PropertyName)[] dependencies)
    {
        ArgumentNullException.ThrowIfNull(selector);

        _selector = selector;

        foreach (var (source, propertyName) in dependencies)
        {
            Subscribe(source, propertyName);
        }

        Recalculate();
    }

    /// <summary>
    /// Forces a recalculation of the computed value.
    /// </summary>
    public void Invalidate()
    {
        _isDirty = true;
        OnPropertyChanged(nameof(Value));
    }

    /// <summary>
    /// Manually subscribes to a property on a source.
    /// </summary>
    public void Subscribe(INotifyPropertyChanged source, string propertyName)
    {
        lock (_lock)
        {
            _subscriptions.Add((source, propertyName));
            source.PropertyChanged += OnPropertyChanged;
        }
    }

    private void AnalyzeDependencies(Expression expression)
    {
        // Walk the expression tree to find member accesses on INotifyPropertyChanged objects
        var visitor = new DependencyVisitor();
        visitor.Visit(expression);

        foreach (var (source, propertyName) in visitor.Dependencies)
        {
            Subscribe(source, propertyName);
        }
    }

    private void SubscribeToOwner(INotifyPropertyChanged owner)
    {
        // Track all property accesses on the owner during the first evaluation
        // This is done by monitoring the selector output
        lock (_lock)
        {
            _subscriptions.Add((owner, "*")); // wildcard = any property change
            owner.PropertyChanged += OnPropertyChanged;
        }
    }

    private void Recalculate()
    {
        lock (_lock)
        {
            try
            {
                var newValue = _selector();
                if (!EqualityComparer<T>.Default.Equals(_value, newValue))
                {
                    _value = newValue;
                    OnPropertyChanged(nameof(Value));
                }
            }
            catch
            {
                // Silently handle evaluation errors
            }
            _isDirty = false;
        }
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        // Check if this property change is relevant
        lock (_lock)
        {
            foreach (var (source, propertyName) in _subscriptions)
            {
                if (ReferenceEquals(source, sender))
                {
                    if (propertyName == "*" || propertyName == e.PropertyName)
                    {
                        _isDirty = true;
                        OnPropertyChanged(nameof(Value));
                        return;
                    }
                }
            }
        }
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        lock (_lock)
        {
            foreach (var (source, _) in _subscriptions)
            {
                source.PropertyChanged -= OnPropertyChanged;
            }
            _subscriptions.Clear();
        }

        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Expression visitor that extracts <see cref="INotifyPropertyChanged"/> sources
    /// and property names from an expression tree.
    /// </summary>
    private class DependencyVisitor : ExpressionVisitor
    {
        public List<(INotifyPropertyChanged Source, string PropertyName)> Dependencies { get; } = new();

        protected override Expression VisitMember(MemberExpression node)
        {
            // Try to evaluate the expression to get the source object
            if (node.Expression is ConstantExpression constant &&
                constant.Value is INotifyPropertyChanged inpc)
            {
                Dependencies.Add((inpc, node.Member.Name));
            }
            else
            {
                // Try to compile and evaluate to get the source
                try
                {
                    var lambda = Expression.Lambda(node.Expression!);
                    var compiled = lambda.Compile();
                    var source = compiled.DynamicInvoke();
                    if (source is INotifyPropertyChanged inpcSource)
                    {
                        Dependencies.Add((inpcSource, node.Member.Name));
                    }
                }
                catch
                {
                    // Cannot evaluate - skip this dependency
                }
            }

            return base.VisitMember(node);
        }
    }
}
