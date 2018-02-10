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

namespace Steropes.UI.Styles
{
  /// <summary>
  ///   A wrapper around HashSet to build a readonly lookup interface, needed because the C# API is designed by random
  ///   events.
  /// </summary>
  public interface IStyleClassSet
  {
    bool Contains(string className);
  }

  public class HashStyleClassSet : IStyleClassSet
  {
    readonly HashSet<string> backend;

    public HashStyleClassSet()
    {
      backend = new HashSet<string>();
    }

    public void Add(string className)
    {
      backend.Add(className);
    }

    public bool Contains(string className)
    {
      return backend.Contains(className);
    }

    public void Remove(string className)
    {
      backend.Remove(className);
    }

    public override string ToString()
    {
      return $"[{string.Join(",", backend)}]";
    }
  }
}