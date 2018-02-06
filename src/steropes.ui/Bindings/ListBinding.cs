using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace Steropes.UI.Bindings
{
  public static class ListBinding
  {
    const string IndexerName = "Item[]";

    class ReadOnlyObservableCollectionWrapper<T> : IReadOnlyObservableListBinding<T>
    {
      readonly ReadOnlyObservableCollection<T> self;

      public ReadOnlyObservableCollectionWrapper(ReadOnlyObservableCollection<T> self)
      {
        this.self = self ?? throw new ArgumentNullException(nameof(self));
        ((INotifyPropertyChanged) this.self).PropertyChanged += OnParentPropertyChanged;
        ((INotifyCollectionChanged) self).CollectionChanged += OnParentCollectionChanged;
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        return GetEnumerator();
      }

      public IEnumerator<T> GetEnumerator()
      {
        return self.GetEnumerator();
      }

      public int Count => self.Count;

      public T this[int index]
      {
        get { return self[index]; }
      }

      public event PropertyChangedEventHandler PropertyChanged;
      public event NotifyCollectionChangedEventHandler CollectionChanged;

      void OnParentCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
      {
        CollectionChanged?.Invoke(this, e);
      }

      void OnParentPropertyChanged(object sender, PropertyChangedEventArgs e)
      {
        PropertyChanged?.Invoke(this, e);
      }
    }

    class ObservableArrayBinding<T> : ReadOnlyObservableListBindingBase<T>
    {
      readonly IReadOnlyObservableValue<IReadOnlyList<T>> listValue;
      IReadOnlyList<T> data;
      int count;

      public ObservableArrayBinding(IReadOnlyObservableValue<IReadOnlyList<T>> listValue)
      {
        this.listValue = listValue;
        this.listValue.PropertyChanged += OnDataChanged;
        this.data = listValue.Value;
        this.count = data?.Count ?? 0;
      }

      void OnDataChanged(object sender, PropertyChangedEventArgs e)
      {
        data = listValue.Value;
        SetCount(data?.Count ?? 0);
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IndexerName)));
        CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
      }

      public override int Count
      {
        get { return count; }
      }

      void SetCount(int value)
      {
        if (value != count)
        {
          count = value;
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
        }
      }

      public override T this[int index]
      {
        get { throw new NotImplementedException(); }
      }

      public override event PropertyChangedEventHandler PropertyChanged;
      public override event NotifyCollectionChangedEventHandler CollectionChanged;
    }

    class BulkChangeListBinding<T> : ReadOnlyObservableListBindingBase<T>
    {
      static readonly T[] empty = new T[0];

      readonly IReadOnlyObservableListBinding<T> parent;
      readonly Func<IReadOnlyList<T>, IReadOnlyList<T>> onChange;
      IReadOnlyList<T> data;

      public BulkChangeListBinding(IReadOnlyObservableListBinding<T> parent,
                                   Func<IReadOnlyList<T>, IReadOnlyList<T>> onChange)
      {
        this.parent = parent ?? throw new ArgumentNullException(nameof(parent));
        this.onChange = onChange;
        this.parent.PropertyChanged += OnParentPropertyChanged;
        this.data = onChange(parent) ?? empty;
      }

      void Refresh()
      {
        this.data = onChange(parent) ?? empty;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(IndexerName));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
        CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
      }

      void OnParentCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
      {
        Refresh();
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

    public static IObservableListBinding<T> From<T>(this ObservableCollection<T> self)
    {
      return new ObservableListBinding<T>(self);
    }

    public static IReadOnlyObservableListBinding<T> From<T>(this ReadOnlyObservableCollection<T> self)
    {
      return new ReadOnlyObservableCollectionWrapper<T>(self);
    }

    public static IReadOnlyObservableValue<int> CountBinding<T>(this IReadOnlyObservableListBinding<T> self)
    {
      return self.BindingFor(s => s.Count);
    }

    public static IReadOnlyObservableValue<IReadOnlyList<T>> ItemsBinding<T>(
      this IReadOnlyObservableListBinding<T> self)
    {
      return self.BindingFor(IndexerName, s => s.ToArray());
    }

    public static IReadOnlyObservableListBinding<T> AsListBinding<T>(
      this IReadOnlyObservableValue<IReadOnlyList<T>> self)
    {
      return new ObservableArrayBinding<T>(self);
    }

    public static IReadOnlyObservableListBinding<TTarget> Map<TSource, TTarget>(
      this IReadOnlyObservableListBinding<TSource> source,
      Func<TSource, TTarget> mapper)
    {
      return new TransformingListBinding<TSource, TTarget>(source, mapper);
    }

    public static IReadOnlyObservableListBinding<T> OrderByBinding<T>(this IReadOnlyObservableListBinding<T> source,
                                                                      IComparer<T> c = null)
    {
      if (c == null)
      {
        c = Comparer<T>.Default;
      }

      var data = new List<T>();
      return new BulkChangeListBinding<T>(source, (l) =>
      {
        if (!ReferenceEquals(data, l))
        {
          data.Clear();
          data.AddRange(l);
        }

        data.Sort(c);
        return data;
      });
    }

    public static IReadOnlyObservableListBinding<T> GetRangeBinding<T>(this IReadOnlyObservableListBinding<T> source,
                                                                       int index,
                                                                       int count)
    {
      if (count < 0)
      {
        throw new ArgumentOutOfRangeException(nameof(count), count, "cannot be negative");
      }

      if (index < 0)
      {
        throw new ArgumentOutOfRangeException(nameof(count), count, "cannot be negative");
      }

      var data = new List<T>();
      return new BulkChangeListBinding<T>(source, (l) =>
      {
        data.Clear();
        if (index < l.Count)
        {
          var max = Math.Min(l.Count - index, count);
          for (var i = 0; i < max; i += 1)
          {
            data.Add(l[i + index]);
          }
        }

        return data;
      });
    }

    public static IReadOnlyObservableValue<T> ItemAt<T>(this IReadOnlyObservableListBinding<T> source, int index)
    {
      T ConditionalGet(IReadOnlyObservableListBinding<T> l)
      {
        if (index < l.Count)
        {
          return l[index];
        }

        return default(T);
      }

      var constBinding = new ConstBinding<IReadOnlyObservableListBinding<T>>(source);
      return new TypeSafePropertyBinding<IReadOnlyObservableListBinding<T>, T>(constBinding,
                                                                               IndexerName,
                                                                               ConditionalGet);
    }

    public static void BindTo<T>(this IReadOnlyObservableListBinding<T> source,
                                 ObservableCollection<T> target)
    {
    }

    public static void BindTwoWay<T>(this IReadOnlyObservableListBinding<T> source,
                                     ObservableCollection<T> target)
    {
    }
  }
}