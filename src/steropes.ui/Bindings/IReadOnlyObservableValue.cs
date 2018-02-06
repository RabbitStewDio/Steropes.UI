using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Steropes.UI.Annotations;

namespace Steropes.UI.Bindings
{
  public interface IReadOnlyObservableValue : INotifyPropertyChanged
  {
    object Value { get; }
  }

  public interface IReadOnlyObservableValue<out T> : IReadOnlyObservableValue
  {
    new T Value { get; }
  }

  public interface IObservableValue<T> : IReadOnlyObservableValue<T>
  {
    new T Value { get; set; }
  }

  public class ObservableValue<T> : IObservableValue<T>
  {
    T value;

    public ObservableValue()
    {
    }

    public ObservableValue(T value)
    {
      this.value = value;
    }

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

  public interface IReadOnlyObservableListBinding<T>: IReadOnlyList<T>, INotifyPropertyChanged, INotifyCollectionChanged
  {
  }
 
  public interface IObservableListBinding<T>: IList, IList<T>, IReadOnlyObservableListBinding<T>
  {
  }
}