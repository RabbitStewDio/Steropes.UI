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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Steropes.UI.Components;
using Steropes.UI.Input;
using Steropes.UI.Input.KeyboardInput;
using Steropes.UI.Styles;
using Steropes.UI.Widgets.Styles;

namespace Steropes.UI.Widgets
{
  public class Notebook : Widget
  {
    readonly NotebookStyleDefinition notebookStyle;

    readonly ScrollPanel panel;

    public Notebook(IUIStyle style) : base(style)
    {
      notebookStyle = StyleSystem.StylesFor<NotebookStyleDefinition>();

      Tabs = new NotebookTabList(UIStyle);
      Tabs.ActiveTabChanged += (s, e) => { panel.Content = Tabs.ActiveTab?.Content; };
      Tabs.AddNotify(this);
      RaiseChildAdded(0, Tabs);

      panel = new ScrollPanel(UIStyle);
      panel.AddNotify(this);
      RaiseChildAdded(1, panel);

      KeyPressed += OnKeyPressed;
    }

    public override int Count => 2;

    public int NotebookTabOverlapY => Style.GetValue(notebookStyle.NotebookTabOverlapY);

    public NotebookTabList Tabs { get; }

    public override IWidget this[int index]
    {
      get
      {
        switch (index)
        {
          case 1:
            return Tabs;
          case 0:
            return panel;
          default:
            throw new IndexOutOfRangeException();
        }
      }
    }

    protected override Rectangle ArrangeOverride(Rectangle layoutSize)
    {
      var tabHeight = Math.Max(0, Tabs.DesiredSize.HeightInt - NotebookTabOverlapY);
      Tabs.Arrange(new Rectangle(layoutSize.X, layoutSize.Y, layoutSize.Width, Tabs.DesiredSize.HeightInt));
      panel.Arrange(new Rectangle(layoutSize.X, layoutSize.Y + tabHeight, layoutSize.Width, Math.Max(0, layoutSize.Height - tabHeight)));
      return layoutSize;
    }

    protected override Size MeasureOverride(Size availableSize)
    {
      Tabs.Measure(availableSize);
      var size = availableSize.ReduceBy(0, Tabs.DesiredSize.Height - NotebookTabOverlapY);
      panel.Measure(size);

      return new Size(panel.DesiredSize.Width, panel.DesiredSize.Height + Tabs.DesiredSize.Height);
    }

    void OnKeyPressed(object sender, KeyEventArgs args)
    {
      if (args.Flags.IsShortcutKeyDown() && args.Key == Keys.W)
      {
        if (Tabs.ActiveTab != null)
        {
          Tabs.ActiveTab.Close();
          args.Consume();
        }
      }

      if (Tabs.Count == 0)
      {
        return;
      }

      if (args.Flags.IsControlDown() && args.Key == Keys.Tab)
      {
        var index = 0;
        if (Tabs.ActiveTab == null)
        {
          index = 0;
        }
        else if (args.Flags.IsShiftDown())
        {
          var activeTabIndex = Tabs.IndexOf(Tabs.ActiveTab);
          if (activeTabIndex <= 0)
          {
            activeTabIndex = Tabs.Count - 1;
          }
          index = activeTabIndex;
        }
        else
        {
          index = (index + 1) % Tabs.Count;
        }

        Tabs.ActiveTab = (NotebookTab)Tabs[index];
        args.Consume();
      }
    }
  }
}