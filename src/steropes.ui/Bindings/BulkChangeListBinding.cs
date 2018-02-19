using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Steropes.UI.Bindings
{
  internal class BulkChangeListBinding<T> : ReadOnlyObservableListBindingBase<T>
  {
    static readonly T[] empty = new T[0];

    readonly IReadOnlyObservableListBinding<T> parent;
    readonly Func<IReadOnlyList<T>, IReadOnlyList<T>> onChange;
    IReadOnlyList<T> data;

    public BulkChangeListBinding(IReadOnlyObservableListBinding<T> parent,
                                 Func<IReadOnlyList<T>, IReadOnlyList<T>> onChange)
    {
      this.parent = parent ?? throw new ArgumentNullException(nameof(parent));
      this.onChange = onChange ?? throw new ArgumentNullException(nameof(onChange));
      this.parent.PropertyChanged += OnParentPropertyChanged;
      this.data = onChange(parent) ?? empty;
    }

    public override void Dispose()
    {
      this.parent.PropertyChanged -= OnParentPropertyChanged;
    }

    public override IReadOnlyList<IBindingSubscription> Sources => new IBindingSubscription[] { parent };

    void Refresh()
    {
      this.data = onChange(parent) ?? empty;
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(ListBinding.IndexerName));
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
      CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }

    void OnParentPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      Refresh();
    }

    public override int Count => data.Count;

    public override T this[int index]
    {
      get { return data[index]; }
    }

    public override event PropertyChangedEventHandler PropertyChanged;
    public override event NotifyCollectionChangedEventHandler CollectionChanged;
  }
}