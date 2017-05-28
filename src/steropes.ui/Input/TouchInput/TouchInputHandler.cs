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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace Steropes.UI.Input.TouchInput
{
  public class TouchInputHandler : BasicGameComponent
  {
    readonly IEventSink<TouchEventData> eventSink;

    int frame;

    public TouchInputHandler(IEventSink<TouchEventData> eventSink)
    {
      this.eventSink = eventSink;
    }

    public override void Update(GameTime gameTime)
    {
      frame += 1;

      var currentTime = gameTime.TotalGameTime;
      var currentFlags = InputFlagsHelper.Create(Keyboard.GetState(), Mouse.GetState());

      var touchCollection = TouchPanel.GetState();

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
        var gesture = TouchPanel.ReadGesture();
        eventSink.PushEvent(new TouchEventData(currentTime, frame, currentFlags, TouchEventType.Gestured, gesture));
      }
    }
  }
}