using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Steropes.UI.Bindings
{
  /// <summary>
  ///  A base implementation of IObservableListBinding to take the pain out of 
  ///  creating new binding implementations. This class implements common 
  ///  boilerplate code that makes up most of the IList and IList[T] interfaces.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public abstract class ObservableListBindingBase<T> : IObservableListBinding<T>
  {
    public abstract int Count { get; }

    public abstract void Dispose();
    public abstract IReadOnlyList<IBindingSubscription> Sources { get; }

    public abstract void Move(int sourceIdx, int targetIdx);
    
    #region Hide explicit Collection and List implementations

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    void ICollection.CopyTo(Array array, int index)
    {
      if (array == null)
      {
        throw new ArgumentNullException(nameof(array));
      }

      if (array.Rank != 1)
      {
        throw new ArgumentException("Array must have a rank of 1");
      }

      if (array.GetLowerBound(0) != 0)
      {
        throw new ArgumentException("Array must have a lower bound of zero");
      }

      if (index < 0)
      {
        throw new ArgumentOutOfRangeException(nameof(index), "Index cannot be negative");
      }

      if (array.Length - index < Count)
      {
        throw new ArgumentException("Array is not large enough.");
      }

      if (array is T[] typedArray)
      {
        CopyTo(typedArray, index);
      }
      else
      {
        var elementType = array.GetType().GetElementType();
        var c = typeof(T);
        if (elementType == null || !elementType.IsAssignableFrom(c) && !c.IsAssignableFrom(elementType))
        {
          throw new ArgumentException("This array is not compatible with the collection type.");
        }

        if (!(array is object[] objArray))
        {
          throw new ArgumentException("This array must be an object array.");
        }

        var count = Count;
        try
        {
          for (var index1 = 0; index1 < count; ++index1)
          {
            objArray[index++] = this[index1];
          }
        }
        catch (ArrayTypeMismatchException)
        {
          throw new ArgumentException("This array is not compatible with the collection type.");
        }
      }
    }
    
    
    int ICollection.Count
    {
      get { return Count; }
    }

    int IList.Add(object value)
    {
      Add((T) value);
      return Count - 1;
    }

    bool IList.Contains(object value)
    {
      if (value is T variable)
      {
        return Contains(variable);
      }

      return false;
    }
    
    void ICollection<T>.Clear()
    {
      Clear();
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
      return GetEnumerator();
    }

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
      set { this[index] = (T)value; }
    }


    #endregion

    public virtual void CopyTo(T[] array, int arrayIndex)
    {
      for (var i = 0; i < Count; i++)
      {
        array[arrayIndex + i] = this[i];
      }
    }

    public abstract object SyncRoot { get; }
    public abstract bool IsSynchronized { get; }
    

    public abstract void Add(T item);



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

    public abstract bool IsFixedSize { get; }
    public abstract event PropertyChangedEventHandler PropertyChanged;
    public abstract event NotifyCollectionChangedEventHandler CollectionChanged;

    public ListEnumerator GetEnumerator()
    {
      return new ListEnumerator(this);
    }

    public struct ListEnumerator : IEnumerator<T>
    {
      readonly IReadOnlyList<T> widget;

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
        var w = widget;
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

      object IEnumerator.Current
      {
        get { return Current; }
      }

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