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
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Xna.Framework;
using Steropes.UI.Components;
using Steropes.UI.Util;

namespace Steropes.UI.Widgets.Container
{
  /// <summary>
  ///   Grid allows packing widgets in a grid. It takes care of positioning each child widget properly
  /// </summary>
  public class Grid : WidgetContainer<Point>
  {
    readonly Dictionary<Point, List<IWidget>> widgetsPerPosition;

    int? columns;

    int[] columnWidths;

    int[] rowHeights;

    int? rows;
    int spacing;

    // ----------------------------------------------------------------------
    public Grid(IUIStyle style) : base(style)
    {
      widgetsPerPosition = new Dictionary<Point, List<IWidget>>();

      RowConstraints = new ObservableCollection<LengthConstraint>();
      RowConstraints.CollectionChanged += (o, e) => InvalidateLayout();
      ColumnConstraints = new ObservableCollection<LengthConstraint>();
      ColumnConstraints.CollectionChanged += (o, e) => InvalidateLayout();
    }

    public ObservableCollection<LengthConstraint> ColumnConstraints { get; }

    public IReadOnlyCollection<LengthConstraint> Columns
    {
      get { return ColumnConstraints.ToArray(); }
      set
      {
        ColumnConstraints.Clear();
        ColumnConstraints.AddRange(value);
      }
    }

    public ObservableCollection<LengthConstraint> RowConstraints { get; }

    public IReadOnlyCollection<LengthConstraint> Rows
    {
      get { return RowConstraints.ToArray(); }
      set
      {
        RowConstraints.Clear();
        RowConstraints.AddRange(value);
      }
    }

    /// <summary>
    ///  DSL helper property. 
    /// </summary>
    public IWidget[][] Children
    {
      set
      {
        Clear();
        if (value == null)
        {
          return;
        }

        var rows = value.Length;
        for (int rowIdx = 0; rowIdx < rows; rowIdx += 1)
        {
          var colArray = value[rowIdx];
          if (colArray == null)
          {
            continue;
          }

          var cols = colArray.Length;
          for (int colIdx = 0; colIdx < cols; colIdx += 1)
          {
            var widget = colArray[colIdx];
            if (widget != null)
            {
              Add(widget, new Point(colIdx, rowIdx));
            }
          }
        }
      }
    }

    public int Spacing
    {
      get { return spacing; }
      set
      {
        if (value == spacing)
        {
          return;
        }

        spacing = value;
        OnPropertyChanged();
        InvalidateLayout();
      }
    }

    int ColumnCount
    {
      get
      {
        if (columns.HasValue)
        {
          return columns.Value;
        }

        if (widgetsPerPosition.Count == 0)
        {
          return 0;
        }

        var cols = widgetsPerPosition.Keys.Max(e => e.X) + 1;
        columns = cols;
        return cols;
      }
    }

    int RowCount
    {
      get
      {
        if (rows.HasValue)
        {
          return rows.Value;
        }

        if (widgetsPerPosition.Count == 0)
        {
          return 0;
        }

        var rowsCalc = widgetsPerPosition.Keys.Max(e => e.Y) + 1;
        rows = rowsCalc;
        return rowsCalc;
      }
    }

    public void AddChildAt(IWidget w, int column, int row)
    {
      Add(w, new Point(column, row));
    }

    // todo:
    public override IWidget GetFirstFocusableDescendant(Direction direction)
    {
      switch (direction)
      {
        case Direction.Left:
          for (var colIndex = ColumnCount - 1; colIndex >= 0; colIndex--)
          {
            for (var rowIndex = 0; rowIndex < RowCount; rowIndex++)
            {
              var widget = LookupForFocusing(rowIndex, colIndex);
              var focusableWidget = widget?.GetFirstFocusableDescendant(direction);
              if (focusableWidget != null)
              {
                return focusableWidget;
              }
            }
          }

          break;
        case Direction.Right:
          for (var colIndex = 0; colIndex < ColumnCount; colIndex++)
          {
            for (var rowIndex = 0; rowIndex < RowCount; rowIndex++)
            {
              var widget = LookupForFocusing(rowIndex, colIndex);
              var focusableWidget = widget?.GetFirstFocusableDescendant(direction);
              if (focusableWidget != null)
              {
                return focusableWidget;
              }
            }
          }

          break;
        case Direction.Up:
          for (var rowIndex = RowCount - 1; rowIndex >= 0; rowIndex--)
          {
            for (var colIndex = 0; colIndex < ColumnCount; colIndex++)
            {
              var widget = LookupForFocusing(rowIndex, colIndex);
              var focusableWidget = widget?.GetFirstFocusableDescendant(direction);
              if (focusableWidget != null)
              {
                return focusableWidget;
              }
            }
          }

          break;
        case Direction.Down:
          for (var rowIndex = 0; rowIndex < RowCount; rowIndex++)
          {
            for (var colIndex = 0; colIndex < ColumnCount; colIndex++)
            {
              var widget = LookupForFocusing(rowIndex, colIndex);
              var focusableWidget = widget?.GetFirstFocusableDescendant(direction);
              if (focusableWidget != null)
              {
                return focusableWidget;
              }
            }
          }

          break;
      }

      return null;
    }

    public override IWidget GetSibling(Direction direction, IWidget sourceWidget)
    {
      if (sourceWidget.Parent != this)
      {
        return null;
      }

      IWidget tileChild = null;
      var childLocation = GetContraintFor(sourceWidget);
      var offset = 0;

      do
      {
        offset++;

        switch (direction)
        {
          case Direction.Left:
            if (childLocation.X - offset < 0)
            {
              return base.GetSibling(direction, sourceWidget);
            }

            tileChild = LookupForFocusing(childLocation.Y, childLocation.X - offset);
            break;
          case Direction.Right:
            if (childLocation.X + offset >= ColumnCount)
            {
              return base.GetSibling(direction, sourceWidget);
            }

            tileChild = LookupForFocusing(childLocation.Y, childLocation.X + offset);
            break;
          case Direction.Up:
            if (childLocation.Y - offset < 0)
            {
              return base.GetSibling(direction, sourceWidget);
            }

            tileChild = LookupForFocusing(childLocation.Y - offset, childLocation.X);
            break;
          case Direction.Down:
            if (childLocation.Y + offset >= RowCount)
            {
              return base.GetSibling(direction, sourceWidget);
            }

            tileChild = LookupForFocusing(childLocation.Y + offset, childLocation.X);
            break;
        }
      } while (tileChild == null);

      return tileChild;
    }

    protected override Rectangle ArrangeOverride(Rectangle layoutSize)
    {
      if (layoutSize.Width != DesiredSize.WidthInt ||
          layoutSize.Height != DesiredSize.HeightInt)
      {
        MeasureOverride(new Size(layoutSize.Width, layoutSize.Height));
      }

      var colStarts = RunningSum(columnWidths, layoutSize.X, Spacing);
      var rowStarts = RunningSum(rowHeights, layoutSize.Y, Spacing);

      var maxX = 0;
      var maxY = 0;
      var widgetsAndConstraints = WidgetsWithConstraints;
      for (var index = 0; index < widgetsAndConstraints.Count; index++)
      {
        var pair = widgetsAndConstraints[index];
        var widget = pair.Widget;
        var location = pair.Constraint;
        var rect = new Rectangle(colStarts[location.X], rowStarts[location.Y], columnWidths[location.X],
                                 rowHeights[location.Y]);
        var widgetLayout = widget.ArrangeChild(rect);
        widget.Arrange(widgetLayout);

        maxX = Math.Max(rect.Right, maxX);
        maxY = Math.Max(rect.Bottom, maxY);
      }

      maxX = Math.Max(0, maxX - layoutSize.X);
      maxY = Math.Max(0, maxY - layoutSize.Y);
      return new Rectangle(layoutSize.X, layoutSize.Y, maxX, maxY);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
      columnWidths = ComputeSpaceConstraints(ColumnConstraints, availableSize.Width, false, columnWidths);
      rowHeights = ComputeSpaceConstraints(RowConstraints, availableSize.Height, true, rowHeights);

      var widgetsAndConstraints = WidgetsWithConstraints;
      for (var index = 0; index < widgetsAndConstraints.Count; index++)
      {
        var pair = widgetsAndConstraints[index];
        var widget = pair.Widget;
        var location = pair.Constraint;
        var size = new Size(columnWidths[location.X], rowHeights[location.Y]);
        widget.MeasureAsAnchoredChild(size);
      }

      var effectiveSpacingX = Math.Max(columnWidths.Length - 1, 0) * Spacing;
      var effectiveSpacingY = Math.Max(rowHeights.Length - 1, 0) * Spacing;
      return new Size(columnWidths.Sum() + effectiveSpacingX, rowHeights.Sum() + effectiveSpacingY);
    }

    protected override void OnChildAdded(IWidget child, int index, Point constraint)
    {
      List<IWidget> widgets;
      if (widgetsPerPosition.TryGetValue(constraint, out widgets))
      {
        widgets.Add(child);
      }
      else
      {
        widgets = new List<IWidget>(1);
        widgets.Add(child);
        widgetsPerPosition[constraint] = widgets;
      }

      var row = constraint.Y;
      var column = constraint.X;

      if (row >= RowCount)
      {
        rows = null;
      }

      if (column >= ColumnCount)
      {
        columns = null;
      }

      while (RowConstraints.Count <= row)
      {
        RowConstraints.Add(LengthConstraint.Auto);
      }

      while (ColumnConstraints.Count <= column)
      {
        ColumnConstraints.Add(LengthConstraint.Auto);
      }
    }

    protected override void OnChildRemoved(IWidget widget, int index, Point constraint)
    {
      List<IWidget> widgets;
      if (widgetsPerPosition.TryGetValue(constraint, out widgets))
      {
        widgets.Remove(widget);
        if (widgets.Count == 0)
        {
          widgetsPerPosition.Remove(constraint);
        }

        rows = null;
        columns = null;
      }
    }

    /// <summary>
    ///   Computes the final width or height of a column based on the given constraints.
    /// </summary>
    /// <param name="availableSpace"></param>
    /// <param name="constraints"></param>
    /// <param name="rows"></param>
    /// <param name="availableRowHeights"></param>
    /// <returns></returns>
    int[] ComputeSpaceConstraints(IReadOnlyList<LengthConstraint> constraints,
                                  float availableSpace,
                                  bool rows,
                                  int[] availableRowHeights)
    {
      float usedHeight = 0;
      var treatRelativeAsAuto = float.IsInfinity(availableSpace);

      if (availableRowHeights == null || availableRowHeights.Length < constraints.Count)
      {
        availableRowHeights = new int[constraints.Count];
      }
      else
      {
        for (var i = 0; i < availableRowHeights.Length; i++)
        {
          availableRowHeights[i] = 0;
        }
      }

      // Step 1: Reserve space for the the absolute size constraints. This space is always allocated.
      // Allocate the minimum size space needed for auto-columns.
      // float autoWeighting = 0;
      float relativeSizes = 0;
      for (var rowIndex = 0; rowIndex < constraints.Count; rowIndex++)
      {
        var rc = constraints[rowIndex];
        if (rc.Unit == LengthConstraint.UnitType.Absolute)
        {
          availableRowHeights[rowIndex] = (int) rc.Value;
          usedHeight = rc.Value;
        }
        else if (rc.Unit == LengthConstraint.UnitType.Auto || treatRelativeAsAuto)
        {
          // autoWeighting += rc.Value;
          var rowIndexTmp = rowIndex;
          if (rows)
          {
            var s = MeasureAutoSizeRow(rowIndexTmp);
            availableRowHeights[rowIndex] = (int) s.Height;
            usedHeight = s.Height;
          }
          else
          {
            var s = MeasureAutoSizeCol(rowIndexTmp);
            availableRowHeights[rowIndex] = (int) s.Width;
            usedHeight = s.Width;
          }
        }
        else
        {
          relativeSizes += rc.Value;
        }
      }

      if (treatRelativeAsAuto)
      {
        return availableRowHeights;
      }

      var extraSpace = Math.Max(0, availableSpace - usedHeight);
      if (extraSpace > 0 && relativeSizes > 0)
      {
        for (var rowIndex = 0; rowIndex < constraints.Count; rowIndex++)
        {
          var rc = constraints[rowIndex];
          if (rc.Unit != LengthConstraint.UnitType.Relative)
          {
            continue;
          }

          if (rc.Value <= 0 || float.IsInfinity(rc.Value) || float.IsNaN(rc.Value))
          {
            availableRowHeights[rowIndex] = 0;
          }
          else
          {
            var pct = rc.Value / relativeSizes;
            availableRowHeights[rowIndex] = (int) (pct * extraSpace);
          }
        }
      }

      return availableRowHeights;
    }

    IWidget LookupForFocusing(int row, int col)
    {
      var p = new Point(col, row);
      List<IWidget> widgets;
      if (widgetsPerPosition.TryGetValue(p, out widgets))
      {
        for (var index = 0; index < widgets.Count; index++)
        {
          var widget = widgets[index];
          if (widget.Visibility == Visibility.Collapsed)
          {
            continue;
          }

          return widget;
        }
      }

      return null;
    }

    Size MeasureAutoSizeRow(int row)
    {
      var usedSize = new Size();
      var size = new Size(float.PositiveInfinity, float.PositiveInfinity);

      var widgetsWithConstraints = WidgetsWithConstraints;
      for (var index = 0; index < widgetsWithConstraints.Count; index++)
      {
        var pair = widgetsWithConstraints[index];
        if (pair.Constraint.Y != row)
        {
          continue;
        }

        var widget = pair.Widget;
        widget.MeasureAsAnchoredChild(size);
        usedSize.Width = Math.Max(usedSize.Width, widget.DesiredSize.Width);
        usedSize.Height = Math.Max(usedSize.Height, widget.DesiredSize.Height);
      }

      return usedSize;
    }

    Size MeasureAutoSizeCol(int col)
    {
      var usedSize = new Size();
      var size = new Size(float.PositiveInfinity, float.PositiveInfinity);

      var widgetsWithConstraints = WidgetsWithConstraints;
      for (var index = 0; index < widgetsWithConstraints.Count; index++)
      {
        var pair = widgetsWithConstraints[index];
        if (pair.Constraint.X != col)
        {
          continue;
        }

        var widget = pair.Widget;
        widget.MeasureAsAnchoredChild(size);
        usedSize.Width = Math.Max(usedSize.Width, widget.DesiredSize.Width);
        usedSize.Height = Math.Max(usedSize.Height, widget.DesiredSize.Height);
      }

      return usedSize;
    }

    int[] RunningSum(int[] array, int start, int padding)
    {
      var colStarts = new int[array.Length];
      var sum = start;
      for (var i = 0; i < array.Length; i++)
      {
        colStarts[i] = sum;
        sum += array[i];
        sum += padding;
      }

      return colStarts;
    }
  }
}