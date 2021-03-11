// MIT License
//
// Copyright (c) 2011-2016 Elisée Maurer, Sparklin Labs, Creative Patterns
// Copyright (c) 2016 Thomas Morgner, Rabbit-StewDio Ltd.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using Steropes.UI.Components;
using Steropes.UI.Components.Window;
using Steropes.UI.Util;

namespace Steropes.UI.Widgets.Container
{
  public class Window<TContent> : ContentWidget<TContent>, IWindow<TContent>
    where TContent : class, IWidget
  {
    readonly EventSupport<EventArgs> closedSupport;
    readonly EventSupport<ClosingEventArgs> closingSupport;

    public Window(IUIStyle style) : base(style)
    {
      closedSupport = new EventSupport<EventArgs>();
      closingSupport = new EventSupport<ClosingEventArgs>();
    }

    public event EventHandler<EventArgs> Closed
    {
      add { closedSupport.Event += value; }
      remove { closedSupport.Event -= value; }
    }
    
    public EventHandler<EventArgs> OnClosed
    {
      get { return closedSupport.Handler; }
      set { closedSupport.Handler = value; }
    }

    public event EventHandler<ClosingEventArgs> Closing
    {
      add { closingSupport.Event += value; }
      remove { closingSupport.Event -= value; }
    }
    
    public EventHandler<ClosingEventArgs> OnClosing
    {
      get { return closingSupport.Handler; }
      set { closingSupport.Handler = value; }
    }

    public void Close()
    {
      if (Parent != null)
      {
        var closingEvent = new ClosingEventArgs();
        closingSupport.Raise(this, closingEvent);
        if (closingEvent.Rejected)
        {
          return;
        }

        closedSupport.Raise(this, EventArgs.Empty);
        if (Parent != null)
        {
          throw new InvalidOperationException("Not closed!");
        }
      }
    }

    public void ToBack()
    {
      LayeredParent?.ToBack(this);
    }

    public void ToFront()
    {
      LayeredParent?.ToFront(this);
    }

    // ReSharper disable once MemberCanBePrivate.Global
    protected ILayeredWidget LayeredParent => Parent as ILayeredWidget;
  }
}