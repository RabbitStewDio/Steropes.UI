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
using Steropes.UI.Styles;
using Steropes.UI.Test.UI.TextWidgets.Documents.PlainText;
using Steropes.UI.Widgets.Styles;
using Steropes.UI.Widgets.TextWidgets;
using Steropes.UI.Widgets.TextWidgets.Documents.PlainText;
using Steropes.UI.Widgets.TextWidgets.Documents.Views;

namespace Steropes.UI.Test.UI.TextWidgets
{
  [Category("Text Widgets")]
  public class TextFieldTest
  {
    [Test]
    public void Arrange_Empty()
    {
      var textField = new TextField(LayoutTestStyle.Create());
      textField.Arrange(new Rectangle(10, 20, 300, 100));
      textField.LayoutRect.Should().Be(new Rectangle(10, 20, 300, 100));
    }

    [Test]
    public void Arrange_Long_Text_caret_at_end()
    {
      var style = LayoutTestStyle.Create();
      var textField = new TextField(style);
      style.StyleResolver.AddRoot(textField);

      textField.Padding = new Insets();
      textField.Text = "A very long text that will exceed the layout size we give to the edit-box";
      textField.Caret.MoveTo(textField.Caret.MaximumOffset - 10);

      textField.Arrange(new Rectangle(10, 20, 100, 30));
      textField.LayoutRect.Should().Be(new Rectangle(10, 20, 100, 30));
      textField.Content.LayoutRect.Should().Be(new Rectangle(-594, 20, 704, 15));
      textField.Caret.LayoutRect.Should().Be(new Rectangle(99, 20, 1, 15));
    }

    [Test]
    public void Arrange_Long_Text_caret_at_start()
    {
      var style = LayoutTestStyle.Create();
      var textField = new TextField(style);
      style.StyleResolver.AddRoot(textField);

      textField.Padding = new Insets();
      textField.Text = "A very long text that will exceed the layout size we give to the edit-box";
      textField.Caret.MoveTo(2);

      textField.Arrange(new Rectangle(10, 20, 100, 20));
      textField.LayoutRect.Should().Be(new Rectangle(10, 20, 100, 20));
      textField.Content.LayoutRect.Should().Be(new Rectangle(10, 20, 100, 15));
      textField.Caret.LayoutRect.Should().Be(new Rectangle(32, 20, 1, 15));
    }

    [Test]
    public void Arrange_Zero()
    {
      var style = LayoutTestStyle.Create();
      var textField = new TextField(style);
      style.StyleResolver.AddRoot(textField);

      textField.Padding = new Insets();
      textField.Arrange(new Rectangle(10, 20, 0, 0));
      textField.LayoutRect.Should().Be(new Rectangle(10, 20, 0, 0));
    }

    [Test]
    public void BackKey_Moves_Cursor_End()
    {
      var eb = new TextField(LayoutTestStyle.Create());
      eb.Text = "Hello World";
      eb.Caret.SelectionStartOffset.Should().Be(11);
      eb.Caret.SelectionEndOffset.Should().Be(11);
      eb.DispatchEvent(new KeyEventArgs(eb, new KeyEventData(KeyEventType.KeyPressed, TimeSpan.Zero, 0, InputFlags.None, Keys.Back)));
      eb.Text.Should().Be("Hello Worl");
      eb.Caret.SelectionStartOffset.Should().Be(10);
      eb.Caret.SelectionEndOffset.Should().Be(10);
    }

    [Test]
    public void BackKey_Moves_Cursor_Middle()
    {
      var eb = new TextField(LayoutTestStyle.Create());
      eb.Text = "Hello World";
      eb.Caret.MoveTo(5);
      eb.DispatchEvent(new KeyEventArgs(eb, new KeyEventData(KeyEventType.KeyPressed, TimeSpan.Zero, 0, InputFlags.None, Keys.Back)));
      eb.Text.Should().Be("Hell World");
      eb.Caret.SelectionStartOffset.Should().Be(4);
      eb.Caret.SelectionEndOffset.Should().Be(4);
    }

    [Test]
    public void Caret_Layout()
    {
      var style = LayoutTestStyle.Create();
      var textField = new TextField(style);
      style.StyleResolver.AddRoot(textField);

      textField.Padding = new Insets();
      textField.Text = "Hello World";
      textField.Caret.MoveTo(6);

      textField.Arrange(new Rectangle(10, 20, 400, 30));
      textField.Caret.LayoutRect.Should().Be(new Rectangle(76, 20, 1, 15));
    }

    [Test]
    public void EnsureWrapIsDisabled()
    {
      var uiStyle = LayoutTestStyle.Create();
      var style = uiStyle.StyleSystem.StylesFor<TextStyleDefinition>();

      var textField = new TextField(uiStyle);
      uiStyle.StyleResolver.AddRoot(textField);

      textField.Text = "TEst";
      textField.WrapText.Should().Be(WrapText.None);
      textField.Measure(Size.Auto);
      textField.WrapText.Should().Be(WrapText.None);

      textField.Arrange(new Rectangle(10, 20, 400, 40));
      textField.Content.Style.GetValue(style.WrapText).Should().Be(WrapText.None);
    }

    [Test]
    public void Initial_text_moves_caret_to_End()
    {
      var style = LayoutTestStyle.Create();
      var textField = new TextField(style);
      style.StyleResolver.AddRoot(textField);

      textField.Text = "Hello World";
      textField.Caret.SelectionStartOffset.Should().Be(11);
      textField.Caret.SelectionEndOffset.Should().Be(11);
    }

    [Test]
    public void Input_At_End()
    {
      var eb = new TextField(LayoutTestStyle.Create());
      eb.Text = "Hello World";
      eb.Caret.SelectionStartOffset.Should().Be(11);
      eb.Caret.SelectionEndOffset.Should().Be(11);
      eb.DispatchEvent(new KeyEventArgs(eb, new KeyEventData(KeyEventType.KeyTyped, TimeSpan.Zero, 0, InputFlags.None, 'a')));
      eb.Text.Should().Be("Hello Worlda");
      eb.Caret.SelectionStartOffset.Should().Be(12);
      eb.Caret.SelectionEndOffset.Should().Be(12);
    }

    [Test]
    public void Input_Delete_at_end_does_nothing()
    {
      var style = LayoutTestStyle.Create();
      var textField = new TextField(style);
      style.StyleResolver.AddRoot(textField);

      textField.Padding = new Insets();
      textField.Text = "Hello World";

      textField.DispatchEvent(new KeyEventArgs(textField, new KeyEventData(KeyEventType.KeyPressed, TimeSpan.Zero, 0, InputFlags.None, Keys.Delete)));
      textField.Caret.SelectionEndOffset.Should().Be(11);
      textField.Text.Should().Be("Hello World");
    }

    [Test]
    public void Input_In_Middle()
    {
      var eb = new TextField(LayoutTestStyle.Create());
      eb.Text = "Hello World";
      eb.Caret.MoveTo(6);
      eb.DispatchEvent(new KeyEventArgs(eb, new KeyEventData(KeyEventType.KeyTyped, TimeSpan.Zero, 0, InputFlags.None, 'a')));
      eb.Text.Should().Be("Hello aWorld");
      eb.Caret.SelectionStartOffset.Should().Be(7);
      eb.Caret.SelectionEndOffset.Should().Be(7);
    }

    [Test]
    public void Input_Navigate_then_backspace()
    {
      var style = LayoutTestStyle.Create();
      var textField = new TextField(style);
      style.StyleResolver.AddRoot(textField);

      textField.Padding = new Insets();
      textField.Text = "Hello World";
      textField.Caret.SelectionEndOffset.Should().Be(11);
      textField.Measure(Size.Auto);

      textField.DispatchEvent(new KeyEventArgs(textField, new KeyEventData(KeyEventType.KeyPressed, TimeSpan.Zero, 0, InputFlags.None, Keys.Left)));
      textField.Caret.SelectionEndOffset.Should().Be(10);
      textField.DispatchEvent(new KeyEventArgs(textField, new KeyEventData(KeyEventType.KeyPressed, TimeSpan.Zero, 0, InputFlags.None, Keys.Left)));
      textField.Caret.SelectionEndOffset.Should().Be(9);
      textField.DispatchEvent(new KeyEventArgs(textField, new KeyEventData(KeyEventType.KeyPressed, TimeSpan.Zero, 0, InputFlags.None, Keys.Back)));
      textField.Caret.SelectionEndOffset.Should().Be(8);
      textField.Text.Should().Be("Hello Wold");
    }

    [Test]
    public void Input_Navigate_then_delete()
    {
      var style = LayoutTestStyle.Create();
      var textField = new TextField(style);
      style.StyleResolver.AddRoot(textField);

      textField.Padding = new Insets();
      textField.Text = "Hello World";
      textField.Caret.SelectionEndOffset.Should().Be(11);
      textField.Measure(Size.Auto);

      textField.DispatchEvent(new KeyEventArgs(textField, new KeyEventData(KeyEventType.KeyPressed, TimeSpan.Zero, 0, InputFlags.None, Keys.Left)));
      textField.Caret.SelectionEndOffset.Should().Be(10);
      textField.DispatchEvent(new KeyEventArgs(textField, new KeyEventData(KeyEventType.KeyPressed, TimeSpan.Zero, 0, InputFlags.None, Keys.Left)));
      textField.Caret.SelectionEndOffset.Should().Be(9);
      textField.DispatchEvent(new KeyEventArgs(textField, new KeyEventData(KeyEventType.KeyPressed, TimeSpan.Zero, 0, InputFlags.None, Keys.Delete)));
      textField.Caret.SelectionEndOffset.Should().Be(9);
      textField.Text.Should().Be("Hello Word");
    }

    [Test]
    public void Input_When_selected_removes_text()
    {
      var eb = new TextField(LayoutTestStyle.Create());
      eb.Text = "Hello World";
      eb.Caret.MoveTo(6);
      eb.Caret.Select(8);

      eb.Caret.SelectionStartOffset.Should().Be(6);
      eb.Caret.SelectionEndOffset.Should().Be(8);

      eb.DispatchEvent(new KeyEventArgs(eb, new KeyEventData(KeyEventType.KeyTyped, TimeSpan.Zero, 0, InputFlags.None, 'a')));
      eb.Text.Should().Be("Hello arld");
      eb.Caret.SelectionStartOffset.Should().Be(7);
      eb.Caret.SelectionEndOffset.Should().Be(7);
    }

    [Test]
    public void Measure_Empty()
    {
      var style = LayoutTestStyle.Create();
      var textField = new TextField(style);
      style.StyleResolver.AddRoot(textField);
      textField.Measure(Size.Auto);
      textField.DesiredSize.Should().Be(new Size(30, 30));
    }

    [Test]
    public void View_Backspace()
    {
      var uiStyle = LayoutTestStyle.Create();

      var eb = new TextField(uiStyle, new TestDocumentViewEditor<PlainTextDocument>(new PlainTextDocumentEditor(uiStyle)));
      uiStyle.StyleResolver.AddRoot(eb);

      eb.Text = "Hello World";
      eb.Caret.SelectionEndOffset.Should().Be(11);
      eb.Arrange(new Rectangle(10, 20, 100, 40));

      eb.DispatchEvent(new KeyEventArgs(eb, new KeyEventData(KeyEventType.KeyPressed, TimeSpan.Zero, 0, InputFlags.None, Keys.Back)));
      eb.Text.Should().Be("Hello Worl");

      eb.Arrange(new Rectangle(10, 20, 100, 40));
      var exposed = (TestDocumentView<PlainTextDocument>)eb.Content;
      var chunk = (TextChunkView<PlainTextDocument>)exposed.ExposedRootView[0][0];
      chunk.Text.Should().Be("Hello Worl");
    }

    [Test]
    public void View_Navigate_then_backspace()
    {
      var style = LayoutTestStyle.Create();
      var textField = new TextField(style, new TestDocumentViewEditor<PlainTextDocument>(new PlainTextDocumentEditor(style)));
      style.StyleResolver.AddRoot(textField);

      textField.Padding = new Insets();
      textField.Text = "Hello World";
      textField.Caret.SelectionEndOffset.Should().Be(11);
      textField.Arrange(new Rectangle(10, 20, 100, 40));

      textField.DispatchEvent(new KeyEventArgs(textField, new KeyEventData(KeyEventType.KeyPressed, TimeSpan.Zero, 0, InputFlags.None, Keys.Left)));
      textField.DispatchEvent(new KeyEventArgs(textField, new KeyEventData(KeyEventType.KeyPressed, TimeSpan.Zero, 0, InputFlags.None, Keys.Left)));
      textField.DispatchEvent(new KeyEventArgs(textField, new KeyEventData(KeyEventType.KeyPressed, TimeSpan.Zero, 0, InputFlags.None, Keys.Back)));
      textField.Text.Should().Be("Hello Wold");

      textField.Arrange(new Rectangle(10, 20, 100, 40));
      var exposed = (TestDocumentView<PlainTextDocument>)textField.Content;
      var chunk = (TextChunkView<PlainTextDocument>)exposed.ExposedRootView[0][0];
      chunk.Text.Should().Be("Hello Wold");
    }

    [Test]
    public void View_Navigate_then_delete()
    {
      var style = LayoutTestStyle.Create();
      var textField = new TextField(style, new TestDocumentViewEditor<PlainTextDocument>(new PlainTextDocumentEditor(style)));
      style.StyleResolver.AddRoot(textField);

      textField.Padding = new Insets();
      textField.Text = "Hello World";
      textField.Caret.SelectionEndOffset.Should().Be(11);
      textField.Arrange(new Rectangle(10, 20, 100, 40));

      textField.DispatchEvent(new KeyEventArgs(textField, new KeyEventData(KeyEventType.KeyPressed, TimeSpan.Zero, 0, InputFlags.None, Keys.Left)));
      textField.DispatchEvent(new KeyEventArgs(textField, new KeyEventData(KeyEventType.KeyPressed, TimeSpan.Zero, 0, InputFlags.None, Keys.Left)));
      textField.DispatchEvent(new KeyEventArgs(textField, new KeyEventData(KeyEventType.KeyPressed, TimeSpan.Zero, 0, InputFlags.None, Keys.Delete)));
      textField.Text.Should().Be("Hello Word");

      textField.Arrange(new Rectangle(10, 20, 100, 40));
      var exposed = (TestDocumentView<PlainTextDocument>)textField.Content;
      var chunk = (TextChunkView<PlainTextDocument>)exposed.ExposedRootView[0][0];

      // the edit should have flushed out the old chunk. Chunks retain the text/strings to match what we give to SpriteBatch.
      chunk.Text.Should().Be("Hello Word");
    }
  }
}