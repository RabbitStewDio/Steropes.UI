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

namespace Steropes.UI.Components.Helper
{
  public class ArrangerHorizontal
  {
    Rectangle layoutRect;

    public ArrangerHorizontal(Rectangle layoutRect)
    {
      this.layoutRect = layoutRect;
      AvailableWidth = layoutRect.Width;
    }

    public int AvailableWidth { get; set; }

    public ArrangerHorizontal Advance(int width)
    {
      return Reserve(width).AdvanceReserved(width);
    }

    public ArrangerHorizontal AdvanceReserved(int width)
    {
      var rect = layoutRect;
      rect.X += width;
      layoutRect = rect;
      return this;
    }

    public ArrangerHorizontal Arrange(IWidget w, int width)
    {
      w?.Arrange(new Rectangle(layoutRect.X, layoutRect.Y, width, layoutRect.Height));
      return this;
    }

    public ArrangerHorizontal Reserve(int width)
    {
      AvailableWidth -= width;
      if (AvailableWidth < 0)
      {
        AvailableWidth = 0;
      }
      return this;
    }
  }
}