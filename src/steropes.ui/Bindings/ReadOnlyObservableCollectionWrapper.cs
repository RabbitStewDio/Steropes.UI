using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Steropes.UI.Bindings
{
  internal class ReadOnlyObservableCollectionWrapper<T> : IReadOnlyObservableListBinding<T>
  {
    readonly ReadOnlyObservableCollection<T> self;

    public ReadOnlyObservableCollectionWrapper(ReadOnlyObservableCollection<T> self)
    {
      this.self = self ?? throw new ArgumentNullException(nameof(self));
      ((INotifyPropertyChanged) this.self).PropertyChanged += OnParentPropertyChanged;
      ((INotifyCollectionChanged) self).CollectionChanged += OnParentCollectionChanged;
    }

    public void Dispose()
    {
      ((INotifyPropertyChanged) self).PropertyChanged -= OnParentPropertyChanged;
      ((INotifyCollectionChanged) self).CollectionChanged -= OnParentCollectionChanged;
    }

    public IReadOnlyList<IBindingSubscription> Sources => new IReadOnlyObservableValue[0];

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    public IEnumerator<T> GetEnumerator()
    {
      return self.GetEnumerator();
    }

    public int Count => self.Count;

    public T this[int index]
    {
      get { return self[index]; }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    public event NotifyCollectionChangedEventHandler CollectionChanged;

    void OnParentCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      CollectionChanged?.Invoke(this, e);
    }

    void OnParentPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      PropertyChanged?.Invoke(this, e);
    }
  }
}