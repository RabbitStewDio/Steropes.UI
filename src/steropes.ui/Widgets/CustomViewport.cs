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
using Microsoft.Xna.Framework.Graphics;

using Steropes.UI.Components;
using Steropes.UI.Platform;

namespace Steropes.UI.Widgets
{
  public abstract class CustomViewport : Widget
  {
    Viewport previousViewport;

    protected CustomViewport(IUIStyle style) : base(style)
    {
    }

    protected sealed override void DrawWidget(IBatchedDrawingService drawingService)
    {
      BeginDraw(drawingService);
      try
      {
        DrawCustomContent(drawingService);
      }
      finally
      {
        EndDraw(drawingService);
      }
    }

    protected abstract void DrawCustomContent(IBatchedDrawingService drawingService);

    protected virtual SpriteBatch BeginDraw(IBatchedDrawingService drawingService)
    {
      var sb = drawingService.SuspendBatch();
      previousViewport = drawingService.GraphicsDevice.Viewport;
      
      var viewport = new Viewport(LayoutRect);
      drawingService.GraphicsDevice.Viewport = viewport;
      return sb;
    }

    protected virtual void EndDraw(IBatchedDrawingService drawingService)
    {
      drawingService.GraphicsDevice.Viewport = previousViewport;
      drawingService.ResumeBatch();
    }
  }
}