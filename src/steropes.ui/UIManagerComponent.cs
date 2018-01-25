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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Steropes.UI.Components.Window;
using Steropes.UI.Input;
using Steropes.UI.Platform;

namespace Steropes.UI
{
  public class UIManagerComponent : DrawableGameComponent
  {
    public IUIManager Manager { get; }

    public UIManagerComponent(Game game,
                              IUIManager manager) : base(game)
    {
      Manager = manager ?? throw new ArgumentNullException(nameof(manager));
    }

    public override void Update(GameTime gameTime)
    {
      Manager.Update(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {
      Manager.Draw();
    }

    public static UIManagerComponent Create(Game game, IInputManager inputManager, string rootDirectory = null)
    {
      rootDirectory = rootDirectory ?? "Content";

      var drawingService = new BatchedDrawingService(game);
      var windowService = new GameWindowService(game);
      var cm = new ContentManager(game.Services) { RootDirectory = rootDirectory };

      var uiManager = new UIManager(inputManager, drawingService, windowService, cm);
      return new UIManagerComponent(game, uiManager);
    }

    public static UIManagerComponent CreateAndInit(Game game, IInputManager inputManager, string rootDirectory = null)
    {
      var uiManagerComponent = Create(game, inputManager, rootDirectory);
      if (inputManager is IGameComponent inputManagerAsComponent)
      {
        game.Components.Add(inputManagerAsComponent);
      }
      game.Components.Add(uiManagerComponent);

      var uiManager = uiManagerComponent.Manager;
      uiManager.Start();
      return uiManagerComponent;
    }

  }
}