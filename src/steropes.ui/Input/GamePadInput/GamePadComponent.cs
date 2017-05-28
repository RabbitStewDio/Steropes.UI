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
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Steropes.UI.Input.GamePadInput
{
  public class GamePadComponent : BasicGameComponent
  {
    static readonly List<Buttons> AllGamePadButtons = Enum.GetValues(typeof(Buttons)).Cast<Buttons>().ToList();

    static readonly List<Buttons> ExcludedButtons = new List<Buttons>
                                                      {
                                                        Buttons.LeftTrigger,
                                                        Buttons.RightTrigger,
                                                        Buttons.LeftThumbstickDown,
                                                        Buttons.LeftThumbstickUp,
                                                        Buttons.LeftThumbstickRight,
                                                        Buttons.LeftThumbstickLeft,
                                                        Buttons.RightThumbstickLeft,
                                                        Buttons.RightThumbstickRight,
                                                        Buttons.RightThumbstickUp,
                                                        Buttons.RightThumbstickDown
                                                      };

    static readonly List<Buttons> NonTriggerButtons = AllGamePadButtons.Where(b => !ExcludedButtons.Contains(b)).ToList();

    readonly GamePadState[] currentStates;

    readonly IEventSink<GamePadEventData> eventSink;

    readonly GamePadState[] previousStates;

    TimeSpan currentTime;

    int frame;

    bool initialized;

    InputFlags inputFlags;

    public GamePadComponent(IEventSink<GamePadEventData> eventSink)
    {
      this.eventSink = eventSink;
      TriggerDownTreshold = 0.15f;
      ThumbstickDownTreshold = 0.5f;
      TriggerDeltaTreshold = 0.1f;
      ThumbStickDeltaTreshold = 0.1f;

      currentStates = new GamePadState[GamePad.MaximumGamePadCount];
      previousStates = new GamePadState[GamePad.MaximumGamePadCount];
    }

    /// <summary>
    ///   The treshold of movement that has to be met in order
    ///   for the listener to fire an event with the thumbstick's
    ///   updated position.
    ///   <para>
    ///     In essence this defines the event's
    ///     resolution.
    ///   </para>
    ///   At a value of 0 this will fire every time
    ///   the thumbstick's position is not {x:0, y:0}.
    /// </summary>
    public float ThumbStickDeltaTreshold { get; }

    /// <summary>
    ///   How deep the triggers have to be depressed in order to
    ///   register as a ButtonDown event.
    /// </summary>
    public float ThumbstickDownTreshold { get; }

    /// <summary>
    ///   The treshold of movement that has to be met in order
    ///   for the listener to fire an event with the trigger's
    ///   updated position.
    ///   <para>
    ///     In essence this defines the event's
    ///     resolution.
    ///   </para>
    ///   At a value of 0 this will fire every time
    ///   the trigger's position is not 0f.
    /// </summary>
    public float TriggerDeltaTreshold { get; }

    /// <summary>
    ///   How deep the triggers have to be depressed in order to
    ///   register as a ButtonDown event.
    /// </summary>
    public float TriggerDownTreshold { get; }

    public override void Update(GameTime gameTime)
    {
      frame += 1;
      currentTime = gameTime.TotalGameTime;
      inputFlags = InputFlagsHelper.Create();

      for (var p = 0; p < currentStates.Length; p += 1)
      {
        previousStates[p] = currentStates[p];
        currentStates[p] = GamePad.GetState(p);

        // collect some valid state, so on the _next_ update call we can detect changes and process events.
        if (!initialized)
        {
          initialized = true;
          return;
        }

        CheckAllButtons(p);
      }
    }

    void CheckAllButtons(int player)
    {
      var currentState = currentStates[player];
      var previousState = previousStates[player];

      for (var i = 0; i < NonTriggerButtons.Count; i++)
      {
        var button = NonTriggerButtons[i];
        if (currentState.IsButtonDown(button) && previousState.IsButtonUp(button))
        {
          eventSink.PushEvent(new GamePadEventData(GamePadEventType.ButtonDown, currentTime, frame, inputFlags, button));
        }

        if (currentState.IsButtonUp(button) && previousState.IsButtonDown(button))
        {
          eventSink.PushEvent(new GamePadEventData(GamePadEventType.ButtonUp, currentTime, frame, inputFlags, button));
        }
      }

      // Checks triggers as buttons and floats
      CheckTriggers(currentState.Triggers.Left, previousState.Triggers.Left, Buttons.LeftTrigger);
      CheckTriggers(currentState.Triggers.Right, previousState.Triggers.Right, Buttons.RightTrigger);

      // Checks thumbsticks as vector2s
      CheckThumbSticks(currentState.ThumbSticks.Right, previousState.ThumbSticks.Right, Buttons.RightStick);
      CheckThumbSticks(currentState.ThumbSticks.Left, previousState.ThumbSticks.Left, Buttons.LeftStick);
    }

    void CheckThumbSticks(Vector2 curVector, Vector2 prevVector, Buttons button)
    {
      const float Debounce = 0.15f;
      var curdown = curVector.Length() > ThumbstickDownTreshold;
      var prevdown = prevVector.Length() > ThumbstickDownTreshold;

      var curdir = ComputeThumbStickDirection(curVector, button);
      var prevdir = ComputeThumbStickDirection(prevVector, button);

      if (!prevdown && curdown)
      {
        eventSink.PushEvent(new GamePadEventData(GamePadEventType.ButtonDown, currentTime, frame, inputFlags, curdir));
      }
      else if (prevdown && curVector.Length() < Debounce)
      {
        eventSink.PushEvent(new GamePadEventData(GamePadEventType.ButtonUp, currentTime, frame, inputFlags, prevdir));
      }
      else if (prevdown && curdown && curdir != prevdir)
      {
        eventSink.PushEvent(new GamePadEventData(GamePadEventType.ButtonUp, currentTime, frame, inputFlags, prevdir));
        eventSink.PushEvent(new GamePadEventData(GamePadEventType.ButtonDown, currentTime, frame, inputFlags, curdir));
      }

      if (curVector.Length() > ThumbStickDeltaTreshold)
      {
        if (Vector2.Distance(curVector, prevVector) >= ThumbStickDeltaTreshold)
        {
          eventSink.PushEvent(new GamePadEventData(GamePadEventType.ThumbStickMoved, currentTime, frame, inputFlags, button, curVector));
        }
      }
      else if (prevVector.Length() > ThumbStickDeltaTreshold)
      {
        eventSink.PushEvent(new GamePadEventData(GamePadEventType.ThumbStickMoved, currentTime, frame, inputFlags, button, curVector));
      }
    }

    void CheckTriggers(float curstate, float prevstate, Buttons button)
    {
      var debounce = 0.05f; // Value used to qualify a trigger as coming Up from a Down state
      var curdown = curstate > TriggerDownTreshold;
      var prevdown = prevstate > TriggerDownTreshold;

      if (!prevdown && curdown)
      {
        eventSink.PushEvent(new GamePadEventData(GamePadEventType.ButtonDown, currentTime, frame, inputFlags, button));
      }
      else if (prevdown && curstate < debounce)
      {
        eventSink.PushEvent(new GamePadEventData(GamePadEventType.ButtonUp, currentTime, frame, inputFlags, button));
      }

      if (curstate > TriggerDeltaTreshold)
      {
        if (Math.Abs(prevstate - curstate) >= TriggerDeltaTreshold)
        {
          eventSink.PushEvent(new GamePadEventData(GamePadEventType.TriggerMoved, currentTime, frame, inputFlags, button, curstate));
        }
      }
      else if (prevstate > TriggerDeltaTreshold)
      {
        eventSink.PushEvent(new GamePadEventData(GamePadEventType.TriggerMoved, currentTime, frame, inputFlags, button, curstate));
      }
    }

    Buttons ComputeThumbStickDirection(Vector2 curVector, Buttons baseButton)
    {
      var right = baseButton == Buttons.RightStick;

      if (curVector.Y > curVector.X)
      {
        if (curVector.Y > -curVector.X)
        {
          return right ? Buttons.RightThumbstickUp : Buttons.LeftThumbstickUp;
        }
        return right ? Buttons.RightThumbstickLeft : Buttons.LeftThumbstickLeft;
      }

      if (curVector.Y < -curVector.X)
      {
        return right ? Buttons.RightThumbstickDown : Buttons.LeftThumbstickDown;
      }
      return right ? Buttons.RightThumbstickRight : Buttons.LeftThumbstickRight;
    }
  }
}