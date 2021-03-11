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

using Microsoft.Xna.Framework.Input;

using Steropes.UI.Components;
using Steropes.UI.Input.KeyboardInput;
using Steropes.UI.Input.MouseInput;
using Steropes.UI.Platform;
using Steropes.UI.Widgets.TextWidgets;

namespace Steropes.UI.Widgets
{
  /// <summary>
  ///   Records a single key-stroke. Useful for menus that remap key-bindings.
  /// </summary>
  public class KeyBox : ContentWidget<TextField>
  {
    IUIFont font;

    KeyStroke key;

    public KeyBox(IUIStyle style, KeyStroke key) : base(style)
    {
      Content = new TextField(style) { Enabled = false, ReadOnly = true };
      Key = key;

      // does not participate in normal focus traversal 
      Focusable = false;

      KeyPressed += OnKeyPressed;
      MouseDown += OnMouseDown;
    }

    public Func<Keys, bool> ChangeHandler { get; set; }

    public IUIFont Font
    {
      get
      {
        return font;
      }

      set
      {
        font = value;
        InvalidateLayout();
      }
    }

    public KeyStroke Key
    {
      get
      {
        return key;
      }

      set
      {
        key = value;
        var vk = Screen?.WindowService?.VirtualKeyLocaliser ?? DefaultVirtualKeyLocaliser.Default;
        Content.Text = value.ToString(vk);
        InvalidateLayout();
        OnPropertyChanged();
      }
    }

    void OnKeyPressed(object sender, KeyEventArgs args)
    {
      args.Consume();

      var newKey = args.Key != Keys.Escape ? args.Key : Keys.None;
      if (newKey != Keys.None && (ChangeHandler == null || ChangeHandler(newKey)))
      {
        Key = new KeyStroke(newKey, args.Flags);
      }
    }

    void OnMouseDown(object sender, MouseEventArgs e)
    {
      this.RequestFocus();
      e.Consume();
    }
  }
}