using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Steropes.UI.Util;

namespace Steropes.UI.Bindings
{
  class ListSynchronizer<T>
  {
    public IReadOnlyObservableListBinding<T> SourceList { get; protected set; }
    public IObservableListBinding<T> Target { get; protected set; }
    IList<T> TargetAsList => Target;

    protected ListSynchronizer()
    {
    }

    public ListSynchronizer(IReadOnlyObservableListBinding<T> sourceList, IObservableListBinding<T> target)
    {
      SourceList = sourceList ?? throw new ArgumentNullException(nameof(sourceList));
      Target = target ?? throw new ArgumentNullException(nameof(target));
    }

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
        TargetAsList.Insert(start + idx, SourceList[start + idx]);
      }
    }

    void DoReplace(int start, int count)
    {
      for (var idx = 0; idx < count; idx += 1)
      {
        TargetAsList[idx + start] = SourceList[idx];
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
        Target.Move(idx + newStart, idx + oldStart);
      }
    }

    internal void HandleSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs evt)
    {
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
          Target.AddRange(SourceList);
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }
  }
}