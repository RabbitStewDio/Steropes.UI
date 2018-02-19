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
using Steropes.UI.Platform;

namespace Steropes.UI.Widgets
{
  public interface IContentWidget : IWidget
  {
    IWidget Content { get; set; }
  }

  public interface IContentWidget<TContent> : IContentWidget
    where TContent : class, IWidget
  {
    new TContent Content { get; set; }

    object Tag { get; set; }
  }

  public class InternalContentWidget<TContent> : Widget
    where TContent : class, IWidget
  {
    TContent internalContent;

    public InternalContentWidget(IUIStyle style) : base(style)
    {
    }

    public bool Clip { get; set; }

    public override int Count => InternalContent == null ? 0 : 1;

    protected TContent InternalContent
    {
      get
      {
        return internalContent;
      }
      set
      {
        if (ReferenceEquals(internalContent, value))
        {
          return;
        }
        var old = internalContent;
        if (old != null)
        {
          old.RemoveNotify(this);
          RaiseChildRemoved(0, old);
        }
        internalContent = value;
        if (internalContent != null)
        {
          internalContent.AddNotify(this);
          RaiseChildAdded(0, value);
        }

        OnContentUpdated();
        OnPropertyChanged();
        InvalidateLayout();
      }
    }

    public override IWidget this[int index]
    {
      get
      {
        if (index == 0 && InternalContent != null)
        {
          return InternalContent;
        }
        throw new IndexOutOfRangeException();
      }
    }

    public override void Update(GameTime elapsedTime)
    {
      base.Update(elapsedTime);
      for (var i = 0; i < this.Count; i++)
      {
        var widget = this[i];
        widget.Update(elapsedTime);
      }
    }

    protected override Rectangle ArrangeOverride(Rectangle layoutSize)
    {
      if (InternalContent != null)
      {
        var childRect = InternalContent.ArrangeChild(layoutSize);
        InternalContent.Arrange(childRect);
        return layoutSize;
      }
      return layoutSize;
    }

    protected override void DrawChildren(IBatchedDrawingService drawingService)
    {
      if (Clip)
      {
        drawingService.PushScissorRectangle(ContentRect);
        try
        {
          base.DrawChildren(drawingService);
        }
        finally
        {
          drawingService.PopScissorRectangle();
        }
      }
      else
      {
        base.DrawChildren(drawingService);
      }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
      if (InternalContent != null)
      {
        InternalContent.MeasureAsAnchoredChild(availableSize);
        return InternalContent.DesiredSize;
      }
      return new Size(0, 0);
    }

    protected virtual void OnContentUpdated()
    {
    }
  }

  public class ContentWidget<TContent> : InternalContentWidget<TContent>, IContentWidget
    where TContent : class, IWidget
  {
    public ContentWidget(IUIStyle style) : base(style)
    {
    }

    public TContent Content
    {
      get
      {
        return InternalContent;
      }
      set
      {
        InternalContent = value;
      }
    }

    IWidget IContentWidget.Content
    {
      get
      {
        return InternalContent;
      }
      set
      {
        var v = value as TContent;
        InternalContent = v ?? throw new ArgumentNullException(nameof(value));
      }
    }
    
    protected override void OnContentUpdated()
    {
      base.OnContentUpdated();
      OnPropertyChanged(nameof(Content));

    }
  }
}