using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Steropes.UI.Annotations;

namespace Steropes.UI.Bindings
{
  public class ObservableValue<T> : IObservableValue<T>
  {
    // ReSharper disable once StaticMemberInGenericType
    static readonly IReadOnlyObservableValue[] empty = new IReadOnlyObservableValue[0];
    T value;

    public ObservableValue()
    {
    }

    public ObservableValue(T value)
    {
      this.value = value;
    }

    public void Dispose()
    {
    }

    public IReadOnlyList<IBindingSubscription> Sources => empty;

    object IReadOnlyObservableValue.Value
    {
      get { return value; }
    }

    public T Value
    {
      get { return value; }
      set
      {
        if (Equals(value, this.value))
        {
          return;
        }

        this.value = value;
        OnPropertyChanged();
      }
    }

    public static implicit operator T(ObservableValue<T> a)
    {
      return a.Value;
    }

    public static implicit operator ObservableValue<T>(T a)
    {
      return new ObservableValue<T>(a);
    }

    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}