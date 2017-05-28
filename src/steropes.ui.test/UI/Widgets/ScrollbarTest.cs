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
using Steropes.UI.Widgets;

namespace Steropes.UI.Test.UI.Widgets
{
  [Category("Widgets")]
  public class ScrollbarTest
  {
    [Test]
    public void ArrangeGreatlySmallerThanContent()
    {
      var style = LayoutTestStyle.Create();
      var sc = new Scrollbar(style) { ScrollContentHeight = 1000 };
      style.StyleResolver.AddRoot(sc);
      sc.Arrange(new Rectangle(10, 20, 300, 100));
      sc.LayoutRect.Should().Be(new Rectangle(300, 20, 10, 100));
      sc.MaximumVisibleOffset.Should().Be(900);
      sc.ScrollbarThumbHeight.Should().Be(10);
      sc.ScrollbarThumbOffset.Should().Be(0);
    }

    [Test]
    public void ArrangeLargerThanContent()
    {
      var style = LayoutTestStyle.Create();
      var sc = new Scrollbar(style) { ScrollContentHeight = 50 };
      style.StyleResolver.AddRoot(sc);
      sc.Arrange(new Rectangle(10, 20, 300, 100));
      sc.LayoutRect.Should().Be(new Rectangle(300, 20, 10, 100));
      sc.MaximumVisibleOffset.Should().Be(0);
      sc.ScrollbarThumbHeight.Should().Be(100);
      sc.ScrollbarThumbOffset.Should().Be(0);
    }

    [Test]
    public void ArrangeSmallerThanContent()
    {
      var style = LayoutTestStyle.Create();
      var sc = new Scrollbar(style) { ScrollContentHeight = 200 };
      style.StyleResolver.AddRoot(sc);
      sc.Arrange(new Rectangle(10, 20, 300, 100));
      sc.LayoutRect.Should().Be(new Rectangle(300, 20, 10, 100));
      sc.MaximumVisibleOffset.Should().Be(100);
      sc.ScrollbarThumbHeight.Should().Be(50);
      sc.ScrollbarThumbOffset.Should().Be(0);
    }

    [Test]
    public void ArrangeSmallerThanContentScrollingToEnd()
    {
      var style = LayoutTestStyle.Create();
      var sc = new Scrollbar(style) { ScrollContentHeight = 1000 };
      style.StyleResolver.AddRoot(sc);
      sc.ScrollTo(1000, true);

      sc.Arrange(new Rectangle(10, 20, 300, 100));

      sc.MaximumVisibleOffset.Should().Be(900);
      sc.ScrollbarThumbHeight.Should().Be(10);
      sc.ScrollbarThumbOffset.Should().Be(90);
    }

    [Test]
    public void ArrangeSmallerThanContentScrollingToMiddle()
    {
      var style = LayoutTestStyle.Create();
      var sc = new Scrollbar(style) { ScrollContentHeight = 1000 };
      style.StyleResolver.AddRoot(sc);
      sc.ScrollTo(450, true);

      sc.Arrange(new Rectangle(10, 20, 300, 100));

      sc.MaximumVisibleOffset.Should().Be(900);
      sc.ScrollbarThumbHeight.Should().Be(10);
      sc.ScrollbarThumbOffset.Should().Be(45);
    }

    [Test]
    public void Measure()
    {
      var style = LayoutTestStyle.Create();
      var sc = new Scrollbar(style) { ScrollContentHeight = 1000 };
      style.StyleResolver.AddRoot(sc);
      sc.Measure(Size.Auto);

      sc.DesiredSize.Should().Be(new Size(10, 10));
    }

    [Test]
    public void ScrollbarThumbExists()
    {
      var style = LayoutTestStyle.Create();
      var sc = new Scrollbar(style) { ScrollContentHeight = 1000 };
      style.StyleResolver.AddRoot(sc);
      sc.Measure(Size.Auto);

      sc.Thumb.Should().NotBeNull();
      sc.Thumb.DesiredSize.Should().Be(new Size(10, 10));
    }
  }
}