// MIT License
// Copyright (c) 2011-2016 Elisée Maurer, Sparklin Labs, Creative Patterns
// Copyright (c) 2016 Thomas Morgner, Rabbit-StewDio Ltd.
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
using Microsoft.Xna.Framework;

using Steropes.UI.Input;
using Steropes.UI.Input.MouseInput;

namespace Steropes.UI.Components.Window.Events
{
  public class MouseToComponentMapper : EventFilterBase<MouseEventData>, IComponentEventSource<MouseEventData, IWidget>
  {
    readonly EventQueue<MouseEventData> eventQueue;

    readonly IMouseHoverManager hoverManager;

    readonly IWidget rootWidget;

    readonly IEventSource<MouseEventData> source;

    Point lastPosition;

    IWidget tempWidget;

    public MouseToComponentMapper(IEventSource<MouseEventData> source, IMouseHoverManager hoverManager, IWidget rootWidget)
    {
      eventQueue = new EventQueue<MouseEventData>();
      this.source = source;
      this.hoverManager = hoverManager;
      this.rootWidget = rootWidget;
    }

    public IWidget Component => hoverManager.HoveredWidget;

    protected override IEventSource<MouseEventData> Source => source;

    public override bool PullEventData(out MouseEventData data)
    {
      if (eventQueue.PullEventData(out data))
      {
        if (tempWidget != null)
        {
          hoverManager.HoveredWidget = tempWidget;
          tempWidget = null;
        }
        return true;
      }

      if (!source.PullEventData(out data))
      {
        return false;
      }

      // if any mouse button is down, the move operation becomes a drag operation. During a drag, 
      // we preserve the current widget context, even if the drag moves outside of the current 
      // component.
      if (data.Flags.AnyMouseButton() && data.EventType == MouseEventType.Moved)
      {
        data = data.ConvertInto(MouseEventType.Dragged, data.ClickCount);
      }

      var w = tempWidget ?? hoverManager.HoveredWidget;
      tempWidget = null;
      var haveWidget = w != null;

      // as long as any button is pressed, we are treating this as a drag operation and thus do not 
      // change the receiving component.
      if (!haveWidget || !data.Flags.AnyMouseButton())
      {
        var pos = data.Position;
        if (lastPosition != pos)
        {
          var target = rootWidget.PerformHitTest(pos);
          lastPosition = pos;
          if (ReferenceEquals(target, w))
          {
            // no change ..
            if (!ReferenceEquals(w, hoverManager.HoveredWidget))
            {
              hoverManager.HoveredWidget = w;
            }
          }
          else
          {
            if (target != null && w != null)
            {
              tempWidget = target;

              eventQueue.PushEvent(data.ConvertInto(MouseEventType.Entered));
              eventQueue.PushEvent(data);

              hoverManager.HoveredWidget = w;
              data = data.ConvertInto(MouseEventType.Exited);
            }
            else if (target != null)
            {
              hoverManager.HoveredWidget = target;

              eventQueue.PushEvent(data);
              data = data.ConvertInto(MouseEventType.Entered);
            }
            else
            {
              tempWidget = rootWidget;

              hoverManager.HoveredWidget = w;
              eventQueue.PushEvent(data);
              data = data.ConvertInto(MouseEventType.Exited);
            }
          }
        }
      }
      else
      {
        if (!ReferenceEquals(w, hoverManager.HoveredWidget))
        {
          hoverManager.HoveredWidget = w;
        }
      }

      return true;
    }
  }
}