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
using Steropes.UI.Input.MouseInput;
using Steropes.UI.Util;
using Steropes.UI.Widgets.Container;

namespace Steropes.UI.Widgets
{
  public class Slider : InternalContentWidget<Widget>
  {
    public static readonly string SliderHandleStyleClass = "SliderHandle";

    readonly Button sliderHandle;
    readonly SliderTrack sliderTrack;
    readonly EventSupport<EventArgs> valueChangedSupport;

    bool pressed;
    float value;
    float maxValue;
    float minValue;
    float step;

    public Slider(IUIStyle style, float min, float max, float initialValue, float step) : base(style)
    {
      valueChangedSupport = new EventSupport<EventArgs>();

      sliderHandle = new Button(UIStyle);
      sliderHandle.AddStyleClass(SliderHandleStyleClass);
      sliderHandle.Anchor = AnchoredRect.CreateLeftAnchored();
      sliderHandle.MouseUp += OnMouseUp;
      sliderHandle.MouseDown += OnMouseDown;
      sliderHandle.MouseDragged += OnMouseDragged;

      sliderTrack = new SliderTrack(UIStyle);
      sliderTrack.Anchor = AnchoredRect.Full;

      Group elements = new Group(UIStyle);
      elements.Anchor = AnchoredRect.Full;
      elements.Add(sliderTrack);
      elements.Add(sliderHandle);

      InternalContent = elements;
      Focusable = true;

      MinValue = Math.Min(min, max);
      MaxValue = Math.Max(min, max);
      Step = step;
      Value = initialValue;

      MouseUp += OnMouseUp;
      MouseDown += OnMouseDown;
      MouseDragged += OnMouseDragged;
    }

    public event EventHandler<EventArgs> ValueChanged
    {
      add { valueChangedSupport.Event += value; }
      remove { valueChangedSupport.Event -= value; }
    }
    
    public EventHandler<EventArgs> OnValueChanged
    {
      get { return valueChangedSupport.Handler; }
      set { valueChangedSupport.Handler = value; }
    }

    public float MaxValue
    {
      get
      {
        return maxValue;
      }
      set
      {
        maxValue = value;
        OnPropertyChanged();
      }
    }

    public float MinValue
    {
      get
      {
        return minValue;
      }
      set
      {
        minValue = value;
        OnPropertyChanged();
      }
    }

    public float Step
    {
      get
      {
        return step;
      }
      set
      {
        step = value;
        OnPropertyChanged();
      }
    }

    public float Value
    {
      get
      {
        return value;
      }

      set
      {
        var clamped = MathHelper.Clamp(value, MinValue, MaxValue);
        if (Math.Abs(this.value - clamped) < 0.0005)
        {
          return;
        }
        this.value = clamped;
        if (Step > 0)
        {
          this.value -= this.value % Math.Abs(Step);
        }

        valueChangedSupport.Raise(this, EventArgs.Empty);
        OnPropertyChanged();
        InvalidateLayout();
      }
    }

    protected override Rectangle ArrangeOverride(Rectangle layoutSize)
    {
      var rect = sliderHandle.Anchor;
      rect.Left = (int)ValueToMousePosition(layoutSize.Width);
      sliderHandle.Anchor = rect;

      return base.ArrangeOverride(layoutSize);
    }

    protected override void OnPadMove(Direction direction)
    {
      if (direction == Direction.Left)
      {
        Value -= Step;
      }
      else if (direction == Direction.Right)
      {
        Value += Step;
      }
      else
      {
        base.OnPadMove(direction);
      }
    }

    float MousePositionToValue(float mouseX)
    {
      var borderRect = BorderRect;
      var railWidth = borderRect.Width - sliderHandle.DesiredSize.Width;
      var pos = mouseX - borderRect.X - sliderHandle.DesiredSize.Width / 2;
      return pos / railWidth;
    }

    void OnMouseDown(object source, MouseEventArgs args)
    {
      if (args.Button != MouseButton.Left)
      {
        return;
      }

      args.Consume();

      pressed = true;
      Tooltip?.DisplayNow();

      var progress = MousePositionToValue(args.Position.X);
      Value = MinValue + (int)Math.Floor(progress * (MaxValue - MinValue) / Step + 0.5f) * Step;
    }

    void OnMouseDragged(object source, MouseEventArgs args)
    {
      if (pressed)
      {
        args.Consume();

        var progress = MousePositionToValue(args.Position.X);
        var newValue = MinValue + (int)Math.Floor(progress * (MaxValue - MinValue) / Step + 0.5f) * Step;

        if (Math.Abs(newValue - Value) > 0.005)
        {
          Value = newValue;
        }
      }
    }

    void OnMouseUp(object source, MouseEventArgs args)
    {
      if (args.Button != MouseButton.Left)
      {
        return;
      }

      args.Consume();
      pressed = false;
    }

    float ValueToMousePosition(float width)
    {
      var railWidth = width - sliderHandle.DesiredSize.Width;
      var relativePos = (Value - MinValue) / (MaxValue - MinValue);
      return railWidth * relativePos;
    }

    /*
    protected override void DrawWidget(IBatchedDrawingService drawingService)
    {
      var rect = new Rectangle(LayoutRect.X, LayoutRect.Center.Y - UIStyle.Style.SliderHandleSize/2, LayoutRect.Width, UIStyle.Style.SliderHandleSize);

      drawingService.DrawBox(Frame, rect, Color.White);

      var handleX = rect.X + (int) ((rect.Width - UIStyle.Style.SliderHandleSize)*(float) (Value - MinValue)/(MaxValue - MinValue));

      drawingService.DrawBox(!pressed ? HandleFrame : HandleDownFrame,
                             new Rectangle(handleX, rect.Y, UIStyle.Style.SliderHandleSize, UIStyle.Style.SliderHandleSize),
                             Color.White);
      if (Hovered && !pressed)
      {
        drawingService.DrawBox(HandleHoverOverlay,
                               new Rectangle(handleX, rect.Y, UIStyle.Style.SliderHandleSize, UIStyle.Style.SliderHandleSize),
                               Color.White);
      }

      if (Screen?.FocusManager?.IsActive == true && Focused && !pressed)
      {
        drawingService.DrawBox(HandleFocusOverlay,
                               new Rectangle(handleX, rect.Y, UIStyle.Style.SliderHandleSize, UIStyle.Style.SliderHandleSize),
                               Color.White);
      }
    }
    */
  }
}