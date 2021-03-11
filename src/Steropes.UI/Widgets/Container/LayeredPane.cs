using System;
using Microsoft.Xna.Framework;
using Steropes.UI.Components;

namespace Steropes.UI.Widgets.Container
{
  /// <summary>
  ///  A container that allows explicit control over the rendering order. 
  ///  This container behaves similar to Group as widgets added are laid
  ///  out using their Anchor property as layout constraint. However,
  ///  widgets are sorted by an layer position and are rendered in
  ///  that order.
  /// </summary>
  public class LayeredPane : WidgetContainerBase<int>, ILayeredWidget
  {
    public const int DefaultLayer = 0;

    public const int GlassPaneLayer = 200000;

    public const int PopupLayer = 300000;

    public const int WindowLayer = 100000;

    public LayeredPane(IUIStyle style) : base(style)
    {
    }

    public enum InsertPosition
    {
      Front,

      Back
    }

    public void Add(IWidget widget, InsertPosition pos = InsertPosition.Front)
    {
      AddInternalHelper(widget, pos, PopupLayer);
    }
    
    public void Remove(IWidget widget)
    {
      RemoveImpl(IndexOf(widget));
    }

    public void ToBack(IWidget widget)
    {
      ReOrder(widget, InsertPosition.Back);
    }

    public void ToFront(IWidget widget)
    {
      ReOrder(widget, InsertPosition.Front);
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
  }
}