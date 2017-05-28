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
using Microsoft.Xna.Framework.Input;

namespace Steropes.UI.Input.GamePadInput
{
  public class GamePadVibrationComponent : BasicGameComponent
  {
    readonly GamePadVibrationState[] states;

    float currentTime;

    float vibrationLeftStrength;

    float vibrationRightStrength;

    public GamePadVibrationComponent()
    {
      VibrationEnabled = false;
      VibrationStrengthLeft = 1;
      VibrationStrengthRight = 1;

      states = new GamePadVibrationState[GamePad.MaximumGamePadCount];
    }

    /// <summary>
    ///   Whether vibration is enabled for this controller.
    /// </summary>
    public bool VibrationEnabled { get; set; }

    /// <summary>
    ///   General setting for the strength of the left motor.
    ///   This motor has a slow, deep, powerful rumble.
    ///   <para>
    ///     This setting will modify all future vibrations
    ///     through this listener.
    ///   </para>
    /// </summary>
    public float VibrationStrengthLeft
    {
      get
      {
        return vibrationLeftStrength;
      }
      set
      {
        // Clamp the value, just to be sure.
        vibrationLeftStrength = MathHelper.Clamp(value, 0, 1);
      }
    }

    /// <summary>
    ///   General setting for the strength of the right motor.
    ///   This motor has a snappy, quick, high-pitched rumble.
    ///   <para>
    ///     This setting will modify all future vibrations
    ///     through this listener.
    ///   </para>
    /// </summary>
    public float VibrationStrengthRight
    {
      get
      {
        return vibrationRightStrength;
      }
      set
      {
        // Clamp the value, just to be sure.
        vibrationRightStrength = MathHelper.Clamp(value, 0, 1);
      }
    }

    public override void Update(GameTime gameTime)
    {
      currentTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
      for (var i = 0; i < states.Length; i += 1)
      {
        CheckVibrateTimeOuts(i);
      }
    }

    /// <summary>
    ///   Send a vibration command to the controller.
    ///   Returns true if the operation succeeded.
    ///   <para>
    ///     Motor values that are unset preserve
    ///     their current vibration strength and duration.
    ///   </para>
    ///   Note: Vibration currently only works on select platforms,
    ///   like Monogame.Windows.
    /// </summary>
    /// <param name="playerIndex"></param>
    /// <param name="durationInSeconds">Duration of the vibration in seconds.</param>
    /// <param name="leftStrength">
    ///   The strength of the left motor.
    ///   This motor has a slow, deep, powerful rumble.
    /// </param>
    /// <param name="rightStrength">
    ///   The strength of the right motor.
    ///   This motor has a snappy, quick, high-pitched rumble.
    /// </param>
    /// <returns>Returns true if the operation succeeded.</returns>
    public bool Vibrate(int playerIndex, float durationInSeconds, float leftStrength = float.NegativeInfinity, float rightStrength = float.NegativeInfinity)
    {
      if (!VibrationEnabled)
      {
        return false;
      }
      if (!GamePad.GetState(playerIndex).IsConnected)
      {
        return false;
      }

      var state = states[playerIndex];

      var lstrength = float.IsNegativeInfinity(leftStrength) ? state.VibrationLeftCurrentStrength : MathHelper.Clamp(leftStrength, 0, 1);
      var rstrength = float.IsNegativeInfinity(rightStrength) ? state.VibrationRightCurrentStrength : MathHelper.Clamp(rightStrength, 0, 1);

      var success = GamePad.SetVibration(playerIndex, lstrength * VibrationStrengthLeft, rstrength * VibrationStrengthRight);
      if (success)
      {
        if (leftStrength > 0)
        {
          state.VibrationLeftEndTime = durationInSeconds + currentTime;
          state.VibrationLeft = true;
        }
        else if (lstrength > 0)
        {
          state.VibrationLeft = true;
        }
        else
        {
          state.VibrationLeft = false;
        }

        state.VibrationRight = true;
        if (rightStrength > 0)
        {
          state.VibrationRightEndTime = durationInSeconds + currentTime;
          state.VibrationRight = true;
        }
        else if (rstrength > 0)
        {
          state.VibrationLeft = true;
        }
        else
        {
          state.VibrationLeft = false;
        }

        state.VibrationLeftCurrentStrength = lstrength;
        state.VibrationRightCurrentStrength = rstrength;
      }
      states[playerIndex] = state;
      return success;
    }

    void CheckVibrateTimeOuts(int playerIndex)
    {
      var state = states[playerIndex];
      if (state.VibrationLeft && state.VibrationLeftEndTime < currentTime)
      {
        // ReSharper disable once ArgumentsStyleLiteral
        Vibrate(playerIndex, 0, leftStrength: 0);
      }
      if (state.VibrationRight && state.VibrationRightEndTime < currentTime)
      {
        Vibrate(playerIndex, 0, rightStrength: 0);
      }
    }

    struct GamePadVibrationState
    {
      public bool VibrationLeft { get; set; }

      public float VibrationLeftCurrentStrength { get; set; }

      public float VibrationLeftEndTime { get; set; }

      public bool VibrationRight { get; set; }

      public float VibrationRightCurrentStrength { get; set; }

      public float VibrationRightEndTime { get; set; }
    }
  }
}