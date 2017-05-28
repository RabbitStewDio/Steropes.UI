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
using System.Collections.Generic;

using FluentAssertions;

using NUnit.Framework;

using Steropes.UI.Util;

namespace Steropes.UI.Test.UI.TextWidgets.Documents
{
  [Category("Text Behaviour")]
  public class PushBackEnumeratorTest
  {
    [Test]
    public void EmptyEnumerator()
    {
      var en = new PushBackEnumerator<int>(new List<int>(), 0, 0);
      en.MoveNext().Should().BeFalse();
    }

    [Test]
    public void EmptyEnumeratorPushBack()
    {
      var en = new PushBackEnumerator<int>(new List<int>(), 0, 0);
      en.MoveNext().Should().BeFalse();
      en.PushBack(-1);
      en.MoveNext().Should().BeTrue();
      en.Current.Should().Be(-1);
      en.MoveNext().Should().BeFalse();
    }

    [Test]
    public void SingleContentEnumerator()
    {
      var en = new PushBackEnumerator<int>(new List<int> { 1, 2 }, 0, 1);
      en.MoveNext().Should().BeTrue();
      en.PushBack(-1);
      en.Current.Should().Be(1);
      en.MoveNext().Should().BeTrue();
      en.Current.Should().Be(-1);
      en.MoveNext().Should().BeFalse();
    }

    [Test]
    public void ThreeItemContentEnumerator()
    {
      var en = new PushBackEnumerator<int>(new List<int> { 1, 2, 3 }, 0, 3);
      en.MoveNext().Should().BeTrue();
      en.PushBack(-1);
      en.Current.Should().Be(1);
      en.MoveNext().Should().BeTrue();
      en.Current.Should().Be(-1);
      en.MoveNext().Should().BeTrue();
      en.Current.Should().Be(2);
      en.MoveNext().Should().BeTrue();
      en.Current.Should().Be(3);
      en.MoveNext().Should().BeFalse();
    }
  }
}