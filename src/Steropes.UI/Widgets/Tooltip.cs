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

using Steropes.UI.Components;
using Steropes.UI.Styles;

namespace Steropes.UI.Widgets
{
  /// <summary>
  ///   A simple component that displays a contained widget as tooltip. Tooltips are normally rendered in the overlay draw
  ///   batch.
  ///   <seealso cref="IVisualContent.DrawOverlay" />
  ///   <p />
  ///   Tooltips can
  /// </summary>
  public class Tooltip<TContent> : ContentWidget<TContent>, IToolTip
    where TContent : class, IWidget
  {
    double tooltipTimer;

    public Tooltip(IUIStyle style) : base(style)
    {
    }

    /// <summary>
    ///   Controls how long to wait before showing the tooltip.
    /// </summary>
    public float TooltipDelay
    {
      get
      {
        return Style.GetValue(WidgetStyle.TooltipDelay);
      }
      set
      {
        Style.SetValue(WidgetStyle.TooltipDelay, value);
      }
    }

    /// <summary>
    ///   Controls for how long many seconds the tooltip is displayed.
    ///   If set to 0 or a negative value, the tooltip will never hide automatically.
    /// </summary>
    public float TooltipDisplayTime
    {
      get
      {
        return Style.GetValue(WidgetStyle.TooltipDisplayTime);
      }
      set
      {
        Style.SetValue(WidgetStyle.TooltipDisplayTime, value);
      }
    }

    public TooltipPositionMode TooltipPosition
    {
      get
      {
        return Style.GetValue(WidgetStyle.TooltipPosition);
      }
      set
      {
        Style.SetValue(WidgetStyle.TooltipPosition, value);
      }
    }

    /// <summary>
    ///   Immediately makes this tooltip visibile. This operation will reset the display-time for the tooltip.
    /// </summary>
    public void DisplayNow()
    {
      Visibility = Visibility.Visible;
      tooltipTimer = Math.Max(0, TooltipDelay);
    }

    public void ResetTimer()
    {
      tooltipTimer = 0;
    }

    public override void Update(GameTime time)
    {
      tooltipTimer += time.ElapsedGameTime.TotalSeconds;
      var delay = Math.Max(0, TooltipDelay);
      if (tooltipTimer < delay)
      {
        // hide if stil within the initial delay ..
        Visibility = Visibility.Collapsed;
      }
      else if (TooltipDisplayTime > 0 && tooltipTimer >= delay + TooltipDisplayTime)
      {
        // hide once the end of the display period has been reached.
        Visibility = Visibility.Collapsed;
      }
      else
      {
        // in between initial delay and final fade out, show the tooltip.
        Visibility = Visibility.Visible;
      }
      base.Update(time);
    }
  }
}