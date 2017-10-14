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
using NSubstitute;
using NUnit.Framework;
using Steropes.UI.Components;
using Steropes.UI.Platform;

namespace Steropes.UI.Test.UI
{
  public class WidgetTest
  {
    [Category("Basic Widget Behaviour")]
    public class ArrangeChildExtension
    {
      static readonly TestCaseData[] ValidateFixedSizeData =
      {
        new TestCaseData(AnchoredRect.Full).Returns(new Rectangle(10, 20, 200, 100)),
        new TestCaseData(AnchoredRect.CreateCentered()).Returns(new Rectangle(85, 50, 50, 40)),
        new TestCaseData(AnchoredRect.CreateCentered(60, 50)).Returns(new Rectangle(80, 45, 60, 50)),
        new TestCaseData(AnchoredRect.CreateFixed(60, 50, 90, 80)).Returns(new Rectangle(70, 70, 90, 80)),
        new TestCaseData(AnchoredRect.CreateFull(10, 20, 30, 45)).Returns(new Rectangle(20, 40, 160, 35)),
        new TestCaseData(AnchoredRect.CreateTopLeftAnchored(10, 20)).Returns(new Rectangle(20, 40, 50, 40)),
        new TestCaseData(AnchoredRect.CreateTopLeftAnchored(10, 20, 30, 45)).Returns(new Rectangle(20, 40, 30, 45))
      };

      static readonly TestCaseData[] ValidateFixedSizeDataSmall =
      {
        // new TestCaseData(AnchoredRect.Full).Returns(new Rectangle(10, 20, 0, 0)).SetName("Full - {m}{a}"),
        // new TestCaseData(AnchoredRect.CreateCentered()).Returns(new Rectangle(-15, 0, 50, 40)).SetName("Centred, auto-size - {m}{a}"),
        // new TestCaseData(AnchoredRect.CreateCentered(60, 50)).Returns(new Rectangle(-20, -5, 60, 50)).SetName("Centred, defined size - {m}{a}"),
        // new TestCaseData(AnchoredRect.CreateFixed(60, 50, 90, 80)).Returns(new Rectangle(70, 70, 90, 80)).SetName("Fixed, defined size - {m}{a}"),
        new TestCaseData(AnchoredRect.CreateFull(10, 20, 30, 40)).Returns(new Rectangle(20, 40, 0, 0))
          .SetName("Full, padded - {m}{a}"),
        new TestCaseData(AnchoredRect.CreateTopLeftAnchored(10, 20)).Returns(new Rectangle(20, 40, 50, 40))
          .SetName("TopLeft, auto-size - {m}{a}"),
        new TestCaseData(AnchoredRect.CreateTopLeftAnchored(10, 20, 30, 45)).Returns(new Rectangle(20, 40, 30, 45))
          .SetName("TopLeft, defined size - {m}{a}")
      };

      [Test]
      [TestCaseSource(nameof(ValidateFixedSizeData))]
      public Rectangle ValidateFixedSizeChild(AnchoredRect rect)
      {
        var widget = Substitute.For<IWidget>();
        widget.DesiredSize.Returns(new Size(50, 40));
        widget.Anchor.Returns(rect);

        return widget.ArrangeChild(new Rectangle(10, 20, 200, 100));
      }

      [Test]
      [TestCaseSource(nameof(ValidateFixedSizeDataSmall))]
      public Rectangle ValidateFixedSizeChildSmall(AnchoredRect rect)
      {
        var widget = Substitute.For<IWidget>();
        widget.DesiredSize.Returns(new Size(50, 40));
        widget.Anchor.Returns(rect);

        return widget.ArrangeChild(new Rectangle(10, 20, 0, 0));
      }
    }

    [Category("Basic Widget Behaviour")]
    public class MeasureAndArrange
    {
      FunctionRecorder<Rectangle, Rectangle> arrangeCalled;

      FunctionRecorder<Size, Size> measureCalled;

      LayoutTestWidget widget;

      [Test]
      public void EnsureBasicLayout()
      {
        widget.Arrange(new Rectangle(10, 20, 200, 100));

        measureCalled.Called.Should().BeTrue();
        measureCalled.Argument.Should().Be(new Size(200, 100));
        arrangeCalled.Called.Should().BeTrue();
        arrangeCalled.Argument.Should().Be(new Rectangle(10, 20, 200, 100));
        widget.DesiredSize.Should().Be(new Size(200, 100));
        widget.LayoutRect.Should().Be(new Rectangle(10, 20, 200, 100));
      }

      [Test]
      public void EnsureMeasureFailureOnInfinity()
      {
        Assert.Throws<Exception>(() => widget.Measure(new Size(float.PositiveInfinity, float.PositiveInfinity)));
      }

      [Test]
      public void EnsurePaddingLayout()
      {
        widget.Padding = new Insets(10);

        widget.Arrange(new Rectangle(10, 20, 200, 100));

        measureCalled.Called.Should().BeTrue();
        measureCalled.Argument.Should().Be(new Size(180, 80));
        arrangeCalled.Called.Should().BeTrue();
        arrangeCalled.Argument.Should().Be(new Rectangle(20, 30, 180, 80));
        widget.DesiredSize.Should().Be(new Size(200, 100));
        widget.LayoutRect.Should().Be(new Rectangle(10, 20, 200, 100));
      }


      [SetUp]
      public void EstablishContext()
      {
        measureCalled = new FunctionRecorder<Size, Size>();
        arrangeCalled = new FunctionRecorder<Rectangle, Rectangle>();

        widget = new LayoutTestWidget
        {
          ArrangeOverrideFunc = arrangeCalled.Decorate(rect => rect),
          MeasureOverrideFunc = measureCalled.Decorate(size => size)
        };
      }
    }
  }
}