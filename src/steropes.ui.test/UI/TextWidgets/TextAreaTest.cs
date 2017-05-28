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

using Steropes.UI.Widgets.TextWidgets;

namespace Steropes.UI.Test.UI.TextWidgets
{
  [Category("Text Widgets")]
  public class TextAreaTest
  {
    [Test]
    public void DisplayLineNumberLayout()
    {
      var style = LayoutTestStyle.Create();

      var ta = new TextArea(style);
      style.StyleResolver.AddRoot(ta);

      ta.DisplayLineNumbers = true;
      ta.Text = "Hello World";
      ta.Arrange(new Rectangle(10, 20, 300, 100));

      ta.LayoutRect.Should().Be(new Rectangle(10, 20, 300, 100));
      ta.Content.LayoutRect.Should().Be(new Rectangle(51, 40, 239, 15));
    }
  }
}