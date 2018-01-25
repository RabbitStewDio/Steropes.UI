// MIT License
//
// Copyright (c) 2011-2016 Elisée Maurer, Sparklin Labs, Creative Patterns
// Copyright (c) 2016 Thomas Morgner, Rabbit-StewDio Ltd.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using Steropes.UI.Components;
using Steropes.UI.Widgets;
using Steropes.UI.Widgets.Container;

namespace Steropes.UI.Demo.Demos
{
  public static class WidgetDemo
  {
    public static Widget CreateRootPanel(IUIStyle uiStyle)
    {
      var basicDemoPane = new BasicDemoPane(uiStyle);

      var demoPanel = new ScrollPanel(uiStyle)
      {
        VerticalScrollbarMode = ScrollbarMode.None, 
        Content = basicDemoPane
      };
      demoPanel.AddStyleClass("DemoPanel");

      var demosBoxGroup = new BoxGroup(uiStyle, Orientation.Vertical, 0)
      {
        { CreateDemoButton("Basic", demoPanel, basicDemoPane), true },
        { CreateDemoButton("Notebook", demoPanel, new NotebookPane(uiStyle)), true },
        { CreateDemoButton("Text Area", demoPanel, new TextAreaPane(uiStyle)), true },
        { CreateDemoButton("Custom Viewport", demoPanel, new CustomViewportPane(uiStyle)), true }
      };

      // Splitter
      return new Splitter(uiStyle, Direction.Left)
      {
        Anchor = AnchoredRect.CreateFull(10),
        Collapsable = true,
        FirstPane = demosBoxGroup,
        SecondPane = demoPanel,
        FirstPaneMinSize = 200
      };
    }

    static Button CreateDemoButton(string demoName, ScrollPanel demoPanel, Widget demoPane)
    {
      var demoPaneButton = new Button(demoPanel.UIStyle, demoName);
      demoPaneButton.ActionPerformed += (sender, args) => demoPanel.Content = demoPane;
      return demoPaneButton;
    }
  }
}