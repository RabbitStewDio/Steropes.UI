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
using System.Diagnostics.Contracts;

using Microsoft.Xna.Framework;

namespace Steropes.UI.Components
{
  public struct AnchoredRect : IEquatable<AnchoredRect>
  {
    public int? Left;

    public int? Top;

    public int? Right;

    public int? Bottom;

    public int? Width;

    public int? Height;

    public static readonly AnchoredRect Full = new AnchoredRect(0, 0, 0, 0, 0, 0);

    public bool HasWidth => (!Left.HasValue || !Right.HasValue) && Width.HasValue;

    public bool HasHeight => (!Top.HasValue || !Bottom.HasValue) && Height.HasValue;

    public AnchoredRect(int? left, int? top, int? right, int? bottom, int? width, int? height)
    {
      Left = left;
      Top = top;

      Right = right;
      Bottom = bottom;

      Width = width;
      Height = height;
    }

    public static AnchoredRect CreateFixed(int left, int top, int width, int height)
    {
      return new AnchoredRect(left, top, null, null, width, height);
    }

    public static AnchoredRect CreateFixed(Rectangle rect)
    {
      return new AnchoredRect(rect.Left, rect.Top, null, null, rect.Width, rect.Height);
    }

    public static AnchoredRect CreateFull(int value)
    {
      return new AnchoredRect(value, value, value, value, 0, 0);
    }

    public static AnchoredRect CreateFull(int left, int top, int right, int bottom)
    {
      return new AnchoredRect(left, top, right, bottom, 0, 0);
    }

    public static AnchoredRect CreateHorizontallyStretched(int left = 0, int right = 0)
    {
      return new AnchoredRect(left, null, right, null, null, null);
    }

    public static AnchoredRect CreateVerticallyStretched(int top = 0, int bottom = 0)
    {
      return new AnchoredRect(null, top, null, bottom, null, null);
    }

    public static AnchoredRect CreateLeftAnchored(int left = 0, int top = 0, int bottom = 0, int? width = null)
    {
      if (!IsValidSize(width))
      {
        throw new ArgumentOutOfRangeException();
      }
      return new AnchoredRect(left, top, null, bottom, width, null);
    }

    public static AnchoredRect CreateRightAnchored(int right = 0, int top = 0, int bottom = 0, int? width = null)
    {
      if (!IsValidSize(width))
      {
        throw new ArgumentOutOfRangeException();
      }
      return new AnchoredRect(null, top, right, bottom, width, null);
    }

    public static AnchoredRect CreateTopAnchored(int left = 0, int top = 0, int right = 0, int? height = null)
    {
      if (!IsValidSize(height))
      {
        throw new ArgumentOutOfRangeException();
      }
      return new AnchoredRect(left, top, right, null, null, height);
    }

    public static AnchoredRect CreateBottomAnchored(int left = 0, int bottom = 0, int right = 0, int? height = null)
    {
      if (!IsValidSize(height))
      {
        throw new ArgumentOutOfRangeException();
      }
      return new AnchoredRect(left, null, right, bottom, null, height);
    }

    public static AnchoredRect CreateBottomLeftAnchored(int left = 0, int bottom = 0, int? width = null, int? height = null)
    {
      if (!IsValidSize(width))
      {
        throw new ArgumentOutOfRangeException();
      }
      if (!IsValidSize(height))
      {
        throw new ArgumentOutOfRangeException();
      }
      return new AnchoredRect(left, null, null, bottom, width, height);
    }

    public static AnchoredRect CreateBottomRightAnchored(int right = 0, int bottom = 0, int? width = null, int? height = null)
    {
      if (!IsValidSize(width))
      {
        throw new ArgumentOutOfRangeException();
      }
      if (!IsValidSize(height))
      {
        throw new ArgumentOutOfRangeException();
      }
      return new AnchoredRect(null, null, right, bottom, width, height);
    }

    public static AnchoredRect CreateTopRightAnchored(int right = 0, int top = 0, int? width = null, int? height = null)
    {
      if (!IsValidSize(width))
      {
        throw new ArgumentOutOfRangeException();
      }
      if (!IsValidSize(height))
      {
        throw new ArgumentOutOfRangeException();
      }
      return new AnchoredRect(null, top, right, null, width, height);
    }

    public static AnchoredRect CreateTopLeftAnchored(int left = 0, int top = 0, int? width = null, int? height = null)
    {
      if (!IsValidSize(width))
      {
        throw new ArgumentOutOfRangeException();
      }
      if (!IsValidSize(height))
      {
        throw new ArgumentOutOfRangeException();
      }
      return new AnchoredRect(left, top, null, null, width, height);
    }

    public static AnchoredRect CreateCentered(int? width = null, int? height = null)
    {
      if (!IsValidSize(width))
      {
        throw new ArgumentOutOfRangeException();
      }
      if (!IsValidSize(height))
      {
        throw new ArgumentOutOfRangeException();
      }

      return new AnchoredRect(null, null, null, null, width, height);
    }

    /// <summary>
    ///  An alias to CreateCentred that makes defining constraints for BoxGroup child widgets more
    ///  natural.
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    public static AnchoredRect CreateSizeConstraint(int? width = null, int? height = null)
    {
      return CreateCentered(width, height);
    }

    static bool IsValidSize(int? width)
    {
      if (width == null)
      {
        return true;
      }
      return VisualContent.IsValidSize(width.Value);
    }

    /// <summary>
    ///   Resolves the required/used size of a widget within the context of a parent. This requires the widget's desired size
    ///   as input.
    /// </summary>
    /// <param name="widgetSize"></param>
    /// <returns></returns>
    [Pure]
    public Size ResolveSize(Size widgetSize)
    {
      if (!VisualContent.IsValidSize(widgetSize.Width))
      {
        throw new ArgumentOutOfRangeException();
      }
      if (!VisualContent.IsValidSize(widgetSize.Height))
      {
        throw new ArgumentOutOfRangeException();
      }

      int height;
      if (Top.HasValue)
      {
        if (Bottom.HasValue)
        {
          height = Top.Value + widgetSize.HeightInt + Bottom.Value;
        }
        else
        {
          height = Top.Value + Height.GetValueOrDefault(widgetSize.HeightInt);
        }
      }
      else
      {
        height = Height.GetValueOrDefault(widgetSize.HeightInt) + Bottom.GetValueOrDefault();
      }

      int width;
      if (Left.HasValue)
      {
        if (Right.HasValue)
        {
          width = Left.Value + widgetSize.WidthInt + Right.Value;
        }
        else
        {
          width = Left.Value + Width.GetValueOrDefault(widgetSize.WidthInt);
        }
      }
      else
      {
        width = Width.GetValueOrDefault(widgetSize.WidthInt) + Right.GetValueOrDefault();
      }
      return new Size(width, height);
    }

    public override string ToString()
    {
      return
        $"Left: {OptionalToString(Left)}, Top: {OptionalToString(Top)}, Right: {OptionalToString(Right)}, Bottom: {OptionalToString(Bottom)}, Width: {OptionalToString(Width)}, Height: {OptionalToString(Height)}, HasWidth: {HasWidth}, HasHeight: {HasHeight}";
    }

    string OptionalToString(int? value)
    {
      if (value.HasValue)
      {
        return value.Value.ToString();
      }
      return "<null>";
    }

    public bool Equals(AnchoredRect other)
    {
      return Left == other.Left && Top == other.Top && Right == other.Right && Bottom == other.Bottom && Width == other.Width && Height == other.Height;
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj))
      {
        return false;
      }
      return obj is AnchoredRect && Equals((AnchoredRect)obj);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        var hashCode = Left.GetHashCode();
        hashCode = (hashCode * 397) ^ Top.GetHashCode();
        hashCode = (hashCode * 397) ^ Right.GetHashCode();
        hashCode = (hashCode * 397) ^ Bottom.GetHashCode();
        hashCode = (hashCode * 397) ^ Width.GetHashCode();
        hashCode = (hashCode * 397) ^ Height.GetHashCode();
        return hashCode;
      }
    }

    public static bool operator ==(AnchoredRect left, AnchoredRect right)
    {
      return left.Equals(right);
    }

    public static bool operator !=(AnchoredRect left, AnchoredRect right)
    {
      return !left.Equals(right);
    }
  }
}