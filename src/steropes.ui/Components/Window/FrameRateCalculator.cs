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
using System.Diagnostics;

using Microsoft.Xna.Framework;

namespace Steropes.UI.Components.Window
{
  public class FrameRateCalculator
  {
    static readonly TimeSpan Second = TimeSpan.FromSeconds(1);

    readonly Stopwatch usedTime;

    TimeSpan elapsedTime;

    int frameCounter;

    double relativeCpuTime;

    int updateCounter;

    public FrameRateCalculator()
    {
      elapsedTime = TimeSpan.Zero;
      usedTime = new Stopwatch();
    }

    public int FrameRate { get; private set; }

    public int UpdateRate { get; private set; }

    public void BeginTime()
    {
      usedTime.Start();
    }

    public void Draw()
    {
      frameCounter += 1;
    }

    public void EndTime()
    {
      usedTime.Stop();
    }

    public override string ToString()
    {
      return $"Updates: {UpdateRate} Draw: {FrameRate} - %CPU: {relativeCpuTime * 100}";
    }

    public void Update(GameTime time)
    {
      elapsedTime += time.ElapsedGameTime;
      if (elapsedTime > Second)
      {
        relativeCpuTime = usedTime.Elapsed.TotalSeconds / elapsedTime.TotalSeconds;
        FrameRate = frameCounter;
        UpdateRate = updateCounter;

        elapsedTime -= Second;
        usedTime.Reset();
        frameCounter = 0;
        updateCounter = 0;
      }
      updateCounter += 1;
    }
  }
}