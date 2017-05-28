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

using Microsoft.Xna.Framework.Graphics;

namespace Steropes.UI.Platform
{
  public interface IUITexture
  {
    int Height { get; }

    string Name { get; }

    Texture2D Texture { get; }

    int Width { get; }
  }

  public class UITexture : IUITexture, IEquatable<UITexture>
  {
    public UITexture(Texture2D texture)
    {
      Texture = texture;
    }

    public int Height => Texture?.Height ?? 0;

    public string Name => Texture?.Name;

    public Texture2D Texture { get; }

    public int Width => Texture?.Width ?? 0;

    public static bool operator ==(UITexture left, UITexture right)
    {
      return Equals(left, right);
    }

    public static bool operator !=(UITexture left, UITexture right)
    {
      return !Equals(left, right);
    }

    public bool Equals(UITexture other)
    {
      if (ReferenceEquals(null, other))
      {
        return false;
      }
      if (ReferenceEquals(this, other))
      {
        return true;
      }
      return Equals(Name, other.Name);
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
      return Equals((UITexture)obj);
    }

    public override int GetHashCode()
    {
      return Name?.GetHashCode() ?? 0;
    }
  }
}