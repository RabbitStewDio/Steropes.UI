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

using Steropes.UI.Input.KeyboardInput;

namespace Steropes.UI.Components.Window
{
  public interface IGameWindowService : IMouseCursorService
  {
    bool Active { get; }

    IClipboardService Clipboard { get; }

    string Title { get; set; }

    IVirtualKeyLocaliser VirtualKeyLocaliser { get; }
  }

  public class EmptyWindowService : IGameWindowService
  {
    public EmptyWindowService()
    {
      Clipboard = new Clipboard();
    }

    public static IGameWindowService Instance { get; } = new EmptyWindowService();

    public bool Active => true;

    public IClipboardService Clipboard { get; set; }

    public MouseCursor Cursor { get; set; }

    public bool MouseVisible { get; set; }

    public string Title { get; set; }

    public IVirtualKeyLocaliser VirtualKeyLocaliser => DefaultVirtualKeyLocaliser.Default;
  }

  public class GameWindowService : IGameWindowService
  {
    readonly Game game;

    public GameWindowService(Game game)
    {
      this.game = game;
      Clipboard = new Clipboard();
      VirtualKeyLocaliser = DefaultVirtualKeyLocaliser.Default;
      MouseCursorService = new GameMouseCursorService(game);
    }

    public bool Active => game.IsActive;

    public IClipboardService Clipboard { get; set; }

    public MouseCursor Cursor
    {
      get
      {
        return MouseCursorService.Cursor;
      }
      set
      {
        MouseCursorService.Cursor = value;
      }
    }

    public IMouseCursorService MouseCursorService { get; set; }

    public bool MouseVisible
    {
      get
      {
        return MouseCursorService.MouseVisible;
      }
      set
      {
        MouseCursorService.MouseVisible = value;
      }
    }

    public string Title
    {
      get
      {
        return game.Window.Title;
      }
      set
      {
        game.Window.Title = value;
      }
    }

    public IVirtualKeyLocaliser VirtualKeyLocaliser { get; set; }
  }
}