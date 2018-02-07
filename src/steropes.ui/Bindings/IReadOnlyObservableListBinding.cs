using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Steropes.UI.Bindings
{
  public interface IReadOnlyObservableListBinding<T>: IReadOnlyList<T>, INotifyPropertyChanged, INotifyCollectionChanged, IBindingSubscription
  {
  }
}