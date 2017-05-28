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

using Steropes.UI.Animation;
using Steropes.UI.Components;
using Steropes.UI.Platform;
using Steropes.UI.Styles;

namespace Steropes.UI.Widgets
{
  public class SpinningWheel : Image
  {
    public const string SmallStyleClass = "Small";

    const float FadeDelay = 0.2f;

    const float FadeDuration = 0.4f;

    readonly AnimatedValue fadeInAnim;

    readonly LerpValue rotationValue;

    public SpinningWheel(IUIStyle style) : base(style)
    {
      fadeInAnim = new SmoothValue(0, 1, FadeDuration, FadeDelay, AnimationLoop.NoLoop);
      rotationValue = new LerpValue(0, MathHelper.Pi, 1, AnimationLoop.Loop);
      FadeIn = true;
      Style.ValueChanged += OnStyleChanged;
    }

    public bool FadeIn { get; set; }

    public bool Small
    {
      get
      {
        return StyleClasses.Contains(SmallStyleClass);
      }
      set
      {
        if (value)
        {
          AddStyleClass(SmallStyleClass);
        }
        else
        {
          RemoveStyleClass(SmallStyleClass);
        }
      }
    }

    public void Reset()
    {
      rotationValue.StartAnimation();
      fadeInAnim.StartAnimation();
    }

    public override void Update(GameTime elapsedTime)
    {
      base.Update(elapsedTime);
      if (Visibility == Visibility.Visible)
      {
        rotationValue.Update(elapsedTime);
        fadeInAnim.Update(elapsedTime);
      }
    }

    protected override void DrawWidget(IBatchedDrawingService drawingService)
    {
      if (Texture == null)
      {
        return;
      }

      var origin = new Vector2(Texture.Width / 2f, Texture.Height / 2f);
      var fadedColor = Color;
      if (FadeIn)
      {
        fadedColor *= fadeInAnim.CurrentValue;
      }

      if (Stretch == ScaleMode.None)
      {
        var contentRect = ContentRect;
        drawingService.Draw(
          Texture,
          new Vector2(contentRect.Center.X - DesiredSize.Width / 2, contentRect.Center.Y - DesiredSize.Height / 2) + origin,
          null,
          fadedColor,
          rotationValue.CurrentValue,
          origin,
          1f,
          SpriteEffects.None,
          1f);
      }
      else
      {
        var contentRect = CalculateImageSize(ContentRect);
        drawingService.Draw(
          Texture,
          new Rectangle(contentRect.Left + (int)origin.X, contentRect.Top + (int)origin.Y, contentRect.Width, contentRect.Height),
          null,
          fadedColor,
          rotationValue.CurrentValue,
          origin,
          SpriteEffects.None,
          1f);
      }
    }

    void OnStyleChanged(object sender, StyleEventArgs e)
    {
      if (ReferenceEquals(e.Key, WidgetStyle.Visibility))
      {
        if (Visibility == Visibility.Visible)
        {
          fadeInAnim.StartAnimation();
        }
      }
    }
  }
}