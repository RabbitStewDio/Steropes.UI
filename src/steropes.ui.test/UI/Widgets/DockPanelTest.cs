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
using Steropes.UI.Widgets.Container;

namespace Steropes.UI.Test.UI.Widgets
{
  [Category("Container Widgets")]
  public class DockPanelTest
  {
    [Test]
    public void LayoutAllFilledElementTopNoFill()
    {
      var p = new DockPanel(LayoutTestStyle.Create());
      p.Add(LayoutTestWidget.FixedSize(100, 50), DockPanelConstraint.Left);
      p.Add(LayoutTestWidget.FixedSize(100, 50), DockPanelConstraint.Right);
      p.Add(LayoutTestWidget.FixedSize(100, 50), DockPanelConstraint.Top);
      p.Add(LayoutTestWidget.FixedSize(100, 50), DockPanelConstraint.Bottom);

      p.Measure(Size.Auto);
      p.DesiredSize.Should().Be(new Size(200, 150));

      p.Arrange(new Rectangle(10, 20, 500, 300));
      p.LayoutRect.Should().Be(new Rectangle(10, 20, 500, 300));
      p[0].LayoutRect.Should().Be(new Rectangle(10, 70, 100, 200));
      p[1].LayoutRect.Should().Be(new Rectangle(410, 70, 100, 200));
      p[2].LayoutRect.Should().Be(new Rectangle(10, 20, 500, 50));
      p[3].LayoutRect.Should().Be(new Rectangle(10, 270, 500, 50));
    }

    [Test]
    public void LayoutEmptyContainer()
    {
      var p = new DockPanel(LayoutTestStyle.Create());
      p.Measure(Size.Auto);
      p.DesiredSize.Should().Be(new Size(0, 0));

      p.Arrange(new Rectangle(10, 20, 500, 300));
      p.LayoutRect.Should().Be(new Rectangle(10, 20, 500, 300));
    }

    [Test]
    public void LayoutHorizontallyOpposingElementTopNoFill()
    {
      var p = new DockPanel(LayoutTestStyle.Create());
      p.Add(LayoutTestWidget.FixedSize(100, 50), DockPanelConstraint.Top);
      p.Add(LayoutTestWidget.FixedSize(100, 50), DockPanelConstraint.Bottom);

      p.Measure(Size.Auto);
      p.DesiredSize.Should().Be(new Size(100, 100));

      p.Arrange(new Rectangle(10, 20, 500, 300));
      p.LayoutRect.Should().Be(new Rectangle(10, 20, 500, 300));
      p[0].LayoutRect.Should().Be(new Rectangle(10, 20, 500, 50));
      p[1].LayoutRect.Should().Be(new Rectangle(10, 270, 500, 50));
    }

    [Test]
    public void LayoutMixedElementsLeftFill()
    {
      var p = new DockPanel(LayoutTestStyle.Create());
      p.LastChildFill = true;
      p.Add(LayoutTestWidget.FixedSize(100, 50), DockPanelConstraint.Top);
      p.Add(LayoutTestWidget.FixedSize(100, 50), DockPanelConstraint.Left);
      p.Add(LayoutTestWidget.FixedSize(100, 50), DockPanelConstraint.Left);

      p.Arrange(new Rectangle(10, 20, 500, 300));
      p.LayoutRect.Should().Be(new Rectangle(10, 20, 500, 300));
      p[0].LayoutRect.Should().Be(new Rectangle(10, 20, 500, 50));
      p[1].LayoutRect.Should().Be(new Rectangle(10, 70, 100, 250));
      p[2].LayoutRect.Should().Be(new Rectangle(110, 70, 400, 250));
    }

    [Test]
    public void LayoutSingleElementTopFill()
    {
      var p = new DockPanel(LayoutTestStyle.Create());
      p.LastChildFill = true;
      p.Add(LayoutTestWidget.FixedSize(100, 50), DockPanelConstraint.Top);

      p.Measure(Size.Auto);
      p.DesiredSize.Should().Be(new Size(100, 50));

      p.Arrange(new Rectangle(10, 20, 500, 300));
      p.LayoutRect.Should().Be(new Rectangle(10, 20, 500, 300));
      p[0].LayoutRect.Should().Be(new Rectangle(10, 20, 500, 300));
    }

    [Test]
    public void LayoutSingleElementTopNoFill()
    {
      var p = new DockPanel(LayoutTestStyle.Create());
      p.Add(LayoutTestWidget.FixedSize(100, 50), DockPanelConstraint.Top);

      p.Measure(Size.Auto);
      p.DesiredSize.Should().Be(new Size(100, 50));

      p.Arrange(new Rectangle(10, 20, 500, 300));
      p.LayoutRect.Should().Be(new Rectangle(10, 20, 500, 300));
      p[0].LayoutRect.Should().Be(new Rectangle(10, 20, 500, 50));
    }

    [Test]
    public void LayoutTwoElementsLeftFill()
    {
      var p = new DockPanel(LayoutTestStyle.Create());
      p.LastChildFill = true;
      p.Add(LayoutTestWidget.FixedSize(100, 50), DockPanelConstraint.Left);
      p.Add(LayoutTestWidget.FixedSize(100, 50), DockPanelConstraint.Left);

      p.Arrange(new Rectangle(10, 20, 500, 300));
      p.LayoutRect.Should().Be(new Rectangle(10, 20, 500, 300));
      p[0].LayoutRect.Should().Be(new Rectangle(10, 20, 100, 300));
      p[1].LayoutRect.Should().Be(new Rectangle(110, 20, 400, 300));
    }

    [Test]
    public void LayoutVerticallyOpposingElementTopNoFill()
    {
      var p = new DockPanel(LayoutTestStyle.Create());
      p.Add(LayoutTestWidget.FixedSize(100, 50), DockPanelConstraint.Left);
      p.Add(LayoutTestWidget.FixedSize(100, 50), DockPanelConstraint.Right);

      p.Measure(Size.Auto);
      p.DesiredSize.Should().Be(new Size(200, 50));

      p.Arrange(new Rectangle(10, 20, 500, 300));
      p.LayoutRect.Should().Be(new Rectangle(10, 20, 500, 300));
      p[0].LayoutRect.Should().Be(new Rectangle(10, 20, 100, 300));
      p[1].LayoutRect.Should().Be(new Rectangle(410, 20, 100, 300));
    }

    [Test]
    public void MeasureMixedElementsLeftFill()
    {
      var p = new DockPanel(LayoutTestStyle.Create());
      p.LastChildFill = true;
      p.Add(LayoutTestWidget.FixedSize(100, 50), DockPanelConstraint.Top);
      p.Add(LayoutTestWidget.FixedSize(100, 50), DockPanelConstraint.Left);
      p.Add(LayoutTestWidget.FixedSize(100, 50), DockPanelConstraint.Left);

      p.Measure(Size.Auto);
      p.DesiredSize.Should().Be(new Size(200, 100));
    }

    [Test]
    public void MeasureTwoElementsLeftFill()
    {
      var p = new DockPanel(LayoutTestStyle.Create());
      p.LastChildFill = true;
      p.Add(LayoutTestWidget.FixedSize(100, 50), DockPanelConstraint.Left);
      p.Add(LayoutTestWidget.FixedSize(100, 50), DockPanelConstraint.Left);

      p.Measure(Size.Auto);
      p.DesiredSize.Should().Be(new Size(200, 50));
    }

    [Test]
    public void ArrangeNonZeroOffset()
    {
      var p = new DockPanel(LayoutTestStyle.Create());
      p.LastChildFill = true;
      p.Add(LayoutTestWidget.FixedSize(100, 50), DockPanelConstraint.Top);
      p.Add(LayoutTestWidget.FixedSize(100, 50), DockPanelConstraint.Bottom);

      p.Arrange(new Rectangle(10, 20, 800, 600));
      p.DesiredSize.Should().Be(new Size(100, 100));
      p.LayoutRect.Should().Be(new Rectangle(10, 20, 800, 600));
      p[0].LayoutRect.Should().Be(new Rectangle(10, 20, 800, 50));
      p[1].LayoutRect.Should().Be(new Rectangle(10, 70, 800, 550));
    }

    [Test]
    public void ArrangeWithLargeElement()
    {
      //// Shows that large elements will make the content overflow.
      
      var p = new DockPanel(LayoutTestStyle.Create());
      p.LastChildFill = true;
      p.Add(LayoutTestWidget.FixedSize(100, 50), DockPanelConstraint.Top);
      p.Add(LayoutTestWidget.FixedSize(100, 850), DockPanelConstraint.Bottom);

      p.Arrange(new Rectangle(10, 20, 800, 600));
      p.DesiredSize.Should().Be(new Size(100, 900));
      p.LayoutRect.Should().Be(new Rectangle(10, 20, 800, 600));
      p[0].LayoutRect.Should().Be(new Rectangle(10, 20, 800, 50));
      p[1].LayoutRect.Should().Be(new Rectangle(10, 70, 800, 850));
    }
  }
}