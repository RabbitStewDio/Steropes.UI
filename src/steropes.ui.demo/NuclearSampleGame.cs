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

using Steropes.UI.Demo.GameStates;
using Steropes.UI.Input;
using Steropes.UI.Platform;
using Steropes.UI.State;
using Steropes.UI.Windows.UI.Window;

namespace Steropes.UI.Demo
{
  public class NuclearSampleGame : Game
  {
    public NuclearSampleGame()
    {
      Content.RootDirectory = "Content";

      Graphics = new GraphicsDeviceManager(this) { PreferredBackBufferWidth = 1280, PreferredBackBufferHeight = 720, SynchronizeWithVerticalRetrace = false};
      IsFixedTimeStep = false;
    }

    public GraphicsDeviceManager Graphics { get; }

    protected override void Initialize()
    {
      base.Initialize();


      Window.Title = "NuclearWinter Sample";
      Window.AllowUserResizing = true;
      IsMouseVisible = false;

      var inputManager = new InputManager(this);
      var drawingService = new BatchedDrawingService(this);

      // Game states; skipping intro state for debugging purposes ..
      var stateManager = new NamedGameStateManager(this);
      stateManager.States["Intro"] = new GameStateIntro(this, drawingService, stateManager, "Main");
      var windowService = new WindowsGameWindowService(this);
      
      // This requires Monogame-DirectX. Remove this line when on DesktopGL or Linux etc.. 
      windowService.MouseCursorService = new WinFormsMouseCursorService(this);
      stateManager.States["Main"] = new GameStateMainMenu(this, drawingService, inputManager, stateManager, windowService);
      stateManager.SwitchState(stateManager.States["Main"]);

      Components.Add(inputManager);
      Components.Add(stateManager);
      Components.Add(new DebugOverlayRenderer(this));

      
    }


  }
}