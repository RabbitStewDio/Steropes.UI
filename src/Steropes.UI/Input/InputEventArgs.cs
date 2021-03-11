// MIT License
//
// Copyright (c) 2011-2016 Elisée Maurer, Sparklin Labs, Creative Patterns
// Copyright (c) 2016-2018 Thomas Morgner, Rabbit-StewDio Ltd.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
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
using System.Diagnostics.CodeAnalysis;

namespace Steropes.UI.Input
{
  public abstract class InputEventArgs : EventArgs
  {
    bool consumed;

    protected InputEventArgs()
    {
      consumed = false;
    }

    public bool Consumed
    {
      get
      {
        return consumed;
      }
      set
      {
        if (value)
        {
          consumed = true;
        }
      }
    }

    public abstract InputFlags Flags { get; }

    public abstract int Frame { get; }

    public abstract TimeSpan Time { get; }

    public void Consume()
    {
      Consumed = true;
    }

    protected void Reset()
    {
      consumed = false;
    }
  }

  [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Generic and non generic class pair.")]
  public abstract class InputEventArgs<TEventType, TEventData> : InputEventArgs
    where TEventType : struct where TEventData : struct
  {
    public abstract TEventData EventData { get; }

    public abstract TEventType EventType { get; }

    public override string ToString()
    {
      return $"({nameof(EventType)}: {EventType}, {nameof(EventData)}: {EventData})";
    }
  }
}