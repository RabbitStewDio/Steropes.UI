// Steopes-UI quickstart by Thomas Morgner and others
// 
// To the extent possible under law, the person who associated CC0 with the
// Steropes-UI quickstart module has waived all copyright and related or neighboring rights
// to the Steropes-UI quickstart module. 
//
// You should have received a copy of the CC0 legalcode along with this
// work.If not, see<http://creativecommons.org/publicdomain/zero/1.0/>.

using Microsoft.Xna.Framework;
using Steropes.UI.Components;
using Steropes.UI.Input;
using Steropes.UI.Styles;
using Steropes.UI.Util;
using Steropes.UI.Widgets;
using Steropes.UI.Widgets.Container;

namespace Steropes.UI.Quickstart
{
  class Program
  {
    public static void Main(string[] args)
    {
      using (var game = new SimpleGame())
      {
        game.Run();
      }
    }
  }

  class SimpleGame: Game
  {
    public GraphicsDeviceManager Graphics { get; private set; }

    public SimpleGame()
    {
      Content.RootDirectory = "Content";
      Graphics = new GraphicsDeviceManager(this) { PreferredBackBufferWidth = 1280, PreferredBackBufferHeight = 720 };
    }

    protected override void Initialize()
    {
      base.Initialize();

      IsMouseVisible = true;

      var uiManager = UIManagerComponent.CreateAndInit(this, new InputManager(this), "Content").Manager;

      var styleSystem = uiManager.UIStyle;
      var styles = styleSystem.LoadStyles("Content/UI/Metro/style.xml", "UI/Metro", GraphicsDevice);
      styleSystem.StyleResolver.StyleRules.AddRange(styles);

      uiManager.Root.Content = new Group(styleSystem)
      {
        new Label(styleSystem)
        {
          Text = "Hello World",
          Anchor = AnchoredRect.CreateCentered()
        }
      };
    }
  }
}