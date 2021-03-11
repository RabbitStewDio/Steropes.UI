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

using Steropes.UI.Widgets.Container;

namespace Steropes.UI.Components.Window
{
  public sealed class RootPane : WidgetContainerBase<int>, IPopupLocationSource, IRootPane
  {
    const int DefaultLayer = 0;

    const int GlassPaneLayer = 200000;

    const int PopupLayer = 300000;

    const int WindowLayer = 100000;

    readonly GlassPane glassPane;

    bool glassPaneForPopups;

    int popUpCounts;

    IWidget rootPane;

    IPopUp topMostPopup;

    public RootPane(IScreenService screen, IUIStyle style) : base(style)
    {
      Screen = screen;
      glassPane = new GlassPane(UIStyle);

      AddInternalHelper(glassPane, InsertPosition.Front, GlassPaneLayer);
      GlassPaneForPopups = true;
    }

    public enum InsertPosition
    {
      Front,

      Back
    }

    public IWidget Content
    {
      get
      {
        return rootPane;
      }
      set
      {
        if (rootPane != null)
        {
          RemoveImpl(IndexOf(rootPane));
        }
        rootPane = value;
        if (rootPane != null)
        {
          AddInternalHelper(rootPane, InsertPosition.Front, DefaultLayer);
        }
      }
    }

    public bool GlassPaneForPopups
    {
      get
      {
        return glassPaneForPopups;
      }
      set
      {
        if (glassPaneForPopups == value)
        {
          return;
        }

        glassPaneForPopups = value;
        OnPropertyChanged();
        UpdateGlassPane();
        InvalidateLayout();
      }
    }

    public override IScreenService Screen { get; }

    public void AddPopUp(IPopUp popUp, InsertPosition pos = InsertPosition.Front)
    {
      AddInternalHelper(popUp, pos, PopupLayer);
    }

    public void AddWindow(IWindow window, InsertPosition pos = InsertPosition.Front)
    {
      AddInternalHelper(window, pos, WindowLayer);
    }

    public bool QueryPopUpLayoutRect(out IPopUp popup)
    {
      if (popUpCounts > 0)
      {
        if (topMostPopup != null)
        {
          if (topMostPopup.Visibility != Visibility.Visible)
          {
            topMostPopup = null;
          }
        }

        if (topMostPopup == null)
        {
          // Popups are added at the back of the list, so that they are drawn last.
          // we can use that to quickly find the last popup that is drawn.
          for (var idx = Count - 1; idx >= 0; idx -= 1)
          {
            var w = this[idx];
            if (w.Visibility != Visibility.Visible)
            {
              continue;
            }

            if (w is IPopUp)
            {
              topMostPopup = w as IPopUp;
              break;
            }
          }
        }

        if (topMostPopup != null)
        {
          popup = topMostPopup;
          return true;
        }
      }

      popup = default(IPopUp);
      return false;
    }

    public void Remove(IWindowBase window)
    {
      RemoveImpl(IndexOf(window));
    }

    public void ToBack(IWidget window)
    {
      ReOrder(window, InsertPosition.Back);
    }

    public void ToFront(IWidget window)
    {
      ReOrder(window, InsertPosition.Front);
    }

    protected override Rectangle ArrangeOverride(Rectangle layoutSize)
    {
      for (var i = 0; i < this.Count; i++)
      {
        var widget = this[i];
        if (widget.Visibility == Visibility.Collapsed)
        {
          continue;
        }

        var childRect = widget.ArrangeChild(layoutSize);
        widget.Arrange(childRect);
      }
      return layoutSize;
    }

    protected override Size MeasureOverride(Size availableSize)
    {
      var contentHeight = 0;
      var contentWidth = 0;
      for (var i = 0; i < this.Count; i++)
      {
        var widget = this[i];
        if (widget.Visibility == Visibility.Collapsed)
        {
          continue;
        }

        var size = widget.MeasureAsAnchoredChild(availableSize);

        contentHeight = (int)Math.Max(contentHeight, size.Height);
        contentWidth = (int)Math.Max(contentWidth, size.Width);
      }
      return new Size(contentWidth, contentHeight);
    }

    protected override void OnChildAdded(IWidget w, int index, int constraint)
    {
      base.OnChildAdded(w, index, constraint);
      if (w is IPopUp)
      {
        popUpCounts += 1;
      }

      UpdateGlassPane();
    }

    protected override void OnChildRemoved(IWidget w, int index, int constraint)
    {
      if (ReferenceEquals(w, rootPane))
      {
        rootPane = null;
      }
      if (w is IPopUp)
      {
        popUpCounts -= 1;
      }
      if (ReferenceEquals(w, topMostPopup))
      {
        topMostPopup = null;
      }

      UpdateGlassPane();
    }

    void AddInternalHelper(IWidget w, InsertPosition pos, int constraint)
    {
      AddImpl(w, GetStartOfLayer(constraint, pos), constraint);
    }

    int GetStartOfLayer(int constraint, InsertPosition insertPosition)
    {
      for (var idx = 0; idx < Count; idx += 1)
      {
        var layer = GetContraintAt(idx);
        switch (insertPosition)
        {
          case InsertPosition.Back:
            if (layer >= constraint)
            {
              return idx;
            }
            break;
          case InsertPosition.Front:
            if (layer > constraint)
            {
              return idx;
            }
            break;
          default:
            throw new ArgumentOutOfRangeException();
        }
      }
      return Count;
    }

    void ReOrder(IWidget w, InsertPosition pos)
    {
      var index = IndexOf(w);
      if (index == -1)
      {
        throw new ArgumentException(nameof(w));
      }

      var constraint = GetContraintAt(index);
      RemoveImpl(index);
      AddInternalHelper(w, pos, constraint);
    }

    void UpdateGlassPane()
    {
      if (popUpCounts > 0 && GlassPaneForPopups)
      {
        glassPane.Visibility = Visibility.Visible;
      }
      else
      {
        glassPane.Visibility = Visibility.Collapsed;
      }
    }
  }
}