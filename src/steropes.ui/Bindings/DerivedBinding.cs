using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Steropes.UI.Annotations;

namespace Steropes.UI.Bindings
{
  internal abstract class DerivedBinding<T> : IReadOnlyObservableValue<T>
  {
    T value;
    protected bool AlreadyHandlingEvent { get; set; }

    protected DerivedBinding()
    {
    }

    protected DerivedBinding(T value)
    {
      this.value = value;
    }

    public abstract void Dispose();
    public abstract IReadOnlyList<IBindingSubscription> Sources { get; }

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
      if (!AlreadyHandlingEvent)
      {
        try
        {
          AlreadyHandlingEvent = true;
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        finally
        {
          AlreadyHandlingEvent = false;
        }
      }
    }
  }
}