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
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Steropes.UI.Input.KeyboardInput
{
  public class KeyboardComponent : BasicGameComponent
  {
    static readonly Keys[] enumerableKeys = (Keys[]) Enum.GetValues(typeof(Keys));

    readonly IEventSink<KeyEventData> eventSink;

    readonly Queue<KeyEventData> typedCharacters;

    InputFlags currentFlags;

    KeyboardState currentState;

    TimeSpan currentTime;

    int frame;

    bool initialized;

    KeyboardState previousState;

    public KeyboardComponent(IEventSink<KeyEventData> eventSink, GameWindow gameWindow)
    {
      this.eventSink = eventSink;
      typedCharacters = new Queue<KeyEventData>();
      gameWindow.TextInput += TextInputReceived;
    }

    public override void Update(GameTime gameTime)
    {
      frame += 1;
      currentTime = gameTime.TotalGameTime;
      previousState = currentState;
      currentState = Keyboard.GetState();
      currentFlags = InputFlagsHelper.Create(currentState, Mouse.GetState());

      // collect some valid state, so on the _next_ update call we can detect changes and process events.
      if (!initialized)
      {
        initialized = true;
        return;
      }

      RaiseEvents();
      DrainTypedCharacters();
    }

    void DrainTypedCharacters()
    {
      KeyEventData data;
      while (typedCharacters.Pop(out data))
      {
        eventSink.PushEvent(data);
      }
    }

    void RaiseEvents()
    {
      for (var i = 0; i < enumerableKeys.Length; i++)
      {
        var key = enumerableKeys[i];
        if (currentState.IsKeyDown(key) && previousState.IsKeyUp(key))
        {
          var keyEventData = new KeyEventData(KeyEventType.KeyPressed, currentTime, frame, currentFlags, key);
          eventSink.PushEvent(keyEventData);
        }

        if (currentState.IsKeyUp(key) && previousState.IsKeyDown(key))
        {
          var keyEventData = new KeyEventData(KeyEventType.KeyReleased, currentTime, frame, currentFlags, key);
          eventSink.PushEvent(keyEventData);
        }
      }
    }

    void TextInputReceived(object sender, TextInputEventArgs args)
    {
      if (!initialized)
      {
        // ignore early events..
        return;
      }

      var keyEventData = new KeyEventData(KeyEventType.KeyTyped, currentTime, frame, currentFlags, args.Character);
      typedCharacters.Enqueue(keyEventData);
    }
  }
}