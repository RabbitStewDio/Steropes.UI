using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Steropes.UI.Bindings
{
  class OneWayObservableListBinding<T> : ListSynchronizer<T>, IBindingSubscription
  {
    bool alreadyHandlingEvent;

    public OneWayObservableListBinding(IObservableListBinding<T> target, IReadOnlyObservableListBinding<T> sourceList)
    {
      this.Target = target ?? throw new ArgumentNullException(nameof(target));
      this.SourceList = sourceList ?? throw new ArgumentNullException(nameof(sourceList));
      this.SourceList.CollectionChanged += OnSourceCollectionChanged;
    }

    void OnSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs evt)
    {
      if (alreadyHandlingEvent)
      {
        return;
      }

      try
      {
        alreadyHandlingEvent = true;
        HandleSourceCollectionChanged(sender, evt);
      }
      finally
      {
        alreadyHandlingEvent = false;
      }
    }

    public IReadOnlyList<IBindingSubscription> Sources => new IBindingSubscription[] { SourceList };

    public void Dispose()
    {
      if (this.SourceList != null)
      {
        this.SourceList.CollectionChanged -= OnSourceCollectionChanged;
      }
    }

  }
}