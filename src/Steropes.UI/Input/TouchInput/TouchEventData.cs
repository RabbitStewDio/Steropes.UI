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
using System.Runtime.InteropServices;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;

namespace Steropes.UI.Input.TouchInput
{
  public struct TouchEventData
  {
    GestureOrTouchLocation payload;

    public TimeSpan Time { get; }

    public InputFlags Flags { get; }

    public TouchEventType EventType { get; }

    public TouchLocation TouchLocation => payload.Touch;

    public GestureSample Gesture => payload.Gesture;

    public Point Position => payload.IsTouchLocation ? TouchLocation.Position.ToPoint() : Gesture.Position.ToPoint();

    public int Frame { get; }

    public bool IsTouchLocation => payload.IsTouchLocation;

    public TouchEventData(TimeSpan time, int frame, InputFlags flags, TouchEventType eventType, TouchLocation touchLocation)
    {
      Time = time;
      Frame = frame;
      Flags = flags;
      EventType = eventType;
      payload = new GestureOrTouchLocation(touchLocation);
    }

    public TouchEventData(TimeSpan time, int frame, InputFlags flags, TouchEventType eventType, GestureSample gesture)
    {
      Time = time;
      Frame = frame;
      Flags = flags;
      EventType = eventType;
      payload = new GestureOrTouchLocation(gesture);
    }

    public override string ToString()
    {
      return $"{nameof(EventType)}: {EventType}, {nameof(Position)}: {Position}, Data: {payload}, {nameof(Flags)}: {Flags}, {nameof(Time)}: {Time}, {nameof(Frame)}: {Frame}";
    }

    [StructLayout(LayoutKind.Explicit)]
    struct GestureOrTouchLocation
    {
      [FieldOffset(0)]
      readonly bool touchLocation;

      [FieldOffset(4)]
      readonly TouchLocation touch;

      [FieldOffset(4)]
      readonly GestureSample gesture;

      public GestureOrTouchLocation(GestureSample gesture)
      {
        touch = default(TouchLocation);
        this.gesture = gesture;
        touchLocation = false;
      }

      public bool IsTouchLocation => touchLocation;

      public GestureOrTouchLocation(TouchLocation touch)
      {
        gesture = default(GestureSample);
        this.touch = touch;
        touchLocation = true;
      }

      public TouchLocation Touch
      {
        get
        {
          if (IsTouchLocation)
          {
            return touch;
          }
          throw new InvalidOperationException();
        }
      }

      public GestureSample Gesture
      {
        get
        {
          if (!IsTouchLocation)
          {
            return gesture;
          }
          throw new InvalidOperationException();
        }
      }

      public override string ToString()
      {
        if (touchLocation)
        {
          return $"({nameof(Touch)}: {Touch})";
        }

        return $"({nameof(Gesture)}: {Gesture})";
      }
    }
  }
}