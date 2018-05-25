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
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Steropes.UI.Platform;

namespace Steropes.UI.Styles
{
  public interface IFontFactory
  {
    IUIFont LoadFont(string name);
  }

  /// <summary>
  ///   At runtime, we do not have access to actual font metrics. To properly align text when we combine
  ///   multiple fonts at different heights, we need a measure of the font's baseline. The baseline is
  ///   the offset where normal characters are printed in real font-layout systems.
  /// </summary>
  public class FontFactory : IFontFactory
  {
    readonly TraceSource tracer;
    readonly ContentManager contentManager;

    readonly GraphicsDevice graphicsDevice;

    public FontFactory(ContentManager contentManager, GraphicsDevice graphicsDevice)
    {
      this.contentManager = contentManager;
      this.graphicsDevice = graphicsDevice;
      this.tracer = TracingUtil.StyleTracing;
    }

    public IUIFont LoadFont(string name)
    {
      var spriteFont = contentManager.Load<SpriteFont>(name);
      var baseLine = ComputeAutoComputeBaseLine(spriteFont, name);
      return new UIFont(spriteFont, baseLine, name);
    }

    int Analyze(Color[] colors, int width)
    {
      var maxLines = colors.Length / width;
      for (var line = maxLines - 1; line >= 0; line -= 1)
      {
        float nonEmptyCount = 0;
        for (var x = 0; x < width; x += 1)
        {
          var c = colors[line * width + x];

          // we only need to analyze one channel, as all of them contain the same information anyway.
          if (c.R < 96)
          {
            nonEmptyCount += 1;
          }
        }

        // we require that more than 60% of the pixels are filled to count as a line. 
        // Combind with our choice of characters, this should eliminiate serifs from
        // the picture.
        if (nonEmptyCount / width > 0.6)
        {
          return line + 1;
        }
      }
      return -1;
    }

    float ComputeAutoComputeBaseLine(SpriteFont font, string name)
    {
      char c;
      if (font.Characters.Contains('z'))
      {
        c = 'z';
      }
      else if (font.Characters.Contains('Z'))
      {
        c = 'Z';
      }
      else if (font.Characters.Contains('_'))
      {
        c = '_';
      }
      else if (font.DefaultCharacter != null)
      {
        c = font.DefaultCharacter.Value;
      }
      else
      {
        return font.LineSpacing * 0.75f;
      }

      var size = font.MeasureString(c.ToString());
      var width = Math.Max(1, (int) Math.Ceiling(size.X));
      var height = Math.Max(1, (int) Math.Ceiling(size.Y));
      var colors = Render(font, c, width, height);
      var line = Analyze(colors, width);
      if (line <= 0)
      {
        tracer.TraceEvent(TraceEventType.Information, 0,
                          "[FontFactory] Unable to find base line for {0}, assuming 75% of font height ({1}) = {2}",
                          name, height, font.LineSpacing * 0.75);
        return height * 0.75f;
      }
      tracer.TraceEvent(TraceEventType.Verbose, 0, 
                        "[FontFactory] Found base line for {0}, using font height ({1}) = {2}", name, height, line);
      return line;
    }

    Color[] Render(SpriteFont font, char c, int width, int height)
    {
      var renderTarget = new RenderTarget2D(graphicsDevice, width, height, false,
                                            graphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.None);

      graphicsDevice.SetRenderTarget(renderTarget);
      graphicsDevice.Clear(Color.White);
      var b = new SpriteBatch(graphicsDevice);
      b.Begin();
      b.DrawString(font, c.ToString(), new Vector2(), Color.Black);
      b.End();
      graphicsDevice.SetRenderTarget(null);
      var colors = new Color[renderTarget.Width * renderTarget.Height];
      renderTarget.GetData(colors);
      return colors;
    }
  }
}