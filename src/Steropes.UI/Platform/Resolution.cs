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
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Steropes.UI.Platform
{
  /// <summary>
  ///   Static class to manage game resolution and scaling. Use this for device-independent graphics.
  ///   Simply render at a perferred resolution and let the system take care of all scaling of graphical content.
  /// </summary>
  public sealed class Resolution
  {
    public Resolution()
    {
      ScaleFactor = 1f;
      InternalMode = new ScreenMode(1920, 1080, 16 / 9f);

      var sortedScreenModes = new List<ScreenMode>();

      foreach (var displayMode in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
      {
        if (displayMode.AspectRatio > 1f && displayMode.Width >= 800f && displayMode.Width <= InternalMode.Width && displayMode.Height <= InternalMode.Height)
        {
          var screenMode = new ScreenMode(displayMode.Width, displayMode.Height, displayMode.AspectRatio);
          if (!sortedScreenModes.Contains(screenMode))
          {
            sortedScreenModes.Add(screenMode);
          }
        }
      }

      sortedScreenModes.Sort();
      SortedScreenModes = sortedScreenModes.AsReadOnly();
    }

    public static Matrix Scale { get; private set; }

    public static float ScaleFactor { get; private set; }

    public Viewport DefaultViewport { get; private set; }

    public ScreenMode InternalMode { get; private set; }

    public ScreenMode Mode { get; private set; }

    public IReadOnlyList<ScreenMode> SortedScreenModes { get; }

    public Viewport Viewport { get; private set; }

    // Initialize the best Resolution available
    public ScreenMode Initialize(GraphicsDeviceManager graphics)
    {
      for (var i = SortedScreenModes.Count - 1; i >= 0; i--)
      {
        var mode = SortedScreenModes[i];
        if (SetScreenMode(graphics, mode, true))
        {
          return mode;
        }
      }

      return new ScreenMode(0, 0, 0);
    }

    // Initialize the Resolution using the specified ScreenMode
    public bool Initialize(GraphicsDeviceManager graphics, ScreenMode mode, bool fullscreen)
    {
      InternalMode = new ScreenMode(1920, 1080, 16 / 9f);
      return SetScreenMode(graphics, mode, fullscreen);
    }

    // Set the specified screen mode
    public bool SetScreenMode(GraphicsDeviceManager graphics, ScreenMode mode, bool fullscreen)
    {
      var graphicsAdapter = GraphicsAdapter.DefaultAdapter;
      if (fullscreen)
      {
        // Fullscreen
        var supportedDisplayModes = graphicsAdapter.SupportedDisplayModes;
        foreach (var displayMode in supportedDisplayModes)
        {
          if ((mode.Width == displayMode.Width) && (mode.Height == displayMode.Height))
          {
            try
            {
              ApplyScreenMode(graphics, mode, true);
            }
            catch
            {
              return false;
            }
            return true;
          }
        }
      }
      else if ((mode.Width <= graphicsAdapter.CurrentDisplayMode.Width) && (mode.Height <= graphicsAdapter.CurrentDisplayMode.Height))
      {
        ApplyScreenMode(graphics, mode, false);
        return true;
      }

      return false;
    }

    // Apply the specified ScreenMode and preserves the aspect ratio expressed in the mode.
    void ApplyScreenMode(GraphicsDeviceManager graphics, ScreenMode mode, bool fullscreen)
    {
      Mode = mode;
      graphics.PreferredBackBufferWidth = Mode.Width;
      graphics.PreferredBackBufferHeight = Mode.Height;
      graphics.PreferMultiSampling = true;
      graphics.IsFullScreen = fullscreen;
      graphics.ApplyChanges();

      ScaleFactor = Mode.Width * Mode.AspectRatio / InternalMode.Height;
      Scale = Matrix.CreateScale(ScaleFactor);

      DefaultViewport = graphics.GraphicsDevice.Viewport;

      var viewport = new Viewport { X = 0, Y = (int)((Mode.Height - Mode.Width / Mode.AspectRatio) / 2), Width = Mode.Width, Height = (int)(Mode.Width / Mode.AspectRatio) };
      graphics.GraphicsDevice.Viewport = Viewport;
      Viewport = viewport;
    }
  }
}