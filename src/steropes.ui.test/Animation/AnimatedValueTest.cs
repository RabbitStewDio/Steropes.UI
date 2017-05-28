// MIT License
//
// Copyright (c) 2011-2016 Elisée Maurer, Sparklin Labs, Creative Patterns
// Copyright (c) 2016 Thomas Morgner, Rabbit-StewDio Ltd.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
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

using Steropes.UI.Animation;

namespace Steropes.UI.Test.Animation
{
  [Category("Platform Behaviour")]
  public class AnimatedValueTest
  {
    AnimatedValue lv;

    [SetUp]
    public void SetUp()
    {
      lv = new LerpValue(0, 1, 5, 2);
    }

    [Test]
    public void IsOver_Forward()
    {
      lv.Loop = AnimationLoop.NoLoop;
      lv.Direction = AnimationDirection.Forward;

      lv.Time = 10;
      lv.IsOver.Should().Be(true);

      lv.Time = 0;
      lv.IsOver.Should().Be(false);

      lv.Time = 7;
      lv.IsOver.Should().Be(true);

      lv.Time = -10;
      lv.IsOver.Should().Be(false);
    }

    [Test]
    public void IsOver_Backward()
    {
      lv.Loop = AnimationLoop.NoLoop;
      lv.Direction = AnimationDirection.Backward;

      lv.Time = 10;
      lv.IsOver.Should().Be(false);

      lv.Time = 0;
      lv.IsOver.Should().Be(true);

      lv.Time = 7;
      lv.IsOver.Should().Be(false);

      lv.Time = -10;
      lv.IsOver.Should().Be(true);
    }

    [Test]
    public void IsRunning_When_Direction_Forward_With_Delay()
    {
      lv.Direction = AnimationDirection.Forward;

      lv.Loop = AnimationLoop.NoLoop;
      lv.Update(new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(0.1)));
      lv.IsRunning.Should().Be(true);

      lv.Update(new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(5.1)));
      lv.IsRunning.Should().Be(true);

      lv.Update(new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(7.1)));
      lv.IsRunning.Should().Be(false);
    }

    [Test]
    public void IsRunning_When_Direction_Backward_With_Delay()
    {
      lv.Direction = AnimationDirection.Backward;
      lv.StartAnimation();

      lv.Loop = AnimationLoop.NoLoop;
      lv.Update(new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(0.1)));
      lv.IsRunning.Should().Be(true);

      lv.Update(new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(5.1)));
      lv.IsRunning.Should().Be(true);

      lv.Update(new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(7.1)));
      lv.IsRunning.Should().Be(false);
    }

    [Test]
    public void IsRunning_When_Stopped()
    {
      lv.Direction = AnimationDirection.Forward;
      lv.Loop = AnimationLoop.NoLoop;
      lv.StartAnimation();
      lv.Stopped = true;

      lv.Update(new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(0.1)));
      lv.IsRunning.Should().Be(false);

      lv.Update(new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(5.1)));
      lv.IsRunning.Should().Be(false);

      lv.Update(new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(7.1)));
      lv.IsRunning.Should().Be(false);
    }
  }
}