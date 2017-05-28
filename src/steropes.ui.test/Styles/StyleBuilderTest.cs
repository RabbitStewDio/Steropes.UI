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

using NUnit.Framework;

using Steropes.UI.Styles;
using Steropes.UI.Styles.Selector;
using Steropes.UI.Widgets;

namespace Steropes.UI.Test.Styles
{
  [Category("Styles")]
  public class StyleBuilderTest
  {
    [Test]
    public void TestBuildComplexRule()
    {
      var b = new StyleBuilder(LayoutTestStyle.CreateStyleSystem());

      var step2 = b.SelectForType<Button>().WithDirectChild(b.SelectForType<IconLabel>()).WithDirectChild(b.SelectForType<Label>());
      step2.ToString().Should().Be("Button > IconLabel > Label");
    }

    [Test]
    public void TestBuildSimple()
    {
      var b = new StyleBuilder(LayoutTestStyle.CreateStyleSystem());

      var step2 = b.SelectForType<Button>().WithDirectChild(b.SelectForType<IconLabel>());
      step2.Should().BeAssignableTo<DescendantSelector>().Which.Selector.ToString().Should().Be("IconLabel");
      step2.Should().BeAssignableTo<DescendantSelector>().Which.AnchestorSelector.ToString().Should().Be("Button");
      step2.ToString().Should().Be("Button > IconLabel");
    }

    [Test]
    public void TestMatching()
    {
      var uiStyle = LayoutTestStyle.Create();
      var styleSystem = uiStyle.StyleSystem;
      var b = new StyleBuilder(styleSystem);
      var step2 = b.SelectForType<Button>().WithDirectChild(b.SelectForType<IconLabel>());

      var button = new Button(uiStyle);
      step2.Matches(button.Content).Should().Be(true);
    }

    [Test]
    public void TestMatchingComplex()
    {
      var uiStyle = LayoutTestStyle.Create();
      var styleSystem = uiStyle.StyleSystem;
      var b = new StyleBuilder(styleSystem);
      var step2 = b.SelectForType<Button>().WithDirectChild(b.SelectForType<IconLabel>()).WithDirectChild(b.SelectForType<Label>());

      var button = new Button(uiStyle);
      step2.Matches(button.Content.Label).Should().Be(true);
      step2.ToString().Should().Be("Button > IconLabel > Label");
    }
  }
}