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
using Steropes.UI.Styles;
using Steropes.UI.Widgets;
using Steropes.UI.Widgets.Styles;

namespace Steropes.UI.Test.UI.Widgets
{
  [Category("Widgets")]
  public class CheckBoxTest
  {
    [Test]
    public void BasicLayout()
    {
      var style = LayoutTestStyle.Create();
      var cb = new CheckBox(style);
      style.StyleResolver.AddRoot(cb);

      cb.Text = "Hello";
      cb.Arrange(new Rectangle(10, 20, 300, 100));
      cb.LayoutRect.Should().Be(new Rectangle(10, 20, 300, 100));
      cb.Content[0].LayoutRect.Should().Be(new Rectangle(25, 50, 40, 40));
      cb.Content[1].LayoutRect.Should().Be(new Rectangle(65, 30, 75, 80));
    }

    [Test]
    public void CheckMark_Rendering()
    {
      var style = LayoutTestStyle.Create();
      var styleDefinition = style.StyleSystem.StylesFor<WidgetStyleDefinition>();

      var cb = new CheckBox(style);
      style.StyleResolver.AddRoot(cb);

      cb.Selected = SelectionState.Selected;
      cb.ValidateStyle();
      cb.Content[0].Should().BeAssignableTo<Button>();
      cb.Content[0].Style.GetValue(styleDefinition.WidgetStateOverlay)?.Name.Should().Be("UI/CheckBox/Checked");
    }

    [Test]
    public void Measure()
    {
      var style = LayoutTestStyle.Create();
      var cb = new CheckBox(style);
      style.StyleResolver.AddRoot(cb);

      cb.Text = "Hello";
      cb.Measure(Size.Auto);
      cb.DesiredSize.Should().Be(new Size(145, 60));
      cb.Content[0].DesiredSize.Should().Be(new Size(40, 40));
      cb.Content[1].DesiredSize.Should().Be(new Size(75, 35));
    }

    [Test]
    public void MeasureCheckMark()
    {
      var style = LayoutTestStyle.Create();

      var checkMark = new Button(style) { Anchor = AnchoredRect.CreateCentered() };

      style.StyleResolver.AddRoot(checkMark);
      checkMark.Measure(Size.Auto);
      checkMark.DesiredSize.Should().Be(new Size(40, 40));
    }
  }
}