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
using System.IO;
using System.Xml.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

using Steropes.UI.Components;
using Steropes.UI.Components.Window;
using Steropes.UI.Demo.Demos;
using Steropes.UI.Input;
using Steropes.UI.Platform;
using Steropes.UI.State;
using Steropes.UI.Styles;
using Steropes.UI.Util;
using Steropes.UI.Widgets;
using Steropes.UI.Widgets.Container;

namespace Steropes.UI.Demo.GameStates
{
  class GameStateMainMenu : GameStateFadeTransition
  {
    struct StyleDefinition
    {
      public string Name { get; }
      public List<IStyleRule> Rules { get; }
      public IUITexture WhitePixel { get; }

      public StyleDefinition(string name, List<IStyleRule> rules, IUITexture whitePixel)
      {
        this.WhitePixel = whitePixel;
        Name = name;
        Rules = rules;
      }



      public override string ToString()
      {
        return Name;
      }
    }

    readonly FrameRateCalculator frameRateCalculator;

    readonly IInputManager inputManager;

    readonly IGameStateManager stateService;

    readonly IGameWindowService windowService;

    readonly List<StyleDefinition> styles;

    public GameStateMainMenu(Game game, IBatchedDrawingService drawingService, IInputManager inputManager, IGameStateManager stateService, IGameWindowService windowService)
      : base(drawingService)
    {
      this.frameRateCalculator = new FrameRateCalculator();
      this.inputManager = inputManager;
      this.stateService = stateService;
      this.windowService = windowService;
      Game = game;

      styles = new List<StyleDefinition>();
    }

    Game Game { get; }

    IUIManager UIManager { get; set; }

    public override void Draw(GameTime time)
    {
      frameRateCalculator.BeginTime();
      Game.GraphicsDevice.Clear(Color.Black);

      UIManager.Draw();
      frameRateCalculator.EndTime();
    }

    StyleDefinition Load(string context, string filename)
    {
      var rules = UIManager.UIStyle.StyleSystem.WithContext(context).CreateParser(Game.GraphicsDevice)
        .Read(XDocument.Load(filename));
      return new StyleDefinition("Metro", rules, UIManager.UIStyle.StyleSystem.WhitePixel);
    }

    public override void Start()
    {
      var content = new ContentManager(Game.Services) { RootDirectory = "Content" };
      UIManager = new StatefulUIManager(inputManager, DrawingService, windowService, content, stateService);
      UIManager.Start();

      styles.Clear();
      styles.Add(Load("UI/Metro", "Content/UI/Metro/style.xml"));
      styles.Add(Load("UI/NuclearWinter", "Content/UI/NuclearWinter/style.xml"));

      UIManager.UIStyle.StyleResolver.StyleRules.Clear();
      UIManager.UIStyle.StyleResolver.StyleRules.AddRange(styles[0].Rules);

      UIManager.Root.Content = CreateContentPane(UIManager.UIStyle);

      windowService.MouseVisible = true;
      base.Start();
    }

    Widget CreateContentPane(IUIStyle style)
    {
      Grid g = new Grid(style);
      g.ColumnConstraints.Add(LengthConstraint.Percentage(100));
      g.RowConstraints.Add(LengthConstraint.Auto);
      g.RowConstraints.Add(LengthConstraint.Relative(1));

      g.Add(CreateStyleSelector(style), new Point(0, 0));
      g.Add(WidgetDemo.CreateRootPanel(style), new Point(0, 1));
      return g;
    }

    Widget CreateStyleSelector(IUIStyle uiStyle)
    {
      RadioButtonSet<StyleDefinition> radioButtons = new RadioButtonSet<StyleDefinition>(uiStyle);
      foreach (var style in styles)
      {
        radioButtons.DataItems.Add(style);
      }
      radioButtons.SelectionChanged += (sender, args) =>
        {
          if (radioButtons.LookUpSelectedItem(out var d))
          {
            uiStyle.StyleResolver.StyleRules.Clear();
            uiStyle.StyleResolver.StyleRules.AddRange(d.Rules);
            DrawingService.WhitePixel = d.WhitePixel;
            UIManager.UIStyle.StyleSystem.WhitePixel = d.WhitePixel;
          }
        };

      return new BoxGroup(uiStyle, Orientation.Vertical, 0) { radioButtons };
    }

    public override void Stop()
    {
      UIManager = null;
      windowService.MouseVisible = false;

      base.Stop();
    }

    public override void Update(GameTime elapsedTime)
    {
      frameRateCalculator.BeginTime();
      UIManager.Update(elapsedTime);
      frameRateCalculator.EndTime();
      frameRateCalculator.Update(elapsedTime);

      UIManager.ScreenService.WindowService.Title = frameRateCalculator.ToString();
    }
  }
}