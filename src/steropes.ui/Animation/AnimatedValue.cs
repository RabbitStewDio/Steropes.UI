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
using Microsoft.Xna.Framework;

namespace Steropes.UI.Animation
{
  public enum AnimationDirection
  {
    Forward,

    Backward,
  }

  public enum AnimationLoop
  {
    NoLoop,

    Loop,

    LoopBackAndForth
  }

  public abstract class AnimatedValue
  {
    double time;

    public abstract float CurrentValue { get; }

    public double Delay { get; set; }

    public AnimationDirection Direction { get; set; }

    public double Duration { get; set; }

    public bool Stopped { get; set; }

    public bool IsOver
    {
      get
      {
        if (Loop == AnimationLoop.NoLoop)
        {
          if (Direction == AnimationDirection.Forward)
          {
            return Time >= Delay + Duration;
          }
          else
          {
            return Time <= 0;
          }
        }
        else
        {
          return false;
        }
      }
    }

    public bool IsRunning => !Stopped && !IsOver;

    public AnimationLoop Loop { get; set; }

    public double Time
    {
      get
      {
        return time;
      }
      set
      {
        time = value;
        UpdateDirection();
      }
    }

    public void FinishAnimation()
    {
      if (Direction == AnimationDirection.Backward)
      {
        Time = 0;
      }
      else
      {
        Time = Delay + Duration;
      }
    }

    public void StartAnimation()
    {
      if (Direction == AnimationDirection.Backward)
      {
        Time = Delay + Duration;
      }
      else
      {
        Time = 0;
      }
    }

    public void Update(GameTime time)
    {
      if (Stopped)
      {
        return;
      }

      var timeInSeconds = time.ElapsedGameTime.TotalSeconds;
      Time += Direction == AnimationDirection.Forward ? timeInSeconds : -timeInSeconds;
    }

    protected void UpdateDirection()
    {
      var animTotalTime = Delay + Duration;
      if (time >= animTotalTime)
      {
        switch (Loop)
        {
          case AnimationLoop.NoLoop:
            time = animTotalTime;
            break;
          case AnimationLoop.Loop:
            time -= animTotalTime;
            break;
          case AnimationLoop.LoopBackAndForth:
            time = animTotalTime - (Time - animTotalTime);
            Direction = Direction == AnimationDirection.Forward ? AnimationDirection.Backward : AnimationDirection.Forward;
            break;
        }
      }
      else if (Time < 0)
      {
        switch (Loop)
        {
          case AnimationLoop.NoLoop:
            time = 0;
            break;
          case AnimationLoop.Loop:
            time += animTotalTime;
            break;
          case AnimationLoop.LoopBackAndForth:
            time = -Time;
            Direction = Direction == AnimationDirection.Forward ? AnimationDirection.Backward : AnimationDirection.Forward;
            break;
        }
      }
    }
  }
}