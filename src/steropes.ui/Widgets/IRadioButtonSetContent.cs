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
using Steropes.UI.Platform;

namespace Steropes.UI.Widgets
{
  public interface IRadioButtonSetContent : IWidget
  {
    event EventHandler<EventArgs> ActionPerformed;

    bool First { get; set; }

    bool Last { get; set; }

    SelectionState Selected { get; set; }
  }

  public class RadioButtonSetContent : Button, IRadioButtonSetContent
  {
    public static readonly string StyleClass = "RadioButtonSetContent";

    bool first;

    bool last;

    public RadioButtonSetContent(IUIStyle style, string text = "", IUITexture iconTex = null) : base(style, text, iconTex)
    {
      AddStyleClass(StyleClass);
    }

    public bool First
    {
      get
      {
        return first;
      }
      set
      {
        first = value;
        if (value)
        {
          AddPseudoStyleClass(WidgetPseudoClasses.FirstChildPseudoClass);
        }
        else
        {
          RemovePseudoStyleClass(WidgetPseudoClasses.FirstChildPseudoClass);
        }
      }
    }

    public bool Last
    {
      get
      {
        return last;
      }
      set
      {
        last = value;
        if (value)
        {
          AddPseudoStyleClass(WidgetPseudoClasses.LastChildPseudoClass);
        }
        else
        {
          RemovePseudoStyleClass(WidgetPseudoClasses.LastChildPseudoClass);
        }
      }
    }

    public override string NodeType => nameof(Button);
  }
}