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
using System.ComponentModel;
using Microsoft.Xna.Framework;

using Steropes.UI.Animation;
using Steropes.UI.Components;
using Steropes.UI.Components.Helper;
using Steropes.UI.Input.MouseInput;
using Steropes.UI.Platform;

namespace Steropes.UI.Widgets.Container
{
  /// <summary>
  ///   A pane that shows to widgets side by side with a draggable divider inbetween to change the space allocation between
  ///   the two sides.
  ///   <p />
  ///   This implementation treats all collapsed widgets in either side of the splitter as merely hidden. They will still
  ///   consume space.
  /// </summary>
  public class Splitter : Widget
  {
    readonly AnimatedValue collapseAnim;

    readonly SplitterBar splitterBar;

    bool displayFirstPane;

    bool displaySecondPane;

    int dragOffset;

    IWidget firstPane;

    int firstPaneMinSize;

    bool isDragging;

    IWidget secondPane;

    int secondPaneMinSize;

    int splitterOffset;

    public Splitter(IUIStyle style, Direction direction, bool collapsable = false) : base(style)
    {
      splitterBar = new SplitterBar(style);
      splitterBar.AddNotify(this);
      splitterBar.PropertyChanged += OnSplitterBarPropertyChange;
      RaiseChildAdded(0, splitterBar, null);

      collapseAnim = new SmoothValue(0f, 1f, 0.2f);

      Direction = direction;
      Collapsable = collapsable;
      FirstPaneMinSize = 100;
      Resizable = true;
      SecondPaneMinSize = 100;

      splitterBar.MouseDragged += OnMouseDragged;
      splitterBar.MouseUp += OnMouseUp;
      splitterBar.MouseDown += OnMouseDown;
      splitterBar.MouseClicked += OnMouseClick;
    }

    void OnSplitterBarPropertyChange(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == nameof(SplitterBar.Collapsed) ||
          e.PropertyName == nameof(SplitterBar.Collapsable))
      {
        OnPropertyChanged(e.PropertyName);
      }
    }

    public bool Collapsable
    {
      get
      {
        return splitterBar.Collapsable;
      }
      set
      {
        splitterBar.Collapsable = value;
      }
    }

    public override int Count
    {
      get
      {
        if (FirstPane == null && SecondPane == null)
        {
          return 1;
        }
        if (FirstPane == null)
        {
          return 2;
        }
        if (SecondPane == null)
        {
          return 2;
        }
        return 3;
      }
    }

    // NOTE: Splitter is using a Direction instead of an Orientation so
    // it know from which side the offset is computed
    public Direction Direction
    {
      get
      {
        return splitterBar.Direction;
      }
      set
      {
        splitterBar.Direction = value;
      }
    }

    public IWidget FirstPane
    {
      get
      {
        return firstPane;
      }
      set
      {
        if (firstPane != null)
        {
          RaiseChildRemoved(1, firstPane);
          firstPane.RemoveNotify(this);
        }

        firstPane = value;

        if (firstPane != null)
        {
          firstPane.AddNotify(this);
          RaiseChildAdded(1, firstPane);
        }
      }
    }

    public int FirstPaneMinSize
    {
      get
      {
        return firstPaneMinSize;
      }
      set
      {
        if (value == firstPaneMinSize)
        {
          return;
        }
        firstPaneMinSize = value;
        OnPropertyChanged();
        InvalidateLayout();
      }
    }

    public bool IsDragging
    {
      get
      {
        return isDragging;
      }
      private set
      {
        isDragging = value;
        OnPropertyChanged();
      }
    }

    public bool Resizable
    {
      get
      {
        return splitterBar.Resizable;
      }
      set
      {
        splitterBar.Resizable = value;
      }
    }

    public IWidget SecondPane
    {
      get
      {
        return secondPane;
      }
      set
      {
        if (secondPane != null)
        {
          RaiseChildRemoved(2, secondPane);
          secondPane.RemoveNotify(this);
        }

        secondPane = value;

        if (secondPane != null)
        {
          RaiseChildAdded(2, secondPane);
          secondPane.AddNotify(this);
        }
      }
    }

    public int SecondPaneMinSize
    {
      get
      {
        return secondPaneMinSize;
      }
      set
      {
        if (value == secondPaneMinSize)
        {
          return;
        }
        secondPaneMinSize = value;
        OnPropertyChanged();
        InvalidateLayout();
      }
    }

    public int SplitterOffset
    {
      get
      {
        return splitterOffset;
      }
      set
      {
        if (value == splitterOffset)
        {
          return;
        }
        splitterOffset = value;
        OnPropertyChanged();
        InvalidateLayout();
      }
    }

    public bool Collapsed
    {
      get
      {
        return splitterBar.Collapsed;
      }
      set
      {
        splitterBar.Collapsed = value;
        if (value)
        {
          collapseAnim.Direction = AnimationDirection.Forward;
          collapseAnim.Time = collapseAnim.Duration;
        }
        else
        {
          collapseAnim.Direction = AnimationDirection.Backward;
          collapseAnim.Time = 0;
        }
      }
    }

    public override IWidget this[int index]
    {
      get
      {
        switch (index)
        {
          case 0:
            return splitterBar;
          case 1:
            if (FirstPane != null)
            {
              return FirstPane;
            }
            if (SecondPane != null)
            {
              return SecondPane;
            }
            throw new IndexOutOfRangeException();
          case 2:
            if (FirstPane == null)
            {
              throw new IndexOutOfRangeException();
            }
            if (SecondPane == null)
            {
              throw new IndexOutOfRangeException();
            }
            return SecondPane;
          default:
            throw new IndexOutOfRangeException();
        }
      }
    }

    public void ToggleCollapse(bool now = false)
    {
      if (Collapsable)
      {
        if (!now)
        {
          splitterBar.Collapsed = !splitterBar.Collapsed;
        }
        else
        {
          Collapsed = !Collapsed;
        }
      }
    }

    public override void Update(GameTime elapsedTime)
    {
      base.Update(elapsedTime);
      var collapseValue = collapseAnim.CurrentValue;
      if (Collapsed)
      {
        collapseAnim.Direction = AnimationDirection.Forward;
        collapseAnim.Update(elapsedTime);
      }
      else
      {
        collapseAnim.Direction = AnimationDirection.Backward;
        collapseAnim.Update(elapsedTime);
      }

      if (Math.Abs(collapseValue - collapseAnim.CurrentValue) > 0.0005)
      {
        InvalidateLayout();
      }

      splitterBar.Update(elapsedTime);
      FirstPane?.Update(elapsedTime);
      SecondPane?.Update(elapsedTime);
    }

    protected override Rectangle ArrangeOverride(Rectangle layoutSize)
    {
      if (Collapsed && collapseAnim.IsOver)
      {
        displayFirstPane = Direction != Direction.Left && Direction != Direction.Up;
        displaySecondPane = Direction != Direction.Right && Direction != Direction.Down;
      }
      else
      {
        displayFirstPane = true;
        displaySecondPane = true;
      }

      switch (Direction)
      {
        case Direction.Left:
          return ArrangeLeft(layoutSize);
        case Direction.Right:
          return ArrangeRight(layoutSize);
        case Direction.Up:
          return ArrangeUp(layoutSize);
        case Direction.Down:
          return ArrangeDown(layoutSize);
      }
      return layoutSize;
    }

    protected Rectangle FirstPaneClippingRect()
    {
      var rect = FirstPane?.LayoutRect ?? LayoutRect;
      if (Direction == Direction.Left || Direction == Direction.Right)
      {
        // horizontal split
        rect.Width = splitterBar.LayoutRect.X - rect.X;
      }
      else
      {
        // vertical split
        rect.Height = splitterBar.LayoutRect.Y - rect.Y;
      }
      return rect;
    }

    protected Rectangle SecondPaneClippingRect()
    {
      var rect = SecondPane?.LayoutRect ?? LayoutRect;
      if (Direction == Direction.Left || Direction == Direction.Right)
      {
        // horizontal split
        rect.Width = rect.Right - splitterBar.LayoutRect.Right;
        rect.X = splitterBar.LayoutRect.Right;
      }
      else
      {
        // vertical split
        rect.Height = rect.Bottom - splitterBar.LayoutRect.Bottom;
        rect.Y = splitterBar.LayoutRect.Bottom;
      }

      return rect;
    }

    protected override void DrawChildren(IBatchedDrawingService drawingService)
    {
      splitterBar.Draw(drawingService);

      if (displayFirstPane && FirstPane != null)
      {
        var clipping = FirstPaneClippingRect();
        drawingService.PushScissorRectangle(clipping);
        FirstPane.Draw(drawingService);
        drawingService.PopScissorRectangle();
      }

      if (displaySecondPane && SecondPane != null)
      {
        drawingService.PushScissorRectangle(SecondPaneClippingRect());
        SecondPane.Draw(drawingService);
        drawingService.PopScissorRectangle();
      }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
      splitterBar.Measure(availableSize);

      var firstComponentSize = new Size();
      var secondComponentSize = new Size();
      if (Direction.IsHorizontal())
      {
        if (float.IsInfinity(availableSize.Width))
        {
          firstComponentSize.Width = float.PositiveInfinity;
          secondComponentSize.Width = float.PositiveInfinity;
        }
        else
        {
          var firstComponentConsumedSpace = Math.Max(Math.Min(availableSize.Width, SplitterOffset), FirstPaneMinSize);
          var remainingSpace = Math.Max(0, availableSize.Width - firstComponentConsumedSpace - splitterBar.DesiredSize.Width);
          firstComponentSize.Width = firstComponentConsumedSpace;
          secondComponentSize.Width = Math.Max(remainingSpace, SecondPaneMinSize);
        }

        firstComponentSize.Height = availableSize.Height;
        secondComponentSize.Height = availableSize.Height;
      }
      else
      {
        firstComponentSize.Width = availableSize.Width;
        secondComponentSize.Width = availableSize.Width;

        if (float.IsInfinity(availableSize.Height))
        {
          firstComponentSize.Height = float.PositiveInfinity;
          secondComponentSize.Height = float.PositiveInfinity;
        }
        else
        {
          var firstComponentConsumedSpace = Math.Max(Math.Min(availableSize.Height, SplitterOffset), FirstPaneMinSize);
          var remainingSpace = Math.Max(0, availableSize.Height - firstComponentConsumedSpace - splitterBar.DesiredSize.Height);
          firstComponentSize.Height = firstComponentConsumedSpace;
          secondComponentSize.Height = Math.Max(remainingSpace, SecondPaneMinSize);
        }
      }

      FirstPane?.Measure(firstComponentSize);
      SecondPane?.Measure(secondComponentSize);

      firstComponentSize = FirstPane?.DesiredSize ?? new Size();
      secondComponentSize = SecondPane?.DesiredSize ?? new Size();

      switch (Direction)
      {
        case Direction.Left:
          {
            SplitterOffset = Math.Max(SplitterOffset, FirstPaneMinSize);
            var collapsedSplitterOffset = (int)Math.Max(0, (1f - collapseAnim.CurrentValue) * SplitterOffset);
            var secondPaneWidth = Math.Max(SecondPaneMinSize, SecondPane?.DesiredSize.Width ?? 0);
            var width = secondPaneWidth + splitterBar.DesiredSize.Width + collapsedSplitterOffset;

            return new Size(width, Math.Max(splitterBar.DesiredSize.Height, Math.Max(firstComponentSize.Height, secondComponentSize.Height)));
          }
        case Direction.Right:
          {
            SplitterOffset = Math.Max(SplitterOffset, SecondPaneMinSize);
            var collapsedSplitterOffset = (int)Math.Max(0, (1f - collapseAnim.CurrentValue) * SplitterOffset);
            var firstPaneWidth = Math.Max(FirstPaneMinSize, FirstPane?.DesiredSize.Width ?? 0);
            var width = firstPaneWidth + splitterBar.DesiredSize.Width + collapsedSplitterOffset;

            return new Size(width, Math.Max(splitterBar.DesiredSize.Height, Math.Max(firstComponentSize.Height, secondComponentSize.Height)));
          }
        case Direction.Up:
          {
            SplitterOffset = Math.Max(SplitterOffset, FirstPaneMinSize);
            var collapsedSplitterOffset = (int)Math.Max(0, (1f - collapseAnim.CurrentValue) * SplitterOffset);
            var secondPaneHeight = Math.Max(SecondPaneMinSize, SecondPane?.DesiredSize.Height ?? 0);
            var height = secondPaneHeight + splitterBar.DesiredSize.Height + collapsedSplitterOffset;

            return new Size(Math.Max(splitterBar.DesiredSize.Width, Math.Max(firstComponentSize.Width, secondComponentSize.Width)), height);
          }
        case Direction.Down:
          {
            SplitterOffset = Math.Max(SplitterOffset, SecondPaneMinSize);
            var collapsedSplitterOffset = (int)Math.Max(0, (1f - collapseAnim.CurrentValue) * SplitterOffset);
            var firstPaneHeight = Math.Max(FirstPaneMinSize, FirstPane?.DesiredSize.Height ?? 0);
            var height = firstPaneHeight + splitterBar.DesiredSize.Height + collapsedSplitterOffset;

            return new Size(Math.Max(splitterBar.DesiredSize.Width, Math.Max(firstComponentSize.Width, secondComponentSize.Width)), height);
          }
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    Rectangle ArrangeDown(Rectangle layoutSize)
    {
      if (layoutSize.Height >= FirstPaneMinSize + SecondPaneMinSize)
      {
        SplitterOffset = MathHelper.Clamp(SplitterOffset, SecondPaneMinSize, layoutSize.Height - FirstPaneMinSize);
      }

      var splitterBarHeight = splitterBar.DesiredSize.HeightInt;
      if (Collapsed && collapseAnim.IsOver)
      {
        displayFirstPane = true;
        displaySecondPane = false;

        layoutSize.Height = Math.Max(layoutSize.Width, splitterBarHeight + FirstPaneMinSize);

        var arr = new ArrangerVertical(layoutSize);
        arr.Reserve(splitterBarHeight);
        arr.Arrange(FirstPane, arr.AvailableHeight).Advance(arr.AvailableHeight);
        arr.Arrange(splitterBar, splitterBarHeight).AdvanceReserved(splitterBarHeight);
        return layoutSize;
      }

      displayFirstPane = true;
      displaySecondPane = true;

      layoutSize.Height = Math.Max(layoutSize.Height, FirstPaneMinSize + splitterBarHeight + SecondPaneMinSize);
      var animatedSplitterPos = (int)MathHelper.Clamp((1f - collapseAnim.CurrentValue) * SplitterOffset, 0, SplitterOffset);

      var arr2 = new ArrangerVertical(layoutSize);
      arr2.Reserve(animatedSplitterPos);
      arr2.Reserve(splitterBarHeight);
      arr2.Arrange(FirstPane, arr2.AvailableHeight).Advance(arr2.AvailableHeight);
      arr2.Arrange(splitterBar, splitterBarHeight).AdvanceReserved(splitterBarHeight);
      arr2.Arrange(SecondPane, SplitterOffset).AdvanceReserved(animatedSplitterPos);
      return layoutSize;
    }

    Rectangle ArrangeLeft(Rectangle layoutSize)
    {
      if (layoutSize.Width >= FirstPaneMinSize + SecondPaneMinSize)
      {
        SplitterOffset = MathHelper.Clamp(SplitterOffset, FirstPaneMinSize, layoutSize.Width - SecondPaneMinSize);
      }

      var splitterBarWidth = splitterBar.DesiredSize.WidthInt;
      if (Collapsed && collapseAnim.IsOver)
      {
        displayFirstPane = false;
        displaySecondPane = true;

        layoutSize.Width = Math.Max(layoutSize.Width, splitterBarWidth + SecondPaneMinSize);
        var arr = new ArrangerHorizontal(layoutSize);
        arr.Arrange(splitterBar, splitterBarWidth).Advance(splitterBarWidth);
        arr.Arrange(SecondPane, arr.AvailableWidth).Advance(arr.AvailableWidth);
        return layoutSize;
      }

      displayFirstPane = true;
      displaySecondPane = true;

      var layoutWidth2 = Math.Max(layoutSize.Width, FirstPaneMinSize + splitterBarWidth + SecondPaneMinSize);
      var animatedSplitterPos = (int)MathHelper.Clamp((1f - collapseAnim.CurrentValue) * SplitterOffset, 0, SplitterOffset);

      // FirstPane might be shrinking to zero while playing the collapse-animation. 
      // We therefore arrange it with its non-collapsed sizing. The remaining parts (splitter-bar, right component)
      // will be positioned and expanded according to the current status of the animation.
      // retain the original size to avoid display artefacts while the left pane is visually shrinking.
      // if the animation is over, we either have a fully collapsed pane (see above) or a fully expanded pane.
      var arr2 = new ArrangerHorizontal(layoutSize);
      arr2.Arrange(FirstPane, SplitterOffset).Advance(animatedSplitterPos);
      arr2.Arrange(splitterBar, splitterBarWidth).Advance(splitterBarWidth);
      arr2.Arrange(SecondPane, arr2.AvailableWidth).Advance(arr2.AvailableWidth);
      layoutSize.Width = layoutWidth2;
      return layoutSize;
    }

    Rectangle ArrangeRight(Rectangle layoutSize)
    {
      if (layoutSize.Width >= FirstPaneMinSize + SecondPaneMinSize)
      {
        SplitterOffset = MathHelper.Clamp(SplitterOffset, SecondPaneMinSize, layoutSize.Width - FirstPaneMinSize);
      }

      var splitterBarWidth = splitterBar.DesiredSize.WidthInt;
      if (Collapsed && collapseAnim.IsOver)
      {
        displayFirstPane = true;
        displaySecondPane = false;

        layoutSize.Width = Math.Max(layoutSize.Width, splitterBarWidth + FirstPaneMinSize);

        var arr = new ArrangerHorizontal(layoutSize);
        arr.Reserve(splitterBarWidth).Arrange(FirstPane, arr.AvailableWidth).Advance(arr.AvailableWidth);
        arr.Arrange(splitterBar, splitterBarWidth).AdvanceReserved(splitterBarWidth);
        return layoutSize;
      }

      displayFirstPane = true;
      displaySecondPane = true;

      layoutSize.Width = Math.Max(layoutSize.Width, FirstPaneMinSize + splitterBarWidth + SecondPaneMinSize);
      var animatedSplitterPos = (int)MathHelper.Clamp((1f - collapseAnim.CurrentValue) * SplitterOffset, 0, SplitterOffset);

      var arr2 = new ArrangerHorizontal(layoutSize);
      arr2.Reserve(animatedSplitterPos);
      arr2.Reserve(splitterBarWidth);
      arr2.Arrange(FirstPane, arr2.AvailableWidth).Advance(arr2.AvailableWidth);
      arr2.Arrange(splitterBar, splitterBarWidth).AdvanceReserved(splitterBarWidth);
      arr2.Arrange(SecondPane, SplitterOffset).AdvanceReserved(animatedSplitterPos);
      return layoutSize;
    }

    Rectangle ArrangeUp(Rectangle layoutSize)
    {
      if (layoutSize.Height >= FirstPaneMinSize + SecondPaneMinSize)
      {
        SplitterOffset = MathHelper.Clamp(SplitterOffset, FirstPaneMinSize, layoutSize.Height - SecondPaneMinSize);
      }

      var splitterBarHeight = splitterBar.DesiredSize.HeightInt;
      if (Collapsed && collapseAnim.IsOver)
      {
        displayFirstPane = false;
        displaySecondPane = true;

        layoutSize.Height = Math.Max(layoutSize.Height, splitterBarHeight + SecondPaneMinSize);
        var arr = new ArrangerVertical(layoutSize);
        arr.Arrange(splitterBar, splitterBarHeight).Advance(splitterBarHeight);
        arr.Arrange(SecondPane, arr.AvailableHeight).Advance(arr.AvailableHeight);
        return layoutSize;
      }

      displayFirstPane = true;
      displaySecondPane = true;

      layoutSize.Height = Math.Max(layoutSize.Height, FirstPaneMinSize + splitterBarHeight + SecondPaneMinSize);

      var animatedSplitterPos = (int)MathHelper.Clamp((1f - collapseAnim.CurrentValue) * SplitterOffset, 0, SplitterOffset);

      // retain the original size to avoid display artefacts while the pane is visually shrinking.
      // if the animation is over, we either have a fully collapsed pane (see above) or a fully expanded pane.
      var arr2 = new ArrangerVertical(layoutSize);
      arr2.Arrange(FirstPane, SplitterOffset).Advance(animatedSplitterPos);
      arr2.Arrange(splitterBar, splitterBarHeight).Advance(splitterBarHeight);
      arr2.Arrange(SecondPane, arr2.AvailableHeight).Advance(arr2.AvailableHeight);
      return layoutSize;
    }

    void OnMouseClick(object sender, MouseEventArgs args)
    {
      args.Consume();
      if (Collapsable && Resizable)
      {
        ToggleCollapse();
      }
    }

    void OnMouseDown(object sender, MouseEventArgs args)
    {
      if (args.Button != MouseButton.Left || !Resizable)
      {
        return;
      }

      args.Consume();
      IsDragging = true;
      var hitPoint = args.Position;
      switch (Direction)
      {
        case Direction.Left:
          dragOffset = SplitterOffset - hitPoint.X;
          break;
        case Direction.Right:
          dragOffset = SplitterOffset + hitPoint.X;
          break;
        case Direction.Up:
          dragOffset = SplitterOffset - hitPoint.Y;
          break;
        case Direction.Down:
          dragOffset = SplitterOffset + hitPoint.Y;
          break;
      }
    }

    void OnMouseDragged(object sender, MouseEventArgs args)
    {
      if (!Resizable)
      {
        IsDragging = false;
        return;
      }

      if (IsDragging)
      {
        args.Consume();
        var hitPoint = args.Position;
        switch (Direction)
        {
          case Direction.Left:
            SplitterOffset = dragOffset + hitPoint.X;
            break;
          case Direction.Right:
            SplitterOffset = dragOffset - hitPoint.X;
            break;
          case Direction.Up:
            SplitterOffset = dragOffset + hitPoint.Y;
            break;
          case Direction.Down:
            SplitterOffset = dragOffset - hitPoint.Y;
            break;
        }
      }
    }

    void OnMouseUp(object sender, MouseEventArgs args)
    {
      args.Consume();
      IsDragging = false;
    }
  }
}