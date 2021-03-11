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

using Steropes.UI.Components;
using Steropes.UI.Widgets.Container;

namespace Steropes.UI.Widgets
{
  public class CheckBox : ContentWidget<Widget>
  {
    public const string CheckBoxButtonStyleClass = "CheckBoxButton";

    public const string CheckBoxLabelStyleClass = "CheckBoxLabel";

    readonly Button checkMark;

    readonly Label label;

    public CheckBox(IUIStyle style) : base(style)
    {
      label = new Label(UIStyle) { Enabled = false };
      label.AddStyleClass(CheckBoxLabelStyleClass);

      checkMark = new Button(UIStyle) { Anchor = AnchoredRect.CreateCentered() };
      checkMark.AddStyleClass(CheckBoxButtonStyleClass);
      checkMark.ActionPerformed += OnActionPerformed;
      Content = new DockPanel(style) { { checkMark, DockPanelConstraint.Left }, { label, DockPanelConstraint.Left } };

      Selected = SelectionState.Selected;
    }

    public SelectionState Selected
    {
      get
      {
        return checkMark.Selected;
      }
      set
      {
        checkMark.Selected = value;
      }
    }

    public string Text
    {
      get
      {
        return label.Text;
      }
      set
      {
        label.Text = value;
      }
    }

    void OnActionPerformed(object sender, EventArgs e)
    {
      checkMark.Selected = checkMark.Selected == SelectionState.Selected ? SelectionState.Unselected : SelectionState.Selected;
    }
  }
}