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
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Steropes.UI.Components;

namespace Steropes.UI.Widgets.Container
{
  /// BoxGroup allows packing widgets in one direction.  It takes care of positioning each child widget properly
  /// 
  /// Widgets are laid out the direction specified by Orientation (Horizontal or vertical). Widgets can be marked as Expanded, 
  /// which will add any extra space on to that widget (in addition to their own space requirements).
  public class BoxGroup : WidgetContainer<bool>
  {
    Orientation orientation;

    int spacing;

    public BoxGroup(IUIStyle style) : base(style)
    {
      Orientation = Orientation.Vertical;
    }

    public BoxGroup(IUIStyle style, Orientation orientation, int spacing) : this(style)
    {
      Orientation = orientation;
      Spacing = spacing;
    }

    public Orientation Orientation
    {
      get
      {
        return orientation;
      }
      set
      {
        orientation = value;
        InvalidateLayout();
        OnPropertyChanged();
      }
    }

    public int Spacing
    {
      get
      {
        return spacing;
      }
      set
      {
        spacing = value;
        InvalidateLayout();
        OnPropertyChanged();
      }
    }

    // todo
    public override IWidget GetFirstFocusableDescendant(Direction direction)
    {
      if (Count == 0)
      {
        return null;
      }

      return this[0].GetFirstFocusableDescendant(direction);
    }

    // todo: Allow for enabled and visible
    public override IWidget GetSibling(Direction direction, IWidget sourceWidget)
    {
      var index = IndexOf(sourceWidget);

      if (Orientation == Orientation.Horizontal)
      {
        if (direction == Direction.Right)
        {
          if (index < Count - 1)
          {
            return this[index + 1];
          }
        }
        else if (direction == Direction.Left)
        {
          if (index > 0)
          {
            return this[index - 1];
          }
        }
      }
      else
      {
        if (direction == Direction.Down)
        {
          if (index < Count - 1)
          {
            return this[index + 1];
          }
        }
        else if (direction == Direction.Up)
        {
          if (index > 0)
          {
            return this[index - 1];
          }
        }
      }

      return base.GetSibling(direction, this);
    }

    protected override Rectangle ArrangeOverride(Rectangle layoutSize)
    {
      if (Count == 0)
      {
        return new Rectangle(layoutSize.X, layoutSize.Y, 0, 0);
      }

      var fixedChildrenSizes = MeasureFixedChildrenSize();

      var width = layoutSize.Width;
      var height = layoutSize.Height;
      var primaryAxisSize = Orientation == Orientation.Horizontal ? width : height;
      var secondAxisSize = Orientation == Orientation.Horizontal ? height : width;
      AdjustSizesForDynamicWidgets(primaryAxisSize, secondAxisSize, fixedChildrenSizes);

      var cellStart = new Point(layoutSize.X, layoutSize.Y);
      var usedSpace = 0;
      for (var index = 0; index < Count; index++)
      {
        var widget = this[index];

        if (widget.Visibility == Visibility.Collapsed)
        {
          continue;
        }

        var widgetSize = fixedChildrenSizes[index];

        var widgetRect = new Rectangle(cellStart.X, cellStart.Y, widgetSize.WidthInt, widgetSize.HeightInt);
        var widgetLayout = widget.ArrangeChild(widgetRect);
        widget.Arrange(widgetLayout);

        switch (Orientation)
        {
          case Orientation.Horizontal:
            usedSpace = Math.Max(usedSpace, widgetRect.Right);
            cellStart.X = widgetRect.Right + Spacing;
            break;
          case Orientation.Vertical:
            usedSpace = Math.Max(usedSpace, widgetRect.Bottom);
            cellStart.Y = widgetRect.Bottom + Spacing;
            break;
          default:
            throw new ArgumentException();
        }
      }

      if (Orientation == Orientation.Horizontal)
      {
        return new Rectangle(layoutSize.X, layoutSize.Y, usedSpace - layoutSize.X, layoutSize.Height);
      }
      return new Rectangle(layoutSize.X, layoutSize.Y, layoutSize.Width, usedSpace - layoutSize.Y);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
      if (Orientation == Orientation.Horizontal)
      {
        var contentWidth = 0;
        var contentHeight = 0;
        for (var i = 0; i < this.Count; i++)
        {
          var child = this[i];
          if (child.Visibility == Visibility.Collapsed)
          {
            continue;
          }
          var size = child.MeasureAsAnchoredChild(availableSize);
          contentWidth += size.WidthInt;
          contentHeight = Math.Max(contentHeight, size.HeightInt);
        }

        if (Count > 1)
        {
          contentWidth += Spacing * (Count - 1);
        }
        return new Size(contentWidth, contentHeight);
      }
      else
      {
        var contentWidth = 0;
        var contentHeight = 0;
        for (var i = 0; i < this.Count; i++)
        {
          var child = this[i];
          if (child.Visibility == Visibility.Collapsed)
          {
            continue;
          }

          var size = child.MeasureAsAnchoredChild(availableSize);

          contentHeight += size.HeightInt;
          contentWidth = Math.Max(contentWidth, size.WidthInt);
        }

        if (Count > 1)
        {
          contentHeight += Spacing * (Count - 1);
        }
        return new Size(contentWidth, contentHeight);
      }
    }

    void AdjustSizesForDynamicWidgets(int actualSize, int secondaryAxis, List<Size> fixedChildrenSizes)
    {
      var fixedChildrenSize = (Count - 1) * Spacing + FixedChildrenSize(fixedChildrenSizes);
      var extraSpaceTotal = actualSize - fixedChildrenSize;

      var dynamicChildrenCount = CountVisibleDynamicHeightChildren();
      var extraSpacePerWidget = (int)Math.Floor(extraSpaceTotal / (float)dynamicChildrenCount);

      var dynamicChildrenProcessed = 0;
      for (var index = 0; index < Count; index++)
      {
        if (this[index].Visibility == Visibility.Collapsed)
        {
          continue;
        }

        var sh = fixedChildrenSizes[index];
        switch (Orientation)
        {
          case Orientation.Horizontal:
            sh.Height = secondaryAxis;
            break;
          case Orientation.Vertical:
            sh.Width = secondaryAxis;
            break;
          default:
            throw new ArgumentException();
        }

        if (GetContraintAt(index))
        {
          dynamicChildrenProcessed += 1;
          int extraSpaceForThisWidget;
          if (dynamicChildrenProcessed == dynamicChildrenCount)
          {
            // avoid artefacts caused by rounding errors. The last widget simply is a bit larger than the others ..
            extraSpaceForThisWidget = extraSpaceTotal - (dynamicChildrenCount - 1) * extraSpacePerWidget;
          }
          else
          {
            extraSpaceForThisWidget = extraSpacePerWidget;
          }

          switch (Orientation)
          {
            case Orientation.Horizontal:
              sh.Width += extraSpaceForThisWidget;
              break;
            case Orientation.Vertical:
              sh.Height += extraSpaceForThisWidget;
              break;
            default:
              throw new ArgumentException();
          }
        }

        fixedChildrenSizes[index] = sh;
      }
    }

    int CountVisibleDynamicHeightChildren()
    {
      var count = 0;
      var wc = WidgetsWithConstraints;
      for (var i = 0; i < wc.Count; i++)
      {
        var c = wc[i];
        if (c.Constraint && c.Widget.Visibility != Visibility.Collapsed)
        {
          count += 1;
        }
      }
      return count;
    }

    int FixedChildrenSize(List<Size> sizes)
    {
      var sum = 0;
      for (var i = 0; i < sizes.Count; i++)
      {
        sum += Orientation == Orientation.Horizontal ? sizes[i].WidthInt : sizes[i].HeightInt;
      }
      return sum;
    }

    List<Size> MeasureFixedChildrenSize()
    {
      var fixedChildrenSizes = new List<Size>();
      for (var i = 0; i < this.Count; i++)
      {
        var child = this[i];
        if (child.Visibility == Visibility.Collapsed)
        {
          fixedChildrenSizes.Add(new Size());
          continue;
        }

        var anchoredSize = child.Anchor.ResolveSize(child.DesiredSize);
        fixedChildrenSizes.Add(anchoredSize);
      }

      return fixedChildrenSizes;
    }
  }
}