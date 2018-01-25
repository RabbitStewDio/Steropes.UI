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
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using Steropes.UI.Animation;
using Steropes.UI.Platform;
using Steropes.UI.State;

namespace Steropes.UI.Demo.GameStates
{
  class GameStateIntro : GameStateFadeTransition
  {
    readonly string nextState;

    readonly INamedStateManager stateService;

    AnimatedValue logoAnim;

    Texture2D logoTex;

    AnimatedValue mushroomAnim;

    AnimatedValue mushroomOpacityAnim;

    Texture2D mushroomTex;

    Random randomSource;

    Texture2D sparklinLabsTex;

    LerpValue switchTimer;

    AnimatedValue titleAnim;

    Texture2D titleTex;

    public GameStateIntro(NuclearSampleGame game, BatchedDrawingService drawingService, INamedStateManager stateService, string nextState) : base(drawingService)
    {
      Game = game;
      this.stateService = stateService;
      this.nextState = nextState;

      Content = new ContentManager(game.Services) { RootDirectory = "Content" };
    }

    public ContentManager Content { get; set; }

    public NuclearSampleGame Game { get; }

    public override void Draw(GameTime time)
    {
      Game.GraphicsDevice.Clear(new Color(45, 51, 49));
      var spriteBatch = new SpriteBatch(Game.GraphicsDevice);

      spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Matrix.Identity);

      var screenCenter = new Vector2(Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height) / 2f;

      var mushroomOrigin = new Vector2(mushroomTex.Width, mushroomTex.Height) / 2f;
      var titleOrigin = new Vector2(titleTex.Width, titleTex.Height) / 2f;
      var logoOrigin = new Vector2(logoTex.Width, logoTex.Height) / 2f;

      var titleOffsetAngle = (float)randomSource.NextDouble() * MathHelper.TwoPi;
      var titleOffset = new Vector2((float)Math.Cos(titleOffsetAngle), (float)Math.Sin(titleOffsetAngle)) * titleAnim.CurrentValue;

      spriteBatch.Draw(
        mushroomTex,
        screenCenter + new Vector2(-190 + 800f * (float)Math.Pow(mushroomAnim.CurrentValue, 2), -340) + mushroomOrigin,
        null,
        Color.White * (float)Math.Pow(mushroomOpacityAnim.CurrentValue, 3),
        0f,
        mushroomOrigin,
        Vector2.One,
        SpriteEffects.None,
        0f);
      spriteBatch.Draw(titleTex, screenCenter + new Vector2(-210, -50) + titleOrigin + titleOffset, null, Color.White, 0f, titleOrigin, Vector2.One, SpriteEffects.None, 0f);
      spriteBatch.Draw(
        logoTex,
        screenCenter + new Vector2(-580, -80) + logoOrigin,
        null,
        Color.White * logoAnim.CurrentValue,
        0f,
        logoOrigin,
        Vector2.One * (2f - (float)Math.Pow(logoAnim.CurrentValue, 3)),
        SpriteEffects.None,
        0f);

      spriteBatch.Draw(
        sparklinLabsTex,
        new Vector2(Game.GraphicsDevice.Viewport.Width - sparklinLabsTex.Width, Game.GraphicsDevice.Viewport.Height - sparklinLabsTex.Height),
        null,
        Color.White);

      spriteBatch.End();
    }

    public override void Start()
    {
      randomSource = new Random();

      mushroomAnim = new SmoothValue(1f, 0f, 0.3f);
      mushroomOpacityAnim = new SmoothValue(0f, 1f, 0.3f);
      logoAnim = new LerpValue(0f, 1f, 0.3f, 0.3f);
      titleAnim = new SmoothValue(0f, 10f, 3f, 0.1f);

      mushroomTex = Content.Load<Texture2D>("Sprites/Mushroom");
      titleTex = Content.Load<Texture2D>("Sprites/NuclearWinterTitle");
      logoTex = Content.Load<Texture2D>("Sprites/NuclearWinterLogo");

      sparklinLabsTex = Content.Load<Texture2D>("Sprites/SparklinLabs");

      switchTimer = new LerpValue(0f, 1f, 1f, 1.5f);

      base.Start();
    }

    public override void Update(GameTime elapsedTime)
    {
      mushroomAnim.Update(elapsedTime);
      mushroomOpacityAnim.Update(elapsedTime);
      logoAnim.Update(elapsedTime);
      titleAnim.Update(elapsedTime);
      switchTimer.Update(elapsedTime);

      if (switchTimer.IsOver && !stateService.IsSwitching)
      {
        stateService.SwitchState(stateService.States[nextState]);
      }
    }
  }
}