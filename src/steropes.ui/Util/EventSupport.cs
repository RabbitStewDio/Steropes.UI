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
using System.Threading;

namespace Steropes.UI.Util
{
  public class EventSupport<T>
  {
    readonly int creatorThread;

    List<EventHandler<T>> eventHandlers;

    public EventSupport()
    {
      creatorThread = Thread.CurrentThread.ManagedThreadId;
    }

    public event EventHandler<T> Event
    {
      add
      {
        if (eventHandlers == null)
        {
          eventHandlers = new List<EventHandler<T>>();
        }
        eventHandlers.Add(value);
      }
      remove
      {
        eventHandlers?.Remove(value);
      }
    }

    public virtual void Raise(object source, T e, Func<T, bool> continuation = null)
    {
      if (creatorThread != Thread.CurrentThread.ManagedThreadId)
      {
        throw new InvalidOperationException("Raise must be called from the UI thread.");
      }

      if (eventHandlers == null)
      {
        return;
      }

      var cont = continuation ?? AcceptAll;

      for (var index = 0; index < eventHandlers.Count; index++)
      {
        var eventHandler = eventHandlers[index];
        if (cont(e))
        {
          eventHandler?.Invoke(source, e);
        }
      }
    }

    public virtual void RaiseReverse(object source, T e, Func<T, bool> continuation = null)
    {
      if (creatorThread != Thread.CurrentThread.ManagedThreadId)
      {
        throw new InvalidOperationException("Raise must be called from the UI thread.");
      }

      if (eventHandlers == null)
      {
        return;
      }

      var cont = continuation ?? AcceptAll;

      for (var i = eventHandlers.Count - 1; i >= 0; i--)
      {
        var eventHandler = eventHandlers[i];
        if (cont(e))
        {
          eventHandler?.Invoke(source, e);
        }
      }
    }

    static bool AcceptAll(T dummy)
    {
      return true;
    }
  }
}