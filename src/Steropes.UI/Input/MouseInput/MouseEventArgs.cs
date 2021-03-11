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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Steropes.UI.Input.MouseInput
{
  public class MouseEventArgs : InputEventArgs<MouseEventType, MouseEventData>
  {
    MouseEventData eventData;

    [Obsolete]
    public MouseEventArgs(object source, MouseEventData data)
    {
      Source = source;
      eventData = data;
    }

    public MouseEventArgs(MouseEventData data)
    {
      eventData = data;
    }

    public MouseButton Button => eventData.Button;

    public int ClickCount => eventData.ClickCount;

    public override MouseEventData EventData => eventData;

    public override MouseEventType EventType => eventData.EventType;

    public override InputFlags Flags => eventData.Flags;

    public override int Frame => eventData.Frame;

    public Point Position => eventData.Position;

    public int ScrollWheelDelta => eventData.ScrollWheelDelta;

    public int ScrollWheelValue => eventData.ScrollWheelValue;

    [Obsolete]
    public object Source { get; }

    public override TimeSpan Time => eventData.Time;

    public MouseEventArgs Derive()
    {
      return Derive(EventType);
    }

    public MouseEventArgs Derive(MouseEventType type)
    {
#pragma warning disable 612
      return new MouseEventArgs(Source, eventData.ConvertInto(type, ClickCount));
#pragma warning restore 612
    }

    public void Reuse(MouseEventData data)
    {
      eventData = data;
      Consumed = false;
    }
  }

  public struct MouseEventData
  {
    public TimeSpan Time { get; }

    public MouseEventType EventType { get; }

    public MouseButton Button { get; }

    public Point Position { get; }

    public int ScrollWheelValue { get; }

    public int ScrollWheelDelta { get; }

    public InputFlags Flags { get; }

    public int ClickCount { get; }

    public int Frame { get; }

    public MouseEventData(
      MouseEventType eventType,
      InputFlags flags,
      TimeSpan time,
      int frame,
      Point position,
      MouseButton button = MouseButton.None,
      int clickCount = 0,
      int scrollWheelValue = 0,
      int scrollWheelDelta = 0)
    {
      ClickCount = clickCount;
      Time = time;
      Frame = frame;
      EventType = eventType;
      Button = button;
      Position = position;
      ScrollWheelValue = scrollWheelValue;
      ScrollWheelDelta = scrollWheelDelta;
      Flags = flags;
    }

    public MouseEventData(MouseEventType type, InputFlags flags, TimeSpan time, int frame, MouseState previousState, MouseState currentState, MouseButton button = MouseButton.None)
    {
      EventType = type;
      Flags = flags;
      Position = new Point(currentState.X, currentState.Y);
      Button = button;
      ScrollWheelValue = currentState.ScrollWheelValue;
      ScrollWheelDelta = currentState.ScrollWheelValue - previousState.ScrollWheelValue;
      Time = time;
      Frame = frame;
      ClickCount = 0;
    }

    public MouseEventData ConvertInto(MouseEventType type, int clickCount = 0)
    {
      return new MouseEventData(type, Flags, Time, Frame, Position, Button, clickCount, ScrollWheelValue, ScrollWheelDelta);
    }

    public override string ToString()
    {
      return string.Format("(EventType: {0}, Position: {1}, Flags: {2}, Button: {3}, ClickCount: {4}, ScrollWheelValue: {5}, ScrollWheelDelta: {6}, Time: {7}, Frame: {8})", 
                           EventType, Position, Flags, Button, ClickCount, ScrollWheelValue, ScrollWheelDelta, Time, Frame);
    }
  }
}