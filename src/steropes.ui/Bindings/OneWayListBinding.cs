using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using Steropes.UI.Util;

namespace Steropes.UI.Bindings
{
  class OneWayListBinding<T> : IBindingSubscription
  {
    const string IndexerName = "Item[]";

    readonly IObservableListBinding<T> target;
    readonly IReadOnlyObservableValue<IReadOnlyList<T>> sourceValue;
    bool alreadyHandlingEvent;

    public OneWayListBinding(IObservableListBinding<T> target,
                             IReadOnlyObservableValue<IReadOnlyList<T>> sourceValue)
    {
      this.target = target ?? throw new ArgumentNullException(nameof(target));
      this.sourceValue = sourceValue ?? throw new ArgumentNullException(nameof(sourceValue));
      this.sourceValue.PropertyChanged += OnSourceValueChanged;

      target.Clear();
      target.AddRange(sourceValue.Value);
    }

    IList<T> TargetAsList => target;

    void OnSourceValueChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!alreadyHandlingEvent)
      {
        try
        {
          alreadyHandlingEvent = true;
          if (IndexerName.Equals(e.PropertyName, StringComparison.Ordinal))
          {
            TargetAsList.Clear();
            target.AddRange(sourceValue.Value);
          }
        }
        finally
        {
          alreadyHandlingEvent = false;
        }
      }
    }

    public void Dispose()
    {
      if (this.sourceValue != null)
      {
        this.sourceValue.PropertyChanged -= OnSourceValueChanged;
      }
    }

    public IReadOnlyList<IBindingSubscription> Sources => new IBindingSubscription[] { sourceValue };
  }

  internal class TwoWayListBinding<T> : IBindingSubscription
  {
    readonly ListSynchronizer<T> forwardBinding;
    readonly ListSynchronizer<T> reverseBinding;
    bool alreadyHandlingEvent;

    public TwoWayListBinding(IObservableListBinding<T> source, IObservableListBinding<T> target)
    {
      this.forwardBinding = new ListSynchronizer<T>(source, target);
      this.forwardBinding.SourceList.CollectionChanged += OnSourceChanged;
      this.reverseBinding = new ListSynchronizer<T>(target, source);
      this.reverseBinding.SourceList.CollectionChanged += OnTargetChanged;
    }

    void OnTargetChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      if (!alreadyHandlingEvent)
      {
        try
        {
          alreadyHandlingEvent = true;
          reverseBinding.HandleSourceCollectionChanged(sender, e);
        }
        finally
        {
          alreadyHandlingEvent = false;
        }
      }
    }

    void OnSourceChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      if (!alreadyHandlingEvent)
      {
        try
        {
          alreadyHandlingEvent = true;
          forwardBinding.HandleSourceCollectionChanged(sender, e);
        }
        finally
        {
          alreadyHandlingEvent = false;
        }
      }
    }

    public void Dispose()
    {
      this.forwardBinding.SourceList.CollectionChanged -= OnSourceChanged;
      this.reverseBinding.SourceList.CollectionChanged -= OnTargetChanged;
    }

    public IReadOnlyList<IBindingSubscription> Sources => new IBindingSubscription[] { forwardBinding.SourceList, reverseBinding.SourceList };
  }
}