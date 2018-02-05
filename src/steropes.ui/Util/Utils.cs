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
using System.Globalization;

using Microsoft.Xna.Framework;

namespace Steropes.UI.Util
{
  public static class Utils
  {
    public static int IndexOf<T>(this IReadOnlyList<T> list, T value)
    {
      for (var i = 0; i < list.Count; i++)
      {
        if (Equals(list[i], value))
        {
          return i;
        }
      }

      return -1;
    }

    public static ICollection<T> AddRange<T>(this ICollection<T> c, IEnumerable<T> r)
    {
      foreach (var element in r)
      {
        c.Add(element);
      }

      return c;
    }

    public static Color ColorFromHex(string str)
    {
      if (str.StartsWith("#"))
      {
        str = str.Substring(1);
      }

      var hex = uint.Parse(str, NumberStyles.HexNumber, CultureInfo.InvariantCulture);

      var color = Color.White;
      if (str.Length == 8)
      {
        color.R = (byte)(hex >> 24);
        color.G = (byte)(hex >> 16);
        color.B = (byte)(hex >> 8);
        color.A = (byte)hex;
      }
      else if (str.Length == 6)
      {
        color.R = (byte)(hex >> 16);
        color.G = (byte)(hex >> 8);
        color.B = (byte)hex;
      }
      else
      {
        throw new InvalidOperationException("Invald hex representation of an ARGB or RGB color value.");
      }

      return color;
    }

    public static void ForEach<T>(this IEnumerable<T> e, Action<T> action)
    {
      foreach (var element in e)
      {
        action(element);
      }
    }

    public static bool IsSameValue<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
    {
      TValue v;
      if (dict.TryGetValue(key, out v))
      {
        return Equals(v, value);
      }

      return Equals(value, default(TValue));
    }
  }
}