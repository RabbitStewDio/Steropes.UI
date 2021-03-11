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

using NUnit.Framework;

using Steropes.UI.Components;
using Steropes.UI.Styles;
using Steropes.UI.Widgets;

namespace Steropes.UI.Test.UI.Widgets
{
  [Category("Platform Behaviour")]
  public class WidgetTest
  {
    [Test]
    public void Focused_State_Is_Reflected_In_PseudoClasses()
    {
      var w = LayoutTestWidget.FixedSize(100, 100);
      w.PseudoClasses.Contains(WidgetPseudoClasses.FocusedPseudoClass).Should().Be(false);
      w.Focused = true;
      w.PseudoClasses.Contains(WidgetPseudoClasses.FocusedPseudoClass).Should().Be(true);
      w.Focused = false;
      w.PseudoClasses.Contains(WidgetPseudoClasses.FocusedPseudoClass).Should().Be(false);
    }

    [Test]
    public void Hover_State_Is_Reflected_In_PseudoClasses()
    {
      var w = LayoutTestWidget.FixedSize(100, 100);
      w.PseudoClasses.Contains(WidgetPseudoClasses.HoveredPseudoClass).Should().Be(false);
      w.Hovered = true;
      w.PseudoClasses.Contains(WidgetPseudoClasses.HoveredPseudoClass).Should().Be(true);
      w.Hovered = false;
      w.PseudoClasses.Contains(WidgetPseudoClasses.HoveredPseudoClass).Should().Be(false);
    }

    [Test]
    public void Nested_Widgets_Revalidate_Styles()
    {
      var style = LayoutTestStyle.Create();
      var b = new StyleBuilder(style.StyleSystem);
      var widgetStyles = b.StyleSystem.StylesFor<TestStyleDefinition>();

      style.StyleResolver.StyleRules.Add(
        b.CreateRule(b.SelectForType<Button>().WithCondition(StyleBuilderExtensions.HasClass("Test")), b.CreateStyle().WithValue(widgetStyles.Inherited, "FromButton-With-Test")));
      style.StyleResolver.StyleRules.Add(b.CreateRule(b.SelectForType<Button>(), b.CreateStyle().WithValue(widgetStyles.Inherited, "FromButton")));

      var button = new Button(style);
      style.StyleResolver.AddRoot(button);

      button.ValidateStyle();

      button.Content.Style.GetValue(widgetStyles.Inherited).Should().Be("FromButton");
      button.AddStyleClass("Test");

      button.ValidateStyle();

      button.Style.GetValue(widgetStyles.Inherited).Should().Be("FromButton-With-Test");
      button.Content.Style.GetValue(widgetStyles.Inherited).Should().Be("FromButton-With-Test");
      button.ValidateStyle();
    }

    [Test]
    public void PropertyAccess()
    {
      var w = LayoutTestWidget.FixedSize(200, 200);
      w.Tag = "Test";
      w.GetPropertyValue(nameof(w.Tag)).Should().Be("Test");
    }

    [Test]
    public void PseudoClassChangeTriggeresLayoutChange()
    {
      bool invalidateCalled = false;
      var w = LayoutTestWidget.FixedSize(100, 100);
      w.Arrange(new Rectangle(10, 20, 200, 40));
      w.StyleInvalidated += (o, e) => invalidateCalled = true;
      w.LayoutInvalid.Should().Be(false);

      w.AddPseudoStyleClass("Test");
      invalidateCalled.Should().Be(true);
    }
  }

  public class TestStyleDefinition : IStyleDefinition
  {
    public IStyleKey<string> Inherited { get; private set; }

    public IStyleKey<string> Normal { get; private set; }

    public void Init(IStyleSystem s)
    {
      Normal = s.CreateKey<string>("TestKey", false);
      Inherited = s.CreateKey<string>("TestKey-Inherited", true);
    }
  }
}