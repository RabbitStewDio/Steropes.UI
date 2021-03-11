using Steropes.UI.Components;
using Steropes.UI.Platform;

namespace Steropes.UI.Widgets
{
  /// <summary>
  ///  An invisible componenent that can receive the keyboard focus and can
  ///  receive input events. Use this to control how keyboard and mouse inputs 
  ///  are processed by your game.
  /// 
  ///  This widget will layout nodes but does not draw itself or any child nodes
  ///  that have been added.
  /// </summary>
  public class InputPlaceholder: Widget
  {
    public InputPlaceholder(IUIStyle style) : base(style)
    {
      Focusable = true;
    }

    protected override void DrawWidget(IBatchedDrawingService drawingService)
    {
    }

    protected override void DrawChildren(IBatchedDrawingService drawingService)
    {
    }
  }
}