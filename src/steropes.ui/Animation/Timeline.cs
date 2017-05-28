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

namespace Steropes.UI.Animation
{
  public struct TimelineEvent
  {
    public TimelineEvent(float time, Action action)
    {
      Time = time;
      Action = action;
    }

    public Action Action { get; }

    public float Time { get; }
  }

  public class Timeline
  {
    /// Time-sorted list of events
    readonly List<TimelineEvent> events;

    int eventOffset;

    public Timeline()
    {
      events = new List<TimelineEvent>();
    }

    public float Time { get; private set; }

    public void AddEvent(TimelineEvent evt)
    {
      events.Add(evt);
      events.Sort((a, b) => a.Time.CompareTo(b.Time));
    }

    public void Reset()
    {
      Time = 0f;
      eventOffset = 0;
    }

    public void Update(float elapsedTime)
    {
      Time += elapsedTime;

      while (eventOffset < events.Count && events[eventOffset].Time <= Time)
      {
        // NOTE: We must increment eventOffset before calling the
        // Action, in case the Action calls Reset() or does any other
        // change to the timeline
        eventOffset++;

        events[eventOffset - 1].Action();
      }
    }
  }
}