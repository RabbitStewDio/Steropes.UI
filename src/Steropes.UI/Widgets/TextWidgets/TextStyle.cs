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

using Steropes.UI.Annotations;
using Steropes.UI.Components;
using Steropes.UI.Platform;

namespace Steropes.UI.Widgets.TextWidgets
{
  public struct TextStyle
  {
    [NotNull]
    public IUIFont Font { get; set; }

    public Color TextColor { get; set; }

    public Color? BackgroundColor { get; set; }

    public bool Underlined { get; set; }

    public bool StrikeThrough { get; set; }

    public Alignment Alignment { get; set; }

    public Color OutlineColor { get; set; }

    public float OutlineRadius { get; set; }

    public WrapText WrapText { get; set; }

    public TextStyle(
      [NotNull] IUIFont font,
      Color textColor,
      Color? backgroundColor = null,
      Color outlineColor = new Color(),
      float outlineRadius = 0,
      Alignment alignment = Alignment.Start,
      bool underlined = false,
      bool strikeThrough = false,
      WrapText wrapText = WrapText.Auto)
    {
      if (font == null)
      {
        throw new ArgumentNullException(nameof(font));
      }

      Font = font;
      TextColor = textColor;
      BackgroundColor = backgroundColor;
      OutlineColor = outlineColor;
      OutlineRadius = outlineRadius;
      Underlined = underlined;
      StrikeThrough = strikeThrough;
      WrapText = wrapText;
      Alignment = alignment;
    }
  }
}