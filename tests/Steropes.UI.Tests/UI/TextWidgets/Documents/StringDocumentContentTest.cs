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

using Steropes.UI.Widgets.TextWidgets.Documents;

namespace Steropes.UI.Test.UI.TextWidgets.Documents
{
  [Category("Text Behaviour")]
  public class StringDocumentContentTest
  {
    [Test]
    public void Insert_At_End()
    {
      var dc = new StringDocumentContent();
      dc.Insert(0, "Test ");
      dc.Insert(5, "Test!");
      dc.TextAt(0, 10).Should().Be("Test Test!");
    }

    [Test]
    public void Insert_At_Start()
    {
      var dc = new StringDocumentContent();
      dc.Insert(0, "Test ");
      dc.Insert(0, "Test!");
      dc.TextAt(0, 10).Should().Be("Test!Test ");
    }

    [Test]
    public void Insert_In_Middle()
    {
      var dc = new StringDocumentContent();
      dc.Insert(0, "TestTest");
      dc.Insert(4, "!Test!");
      dc.TextAt(0, 14).Should().Be("Test!Test!Test");
    }

    [Test]
    public void InsertRemoveTest()
    {
      var dc = new StringDocumentContent();
      dc.Insert(0, "Test ");
      dc.Remove(0, 5);
      dc.Length.Should().Be(0);
    }
  }
}