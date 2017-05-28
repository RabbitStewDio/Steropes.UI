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
  public enum DockPanelConstraint
  {
    Top = 0,

    Left = 1,

    Right = 3,

    Bottom = 4
  }

  /// <summary>
  ///  A dock-panel allows to attach child widgets to one of the edges of the dock-panel box. The last widget can be stretched to fill any remaining space.
  ///  If the widgets contained in the dock-panel are larger than the available layout space for the dock-panel, the panel will overflow.
  /// </summary>
  public class DockPanel : WidgetContainer<DockPanelConstraint>
  {
    struct WidgetConstraintAndPosition
    {
      public int Position { get; }

      public IWidget Widget { get; }

      public DockPanelConstraint Constraint { get; }

      public WidgetConstraintAndPosition(int position, IWidget widget, DockPanelConstraint constraint)
      {
        Position = position;
        Widget = widget;
        Constraint = constraint;
      }
    }

    readonly WidgetSorter comparator;

    readonly List<WidgetConstraintAndPosition> sortedWidgets;

    bool lastChildFill;

    public DockPanel(IUIStyle style, bool lastChildFill) : this(style)
    {
      LastChildFill = lastChildFill;
    }

    public DockPanel(IUIStyle style) : base(style)
    {
      sortedWidgets = new List<WidgetConstraintAndPosition>();
      comparator = new WidgetSorter(this);
    }

    public bool LastChildFill
    {
      get
      {
        return lastChildFill;
      }
      set
      {
        lastChildFill = value;
        OnPropertyChanged();
        InvalidateLayout();
      }
    }

    protected override Rectangle ArrangeOverride(Rectangle layoutSize)
    {
      var lastWidget = LastChildFill && Count > 0 ? FindLastVisibleWidget() : null;

      if (sortedWidgets.Count != Count)
      {
        // Sort order is "Top, Left, Right, Bottom".
        sortedWidgets.Clear();
        var list = WidgetsWithConstraints();
        for (int idx = 0; idx < list.Count; idx += 1)
        {
          var widget = list[idx];
          sortedWidgets.Add(new WidgetConstraintAndPosition(idx, widget.Widget, widget.Constraint));
        }
        sortedWidgets.Sort(comparator);
      }

      var centerRect = ComputeCenterWidget(layoutSize);
      var xOffset = layoutSize.X;
      var yOffset = layoutSize.Y;
      var lastConstraint = DockPanelConstraint.Top;
      for (var index = 0; index < sortedWidgets.Count; index++)
      {
        var widgetAndConstraint = sortedWidgets[index];
        var widget = widgetAndConstraint.Widget;
        if (widget == null)
        {
          continue;
        }
        if (widget.Visibility == Visibility.Collapsed)
        {
          continue;
        }

        if (ReferenceEquals(widget, lastWidget))
        {
          widget.Arrange(widget.ArrangeChild(centerRect));
        }
        else
        {
          switch (widgetAndConstraint.Constraint)
          {
            case DockPanelConstraint.Top:
              {
                widget.Arrange(widget.ArrangeChild(new Rectangle(layoutSize.X, yOffset, layoutSize.Width, widget.DesiredSize.HeightInt)));
                yOffset += widget.DesiredSize.HeightInt;
                break;
              }
            case DockPanelConstraint.Bottom:
              {
                if (lastConstraint != DockPanelConstraint.Bottom)
                {
                  yOffset += centerRect.Height;
                }
                widget.Arrange(widget.ArrangeChild(new Rectangle(layoutSize.X, yOffset, layoutSize.Width, widget.DesiredSize.HeightInt)));
                yOffset += widget.DesiredSize.HeightInt;
                break;
              }
            case DockPanelConstraint.Left:
              {
                widget.Arrange(widget.ArrangeChild(new Rectangle(xOffset, yOffset, widget.DesiredSize.WidthInt, centerRect.Height)));
                xOffset += widget.DesiredSize.WidthInt;
                break;
              }
            case DockPanelConstraint.Right:
              {
                if (lastConstraint != DockPanelConstraint.Right)
                {
                  xOffset += centerRect.Width;
                }
                widget.Arrange(widget.ArrangeChild(new Rectangle(xOffset, yOffset, widget.DesiredSize.WidthInt, centerRect.Height)));
                xOffset += widget.DesiredSize.WidthInt;
                break;
              }
            default:
              {
                throw new ArgumentOutOfRangeException();
              }
          }
          lastConstraint = widgetAndConstraint.Constraint;
        }
      }

      return layoutSize;
    }

    protected Rectangle ComputeCenterWidget(Rectangle layoutSize)
    {
      var lastWidget = FindLastVisibleWidget();

      var usedHeight = 0;
      var usedWidth = 0;
      var centerOffsetX = 0;
      var centerOffsetY = 0;
      var centerWidth = 0;
      var centerHeight = 0;

      for (var index = 0; index < sortedWidgets.Count; index++)
      {
        var widgetAndConstraint = sortedWidgets[index];
        var widget = widgetAndConstraint.Widget;
        if (widget == null)
        {
          continue;
        }
        if (widget.Visibility == Visibility.Collapsed)
        {
          continue;
        }

        if (ReferenceEquals(widget, lastWidget))
        {
          centerWidth = widget.DesiredSize.WidthInt;
          centerHeight = widget.DesiredSize.HeightInt;
        }
        else
        {
          switch (widgetAndConstraint.Constraint)
          {
            case DockPanelConstraint.Top:
              {
                usedHeight += widget.DesiredSize.HeightInt;
                centerOffsetY = usedHeight;
                break;
              }
            case DockPanelConstraint.Bottom:
              {
                usedHeight += widget.DesiredSize.HeightInt;
                break;
              }
            case DockPanelConstraint.Left:
              {
                usedWidth += widget.DesiredSize.WidthInt;
                centerOffsetX = usedWidth;
                break;
              }
            case DockPanelConstraint.Right:
              {
                usedWidth += widget.DesiredSize.WidthInt;
                break;
              }
            default:
              {
                throw new ArgumentOutOfRangeException();
              }
          }
        }
      }
      centerWidth = Math.Max(centerWidth, layoutSize.Width - usedWidth);
      centerHeight = Math.Max(centerHeight, layoutSize.Height - usedHeight);

      return new Rectangle(layoutSize.X + centerOffsetX, layoutSize.Y + centerOffsetY, centerWidth, centerHeight);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
      float horizontalWidgetWidth = 0;
      float horizontalWidgetHeight = 0;
      float verticalWidgetWidth = 0;
      float verticalWidgetHeight = 0;

      var widgetsAndConstraints = WidgetsWithConstraints();
      for (var index = 0; index < widgetsAndConstraints.Count; index++)
      {
        var widgetAndConstraint = widgetsAndConstraints[index];
        var widget = widgetAndConstraint.Widget;
        if (widget == null)
        {
          continue;
        }
        if (widget.Visibility == Visibility.Collapsed)
        {
          continue;
        }

        switch (widgetAndConstraint.Constraint)
        {
          case DockPanelConstraint.Top:
          case DockPanelConstraint.Bottom:
            {
              widget.Measure(new Size(availableSize.Width, float.PositiveInfinity));
              horizontalWidgetWidth = Math.Max(horizontalWidgetWidth, widget.DesiredSize.Width);
              horizontalWidgetHeight += widget.DesiredSize.Height;
              break;
            }
          case DockPanelConstraint.Left:
          case DockPanelConstraint.Right:
            {
              widget.Measure(new Size(float.PositiveInfinity, availableSize.Height));
              verticalWidgetWidth += widget.DesiredSize.Width;
              verticalWidgetHeight = Math.Max(verticalWidgetHeight, widget.DesiredSize.Height);
              break;
            }
          default:
            {
              throw new ArgumentOutOfRangeException();
            }
        }
      }

      return new Size(Math.Max(verticalWidgetWidth, horizontalWidgetWidth), verticalWidgetHeight + horizontalWidgetHeight);
    }

    protected override void OnChildAdded(IWidget w, int index, DockPanelConstraint constraint)
    {
      sortedWidgets.Clear();
      base.OnChildAdded(w, index, constraint);
    }

    protected override void OnChildRemoved(IWidget w, int index, DockPanelConstraint constraint)
    {
      sortedWidgets.Clear();
      base.OnChildRemoved(w, index, constraint);
    }

    IWidget FindLastVisibleWidget()
    {
      if (LastChildFill == false)
      {
        return null;
      }
      for (var i = Count - 1; i >= 0; i--)
      {
        var widget = this[i];
        if (widget.Visibility != Visibility.Collapsed)
        {
          return widget;
        }
      }
      return null;
    }

    class WidgetSorter : IComparer<WidgetConstraintAndPosition>
    {
      readonly DockPanel parent;

      public WidgetSorter(DockPanel parent)
      {
        this.parent = parent;
      }

      public int Compare(WidgetConstraintAndPosition x, WidgetConstraintAndPosition y)
      {
        // this is safe as this code can only be called if there are two or more elements in the panel.
        var lastWidget = parent.FindLastVisibleWidget();
        if (ReferenceEquals(x.Widget, lastWidget))
        {
          return 1;
        }
        if (ReferenceEquals(y.Widget, lastWidget))
        {
          return -1;
        }
        int order = x.Constraint.CompareTo(y.Constraint);
        if (order != 0)
        {
          return order;
        }
        return x.Position.CompareTo(y.Position);
      }
    }
  }
}