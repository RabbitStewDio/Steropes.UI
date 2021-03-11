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
  [Category("Widget")]
  public class SliderTest
  {
    [Test]
    public void HandlePositionAtFull()
    {
      var s = new Slider(LayoutTestStyle.Create(), 10, 60, 100, 5);
      s.UIStyle.StyleResolver.AddRoot(s);
      s.Arrange(new Rectangle(10, 20, 400, 100));
      s.LayoutRect.Should().Be(new Rectangle(10, 20, 400, 100));
      s[0].LayoutRect.Should().Be(new Rectangle(10, 20, 400, 100), "Group Container");
      s[0][0].LayoutRect.Should().Be(new Rectangle(10, 20, 400, 100), "Track");
      s[0][1].DesiredSize.Should().Be(new Size(40, 40), "HandleSize");
      s[0][1].LayoutRect.Should().Be(new Rectangle(370, 20, 40, 100), "HandleSize");
    }

    [Test]
    public void HandlePositionAtMiddle()
    {
      var s = new Slider(LayoutTestStyle.Create(), 10, 60, 35, 5);
      s.UIStyle.StyleResolver.AddRoot(s);
      s.Arrange(new Rectangle(10, 20, 400, 100));
      s.LayoutRect.Should().Be(new Rectangle(10, 20, 400, 100));
      s[0].LayoutRect.Should().Be(new Rectangle(10, 20, 400, 100), "Group Container");
      s[0][0].LayoutRect.Should().Be(new Rectangle(10, 20, 400, 100), "Track");
      s[0][1].DesiredSize.Should().Be(new Size(40, 40), "HandleSize");
      s[0][1].LayoutRect.Should().Be(new Rectangle(190, 20, 40, 100), "HandleSize");
    }

    [Test]
    public void HandlePositionAtZero()
    {
      var s = new Slider(LayoutTestStyle.Create(), 10, 60, 0, 5);
      s.UIStyle.StyleResolver.AddRoot(s);
      s.Arrange(new Rectangle(10, 20, 400, 100));
      s.LayoutRect.Should().Be(new Rectangle(10, 20, 400, 100));
      s[0][1].LayoutRect.Should().Be(new Rectangle(10, 20, 40, 100));
    }
  }
}