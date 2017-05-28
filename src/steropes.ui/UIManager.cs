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
using Microsoft.Xna.Framework.Content;

using Steropes.UI.Components;
using Steropes.UI.Components.Window;
using Steropes.UI.Input;
using Steropes.UI.Platform;
using Steropes.UI.Styles;

namespace Steropes.UI
{
  public interface IUIManager
  {
    bool IsActive { get; }

    void Start();

    void Update(GameTime time);

    void Draw();

    IUIStyle UIStyle { get; }

    IRootPane Root { get; }

    IScreenService ScreenService { get; }
  }

  public class UIManager : IUIManager
  {
    readonly ContentManager contentManager;

    readonly IBatchedDrawingService drawingService;

    readonly IInputManager inputManager;

    readonly IGameWindowService windowService;

    public UIManager(IInputManager inputManager, IBatchedDrawingService drawingService, IGameWindowService windowService, ContentManager contentManager)
    {
      this.inputManager = inputManager;
      this.drawingService = drawingService;
      this.windowService = windowService;
      this.contentManager = contentManager;
    }

    public virtual bool IsActive => Screen?.WindowService?.Active ?? false;

    public Screen Screen { get; private set; }

    public IUIStyle UIStyle { get; private set; }

    public void Draw()
    {
      Screen.Draw();
    }

    public IRootPane Root => Screen.Root;

    public IScreenService ScreenService => Screen;

    public void Start()
    {
      UIStyle = new UIStyle(new ContentLoader(contentManager, drawingService.GraphicsDevice));
      Screen = new Screen(inputManager, UIStyle, drawingService, windowService);
    }

    public virtual void Update(GameTime time)
    {
      Screen.IsActive = IsActive;
      Screen.Update(time);
    }
  }
}