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

namespace Steropes.UI.Animation
{
  public class BounceValue : AnimatedValue
  {
    public BounceValue(float start, float end, float duration, int bounceCount, float delay = 0f, AnimationLoop loop = AnimationLoop.NoLoop)
    {
      Start = start;
      BounceRestitution = 0.5f;
      End = end;
      Duration = duration;
      BounceCount = bounceCount;
      Delay = delay;
      Time = 0f;
      Loop = loop;
    }

    public BounceValue(float start, float end, float duration, int bounceCount, AnimationLoop loop) : this(start, end, duration, bounceCount, 0f, loop)
    {
    }

    public int BounceCount { get; set; }

    public float BounceRestitution { get; set; }

    public override float CurrentValue
    {
      get
      {
        if (Time <= Delay)
        {
          return Start;
        }

        if (Math.Abs(Duration) < 0.0005 || BounceCount == 0)
        {
          return Start;
        }

        var progress = (Time - Delay) / Duration;

        var bounceInterval = 1.0 / BounceCount;
        progress += bounceInterval / 2.0;

        float bounceNumber = (int)progress * BounceCount;
        var currentBounceProgress = (progress - bounceNumber * bounceInterval) / bounceInterval;

        if (currentBounceProgress > 0.5)
        {
          currentBounceProgress = 1.0 - currentBounceProgress;
        }
        currentBounceProgress = 1.0 - (1.0 - currentBounceProgress) * (1.0 - currentBounceProgress);

        var bounceAmplitude = (float)Math.Pow(bounceNumber / BounceCount, BounceRestitution);
        var value = 1.0 - (1.0 - bounceAmplitude) * currentBounceProgress;

        return (float)(Start + value * (End - Start));
      }
    }

    public float End { get; set; }

    public float Start { get; set; }
  }
}