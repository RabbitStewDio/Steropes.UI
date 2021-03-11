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
using Steropes.UI.Styles;
using Steropes.UI.Widgets.Styles;

namespace Steropes.UI.Widgets
{
  public enum ScaleMode
  {
    None = 0,
    KeepAspectRatio = 1,
    Scale = 2
  }

  public class Image : Widget
  {
    readonly ImageStyleDefinition imageStyle;

    public Image(IUIStyle style) : base(style)
    {
      imageStyle = StyleSystem.StylesFor<ImageStyleDefinition>();
    }

    public bool HasTexture => Texture != null && Texture.Width > 0 && Texture.Height > 0;

    public ScaleMode Stretch
    {
      get { return Style.GetValue(imageStyle.TextureScale); }
      set
      {
        Style.SetValue(imageStyle.TextureScale, value);
        InvalidateLayout();
      }
    }

    public IUITexture Texture
    {
      get { return Style.GetValue(imageStyle.Texture); }
      set
      {
        Style.SetValue(imageStyle.Texture, value);
        InvalidateLayout();
      }
    }

    public Color TextureColor
    {
      get { return Style.GetValue(imageStyle.TextureColor, Color); }
      set { Style.SetValue(imageStyle.TextureColor, value); }
    }

    protected override Rectangle ArrangeOverride(Rectangle layoutSize)
    {
      return CalculateImageSize(layoutSize);
    }

    protected Rectangle CalculateImageSize(Rectangle layoutSize)
    {
      switch (Stretch)
      {
        case ScaleMode.Scale:
        {
          return layoutSize;
        }
        case ScaleMode.None:
        {
          var center = layoutSize.Center;
          return new Rectangle(center.X - DesiredSize.WidthInt / 2, center.Y - DesiredSize.HeightInt / 2,
            DesiredSize.WidthInt, DesiredSize.HeightInt);
        }
        case ScaleMode.KeepAspectRatio:
        {
          var center = layoutSize.Center;
          if (DesiredSize.HeightInt == 0 || DesiredSize.WidthInt == 0)
          {
            return new Rectangle(center.X - DesiredSize.WidthInt / 2, center.Y - DesiredSize.HeightInt / 2,
              DesiredSize.WidthInt, DesiredSize.HeightInt);
          }

          var scale = Math.Min(DesiredSize.Width / layoutSize.Width, DesiredSize.Height / layoutSize.Height);
          var width = DesiredSize.Width * scale;
          var height = DesiredSize.Height * scale;
          return new Rectangle((int) Math.Round(center.X - width / 2), (int) Math.Round(center.Y - height / 2),
            (int) width, (int) height);
        }
        default:
          throw new ArgumentException();
      }
    }

    protected override void DrawWidget(IBatchedDrawingService drawingService)
    {
      var texture = Texture;
      if (texture == null)
      {
        return;
      }

      var targetRect = CalculateImageSize(ContentRect);
      drawingService.DrawImage(texture, targetRect, TextureColor);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
      var texture = Texture;
      var width = texture?.Width ?? 0;
      var height = texture?.Height ?? 0;
      return new Size(width, height);
    }
  }
}