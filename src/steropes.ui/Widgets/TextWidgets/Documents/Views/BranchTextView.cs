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
using Steropes.UI.Styles;

namespace Steropes.UI.Widgets.TextWidgets.Documents.Views
{
  public abstract class BranchTextView<TDocument> : TextView<TDocument>
    where TDocument : ITextDocument
  {
    readonly List<ITextView<TDocument>> childViews;

    protected BranchTextView(ITextNode node, IStyle style) : base(node, style)
    {
      childViews = new List<ITextView<TDocument>>();
    }

    public override int Count => childViews.Count;

    public override ITextView<TDocument> this[int index] => childViews[index];

    public void Add(ITextView<TDocument> view)
    {
      Insert(Count, view);
    }

    public sealed override void Draw(IBatchedDrawingService drawingService)
    {
      DrawBox(drawingService);
      for (var i = 0; i < Count; i += 1)
      {
        var child = this[i];
        child.Draw(drawingService);
      }
      EndDrawBox(drawingService);
    }

    public void Insert(int index, ITextView<TDocument> view)
    {
      if (view == null)
      {
        throw new ArgumentNullException(nameof(view));
      }

      childViews.Insert(index, view);
      view.AddNotify(this);
      OnViewInserted(index, view);
    }

    public override bool ModelToView(int offset, out Rectangle result)
    {
      if (LayoutInvalid)
      {
        result = default(Rectangle);
        return false;
      }
      if (!Node.InEndOffsetRange(offset))
      {
        result = default(Rectangle);
        return false;
      }

      result = LayoutRect;

      for (var i = 0; i < Count; i += 1)
      {
        var child = this[i];
        if (child.InRange(offset))
        {
          return child.ModelToView(offset, out result);
        }
        if (child.InEndOffsetRange(offset))
        {
          // produce a potentially temporary result to handle a cursor places at
          // the end of the document.
          child.ModelToView(offset, out result);
        }
      }

      return true;
    }

    public override NavigationResult Navigate(int editOffset, Direction direction, out int targetOffset)
    {
      if (LayoutInvalid)
      {
        targetOffset = -1;
        return NavigationResult.Invalid;
      }

      switch (direction)
      {
        case Direction.Right:
        case Direction.Down:
          {
            for (var i = 0; i < Count; i += 1)
            {
              var child = this[i];
              if (editOffset > child.EndOffset)
              {
                continue;
              }
              var navResult = child.Navigate(editOffset, direction, out targetOffset);
              switch (navResult)
              {
                case NavigationResult.Valid:
                  {
                    return NavigationResult.Valid;
                  }
                case NavigationResult.BoundaryChanged:
                  {
                    if (editOffset != targetOffset)
                    {
                      return NavigationResult.Valid;
                    }
                    break;
                  }
                case NavigationResult.Invalid:
                  {
                    break;
                  }
                default:
                  {
                    throw new ArgumentOutOfRangeException();
                  }
              }
            }

            targetOffset = EndOffset;
            return NavigationResult.BoundaryChanged;
          }
        case Direction.Left:
        case Direction.Up:
          {
            for (var i = Count - 1; i >= 0; i -= 1)
            {
              var child = this[i];
              if (editOffset < child.Offset)
              {
                continue;
              }
              var navResult = child.Navigate(editOffset, direction, out targetOffset);
              switch (navResult)
              {
                case NavigationResult.Valid:
                  {
                    return NavigationResult.Valid;
                  }
                case NavigationResult.BoundaryChanged:
                  {
                    if (editOffset != targetOffset)
                    {
                      return NavigationResult.Valid;
                    }

                    // try the sibling element ..
                    break;
                  }
                case NavigationResult.Invalid:
                  {
                    break;
                  }
                default:
                  {
                    throw new ArgumentOutOfRangeException();
                  }
              }
            }
            targetOffset = Offset;
            return NavigationResult.BoundaryChanged;
          }
        default:
          {
            throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
          }
      }
    }

    public override void OnNodeStructureChanged(IDocumentView<TDocument> docView, IElementEdit edit)
    {
      base.OnNodeStructureChanged(docView, edit);

      for (var index = 0; index < edit.RemovedNodes.Length; index++)
      {
        var removedNode = edit.RemovedNodes[index];
        var view = childViews[edit.Index];
        if (!ReferenceEquals(view.Node, removedNode))
        {
          // pure paranoia ..
          throw new InvalidOperationException();
        }
        Remove(edit.Index);
      }

      var insertIndex = edit.Index;
      for (var index = 0; index < edit.AddedNodes.Length; index++)
      {
        var addedNode = edit.AddedNodes[index];
        var view = docView.ViewFactory.CreateFor(addedNode, Style);
        Insert(insertIndex, view);
        insertIndex += 1;
      }
    }

    public void Remove(int index)
    {
      var childView = childViews[index];

      OnViewRemoved(index, childView);
      childView.RemoveNotify(this);
      childViews.RemoveAt(index);
    }

    public override bool ViewToModel(Point position, out int offset, out Bias bias)
    {
      if (!LayoutRect.Contains(position))
      {
        offset = -1;
        bias = Bias.Forward;
        return false;
      }

      for (var i = 0; i < Count; i += 1)
      {
        var child = this[i];
        if (child.ViewToModel(position, out offset, out bias))
        {
          return true;
        }
      }

      offset = -1;
      bias = Bias.Forward;
      return false;
    }

    protected void Clear()
    {
      for (var i = childViews.Count - 1; i >= 0; i--)
      {
        Remove(i);
      }
    }

    protected virtual void DrawBox(IBatchedDrawingService drawingService)
    {
    }

    protected virtual void EndDrawBox(IBatchedDrawingService drawingService)
    {
    }

    protected virtual void OnViewInserted(int index, ITextView<TDocument> view)
    {
    }

    protected virtual void OnViewRemoved(int index, ITextView<TDocument> view)
    {
    }
  }
}