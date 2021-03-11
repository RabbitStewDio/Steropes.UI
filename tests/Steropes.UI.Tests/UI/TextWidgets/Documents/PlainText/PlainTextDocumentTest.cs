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

using Steropes.UI.Widgets.TextWidgets.Documents.PlainText;

namespace Steropes.UI.Test.UI.TextWidgets.Documents.PlainText
{
  [Category("Text Behaviour")]
  public class ImmutablePlainTextDocumentTest
  {
    [Test]
    public void AppendToLineBreaksAtEndOfLine()
    {
      var text = "Hello World\n";
      var doc = new PlainTextDocument();
      doc.InsertAt(0, text);
      doc.InsertAt(doc.TextLength, " More");
      doc.TextAt(0, doc.TextLength).Should().Be(text + " More");
      doc.Root.Offset.Should().Be(0);
      doc.Root.EndOffset.Should().Be(17);
      doc.Root.Count.Should().Be(2);
      doc.Root[0].Offset.Should().Be(0);
      doc.Root[0].EndOffset.Should().Be(12);
      doc.Root[1].Offset.Should().Be(12);
      doc.Root[1].EndOffset.Should().Be(17);
    }

    [Test]
    public void EditDocument()
    {
      var doc = new PlainTextDocument();
      doc.InsertAt(0, "A");
      doc.Root.Offset.Should().Be(0);
      doc.Root.EndOffset.Should().Be(1);
      doc.DeleteAt(0, doc.TextLength);
      doc.Root.Offset.Should().Be(0);
      doc.Root.EndOffset.Should().Be(0);
      doc.InsertAt(0, "B");
      doc.Root.Offset.Should().Be(0);
      doc.Root.EndOffset.Should().Be(1);
    }

    [Test]
    public void InsertAtEnd()
    {
      var text = "Hello World";
      var doc = new PlainTextDocument();
      doc.InsertAt(0, text);
      doc.InsertAt(doc.TextLength, " More");
      doc.TextAt(0, doc.TextLength).Should().Be("Hello World More");
      doc.Root.Count.Should().Be(1);
      doc.Root.Offset.Should().Be(0);
      doc.Root.EndOffset.Should().Be(16);
    }

    [Test]
    public void InsertNewLine()
    {
      var doc = new PlainTextDocument();
      doc.InsertAt(0, "Hello World!");
      doc.Root.Offset.Should().Be(0);
      doc.Root.EndOffset.Should().Be(12);
      doc.Root.Count.Should().Be(1);
      doc.Root[0].Offset.Should().Be(0);
      doc.Root[0].EndOffset.Should().Be(12);

      doc.InsertAt(5, '\n');
      doc.Root.Count.Should().Be(2);
      doc.Root[0].Offset.Should().Be(0);
      doc.Root[0].EndOffset.Should().Be(6);
      doc.Root[1].Offset.Should().Be(6);
      doc.Root[1].EndOffset.Should().Be(13);
    }

    [Test]
    public void InsertOrdinaryText()
    {
      var doc = new PlainTextDocument();
      doc.InsertAt(0, "Hello World");
      doc.TextAt(0, doc.TextLength).Should().Be("Hello World");
      doc.Root.Offset.Should().Be(0);
      doc.Root.EndOffset.Should().Be(11);
      doc.Root.Count.Should().Be(1);
    }

    [Test]
    public void InsertWithLineBreaks()
    {
      var text = "Hello\nWorld";
      var doc = new PlainTextDocument();
      doc.InsertAt(0, text);
      doc.Root.Offset.Should().Be(0);
      doc.Root.EndOffset.Should().Be(11);
      doc.Root.Count.Should().Be(2);
    }

    [Test]
    public void InsertWithLineBreaksAtEndOfLine()
    {
      var text = "Hello World\n";
      var doc = new PlainTextDocument();
      doc.InsertAt(0, text);
      doc.TextAt(0, doc.TextLength).Should().Be(text);
      doc.Root.Offset.Should().Be(0);
      doc.Root.EndOffset.Should().Be(12);
      doc.Root.Count.Should().Be(2);
      doc.Root[1].Offset.Should().Be(12);
      doc.Root[1].EndOffset.Should().Be(12);
    }

    [Test]
    public void RemoveAcrossLineBreaks()
    {
      var text = "Hello World\n More";
      var doc = new PlainTextDocument();
      doc.InsertAt(0, text);

      doc.DeleteAt(6, 7);
      doc.TextAt(0, doc.TextLength).Should().Be("Hello More");
      doc.Root.Offset.Should().Be(0);
      doc.Root.EndOffset.Should().Be(10);
      doc.Root.Count.Should().Be(1);
    }

    [Test]
    public void RemoveAllText()
    {
      var text = "Hello World";
      var doc = new PlainTextDocument();
      doc.InsertAt(0, text);
      doc.DeleteAt(0, text.Length);
      doc.TextAt(0, doc.TextLength).Should().Be("");
      doc.Root.Offset.Should().Be(0);
      doc.Root.EndOffset.Should().Be(0);
      doc.Root.Count.Should().Be(1);
    }
  }
}