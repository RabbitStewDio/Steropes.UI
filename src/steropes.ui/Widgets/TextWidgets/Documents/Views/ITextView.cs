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

using Microsoft.Xna.Framework;

using Steropes.UI.Components;

namespace Steropes.UI.Widgets.TextWidgets.Documents.Views
{
  public interface ITextView<TDocument> : IVisualContent
    where TDocument : ITextDocument
  {
    int Count { get; }

    int EndOffset { get; }

    ITextNode Node { get; }

    int Offset { get; }

    ITextView<TDocument> this[int index] { get; }

    void AddNotify(ITextView<TDocument> parent);

    /// <summary>
    ///   Attempts to find the rendered position for a given document offset.
    ///   <p />
    ///   If the offset points to a non-visible element (eg the header-portion of an HTML element),
    ///   this method will return false, and result will not have a valid value.
    /// </summary>
    /// <param name="offset"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    bool ModelToView(int offset, out Rectangle result);

    /// <summary>
    ///   Attempts to move the given edit-position in the direction specified by "Bias".
    ///   This can only work if the edit-offset is actually covered by the element's node.
    ///   <p />
    ///   Possible outcomes:
    ///   * Target position is still wthin the view.
    ///   * Target position is before the view's active range.
    ///   * Target position is after the view's active range.
    ///   If the target is still within the view, accept it as a result.
    ///   If outside, take the returned offset and query the nodes before or after this node.
    /// </summary>
    /// <param name="editOffset"></param>
    /// <param name="direction"></param>
    /// <param name="targetOffset"></param>
    /// <returns></returns>
    NavigationResult Navigate(int editOffset, Direction direction, out int targetOffset);

    void OnNodeContentChanged(IDocumentView<TDocument> docView, TextModificationEventArgs args);

    void OnNodeStructureChanged(IDocumentView<TDocument> docView, IElementEdit edit);

    void RemoveNotify(ITextView<TDocument> parent);

    /// <summary>
    ///   Attempts to find the document offset at the given point. This method is only
    ///   valid after the view has finished layouting. If the position given is not
    ///   in range of the rendered area of the document, this method will return false.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="offset"></param>
    /// <param name="bias"></param>
    /// <returns></returns>
    bool ViewToModel(Point position, out int offset, out Bias bias);
  }

  public static class TextViewExtensions
  {
    public static bool InEditRange<T>(this ITextView<T> view, int editOffset, int editEndOffset) where T : ITextDocument
    {
      if (editOffset > view.EndOffset)
      {
        return false;
      }
      if (editEndOffset < view.Offset)
      {
        return false;
      }
      return true;
    }

    public static bool InEndOffsetRange<T>(this ITextView<T> view, int offset) where T : ITextDocument
    {
      if (offset < view.Offset || offset > view.EndOffset)
      {
        return false;
      }
      return true;
    }

    public static bool InRange<T>(this ITextView<T> view, int offset) where T : ITextDocument
    {
      if (offset < view.Offset || offset >= view.EndOffset)
      {
        return false;
      }
      return true;
    }

    public static NavigationResult NavigateVerticalFlat<T>(this ITextView<T> view, int editOffset, Direction direction, out int targetOffset) where T : ITextDocument
    {
      if (direction != Direction.Up && direction != Direction.Down)
      {
        throw new ArgumentException();
      }

      // find current line ..
      Rectangle result;
      if (!view.ModelToView(editOffset, out result))
      {
        // cannot navigate if the offset is invalid.
        targetOffset = -1;
        return NavigationResult.Invalid;
      }

      var targetX = result.X;
      var existingTopY = result.Top;
      var existingBottomY = result.Bottom;

      // now find next line. 
      // nav direction
      var navDirection = direction == Direction.Down ? Direction.Right : Direction.Left;

      var nextLineResult = FindNextLineOffset(view, editOffset, navDirection, existingTopY, existingBottomY, out targetOffset);
      if (nextLineResult == NavigationResult.Invalid || nextLineResult == NavigationResult.BoundaryChanged)
      {
        return nextLineResult;
      }

      return FindNearestOffsetWithinLine(view, targetOffset, targetX, navDirection, out targetOffset);
    }

    static NavigationResult FindNearestOffsetWithinLine<T>(ITextView<T> view, int offset, int targetX, Direction navDirection, out int targetOffset) where T : ITextDocument
    {
      Rectangle result;
      if (!view.ModelToView(offset, out result))
      {
        targetOffset = offset;
        return NavigationResult.Invalid;
      }

      var currentX = Math.Abs(result.Left - targetX);
      var bestOffset = offset;

      while (true)
      {
        var innerNavResult = view.Navigate(offset, navDirection, out targetOffset);
        if (innerNavResult == NavigationResult.Invalid || innerNavResult == NavigationResult.BoundaryChanged)
        {
          if (currentX != int.MaxValue)
          {
            targetOffset = bestOffset;
            return NavigationResult.Valid;
          }
          return innerNavResult;
        }

        if (!view.ModelToView(targetOffset, out result))
        {
          return NavigationResult.Invalid;
        }

        var dist = Math.Abs(result.Left - targetX);
        if (dist < currentX)
        {
          bestOffset = targetOffset;
          currentX = dist;
        }
        else
        {
          targetOffset = bestOffset;
          return NavigationResult.Valid;
        }

        offset += navDirection == Direction.Left ? -1 : +1;
      }
    }

    static NavigationResult FindNextLineOffset<T>(ITextView<T> view, int offset, Direction navDirection, int lineTop, int lineBottom, out int targetOffset) where T : ITextDocument
    {
      while (true)
      {
        var innerNavResult = view.Navigate(offset, navDirection, out targetOffset);
        if (innerNavResult == NavigationResult.Invalid || innerNavResult == NavigationResult.BoundaryChanged)
        {
          return innerNavResult;
        }

        Rectangle result;
        if (!view.ModelToView(offset, out result))
        {
          return NavigationResult.Invalid;
        }

        if (!InRange(result.Top, result.Bottom, lineTop, lineBottom))
        {
          targetOffset = offset;
          return NavigationResult.Valid;
        }

        // assumed to be on the same line. Adjust line-box.
        lineTop = Math.Max(lineTop, result.Top);
        lineBottom = Math.Max(lineBottom, result.Bottom);
        offset += navDirection == Direction.Left ? -1 : +1;
      }
    }

    static bool InRange(int valueTop, int valueBottom, int lineMin, int lineMax)
    {
      if (valueTop >= lineMax)
      {
        return false;
      }
      if (valueBottom <= lineMin)
      {
        return false;
      }
      return true;
    }
  }
}