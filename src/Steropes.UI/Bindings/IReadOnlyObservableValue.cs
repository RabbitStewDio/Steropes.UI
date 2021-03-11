using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Steropes.UI.Bindings
{
  public interface IBindingSubscription : IDisposable
  {
    IReadOnlyList<IBindingSubscription> Sources { get; }
  }

  public interface IReadOnlyObservableValue : INotifyPropertyChanged, IBindingSubscription
  {
    object Value { get; }
  }

  public interface IReadOnlyObservableValue<out T> : IReadOnlyObservableValue
  {
    new T Value { get; }
  }
}