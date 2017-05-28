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

namespace Steropes.UI.Components
{
  public struct Size : IEquatable<Size>
  {
    public float Width;

    public float Height;

    public Size(float availableWidth, float availableHeight) : this()
    {
      Width = availableWidth;
      Height = availableHeight;
    }

    public int WidthInt => (int)Math.Ceiling(Width);

    public int HeightInt => (int)Math.Ceiling(Height);

    public override string ToString()
    {
      return $"Size={{Width: {Width}, Height: {Height}}}";
    }

    public static Size Auto => new Size(float.PositiveInfinity, float.PositiveInfinity);

    public bool Equals(Size other)
    {
      return Width.Equals(other.Width) && Height.Equals(other.Height);
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj))
      {
        return false;
      }
      return obj is Size && Equals((Size)obj);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        return (Width.GetHashCode() * 397) ^ Height.GetHashCode();
      }
    }

    public static bool operator ==(Size left, Size right)
    {
      return left.Equals(right);
    }

    public static bool operator !=(Size left, Size right)
    {
      return !left.Equals(right);
    }

    public Size ReduceBy(float width, float height)
    {
      var availableWidth = float.IsPositiveInfinity(Width) ? Width : Math.Max(0, Width - width);
      var availableHeight = float.IsPositiveInfinity(Height) ? Height : Math.Max(0, Height - height);
      return new Size(availableWidth, availableHeight);
    }

    public Size IncreaseBy(float width, float height)
    {
      var availableWidth = float.IsPositiveInfinity(Width) ? Width : Math.Max(0, Width + width);
      var availableHeight = float.IsPositiveInfinity(Height) ? Height : Math.Max(0, Height + height);
      return new Size(availableWidth, availableHeight);
    }
  }
}