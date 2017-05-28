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
  public class IconLabelTest
  {
    [Test]
    public void TestArrangeFull()
    {
      var style = LayoutTestStyle.Create();
      var l = new IconLabel(style);
      style.StyleResolver.AddRoot(l);

      l.Image.Texture = LayoutTestStyle.CreateTexture("Image", 40, 30);
      l.Label.Text = "Test";

      l.Arrange(new Rectangle(10, 20, 300, 200));
      l.IconTextGap.Should().Be(10);
      l.LayoutRect.Should().Be(new Rectangle(10, 20, 134, 200));
      l.Image.LayoutRect.Should().Be(new Rectangle(20, 105, 40, 30));
      l.Label.LayoutRect.Should().Be(new Rectangle(70, 30, 64, 180));

      l.LayoutRect.Center.Y.Should().Be(120);
      l.Image.LayoutRect.Center.Y.Should().Be(120);
      l.Label.LayoutRect.Center.Y.Should().Be(120);
    }

    [Test]
    public void TestArrangeFullZeroWidth()
    {
      var style = LayoutTestStyle.Create();
      var l = new IconLabel(style);
      style.StyleResolver.AddRoot(l);

      l.Image.Texture = LayoutTestStyle.CreateTexture("Image", 40, 30);
      l.Label.Text = "Test";

      l.Arrange(new Rectangle(10, 20, 0, 0));

      l.LayoutRect.Should().Be(new Rectangle(10, 5, 70, 50));
      l.Image.LayoutRect.Should().Be(new Rectangle(20, 15, 40, 30));
      l.Label.LayoutRect.Should().Be(new Rectangle(70, 30, 0, 0));

      l.LayoutRect.Center.Y.Should().Be(30);
      l.Image.LayoutRect.Center.Y.Should().Be(30);
      l.Label.LayoutRect.Center.Y.Should().Be(30);
    }

    [Test]
    public void TestMeasureFull()
    {
      var style = LayoutTestStyle.Create();
      var l = new IconLabel(style);
      style.StyleResolver.AddRoot(l);

      l.Image.Texture = LayoutTestStyle.CreateTexture("Image", 40, 30);
      l.Label.Text = "Test";

      l.Measure(Size.Auto);

      l.DesiredSize.Should().Be(new Size(114, 50));
    }

    [Test]
    public void TestMeasureSingleIcon()
    {
      var style = LayoutTestStyle.Create();
      var l = new IconLabel(style);
      style.StyleResolver.AddRoot(l);

      l.Image.Texture = LayoutTestStyle.CreateTexture("Image", 40, 30);

      l.Measure(Size.Auto);

      l.DesiredSize.Should().Be(new Size(60, 50));
    }

    [Test]
    public void TestMeasureSingleLabel()
    {
      var style = LayoutTestStyle.Create();
      var l = new IconLabel(style);
      style.StyleResolver.AddRoot(l);

      l.Label.Text = "Test";

      l.Measure(Size.Auto);

      l.DesiredSize.Should().Be(new Size(64, 35));
    }
  }
}