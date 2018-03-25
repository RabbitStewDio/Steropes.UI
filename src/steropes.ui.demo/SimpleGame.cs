// MIT License
//
// Copyright (c) 2011-2016 Elisée Maurer, Sparklin Labs, Creative Patterns
// Copyright (c) 2016 Thomas Morgner, Rabbit-StewDio Ltd.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
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

using Steropes.UI.Components.Window;
using Steropes.UI.Demo.Demos;
using Steropes.UI.Input;
using Steropes.UI.Styles;
using Steropes.UI.Util;

namespace Steropes.UI.Demo
{
  public class SimpleGame: Game
  {
    public SimpleGame()
    {
      Content.RootDirectory = "Content";

      Graphics = new GraphicsDeviceManager(this) { PreferredBackBufferWidth = 1280, PreferredBackBufferHeight = 720 };

      frameRateCalculator = new FrameRateCalculator();
    }

    GraphicsDeviceManager Graphics { get; }

    FrameRateCalculator frameRateCalculator;

    IUIManager uiManager;

    protected override void Initialize()
    {
      base.Initialize();

      IsMouseVisible = true;

      uiManager = UIManagerComponent.CreateAndInit(this, new InputManager(this), "Content").Manager;

      var styleSystem = uiManager.UIStyle;
      var styles = styleSystem.LoadStyles("Content/UI/Metro/style.xml", "UI/Metro", GraphicsDevice);
      styleSystem.StyleResolver.StyleRules.AddRange(styles);

      uiManager.Root.Content = WidgetDemo.CreateRootPanel(styleSystem);

      this.CenterOnScreen();
    }

    protected override void Update(GameTime gameTime)
    {
      frameRateCalculator.BeginTime();
      base.Update(gameTime);
      frameRateCalculator.EndTime();
      frameRateCalculator.Update(gameTime);
      uiManager.ScreenService.WindowService.Title = frameRateCalculator.ToString();
    }

    protected override void Draw(GameTime gameTime)
    {
      frameRateCalculator.BeginTime();
      base.Draw(gameTime);
      frameRateCalculator.Draw();
      frameRateCalculator.EndTime();
    }
  }
}