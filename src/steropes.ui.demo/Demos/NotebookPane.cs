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
using Steropes.UI.Components;
using Steropes.UI.Widgets;

namespace Steropes.UI.Demo.Demos
{
  class NotebookPane : ScrollPanel
  {
    public NotebookPane(IUIStyle style) : base(style)
    {
      var notebook = new Notebook(UIStyle);
      notebook.Tabs.Add(CreateHomeTab(notebook));
      Content = notebook;
    }

    public NotebookTab CreateHomeTab(Notebook notebook)
    {
      var tabCounter = 0;

      var createTabButton = new Button(UIStyle, "CreatePopup tab");
      createTabButton.Anchor = AnchoredRect.CreateFull(10);
      createTabButton.ActionPerformed += (sender, args) =>
        {
          tabCounter += 1;
          var tab = new NotebookTab(UIStyle, new Label(UIStyle, $"Tab {tabCounter}"), new Label(UIStyle, "Tab Content for " + tabCounter));
          tab.Tag = "Tab Content for " + tabCounter;
          notebook.Tabs.Add(tab);
        };

      var homeTab = new NotebookTab(UIStyle, new Label(UIStyle, "Home"), createTabButton);
      homeTab.IsPinned = true;
      homeTab.Tag = "Home";
      return homeTab;
    }
  }
}