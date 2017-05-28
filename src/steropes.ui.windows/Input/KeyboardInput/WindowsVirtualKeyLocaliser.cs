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
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

using Microsoft.Xna.Framework.Input;

using Steropes.UI.Input.KeyboardInput;

namespace Steropes.UI.Windows.Input.KeyboardInput
{
  public class WindowsVirtualKeyLocaliser : DefaultVirtualKeyLocaliser
  {
    //// ReSharper disable InconsistentNaming
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "WinAPI code.")]
    internal const uint KLF_NOTELLSHELL = 0x00000080;

    //// ReSharper restore InconsistentNaming
    internal enum MappingType : uint
    {
      // ReSharper disable InconsistentNaming
      VirtualKey_To_ScanCode = 4,

      ScanCode_To_VirtualKey = 3,

      VirtualKey_To_Char = 2

      // ReSharper restore InconsistentNaming
    }

    public static Keys Windows_LocalToUSEnglish(Keys key)
    {
      if (HasNoScanCodeMapping(key))
      {
        return key;
      }

      var activeScanCode = MapVirtualKeyEx((uint)key, MappingType.VirtualKey_To_ScanCode, KeyboardLayout.Active.Handle);
      if (activeScanCode == 0)
      {
        // no translation exists 
        return key;
      }

      var nativeVirtualCode = MapVirtualKeyEx(activeScanCode, MappingType.ScanCode_To_VirtualKey, KeyboardLayout.US_English.Handle);
      if (nativeVirtualCode == 0)
      {
        return key;
      }

      return (Keys)nativeVirtualCode;
    }

    public static Keys Windows_USEnglishToLocal(Keys key)
    {
      if (HasNoScanCodeMapping(key))
      {
        return key;
      }
      var activeScanCode = MapVirtualKeyEx((uint)key, MappingType.VirtualKey_To_ScanCode, KeyboardLayout.US_English.Handle);
      if (activeScanCode == 0)
      {
        // no translation exists 
        return key;
      }
      var nativeVirtualCode = MapVirtualKeyEx(activeScanCode, MappingType.ScanCode_To_VirtualKey, KeyboardLayout.Active.Handle);
      if (nativeVirtualCode == 0)
      {
        return key;
      }

      return (Keys)nativeVirtualCode;
    }

    public static char Windows_VKey_To_Char(Keys key)
    {
      var activeScanCode = MapVirtualKeyEx((uint)key, MappingType.VirtualKey_To_Char, KeyboardLayout.Active.Handle);

      return (char)activeScanCode;
    }

    public override Keys ToLocalisedKey(Keys key)
    {
      return Windows_USEnglishToLocal(key);
    }

    public override string ToLocalisedText(Keys k)
    {
      var retval = Windows_VKey_To_Char(k);
      var c = (char)(retval & char.MaxValue);
      if (c == 0)
      {
        Debug.WriteLine("No translation for " + k);
        return base.ToLocalisedText(k);
      }

      Debug.WriteLine("translation for " + k + " is " + (int)c);
      if (k >= Keys.NumPad0 && k <= Keys.Divide)
      {
        return "KeyPad-" + c;
      }
      if (c == '\n')
      {
        return "Enter";
      }
      if (c == '\t')
      {
        return "Tab";
      }
      if (c == 12)
      {
        return "Clear";
      }
      return c.ToString();
    }

    [DllImport("user32.dll", CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern IntPtr GetKeyboardLayout(IntPtr threadId);

    [DllImport("user32.dll", CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern IntPtr LoadKeyboardLayout(string keyboardLayoutID, uint flags);

    [DllImport("user32.dll", CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern uint MapVirtualKeyEx(uint key, MappingType mappingType, IntPtr keyboardLayout);

    [DllImport("user32.dll", CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern bool UnloadKeyboardLayout(IntPtr handle);

    static bool HasNoScanCodeMapping(Keys key)
    {
      if (key >= Keys.NumPad0 && key <= Keys.NumPad9)
      {
        // this would be mapped to the non-numpad key meaning (cursor keys, home etc). 
        // We would loose the differentiation between num-pad keys and the mapped keys.
        return true;
      }
      if (key >= Keys.BrowserBack && key <= Keys.LaunchApplication2)
      {
        // not mappable in any normal scenario. Return as is.
        return true;
      }
      return false;
    }

    internal struct KeyboardLayout : IDisposable
    {
      public readonly IntPtr Handle;

      public KeyboardLayout(IntPtr handle) : this()
      {
        Handle = handle;
      }

      public KeyboardLayout(string keyboardLayoutID) : this(LoadKeyboardLayout(keyboardLayoutID, KLF_NOTELLSHELL))
      {
      }

      public bool IsDisposed { get; private set; }

      public void Dispose()
      {
        if (IsDisposed)
        {
          return;
        }

        UnloadKeyboardLayout(Handle);
        IsDisposed = true;
      }

      // ReSharper disable once InconsistentNaming
      [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Reviewed. Suppression is OK here.")]
      public static KeyboardLayout US_English = new KeyboardLayout("00000409");

      public static KeyboardLayout Active => new KeyboardLayout(GetKeyboardLayout(IntPtr.Zero));
    }
  }
}