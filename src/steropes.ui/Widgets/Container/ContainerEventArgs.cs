using Steropes.UI.Components;

namespace Steropes.UI.Widgets.Container
{
  public class ContainerEventArgs
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
  }
}