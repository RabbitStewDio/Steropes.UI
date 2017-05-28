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

namespace Steropes.UI.Util
{
  public class LRUCache<TKey, TValue>
  {
    readonly int capacity;

    readonly Dictionary<TKey, LinkedListNode<Tuple<TKey, TValue>>> dict;

    readonly LinkedList<Tuple<TKey, TValue>> list;

    public LRUCache(int cap)
    {
      if (cap <= 0)
      {
        throw new ArgumentOutOfRangeException();
      }

      dict = new Dictionary<TKey, LinkedListNode<Tuple<TKey, TValue>>>();
      list = new LinkedList<Tuple<TKey, TValue>>();
      capacity = cap;
    }

    public void Add(TKey key, TValue value)
    {
      while (dict.Count >= capacity)
      {
        Remove(list.First);
      }

      LinkedListNode<Tuple<TKey, TValue>> node;
      if (dict.TryGetValue(key, out node))
      {
        node.Value = Tuple.Create(key, value);
        list.Remove(node);
        list.AddLast(node);
      }
      else
      {
        node = new LinkedListNode<Tuple<TKey, TValue>>(Tuple.Create(key, value));
        dict[key] = node;
        list.AddLast(node);
      }
    }

    public bool Remove(TKey key)
    {
      LinkedListNode<Tuple<TKey, TValue>> node;
      if (dict.TryGetValue(key, out node))
      {
        dict.Remove(key);
        list.Remove(node);
        return true;
      }
      return false;
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
      LinkedListNode<Tuple<TKey, TValue>> node;
      if (dict.TryGetValue(key, out node))
      {
        value = node.Value.Item2;
        list.Remove(node);
        list.AddLast(node);
        return true;
      }
      value = default(TValue);
      return false;
    }

    void Remove(LinkedListNode<Tuple<TKey, TValue>> node)
    {
      list.RemoveFirst();
      dict.Remove(node.Value.Item1);
    }
  }
}