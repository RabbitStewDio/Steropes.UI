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
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Steropes.UI.Input;
using Steropes.UI.Input.TouchInput;

namespace Steropes.UI.Components.Window.Events
{
  public class TouchToComponentMapper : IComponentEventSource<TouchEventData, IWidget>
  {
    readonly IMouseHoverManager hoverManager;

    readonly IWidget rootWidget;

    readonly IEventSource<TouchEventData> source;

    readonly HashSet<int> touchDownById;

    Point lastPosition;

    bool layoutInvalid;

    public TouchToComponentMapper(IEventSource<TouchEventData> source, IMouseHoverManager hoverManager, IWidget rootWidget)
    {
      this.source = source;
      this.hoverManager = hoverManager;
      this.rootWidget = rootWidget;
      this.rootWidget.LayoutInvalidated += (sender, args) => layoutInvalid = true;

      touchDownById = new HashSet<int>();
      layoutInvalid = true;
    }

    public IWidget Component
    {
      get
      {
        return hoverManager.HoveredWidget;
      }
      private set
      {
        hoverManager.HoveredWidget = value;
      }
    }

    public bool PullEventData(out TouchEventData data)
    {
      if (!source.PullEventData(out data))
      {
        return false;
      }

      switch (data.EventType)
      {
        case TouchEventType.Gestured:
          {
            if (IsAnyTouchActive())
            {
              return true;
            }

            var pos = data.Position;
            UpdateCurrentWidget(pos);
            break;
          }
        case TouchEventType.Pressed:
        case TouchEventType.Moved:
          {
            if (!IsAnyTouchActive())
            {
              UpdateCurrentWidget(data.Position);
            }
            touchDownById.Add(data.TouchLocation.Id);
            break;
          }
        case TouchEventType.Cancelled:
        case TouchEventType.Released:
          {
            if (!IsAnyTouchActive())
            {
              UpdateCurrentWidget(data.Position);
            }
            touchDownById.Remove(data.TouchLocation.Id);
            break;
          }
        default:
          {
            break;
          }
      }
      return true;
    }

    bool IsAnyTouchActive()
    {
      return touchDownById.Count > 0;
    }

    void UpdateCurrentWidget(Point pos)
    {
      if (layoutInvalid || lastPosition != pos)
      {
        Component = rootWidget.PerformHitTest(pos);
        lastPosition = pos;
        layoutInvalid = false;
      }
    }
  }
}