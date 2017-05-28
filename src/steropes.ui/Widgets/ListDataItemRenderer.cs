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
using Steropes.UI.Input;
using Steropes.UI.Input.KeyboardInput;
using Steropes.UI.Input.MouseInput;

namespace Steropes.UI.Widgets
{
  public class ListDataItemRenderer : ContentWidget<IWidget>, IListDataItemRenderer
  {
    bool selected;

    public ListDataItemRenderer(IUIStyle style) : base(style)
    {
      Anchor = AnchoredRect.CreateHorizontallyStretched();
      FocusedChanged += (s, e) =>
        {
          if (Focused)
          {
            OnSelection?.Invoke(this, ListSelectionEventArgs.Adjusted);
          }
        };

      MouseClicked += OnMouseClick;
      KeyPressed += OnKeyPressed;
    }

    public event EventHandler<ListSelectionEventArgs> OnSelection;

    public bool Selected
    {
      get
      {
        return selected;
      }
      set
      {
        if (value == selected)
        {
          return;
        }
        selected = value;
        OnPropertyChanged();
      }
    }

    void OnKeyPressed(object source, KeyEventArgs args)
    {
      if (args.Flags.IsAnyDown(InputFlags.Alt | InputFlags.Control | InputFlags.Meta))
      {
        return;
      }

      args.Consumed = true;
      if (args.Key == Keys.Space || args.Key == Keys.Enter)
      {
        OnSelection?.Invoke(this, ListSelectionEventArgs.Confirmed);
      }
    }

    void OnMouseClick(object source, MouseEventArgs args)
    {
      OnSelection?.Invoke(this, ListSelectionEventArgs.Confirmed);
      args.Consume();
    }
  }
}