using Microsoft.Xna.Framework;
using Steropes.UI.Components;
using Steropes.UI.Input;
using Steropes.UI.Styles;
using Steropes.UI.Util;
using Steropes.UI.Widgets;
using Steropes.UI.Widgets.Container;

namespace Steropes.UI.Quickstart
{
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

      this.CenterOnScreen();
    }
  }
}