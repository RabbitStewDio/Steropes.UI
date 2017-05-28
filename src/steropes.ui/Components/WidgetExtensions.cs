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
using System.ComponentModel;

using Microsoft.Xna.Framework;

using Steropes.UI.Components.Window;
using Steropes.UI.Input;
using Steropes.UI.Platform;

namespace Steropes.UI.Components
{
  public static class WidgetExtensions
  {
    public static Rectangle ArrangeChild(this IWidget w, Rectangle layoutSize)
    {
      Rectangle childRectangle;

      var anchoredRect = w.Anchor;
      var desiredSize = w.DesiredSize;

      // Horizontal
      if (anchoredRect.Left.HasValue)
      {
        childRectangle.X = layoutSize.Left + anchoredRect.Left.Value;
        if (anchoredRect.Right.HasValue)
        {
          // Horizontally anchored
          childRectangle.Width = Math.Max(0, layoutSize.Right - anchoredRect.Right.Value - childRectangle.X);
        }
        else
        {
          // Left-anchored
          childRectangle.Width = anchoredRect.Width.GetValueOrDefault(desiredSize.WidthInt);
        }
      }
      else
      {
        childRectangle.Width = anchoredRect.Width.GetValueOrDefault(desiredSize.WidthInt);

        if (anchoredRect.Right.HasValue)
        {
          // Right-anchored
          childRectangle.X = layoutSize.Right - anchoredRect.Right.Value - childRectangle.Width;
        }
        else
        {
          // Centered
          childRectangle.X = layoutSize.Center.X - childRectangle.Width / 2;
        }
      }

      // Vertical
      if (anchoredRect.Top.HasValue)
      {
        childRectangle.Y = layoutSize.Top + anchoredRect.Top.Value;
        if (anchoredRect.Bottom.HasValue)
        {
          // Horizontally anchored
          childRectangle.Height = Math.Max(0, layoutSize.Bottom - anchoredRect.Bottom.Value - childRectangle.Y);
        }
        else
        {
          // Top-anchored
          childRectangle.Height = anchoredRect.Height.GetValueOrDefault(desiredSize.HeightInt);
        }
      }
      else
      {
        childRectangle.Height = anchoredRect.Height.GetValueOrDefault(desiredSize.HeightInt);

        if (anchoredRect.Bottom.HasValue)
        {
          // Bottom-anchored
          childRectangle.Y = layoutSize.Bottom - anchoredRect.Bottom.Value - childRectangle.Height;
        }
        else
        {
          // Centered
          childRectangle.Y = layoutSize.Center.Y - childRectangle.Height / 2;
        }
      }

      return childRectangle;
    }

    public static void DrawClipped(this IWidget w, IBatchedDrawingService ds)
    {
      ds.PushScissorRectangle(w.LayoutRect);
      try
      {
        w.Draw(ds);
      }
      finally
      {
        ds.PopScissorRectangle();
      }
    }

    public static PropertyChangedEventHandler FilterBy(string propertyName, PropertyChangedEventHandler other)
    {
      return (source, args) =>
        {
          if (propertyName == args.PropertyName)
          {
            other(source, args);
          }
        };
    }

    public static IWidget FindChildForLocation(this IWidget widget, Point p)
    {
      var zIndex = 0;
      IWidget retval = null;
      for (var i = 0; i < widget.Count; i++)
      {
        var w = widget[i];
        if (!w.Enabled)
        {
          continue;
        }
        if (w.Visibility != Visibility.Visible)
        {
          continue;
        }
        if (!w.BorderRect.Contains(p))
        {
          continue;
        }

        var zIndexForChild = widget.GetDrawOrderForChild(w);
        if (retval == null || zIndex < zIndexForChild)
        {
          retval = w;
          zIndex = zIndexForChild;
        }
      }
      return retval;
    }

    public static void FocusNext(this IWidget w)
    {
      var widgetRight = w.GetSibling(Direction.Right, w)?.GetFirstFocusableDescendant(Direction.Right);
      if (widgetRight != null)
      {
        widgetRight.RequestFocus();
        return;
      }
      var widgetDown = w.GetSibling(Direction.Down, w)?.GetFirstFocusableDescendant(Direction.Down);
      widgetDown?.RequestFocus();
    }

    public static void FocusPrevious(this IWidget w)
    {
      var widgetRight = w.GetSibling(Direction.Left, w)?.GetFirstFocusableDescendant(Direction.Left);
      if (widgetRight != null)
      {
        widgetRight.RequestFocus();
        return;
      }
      var widgetDown = w.GetSibling(Direction.Up, w)?.GetFirstFocusableDescendant(Direction.Up);
      widgetDown?.RequestFocus();
    }

    public static bool IsOrphan(this IWidget w)
    {
      var widget = w;
      while (widget.Parent != null)
      {
        if (widget.Parent is IRootPane)
        {
          return false;
        }
        widget = widget.Parent;
      }
      return true;
    }

    public static float LimitSpace(float size, float consumedSpace)
    {
      if (float.IsPositiveInfinity(size))
      {
        return size;
      }
      return Math.Max(0, size - consumedSpace);
    }

    public static Size MeasureAsAnchoredChild(this IWidget child, Size layoutSize)
    {
      child.Measure(layoutSize);
      return child.Anchor.ResolveSize(child.DesiredSize);
    }

    public static IWidget PerformHitTest(this IWidget root, Point p)
    {
      if (!root.BorderRect.Contains(p))
      {
        return null;
      }

      var parent = root;
      while (true)
      {
        var child = parent.FindChildForLocation(p);
        if (child != null)
        {
          parent = child;
        }
        else
        {
          return parent;
        }
      }
    }

    public static void RaiseInputEvent<T>(this EventHandler<T> events, object source, T args) where T : InputEventArgs
    {
      if (events == null)
      {
        return;
      }

      var invocationList = events.GetInvocationList();
      for (var index = invocationList.Length - 1; index >= 0; index -= 0)
      {
        var d = invocationList[index];
        var evt = d as EventHandler<T>;
        evt?.Invoke(source, args);
      }
    }

    public static void RequestFocus(this IWidget w)
    {
      var fm = w.Screen?.FocusManager;
      if (fm != null)
      {
        fm.FocusedWidget = w;
      }
    }
  }
}