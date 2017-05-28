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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Steropes.UI.Annotations;

namespace Steropes.UI.Platform
{
  public static class BatchedDrawingServiceExtensions
  {
    public static void Draw(
      [NotNull] this IBatchedDrawingService ds,
      [CanBeNull] IUITexture texture,
      Vector2 position,
      Rectangle? sourceRectangle,
      Color color,
      float rotation,
      Vector2 origin,
      float scale,
      SpriteEffects effects,
      float layerDepth)
    {
      if (texture == null)
      {
        return;
      }

      var rect = new Rectangle((int)position.X, (int)position.Y, (int)(texture.Width * scale), (int)(texture.Height * scale));
      ds.Draw(texture, rect, sourceRectangle, color, rotation, origin, effects, layerDepth);
    }

    public static void DrawBox([NotNull] this IBatchedDrawingService ds, [CanBeNull] IBoxTexture tex, Rectangle layoutArea, Color color)
    {
      if (tex == null)
      {
        return;
      }

      var boxArea = layoutArea.ReduceRectBy(tex.Margins);

      var corners = tex.CornerArea;
      if (corners.Top > 0)
      {
        if (corners.Left > 0)
        {
          DrawImageFromAtlas(ds, tex, new Rectangle(boxArea.Left, boxArea.Top, corners.Top, corners.Left), new Rectangle(0, 0, corners.Top, corners.Left), color);
        }

        if (corners.Right > 0)
        {
          DrawImageFromAtlas(
            ds,
            tex,
            new Rectangle(boxArea.Right - corners.Right, boxArea.Top, corners.Right, corners.Top),
            new Rectangle(tex.Width - corners.Right, 0, corners.Right, corners.Top),
            color);
        }

        DrawImageFromAtlas(
          ds,
          tex,
          new Rectangle(boxArea.Left + corners.Left, boxArea.Top, boxArea.Width - corners.Horizontal, corners.Top),
          new Rectangle(corners.Left, 0, tex.Width - corners.Horizontal, corners.Top),
          color);
      }
      if (corners.Bottom > 0)
      {
        if (corners.Left > 0)
        {
          DrawImageFromAtlas(
            ds,
            tex,
            new Rectangle(boxArea.Left, boxArea.Bottom - corners.Bottom, corners.Left, corners.Bottom),
            new Rectangle(0, tex.Height - corners.Bottom, corners.Left, corners.Bottom),
            color);
        }
        if (corners.Right > 0)
        {
          DrawImageFromAtlas(
            ds,
            tex,
            new Rectangle(boxArea.Right - corners.Right, boxArea.Bottom - corners.Bottom, corners.Right, corners.Bottom),
            new Rectangle(tex.Width - corners.Right, tex.Height - corners.Bottom, corners.Right, corners.Bottom),
            color);
        }

        DrawImageFromAtlas(
          ds,
          tex,
          new Rectangle(boxArea.Left + corners.Left, boxArea.Bottom - corners.Bottom, boxArea.Width - corners.Horizontal, corners.Bottom),
          new Rectangle(corners.Left, tex.Height - corners.Bottom, tex.Width - corners.Horizontal, corners.Bottom),
          color);
      }

      if (corners.Left > 0)
      {
        DrawImageFromAtlas(
          ds,
          tex,
          new Rectangle(boxArea.Left, boxArea.Top + corners.Top, corners.Left, boxArea.Height - corners.Vertical),
          new Rectangle(0, corners.Left, corners.Top, tex.Height - corners.Vertical),
          color);
      }
      if (corners.Right > 0)
      {
        DrawImageFromAtlas(
          ds,
          tex,
          new Rectangle(boxArea.Right - corners.Right, boxArea.Top + corners.Top, corners.Right, boxArea.Height - corners.Vertical),
          new Rectangle(tex.Width - corners.Right, corners.Top, corners.Right, tex.Height - corners.Vertical),
          color);
      }

      // Content
      DrawImageFromAtlas(
        ds,
        tex,
        new Rectangle(boxArea.Left + corners.Left, boxArea.Top + corners.Top, boxArea.Width - corners.Horizontal, boxArea.Height - corners.Vertical),
        new Rectangle(corners.Left, corners.Top, tex.Width - corners.Horizontal, tex.Height - corners.Vertical),
        color);
    }

    public static void DrawImage(this IBatchedDrawingService ds, IUITexture image, Vector2 position, Color tint)
    {
      ds.DrawImage(image, new Rectangle((int)position.X, (int)position.Y, image.Width, image.Height), tint);
    }

    public static void DrawImageFromAtlas(this IBatchedDrawingService ds, IUITexture image, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color)
    {
      ds.Draw(image, destinationRectangle, sourceRectangle, color, 0.0f, Vector2.Zero, SpriteEffects.None, 0.0f);
    }
  }
}