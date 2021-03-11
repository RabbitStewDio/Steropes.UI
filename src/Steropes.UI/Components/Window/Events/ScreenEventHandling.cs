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

using Steropes.UI.Input;
using Steropes.UI.Input.GamePadInput;
using Steropes.UI.Input.KeyboardInput;
using Steropes.UI.Input.MouseInput;
using Steropes.UI.Input.TouchInput;

namespace Steropes.UI.Components.Window.Events
{
  public class ScreenEventHandling
  {
    readonly IComponentEventSource<GamePadEventData, IWidget> gamePadInputs;

    readonly List<IComponentEventSink<GamePadEventData>> gamePadPostProcessors;

    readonly List<IComponentEventSink<GamePadEventData>> gamePadPreProcessors;

    readonly IComponentEventSource<KeyEventData, IWidget> keyInputs;

    readonly List<IComponentEventSink<KeyEventData>> keyPostProcessors;

    readonly List<IComponentEventSink<KeyEventData>> keyPreProcessors;

    readonly IComponentEventSource<MouseEventData, IWidget> mouseInputs;

    readonly List<IComponentEventSink<MouseEventData>> mousePostProcessors;

    readonly List<IComponentEventSink<MouseEventData>> mousePreProcessors;

    readonly IWidget root;

    readonly IScreenService screenService;

    readonly IComponentEventSource<TouchEventData, IWidget> touchInputs;

    readonly List<IComponentEventSink<TouchEventData>> touchPostProcessors;

    readonly List<IComponentEventSink<TouchEventData>> touchPreProcessors;

    public ScreenEventHandling(IScreenService screenService, IWidget root, IInputManager rawInputs)
    {
      this.screenService = screenService;
      this.root = root;

      mousePostProcessors = new List<IComponentEventSink<MouseEventData>>();
      mousePreProcessors = new List<IComponentEventSink<MouseEventData>>();
      touchPreProcessors = new List<IComponentEventSink<TouchEventData>>();
      touchPostProcessors = new List<IComponentEventSink<TouchEventData>>();
      keyPostProcessors = new List<IComponentEventSink<KeyEventData>>();
      keyPreProcessors = new List<IComponentEventSink<KeyEventData>>();
      gamePadPostProcessors = new List<IComponentEventSink<GamePadEventData>>();
      gamePadPreProcessors = new List<IComponentEventSink<GamePadEventData>>();

      mouseInputs = ConfigureMouseInput(rawInputs.MouseSource);
      keyInputs = ConfigureKeyInput(rawInputs.KeySource);
      gamePadInputs = ConfigureGamePadInput(rawInputs.GamePadSource);
      touchInputs = ConfigureTouchInput(rawInputs.TouchSource);
    }

    public void AddGamePadPostProcessor(IComponentEventSink<GamePadEventData> filter)
    {
      gamePadPostProcessors.Add(filter);
    }

    public void AddGamePadPreProcessor(IComponentEventSink<GamePadEventData> filter)
    {
      gamePadPreProcessors.Add(filter);
    }

    public void AddTouchPostProcessor(IComponentEventSink<TouchEventData> filter)
    {
      touchPostProcessors.Add(filter);
    }

    public void AddTouchPreProcessor(IComponentEventSink<TouchEventData> filter)
    {
      touchPreProcessors.Add(filter);
    }

    public void AddKeyPostProcessor(IComponentEventSink<KeyEventData> filter)
    {
      keyPostProcessors.Add(filter);
    }

    public void AddKeyPreProcessor(IComponentEventSink<KeyEventData> filter)
    {
      keyPreProcessors.Add(filter);
    }

    public void AddMousePostProcessor(IComponentEventSink<MouseEventData> filter)
    {
      mousePostProcessors.Add(filter);
    }

    public void AddMousePreProcessor(IComponentEventSink<MouseEventData> filter)
    {
      mousePreProcessors.Add(filter);
    }

    public void ProcessEvents(GameTime time)
    {
      PumpKeys();
      PumpGamePad();
      PumpMouse();
      PumpTouch();
    }

    IComponentEventSource<GamePadEventData, IWidget> ConfigureGamePadInput(IEventSource<GamePadEventData> source)
    {
      return new FocusedComponentMapper<GamePadEventData>(new SwallowEventsWhenInactiveFilter<GamePadEventData>(source, screenService), screenService.FocusManager);
    }

    IComponentEventSource<KeyEventData, IWidget> ConfigureKeyInput(IEventSource<KeyEventData> source)
    {
      return
        new FocusedComponentMapper<KeyEventData>(
          new LocaliseKeysEventFilter(
            new SwallowEventsWhenInactiveFilter<KeyEventData>(new RepeatKeysEventFilter(source, screenService), screenService),
            screenService.WindowService.VirtualKeyLocaliser),
          screenService.FocusManager);
    }

    IComponentEventSource<MouseEventData, IWidget> ConfigureMouseInput(IEventSource<MouseEventData> source)
    {
      return new MouseClickDetector(new MouseToComponentMapper(new SwallowEventsWhenInactiveFilter<MouseEventData>(source, screenService), screenService.MouseHoverManager, root));
    }

    IComponentEventSource<TouchEventData, IWidget> ConfigureTouchInput(IEventSource<TouchEventData> source)
    {
      return new TouchToComponentMapper(new SwallowEventsWhenInactiveFilter<TouchEventData>(source, screenService), screenService.MouseHoverManager, root);
    }

    void Process<T>(List<IComponentEventSink<T>> eventSinks, T eventData, IWidget target) where T : struct
    {
      for (var index = 0; index < eventSinks.Count; index++)
      {
        var filter = eventSinks[index];
        filter.PushEvent(eventData, target);
      }
    }

    void PumpGamePad()
    {
      GamePadEventData data;
      while (gamePadInputs.PullEventData(out data))
      {
        var target = gamePadInputs.Component;
        Process(gamePadPreProcessors, data, target);
        if (target != null)
        {
          var args = new GamePadEventArgs(data);
          target.DispatchEvent(args);
        }

        Process(gamePadPostProcessors, data, target);
      }
    }

    void PumpKeys()
    {
      KeyEventData data;
      while (keyInputs.PullEventData(out data))
      {
        var target = keyInputs.Component;
        Process(keyPreProcessors, data, target);
        if (target != null)
        {
          var args = new KeyEventArgs(data);
          target.DispatchEvent(args);
        }

        Process(keyPostProcessors, data, target);
      }
    }

    void PumpMouse()
    {
      MouseEventData data;
      while (mouseInputs.PullEventData(out data))
      {
        var target = mouseInputs.Component;
        Process(mousePreProcessors, data, target);
        if (target != null)
        {
          var args = new MouseEventArgs(target, data);
          target.DispatchEvent(args);
        }

        Process(mousePostProcessors, data, target);
      }
    }

    void PumpTouch()
    {
      TouchEventData data;
      while (touchInputs.PullEventData(out data))
      {
        var target = touchInputs.Component;
        Process(touchPreProcessors, data, target);
        if (target != null)
        {
          var args = new TouchEventArgs(target, data);
          target.DispatchEvent(args);
        }

        Process(touchPostProcessors, data, target);
      }
    }
  }
}