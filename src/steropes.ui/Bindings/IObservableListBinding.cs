using System.Collections;
using System.Collections.Generic;

namespace Steropes.UI.Bindings
{
  public interface IObservableListBinding<T>: IList, IList<T>, IReadOnlyObservableListBinding<T>
  {
    new void Clear();
    void Move(int sourceIdx, int targetIdx);
  }
}