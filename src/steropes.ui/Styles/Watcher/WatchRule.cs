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
using System.ComponentModel;

using Steropes.UI.Components;
using Steropes.UI.Util;

namespace Steropes.UI.Styles.Watcher
{
  public abstract class WatchRule : IWatchRule, IEquatable<WatchRule>
  {
    readonly PropertyChangedEventHandler propertyChangedHandler;

    readonly EventSupport<EventArgs> stateChangedSupport;

    bool match;

    protected WatchRule(IWidget target)
    {
      Target = target;
      propertyChangedHandler = OnPropertyChanged;
      stateChangedSupport = new EventSupport<EventArgs>();
      Target.PropertyChanged += propertyChangedHandler;
    }

    public event EventHandler<EventArgs> StateChanged
    {
      add
      {
        stateChangedSupport.Event += value;
      }
      remove
      {
        stateChangedSupport.Event -= value;
      }
    }

    public bool Match
    {
      get
      {
        return match;
      }
      set
      {
        if (match == value)
        {
          return;
        }
        match = value;
        stateChangedSupport.Raise(this, EventArgs.Empty);
      }
    }

    public IWidget Target { get; }

    public static bool operator ==(WatchRule left, WatchRule right)
    {
      return Equals(left, right);
    }

    public static bool operator !=(WatchRule left, WatchRule right)
    {
      return !Equals(left, right);
    }

    public bool Equals(WatchRule other)
    {
      if (ReferenceEquals(null, other))
      {
        return false;
      }
      if (ReferenceEquals(this, other))
      {
        return true;
      }
      return Target.Equals(other.Target);
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj))
      {
        return false;
      }
      if (ReferenceEquals(this, obj))
      {
        return true;
      }
      if (obj.GetType() != GetType())
      {
        return false;
      }
      return Equals((WatchRule)obj);
    }

    public override int GetHashCode()
    {
      return Target.GetHashCode();
    }

    public void Remove()
    {
      Target.PropertyChanged -= propertyChangedHandler;
    }

    protected void Init()
    {
      match = IsMatch();
    }

    protected abstract bool IsMatch();

    void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      Match = IsMatch();
    }
  }
}