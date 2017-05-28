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

using Microsoft.Xna.Framework.Input;

namespace Steropes.UI.Input.KeyboardInput
{
  public struct KeyStroke : IEquatable<KeyStroke>
  {
    public KeyStroke(Keys key, InputFlags modifiers = InputFlags.None)
    {
      Modifiers = modifiers;
      Key = key;
    }

    public KeyStroke(KeyEventArgs args)
    {
      Modifiers = args.Flags;
      Key = args.Key;
    }

    public override string ToString()
    {
      return ToString(DefaultVirtualKeyLocaliser.Default);
    }

    public string ToString(IVirtualKeyLocaliser localiser)
    {
      if (Key.IsModifier())
      {
        return Modifiers.AsKeyModifiers().AsText();
      }

      var keyAsText = localiser.ToLocalisedText(Key);
      if (Modifiers == InputFlags.None)
      {
        return keyAsText;
      }

      return $"{Modifiers.AsKeyModifiers().AsText()} {keyAsText}";
    }

    public bool Equals(KeyStroke other)
    {
      return Modifiers == other.Modifiers && Key == other.Key;
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj))
      {
        return false;
      }
      return obj is KeyStroke && Equals((KeyStroke)obj);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        return ((int)Modifiers * 397) ^ (int)Key;
      }
    }

    public static bool operator ==(KeyStroke left, KeyStroke right)
    {
      return left.Equals(right);
    }

    public static bool operator !=(KeyStroke left, KeyStroke right)
    {
      return !left.Equals(right);
    }

    /// <summary>
    ///   Checks whether this Keystroke is a match for the other keystroke given.
    ///   The directionality of the comparison is important. It is assumed that the
    ///   keystroke given in "other" is the registered Keystroke of an Action.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool IsMatchForAction(KeyStroke other)
    {
      return Key == other.Key && (Modifiers & other.Modifiers) == other.Modifiers;
    }

    public InputFlags Modifiers { get; }

    public Keys Key { get; }
  }
}