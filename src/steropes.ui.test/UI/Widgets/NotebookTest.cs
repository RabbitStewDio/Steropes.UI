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
using FluentAssertions;

using Microsoft.Xna.Framework;

using NUnit.Framework;

using Steropes.UI.Components;
using Steropes.UI.Input.MouseInput;
using Steropes.UI.Widgets;

namespace Steropes.UI.Test.UI.Widgets
{
  [Category("Widgets")]
  public class NotebookTest
  {
    [Test]
    public void Arrange()
    {
      var style = LayoutTestStyle.Create();
      var b = new Notebook(style);
      style.StyleResolver.AddRoot(b);
      b.Tabs.Add(new NotebookTab(style, new Label(style, "Tab 1"), null));
      b.Tabs.Add(new NotebookTab(style, new Label(style, "Tab 2"), null));
      b.Tabs.Add(new NotebookTab(style, new Label(style, "Tab 3"), null));

      b.Arrange(new Rectangle(10, 20, 400, 300));
      b.LayoutRect.Should().Be(new Rectangle(10, 20, 400, 300));
      b.Tabs.LayoutRect.Should().Be(new Rectangle(10, 20, 400, 35));
      b.Tabs[0].LayoutRect.Should().Be(new Rectangle(30, 20, 155, 35));
      b.Tabs[1].LayoutRect.Should().Be(new Rectangle(170, 20, 155, 35));
      b.Tabs[2].LayoutRect.Should().Be(new Rectangle(310, 20, 155, 35));
    }

    [Test]
    public void DragAndDrop_Move_Tab2_To_Tab3()
    {
      var style = LayoutTestStyle.Create();

      var tab1 = new NotebookTab(style, new Label(style, "Tab 1"), null) { Tag = "Tab 1" };
      var tab2 = new NotebookTab(style, new Label(style, "Tab 2"), null) { Tag = "Tab 2" };
      var tab3 = new NotebookTab(style, new Label(style, "Tab 3"), null) { Tag = "Tab 3" };

      var b = new DnDNoteBookTab(style) { tab1, tab2, tab3 };
      style.StyleResolver.AddRoot(b);
      b.Arrange(new Rectangle(10, 20, 400, 40));

      b[1].DispatchMouseDown(MouseButton.Left, 20, 30);
      b.Arrange(new Rectangle(10, 20, 400, 40));

      b[1].DispatchMouseDrag(MouseButton.Left, 250, 30);
      b.DebugInfo.DraggedTab.Should().BeSameAs(b[1]);
      b.DebugInfo.DraggedTabEventOrigin.Should().Be(20);
      b.DebugInfo.DraggedTabOffsetOrigin.Should().Be(170);
      b.DebugInfo.DraggedTabOffset.Should().Be(170 + (250 - 20));

      b.Arrange(new Rectangle(10, 20, 400, 40));
      b[1].DispatchMouseUp(MouseButton.Left, 201, 30);
      b.DebugInfo.DraggedTab.Should().BeNull();
      b.DebugInfo.DraggedTabEventOrigin.Should().Be(0);
      b.DebugInfo.DraggedTabOffsetOrigin.Should().Be(0);
      b.DebugInfo.DraggedTabOffset.Should().Be(0);

      b.Arrange(new Rectangle(10, 20, 400, 40));

      b[0].Should().BeSameAs(tab1);
      b[1].Should().BeSameAs(tab3);
      b[2].Should().BeSameAs(tab2);

      // should retain normal layout.
      b[0].LayoutRect.Should().Be(new Rectangle(30, 25, 155, 35));
      b[1].LayoutRect.Should().Be(new Rectangle(170, 25, 155, 35));
      b[2].LayoutRect.Should().Be(new Rectangle(310, 25, 155, 35));
    }

    [Test]
    public void DragAndDrop_No_Move()
    {
      var style = LayoutTestStyle.Create();

      var tab1 = new NotebookTab(style, new Label(style, "Tab 1"), null) { Tag = "Tab 1" };
      var tab2 = new NotebookTab(style, new Label(style, "Tab 2"), null) { Tag = "Tab 2" };
      var tab3 = new NotebookTab(style, new Label(style, "Tab 3"), null) { Tag = "Tab 3" };

      var b = new DnDNoteBookTab(style) { tab1, tab2, tab3 };
      style.StyleResolver.AddRoot(b);
      b.Arrange(new Rectangle(10, 20, 400, 40));

      b[1].DispatchMouseDown(MouseButton.Left, 20, 30);
      b.Arrange(new Rectangle(10, 20, 400, 40));

      b[1].DispatchMouseDrag(MouseButton.Left, 30, 30);
      b.DebugInfo.DraggedTab.Should().BeSameAs(b[1]);
      b.DebugInfo.DraggedTabEventOrigin.Should().Be(20);
      b.DebugInfo.DraggedTabOffsetOrigin.Should().Be(170);
      b.DebugInfo.DraggedTabOffset.Should().Be(170 + (30 - 20));

      b.Arrange(new Rectangle(10, 20, 400, 40));
      b[1].DispatchMouseUp(MouseButton.Left, 201, 30);
      b.DebugInfo.DraggedTab.Should().BeNull();
      b.DebugInfo.DraggedTabEventOrigin.Should().Be(0);
      b.DebugInfo.DraggedTabOffsetOrigin.Should().Be(0);
      b.DebugInfo.DraggedTabOffset.Should().Be(0);

      b.Arrange(new Rectangle(10, 20, 400, 40));

      b[0].Should().BeSameAs(tab1);
      b[1].Should().BeSameAs(tab2);
      b[2].Should().BeSameAs(tab3);

      // should retain normal layout.
      b[0].LayoutRect.Should().Be(new Rectangle(30, 25, 155, 35));
      b[1].LayoutRect.Should().Be(new Rectangle(170, 25, 155, 35));
      b[2].LayoutRect.Should().Be(new Rectangle(310, 25, 155, 35));
    }

    [Test]
    public void DragAnDrop_Inside_Arrange_Far_Left()
    {
      var style = LayoutTestStyle.Create();

      var tab1 = new NotebookTab(style, new Label(style, "Tab 1"), null) { Tag = "Tab 1" };
      var tab2 = new NotebookTab(style, new Label(style, "Tab 2"), null) { Tag = "Tab 2" };
      var tab3 = new NotebookTab(style, new Label(style, "Tab 3"), null) { Tag = "Tab 3" };

      var b = new DnDNoteBookTab(style) { tab1, tab2, tab3 };
      style.StyleResolver.AddRoot(b);
      b.Arrange(new Rectangle(10, 20, 600, 40));

      b[1].DispatchMouseDown(MouseButton.Left, 20, 30);
      b.Arrange(new Rectangle(10, 20, 600, 40));
      b[1].DispatchMouseDrag(MouseButton.Left, -500, 30);
      b.Arrange(new Rectangle(10, 20, 600, 40));

      // The dragged element floating range is limited to the visible space.
      b[0].LayoutRect.Should().Be(new Rectangle(170, 25, 155, 35));
      b[1].LayoutRect.Should().Be(new Rectangle(30, 25, 155, 35));
      b[2].LayoutRect.Should().Be(new Rectangle(310, 25, 155, 35));
    }

    [Test]
    public void DragAnDrop_Inside_Arrange_Far_Right()
    {
      var style = LayoutTestStyle.Create();

      var tab1 = new NotebookTab(style, new Label(style, "Tab 1"), null) { Tag = "Tab 1" };
      var tab2 = new NotebookTab(style, new Label(style, "Tab 2"), null) { Tag = "Tab 2" };
      var tab3 = new NotebookTab(style, new Label(style, "Tab 3"), null) { Tag = "Tab 3" };

      var b = new DnDNoteBookTab(style) { tab1, tab2, tab3 };
      style.StyleResolver.AddRoot(b);
      b.Arrange(new Rectangle(10, 20, 600, 40));

      b[1].DispatchMouseDown(MouseButton.Left, 20, 30);
      b.Arrange(new Rectangle(10, 20, 600, 40));
      b[1].DispatchMouseDrag(MouseButton.Left, 500, 30);
      b.Arrange(new Rectangle(10, 20, 600, 40));

      // The dragged element floating range is limited to the visible space.
      b[0].LayoutRect.Should().Be(new Rectangle(30, 25, 155, 35));
      b[1].LayoutRect.Should().Be(new Rectangle(325, 25, 155, 35));
      b[2].LayoutRect.Should().Be(new Rectangle(170, 25, 155, 35));
    }

    [Test]
    public void DragAnDrop_Inside_Arrange_SelfPos()
    {
      var style = LayoutTestStyle.Create();

      var tab1 = new NotebookTab(style, new Label(style, "Tab 1"), null) { Tag = "Tab 1" };
      var tab2 = new NotebookTab(style, new Label(style, "Tab 2"), null) { Tag = "Tab 2" };
      var tab3 = new NotebookTab(style, new Label(style, "Tab 3"), null) { Tag = "Tab 3" };

      var b = new DnDNoteBookTab(style) { tab1, tab2, tab3 };
      style.StyleResolver.AddRoot(b);
      b.Arrange(new Rectangle(10, 20, 600, 40));

      b[1].DispatchMouseDown(MouseButton.Left, 20, 30);
      b.Arrange(new Rectangle(10, 20, 600, 40));
      
      // dragndrop mode only activates after the element moved at least 5 pixels. This ensures
      // tha clicks and double-clicks are smooth and dont cause disruptions 
      b[1].DispatchMouseDrag(MouseButton.Left, 25, 30);
      b.Arrange(new Rectangle(10, 20, 600, 40));

      b[0].LayoutRect.Should().Be(new Rectangle(30, 25, 155, 35));
      b[1].LayoutRect.Should().Be(new Rectangle(170, 25, 155, 35));
      b[2].LayoutRect.Should().Be(new Rectangle(310, 25, 155, 35));
    }

    [Test]
    public void DragAnDrop_Inside_Arrange_Over_Threshold()
    {
      var style = LayoutTestStyle.Create();

      var tab1 = new NotebookTab(style, new Label(style, "Tab 1"), null) { Tag = "Tab 1" };
      var tab2 = new NotebookTab(style, new Label(style, "Tab 2"), null) { Tag = "Tab 2" };
      var tab3 = new NotebookTab(style, new Label(style, "Tab 3"), null) { Tag = "Tab 3" };

      var b = new DnDNoteBookTab(style) { tab1, tab2, tab3 };
      style.StyleResolver.AddRoot(b);
      b.Arrange(new Rectangle(10, 20, 600, 40));

      b[1].DispatchMouseDown(MouseButton.Left, 20, 30);
      b.Arrange(new Rectangle(10, 20, 600, 40));
      b[1].DispatchMouseDrag(MouseButton.Left, 26, 30);
      b.Arrange(new Rectangle(10, 20, 600, 40));

      // The dragged element floats above all other elements, but consumes its desired space.
      // Thus the layout will stay largely the same
      b[0].LayoutRect.Should().Be(new Rectangle(30, 25, 155, 35));
      b[1].LayoutRect.Should().Be(new Rectangle(176, 25, 155, 35));
      b[2].LayoutRect.Should().Be(new Rectangle(310, 25, 155, 35));
    }

    [Test]
    public void DragAnDrop_Inside_Move_Arrange()
    {
      var style = LayoutTestStyle.Create();

      var tab1 = new NotebookTab(style, new Label(style, "Tab 1"), null) { Tag = "Tab 1" };
      var tab2 = new NotebookTab(style, new Label(style, "Tab 2"), null) { Tag = "Tab 2" };
      var tab3 = new NotebookTab(style, new Label(style, "Tab 3"), null) { Tag = "Tab 3" };
      var tab4 = new NotebookTab(style, new Label(style, "Tab 4"), null) { Tag = "Tab 4" };

      var b = new DnDNoteBookTab(style) { tab1, tab2, tab3, tab4 };
      style.StyleResolver.AddRoot(b);
      b.Arrange(new Rectangle(10, 20, 600, 40));

      b[1].DispatchMouseDown(MouseButton.Left, 20, 30);
      b.Arrange(new Rectangle(10, 20, 600, 40));
      b[1].DispatchMouseDrag(MouseButton.Left, 20 + 170, 30);
      b.Arrange(new Rectangle(10, 20, 600, 40));

      // The dragged element floats above all other elements, and the placeholder for the tab shifts accordingly.
      // we dragged the tab beyond the center point of Tab3, so Tab-3 moves forward to the space previously occupied
      // by Tab-2. 
      b[0].LayoutRect.Should().Be(new Rectangle(30, 25, 155, 35));
      b[1].LayoutRect.Should().Be(new Rectangle(170 + 170, 25, 155, 35));
      b[2].LayoutRect.Should().Be(new Rectangle(170, 25, 155, 35));
      b[3].LayoutRect.Should().Be(new Rectangle(450, 25, 155, 35));
    }

    [Test]
    public void StyleOverlapIsAppliedToNotebook()
    {
      var style = LayoutTestStyle.Create();
      var b = new Notebook(style);
      style.StyleResolver.AddRoot(b);
      b.Measure(Size.Auto);
      b.NotebookTabOverlapY.Should().Be(15);
    }

    [Test]
    public void StyleOverlapIsAppliedToNotebookTabList()
    {
      var style = LayoutTestStyle.Create();
      var b = new NotebookTabList(style);
      style.StyleResolver.AddRoot(b);
      b.Measure(Size.Auto);
      b.NotebookTabOverlapX.Should().Be(15);
    }

    class DnDNoteBookTab : NotebookTabList
    {
      public DnDNoteBookTab(IUIStyle style) : base(style)
      {
      }

      public DragNDropState DebugInfo => DragState;

      public override string NodeType => "NotebookTabList";
    }
  }
}