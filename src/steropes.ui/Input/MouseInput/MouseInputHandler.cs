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
using Microsoft.Xna.Framework.Input;

namespace Steropes.UI.Input.MouseInput
{
  public class MouseInputHandler : BasicGameComponent
  {
    static readonly List<MouseButton> Buttons = MouseButtonExtensions.GetAll();

    readonly IEventSink<MouseEventData> eventSink;

    InputFlags currentFlags;

    MouseState currentState;

    TimeSpan currentTime;

    int frame;

    bool initialized;

    MouseState previousState;

    public MouseInputHandler(IEventSink<MouseEventData> eventSink)
    {
      this.eventSink = eventSink;
    }

    public override void Update(GameTime gameTime)
    {
      frame += 1;
      currentTime = gameTime.TotalGameTime;
      currentFlags = InputFlagsHelper.Create(Keyboard.GetState(), currentState);
      previousState = currentState;
      currentState = Mouse.GetState();

      // collect some valid state, so on the _next_ update call we can detect changes and process events.
      if (!initialized)
      {
        initialized = true;
        return;
      }

      for (var index = 0; index < Buttons.Count; index++)
      {
        var button = Buttons[index];
        CheckButtonPressed(button);
        CheckButtonReleased(button);
      }

      if (currentState.Position != previousState.Position)
      {
        var args = new MouseEventData(MouseEventType.Moved, currentFlags, currentTime, frame, previousState, currentState);
        eventSink.PushEvent(args);
      }

      if (currentState.ScrollWheelValue != previousState.ScrollWheelValue)
      {
        var args = new MouseEventData(MouseEventType.WheelMoved, currentFlags, currentTime, frame, previousState, currentState);
        eventSink.PushEvent(args);
      }
    }

    void CheckButtonPressed(MouseButton button)
    {
      if (currentState.StateFor(button) == ButtonState.Pressed && previousState.StateFor(button) == ButtonState.Released)
      {
        var args = new MouseEventData(MouseEventType.Down, currentFlags, currentTime, frame, previousState, currentState, button);
        eventSink.PushEvent(args);
      }
    }

    void CheckButtonReleased(MouseButton button)
    {
      if (currentState.StateFor(button) == ButtonState.Released && previousState.StateFor(button) == ButtonState.Pressed)
      {
        var args = new MouseEventData(MouseEventType.Up, currentFlags, currentTime, frame, previousState, currentState, button);
        eventSink.PushEvent(args);
      }
    }
  }
}