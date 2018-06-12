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

using Microsoft.Xna.Framework;

using Steropes.UI.Animation;
using Steropes.UI.Components;
using Steropes.UI.Platform;

namespace Steropes.UI.Widgets
{
  public enum ProgressBarDirection
  {
    LeftToRight, RightToLeft
  }

  public class ProgressBar : Widget
  {
    readonly LerpValue lerpValue;
    float scale;
    float max;
    float min;
    ProgressBarDirection direction;

    public ProgressBar(IUIStyle style) : base(style)
    {
      lerpValue = new LerpValue(0, 0, 0.5f, AnimationLoop.NoLoop);
      Min = 0;
      Max = 100;
      Value = 0;
      Direction = ProgressBarDirection.LeftToRight;
    }

    public ProgressBarDirection Direction
    {
      get
      {
        return direction;
      }
      set
      {
        if (value == direction)
          return;
        direction = value;
        OnPropertyChanged();
      }
    }

    public double AnimationDuration
    {
      get
      {
        return lerpValue.Duration;
      }
      set
      {
        lerpValue.Duration = value;
      }
    }

    public float Max
    {
      get
      {
        return max;
      }
      set
      {
        if (value.Equals(max))
          return;
        max = value;
        OnPropertyChanged();
        if (scale > 0)
        {
          InvalidateLayout();
        }
      }
    }

    public float Min
    {
      get
      {
        return min;
      }
      set
      {
        if (value.Equals(min))
          return;
        min = value;
        OnPropertyChanged();
        if (scale > 0)
        {
          InvalidateLayout();
        }
      }
    }

    /// <summary>
    ///   Reserves space in the content area of the progress bar for each unit of value.
    ///   Use this to request a minimum size for the progress bar without having to worry
    ///   about margins and frame borders.
    /// </summary>
    public float Scale
    {
      get
      {
        return scale;
      }
      set
      {
        if (value.Equals(scale))
        {
          return;
        }
        scale = value;
        OnPropertyChanged();
        InvalidateLayout();
      }
    }

    public float Value
    {
      get
      {
        return lerpValue.End;
      }
      set
      {
        if (value.Equals(lerpValue.End))
          return;

        lerpValue.Start = lerpValue.End;
        lerpValue.End = value;
        if (!lerpValue.IsRunning)
        {
          lerpValue.StartAnimation();
        }
      }
    }

    [Obsolete]
    public void SetValue(int newValue, bool now = false)
    {
      SetValue((float) newValue, now);
    }

    public void SetValue(float newValue, bool now = false)
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
      var reservedSize = FrameTexture?.CornerArea.Horizontal ?? 0;
      var usableWidth = Math.Max(0, borderRect.Width - reservedSize);
      var range = Math.Max(Max, Min) - Math.Min(Max, Min);

      var effectiveValue = MathHelper.Clamp(lerpValue.CurrentValue, Min, Max) - Min;
      var width = usableWidth * effectiveValue / range;

      if (width > 0)
      {
        Rectangle progressRect;
        if (Direction == ProgressBarDirection.LeftToRight)
        {
          progressRect = new Rectangle(borderRect.X, borderRect.Y, 
                                       (int) (width + reservedSize), borderRect.Height);
        }
        else
        {
          progressRect = new Rectangle((int) (borderRect.X + (usableWidth - width)), borderRect.Y, 
                                       (int) (width + reservedSize), borderRect.Height);
        }
        drawingService.DrawBox(WidgetStateOverlayTexture, progressRect, WidgetStateOverlayColor);
      }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
      var range = Math.Max(Max, Min) - Math.Min(Max, Min);
      var scaledRange = Math.Max (0, range * Scale);
      if (FrameTexture != null)
      {
        return new Size(scaledRange + FrameTexture.CornerArea.Horizontal, FrameTexture.CornerArea.Horizontal);
      }
      return new Size(scaledRange, 0);
    }
  }
}