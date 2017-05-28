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

namespace Steropes.UI.Widgets
{
  /// <summary>
  ///   A container widget that contains another widget and allows the content to scroll.
  /// </summary>
  public class ScrollPanel<T> : ContentWidget<T>, IScrollControl
    where T : class, IWidget
  {
    public ScrollPanel(IUIStyle style) : base(style)
    {
      Scrollbar = new Scrollbar(style) { ScrollbarMode = ScrollbarMode.Auto };
      Scrollbar.AddNotify(this);
      RaiseChildrenChanged(null, Scrollbar);
      Clip = true;

      MouseClicked += OnMouseClick;
      MouseUp += OnMouseUp;
      MouseDown += OnMouseDown;
      MouseWheel += OnMouseWheel;
      MouseDragged += OnMouseDragged;
    }

    public override int Count => Content == null ? 1 : 2;

    public ScrollbarMode VerticalScrollbarMode
    {
      get
      {
        return Scrollbar.ScrollbarMode;
      }
      set
      {
        Scrollbar.ScrollbarMode = value;
      }
    }

    protected IScrollable Scrollable { get; private set; }

    public Scrollbar Scrollbar { get; }

    public override IWidget this[int index]
    {
      get
      {
        if (index == 0)
        {
          return Scrollbar;
        }
        if (index == 1 && Content != null)
        {
          return Content;
        }
        throw new IndexOutOfRangeException();
      }
    }

    public void ScrollTo(Rectangle visibleBox)
    {
      var contentRect = ContentRect;
      if (visibleBox.Top < contentRect.Top)
      {
        var delta = visibleBox.Top - contentRect.Top;
        Scrollbar.Scroll(delta);
      }
      else if (visibleBox.Bottom > contentRect.Bottom)
      {
        var delta = visibleBox.Bottom - contentRect.Bottom;
        Scrollbar.Scroll(delta);
      }
    }

    protected override Rectangle ArrangeOverride(Rectangle layoutSize)
    {
      var scrollbarWidth = Scrollbar.DesiredSize.WidthInt;

      if (Content != null)
      {
        var consumedSize = Content.MeasureAsAnchoredChild(new Size(layoutSize.Width, layoutSize.Height));
        Scrollbar.ScrollContentHeight = consumedSize.HeightInt;

        Rectangle childRectangle;
        if (VerticalScrollbarMode == ScrollbarMode.Always || 
            (VerticalScrollbarMode == ScrollbarMode.Auto && consumedSize.Height > layoutSize.Height))
        {
          Scrollbar.Measure(new Size(scrollbarWidth, layoutSize.Height));
          var contentHeight = Math.Max(Scrollbar.DesiredSize.HeightInt, consumedSize.HeightInt);

          childRectangle = new Rectangle(layoutSize.X, layoutSize.Y - (int)Scrollbar.LerpOffset, layoutSize.Width - scrollbarWidth, contentHeight);
        }
        else
        {
          childRectangle = layoutSize;
        }

        var childLayoutRect = Content.ArrangeChild(childRectangle);
        Content.Arrange(childLayoutRect);
      }

      Rectangle scrollbarBounds = new Rectangle(layoutSize.Right - scrollbarWidth, layoutSize.Y, scrollbarWidth, layoutSize.Height);
      Scrollbar.Arrange(scrollbarBounds);
      return layoutSize;
    }

    protected override Size MeasureOverride(Size availableSize)
    {
      var consumedSize = Content?.MeasureAsAnchoredChild(availableSize) ?? new Size();
      Scrollbar.ScrollContentHeight = consumedSize.HeightInt;

      if (VerticalScrollbarMode == ScrollbarMode.None)
      {
        return consumedSize;
      }

      //// Gather the scrollbar's minimum size. A scrollbar will consume whatever height is given, even if the actual content does not need that much space.
      //// We need to ensure that the panel requests at mimumum enough space to allow the scrollbar to show up.
      Scrollbar.Measure(Size.Auto);
      consumedSize.Width += Scrollbar.DesiredSize.Width;
      consumedSize.Height = Math.Max(consumedSize.Height, Scrollbar.DesiredSize.Height);
      return consumedSize;
    }

    protected override void OnContentUpdated()
    {
      base.OnContentUpdated();
      if (Content is IScrollable)
      {
        Scrollable = (IScrollable)Content;
      }
      else
      {
        Scrollable = new DefaultScrollable(Content);
      }
    }

    void OnMouseClick(object sender, MouseEventArgs args)
    {
      Scrollbar.DispatchEvent(args);
    }

    void OnMouseDown(object sender, MouseEventArgs args)
    {
      Scrollbar.DispatchEvent(args);
    }

    void OnMouseDragged(object sender, MouseEventArgs args)
    {
      Scrollbar.DispatchEvent(args);
    }

    void OnMouseUp(object sender, MouseEventArgs args)
    {
      Scrollbar.DispatchEvent(args);
    }

    void OnMouseWheel(object sender, MouseEventArgs args)
    {
      Scrollbar.DispatchEvent(args);
    }
  }

  public class ScrollPanel : ScrollPanel<IWidget>
  {
    public ScrollPanel(IUIStyle style) : base(style)
    {
    }
  }
}