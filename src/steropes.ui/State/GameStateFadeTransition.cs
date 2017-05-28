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

using Microsoft.Xna.Framework;

using Steropes.UI.Animation;
using Steropes.UI.Platform;

namespace Steropes.UI.State
{
  public abstract class GameStateFadeTransition : GameState
  {
    const float TransitionDuration = 0.3f;

    protected GameStateFadeTransition(IBatchedDrawingService drawingService)
    {
      if (drawingService == null)
      {
        throw new ArgumentNullException(nameof(drawingService));
      }

      Transition = new LerpValue(0, 1, TransitionDuration);
      DrawingService = drawingService;
    }

    public IBatchedDrawingService DrawingService { get; }

    public AnimatedValue Transition { get; set; }

    protected bool Starting { get; private set; }

    protected bool Stopping { get; private set; }

    public override void DrawFadeIn()
    {
      Draw(Transition.CurrentValue);
    }

    public override void DrawFadeOut()
    {
      Draw(Transition.CurrentValue);
    }

    public override void Start()
    {
      base.Start();
      if (Transition != null)
      {
        Transition.Direction = AnimationDirection.Forward;
        Transition.StartAnimation();
      }
      Starting = true;
      Stopping = false;
    }

    public override bool UpdateFadeIn(GameTime time)
    {
      if (Transition.IsOver)
      {
        Transition.Update(time);
        Update(time);
        Starting = !Transition.IsOver;
        return Transition.IsOver;
      }

      Update(time);
      return true;
    }

    public override bool UpdateFadeOut(GameTime time)
    {
      if (Stopping == false)
      {
        Starting = false;
        Stopping = true;
        if (Transition != null)
        {
          Transition.FinishAnimation();
          Transition.Direction = AnimationDirection.Backward;
        }
      }

      if (Transition != null)
      {
        Transition.Update(time);
        Update(time);
        Stopping = !Transition.IsOver;
        return Transition.IsOver;
      }

      Update(time);
      return true;
    }

    void Draw(float transitionTime)
    {
      Draw();

      var fadeColor = Color.Black * (1f - transitionTime);

      DrawingService.StartDrawing();
      DrawingService.FillRect(new Rectangle(DrawingService.Bounds.X, DrawingService.Bounds.Y, DrawingService.Bounds.Width + 1, DrawingService.Bounds.Height + 1), fadeColor);
      DrawingService.EndDrawing();
    }
  }
}