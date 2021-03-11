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
using Microsoft.Xna.Framework.Graphics;

using Steropes.UI.Components;
using Steropes.UI.Platform;

namespace Steropes.UI.Widgets.Container
{
  public class SplitterBar : Widget
  {
    bool collapsable;

    bool collapsed;

    Direction direction;

    bool resizable;

    public SplitterBar(IUIStyle style) : base(style)
    {
    }

    public bool Collapsable
    {
      get
      {
        return collapsable;
      }
      set
      {
        collapsable = value;
        OnPropertyChanged();
        InvalidateLayout();
      }
    }

    public bool Collapsed
    {
      get
      {
        return collapsed;
      }
      set
      {
        collapsed = value;
        InvalidateLayout();
      }
    }

    public Direction Direction
    {
      get
      {
        return direction;
      }
      set
      {
        direction = value;
        InvalidateLayout();
      }
    }

    public bool Resizable
    {
      get
      {
        return resizable;
      }
      set
      {
        if (value == resizable)
        {
          return;
        }
        resizable = value;
        UpdateCursor();
        OnPropertyChanged();
      }
    }

    protected override Rectangle ArrangeOverride(Rectangle layoutSize)
    {
      var textureHeight = FrameOverlayTexture?.Height ?? 0;
      var textureWidth = FrameOverlayTexture?.Width ?? 0;
      switch (Direction)
      {
        case Direction.Up:
        case Direction.Down:
          {
            var width = Math.Max(textureWidth, layoutSize.Width);
            return new Rectangle(layoutSize.X, layoutSize.Y, width, textureHeight);
          }
        case Direction.Left:
        case Direction.Right:
          {
            // handle will be drawn rotated by 90 degrees, so width becomes height ...
            var height = Math.Max(textureHeight, layoutSize.Height);
            return new Rectangle(layoutSize.X, layoutSize.Y, textureWidth, height);
          }
        default:
          throw new ArgumentException();
      }
    }

    protected override void DrawFrameOverlay(IBatchedDrawingService drawingService)
    {
      var texture = FrameOverlayTexture;
      if (texture == null)
      {
        return;
      }

      float handleAngle;
      switch (Direction)
      {
        case Direction.Left:
          {
            handleAngle = Collapsed ? 0f : MathHelper.Pi;
            break;
          }
        case Direction.Right:
          {
            handleAngle = Collapsed ? MathHelper.Pi : 0f;
            break;
          }
        case Direction.Up:
          {
            handleAngle = Collapsed ? MathHelper.PiOver2 : 3f * MathHelper.PiOver2;
            break;
          }
        case Direction.Down:
          {
            handleAngle = Collapsed ? 3f * MathHelper.PiOver2 : MathHelper.PiOver2;
            break;
          }
        default:
          {
            throw new NotSupportedException();
          }
      }

      drawingService.Draw(
        texture,
        new Vector2(ContentRect.Center.X, ContentRect.Center.Y),
        null,
        FrameOverlayColor,
        handleAngle,
        new Vector2(texture.Width / 2f, texture.Height / 2f),
        1f,
        SpriteEffects.None,
        0f);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
      var textureHeight = FrameOverlayTexture?.Height ?? 0;
      var textureWidth = FrameOverlayTexture?.Width ?? 0;
      switch (Direction)
      {
        case Direction.Up:
        case Direction.Down:
          var width = Math.Max(textureWidth, float.IsInfinity(availableSize.Width) ? 0 : availableSize.Width);
          return new Size(width, textureHeight);
        case Direction.Left:
        case Direction.Right:

          // handle will be drawn rotated by 90 degrees, so width becomes height ...
          var height = Math.Max(textureHeight, float.IsInfinity(availableSize.Height) ? 0 : availableSize.Height);
          return new Size(textureWidth, height);
        default:
          throw new ArgumentException();
      }
    }

    void UpdateCursor()
    {
      if (!Enabled)
      {
        Cursor = MouseCursor.Default;
        return;
      }

      switch (Direction)
      {
        case Direction.Left:
        case Direction.Right:
          {
            Cursor = Collapsable ? MouseCursor.Hand : MouseCursor.SizeWE;
            break;
          }
        case Direction.Up:
        case Direction.Down:
          {
            Cursor = Collapsable ? MouseCursor.Hand : MouseCursor.SizeNS;
            break;
          }
      }
    }
  }
}