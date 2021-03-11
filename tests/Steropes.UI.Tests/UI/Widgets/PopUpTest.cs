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

using NUnit.Framework;

using Steropes.UI.Components;
using Steropes.UI.Platform;
using Steropes.UI.Widgets.Container;

namespace Steropes.UI.Test.UI.Widgets
{
  [Category("Window Widget Behaviour")]
  public class PopUpTest
  {
    [Test]
    public void ChildPropertyDoesNotCrashOnSelf()
    {
      var p = new PopUp<IWidget>(LayoutTestStyle.Create());
      p.Content = new LayoutTestWidget();
      p.Content = p.Content;
    }

    [Test]
    public void ChildPropertyFailOnForeignParent()
    {
      var p = new PopUp<IWidget>(LayoutTestStyle.Create());
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
      var p = new PopUp<IWidget>(LayoutTestStyle.Create()) { Padding = new Insets(10), Content = LayoutTestWidget.FixedSize(500, 300).WithAnchorRect(AnchoredRect.CreateFull(40)) };

      p.Measure(Size.Auto);

      p.DesiredSize.Should().Be(new Size(520, 320));
      p.Content.DesiredSize.Should().Be(new Size(500, 300));
    }

    [Test]
    public void MeasureNoScrollTopLeft()
    {
      var p = new PopUp<IWidget>(LayoutTestStyle.Create())
                {
                  Padding = new Insets(10),
                  Content = LayoutTestWidget.FixedSize(500, 300).WithAnchorRect(AnchoredRect.CreateTopLeftAnchored(40, 50))
                };

      p.Measure(Size.Auto);

      p.DesiredSize.Should().Be(new Size(520, 320));
      p.Content.DesiredSize.Should().Be(new Size(500, 300));
    }
  }
}