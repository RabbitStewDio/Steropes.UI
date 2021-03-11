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

using NUnit.Framework;

using Steropes.UI.Components;

namespace Steropes.UI.Test.UI
{
  [Category("Basic Widget Behaviour")]
  public class AnchoredRectTest
  {
    [Test]
    public void MeasureFull()
    {
      AnchoredRect.Full.ResolveSize(new Size(200, 200)).Should().Be(new Size(200, 200));
    }

    [Test]
    public void MeasureThrows()
    {
      Assert.Throws<ArgumentOutOfRangeException>(() => AnchoredRect.Full.ResolveSize(new Size(0, float.PositiveInfinity)));
      Assert.Throws<ArgumentOutOfRangeException>(() => AnchoredRect.Full.ResolveSize(new Size(float.PositiveInfinity, 0)));
    }

    public void MeasureTopLeft()
    {
      AnchoredRect.CreateTopLeftAnchored(1, 2).ResolveSize(new Size(0, 0)).Should().Be(new Size(1, 2));
      AnchoredRect.CreateTopLeftAnchored(1, 2).ResolveSize(new Size(10, 10)).Should().Be(new Size(11, 12));
      AnchoredRect.CreateTopLeftAnchored(1, 2, 20, 30).ResolveSize(new Size(10, 10)).Should().Be(new Size(21, 32));
    }

    public void MeasureTopLeftThrows()
    {
      Assert.Throws<ArgumentOutOfRangeException>(() => AnchoredRect.CreateTopLeftAnchored(1, 2, -1, -1));
    }
  }
}