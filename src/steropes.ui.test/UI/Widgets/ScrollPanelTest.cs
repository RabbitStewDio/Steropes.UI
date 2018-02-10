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

using FluentAssertions;

using Microsoft.Xna.Framework;

using NUnit.Framework;

using Steropes.UI.Components;
using Steropes.UI.Platform;
using Steropes.UI.Widgets;

namespace Steropes.UI.Test.UI.Widgets
{
  [Category("Container Widgets")]
  public class ScrollPanelTest
  {
    [Test]
    public void Arrange()
    {
      var p = new TestScrollPanel(LayoutTestStyle.Create())
                {
                  // CornerSize = 10,
                  Padding = new Insets(10),
                  VerticalScrollbarMode = ScrollbarMode.Always,
                  Content = LayoutTestWidget.FixedSize(500, 300).WithAnchorRect(AnchoredRect.CreateFull(40))
                };

      p.UIStyle.StyleResolver.AddRoot(p);
      p.Arrange(new Rectangle(10, 20, 300, 200));

      p.DesiredSize.Should().Be(new Size(610, 400));
      p.Content.DesiredSize.Should().Be(new Size(500, 300));

      p.LayoutRect.Should().Be(new Rectangle(10, 20, 300, 200));

      // width = 300 - 2*40 (anchor) - 2*10 (padding) - 10 (scrollbar)
      p.Content.LayoutRect.Should().Be(new Rectangle(60, 70, 190, 300));
      p.TestScrollbar.LayoutRect.Should().Be(new Rectangle(290, 30, 10, 180));

      // height = 300 + 2*40 from anchor
      p.TestScrollbar.ScrollContentHeight.Should().Be(380);
    }

    [Test]
    public void ChildPropertyDoesNotCrashOnSelf()
    {
      var p = new ScrollPanel(LayoutTestStyle.Create());
      p.Content = new LayoutTestWidget();
      p.Content = p.Content;
    }

    [Test]
    public void ChildPropertyFailOnForeignParent()
    {
      var p = new ScrollPanel(LayoutTestStyle.Create());
      Assert.Throws<InvalidOperationException>(
        () =>
          {
            var child = new LayoutTestWidget();
            child.AddNotify(new LayoutTestWidget());
            p.Content = child;
          });
    }

    [Test]
    public void MeasureNoScroll()
    {
      var p = new ScrollPanel(LayoutTestStyle.Create())
                {
                  // CornerSize = 10,
                  Padding = new Insets(10),
                  VerticalScrollbarMode = ScrollbarMode.None,
                  Content = LayoutTestWidget.FixedSize(500, 300).WithAnchorRect(AnchoredRect.CreateFull(40))
                };

      p.UIStyle.StyleResolver.AddRoot(p);
      p.Measure(Size.Auto);

      p.DesiredSize.Should().Be(new Size(600, 400));
    }

    [Test]
    public void MeasureScroll()
    {
      var p = new ScrollPanel(LayoutTestStyle.Create())
                {
                  // CornerSize = 10,
                  Padding = new Insets(10),
                  VerticalScrollbarMode = ScrollbarMode.Always,
                  Content = LayoutTestWidget.FixedSize(500, 300).WithAnchorRect(AnchoredRect.CreateFull(40))
                };

      p.UIStyle.StyleResolver.AddRoot(p);
      p.Measure(Size.Auto);

      p.DesiredSize.Should().Be(new Size(610, 400));
    }

    [Test]
    public void NoZeroOffset()
    {
      var p = new ScrollPanel(LayoutTestStyle.Create())
                {
                  // CornerSize = 10,
                  Padding = new Insets(10),
                  VerticalScrollbarMode = ScrollbarMode.Always,
                  Content = LayoutTestWidget.FixedSize(500, 300)
                };

      // first arrange to initialize all sizes
      p.Arrange(new Rectangle(10, 20, 500, 200));
      // scroll down
      p.Scrollbar.ScrollToBottom(true);
      // second arrange should have moved the child ..
      p.Arrange(new Rectangle(10, 20, 500, 200));

      p.Scrollbar.ScrollContentHeight.Should().Be(300);
      p.Scrollbar.ScrollContentOrigin.Should().Be(-90);
      p.Content.LayoutRect.Should().Be(new Rectangle(20, -90, 480, 300));
      p.ContentRect.Bottom.Should().Be(p.Content.LayoutRect.Bottom);
    }

    public class TestScrollPanel : ScrollPanel
    {
      public TestScrollPanel(IUIStyle style) : base(style)
      {
      }

      public Scrollbar TestScrollbar => Scrollbar;
    }
  }
}