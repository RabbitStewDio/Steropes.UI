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
using Steropes.UI.Input.MouseInput;
using Steropes.UI.Platform;
using Steropes.UI.Styles;
using Steropes.UI.Widgets.Styles;

namespace Steropes.UI.Widgets
{
  public class ScrollbarThumb : ButtonBase<Image>
  {
    public ScrollbarThumb(IUIStyle style) : base(style)
    {
    }
  }

  public class Scrollbar : InternalContentWidget<Widget>
  {
    readonly LerpValue lerpOffset;

    readonly ScrollbarStyleDefinition scrollbarStyle;

    bool isDragging;

    Point lastMouseDragPoint;

    int maximumVisibleOffset;

    float mouseOffset;

    public Scrollbar(IUIStyle style) : base(style)
    {
      scrollbarStyle = StyleSystem.StylesFor<ScrollbarStyleDefinition>();

      ScrollUnit = 50 / 120f;
      lerpOffset = new LerpValue(0, 0, 0.5f, AnimationLoop.NoLoop);

      MouseDragged += OnMouseDragged;
      MouseWheel += OnMouseWheel;
      MouseUp += OnMouseUp;
      MouseDown += OnMouseDown;
      MouseClicked += OnMouseClick;

      Thumb = new ScrollbarThumb(UIStyle);
    }

    public Widget Thumb
    {
      get
      {
        return InternalContent;
      }
      set
      {
        if (InternalContent != null)
        {
          InternalContent.MouseDragged -= OnMouseDragged;
          InternalContent.MouseWheel -= OnMouseWheel;
          InternalContent.MouseUp -= OnMouseUp;
          InternalContent.MouseDown -= OnMouseDown;
        }

        InternalContent = value;

        if (InternalContent != null)
        {
          InternalContent.MouseDragged += OnMouseDragged;
          InternalContent.MouseWheel += OnMouseWheel;
          InternalContent.MouseUp += OnMouseUp;
          InternalContent.MouseDown += OnMouseDown;
        }
      }
    }

    public event EventHandler<EventArgs> ScrollbarPositionChanged;

    public float LerpOffset => lerpOffset.CurrentValue;

    public int MaximumVisibleOffset
    {
      get
      {
        return maximumVisibleOffset;
      }

      private set
      {
        maximumVisibleOffset = value;
        Offset = Math.Min(Offset, maximumVisibleOffset);
      }
    }

    public ScrollbarMode ScrollbarMode
    {
      get
      {
        return Style.GetValue(scrollbarStyle.ScrollbarMode);
      }
      set
      {
        Style.SetValue(scrollbarStyle.ScrollbarMode, value);
      }
    }
    
    public int ScrollbarThumbHeight { get; private set; }

    public int ScrollbarThumbOffset { get; private set; }

    public int ScrollContentHeight { get; set; }

    public int ScrollContentOrigin => (int)(BorderRect.Y - LerpOffset);

    public bool Scrolling => Math.Abs(LerpOffset - Offset) > 1f;

    public float ScrollUnit { get; set; }

    protected bool ScrollbarVisible
    {
      get
      {
        switch (ScrollbarMode)
        {
          case ScrollbarMode.None:
            return false;
          case ScrollbarMode.Always:
            return true;
          case ScrollbarMode.Auto:
            return ScrollContentHeight > BorderRect.Height;
          default:
            throw new ArgumentException();
        }
      }
    }

    /// <summary>
    ///   The current offset for the content within scrollcontentheight.
    /// </summary>
    float Offset
    {
      get
      {
        return lerpOffset.End;
      }
      set
      {
        lerpOffset.End = value;
      }
    }

    public void Scroll(float delta, bool now = false)
    {
      ScrollTo(Offset + delta, now);
    }

    public void ScrollTo(float offset, bool now = false)
    {
      lerpOffset.Start = lerpOffset.CurrentValue;
      Offset = MathHelper.Clamp(offset, 0, LayoutInvalid ? ScrollContentHeight : MaximumVisibleOffset);
      if (now)
      {
        lerpOffset.Start = Offset;
        lerpOffset.FinishAnimation();
        InvalidateLayout();
      }
      else
      {
        lerpOffset.StartAnimation();
      }
    }

    public void ScrollToBottom(bool now = false)
    {
      var target = LayoutInvalid ? ScrollContentHeight : MaximumVisibleOffset;
      ScrollTo(target, now);
    }

    public void ScrollToTop(bool now = false)
    {
      ScrollTo(0, now);
    }

    public override void Update(GameTime elapsedTime)
    {
      base.Update(elapsedTime);
      var oldValue = lerpOffset.CurrentValue;
      lerpOffset.Update(elapsedTime);
      if (Math.Abs(LerpOffset - oldValue) > 0.005)
      {
        ScrollbarPositionChanged?.Invoke(this, new EventArgs());
        InvalidateLayout();
      }
    }

    protected override Rectangle ArrangeOverride(Rectangle layoutSize)
    {
      var scrollRect = layoutSize;
      var contentHeight = ScrollContentHeight;

      MaximumVisibleOffset = Math.Max(0, contentHeight - scrollRect.Height);
      Offset = MathHelper.Clamp(Offset, 0, MaximumVisibleOffset);
      lerpOffset.Start = MathHelper.Clamp(lerpOffset.Start, 0, MaximumVisibleOffset);

      ScrollbarThumbHeight = (int)ComputeThumbHeight(scrollRect.Height);
      ScrollbarThumbOffset = (int)ComputeThumbOffset(scrollRect.Height);

      var width = 10;
      if (Thumb != null)
      {
        var thumbWidth = Thumb.DesiredSize.WidthInt;
        Thumb.Arrange(new Rectangle(layoutSize.Right - thumbWidth, layoutSize.Y + ScrollbarThumbOffset, thumbWidth, ScrollbarThumbHeight));
        width = Thumb.DesiredSize.WidthInt;
      }
      return new Rectangle(scrollRect.Right - width, scrollRect.Y, width, scrollRect.Height);
    }

    protected override void DrawChildren(IBatchedDrawingService drawingService)
    {
      if (ScrollbarVisible)
      {
        base.DrawChildren(drawingService);
      }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
      Thumb?.Measure(availableSize);
      var width = Thumb?.DesiredSize.WidthInt ?? 10;
      var height = Thumb?.DesiredSize.HeightInt ?? 10;
      return new Size(width, float.IsInfinity(availableSize.Height) ? height : availableSize.Height);
    }

    float ComputeThumbHeight(float widgetHeight)
    {
      if (ScrollContentHeight <= 0)
      {
        return widgetHeight;
      }

      var minHeightFromTexture = Thumb?.DesiredSize.HeightInt ?? 10;
      var thumbSize = widgetHeight / ScrollContentHeight * widgetHeight;
      return MathHelper.Clamp(thumbSize, minHeightFromTexture, widgetHeight);
    }

    float ComputeThumbOffset(float widgetHeight)
    {
      if (MaximumVisibleOffset <= 0)
      {
        return 0;
      }

      var extraTrackSpace = Math.Max(0, widgetHeight - ScrollbarThumbHeight);
      if (extraTrackSpace <= 0)
      {
        return 0;
      }

      return LerpOffset * extraTrackSpace / MaximumVisibleOffset;
    }

    void OnMouseClick(object source, MouseEventArgs args)
    {
      args.Consume();
      var borderRect = BorderRect;
      var scrollBarTop = borderRect.Top + ScrollbarThumbOffset;
      var scrollbarBottom = borderRect.Top + ScrollbarThumbOffset + ScrollbarThumbHeight;
      if (args.Position.Y < scrollBarTop)
      {
        var x = Math.Max(1, borderRect.Height - ScrollbarThumbHeight) / 2;
        Scroll(-TranslateMouseOffsetToScrollOffset(x));
      }
      else if (args.Position.Y > scrollbarBottom)
      {
        var x = Math.Max(1, borderRect.Height - ScrollbarThumbHeight) / 2;
        Scroll(TranslateMouseOffsetToScrollOffset(x));
      }
    }

    void OnMouseDown(object source, MouseEventArgs args)
    {
      if (args.Button != MouseButton.Left)
      {
        return;
      }

      args.Consume();

      var borderRect = BorderRect;
      var scrollBarTop = borderRect.Top + ScrollbarThumbOffset;
      var scrollbarBottom = borderRect.Top + ScrollbarThumbOffset + ScrollbarThumbHeight;

      var inside = borderRect.Contains(args.Position);
      if ((inside && args.Position.Y >= scrollBarTop && args.Position.Y <= scrollbarBottom) || !inside)
      {
        isDragging = true;
        lastMouseDragPoint = args.Position;
        mouseOffset = Offset;
      }
    }

    void OnMouseDragged(object source, MouseEventArgs args)
    {
      if (isDragging)
      {
        args.Consume();

        var mouseDragPoint = args.Position;
        var mouseDragDelta = mouseDragPoint.Y - lastMouseDragPoint.Y;

        var borderRect = BorderRect;
        mouseOffset += (float)mouseDragDelta * borderRect.Height / ScrollbarThumbHeight;
        lastMouseDragPoint = mouseDragPoint;
        ScrollTo(mouseOffset, true);
      }
    }

    void OnMouseUp(object source, MouseEventArgs args)
    {
      if (args.Button != MouseButton.Left)
      {
        return;
      }

      args.Consume();
      isDragging = false;
    }

    void OnMouseWheel(object source, MouseEventArgs args)
    {
      args.Consume();

      var scrollWheelDelta = args.ScrollWheelDelta;
      if ((scrollWheelDelta < 0 && Offset >= MaximumVisibleOffset) || (scrollWheelDelta > 0 && Offset <= 0))
      {
        return;
      }

      var delta = (int)(-scrollWheelDelta * Math.Max(0, ScrollUnit));
      Scroll(delta);
    }

    int TranslateMouseOffsetToScrollOffset(float mouseMovementInPixel)
    {
      if (MaximumVisibleOffset <= 0)
      {
        return 0;
      }

      var extraTrackSpace = Math.Max(0, BorderRect.Height - ScrollbarThumbHeight);
      if (extraTrackSpace <= 0)
      {
        return 0;
      }

      return (int)(mouseMovementInPixel * MaximumVisibleOffset / extraTrackSpace);
    }
  }
}