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
using System.Collections;
using System.Collections.Generic;

namespace Steropes.UI.Util
{
  public struct PushBackEnumerator<T> : IEnumerator<T>
  {
    readonly List<T> list;

    readonly int startIndex;

    readonly int endIndex;

    T pushedBack;

    T pendingPushBack;

    bool pendingFilled;

    bool currentFilled;

    int index;

    public PushBackEnumerator(List<T> list, int startIndex, int endIndex)
    {
      this.list = list;
      this.startIndex = startIndex;
      this.endIndex = endIndex;
      pushedBack = default(T);
      pendingPushBack = default(T);
      index = startIndex - 1;
      pendingFilled = false;
      currentFilled = false;
    }

    public void PushBack(T other)
    {
      if (pendingFilled)
      {
        throw new InvalidOperationException();
      }

      pendingPushBack = other;
      pendingFilled = true;
    }

    public void Dispose()
    {
    }

    public bool MoveNext()
    {
      if (pendingFilled)
      {
        currentFilled = true;
        pushedBack = pendingPushBack;
        pendingPushBack = default(T);
        pendingFilled = false;
        return true;
      }

      currentFilled = false;
      pushedBack = default(T);

      if (index >= endIndex)
      {
        return false;
      }

      index += 1;
      return index < endIndex;
    }

    public void Reset()
    {
      currentFilled = false;
      pendingFilled = false;
      pendingPushBack = default(T);
      pushedBack = default(T);
      index = startIndex - 1;
    }

    public T Current
    {
      get
      {
        if (currentFilled)
        {
          return pushedBack;
        }

        return list[index];
      }
    }

    object IEnumerator.Current => Current;
  }
}