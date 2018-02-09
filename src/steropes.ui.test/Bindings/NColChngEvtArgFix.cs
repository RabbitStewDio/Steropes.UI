using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Steropes.UI.Test.Bindings
{
  struct NColChngEvtArgFix : IEquatable<NColChngEvtArgFix>
  {
    public NColChngEvtArgFix(NotifyCollectionChangedEventArgs args) : this()
    {
      OldStartingIndex = args.OldStartingIndex;
      Action = args.Action;
      NewItems = args.NewItems;
      OldItems = args.OldItems;
      NewStartingIndex = args.NewStartingIndex;
    }

    /// <summary>
    /// The action that caused the event.
    /// </summary>
    public NotifyCollectionChangedAction Action { get; set; }

    /// <summary>
    /// The items affected by the change.
    /// </summary>
    public IList NewItems { get; set; }

    /// <summary>
    /// The old items affected by the change (for Replace events).
    /// </summary>
    public IList OldItems { get; set; }

    /// <summary>
    /// The index where the change occurred.
    /// </summary>
    public int NewStartingIndex { get; set; }

    /// <summary>
    /// The old index where the change occurred (for Move events).
    /// </summary>
    public int OldStartingIndex { get; set; }

    public bool Equals(NColChngEvtArgFix other)
    {
      return Action == other.Action &&
             EqualList(NewItems, other.NewItems) &&
             EqualList(OldItems, other.OldItems) &&
             NewStartingIndex == other.NewStartingIndex &&
             OldStartingIndex == other.OldStartingIndex;
    }

    /// <summary>
    ///  Crappy List equals implementation as the standard library 
    ///  does not have a built in method for non generic lists.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    bool EqualList(IList a, IList b)
    {
      if (ReferenceEquals(a, b))
      {
        return true;
      }

      if (a == null || b == null)
      {
        return false;
      }

      var listA = new List<object>();
      foreach (var o in a)
      {
        listA.Add(o);
      }

      var listB = new List<object>();
      foreach (var o in b)
      {
        listB.Add(o);
      }

      return listA.SequenceEqual(listB);
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      return obj is NColChngEvtArgFix && Equals((NColChngEvtArgFix) obj);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        var hashCode = (int) Action;
        hashCode = (hashCode * 397) ^ (NewItems != null ? NewItems.GetHashCode() : 0);
        hashCode = (hashCode * 397) ^ (OldItems != null ? OldItems.GetHashCode() : 0);
        hashCode = (hashCode * 397) ^ NewStartingIndex;
        hashCode = (hashCode * 397) ^ OldStartingIndex;
        return hashCode;
      }
    }

    public static bool operator ==(NColChngEvtArgFix left, NColChngEvtArgFix right)
    {
      return left.Equals(right);
    }

    public static bool operator !=(NColChngEvtArgFix left, NColChngEvtArgFix right)
    {
      return !left.Equals(right);
    }
  }
}