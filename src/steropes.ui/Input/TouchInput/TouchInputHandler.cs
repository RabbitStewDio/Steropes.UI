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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace Steropes.UI.Input.TouchInput
{
  public class TouchInputHandler : BasicGameComponent
  {
    readonly IEventSink<TouchEventData> eventSink;
    readonly Vector2[] gestureBuffer;
    int frame;
    Matrix transform;
    bool hasTransform;

    public TouchInputHandler(IEventSink<TouchEventData> eventSink)
    {
      this.eventSink = eventSink;
      this.Transform = Matrix.Identity;
      this.gestureBuffer = new Vector2[4];
    }

    public Matrix Transform
    {
      get { return transform; }
      set
      {
        transform = value;
        hasTransform = transform != Matrix.Identity;
      }
    }

    TouchCollection TransformState(TouchCollection input)
    {
      if (input.Count == 0 || hasTransform == false)
      {
        return input;
      }
      
      var retval = new TouchCollection();
      foreach (var touchLocation in input)
      {
        retval.Add(TransformLocation(touchLocation));
      }

      return retval;
    }

    TouchLocation TransformLocation(TouchLocation touchLocation)
    {
      if (touchLocation.TryGetPreviousLocation(out var prev))
      {
        var prevTransformed = TransformLocation(prev);
        var pos = Vector2.Transform(touchLocation.Position, transform);
        var state = touchLocation.State;
        return new TouchLocation(touchLocation.Id, state, pos, prevTransformed.State, prevTransformed.Position);
      }
      else
      {
        var pos = Vector2.Transform(touchLocation.Position, transform);
        var state = touchLocation.State;
        return new TouchLocation(touchLocation.Id, state, pos);
      }
    }

    GestureSample TransformGesture(GestureSample input)
    {
      if (!hasTransform)
      {
        return input;
      }

      gestureBuffer[0] = input.Position;
      gestureBuffer[1] = input.Position + input.Delta;
      gestureBuffer[2] = input.Position2;
      gestureBuffer[3] = input.Position2 + input.Delta2;
      Vector2.Transform(gestureBuffer, ref transform, gestureBuffer);
      return new GestureSample(input.GestureType, input.Timestamp, 
                               gestureBuffer[0], gestureBuffer[2], 
                               gestureBuffer[1] - gestureBuffer[0], 
                               gestureBuffer[2] - gestureBuffer[3]);
    }

    public override void Update(GameTime gameTime)
    {
      frame += 1;

      var currentTime = gameTime.TotalGameTime;
      var currentFlags = InputFlagsHelper.Create(Keyboard.GetState(), Mouse.GetState());

      var touchCollection = TransformState(TouchPanel.GetState());

      for (var index = 0; index < touchCollection.Count; index++)
      {
        var touchLocation = touchCollection[index];
        switch (touchLocation.State)
        {
          case TouchLocationState.Pressed:
            eventSink.PushEvent(new TouchEventData(currentTime, frame, currentFlags, TouchEventType.Pressed, touchLocation));
            break;
          case TouchLocationState.Moved:
            eventSink.PushEvent(new TouchEventData(currentTime, frame, currentFlags, TouchEventType.Moved, touchLocation));
            break;
          case TouchLocationState.Released:
            eventSink.PushEvent(new TouchEventData(currentTime, frame, currentFlags, TouchEventType.Released, touchLocation));
            break;
          case TouchLocationState.Invalid:
            eventSink.PushEvent(new TouchEventData(currentTime, frame, currentFlags, TouchEventType.Cancelled, touchLocation));
            break;
        }
      }

      while (TouchPanel.IsGestureAvailable)
      {
        var gesture = hasTransform ? TransformGesture(TouchPanel.ReadGesture()) : TouchPanel.ReadGesture();
        eventSink.PushEvent(new TouchEventData(currentTime, frame, currentFlags, TouchEventType.Gestured, gesture));
      }
    }
  }
}