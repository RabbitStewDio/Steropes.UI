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
using System.ComponentModel;
using System.Threading;

namespace Steropes.UI.Util
{
  public class PropertyChangedEventSupport
  {
    readonly int creatorThread;

    Dictionary<string, PropertyChangedEventArgs> eventArgs;

    List<PropertyChangedEventHandler> eventHandlers;

    public PropertyChangedEventSupport()
    {
      this.creatorThread = Thread.CurrentThread.ManagedThreadId;
    }

    public event PropertyChangedEventHandler Event
    {
      add
      {
        if (this.eventHandlers == null)
        {
          this.eventHandlers = new List<PropertyChangedEventHandler>();
        }
        this.eventHandlers.Add(value);
      }
      remove
      {
        this.eventHandlers?.Remove(value);
      }
    }

    public virtual void Raise(object source, string propertyName)
    {
      if (this.creatorThread != Thread.CurrentThread.ManagedThreadId)
      {
        throw new InvalidOperationException("Raise must be called from the UI thread.");
      }

      if (this.eventHandlers == null)
      {
        return;
      }

      if (this.eventArgs == null)
      {
        this.eventArgs = new Dictionary<string, PropertyChangedEventArgs>();
      }
      PropertyChangedEventArgs eventObj;
      if (!this.eventArgs.TryGetValue(propertyName, out eventObj))
      {
        eventObj = new PropertyChangedEventArgs(propertyName);
        this.eventArgs.Add(propertyName, eventObj);
      }

      for (var index = 0; index < this.eventHandlers.Count; index++)
      {
        var eventHandler = this.eventHandlers[index];
        eventHandler?.Invoke(source, eventObj);
      }
    }
  }
}