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
using Steropes.UI.Input.TouchInput;

namespace Steropes.UI.Components.Window.Events
{
  public class PopupClosingEventProcessor : IComponentEventSink<MouseEventData>, IComponentEventSink<TouchEventData>
  {
    readonly IPopupLocationSource root;

    public PopupClosingEventProcessor(IPopupLocationSource root)
    {
      this.root = root;
    }

    public void PushEvent(MouseEventData data, IWidget target)
    {
      if (data.EventType == MouseEventType.Clicked || data.EventType == MouseEventType.Down)
      {
        CheckPopUpClosing(data.Position);
      }
    }

    public void PushEvent(TouchEventData data, IWidget target)
    {
      if (data.EventType == TouchEventType.Gestured || data.EventType == TouchEventType.Pressed)
      {
        CheckPopUpClosing(data.Position);
      }
    }

    bool CheckPopUpClosing(Point p)
    {
      var closedAtLeastOnePopUp = false;
      IPopUp popup;
      while (root.QueryPopUpLayoutRect(out popup))
      {
        if (popup.BorderRect.Contains(p))
        {
          return false;
        }

        closedAtLeastOnePopUp = true;
        popup.Close();
        if (popup.Parent != null)
        {
          // if the popup refuses to go away, we cannot continue.
          return true;
        }
      }
      return closedAtLeastOnePopUp;
    }
  }
}