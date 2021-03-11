using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace Steropes.UI.Bindings
{
  internal class IndexerBinding<T> : IObservableValue<T>
  {
    readonly IObservableListBinding<T> source;
    readonly int index;
    readonly T defaultValue;
    bool alreadyHandlingEvent;
    T value;

    public IndexerBinding(IObservableListBinding<T> source, int index, T defaultValue = default(T))
    {
      this.source = source ?? throw new ArgumentNullException(nameof(source));
      this.index = index;
      this.defaultValue = defaultValue;
      this.source.CollectionChanged += OnCollectionChanged;
    }

    IList<T> SourceAsList => source;

    public event PropertyChangedEventHandler PropertyChanged;

    public void Dispose()
    {
      source.CollectionChanged -= OnCollectionChanged;
    }

    void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      if (!alreadyHandlingEvent)
      {
        try
        {
          alreadyHandlingEvent = true;
          Value = Fetch();
        }
        finally
        {
          alreadyHandlingEvent = false;
        }
      }
    }

    T Fetch()
    {
      if (index < SourceAsList.Count)
      {
        return SourceAsList[index];
      }

      return defaultValue;
    }

    object IReadOnlyObservableValue.Value => Value;

    public T Value
    {
      get { return value; }
      set
      {
        if (Equals(value, this.value))
        {
          return;
        }

        while (index >= SourceAsList.Count)
        {
          source.Add(defaultValue);
        }

        this.value = value;
        source[index] = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
      }
    }

    public IReadOnlyList<IBindingSubscription> Sources { get; }
  }

  public static class ListBinding
  {
    internal const string IndexerName = "Item[]";

    public static IObservableListBinding<T> ToBinding<T>(this ObservableCollection<T> self)
    {
      return new ObservableListBinding<T>(self);
    }

    public static IReadOnlyObservableListBinding<T> ToBinding<T>(this ReadOnlyObservableCollection<T> self)
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

    public static IReadOnlyObservableListBinding<T> ToListBinding<T>(this IReadOnlyObservableValue<IReadOnlyList<T>> self)
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

    public static IReadOnlyObservableListBinding<T> MapAll<T>(this IReadOnlyObservableListBinding<T> source,
                                                              Func<IReadOnlyList<T>, IReadOnlyList<T>> transform)
    {
      return new BulkChangeListBinding<T>(source, transform);
    }

    public static IReadOnlyObservableListBinding<T> RangeBinding<T>(this IReadOnlyObservableListBinding<T> source,
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

    public static IReadOnlyObservableValue<T> First<T>(this IReadOnlyObservableListBinding<T> source, T defaultValue = default(T))
    {
      return source.ItemAt(0, defaultValue);
    }

    public static IReadOnlyObservableValue<T> ItemAt<T>(this IReadOnlyObservableListBinding<T> source, int index, T defaultValue = default(T))
    {
      T ConditionalGet(IReadOnlyObservableListBinding<T> l)
      {
        if (index < l.Count)
        {
          return l[index];
        }

        return defaultValue;
      }

      var constBinding = new ConstBinding<IReadOnlyObservableListBinding<T>>(source);
      return new TypeSafePropertyBinding<IReadOnlyObservableListBinding<T>, T>(constBinding,
                                                                               IndexerName,
                                                                               ConditionalGet);
    }

    public static IReadOnlyObservableValue<T> Last<T>(this IReadOnlyObservableListBinding<T> source, T defaultValue = default(T))
    {
      T ConditionalGet(IReadOnlyObservableListBinding<T> l)
      {
        if (l.Count > 0)
        {
          return l[l.Count - 1];
        }

        return defaultValue;
      }

      var constBinding = new ConstBinding<IReadOnlyObservableListBinding<T>>(source);
      return new TypeSafePropertyBinding<IReadOnlyObservableListBinding<T>, T>(constBinding,
                                                                               IndexerName,
                                                                               ConditionalGet);
    }

    public static IObservableValue<T> ItemAt<T>(this IObservableListBinding<T> source, int index, T defaultValue = default(T))
    {
      return new IndexerBinding<T>(source, index, defaultValue);
    }

    public static IBindingSubscription BindTo<T>(this IReadOnlyObservableValue<IReadOnlyList<T>> source,
                                 IObservableListBinding<T> target)
    {
      return new OneWayListBinding<T>(target, source);
    }

    public static IBindingSubscription BindTo<T>(this IReadOnlyObservableListBinding<T> source,
                                 IObservableListBinding<T> target)
    {
      return new OneWayObservableListBinding<T>(target, source);
    }

    public static IBindingSubscription BindTo<T>(this IReadOnlyObservableValue<IReadOnlyList<T>> source,
                                 ObservableCollection<T> target)
    {
      return source.BindTo(target.ToBinding());
    }

    public static IBindingSubscription BindTo<T>(this IReadOnlyObservableListBinding<T> source,
                                 ObservableCollection<T> target)
    {
      return source.BindTo(target.ToBinding());
    }

    public static IBindingSubscription BindTwoWay<T>(this IObservableListBinding<T> source,
                                            ObservableCollection<T> target)
    {
      var targetBinding = target.ToBinding();
      return source.BindTwoWay(targetBinding);
    }

    public static IBindingSubscription BindTwoWay<T>(this IObservableListBinding<T> source,
                                            IObservableListBinding<T> target)
    {
      return new TwoWayListBinding<T>(target, source);
    }
  }
}