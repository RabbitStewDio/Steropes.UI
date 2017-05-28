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

using Steropes.UI.Components;
using Steropes.UI.Platform;
using Steropes.UI.Styles;

namespace Steropes.UI.Widgets.Styles
{
  public class WidgetStyleDefinition : IStyleDefinition
  {
    public IStyleKey<Color> Color { get; private set; }

    public IStyleKey<Color> FocusedOverlayColor { get; private set; }

    public IStyleKey<IBoxTexture> FocusedOverlayTexture { get; private set; }

    public IStyleKey<Color> FrameOverlayColor { get; private set; }

    public IStyleKey<IBoxTexture> FrameOverlayTexture { get; private set; }

    public IStyleKey<IBoxTexture> FrameTexture { get; private set; }

    public IStyleKey<Color> HoverOverlayColor { get; private set; }

    public IStyleKey<IBoxTexture> HoverOverlayTexture { get; private set; }

    public IStyleKey<Insets> Margin { get; private set; }

    public IStyleKey<Insets> Padding { get; private set; }

    public IStyleKey<float> TooltipDelay { get; private set; }

    public IStyleKey<float> TooltipDisplayTime { get; private set; }

    public IStyleKey<TooltipPositionMode> TooltipPosition { get; private set; }

    public IStyleKey<Visibility> Visibility { get; private set; }

    public IStyleKey<IBoxTexture> WidgetStateOverlay { get; private set; }

    public IStyleKey<Color> WidgetStateOverlayColor { get; private set; }

    public IStyleKey<bool> WidgetStateOverlayScale { get; private set; }

    public void Init(IStyleSystem s)
    {
      TooltipDelay = s.CreateKey<float>("tooltip-delay", true);
      TooltipDisplayTime = s.CreateKey<float>("tooltip-display-time", true);
      TooltipPosition = s.CreateKey<TooltipPositionMode>("tooltip-position", true);
      Visibility = s.CreateKey<Visibility>("visibility", false);

      FrameTexture = s.CreateKey<IBoxTexture>("frame-texture", false);

      FrameOverlayTexture = s.CreateKey<IBoxTexture>("frame-overlay-texture", false);
      FrameOverlayColor = s.CreateKey<Color>("frame-overlay-color", false);

      HoverOverlayTexture = s.CreateKey<IBoxTexture>("hover-overlay-texture", false);
      HoverOverlayColor = s.CreateKey<Color>("hover-overlay-color", false);

      FocusedOverlayTexture = s.CreateKey<IBoxTexture>("focused-overlay-texture", false);
      FocusedOverlayColor = s.CreateKey<Color>("focused-overlay-color", false);

      WidgetStateOverlay = s.CreateKey<IBoxTexture>("widget-state-overlay", false);
      WidgetStateOverlayScale = s.CreateKey<bool>("widget-state-overlay-scale", false);
      WidgetStateOverlayColor = s.CreateKey<Color>("widget-state-overlay-color", false);

      Padding = s.CreateKey<Insets>("padding", false);
      Margin = s.CreateKey<Insets>("margin", false);
      Color = s.CreateKey<Color>("color", false);
    }
  }
}