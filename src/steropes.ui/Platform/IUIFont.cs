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
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Steropes.UI.Platform
{
  public interface IUIFont
  {
    float Baseline { get; }

    int LineSpacing { get; }

    string Name { get; }

    float Spacing { get; }

    SpriteFont SpriteFont { get; }

    Vector2 MeasureString(string text);

    Vector2 MeasureString(StringBuilder text);
  }

  public class UIFont : IUIFont, IEquatable<UIFont>
  {
    public UIFont(SpriteFont font, float baseline, string name = null)
    {
      SpriteFont = font;
      Baseline = baseline;
      Name = name;
    }

    public float Baseline { get; }

    public IReadOnlyCollection<char> Characters => SpriteFont.Characters;

    public char? DefaultCharacter
    {
      get
      {
        return SpriteFont.DefaultCharacter;
      }
      set
      {
        SpriteFont.DefaultCharacter = value;
      }
    }

    public int LineSpacing
    {
      get
      {
        return SpriteFont.LineSpacing;
      }
      set
      {
        SpriteFont.LineSpacing = value;
      }
    }

    public string Name { get; }

    public float Spacing
    {
      get
      {
        return SpriteFont.Spacing;
      }
      set
      {
        SpriteFont.Spacing = value;
      }
    }

    public SpriteFont SpriteFont { get; }

    public static bool operator ==(UIFont left, UIFont right)
    {
      return Equals(left, right);
    }

    public static bool operator !=(UIFont left, UIFont right)
    {
      return !Equals(left, right);
    }

    public bool Equals(UIFont other)
    {
      if (ReferenceEquals(null, other))
      {
        return false;
      }
      if (ReferenceEquals(this, other))
      {
        return true;
      }
      return string.Equals(Name, other.Name) && Baseline.Equals(other.Baseline);
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
      return Equals((UIFont)obj);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        var hashCode = Name?.GetHashCode() ?? 0;
        hashCode = (hashCode * 397) ^ Baseline.GetHashCode();
        return hashCode;
      }
    }

    public Vector2 MeasureString(string text)
    {
      return SpriteFont.MeasureString(text);
    }

    public Vector2 MeasureString(StringBuilder text)
    {
      return SpriteFont.MeasureString(text);
    }

    public override string ToString()
    {
      return $"UIFont={{Name: {Name}, Baseline: {Baseline}, LineSpacing: {LineSpacing}, Spacing: {Spacing}}}";
    }
  }
}