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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Steropes.UI.Input.GamePadInput
{
  public class GamePadEventArgs : InputEventArgs<GamePadEventType, GamePadEventData>
  {
    GamePadEventData eventData;

    [Obsolete]
    public GamePadEventArgs(object source, GamePadEventData eventData)
    {
      Source = source;
      this.eventData = eventData;
    }

    public GamePadEventArgs(GamePadEventData eventData)
    {
      this.eventData = eventData;
    }

    public Buttons Button => eventData.Button;

    public override GamePadEventData EventData => eventData;

    public override GamePadEventType EventType => eventData.EventType;

    public override InputFlags Flags => eventData.Flags;

    public override int Frame => eventData.Frame;

    [Obsolete]
    public object Source { get; }

    public Vector2 ThumbstickState => eventData.ThumbstickState;

    public override TimeSpan Time => eventData.Time;

    public float TriggerState => eventData.TriggerState;
  }

  public struct GamePadEventData
  {
    public GamePadEventType EventType { get; }

    public TimeSpan Time { get; }

    public int Frame { get; }

    public InputFlags Flags { get; }

    public Buttons Button { get; }

    public float TriggerState { get; }

    public Vector2 ThumbstickState { get; }

    public GamePadEventData(GamePadEventType eventType, TimeSpan time, int frame, InputFlags flags, Buttons button, float triggerstate = 0)
    {
      EventType = eventType;
      Time = time;
      Frame = frame;
      Flags = flags;
      Button = button;
      TriggerState = triggerstate;
      ThumbstickState = Vector2.Zero;
    }

    public GamePadEventData(GamePadEventType eventType, TimeSpan time, int frame, InputFlags flags, Buttons button, Vector2 thumbStickState)
    {
      EventType = eventType;
      Time = time;
      Frame = frame;
      Flags = flags;
      Button = button;
      TriggerState = 0;
      ThumbstickState = thumbStickState;
    }

    public override string ToString()
    {
      return string.Format("(EventType: {0}, ThumbstickState: {1}, TriggerState: {2}, Button: {3}, Flags: {4}, Time: {5}, Frame: {6}, Source: {7})", 
                           EventType, ThumbstickState, TriggerState, Button, Flags, Time, Frame);
    }
  }
}