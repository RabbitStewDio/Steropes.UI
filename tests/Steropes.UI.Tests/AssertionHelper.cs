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
using System;
using System.Runtime.CompilerServices;

using FluentAssertions.Execution;

using Microsoft.Xna.Framework;

using Steropes.UI.Components;
using Steropes.UI.Input;
using Steropes.UI.Input.MouseInput;

namespace Steropes.UI.Test
{
  public static class AssertionHelper
  {
    public static object AsObject(this object o)
    {
      return o;
    }

    public static MouseEventArgs CreateMouseEvent(this IWidget widget, MouseEventType type, MouseButton button, int x, int y, InputFlags flags = InputFlags.None)
    {
      switch (button)
      {
        case MouseButton.Left:
          flags |= InputFlags.Mouse1;
          break;
        case MouseButton.Right:
          flags |= InputFlags.Mouse3;
          break;
        case MouseButton.Middle:
          flags |= InputFlags.Mouse2;
          break;
      }

      return new MouseEventArgs(widget, new MouseEventData(type, flags, TimeSpan.FromMilliseconds(10), 0, new Point(x, y), button));
    }

    public static MouseEventArgs DispatchMouseClick(this IWidget widget, MouseButton button, int x, int y, InputFlags flags = InputFlags.None)
    {
      var eventArgs = CreateMouseEvent(widget, MouseEventType.Clicked, button, x, y, flags);
      widget.DispatchEvent(eventArgs);
      return eventArgs;
    }

    public static MouseEventArgs DispatchMouseDown(this IWidget widget, MouseButton button, int x, int y, InputFlags flags = InputFlags.None)
    {
      var eventArgs = CreateMouseEvent(widget, MouseEventType.Down, button, x, y, flags);
      widget.DispatchEvent(eventArgs);
      return eventArgs;
    }

    public static MouseEventArgs DispatchMouseDrag(this IWidget widget, MouseButton button, int x, int y, InputFlags flags = InputFlags.None)
    {
      var eventArgs = CreateMouseEvent(widget, MouseEventType.Dragged, button, x, y, flags);
      widget.DispatchEvent(eventArgs);
      return eventArgs;
    }

    public static MouseEventArgs DispatchMouseMove(this IWidget widget, MouseButton button, int x, int y, InputFlags flags = InputFlags.None)
    {
      var eventArgs = CreateMouseEvent(widget, MouseEventType.Moved, button, x, y, flags);
      widget.DispatchEvent(eventArgs);
      return eventArgs;
    }

    public static MouseEventArgs DispatchMouseUp(this IWidget widget, MouseButton button, int x, int y, InputFlags flags = InputFlags.None)
    {
      var eventArgs = CreateMouseEvent(widget, MouseEventType.Up, button, x, y, flags);
      widget.DispatchEvent(eventArgs);
      return eventArgs;
    }

    public static void ShouldBeSameObjectReference(this object o, object other)
    {
      if (!ReferenceEquals(o, other))
      {
        var hc1 = RuntimeHelpers.GetHashCode(o);
        var hc2 = RuntimeHelpers.GetHashCode(other);
        throw new AssertionFailedException($"{o} #({hc1}) is not the same reference as {other} (#{hc2})");
      }
    }
  }
}