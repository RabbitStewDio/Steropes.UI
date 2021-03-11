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

using Steropes.UI.Widgets.TextWidgets.Documents;
using Steropes.UI.Widgets.TextWidgets.Documents.Helper;

namespace Steropes.UI.Test.UI.TextWidgets.Documents
{
  [Category("Text Behaviour")]
  public class PositionCollectionTest
  {
    [Test]
    public void BiasBackward_After_Ignored()
    {
      var pc = new PositionCollection();
      var end = pc.Create(10, Bias.Backward);
      pc.InsertAt(15, 10);

      end.Offset.Should().Be(10);
    }

    [Test]
    public void BiasBackward_Before_Shifts()
    {
      var pc = new PositionCollection();
      var end = pc.Create(10, Bias.Backward);
      pc.InsertAt(5, 10);

      end.Offset.Should().Be(20);
    }

    [Test]
    public void BiasBackward_OnPos_Ignores()
    {
      var pc = new PositionCollection();
      var end = pc.Create(10, Bias.Backward);
      pc.InsertAt(10, 10);

      end.Offset.Should().Be(20);
    }

    [Test]
    public void BiasForward_After_Ignored()
    {
      var pc = new PositionCollection();
      var end = pc.Create(10, Bias.Forward);
      pc.InsertAt(15, 10);

      end.Offset.Should().Be(10);
    }

    [Test]
    public void BiasForward_Before_Shifts()
    {
      var pc = new PositionCollection();
      var end = pc.Create(10, Bias.Forward);
      pc.InsertAt(5, 10);

      end.Offset.Should().Be(20);
    }

    [Test]
    public void BiasForward_OnPos_Ignores()
    {
      var pc = new PositionCollection();
      var end = pc.Create(10, Bias.Forward);
      pc.InsertAt(10, 10);

      end.Offset.Should().Be(10);
    }

    [Test]
    public void Insert_After()
    {
      var pc = new PositionCollection();
      var start = pc.Create(5, Bias.Forward);
      var end = pc.Create(10, Bias.Backward);
      pc.InsertAt(20, 10);
      start.Offset.Should().Be(5);
      end.Offset.Should().Be(10);
    }

    [Test]
    public void Insert_At_End()
    {
      var pc = new PositionCollection();
      var start = pc.Create(0, Bias.Forward);
      var end = pc.Create(10, Bias.Backward);
      pc.InsertAt(10, 10);
      start.Offset.Should().Be(0);
      end.Offset.Should().Be(20);
    }

    [Test]
    public void Insert_At_start()
    {
      var pc = new PositionCollection();
      var start = pc.Create(0, Bias.Forward);
      var end = pc.Create(0, Bias.Backward);
      pc.InsertAt(0, 10);
      start.Offset.Should().Be(0);
      end.Offset.Should().Be(10);
    }

    [Test]
    public void Insert_Before()
    {
      var pc = new PositionCollection();
      var start = pc.Create(5, Bias.Forward);
      var end = pc.Create(10, Bias.Backward);
      pc.InsertAt(0, 10);
      start.Offset.Should().Be(15);
      end.Offset.Should().Be(20);
    }

    /// <summary>
    ///   Demonstrate that with empty elements, offsets are wrong and cannot be trusted.
    /// </summary>
    [Test]
    public void Insert_WithEmptyElement()
    {
      var pc = new PositionCollection();
      var firstStart = pc.Create(5, Bias.Forward);
      var firstEnd = pc.Create(10, Bias.Backward);

      var start = pc.Create(10, Bias.Forward);
      var end = pc.Create(10, Bias.Backward);
      pc.InsertAt(10, 10);
      start.Offset.Should().Be(10);
      end.Offset.Should().Be(20);
      firstStart.Offset.Should().Be(5);
      firstEnd.Offset.Should().Be(20);
    }

    [Test]
    public void Remove_After()
    {
      var pc = new PositionCollection();
      var start = pc.Create(5, Bias.Forward);
      var end = pc.Create(10, Bias.Backward);

      pc.RemoveAt(20, 2);

      start.Offset.Should().Be(5);
      end.Offset.Should().Be(10);
    }

    [Test]
    public void Remove_Before()
    {
      var pc = new PositionCollection();
      var start = pc.Create(5, Bias.Forward);
      var end = pc.Create(10, Bias.Backward);

      pc.RemoveAt(0, 2);

      start.Offset.Should().Be(3);
      end.Offset.Should().Be(8);
    }

    [Test]
    public void Remove_In_Middle()
    {
      var pc = new PositionCollection();
      var start = pc.Create(5, Bias.Forward);
      var end = pc.Create(10, Bias.Backward);

      pc.RemoveAt(7, 2);

      start.Offset.Should().Be(5);
      end.Offset.Should().Be(8);
    }

    [Test]
    public void Remove_On_End()
    {
      var pc = new PositionCollection();
      var start = pc.Create(5, Bias.Forward);
      var end = pc.Create(10, Bias.Backward);

      pc.RemoveAt(10, 2);

      start.Offset.Should().Be(5);
      end.Offset.Should().Be(10);
    }

    [Test]
    public void Remove_On_Start()
    {
      var pc = new PositionCollection();
      var start = pc.Create(5, Bias.Forward);
      var end = pc.Create(10, Bias.Backward);

      pc.RemoveAt(5, 2);

      start.Offset.Should().Be(5);
      end.Offset.Should().Be(8);
    }

    [Test]
    public void SortedInsertation()
    {
      var pc = new PositionCollection();
      pc.Create(0, Bias.Forward);
      pc.Create(10, Bias.Backward);
      pc.Create(6, Bias.Backward);
      pc.Create(6, Bias.Forward);
      pc.Contents().Should().BeInAscendingOrder(new TextPositionComparer());
    }

    class TextPositionComparer : IComparer<ITextPosition>
    {
      public int Compare(ITextPosition me, ITextPosition other)
      {
        var c1 = me.Offset.CompareTo(other.Offset);
        if (c1 != 0)
        {
          return c1;
        }
        var b1 = (int)me.Bias;
        var b2 = (int)other.Bias;
        return b1.CompareTo(b2);
      }
    }
  }
}