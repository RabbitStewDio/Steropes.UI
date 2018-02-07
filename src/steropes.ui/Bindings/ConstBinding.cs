using System.Collections.Generic;
using System.ComponentModel;

namespace Steropes.UI.Bindings
{
  internal class ConstBinding : IReadOnlyObservableValue
  {
    public ConstBinding(object value)
    {
      Value = value;
    }

    public event PropertyChangedEventHandler PropertyChanged
    {
      add { }
      remove { }
    }

    public object Value { get; }

    public void Dispose()
    {
    }

    static IBindingSubscription[] empty = new IBindingSubscription[0];
    public IReadOnlyList<IBindingSubscription> Sources => empty;
  }

  internal class ConstBinding<T> : IReadOnlyObservableValue<T>
  {
    public ConstBinding(T value)
    {
      Value = value;
    }

    public event PropertyChangedEventHandler PropertyChanged
    {
      add { }
      remove { }
    }

    public T Value { get; }

    object IReadOnlyObservableValue.Value
    {
      get { return Value; }
    }

    public void Dispose()
    {
    }

    static IBindingSubscription[] empty = new IBindingSubscription[0];
    public IReadOnlyList<IBindingSubscription> Sources => empty;
  }
}