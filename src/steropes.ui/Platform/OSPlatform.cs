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
using System.IO;

namespace Steropes.UI.Platform
{
  /// <summary>
  ///   A cross platform OS detection system. Sadly, and as usual, the system APIs around
  ///   this functionality are badly designed and only half-implemented across the various
  ///   platforms (Mono vs Windows .NET vs .NET core).
  ///   <p />
  ///   This code bypasses all the unreliable stuff and asks the OS directly.
  ///   This code is based on the answer given here: http://stackoverflow.com/a/38795621
  /// </summary>
  public static class OSPlatform
  {
    static OSPlatform()
    {
      var windir = Environment.GetEnvironmentVariable("windir");
      if (!string.IsNullOrEmpty(windir) && windir.Contains(@"\") && Directory.Exists(windir))
      {
        OS = OperatingSystem.Windows;
      }
      else if (File.Exists(@"/proc/sys/kernel/ostype"))
      {
        var osType = File.ReadAllText(@"/proc/sys/kernel/ostype");
        if (osType.StartsWith("Linux", StringComparison.OrdinalIgnoreCase))
        {
          // Note: Android gets here too
          OS = OperatingSystem.Linux;
        }
        else
        {
          OS = OperatingSystem.Unknown;
        }
      }
      else if (File.Exists(@"/System/Library/CoreServices/SystemVersion.plist"))
      {
        // Note: iOS gets here too
        OS = OperatingSystem.Mac;
      }
      else
      {
        OS = OperatingSystem.Unknown;
      }
    }

    public enum OperatingSystem
    {
      Windows,

      Mac,

      Linux,

      Unknown
    }

    public static OperatingSystem OS { get; private set; }
  }
}