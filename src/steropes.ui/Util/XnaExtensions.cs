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

namespace Steropes.UI.Util
{
  public static class XnaExtensions
  {
    public static Rectangle Clip(this Rectangle rect1, Rectangle rect2)
    {
      var rect = new Rectangle(Math.Max(rect1.X, rect2.X), Math.Max(rect1.Y, rect2.Y), 0, 0);

      rect.Width = Math.Min(rect1.Right, rect2.Right) - rect.X;
      rect.Height = Math.Min(rect1.Bottom, rect2.Bottom) - rect.Y;

      if (rect.Width < 0)
      {
        rect.X += rect.Width;
        rect.Width = 0;
      }

      if (rect.Height < 0)
      {
        rect.Y += rect.Height;
        rect.Height = 0;
      }

      return rect;
    }

    public static Vector2 ToVector2(this Point point)
    {
      return new Vector2(point.X, point.Y);
    }

    public static Rectangle Union(this Rectangle r1, Rectangle r2)
    {
      var x = Math.Min(r1.Left, r2.Left);
      var y = Math.Min(r1.Top, r2.Top);
      return new Rectangle(x, y, Math.Max(r1.Right, r2.Right) - x, Math.Max(r1.Bottom, r2.Bottom) - y);
    }

    public static void CenterOnScreen(this Game game)
    {
      var size = game.Window.ClientBounds.Size;
      var dm = game.GraphicsDevice.Adapter.CurrentDisplayMode;
      game.Window.Position = new Point((dm.Width - size.X) / 2, (dm.Height - size.Y) / 2);
    }
  }
}