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
using Steropes.UI.Components;
using Steropes.UI.Platform;

namespace Steropes.UI.Widgets
{
  public class ProgressBar : Widget
  {
    readonly LerpValue lerpValue;

    public ProgressBar(IUIStyle style) : base(style)
    {
      lerpValue = new LerpValue(0, 0, 0.5f, AnimationLoop.NoLoop);
      Min = 0;
      Max = 100;
      Value = 0;
    }

    public float Max { get; set; }

    public float Min { get; set; }

    public float Value
    {
      get
      {
        return lerpValue.End;
      }
      set
      {
        lerpValue.End = value;
        if (!lerpValue.IsRunning)
        {
          lerpValue.StartAnimation();
        }
      }
    }

    public void SetValue(int newValue, bool now = false)
    {
      lerpValue.End = newValue;
      if (now)
      {
        lerpValue.Start = newValue;
      }
      lerpValue.StartAnimation();
    }

    public override void Update(GameTime elapsedTime)
    {
      base.Update(elapsedTime);
      lerpValue.Update(elapsedTime);
    }

    protected override Rectangle ArrangeOverride(Rectangle layoutSize)
    {
      return layoutSize;
    }

    protected override void DrawWidgetStateOverlay(IBatchedDrawingService drawingService)
    {
      var borderRect = BorderRect;
      var effectiveValue = MathHelper.Clamp(lerpValue.CurrentValue, Min, Max) - Min;
      var usableWidth = Math.Max(0, borderRect.Width - DesiredSize.Width);
      var range = Math.Max(Max, Min) - Math.Min(Max, Min);
      var width = usableWidth * effectiveValue / range;

      if (width > 0)
      {
        var progressRect = new Rectangle(borderRect.X, borderRect.Y, (int)(width + DesiredSize.Width), borderRect.Height);
        drawingService.DrawBox(WidgetStateOverlayTexture, progressRect, WidgetStateOverlayColor);
      }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
      if (FrameTexture != null)
      {
        return new Size(FrameTexture.CornerArea.Horizontal, FrameTexture.CornerArea.Horizontal);
      }
      return new Size();
    }
  }
}