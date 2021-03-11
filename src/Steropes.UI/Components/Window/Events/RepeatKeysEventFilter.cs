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

using Microsoft.Xna.Framework.Input;

using Steropes.UI.Input;
using Steropes.UI.Input.KeyboardInput;

namespace Steropes.UI.Components.Window.Events
{
  public class RepeatKeysEventFilter : IEventSource<KeyEventData>
  {
    readonly EventQueue<KeyEventData> eventQueue;

    readonly Dictionary<Keys, double> keysDown;

    readonly IScreenService screen;

    readonly IEventSource<KeyEventData> source;

    TimeSpan lastScheduleTime;

    bool replay;

    public RepeatKeysEventFilter(IEventSource<KeyEventData> source, IScreenService screen)
    {
      this.source = source;
      this.screen = screen;

      keysDown = new Dictionary<Keys, double>();
      eventQueue = new EventQueue<KeyEventData>();
    }

    TimeSpan InitialDelay { get; } = TimeSpan.FromMilliseconds(500);

    TimeSpan RepeatDelay { get; } = TimeSpan.FromMilliseconds(50);

    public bool PullEventData(out KeyEventData data)
    {
      if (replay)
      {
        if (eventQueue.PullEventData(out data))
        {
          keysDown[data.Key] = (screen.Time.TotalGameTime + RepeatDelay).TotalSeconds;
          return true;
        }

        replay = false;
      }

      if (!source.PullEventData(out data))
      {
        // repeat events are fired when we run out of other events to process.
        // this way we dont end up fireing repeats when a key-up happens in the
        // same frame.
        replay = ScheduleRepeatEvents();
        return false;
      }

      if (data.EventType == KeyEventType.KeyReleased)
      {
        keysDown.Remove(data.Key);
      }
      else if (data.EventType == KeyEventType.KeyPressed)
      {
        keysDown[data.Key] = (screen.Time.TotalGameTime + InitialDelay).TotalSeconds;
      }

      return true;
    }

    bool ScheduleRepeatEvents()
    {
      var gameTime = screen.Time.TotalGameTime;
      if (gameTime == lastScheduleTime)
      {
        return false;
      }
      lastScheduleTime = gameTime;

      var now = gameTime.TotalSeconds;
      var retval = false;
      foreach (var entry in keysDown)
      {
        if (entry.Value <= now)
        {
          eventQueue.PushEvent(new KeyEventData(KeyEventType.KeyRepeat, gameTime, 0, InputFlags.None, entry.Key));
          retval = true;
        }
      }
      return retval;
    }
  }
}