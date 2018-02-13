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
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Steropes.UI.Components;
using Steropes.UI.Platform;

namespace Steropes.UI.Widgets.Container
{
  public abstract class WidgetContainerBase<TConstraint> : Widget
  {
    readonly List<WidgetAndConstraint<TConstraint>> children;

    protected WidgetContainerBase(IUIStyle style) : base(style)
    {
      children = new List<WidgetAndConstraint<TConstraint>>();
      WidgetsWithConstraints = children.AsReadOnly();
    }

    public override int Count => children.Count;

    public override IWidget this[int index] => children[index].Widget;

    public void ClearImpl()
    {
      var count = children.Count - 1;
      for (var idx = count; idx >= 0; idx -= 1)
      {
        RemoveImpl(idx);
      }
    }

    public override int GetDrawOrderForChild(IWidget w)
    {
      return IndexOf(w);
    }

    public override IWidget GetFirstFocusableDescendant(Direction direction)
    {
      IWidget firstChild = null;
      IWidget focusableDescendant = null;

      for (var i = 0; i < this.Count; i++)
      {
        var child = this[i];
        if (child.Enabled == false || child.Visibility != Visibility.Visible)
        {
          continue;
        }

        switch (direction)
        {
          case Direction.Up:
            if (firstChild == null || child.BorderRect.Bottom > firstChild.BorderRect.Bottom)
            {
              var childFocusableWidget = child.GetFirstFocusableDescendant(direction);
              if (childFocusableWidget != null)
              {
                firstChild = child;
                focusableDescendant = childFocusableWidget;
              }
            }

            break;
          case Direction.Down:
            if (firstChild == null || child.BorderRect.Top < firstChild.BorderRect.Top)
            {
              var childFocusableWidget = child.GetFirstFocusableDescendant(direction);
              if (childFocusableWidget != null)
              {
                firstChild = child;
                focusableDescendant = childFocusableWidget;
              }
            }

            break;
          case Direction.Left:
            if (firstChild == null || child.BorderRect.Right > firstChild.BorderRect.Right)
            {
              var childFocusableWidget = child.GetFirstFocusableDescendant(direction);
              if (childFocusableWidget != null)
              {
                firstChild = child;
                focusableDescendant = childFocusableWidget;
              }
            }

            break;
          case Direction.Right:
            if (firstChild == null || child.BorderRect.Left < firstChild.BorderRect.Left)
            {
              var childFocusableWidget = child.GetFirstFocusableDescendant(direction);
              if (childFocusableWidget != null)
              {
                firstChild = child;
                focusableDescendant = childFocusableWidget;
              }
            }

            break;
        }
      }

      return focusableDescendant;
    }

    public override IWidget GetSibling(Direction direction, IWidget sourceWidget)
    {
      IWidget nearestSibling = null;
      IWidget focusableSibling = null;

      var fixedChild = sourceWidget;

      for (var i = 0; i < this.Count; i++)
      {
        var child = this[i];
        if (child == sourceWidget)
        {
          continue;
        }

        if (child.Enabled == false || child.Visibility != Visibility.Visible)
        {
          continue;
        }

        switch (direction)
        {
          case Direction.Up:
            if (child.BorderRect.Bottom <= fixedChild.BorderRect.Center.Y &&
                (nearestSibling == null || child.BorderRect.Bottom > nearestSibling.BorderRect.Bottom))
            {
              var childFocusableWidget = child.GetFirstFocusableDescendant(direction);
              if (childFocusableWidget != null)
              {
                nearestSibling = child;
                focusableSibling = childFocusableWidget;
              }
            }

            break;
          case Direction.Down:
            if (child.BorderRect.Top >= fixedChild.BorderRect.Center.Y &&
                (nearestSibling == null || child.BorderRect.Top < nearestSibling.BorderRect.Top))
            {
              var childFocusableWidget = child.GetFirstFocusableDescendant(direction);
              if (childFocusableWidget != null)
              {
                nearestSibling = child;
                focusableSibling = childFocusableWidget;
              }
            }

            break;
          case Direction.Left:
            if (child.BorderRect.Right <= fixedChild.BorderRect.Center.X &&
                (nearestSibling == null || child.BorderRect.Right > nearestSibling.BorderRect.Right))
            {
              var childFocusableWidget = child.GetFirstFocusableDescendant(direction);
              if (childFocusableWidget != null)
              {
                nearestSibling = child;
                focusableSibling = childFocusableWidget;
              }
            }

            break;
          case Direction.Right:
            if (child.BorderRect.Left >= fixedChild.BorderRect.Center.X &&
                (nearestSibling == null || child.BorderRect.Left < nearestSibling.BorderRect.Left))
            {
              var childFocusableWidget = child.GetFirstFocusableDescendant(direction);
              if (childFocusableWidget != null)
              {
                nearestSibling = child;
                focusableSibling = childFocusableWidget;
              }
            }

            break;
        }
      }

      if (focusableSibling == null)
      {
        return base.GetSibling(direction, this);
      }

      return focusableSibling;
    }

    public int IndexOf(IWidget w)
    {
      return children.FindIndex(wc => ReferenceEquals(wc.Widget, w));
    }

    public override void Update(GameTime elapsedTime)
    {
      for (var i = 0; i < this.Count; i++)
      {
        var widget = this[i];
        widget.Update(elapsedTime);
      }

      base.Update(elapsedTime);
    }

    protected virtual void AddImpl(IWidget w, int index, TConstraint constraint)
    {
      if (w.Parent != null)
      {
        throw new ArgumentException();
      }

      var item = new WidgetAndConstraint<TConstraint>(w, constraint);
      children.Insert(index, item);
      w.AddNotify(this);
      OnChildAdded(w, index, constraint);
      RaiseChildAdded(index, w, constraint);
      InvalidateLayout();
    }

    protected override void DrawChildren(IBatchedDrawingService drawingService)
    {
      for (var i = 0; i < this.Count; i++)
      {
        var child = this[i];
        if (child.Visibility == Visibility.Visible)
        {
          child.Draw(drawingService);
        }
      }
    }

    protected TConstraint GetContraintAt(int idx)
    {
      if (idx == -1)
      {
        throw new ArgumentException();
      }

      return children[idx].Constraint;
    }

    protected TConstraint GetContraintFor(IWidget w)
    {
      var idx = IndexOf(w);
      if (idx == -1)
      {
        throw new ArgumentException();
      }

      return children[idx].Constraint;
    }

    protected virtual void OnChildAdded(IWidget w, int index, TConstraint constraint)
    {
    }

    protected virtual void OnChildRemoved(IWidget w, int index, TConstraint constraint)
    {
    }

    protected virtual void RemoveImpl(int index)
    {
      var w = children[index];
      w.Widget.RemoveNotify(this);
      children.RemoveAt(index);
      OnChildRemoved(w.Widget, index, w.Constraint);
      RaiseChildRemoved(index, w.Widget, w.Constraint);
      InvalidateLayout();
    }

    public IReadOnlyList<WidgetAndConstraint<TConstraint>> WidgetsWithConstraints { get; }
  }

  public struct WidgetAndConstraint<TConstraint> : IEquatable<WidgetAndConstraint<TConstraint>>
  {
    public IWidget Widget { get; }

    public TConstraint Constraint { get; }

    public WidgetAndConstraint(IWidget widget, TConstraint constraint)
    {
      Widget = widget;
      Constraint = constraint;
    }

    public override string ToString()
    {
      return $"(Constraint: {Constraint}, Widget: {Widget})";
    }

    public bool Equals(WidgetAndConstraint<TConstraint> other)
    {
      return Equals(Widget, other.Widget);
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      return obj is WidgetAndConstraint<TConstraint> && Equals((WidgetAndConstraint<TConstraint>) obj);
    }

    public override int GetHashCode()
    {
      return (Widget != null ? Widget.GetHashCode() : 0);
    }

    public static bool operator ==(WidgetAndConstraint<TConstraint> left, WidgetAndConstraint<TConstraint> right)
    {
      return left.Equals(right);
    }

    public static bool operator !=(WidgetAndConstraint<TConstraint> left, WidgetAndConstraint<TConstraint> right)
    {
      return !left.Equals(right);
    }
  }
}