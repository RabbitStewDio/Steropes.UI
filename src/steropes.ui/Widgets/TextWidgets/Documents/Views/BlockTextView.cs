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
using Steropes.UI.Styles;

namespace Steropes.UI.Widgets.TextWidgets.Documents.Views
{
  public class BlockTextView<TDocument> : BranchTextView<TDocument>
    where TDocument : ITextDocument
  {
    public BlockTextView(ITextNode node, IStyle style) : base(node, style)
    {
    }

    public int Spacing { get; set; }

    public override NavigationResult Navigate(int editOffset, Direction direction, out int targetOffset)
    {
      switch (direction)
      {
        case Direction.Right:
        case Direction.Left:
          {
            return base.Navigate(editOffset, direction, out targetOffset);
          }
        case Direction.Down:
        case Direction.Up:
          {
            return this.NavigateVerticalFlat(editOffset, direction, out targetOffset);
          }
        default:
          {
            throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
          }
      }
    }

    protected override Rectangle ArrangeOverride(Rectangle layoutSize)
    {
      var cellStart = new Point(layoutSize.X, layoutSize.Y);
      var usedSpace = 0;
      for (var index = 0; index < Count; index++)
      {
        var widget = this[index];
        var widgetSize = widget.DesiredSize;

        var widgetRect = new Rectangle(cellStart.X, cellStart.Y, layoutSize.Width, widgetSize.HeightInt);
        widget.Arrange(widgetRect);

        usedSpace = Math.Max(usedSpace, widgetRect.Bottom);
        cellStart.Y = widgetRect.Bottom + Spacing;
      }
      return new Rectangle(layoutSize.X, layoutSize.Y, layoutSize.Width, usedSpace - layoutSize.Y);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
      var contentWidth = 0;
      var contentHeight = 0;
      for (var i = 0; i < Count; i += 1)
      {
        var child = this[i];
        child.Measure(availableSize);
        var size = child.DesiredSize;
        contentHeight += size.HeightInt;
        contentWidth = Math.Max(contentWidth, size.WidthInt);
      }

      if (Count == 0)
      {
        contentHeight = GetFontHeight();
      }
      else if (Count > 1)
      {
        contentHeight += Spacing * (Count - 1);
      }
      return new Size(contentWidth, contentHeight);
    }

  }
}