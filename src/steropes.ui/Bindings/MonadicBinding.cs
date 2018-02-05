using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Steropes.UI.Annotations;

namespace Steropes.UI.Bindings
{
  internal abstract class DerivedBinding<T> : IReadOnlyObservableValue<T>
  {
    T value;

    public DerivedBinding()
    {
    }

    public event PropertyChangedEventHandler PropertyChanged;

    object IReadOnlyObservableValue.Value => Value;

    public T Value
    {
      get { return value; }
      protected set
      {
        if (Equals(value, this.value))
        {
          return;
        }

        this.value = value;
        OnPropertyChanged();
      }
    }

    protected void OnSourcePropertyChange(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == nameof(IReadOnlyObservableValue.Value))
      {
        Value = ComputeValue();
      }
    }

    protected abstract T ComputeValue();

    [NotifyPropertyChangedInvocator]
    void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

  }

  internal class DelegateBoundBinding<T> : DerivedBinding<T>
  {
    readonly Func<T> computation;
    readonly IReadOnlyObservableValue[] sources;

    public DelegateBoundBinding(Func<T> computation, params IReadOnlyObservableValue[] sources)
    {
      this.computation = computation ?? throw new ArgumentNullException(nameof(computation));
      this.sources = sources;
      foreach (var source in sources)
      {
        source.PropertyChanged += OnSourcePropertyChange;
      }
    }

    protected override T ComputeValue()
    {
      return computation();
    }
  }

  internal class MonadicBinding : DerivedBinding<object>
  {
    readonly IReadOnlyObservableValue source;
    readonly Func<object, object> mapping;


    public MonadicBinding(IReadOnlyObservableValue source, Func<object, object> mapping)
    {
      this.source = source ?? throw new ArgumentNullException(nameof(source));
      this.source = source;
      this.mapping = mapping ?? throw new ArgumentNullException(nameof(mapping));
      this.source.PropertyChanged += OnSourcePropertyChange;
      Value = mapping(source.Value);
    }

    protected override object ComputeValue()
    {
      return mapping(source.Value);
    }
  }

  internal class MonadicBinding<TSource, TTarget> : DerivedBinding<TTarget>
  {
    readonly IReadOnlyObservableValue<TSource> source;
    readonly Func<TSource, TTarget> mapping;


    public MonadicBinding(IReadOnlyObservableValue<TSource> source, Func<TSource, TTarget> mapping)
    {
      this.source = source ?? throw new ArgumentNullException(nameof(source));
      this.source = source;
      this.mapping = mapping ?? throw new ArgumentNullException(nameof(mapping));
      this.source.PropertyChanged += OnSourcePropertyChange;
      Value = mapping(source.Value);
    }

    protected override TTarget ComputeValue()
    {
      return mapping(source.Value);
    }
  }
}