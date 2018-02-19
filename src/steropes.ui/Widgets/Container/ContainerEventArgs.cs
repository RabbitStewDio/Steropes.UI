using System;
using Steropes.UI.Components;

namespace Steropes.UI.Widgets.Container
{
  public class ContainerEventArgs : IEquatable<ContainerEventArgs>
  {
    public ContainerEventArgs(int index,
                              IWidget removedChild,
                              object removedConstraints,
                              IWidget addedChild,
                              object addedConstraints)
    {
      Index = index;
      RemovedChild = removedChild;
      RemovedConstraints = removedConstraints;
      AddedChild = addedChild;
      AddedConstraints = addedConstraints;
    }

    public int Index { get; }

    public IWidget AddedChild { get; }
    public object AddedConstraints { get; }

    public IWidget RemovedChild { get; }
    public object RemovedConstraints { get; }

    public bool Equals(ContainerEventArgs other)
    {
      if (ReferenceEquals(null, other)) return false;
      if (ReferenceEquals(this, other)) return true;
      return Index == other.Index &&
             Equals(AddedChild, other.AddedChild) &&
             Equals(AddedConstraints, other.AddedConstraints) &&
             Equals(RemovedChild, other.RemovedChild) &&
             Equals(RemovedConstraints, other.RemovedConstraints);
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;
      if (obj.GetType() != this.GetType()) return false;
      return Equals((ContainerEventArgs) obj);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        var hashCode = Index;
        hashCode = (hashCode * 397) ^ (AddedChild != null ? AddedChild.GetHashCode() : 0);
        hashCode = (hashCode * 397) ^ (AddedConstraints != null ? AddedConstraints.GetHashCode() : 0);
        hashCode = (hashCode * 397) ^ (RemovedChild != null ? RemovedChild.GetHashCode() : 0);
        hashCode = (hashCode * 397) ^ (RemovedConstraints != null ? RemovedConstraints.GetHashCode() : 0);
        return hashCode;
      }
    }

    public static bool operator ==(ContainerEventArgs left, ContainerEventArgs right)
    {
      return Equals(left, right);
    }

    public static bool operator !=(ContainerEventArgs left, ContainerEventArgs right)
    {
      return !Equals(left, right);
    }

    public override string ToString()
    {
      return$"{nameof(Index)}: {Index}, {nameof(AddedChild)}: {AddedChild}, {nameof(AddedConstraints)}: {AddedConstraints}, {nameof(RemovedChild)}: {RemovedChild}, {nameof(RemovedConstraints)}: {RemovedConstraints}";
    }
  }
}