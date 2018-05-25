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
using Microsoft.Xna.Framework.Input;
using NUnit.Framework;

using Steropes.UI.Components;
using Steropes.UI.Input;
using Steropes.UI.Input.KeyboardInput;
using Steropes.UI.Platform;
using Steropes.UI.Test.UI.TextWidgets.Documents.PlainText;
using Steropes.UI.Widgets;
using Steropes.UI.Widgets.Container;
using Steropes.UI.Widgets.TextWidgets;
using Steropes.UI.Widgets.TextWidgets.Documents.PlainText;

namespace Steropes.UI.Test.UI.Widgets
{
  [Category("Widgets")]
  public class ButtonTest
  {
    [Test]
    public void Measure()
    {
      var style = LayoutTestStyle.Create();
      var b = new Button(style);
      style.StyleResolver.AddRoot(b);

      b.Content.Label.Text = "Test";
      b.Measure(Size.Auto);
      b.DesiredSize.Should().Be(new Size(84, 55));
      b.Content.DesiredSize.Should().Be(new Size(44, 15));
    }

    [Test]
    public void MeasureWithMargins()
    {
      var style = LayoutTestStyle.Create();
      var b = new Button(style);
      style.StyleResolver.AddRoot(b);

      b.Margin = new Insets(5);
      b.Content.Label.Text = "Test";
      b.Measure(Size.Auto);
      b.DesiredSize.Should().Be(new Size(94, 65));
      b.Content.DesiredSize.Should().Be(new Size(44, 15));
    }

    [Test]
    public void TestArrange()
    {
      var style = LayoutTestStyle.Create();
      var b = new Button(style);
      style.StyleResolver.AddRoot(b);

      b.Content.Label.Text = "Test";
      b.Arrange(new Rectangle(10, 20, 200, 200));
      b.Content.Label.DesiredSize.Should().Be(new Size(44, 15));
      b.Content.Label.LayoutRect.Should().Be(new Rectangle(30, 113, 160, 15));
    }

    [Test]
    public void TestLabelAlignment()
    {
      var style = LayoutTestStyle.Create();
      var b = new Button(style);
      style.StyleResolver.AddRoot(b);

      b.Content.Label.Text = "Test";
      b.Measure(Size.Auto);
      b.Content.Label.Alignment.Should().Be(Alignment.Center);
    }

    [Test]
    public void Consumed_Widget_Events_Dont_Pass_To_Parent_Widgets()
    {
      var style = LayoutTestStyle.Create();
      var textField = new Button(style, "Hello");

      bool keyPressedReceived = false;
      var group = new Group(style);
      group.Focusable = true;
      group.Add(textField);
      group.KeyPressed += (s, e) => keyPressedReceived = true;

      var eventData = new KeyEventArgs(new KeyEventData(KeyEventType.KeyPressed, TimeSpan.Zero, 0, InputFlags.None, Keys.Left));
      textField.DispatchEvent(eventData);

      keyPressedReceived.Should().Be(false);
    }

    [Test]
    public void Repeat_Events_Dont_Pass_To_Parent_Widgets()
    {
      var style = LayoutTestStyle.Create();
      var textField = new Button(style, "Hello");

      bool keyPressedReceived = false;
      var group = new Group(style);
      group.Focusable = true;
      group.Add(textField);
      group.KeyRepeated += (s, e) => keyPressedReceived = true;

      var eventData = new KeyEventArgs(new KeyEventData(KeyEventType.KeyRepeat, TimeSpan.Zero, 0, InputFlags.None, Keys.Left));
      textField.DispatchEvent(eventData);

      keyPressedReceived.Should().Be(false);
    }
  }
}