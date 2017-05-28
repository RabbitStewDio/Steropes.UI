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
using System.Text;

using Microsoft.Xna.Framework.Input;

using Steropes.UI.I18N;
using Steropes.UI.Platform;

namespace Steropes.UI.Input
{
  [Flags]
  public enum InputFlags
  {
    Control = 1 << 0,

    Shift = 1 << 1,

    Alt = 1 << 2,

    Meta = 1 << 3,

    None = 0,

    Mouse1 = 1 << 16,

    Mouse2 = 1 << 17,

    Mouse3 = 1 << 18,

    Mouse4 = 1 << 19,

    Mouse5 = 1 << 20,

    ShortCutKey = Control
  }

  public static class InputFlagsHelper
  {
    const InputFlags KeyModifiers = InputFlags.Shift | InputFlags.Control | InputFlags.Alt | InputFlags.Meta;

    const InputFlags MouseButtons = InputFlags.Mouse1 | InputFlags.Mouse2 | InputFlags.Mouse3 | InputFlags.Mouse4 | InputFlags.Mouse5;

    public static bool AnyMouseButton(this InputFlags flag)
    {
      return (flag & MouseButtons) != 0;
    }

    public static InputFlags AsKeyModifiers(this InputFlags m)
    {
      return m & KeyModifiers;
    }

    public static string AsText(this InputFlags m)
    {
      if (m == InputFlags.None)
      {
        return "";
      }

      var needPadding = false;
      var b = new StringBuilder();
      if (m.IsControlDown())
      {
        b.Append(Common.Ctrl);
        needPadding = true;
      }
      if (m.IsAltDown())
      {
        if (needPadding)
        {
          b.Append("-");
        }
        b.Append(Common.Alt);
        needPadding = true;
      }
      if (m.IsMetaDown())
      {
        if (needPadding)
        {
          b.Append("-");
        }
        b.Append(OSPlatform.OS == OSPlatform.OperatingSystem.Mac ? Common.Meta : Common.Windows);
        needPadding = true;
      }
      if (m.IsShiftDown())
      {
        if (needPadding)
        {
          b.Append("-");
        }
        b.Append(Common.Shift);
      }
      return b.ToString();
    }

    public static InputFlags Create()
    {
      return Create(Keyboard.GetState(), Mouse.GetState());
    }

    public static InputFlags Create(KeyboardState keys, MouseState mouse)
    {
      var inputFlags = InputFlags.None;
      inputFlags |= InputFlags.Shift.Set(keys.IsKeyDown(Keys.LeftShift) || keys.IsKeyDown(Keys.RightShift));
      inputFlags |= InputFlags.Control.Set(keys.IsKeyDown(Keys.LeftControl) || keys.IsKeyDown(Keys.RightControl));
      inputFlags |= InputFlags.Alt.Set(keys.IsKeyDown(Keys.LeftAlt) || keys.IsKeyDown(Keys.RightAlt));
      inputFlags |= InputFlags.Meta.Set(keys.IsKeyDown(Keys.LeftWindows) || keys.IsKeyDown(Keys.RightWindows));

      inputFlags |= InputFlags.Mouse1.Set(mouse.LeftButton == ButtonState.Pressed);
      inputFlags |= InputFlags.Mouse2.Set(mouse.MiddleButton == ButtonState.Pressed);
      inputFlags |= InputFlags.Mouse3.Set(mouse.RightButton == ButtonState.Pressed);
      inputFlags |= InputFlags.Mouse4.Set(mouse.XButton1 == ButtonState.Pressed);
      inputFlags |= InputFlags.Mouse5.Set(mouse.XButton2 == ButtonState.Pressed);

      return inputFlags;
    }

    public static bool IsAllDown(this InputFlags m, InputFlags flags)
    {
      return (m & flags) == flags;
    }

    public static bool IsAltDown(this InputFlags m)
    {
      return (m & InputFlags.Alt) != 0;
    }

    public static bool IsAnyDown(this InputFlags m, InputFlags flags)
    {
      return (m & flags) != 0;
    }

    public static bool IsControlDown(this InputFlags m)
    {
      return (m & InputFlags.Control) != 0;
    }

    public static bool IsMetaDown(this InputFlags m)
    {
      return (m & InputFlags.Meta) != 0;
    }

    public static bool IsModifier(this Keys keys)
    {
      if (keys == Keys.LeftShift || keys == Keys.RightShift)
      {
        return true;
      }
      if (keys == Keys.LeftControl || keys == Keys.RightControl)
      {
        return true;
      }
      if (keys == Keys.LeftAlt || keys == Keys.RightAlt)
      {
        return true;
      }
      if (keys == Keys.LeftWindows || keys == Keys.RightWindows)
      {
        return true;
      }
      return false;
    }

    public static bool IsShiftDown(this InputFlags m)
    {
      return (m & InputFlags.Shift) != 0;
    }

    public static bool IsShortcutKeyDown(this InputFlags m)
    {
      return (m & InputFlags.ShortCutKey) != 0;
    }

    static InputFlags Set(this InputFlags flag, bool condition)
    {
      return condition ? flag : InputFlags.None;
    }
  }
}