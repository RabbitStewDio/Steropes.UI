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
  public struct Insets : IEquatable<Insets>
  {
    public static readonly Insets Zero = new Insets();

    public int Top;

    public int Right;

    public int Bottom;

    public int Left;

    public int Horizontal => Left + Right;

    public int Vertical => Top + Bottom;

    public Insets(int top, int left, int bottom, int right)
    {
      Top = top;
      Right = right;
      Bottom = bottom;
      Left = left;
    }

    public Insets(int value)
    {
      Left = value;
      Right = value;
      Top = value;
      Bottom = value;
    }

    public Insets(int vertical, int horizontal)
    {
      Top = Bottom = vertical;
      Left = Right = horizontal;
    }

    public bool Equals(Insets other)
    {
      return Top == other.Top && Right == other.Right && Bottom == other.Bottom && Left == other.Left;
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj))
      {
        return false;
      }
      return obj is Insets && Equals((Insets)obj);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        var hashCode = Top;
        hashCode = (hashCode * 397) ^ Right;
        hashCode = (hashCode * 397) ^ Bottom;
        hashCode = (hashCode * 397) ^ Left;
        return hashCode;
      }
    }

    public static bool operator ==(Insets left, Insets right)
    {
      return left.Equals(right);
    }

    public static bool operator !=(Insets left, Insets right)
    {
      return !left.Equals(right);
    }

    public static Insets operator +(Insets left, Insets right)
    {
      return new Insets(left.Top + right.Top, left.Left + right.Left, left.Bottom + right.Bottom, left.Right + right.Right);
    }

    public override string ToString()
    {
      return $"{nameof(Top)}: {Top}, {nameof(Right)}: {Right}, {nameof(Bottom)}: {Bottom}, {nameof(Left)}: {Left}";
    }
  }

  public static class BoxExtensions
  {
    public static Rectangle IncreaseRectBy(this Rectangle layoutSize, Insets insets)
    {
      return new Rectangle(
        layoutSize.X - insets.Left,
        layoutSize.Y - insets.Top,
        Math.Max(0, layoutSize.Width + insets.Horizontal),
        Math.Max(0, layoutSize.Height + insets.Vertical));
    }

    public static Rectangle ReduceRectBy(this Rectangle layoutSize, Insets insets)
    {
      return new Rectangle(
        layoutSize.X + insets.Left,
        layoutSize.Y + insets.Top,
        Math.Max(0, layoutSize.Width - insets.Horizontal),
        Math.Max(0, layoutSize.Height - insets.Vertical));
    }
  }
}