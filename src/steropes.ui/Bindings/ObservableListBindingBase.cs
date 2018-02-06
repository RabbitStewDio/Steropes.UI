using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Steropes.UI.Bindings
{
  internal abstract class ObservableListBindingBase<T> : IObservableListBinding<T>
  {
    public abstract int Count { get; }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    void ICollection.CopyTo(Array array, int index)
    {
      if (array == null)
        throw new ArgumentNullException(nameof(array));
      if (array.Rank != 1)
        throw new ArgumentException("Array must have a rank of 1");
      if (array.GetLowerBound(0) != 0)
        throw new ArgumentException("Array must have a lower bound of zero");
      if (index < 0)
        throw new ArgumentOutOfRangeException(nameof(index), "Index cannot be negative");

      if (array.Length - index < this.Count)
        throw new ArgumentException("Array is not large enough.");

      if (array is T[] typedArray)
      {
        CopyTo(typedArray, index);
      }
      else
      {
        Type elementType = array.GetType().GetElementType();
        Type c = typeof(T);
        if (elementType == null || (!elementType.IsAssignableFrom(c) && !c.IsAssignableFrom(elementType)))
          throw new ArgumentException("This array is not compatible with the collection type.");

        if (!(array is object[] objArray))
          throw new ArgumentException("This array must be an object array.");
        int count = Count;
        try
        {
          for (int index1 = 0; index1 < count; ++index1)
            objArray[index++] = this[index1];
        }
        catch (ArrayTypeMismatchException)
        {
          throw new ArgumentException("This array is not compatible with the collection type.");
        }
      }
    }

    public virtual void CopyTo(T[] array, int arrayIndex)
    {
      for (var i = 0; i < this.Count; i++)
      {
        array[arrayIndex + i] = this[i];
      }
    }


    int ICollection.Count
    {
      get { return Count; }
    }

    public abstract object SyncRoot { get; }
    public abstract bool IsSynchronized { get; }

    int IList.Add(object value)
    {
      Add((T) value);
      return this.Count - 1;
    }

    bool IList.Contains(object value)
    {
      if (value is T variable)
      {
        return Contains(variable);
      }

      return false;
    }

    public ListEnumerator GetEnumerator()
    {
      return new ListEnumerator(this);
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
      return GetEnumerator();
    }

    public abstract void Add(T item);

    void ICollection<T>.Clear()
    {
      Clear();
    }

    public abstract void Clear();

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

    public abstract bool Remove(T item);
    public abstract bool IsReadOnly { get; }

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
    public abstract void Insert(int index, T item);
    public abstract void RemoveAt(int index);
    public abstract T this[int index] { get; set; }

    void IList.Clear()
    {
      Clear();
    }

    int IList.IndexOf(object value)
    {
      if (value is T variable)
      {
        return IndexOf(variable);
      }

      return -1;
    }

    void IList.Insert(int index, object value)
    {
      if (value is T variable)
      {
        Insert(index, variable);
      }
      else
      {
        throw new InvalidCastException();
      }
    }

    void IList.Remove(object value)
    {
      if (value is T variable)
      {
        Remove(variable);
      }
    }

    void IList.RemoveAt(int index)
    {
      RemoveAt(index);
    }

    object IList.this[int index]
    {
      get { return this[index]; }
      set { this[index] = (T) value; }
    }

    public abstract bool IsFixedSize { get; }
    public abstract event PropertyChangedEventHandler PropertyChanged;
    public abstract event NotifyCollectionChangedEventHandler CollectionChanged;
    
    public struct ListEnumerator : IEnumerator<T>
    {
      readonly IList<T> widget;
      
      int index;

      T current;

      internal ListEnumerator(IObservableListBinding<T> widget) : this()
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
        IList<T> w = widget;
        if (index + 1 < w.Count)
        {
          index += 1;
          current = w[index];
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
  }
}