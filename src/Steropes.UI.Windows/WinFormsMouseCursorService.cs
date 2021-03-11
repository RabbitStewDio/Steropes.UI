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
using System.Windows.Forms;

using Microsoft.Xna.Framework;

using Steropes.UI.Components.Window;

namespace Steropes.UI.Windows.UI.Window
{
  /// <summary>
  ///   Use this when using the DirectX Monogame platform.
  ///   <p />
  ///   This will not work for the Desktop-GL platform. OpenTK uses the low-level native windows API
  ///   to create a window and allows no hooks to actually change the cursor from its arrow shape.
  ///   For DesktopGL you have to use your own implementation.
  /// </summary>
  public class WinFormsMouseCursorService : IMouseCursorService
  {
    readonly Control control;

    readonly Dictionary<MouseCursor, Cursor> cursorMapping;

    readonly Game game;

    MouseCursor cursor;

    public WinFormsMouseCursorService(Game game)
    {
      this.game = game;
      control = Control.FromHandle(game.Window.Handle);
      cursorMapping = new Dictionary<MouseCursor, Cursor>
                        {
                          { MouseCursor.Default, Cursors.Arrow },
                          { MouseCursor.SizeWE, Cursors.SizeWE },
                          { MouseCursor.SizeNS, Cursors.SizeNS },
                          { MouseCursor.SizeAll, Cursors.SizeAll },
                          { MouseCursor.Cross, Cursors.Cross },
                          { MouseCursor.Hand, Cursors.Hand },
                          { MouseCursor.IBeam, Cursors.IBeam }
                        };
    }

    public MouseCursor Cursor
    {
      get
      {
        return cursor;
      }
      set
      {
        cursor = value;
        if (control != null)
        {
          control.Cursor = cursorMapping[value];
        }
      }
    }

    public bool MouseVisible
    {
      get
      {
        return game.IsMouseVisible;
      }
      set
      {
        game.IsMouseVisible = true;
      }
    }
  }
}