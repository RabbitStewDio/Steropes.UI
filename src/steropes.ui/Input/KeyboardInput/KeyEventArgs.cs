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

using Microsoft.Xna.Framework.Input;

namespace Steropes.UI.Input.KeyboardInput
{
  public class KeyEventArgs : InputEventArgs<KeyEventType, KeyEventData>
  {
    readonly KeyEventData eventData;

    public KeyEventArgs(object source, KeyEventData eventData)
    {
      Source = source;
      this.eventData = eventData;
    }

    public char Character => eventData.Character;

    public override KeyEventData EventData => eventData;

    public override KeyEventType EventType => eventData.EventType;

    public override InputFlags Flags => eventData.Flags;

    public override int Frame => eventData.Frame;

    public Keys Key => eventData.Key;

    public object Source { get; }

    public override TimeSpan Time => eventData.Time;
  }

  public struct KeyEventData
  {
    public KeyEventType EventType { get; }

    public TimeSpan Time { get; }

    public int Frame { get; }

    public InputFlags Flags { get; }

    public Keys Key { get; }

    public char Character { get; }

    public KeyEventData(KeyEventType eventType, TimeSpan time, int frame, InputFlags flags, Keys key)
    {
      EventType = eventType;
      Time = time;
      Frame = frame;
      Flags = flags;
      Key = key;
      Character = (char)0;
    }

    public KeyEventData(KeyEventType eventType, TimeSpan time, int frame, InputFlags flags, char character)
    {
      EventType = eventType;
      Time = time;
      Frame = frame;
      Flags = flags;
      Key = Keys.None;
      Character = character;
    }

    public KeyEventData WithLocalisedKey(Keys k)
    {
      return new KeyEventData(EventType, Time, Frame, Flags, k);
    }
  }
}