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
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Steropes.UI.Input.GamePadInput;
using Steropes.UI.Input.KeyboardInput;
using Steropes.UI.Input.MouseInput;
using Steropes.UI.Input.TouchInput;

namespace Steropes.UI.Input
{
  /// <summary>
  ///  The input manager is responsible for polling the game instance and for populating the event queues.
  /// </summary>
  public interface IInputManager
  {
    IEventSource<GamePadEventData> GamePadSource { get; }

    IEventSource<KeyEventData> KeySource { get; }

    IEventSource<MouseEventData> MouseSource { get; }

    IEventSource<TouchEventData> TouchSource { get; }
  }

  public class InputManager : GameComponent, IInputManager
  {
    readonly List<IUpdateable> components;

    readonly EventQueue<GamePadEventData> gamePadEvents;

    readonly EventQueue<KeyEventData> keyEvents;

    readonly EventQueue<MouseEventData> mouseEvents;

    readonly EventQueue<TouchEventData> touchEvents;

    public InputManager(Game game) : base(game)
    {
      mouseEvents = new EventQueue<MouseEventData>();
      keyEvents = new EventQueue<KeyEventData>();
      touchEvents = new EventQueue<TouchEventData>();
      gamePadEvents = new EventQueue<GamePadEventData>();

      components = new List<IUpdateable>
                     {
                       new KeyboardComponent(keyEvents, game.Window),
                       new MouseInputHandler(mouseEvents),
                       new TouchInputHandler(touchEvents),
                       new GamePadComponent(gamePadEvents)
                     };
    }

    public IEventSource<GamePadEventData> GamePadSource => gamePadEvents;

    public IEventSource<KeyEventData> KeySource => keyEvents;

    public IEventSource<MouseEventData> MouseSource => mouseEvents;

    public IEventSource<TouchEventData> TouchSource => touchEvents;

    public override void Update(GameTime time)
    {
      for (var i = 0; i < components.Count; i++)
      {
        components[i].Update(time);
      }
    }
  }
}