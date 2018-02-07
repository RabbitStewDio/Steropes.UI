using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using Steropes.UI.Util;

namespace Steropes.UI.Bindings
{
  class OneWayListBinding<T> : IDisposable
  {
    const string IndexerName = "Item[]";

    readonly IObservableListBinding<T> target;
    readonly IReadOnlyObservableValue<IReadOnlyList<T>> sourceValue;
    readonly IReadOnlyObservableListBinding<T> sourceList;
    bool alreadyHandlingEvent;

    public OneWayListBinding(IObservableListBinding<T> target, IReadOnlyObservableValue<IReadOnlyList<T>> sourceValue)
    {
      this.target = target ?? throw new ArgumentNullException(nameof(target));
      this.sourceValue = sourceValue ?? throw new ArgumentNullException(nameof(sourceValue));
      this.sourceValue.PropertyChanged += OnSourceValueChanged;

      target.Clear();
      target.AddRange(sourceValue.Value);
    }

    public OneWayListBinding(IObservableListBinding<T> target, IReadOnlyObservableListBinding<T> sourceList)
    {
      this.target = target ?? throw new ArgumentNullException(nameof(target));
      this.sourceList = sourceList ?? throw new ArgumentNullException(nameof(sourceList));
      this.sourceList.CollectionChanged += OnSourceCollectionChanged;
    }

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

    IList TargetAsList => target;

    void DoRemove(int start, int count)
    {
      for (var idx = 0; idx < count; idx += 1)
      {
        TargetAsList.RemoveAt(start);
      }
    }

    void DoAdd(int start, int count)
    {
      for (var idx = 0; idx < count; idx += 1)
      {
        target.Insert(start + idx, sourceList[start + idx]);
      }
    }

    void DoReplace(int start, int count)
    {
      for (var idx = 0; idx < count; idx += 1)
      {
        TargetAsList[idx + start] = sourceList[idx];
      }
    }

    void DoMove(int oldStart, int newStart, int count)
    {
      if (oldStart == newStart)
      {
        return;
      }

      for (var idx = 0; idx < count; idx += 1)
      {
        target.Move(idx + newStart, idx + oldStart);
      }
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

        switch (evt.Action)
        {
          case NotifyCollectionChangedAction.Add:
            DoAdd(evt.NewStartingIndex, evt.NewItems.Count);
            break;
          case NotifyCollectionChangedAction.Remove:
            DoRemove(evt.OldStartingIndex, evt.OldItems.Count);
            break;
          case NotifyCollectionChangedAction.Replace:
            DoReplace(evt.NewStartingIndex, evt.NewItems.Count);
            break;
          case NotifyCollectionChangedAction.Move:
            DoMove(evt.OldStartingIndex, evt.NewStartingIndex, evt.OldItems.Count);
            break;
          case NotifyCollectionChangedAction.Reset:
            TargetAsList.Clear();
            target.AddRange(sourceList);
            break;
          default:
            throw new ArgumentOutOfRangeException();
        }
      }
      finally
      {
        alreadyHandlingEvent = false;
      }
    }

    public void Dispose()
    {
      if (this.sourceList != null)
      {
        this.sourceList.CollectionChanged -= OnSourceCollectionChanged;
      }

      if (this.sourceValue != null)
      {
        this.sourceValue.PropertyChanged -= OnSourceValueChanged;
      }
    }
  }

  internal class TwoWayListBinding<T> : IDisposable
  {
    readonly OneWayListBinding<T> forwardBinding;
    readonly OneWayListBinding<T> reverseBinding;

    public TwoWayListBinding(IObservableListBinding<T> source, IObservableListBinding<T> target)
    {
      this.forwardBinding = new OneWayListBinding<T>(source, target);
      this.reverseBinding = new OneWayListBinding<T>(target, source);
    }

    public void Dispose()
    {
      forwardBinding.Dispose();
      reverseBinding.Dispose();
    }
    
    
  }
}