using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Steropes.UI.Bindings
{
  internal abstract class ReadOnlyObservableListBindingBase<T> : IReadOnlyObservableListBinding<T>
  {
    public abstract int Count { get; }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }
    
    public virtual void CopyTo(T[] array, int arrayIndex)
    {
      for (var i = 0; i < this.Count; i++)
      {
        array[arrayIndex + i] = this[i];
      }
    }

    public ListEnumerator GetEnumerator()
    {
      return new ListEnumerator(this);
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
      return GetEnumerator();
    }

    public virtual bool Contains(T item)
    {
      foreach (var i in this)
      {
        if (Equals(item, i))
        {
          return true;
        }
      }

      return false;
    }

    public virtual int IndexOf(T item)
    {
      for (var i = 0; i < Count; i += 1)
      {
        if (Equals(item, this[i]))
        {
          return i;
        }
      }

      return -1;
    }
    
    public struct ListEnumerator : IEnumerator<T>
    {
      readonly IReadOnlyObservableListBinding<T> widget;

      int index;

      T current;

      internal ListEnumerator(IReadOnlyObservableListBinding<T> widget) : this()
      {
        this.widget = widget;
        index = -1;
        current = default(T);
      }

      public void Dispose()
      {
      }

      public bool MoveNext()
      {
        if (index + 1 < widget.Count)
        {
          index += 1;
          current = widget[index];
          return true;
        }

        current = default(T);
        return false;
      }

      public void Reset()
      {
        index = -1;
        current = default(T);
      }

      object IEnumerator.Current => Current;

      public T Current
      {
        get
        {
          if (index < 0 || index >= widget.Count)
          {
            throw new InvalidOperationException();
          }
          return current;
        }
      }
    }

    public abstract T this[int index] { get; }

    public abstract event PropertyChangedEventHandler PropertyChanged;
    public abstract event NotifyCollectionChangedEventHandler CollectionChanged;
  }
}