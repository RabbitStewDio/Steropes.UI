using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Steropes.UI.Bindings
{
  /// <summary>
  ///  A simple wrapper around ObservableCollection. This acts as a source 
  ///  in a binding chain.
  /// 
  ///  Lifting class that takes an ordinary ReadOnlyCollection and lifts it
  ///  into an IObservableListBinding instance. If the library designers of
  ///  C# would have used interfaces consistently this mess would not be
  ///  necessary.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class ObservableListBinding<T> : IObservableListBinding<T>
  {
    readonly ObservableCollection<T> self;
    bool alreadyHandlingChange;

    public ObservableListBinding(ObservableCollection<T> self)
    {
      this.self = self ?? throw new ArgumentNullException(nameof(self));
      ((INotifyPropertyChanged) this.self).PropertyChanged += OnParentPropertyChanged;
      self.CollectionChanged += OnParentCollectionChanged;
    }

    public void Dispose()
    {
      ((INotifyPropertyChanged) this.self).PropertyChanged -= OnParentPropertyChanged;
      self.CollectionChanged -= OnParentCollectionChanged;
    }

    public IReadOnlyList<IBindingSubscription> Sources => new IBindingSubscription[0];

    void OnParentCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      if (!alreadyHandlingChange)
      {
        try
        {
          alreadyHandlingChange = true;
          CollectionChanged?.Invoke(this, e);
        }
        finally
        {
          alreadyHandlingChange = false;
        }
      }
    }

    void OnParentPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!alreadyHandlingChange)
      {
        try
        {
          alreadyHandlingChange = true;
          PropertyChanged?.Invoke(this, e);
        }
        finally
        {
          alreadyHandlingChange = false;
        }
      }

    }

    public void Move(int sourceIdx, int targetIdx)
    {
      self.Move(sourceIdx, targetIdx);
    }

    IList SelfAsList => self;

    public void CopyTo(Array array, int index)
    {
      SelfAsList.CopyTo(array, index);
    }

    #region Explicit Interface Implementations
    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    int IList.Add(object value)
    {
      return SelfAsList.Add(value);
    }

    bool IList.Contains(object value)
    {
      return SelfAsList.Contains(value);
    }

    int IList.IndexOf(object value)
    {
      return SelfAsList.IndexOf(value);
    }

    void IList.Insert(int index, object value)
    {
      SelfAsList.Insert(index, value);
    }

    void IList.Remove(object value)
    {
      SelfAsList.Remove(value);
    }

    object IList.this[int index]
    {
      get { return SelfAsList[index]; }
      set { SelfAsList[index] = value; }
    }

    object ICollection.SyncRoot
    {
      get { return ((ICollection)self).SyncRoot; }
    }

    bool ICollection.IsSynchronized
    {
      get { return ((ICollection)self).IsSynchronized; }
    }

    public IEnumerator<T> GetEnumerator()
    {
      return self.GetEnumerator();
    }

    #endregion

    public T this[int index]
    {
      get { return self[index]; }
      set { self[index] = value; }
    }

    public bool IsFixedSize
    {
      get { return SelfAsList.IsFixedSize; }
    }

    public void Add(T item)
    {
      self.Add(item);
    }

    public void Clear()
    {
      self.Clear();
    }

    public void CopyTo(T[] array, int index)
    {
      self.CopyTo(array, index);
    }

    public bool Contains(T item)
    {
      return self.Contains(item);
    }

    public int IndexOf(T item)
    {
      return self.IndexOf(item);
    }

    public void Insert(int index, T item)
    {
      self.Insert(index, item);
    }

    public bool Remove(T item)
    {
      return self.Remove(item);
    }

    public void RemoveAt(int index)
    {
      self.RemoveAt(index);
    }

    public int Count
    {
      get { return self.Count; }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public event NotifyCollectionChangedEventHandler CollectionChanged;

    public bool IsReadOnly
    {
      get { return false; }
    }

    public static implicit operator ObservableListBinding<T>(ObservableCollection<T> c)
    {
      return new ObservableListBinding<T>(c);
    }
  }
}