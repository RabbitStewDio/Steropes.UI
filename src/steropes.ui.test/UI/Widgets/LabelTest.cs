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
using Steropes.UI.Platform;
using Steropes.UI.Widgets;

namespace Steropes.UI.Test.UI.Widgets
{
  [Category("Widgets")]
  public class LabelTest
  {
    [Test]
    public void TestArrange()
    {
      var l = new Label(LayoutTestStyle.Create());
      l.Padding = new Insets(1, 2, 3, 4);
      l.Font = LayoutTestStyle.CreateFont();
      l.Text = "Test";
      l.Alignment = Alignment.Center;

      l.Arrange(new Rectangle(10, 20, 150, 60));
      l.Content.LayoutRect.Should().Be(new Rectangle(12, 42, 144, 15));
    }

    [Test]
    public void TestArrangeZero()
    {
      var l = new Label(LayoutTestStyle.Create());
      l.Padding = new Insets(1, 2, 3, 4);
      l.Font = LayoutTestStyle.CreateFont();
      l.Text = "Test";
      l.Alignment = Alignment.Center;

      l.Arrange(new Rectangle(10, 20, 0, 0));
      l.LayoutRect.Should().Be(new Rectangle(10, 20, 6, 4));
    }

    [Test]
    public void TestLineBreaking()
    {
      var l = new Label(LayoutTestStyle.Create());
      l.Padding = new Insets(1, 2, 3, 4);
      l.Font = LayoutTestStyle.CreateFont();
      l.Text = "Test Test tst tasd asd asd asda sadsda sdads asad ad";
      l.Alignment = Alignment.Center;

      l.Arrange(new Rectangle(10, 20, 40, 40));
      l.LayoutRect.Should().Be(new Rectangle(10, 20, 40, 40));
      l.Content.LayoutRect.Should().Be(new Rectangle(12, -36, 34, 150));
    }

    [Test]
    public void TestLineBreakingZero()
    {
      var l = new Label(LayoutTestStyle.Create());
      l.Padding = new Insets(1, 2, 3, 4);
      l.Font = LayoutTestStyle.CreateFont();
      l.Text = "Test Test tst tasd asd asd asda sadsda sdads asad ad";
      l.Alignment = Alignment.Center;

      l.Arrange(new Rectangle(10, 20, 0, 0));
      l.LayoutRect.Should().Be(new Rectangle(10, 20, 6, 4));
      l.Content.LayoutRect.Should().Be(new Rectangle(12, -54, 0, 150));
    }

    [Test]
    public void TestMeasureEmptyString()
    {
      var l = new Label(LayoutTestStyle.Create());
      l.Padding = new Insets();
      l.Font = LayoutTestStyle.CreateFont();
      l.Measure(Size.Auto);
      l.DesiredSize.Should().Be(new Size(0, 12));
    }

    [Test]
    public void TestMeasureString()
    {
      var l = new Label(LayoutTestStyle.Create());
      l.Padding = new Insets();
      l.Font = LayoutTestStyle.CreateFont();
      l.Text = "Test";
      l.Measure(Size.Auto);
      l.DesiredSize.Should().Be(new Size(44, 15));
    }

    [Test]
    public void TestMeasureStringPadded()
    {
      var l = new Label(LayoutTestStyle.Create());
      l.Padding = new Insets(1, 2, 3, 4);
      l.Font = LayoutTestStyle.CreateFont();
      l.Text = "Test";
      l.Measure(Size.Auto);
      l.DesiredSize.Should().Be(new Size(50, 19));
    }

    [Test]
    public void TestResolvedStyle()
    {
      var uiFont = LayoutTestStyle.CreateFont();

      var l = new Label(LayoutTestStyle.Create());
      l.Padding = new Insets();
      l.Font = uiFont;
      l.Text = "Test";
      l.Measure(Size.Auto);
      l.Font.Should().BeSameAs(uiFont);
    }
  }
}