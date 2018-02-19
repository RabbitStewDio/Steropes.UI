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
using System.Runtime.CompilerServices;

using Microsoft.Xna.Framework;

using Steropes.UI.Annotations;
using Steropes.UI.Platform;
using Steropes.UI.Util;
using Steropes.UI.Widgets;

namespace Steropes.UI.Components
{
  public interface IVisualContent
  {
    Size DesiredSize { get; }

    Rectangle LayoutRect { get; }

    Insets Margin { get; }

    Insets Padding { get; }

    void Arrange(Rectangle rect);

    void Draw(IBatchedDrawingService drawingService);

    void DrawOverlay(IBatchedDrawingService drawingService);

    void InvalidateLayout();

    void Measure(Size size);

    void Update(GameTime time);
  }

  public abstract class VisualContent : IVisualContent, INotifyPropertyChanged
  {
    Rectangle borderRect;

    Rectangle? lastLayoutArguments;

    Size? lastMeasureSize;

    bool layoutInvalid;

    EventSupport<EventArgs> layoutInvalidatedSupport;

    Rectangle layoutRect;

    Insets margin;

    Insets padding;

    EventSupport<EventArgs> parentChangedSupport;

    PropertyChangedEventSupport propertyChangedSupport;

    protected VisualContent()
    {
      layoutInvalidatedSupport = new EventSupport<EventArgs>();
      layoutInvalid = true;
    }

    public event EventHandler<EventArgs> LayoutInvalidated
    {
      add
      {
        if (layoutInvalidatedSupport == null)
        {
          layoutInvalidatedSupport = new EventSupport<EventArgs>();
        }
        layoutInvalidatedSupport.Event += value;
      }
      remove
      {
        if (layoutInvalidatedSupport == null)
        {
          return;
        }
        layoutInvalidatedSupport.Event -= value;
      }
    }

    public event EventHandler<EventArgs> ParentChanged
    {
      add
      {
        if (parentChangedSupport == null)
        {
          parentChangedSupport = new EventSupport<EventArgs>();
        }
        parentChangedSupport.Event += value;
      }
      remove
      {
        if (parentChangedSupport == null)
        {
          return;
        }
        parentChangedSupport.Event -= value;
      }
    }

    public event PropertyChangedEventHandler PropertyChanged
    {
      add
      {
        if (propertyChangedSupport == null)
        {
          propertyChangedSupport = new PropertyChangedEventSupport();
        }

        if (this is Slider)
        {
          Console.WriteLine();
        }
        propertyChangedSupport.Event += value;
      }
      remove
      {
        if (propertyChangedSupport == null)
        {
          return;
        }
        propertyChangedSupport.Event -= value;
      }
    }

    public Rectangle BorderRect => borderRect;

    public Rectangle ContentRect { get; set; }

    public Size DesiredSize { get; protected set; }

    public bool LayoutInvalid
    {
      get
      {
        return layoutInvalid;
      }
      private set
      {
        if (value)
        {
          layoutInvalid = true;
          layoutInvalidatedSupport.Raise(this, EventArgs.Empty);
        }
        else
        {
          layoutInvalid = false;
        }
      }
    }

    public Rectangle LayoutRect
    {
      get
      {
        if (IsArranging)
        {
          throw new InvalidOperationException();
        }
        if (LayoutInvalid && lastLayoutArguments.HasValue)
        {
          Arrange(lastLayoutArguments.Value);
        }
        return layoutRect;
      }
      protected set
      {
        layoutRect = value;
        borderRect = value.ReduceRectBy(Margin);
        ContentRect = borderRect.ReduceRectBy(Padding);
      }
    }

    public virtual Insets Margin
    {
      get
      {
        return margin;
      }
      set
      {
        if (margin != value)
        {
          margin = value;
          OnPropertyChanged();
          InvalidateLayout();
        }
      }
    }

    public virtual Insets Padding
    {
      get
      {
        return padding;
      }
      set
      {
        if (padding != value)
        {
          padding = value;
          OnPropertyChanged();
          InvalidateLayout();
        }
      }
    }

    protected bool IsArranging { get; set; }

    public static bool IsValidSize(float f)
    {
      if (float.IsNaN(f) || float.IsInfinity(f) || f < 0)
      {
        return false;
      }
      return true;
    }

    public void Arrange(Rectangle layoutSize)
    {
      if (!IsValidSize(layoutSize.Width) || !IsValidSize(layoutSize.Height))
      {
        throw new ArgumentOutOfRangeException();
      }
      if (float.IsInfinity(layoutSize.X) || float.IsNaN(layoutSize.X) || float.IsInfinity(layoutSize.Y) || float.IsNaN(layoutSize.Y))
      {
        throw new ArgumentOutOfRangeException();
      }

      if (IsArranging)
      {
        throw new StackOverflowException();
      }

      IsArranging = true;
      ValidateStyle();

      try
      {
        var s = new Size(layoutSize.Width, layoutSize.Height);
        if (!IsMeasureValid(s))
        {
          Measure(s);
        }

        var insets = Margin + Padding;
        var paddedLayoutSize = layoutSize.ReduceRectBy(insets);
        var finalLayout = ArrangeOverride(paddedLayoutSize);
        ArrangeOverlays(finalLayout);

        var finalFullLayoutSize = finalLayout.IncreaseRectBy(insets);

        LayoutInvalid = false;
        LayoutRect = finalFullLayoutSize;
        lastLayoutArguments = layoutSize;
      }
      finally
      {
        IsArranging = false;
      }
    }

    public abstract void Draw(IBatchedDrawingService drawingService);

    public abstract void DrawOverlay(IBatchedDrawingService drawingService);

    public void InvalidateLayout()
    {
      if (!LayoutInvalid)
      {
        LayoutInvalid = true;
        lastMeasureSize = null;
        OnLayoutInvalidated();
      }
    }

    public void Measure(Size availableSize)
    {
      ValidateStyle();
      var insets = Margin + Padding;

      var availableWidth = float.IsPositiveInfinity(availableSize.Width) ? availableSize.Width : Math.Max(0, availableSize.Width - insets.Horizontal);
      var availableHeight = float.IsPositiveInfinity(availableSize.Height) ? availableSize.Height : Math.Max(0, availableSize.Height - insets.Vertical);
      var size = MeasureOverride(new Size(availableWidth, availableHeight));
      if (!IsValidSize(size.Width) || !IsValidSize(size.Height))
      {
        throw new Exception("MaxValue is not a valid return value for Measure");
      }

      var sizeWithPaddings = new Size(size.Width + insets.Horizontal, size.Height + insets.Vertical);
      MarkMeasureValid(availableSize);
      DesiredSize = sizeWithPaddings;
    }

    public virtual void Update(GameTime time)
    {
    }

    public virtual void ValidateStyle()
    {
    }

    protected virtual void ArrangeOverlays(Rectangle rect)
    {
    }

    protected abstract Rectangle ArrangeOverride(Rectangle layoutSize);

    protected Rectangle LastValidLayout()
    {
      return layoutRect;
    }

    protected abstract Size MeasureOverride(Size availableSize);

    protected virtual void OnLayoutInvalidated()
    {
    }

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      propertyChangedSupport?.Raise(this, propertyName);
    }

    protected void RaiseParentChangedEvent()
    {
      parentChangedSupport?.Raise(this, EventArgs.Empty);
    }

    bool IsMeasureValid(Size s)
    {
      if (lastMeasureSize == null)
      {
        return false;
      }
      return s == lastMeasureSize.Value;
    }

    void MarkMeasureValid(Size s)
    {
      lastMeasureSize = s;
    }
  }
}