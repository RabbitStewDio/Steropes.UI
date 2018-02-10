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
using System.Collections.Generic;

namespace Steropes.UI.Util
{
  /// <summary>
  ///  A dictionary implementation for a limited number of keys. This statically
  ///  maps the keys into an array-index position using a static lookup dictionary.
  ///  This avoids some memory allocations that would happen otherwise while modifying
  ///  the enum map.
  /// </summary>
  /// <typeparam name="TEnum"></typeparam>
  /// <typeparam name="TValueType"></typeparam>
  public class EnumMap<TEnum, TValueType>
  {
    readonly TValueType[] data;

    readonly Dictionary<TEnum, int> keyToIndexMapping;

    public EnumMap(IReadOnlyList<TEnum> possibleKeys)
    {
      keyToIndexMapping = new Dictionary<TEnum, int>();
      for (var index = 0; index < possibleKeys.Count; index++)
      {
        var key = possibleKeys[index];
        keyToIndexMapping.Add(key, index);
      }
      data = new TValueType[keyToIndexMapping.Count];
    }

    public TValueType this[TEnum key]
    {
      get
      {
        var idx = keyToIndexMapping[key];
        return data[idx];
      }
      set
      {
        var idx = keyToIndexMapping[key];
        data[idx] = value;
      }
    }

    public void Fill(TValueType value)
    {
      for (var i = 0; i < data.Length; i++)
      {
        data[i] = value;
      }
    }

    public bool TryGet(TEnum key, out TValueType value)
    {
      int index;
      if (keyToIndexMapping.TryGetValue(key, out index))
      {
        value = data[index];
        return true;
      }

      value = default(TValueType);
      return false;
    }
  }
}