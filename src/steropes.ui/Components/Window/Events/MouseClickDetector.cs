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

using Microsoft.Xna.Framework;

using Steropes.UI.Input;
using Steropes.UI.Input.MouseInput;
using Steropes.UI.Util;

namespace Steropes.UI.Components.Window.Events
{
  /// <summary>
  ///   Detects mouse-clicks by monitoring the stream of events that pass by. A click only happens
  ///   if the mouse-down and mouse-up happen to occur on the same component, within a given time frame
  ///   and within a few pixels from the original mouse-down event.
  ///   If a click is detected, the "Clicked" event will be inserted immediately before the mouse-up event
  ///   to allow user code to correctly handle UI animations and to fully track the state of the mouse-button.
  /// </summary>
  public class MouseClickDetector : EventFilterBase<MouseEventData>, IComponentEventSource<MouseEventData, IWidget>
  {
    static readonly List<MouseButton> Buttons = MouseButtonExtensions.GetAll();

    readonly IComponentEventSource<MouseEventData, IWidget> eventSource;

    readonly EnumMap<MouseButton, MouseClickRecord> lastDownTime;

    readonly EventQueue<MouseEventData> queue;

    WeakReference<IWidget> lastComponentSeen;

    public MouseClickDetector(IComponentEventSource<MouseEventData, IWidget> eventSource)
    {
      DragThreshold = 4;
      DoubleClickDelay = 0.5f;

      queue = new EventQueue<MouseEventData>();
      this.eventSource = eventSource;

      lastDownTime = new EnumMap<MouseButton, MouseClickRecord>(Buttons);
      lastDownTime.Fill(MouseClickRecord.Invalid);

      lastComponentSeen = new WeakReference<IWidget>(null);
    }

    public IWidget Component => eventSource.Component;

    public float DoubleClickDelay { get; set; }

    public int DragThreshold { get; set; }

    protected override IEventSource<MouseEventData> Source => eventSource;

    public override bool PullEventData(out MouseEventData data)
    {
      if (queue.PullEventData(out data))
      {
        return true;
      }

      if (!eventSource.PullEventData(out data))
      {
        return false;
      }

      IWidget widget;
      if (!lastComponentSeen.TryGetTarget(out widget) || !ReferenceEquals(eventSource.Component, widget))
      {
        lastDownTime.Fill(MouseClickRecord.Invalid);
        lastComponentSeen = new WeakReference<IWidget>(eventSource.Component);
      }

      switch (data.EventType)
      {
        case MouseEventType.Down:
          HandleMouseDown(ref data);
          break;
        case MouseEventType.Up:
          HandleMouseUp(ref data);
          break;
      }

      return true;
    }

    void HandleMouseDown(ref MouseEventData data)
    {
      MouseClickRecord lastData;
      if (!lastDownTime.TryGet(data.Button, out lastData))
      {
        return;
      }

      if (IsWithinClickTime(data.Time.TotalSeconds, lastData.Time, lastData.Count) && IsWithinClickDistance(data.Position, lastData.Position))
      {
        // potential double click ..
        lastData.Count += 1;
        lastData.Time = data.Time.TotalSeconds;

        // not updating position ..
        lastDownTime[data.Button] = lastData;
      }
      else
      {
        // normal first click, eventually after double click timed out
        lastData.Count = 1;
        lastData.Time = data.Time.TotalSeconds;
        lastData.Position = data.Position;
        lastDownTime[data.Button] = lastData;
      }
    }

    void HandleMouseUp(ref MouseEventData data)
    {
      MouseClickRecord recordedClickTime;
      if (!lastDownTime.TryGet(data.Button, out recordedClickTime))
      {
        return;
      }

      if (IsWithinClickTime(data.Time.TotalSeconds, recordedClickTime.Time, recordedClickTime.Count - 1) && IsWithinClickDistance(data.Position, recordedClickTime.Position))
      {
        queue.PushEvent(data);
        data = data.ConvertInto(MouseEventType.Clicked, recordedClickTime.Count);
      }
    }

    bool IsWithinClickDistance(Point a, Point b)
    {
      // crude distance avoids square root.
      return Math.Abs(a.X - b.X) <= DragThreshold && Math.Abs(a.Y - b.Y) <= DragThreshold;
    }

    bool IsWithinClickTime(double currentTime, double recordedTime, int recordedClicks)
    {
      if (double.IsInfinity(recordedTime))
      {
        return false;
      }
      if (recordedClicks < 1)
      {
        return true;
      }
      return currentTime - recordedTime < DoubleClickDelay;
    }

    struct MouseClickRecord
    {
      public double Time { get; set; }

      public int Count { get; set; }

      public Point Position { get; set; }

      public static readonly MouseClickRecord Invalid = new MouseClickRecord { Time = float.PositiveInfinity, Count = 0, Position = new Point(int.MinValue, int.MinValue) };
    }
  }
}