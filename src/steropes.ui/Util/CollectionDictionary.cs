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
using System.Collections;
using System.Collections.Generic;

namespace Steropes.UI.Util
{
  public class CollectionDictionary<TKey, TValue, TCollection> : IReadOnlyDictionary<TKey, TCollection>
    where TCollection : ICollection<TValue>, new()
  {
    readonly Dictionary<TKey, TCollection> values;

    public CollectionDictionary()
    {
      values = new Dictionary<TKey, TCollection>();
    }

    public int Count => values.Count;

    public IEnumerable<TKey> Keys => values.Keys;

    public IEnumerable<TCollection> Values => values.Values;

    public TCollection this[TKey key]
    {
      get
      {
        TCollection t;
        if (values.TryGetValue(key, out t))
        {
          return t;
        }
        return new TCollection();
      }
    }

    public void Add(TKey key, TValue value)
    {
      TCollection valueList;
      if (values.TryGetValue(key, out valueList))
      {
        valueList.Add(value);
      }
      else
      {
        valueList = new TCollection { value };
        values.Add(key, valueList);
      }
    }

    public bool ContainsKey(TKey key)
    {
      return values.ContainsKey(key);
    }

    public IEnumerator<KeyValuePair<TKey, TCollection>> GetEnumerator()
    {
      return values.GetEnumerator();
    }

    public bool Remove(TKey key, TValue value)
    {
      TCollection t;
      if (values.TryGetValue(key, out t))
      {
        return t.Remove(value);
      }
      return false;
    }

    public bool RemoveAll(TKey key)
    {
      return values.Remove(key);
    }

    public bool TryGetValue(TKey key, out TCollection value)
    {
      return values.TryGetValue(key, out value);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return ((IEnumerable)values).GetEnumerator();
    }
  }
}