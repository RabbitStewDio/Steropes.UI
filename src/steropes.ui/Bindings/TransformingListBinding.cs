using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Steropes.UI.Bindings
{
  internal class TransformingListBinding<TSource, TTarget> : ReadOnlyObservableListBindingBase<TTarget>
  {
    readonly IReadOnlyObservableListBinding<TSource> parent;
    readonly Func<TSource, TTarget> mappingFunction;

    public TransformingListBinding(IReadOnlyObservableListBinding<TSource> parent, Func<TSource, TTarget> mappingFunction)
    {
      this.parent = parent ?? throw new ArgumentNullException(nameof(parent));
      this.mappingFunction = mappingFunction;
      this.parent.PropertyChanged += OnParentPropertyChanged;
      this.parent.CollectionChanged += OnParentCollectionChanged;
    }

    void OnParentCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      IList<TTarget> Map(IList l)
      {
        List<TTarget> retval = new List<TTarget>();
        foreach (var o in l)
        {
          if (o is TSource s)
          {
            retval.Add(mappingFunction(s));
          }
        }

        return retval;
      }

      NotifyCollectionChangedEventArgs evt;
      switch (e.Action)
      {
        case NotifyCollectionChangedAction.Add:
          evt = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, Map(e.NewItems), e.NewStartingIndex);
          break;
        case NotifyCollectionChangedAction.Remove:
          evt = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, Map(e.OldItems), e.OldStartingIndex);
          break;
        case NotifyCollectionChangedAction.Replace:
          evt = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, Map(e.NewItems), Map(e.OldItems), e.OldStartingIndex);
          break;
        case NotifyCollectionChangedAction.Move:
          evt = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, Map(e.NewItems), e.NewStartingIndex, e.OldStartingIndex);
          break;
        case NotifyCollectionChangedAction.Reset:
          evt = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
      CollectionChanged?.Invoke(this, evt);
    }

    void OnParentPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      PropertyChanged?.Invoke(this, e);
    }


    public override int Count => parent.Count;

    public override TTarget this[int index]
    {
      get { return mappingFunction(parent[index]); }
    }

    public override event PropertyChangedEventHandler PropertyChanged;
    public override event NotifyCollectionChangedEventHandler CollectionChanged;
  }
}