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
using Microsoft.Xna.Framework.Graphics;

namespace Steropes.UI.Platform
{
  public interface IBoxTexture : IUITexture
  {
    Insets CornerArea { get; }

    Insets Margins { get; }
  }

  public class BoxTexture : UITexture, IBoxTexture, IEquatable<BoxTexture>
  {
    public BoxTexture(Texture2D texture, int insets) : this(texture, new Insets(insets), Insets.Zero)
    {
    }

    public BoxTexture(Texture2D texture, Insets cornerArea, Insets margins) : base(texture)
    {
      CornerArea = cornerArea;
      Margins = margins;
    }

    public BoxTexture(Texture2D texture, Rectangle bounds, string name, int insets) : this(texture, bounds, name, new Insets(insets), Insets.Zero)
    {
    }

    public BoxTexture(Texture2D texture, Rectangle bounds, string name, Insets cornerArea, Insets margins) : base(texture, bounds, name)
    {
      CornerArea = cornerArea;
      Margins = margins;
    }

    public override IUITexture Rebase(Texture2D texture, Rectangle bounds, string name)
    {
      return new BoxTexture(texture, bounds, name, CornerArea, Margins);
    }

    public Insets CornerArea { get; }

    public Insets Margins { get; }

    public static bool operator ==(BoxTexture left, BoxTexture right)
    {
      return Equals(left, right);
    }

    public static bool operator !=(BoxTexture left, BoxTexture right)
    {
      return !Equals(left, right);
    }

    public bool Equals(BoxTexture other)
    {
      if (ReferenceEquals(null, other))
      {
        return false;
      }
      if (ReferenceEquals(this, other))
      {
        return true;
      }
      return base.Equals(other) && CornerArea.Equals(other.CornerArea) && Margins.Equals(other.Margins);
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj))
      {
        return false;
      }
      if (ReferenceEquals(this, obj))
      {
        return true;
      }
      if (obj.GetType() != this.GetType())
      {
        return false;
      }
      return Equals((BoxTexture)obj);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        var hashCode = base.GetHashCode();
        hashCode = (hashCode * 397) ^ CornerArea.GetHashCode();
        hashCode = (hashCode * 397) ^ Margins.GetHashCode();
        return hashCode;
      }
    }
  }
}