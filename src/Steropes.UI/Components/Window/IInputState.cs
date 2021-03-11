using Microsoft.Xna.Framework;
using Steropes.UI.Input;

namespace Steropes.UI.Components.Window
{
  public interface IInputState
  {
    Point MousePosition { get; }
    Point MousePositionChange { get; }
    InputFlags InputFlags { get;  }
  }
}
