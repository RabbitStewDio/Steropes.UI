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

using Microsoft.Xna.Framework.Input;

namespace Steropes.UI.Input.MouseInput
{
  [Flags]
  public enum MouseButton
  {
    None = 0,

    Left = 1 << 0,

    Middle = 1 << 1,

    Right = 1 << 2,

    XButton1 = 1 << 3,

    XButton2 = 1 << 4
  }

  public static class MouseButtonExtensions
  {
    public static List<MouseButton> GetAll()
    {
      return new List<MouseButton> { MouseButton.Left, MouseButton.Middle, MouseButton.Right, MouseButton.XButton1, MouseButton.XButton2 };
    }

    public static ButtonState StateFor(this MouseState state, MouseButton b)
    {
      switch (b)
      {
        case MouseButton.None:
          return ButtonState.Released;
        case MouseButton.Left:
          return state.LeftButton;
        case MouseButton.Middle:
          return state.MiddleButton;
        case MouseButton.Right:
          return state.RightButton;
        case MouseButton.XButton1:
          return state.XButton1;
        case MouseButton.XButton2:
          return state.XButton2;
        default:
          throw new ArgumentOutOfRangeException(nameof(b), b, null);
      }
    }
  }
}