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
using System.Diagnostics;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Steropes.UI.Util;

namespace Steropes.UI.Platform
{
  public interface IBatchedDrawingService
  {
    Rectangle Bounds { get; }

    GraphicsDevice GraphicsDevice { get; }

    IUITexture WhitePixel { get; set; }

    void Draw(IUITexture texture,
              Rectangle destinationRectangle,
              Rectangle? sourceRectangle,
              Color color,
              float rotation,
              Vector2 origin,
              SpriteEffects effects,
              float layerDepth);

    void DrawBlurredText(IUIFont uiFont,
                         string label,
                         Vector2 position,
                         Color color,
                         float blurRadius,
                         Color blurColor,
                         Vector2 origin = new Vector2(),
                         float scale = 1f);

    void DrawBlurredText(IUIFont uiFont,
                         StringBuilder label,
                         Vector2 position,
                         Color color,
                         float blurRadius,
                         Color blurColor,
                         Vector2 origin = new Vector2(),
                         float scale = 1f);

    void DrawImage(IUITexture image, Rectangle position, Color tint);

    void DrawLine(Vector2 from, Vector2 to, Color color, float width = 1f);

    void DrawRect(Rectangle rect, Color color, float width = 1);

    /// <summary>
    ///   Draws the given text at its baseline offset.
    /// </summary>
    /// <param name="spriteFont"></param>
    /// <param name="text"></param>
    /// <param name="pos"></param>
    /// <param name="color"></param>
    void DrawString(IUIFont spriteFont, string text, Vector2 pos, Color color);

    void DrawString(IUIFont spriteFont, StringBuilder text, Vector2 pos, Color color);

    void EndDrawing();

    void FillRect(Rectangle bounds, Color color);

    bool IsVisible(Rectangle rectangle);

    void PopScissorRectangle();

    void PushScissorRectangle(Rectangle scissorRect);

    void ResumeBatch();

    void StartDrawing();

    SpriteBatch SuspendBatch();
  }

  public class BatchedDrawingService : IBatchedDrawingService
  {
    readonly Stack<Rectangle> scissorRects = new Stack<Rectangle>();
    readonly UITexture whitePixelTex;

    IUITexture whitePixel;

    public BatchedDrawingService(Game game): this(game.GraphicsDevice)
    {
    }

    public BatchedDrawingService(GraphicsDevice graphicsDevice)
    {
      GraphicsDevice = graphicsDevice ?? throw new ArgumentNullException(nameof(graphicsDevice));

      whitePixelTex = new UITexture(new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color));
      whitePixelTex.Texture.SetData(new[] { Color.White });

      SpriteBatch = new SpriteBatch(GraphicsDevice);

      ScissorRasterizerState = new RasterizerState { ScissorTestEnable = true };

      SpriteMatrix = Matrix.Identity;
    }

    public Rectangle ScissorRectangle
    {
      get { return scissorRects.Count > 0 ? scissorRects.Peek() : GraphicsDevice.Viewport.Bounds; }
    }

    public Matrix SpriteMatrix { get; }

    bool DrawingInProgress { get; set; }

    protected RasterizerState ScissorRasterizerState { get; }

    SpriteBatch SpriteBatch { get; }

    public IUITexture WhitePixel
    {
      get { return whitePixel ?? whitePixelTex; }
      set { whitePixel = value; }
    }

    public Rectangle Bounds
    {
      get { return GraphicsDevice.Viewport.Bounds; }
    }

    public GraphicsDevice GraphicsDevice { get; }

    public void Draw(IUITexture texture,
                     Rectangle destinationRectangle,
                     Rectangle? sourceRectangle,
                     Color color,
                     float rotation,
                     Vector2 origin,
                     SpriteEffects effects,
                     float layerDepth)
    {
      if (texture.Texture != null)
      {
        SpriteBatch.Draw(texture.Texture, destinationRectangle, sourceRectangle, color, rotation, origin, effects,
                         layerDepth);
      }
    }

    public void DrawBlurredText(IUIFont uiFont,
                                string label,
                                Vector2 position,
                                Color color,
                                float blurRadius,
                                Color blurColor,
                                Vector2 origin = new Vector2(),
                                float scale = 1f)
    {
      position.Y -= uiFont.Baseline;
      var font = uiFont.SpriteFont;
      if (blurRadius > 0f)
      {
        var effectiveBlurColor = blurColor * 0.1f * (color.A / 255f);

        SpriteBatch.DrawString(font, label, new Vector2(position.X - blurRadius, position.Y - blurRadius),
                               effectiveBlurColor, 0f, origin, scale, SpriteEffects.None, 0f);
        SpriteBatch.DrawString(font, label, new Vector2(position.X + blurRadius, position.Y - blurRadius),
                               effectiveBlurColor, 0f, origin, scale, SpriteEffects.None, 0f);
        SpriteBatch.DrawString(font, label, new Vector2(position.X + blurRadius, position.Y + blurRadius),
                               effectiveBlurColor, 0f, origin, scale, SpriteEffects.None, 0f);
        SpriteBatch.DrawString(font, label, new Vector2(position.X - blurRadius, position.Y + blurRadius),
                               effectiveBlurColor, 0f, origin, scale, SpriteEffects.None, 0f);
      }

      SpriteBatch.DrawString(font, label, new Vector2(position.X, position.Y), color, 0f, origin, scale,
                             SpriteEffects.None, 0f);
    }

    public void DrawBlurredText(IUIFont uiFont,
                                StringBuilder label,
                                Vector2 position,
                                Color color,
                                float blurRadius,
                                Color blurColor,
                                Vector2 origin,
                                float scale)
    {
      position.Y -= uiFont.Baseline;
      var font = uiFont.SpriteFont;
      if (blurRadius > 0f)
      {
        var effectiveBlurColor = blurColor * 0.1f * (color.A / 255f);

        SpriteBatch.DrawString(font, label, new Vector2(position.X - blurRadius, position.Y - blurRadius),
                               effectiveBlurColor, 0f, origin, scale, SpriteEffects.None, 0f);
        SpriteBatch.DrawString(font, label, new Vector2(position.X + blurRadius, position.Y - blurRadius),
                               effectiveBlurColor, 0f, origin, scale, SpriteEffects.None, 0f);
        SpriteBatch.DrawString(font, label, new Vector2(position.X + blurRadius, position.Y + blurRadius),
                               effectiveBlurColor, 0f, origin, scale, SpriteEffects.None, 0f);
        SpriteBatch.DrawString(font, label, new Vector2(position.X - blurRadius, position.Y + blurRadius),
                               effectiveBlurColor, 0f, origin, scale, SpriteEffects.None, 0f);
      }

      SpriteBatch.DrawString(font, label, new Vector2(position.X, position.Y), color, 0f, origin, scale,
                             SpriteEffects.None, 0f);
    }

    public void DrawImage(IUITexture image, Rectangle position, Color tint)
    {
      if (image.Texture != null)
      {
        SpriteBatch.Draw(image.Texture, position, image.Bounds, tint);
      }
    }

    public void DrawLine(Vector2 from, Vector2 to, Color color, float width = 1f)
    {
      var wp = WhitePixel;
      if (wp?.Texture != null)
      {
        var angle = (float)Math.Atan2(to.Y - from.Y, to.X - from.X);
        var length = Vector2.Distance(from, to);

        SpriteBatch.Draw(wp.Texture, (from + to) / 2f, null, color, angle, new Vector2(0.5f),
                         new Vector2(length + width, width), SpriteEffects.None, 0);
      }
    }

    public void DrawRect(Rectangle rect, Color color, float width = 1)
    {
      DrawRect(new Vector2(rect.Left, rect.Top), new Vector2(rect.Right, rect.Bottom), color, width);
    }

    public void DrawString(IUIFont spriteFont, string text, Vector2 pos, Color color)
    {
      var posF = new Vector2(pos.X, pos.Y - spriteFont.Baseline);
      SpriteBatch.DrawString(spriteFont.SpriteFont, text, posF, color);
    }

    public void DrawString(IUIFont spriteFont, StringBuilder text, Vector2 pos, Color color)
    {
      var posF = new Vector2(pos.X, pos.Y - spriteFont.Baseline);
      SpriteBatch.DrawString(spriteFont.SpriteFont, text, posF, color);
    }

    public void EndDrawing()
    {
      if (!DrawingInProgress)
      {
        throw new InvalidOperationException();
      }

      SpriteBatch.End();
      DrawingInProgress = false;
      ValidateBalancedScissorOperations();
      EndDrawingOverride();
    }

    public virtual void EndDrawingOverride()
    {

    }

    public void FillRect(Rectangle bounds, Color color)
    {
      Draw(WhitePixel, bounds, color);
    }

    public bool IsVisible(Rectangle rectangle)
    {
      return rectangle.Intersects(ScissorRectangle);
    }

    public void PopScissorRectangle()
    {
      SuspendBatch();
      scissorRects.Pop();
      ResumeBatch();
    }

    public void PushScissorRectangle(Rectangle scissorRect)
    {
      var rect = TransformRect(scissorRect, SpriteMatrix);
      rect.Offset(0, GraphicsDevice.Viewport.Y);
      var parentRect = ScissorRectangle;

      var newRect = parentRect.Clip(rect).Clip(GraphicsDevice.Viewport.Bounds);

      SuspendBatch();
      scissorRects.Push(newRect);
      ResumeBatch();
    }

    public void ResumeBatch()
    {

      if (scissorRects.Count > 0)
      {
        var rect = scissorRects.Peek();
        if (rect.Width > 0 && rect.Height > 0)
        {
          GraphicsDevice.ScissorRectangle = rect;
        }

        StartSpriteBatch();
      }
      else
      {
        StartSpriteBatch();
      }
    }

    public void StartDrawing()
    {
      if (DrawingInProgress)
      {
        throw new InvalidOperationException();
      }

      DrawingInProgress = true;
      StartSpriteBatch();
    }

    protected virtual void StartSpriteBatch()
    {
      SpriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointWrap, null, ScissorRasterizerState, null, SpriteMatrix);
    }

    public SpriteBatch SuspendBatch()
    {
      SpriteBatch.End();
      GraphicsDevice.RasterizerState = RasterizerState.CullNone;
      return SpriteBatch;
    }

    void Draw(IUITexture texture, Rectangle destinationRectangle, Color color)
    {
      SpriteBatch.Draw(texture.Texture, destinationRectangle, color);
    }

    void DrawRect(Vector2 from, Vector2 to, Color color, float width = 1)
    {
      if (from.X > to.X)
      {
        var x = from.X;
        from.X = to.X;
        to.X = x;
      }

      if (from.Y > to.Y)
      {
        var y = from.Y;
        from.Y = to.Y;
        to.Y = y;
      }

      DrawLine(from + new Vector2(width / 2f, 0f), new Vector2(to.X - width, from.Y), color, width);
      DrawLine(new Vector2(from.X + width / 2f, to.Y), to - new Vector2(width, 0f), color, width);

      DrawLine(from, new Vector2(from.X, to.Y), color, width);
      DrawLine(new Vector2(to.X, from.Y), to, color, width);
    }

    Rectangle TransformRect(Rectangle rect, Matrix matrix)
    {
      var origin = new Vector2(rect.X, rect.Y);
      var max = new Vector2(rect.Right, rect.Bottom);
      origin = Vector2.Transform(origin, matrix);
      max = Vector2.Transform(max, matrix);
      var bounds = new Rectangle((int) origin.X, (int) origin.Y, (int) (max.X - origin.X), (int) (max.Y - origin.Y));

      return bounds;
    }

    void ValidateBalancedScissorOperations()
    {
      Debug.Assert(scissorRects.Count == 0, "Unbalanced calls to PushScissorRectangles");
    }
  }
}