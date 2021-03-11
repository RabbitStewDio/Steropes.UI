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
  public class BoxGroupTests
  {
    [Test]
    public void ArrangeEmptyMustNotCrash()
    {
      var g = new BoxGroup(LayoutTestStyle.Create());

      g.Arrange(new Rectangle(10, 20, 400, 300));

      g.DesiredSize.Should().Be(new Size());
      g.LayoutRect.Should().Be(new Rectangle(10, 20, 0, 0));
    }

    [Test]
    public void ArrangeHorizontal()
    {
      var g = new BoxGroup(LayoutTestStyle.Create());
      g.Spacing = 5;
      g.Orientation = Orientation.Horizontal;
      g.Add(LayoutTestWidget.FixedSize(200, 100).WithAnchorRect(AnchoredRect.CreateTopLeftAnchored(0, 0)));
      g.Add(LayoutTestWidget.FixedSize(150, 50).WithAnchorRect(AnchoredRect.CreateTopLeftAnchored(0, 0)));

      g.Arrange(new Rectangle(10, 20, 400, 300));

      g.LayoutRect.Should().Be(new Rectangle(10, 20, 355, 300));
      g[0].LayoutRect.Should().Be(new Rectangle(10, 20, 200, 100));
      g[1].LayoutRect.Should().Be(new Rectangle(215, 20, 150, 50));
    }

    [Test]
    public void ArrangeHorizontalExpanded()
    {
      var g = new BoxGroup(LayoutTestStyle.Create());
      g.Spacing = 5;
      g.Orientation = Orientation.Horizontal;
      g.Add(LayoutTestWidget.FixedSize(200, 100).WithAnchorRect(AnchoredRect.Full));
      g.Add(LayoutTestWidget.FixedSize(150, 50).WithAnchorRect(AnchoredRect.Full), true);
      g.Add(LayoutTestWidget.FixedSize(150, 50).WithAnchorRect(AnchoredRect.Full));

      g.Arrange(new Rectangle(10, 20, 800, 500));

      g.LayoutRect.Should().Be(new Rectangle(10, 20, 800, 500));
      g[0].LayoutRect.Should().Be(new Rectangle(10, 20, 200, 500));
      g[1].LayoutRect.Should().Be(new Rectangle(215, 20, 440, 500));
      g[2].LayoutRect.Should().Be(new Rectangle(660, 20, 150, 500));
    }

    [Test]
    public void ArrangeHuge()
    {
      var g = new BoxGroup(LayoutTestStyle.Create());
      g.Spacing = 5;
      g.Orientation = Orientation.Horizontal;
      g.Add(LayoutTestWidget.FixedSize(2000, 1000).WithAnchorRect(AnchoredRect.CreateTopLeftAnchored(0, 0)));
      g.Add(LayoutTestWidget.FixedSize(150, 50).WithAnchorRect(AnchoredRect.CreateTopLeftAnchored(0, 0)));

      g.Arrange(new Rectangle(10, 20, 400, 300));

      g.DesiredSize.Should().Be(new Size(2155, 1000));
      g[0].DesiredSize.Should().Be(new Size(2000, 1000));
      g[1].DesiredSize.Should().Be(new Size(150, 50));

      g.LayoutRect.Should().Be(new Rectangle(10, 20, 2155, 300));
      g[0].LayoutRect.Should().Be(new Rectangle(10, 20, 2000, 1000));
      g[1].LayoutRect.Should().Be(new Rectangle(2015, 20, 150, 50));
    }

    [Test]
    public void ArrangeHugeAnchored()
    {
      var g = new BoxGroup(LayoutTestStyle.Create());
      g.Spacing = 5;
      g.Orientation = Orientation.Horizontal;
      g.Add(LayoutTestWidget.FixedSize(2000, 1000).WithAnchorRect(AnchoredRect.CreateCentered(500, 500)));
      g.Add(LayoutTestWidget.FixedSize(150, 50).WithAnchorRect(AnchoredRect.CreateFull(10)));

      g.Arrange(new Rectangle(10, 20, 400, 300));

      g.DesiredSize.Should().Be(new Size(675, 500)); // 500 + 150 + 20 (from full-anchor) + 5 
      g[0].DesiredSize.Should().Be(new Size(2000, 1000));
      g[1].DesiredSize.Should().Be(new Size(150, 50));

      g.LayoutRect.Should().Be(new Rectangle(10, 20, 675, 300));
      g[0].LayoutRect.Should().Be(new Rectangle(10, -80, 500, 500));
      g[1].LayoutRect.Should().Be(new Rectangle(525, 30, 150, 280));
    }

    [Test]
    public void ArrangeVertical()
    {
      var g = new BoxGroup(LayoutTestStyle.Create());
      g.Spacing = 5;
      g.Add(LayoutTestWidget.FixedSize(200, 100).WithAnchorRect(AnchoredRect.CreateTopLeftAnchored(0, 0)));
      g.Add(LayoutTestWidget.FixedSize(150, 50).WithAnchorRect(AnchoredRect.CreateTopLeftAnchored(0, 0)));

      g.Arrange(new Rectangle(10, 20, 400, 300));

      g.LayoutRect.Should().Be(new Rectangle(10, 20, 400, 155));
      g[0].LayoutRect.Should().Be(new Rectangle(10, 20, 200, 100));
      g[1].LayoutRect.Should().Be(new Rectangle(10, 125, 150, 50));
    }

    [Test]
    public void ArrangeVerticalExpanded()
    {
      var g = new BoxGroup(LayoutTestStyle.Create());
      g.Spacing = 5;
      g.Add(LayoutTestWidget.FixedSize(200, 100));
      g.Add(LayoutTestWidget.FixedSize(150, 50), true);
      g.Add(LayoutTestWidget.FixedSize(150, 50), true);
      g.Add(LayoutTestWidget.FixedSize(150, 50));

      g.Arrange(new Rectangle(10, 20, 400, 1300));

      g.LayoutRect.Should().Be(new Rectangle(10, 20, 400, 1300));
      g[0].LayoutRect.Should().Be(new Rectangle(10, 20, 400, 100));
      g[1].LayoutRect.Should().Be(new Rectangle(10, 125, 400, 567));
      g[2].LayoutRect.Should().Be(new Rectangle(10, 697, 400, 568));
      g[3].LayoutRect.Should().Be(new Rectangle(10, 1270, 400, 50));
    }

    [Test]
    public void ArrangeVerticalWithAnchor()
    {
      var g = new BoxGroup(LayoutTestStyle.Create());
      g.Spacing = 5;
      g.Add(LayoutTestWidget.FixedSize(200, 100).WithAnchorRect(AnchoredRect.Full));
      g.Add(LayoutTestWidget.FixedSize(150, 50).WithAnchorRect(AnchoredRect.Full));

      g.Arrange(new Rectangle(10, 20, 400, 300));

      g.LayoutRect.Should().Be(new Rectangle(10, 20, 400, 155));
      g[0].LayoutRect.Should().Be(new Rectangle(10, 20, 400, 100));
      g[1].LayoutRect.Should().Be(new Rectangle(10, 125, 400, 50));
    }

    [Test]
    public void BoxDefaultOrientationIsVertical()
    {
      var g = new BoxGroup(LayoutTestStyle.Create());
      g.Orientation.Should().Be(Orientation.Vertical);
    }

    [Test]
    public void MeasureEmptyMustNotCrash()
    {
      var g = new BoxGroup(LayoutTestStyle.Create());

      g.Measure(Size.Auto);

      g.DesiredSize.Should().Be(new Size());
    }

    [Test]
    public void MeasureHorizontal()
    {
      var g = new BoxGroup(LayoutTestStyle.Create());
      g.Spacing = 5;
      g.Orientation = Orientation.Horizontal;
      g.Add(LayoutTestWidget.FixedSize(200, 100));
      g.Add(LayoutTestWidget.FixedSize(150, 50));

      g.Measure(Size.Auto);

      g.DesiredSize.Should().Be(new Size(355, 100));
      g[0].DesiredSize.Should().Be(new Size(200, 100));
      g[1].DesiredSize.Should().Be(new Size(150, 50));
    }

    [Test]
    public void MeasureVertical()
    {
      var g = new BoxGroup(LayoutTestStyle.Create());
      g.Spacing = 5;
      g.Add(LayoutTestWidget.FixedSize(200, 100));
      g.Add(LayoutTestWidget.FixedSize(150, 50));

      g.Measure(Size.Auto);

      g.DesiredSize.Should().Be(new Size(200, 155));
      g[0].DesiredSize.Should().Be(new Size(200, 100));
      g[1].DesiredSize.Should().Be(new Size(150, 50));
    }
  }
}