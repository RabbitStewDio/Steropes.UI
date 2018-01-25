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
using Steropes.UI.Platform;
using Steropes.UI.Styles;
using Steropes.UI.Util;
using Steropes.UI.Widgets.Container;
using Steropes.UI.Widgets.Styles;

namespace Steropes.UI.Widgets
{
  public class NotebookTabList : WidgetContainerBase<bool>
  {
    readonly NotebookStyleDefinition notebookStyle;
    readonly EventSupport<EventArgs> activeTabChangedSupport;

    INotebookTab activeTab;

    bool allowDragReorder;

    public NotebookTabList(IUIStyle style) : base(style)
    {
      activeTabChangedSupport = new EventSupport<EventArgs>();
      allowDragReorder = true;
      notebookStyle = StyleSystem.StylesFor<NotebookStyleDefinition>();
      DragState = new DragNDropState(this);
    }

    public event EventHandler<EventArgs> ActiveTabChanged
    {
      add { activeTabChangedSupport.Event += value; }
      remove { activeTabChangedSupport.Event -= value; }
    }
    
    public EventHandler<EventArgs> OnActiveTabChanged
    {
      get { return activeTabChangedSupport.Handler; }
      set { activeTabChangedSupport.Handler = value; }
    }

    public INotebookTab ActiveTab
    {
      get
      {
        return activeTab;
      }
      set
      {
        if (ReferenceEquals(value, activeTab))
        {
          return;
        }
        if (activeTab != null)
        {
          activeTab.IsActive = false;
        }
        activeTab = value;
        if (activeTab != null)
        {
          activeTab.IsActive = true;
        }
        OnPropertyChanged();
        activeTabChangedSupport.Raise(this, EventArgs.Empty);
      }
    }
    
    public bool AllowDragReorder
    {
      get
      {
        return allowDragReorder;
      }
      set
      {
        if (allowDragReorder != value)
        {
          allowDragReorder = value;
          OnPropertyChanged();
        }
      }
    }

    public int NotebookTabOverlapX
    {
      get
      {
        return Style.GetValue(notebookStyle.NotebookTabOverlapX);
      }
      set
      {
        Style.SetValue(notebookStyle.NotebookTabOverlapX, value);
      }
    }

    /// <summary>
    ///   Protected for test introspection.
    /// </summary>
    protected DragNDropState DragState { get; }

    public void ActivateTab(INotebookTab notebookTab)
    {
      if (IndexOf(notebookTab) != -1)
      {
        ActiveTab = notebookTab;
      }
    }

    public void Add(INotebookTab widget, bool pinned = false)
    {
      AddImpl(widget, Count, pinned);
    }

    public void Add(INotebookTab widget, int index, bool pinned = false)
    {
      AddImpl(widget, index, pinned);
    }

    public void CloseTab(INotebookTab notebookTab)
    {
      Remove(notebookTab);
    }

    public void Remove(INotebookTab widget)
    {
      var idx = IndexOf(widget);
      if (idx == -1)
      {
        throw new ArgumentException();
      }

      RemoveImpl(idx);
    }

    /// <summary>
    ///   Todo: Handle minimum sizing on tabs.
    ///   Todo: Handle scrolling (when more space needed by tabs than available).
    ///   Todo: Handle drag'n'drop.
    /// </summary>
    /// <param name="layoutSize"></param>
    /// <returns></returns>
    protected override Rectangle ArrangeOverride(Rectangle layoutSize)
    {
      var draggedTab = DragState.DraggedTab;
      var x = layoutSize.X;
      var dragHandled = false;
      for (var i = 0; i < this.Count; i++)
      {
        var tab = this[i];
        if (DragState.DragActive && ReferenceEquals(tab, draggedTab))
        {
          continue;
        }

        if (DragState.DragActive && draggedTab != null)
        {
          if (dragHandled == false)
          {
            var centerPoint = x + tab.DesiredSize.Width / 2;
            if (centerPoint > DragState.DraggedTabOffset)
            {
              x += draggedTab.DesiredSize.WidthInt - NotebookTabOverlapX;
              dragHandled = true;
            }
          }
        }

        tab.Arrange(new Rectangle(x, layoutSize.Bottom - tab.DesiredSize.HeightInt, tab.DesiredSize.WidthInt, tab.DesiredSize.HeightInt));
        x += tab.DesiredSize.WidthInt - NotebookTabOverlapX;
      }

      if (Count > 0)
      {
        // correct for last tab overlap area.
        x += NotebookTabOverlapX;
      }

      if (DragState.DragActive && draggedTab != null)
      {
        var dx = (int)MathHelper.Clamp(DragState.DraggedTabOffset, layoutSize.Left, x);
        var dy = layoutSize.Bottom - draggedTab.DesiredSize.HeightInt;
        draggedTab.Arrange(new Rectangle(dx, dy, draggedTab.DesiredSize.WidthInt, draggedTab.DesiredSize.HeightInt));
      }
      return layoutSize;
    }

    protected override void DrawChildren(IBatchedDrawingService drawingService)
    {
      for (var i = 0; i < this.Count; i++)
      {
        var child = this[i];
        if (ReferenceEquals(child, ActiveTab))
        {
          continue;
        }
        if (DragState.DragActive && ReferenceEquals(child, DragState.DraggedTab))
        {
          continue;
        }
        if (child.Visibility == Visibility.Visible)
        {
          child.Draw(drawingService);
        }
      }

      if (ActiveTab?.Visibility == Visibility.Visible)
      {
        ActiveTab.Draw(drawingService);
      }
      if (DragState.DragActive && DragState.DraggedTab != ActiveTab)
      {
        DragState.DraggedTab?.Draw(drawingService);
      }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
      float height = 0;
      float width = 0;
      for (var i = 0; i < Count; i++)
      {
        var tab = this[i];
        tab.Measure(availableSize);
        width += tab.DesiredSize.Width;
        height = Math.Max(height, tab.DesiredSize.Height);
      }
      width += NotebookTabOverlapX * Math.Max(0, Count - 1);
      return new Size(width, height);
    }

    protected override void OnChildAdded(IWidget w, int index, bool constraint)
    {
      base.OnChildAdded(w, index, constraint);
      DragState.Install(w);
      if (w is INotebookTab)
      {
        var tab = (INotebookTab)w;
        tab.CloseRequested += OnCloseRequested;
        tab.ActivationRequested += OnActivationRequested;
      }

      if (ActiveTab == null)
      {
        ActiveTab = (NotebookTab)w;
      }
    }

    protected override void OnChildRemoved(IWidget w, int index, bool constraint)
    {
      base.OnChildRemoved(w, index, constraint);
      DragState.Uninstall(w);
      if (w is INotebookTab)
      {
        var tab = (INotebookTab)w;
        tab.CloseRequested -= OnCloseRequested;
        tab.ActivationRequested -= OnActivationRequested;
      }

      if (Count > 0)
      {
        var nextIndex = Math.Min(Count - 1, index + 1);
        ActiveTab = (NotebookTab)this[nextIndex];
      }
      else
      {
        ActiveTab = null;
      }
    }

    void OnActivationRequested(object sender, EventArgs args)
    {
      var tab = sender as INotebookTab;
      if (tab == null)
      {
        return;
      }
      if (tab.Parent != this)
      {
        return;
      }
      ActivateTab(tab);
    }

    void OnCloseRequested(object sender, EventArgs args)
    {
      var tab = sender as INotebookTab;
      if (tab == null)
      {
        return;
      }
      if (tab.Parent != this)
      {
        return;
      }
      CloseTab(tab);
    }

    /// <summary>
    ///   Public to allow deep testing.
    /// </summary>
    public class DragNDropState
    {
      readonly NotebookTabList parent;

      public DragNDropState(NotebookTabList parent)
      {
        this.parent = parent;
      }

      public bool DragActive { get; private set; }

      public NotebookTab DraggedTab { get; private set; }

      public float DraggedTabEventOrigin { get; private set; }

      public float DraggedTabOffset { get; private set; }

      public float DraggedTabOffsetOrigin { get; private set; }

      public void Install(IWidget widget)
      {
        widget.MouseDown += OnMouseDown;
        widget.MouseDragged += OnMouseDragged;
        widget.MouseUp += OnMouseUp;
      }

      public void Uninstall(IWidget widget)
      {
        widget.MouseDown -= OnMouseDown;
        widget.MouseDragged -= OnMouseDragged;
        widget.MouseUp -= OnMouseUp;
      }

      int FindInsertIndex()
      {
        var retval = 0;
        for (var i = 0; i < parent.Count; i++)
        {
          var tab = parent[i];
          if (ReferenceEquals(tab, DraggedTab))
          {
            retval += 1;
            continue;
          }
          if (tab.LayoutRect.Center.X < DraggedTabOffset)
          {
            retval += 1;
            continue;
          }

          return retval;
        }
        return retval;
      }

      void OnMouseDown(object sender, MouseEventArgs args)
      {
        if (args.Button != MouseButton.Left || parent.AllowDragReorder == false)
        {
          return;
        }

        DraggedTab = sender as NotebookTab;
        if (DraggedTab != null)
        {
          DraggedTabOffsetOrigin = DraggedTab.LayoutRect.X;
          DraggedTabEventOrigin = args.Position.X;
          DraggedTabOffset = DraggedTab.LayoutRect.X;
        }
        args.Consume();
      }

      void OnMouseDragged(object sender, MouseEventArgs args)
      {
        if (DraggedTab != null)
        {
          var deltaX = args.Position.X - DraggedTabEventOrigin;
          if (Math.Abs(deltaX) > 5)
          {
            DragActive = true;
          }

          DraggedTabOffset = DraggedTabOffsetOrigin + deltaX;
          parent.InvalidateLayout();

          args.Consume();
        }
      }

      void OnMouseUp(object sender, MouseEventArgs args)
      {
        if (DraggedTab == null || DragActive == false)
        {
          return;
        }

        args.Consume();

        var idx = parent.IndexOf(DraggedTab);
        if (idx == -1)
        {
          DraggedTab = null;
          return;
        }
        var insertIdx = FindInsertIndex();
        if (insertIdx != idx)
        {
          var focused = parent.Screen?.FocusManager?.FocusedWidget;
          var activeTab = parent.ActiveTab;

          parent.Remove(DraggedTab);
          var newIndex = FindInsertIndex();
          parent.Add(DraggedTab, newIndex);

          if (focused != null)
          {
            parent.Screen.FocusManager.FocusedWidget = focused;
          }
          parent.ActiveTab = activeTab;
          parent.InvalidateLayout();
        }

        DragActive = false;
        DraggedTab = null;
        DraggedTabOffset = 0;
        DraggedTabOffsetOrigin = 0;
        DraggedTabEventOrigin = 0;
      }
    }
  }
}