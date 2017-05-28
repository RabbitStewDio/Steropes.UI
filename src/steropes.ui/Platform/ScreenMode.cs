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

namespace Steropes.UI.Platform
{
  /// <summary>
  ///   Describes a screen mode.
  /// </summary>
  public struct ScreenMode : IComparable<ScreenMode>
  {
    public ScreenMode(int width, int height, float aspectRatio)
    {
      Width = width;
      Height = height;
      AspectRatio = aspectRatio;
    }

    public override string ToString()
    {
      return Width + "x" + Height;
    }

    public int CompareTo(ScreenMode other)
    {
      var order = Width.CompareTo(other.Width);
      if (order == 0)
      {
        return Height.CompareTo(other.Height);
      }
      return order;
    }

    public int Width;

    public int Height;

    public float AspectRatio;

    public Vector2 Size => new Vector2(Width, Height);

    public Rectangle Rectangle => new Rectangle(0, 0, Width, Height);
  }
}