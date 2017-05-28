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
  public static class GridTest
  {
    [Category("Container Widgets")]
    public class GridGroup_MeasureAndArrange
    {
      [Test]
      public void Auto_Relative_Mixed()
      {
        var g = new Grid(LayoutTestStyle.Create());
        g.ColumnConstraints.Add(LengthConstraint.Auto);
        g.ColumnConstraints.Add(LengthConstraint.Relative(1));
        g.ColumnConstraints.Add(LengthConstraint.Relative(1));

        g.AddChildAt(LayoutTestWidget.FixedSize(100, 10), 0, 0);
        g.AddChildAt(LayoutTestWidget.FixedSize(200, 10), 1, 0);
        g.AddChildAt(LayoutTestWidget.FixedSize(10, 10), 2, 0);

        g.Arrange(new Rectangle(10, 20, 400, 20));
        g.DesiredSize.Should().Be(new Size(400, 10));
        g.LayoutRect.Should().Be(new Rectangle(10, 20, 400, 10));
        g[0].LayoutRect.Should().Be(new Rectangle(10, 20, 100, 10));
        g[1].LayoutRect.Should().Be(new Rectangle(110, 20, 150, 10));
        g[2].LayoutRect.Should().Be(new Rectangle(260, 20, 150, 10));
      }

      [Test]
      public void TestImplicitConstraintsAdded()
      {
        var g = new Grid(LayoutTestStyle.Create());
        g.AddChildAt(new LayoutTestWidget(), 0, 0);
        g.AddChildAt(new LayoutTestWidget(), 1, 1);
        g.Measure(new Size(float.PositiveInfinity, float.PositiveInfinity));

        g.RowConstraints.Should().BeEquivalentTo(LengthConstraint.Auto, LengthConstraint.Auto);
        g.ColumnConstraints.Should().BeEquivalentTo(LengthConstraint.Auto, LengthConstraint.Auto);
      }

      [Test]
      public void TestImplicitConstraintsOnMinSpaceChild()
      {
        var g = new Grid(LayoutTestStyle.Create());
        g.AddChildAt(new LayoutTestWidget { MeasureOverrideFunc = s => new Size(100, 50) }, 0, 0);
        g.AddChildAt(new LayoutTestWidget { MeasureOverrideFunc = s => new Size(200, 100) }, 1, 1);
        g.Measure(new Size(float.PositiveInfinity, float.PositiveInfinity));

        g[0].DesiredSize.Should().Be(new Size(100, 50));
        g[1].DesiredSize.Should().Be(new Size(200, 100));
        g.DesiredSize.Should().Be(new Size(300, 150));

        g.Arrange(new Rectangle(10, 20, 400, 300));

        g[0].DesiredSize.Should().Be(new Size(100, 50));
        g[1].DesiredSize.Should().Be(new Size(200, 100));
        g[0].LayoutInvalid.Should().Be(false);
        g[1].LayoutInvalid.Should().Be(false);
        g[0].LayoutRect.Should().Be(new Rectangle(10, 20, 100, 50));
        g[1].LayoutRect.Should().Be(new Rectangle(110, 70, 200, 100));
        g.LayoutRect.Should().Be(new Rectangle(10, 20, 300, 150));
      }

      [Test]
      public void TestImplicitConstraintsOnZeroSpaceChild()
      {
        var g = new Grid(LayoutTestStyle.Create());
        g.AddChildAt(new LayoutTestWidget(), 0, 0);
        g.AddChildAt(new LayoutTestWidget(), 1, 1);
        g.Measure(new Size(float.PositiveInfinity, float.PositiveInfinity));

        g[0].DesiredSize.Should().Be(new Size());
        g[1].DesiredSize.Should().Be(new Size());
        g.DesiredSize.Should().Be(new Size());

        g.Arrange(new Rectangle(10, 20, 400, 300));

        g[0].DesiredSize.Should().Be(new Size());
        g[1].DesiredSize.Should().Be(new Size());
        g[0].LayoutInvalid.Should().Be(false);
        g[1].LayoutInvalid.Should().Be(false);
        g[0].LayoutRect.Should().Be(new Rectangle(10, 20, 0, 0));
        g[1].LayoutRect.Should().Be(new Rectangle(10, 20, 0, 0));
        g.LayoutRect.Should().Be(new Rectangle(10, 20, 0, 0));
      }

      [Test]
      public void TestImplicitFixedMeasure()
      {
        var g = new Grid(LayoutTestStyle.Create());
        g.AddChildAt(LayoutTestWidget.FixedSize(100, 100), 0, 0);
        g.AddChildAt(LayoutTestWidget.FixedSize(200, 200), 1, 1);

        g.Arrange(new Rectangle(10, 20, 0, 0));
        g.DesiredSize.Should().Be(new Size(300, 300));
        g.LayoutRect.Should().Be(new Rectangle(10, 20, 300, 300));
        g[0].LayoutRect.Should().Be(new Rectangle(10, 20, 100, 100));
        g[1].LayoutRect.Should().Be(new Rectangle(110, 120, 200, 200));
      }
    }
  }
}