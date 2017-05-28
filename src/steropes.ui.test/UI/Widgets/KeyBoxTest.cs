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
using Microsoft.Xna.Framework.Input;

using NUnit.Framework;

using Steropes.UI.Input;
using Steropes.UI.Input.KeyboardInput;
using Steropes.UI.Widgets;

namespace Steropes.UI.Test.UI.Widgets
{
  [Category("Widgets")]
  public class KeyBoxTest
  {
    [Test]
    public void When_Updating_key_stroke_text_gets_replaced()
    {
      var style = LayoutTestStyle.Create();
      var b = new KeyBox(style, new KeyStroke(Keys.A));
      style.StyleResolver.AddRoot(b);

      b.Content.Text.Should().Be(Keys.A.ToString());
      b.Arrange(new Rectangle(10, 20, 200, 40));

      b.Key = new KeyStroke(Keys.B, InputFlags.Shift);
      b.Content.Text.Should().Be("Shift B");
      b.Arrange(new Rectangle(10, 20, 200, 40));

      b.Key = new KeyStroke(Keys.C, InputFlags.Control | InputFlags.Shift);
      b.Content.Text.Should().Be("Ctrl-Shift C");
      b.Arrange(new Rectangle(10, 20, 200, 40));

      b.Key = new KeyStroke(Keys.A);
      b.Content.Text.Should().Be("A");
      b.Arrange(new Rectangle(10, 20, 200, 40));
    }
  }
}