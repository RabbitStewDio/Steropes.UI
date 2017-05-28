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

namespace Steropes.UI.Widgets.Container
{
  /// <summary>
  ///   A widget to lay out a bunch of child widgets. The children will use their AnchorRect property as constraints. The
  ///   bool constraint type has no meaning and exists simply as an implementation artefact.
  /// </summary>
  public class Group : WidgetContainer<bool>
  {
    public Group(IUIStyle style) : base(style)
    {
    }

    protected override Rectangle ArrangeOverride(Rectangle layoutSize)
    {
      for (var i = 0; i < this.Count; i++)
      {
        var widget = this[i];
        if (widget.Visibility == Visibility.Collapsed)
        {
          continue;
        }

        var childRect = widget.ArrangeChild(layoutSize);
        widget.Arrange(childRect);
      }
      return layoutSize;
    }

    protected override Size MeasureOverride(Size availableSize)
    {
      var contentHeight = 0;
      var contentWidth = 0;
      for (var i = 0; i < this.Count; i++)
      {
        var widget = this[i];
        if (widget.Visibility == Visibility.Collapsed)
        {
          continue;
        }

        var size = widget.MeasureAsAnchoredChild(availableSize);

        contentHeight = (int)Math.Max(contentHeight, size.Height);
        contentWidth = (int)Math.Max(contentWidth, size.Width);
      }
      return new Size(contentWidth, contentHeight);
    }
  }
}