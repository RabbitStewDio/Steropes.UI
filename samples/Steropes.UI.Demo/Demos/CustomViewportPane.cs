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

using Steropes.UI.Components;
using Steropes.UI.Input.MouseInput;
using Steropes.UI.Platform;
using Steropes.UI.Widgets;

namespace Steropes.UI.Demo.Demos
{
  class CustomViewportPane : ScrollPanel
  {
    public CustomViewportPane(IUIStyle style) : base(style)
    {
      Content = new MyCustomViewport(UIStyle);
    }

    class MyCustomViewport : CustomViewport
    {
      float distance = -3f;

      BasicEffect effect;

      float rotation;

      public MyCustomViewport(IUIStyle style) : base(style)
      {
        MouseWheel += OnMouseWheel;
      }

      public override void Update(GameTime elapsedTime)
      {
        base.Update(elapsedTime);
        rotation += MathHelper.TwoPi / 180f;
      }

      protected override Rectangle ArrangeOverride(Rectangle layoutSize)
      {
        return layoutSize;
      }

      protected override void DrawCustomContent(IBatchedDrawingService drawingService)
      {
        if (effect == null)
        {
          effect = new BasicEffect(drawingService.GraphicsDevice);
          effect.VertexColorEnabled = true;
        }

        var viewportRatio = (float)ContentRect.Width / ContentRect.Height;
        effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver2, viewportRatio, 0.1f, 1000f);
        effect.View = Matrix.CreateRotationY(rotation) * Matrix.CreateTranslation(0, 0, distance);

        effect.CurrentTechnique.Passes[0].Apply();
        effect.GraphicsDevice.DrawUserPrimitives(
          PrimitiveType.TriangleList,
          new[]
          {
            new VertexPositionColor(new Vector3(-1f, -1f, 0f), Color.Red), new VertexPositionColor(new Vector3(1f, 1f, 0f), Color.Green),
            new VertexPositionColor(new Vector3(1f, -1f, 0f), Color.Blue)
          },
          0,
          1);
      }

      protected override Size MeasureOverride(Size availableSize)
      {
        return new Size();
      }

      void OnMouseWheel(object source, MouseEventArgs args)
      {
        // Just an example of how you can interact through the CustomViewport widget
        // You can override lots of other event handlers for mouse & keyboard events
        distance += args.ScrollWheelDelta / 120f * 0.5f;
      }
    }
  }
}