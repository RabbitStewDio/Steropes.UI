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
using Steropes.UI.Widgets.TextWidgets;

namespace Steropes.UI.Widgets.Styles
{
  public class TextStyleDefinition : IStyleDefinition
  {
    public IStyleKey<Alignment> Alignment { get; private set; }

    public IStyleKey<int> CaretWidth { get; private set; }
    public IStyleKey<float> CaretBlinkRate { get; private set; }
    public IStyleKey<bool> CaretBlinking { get; private set; }

    public IStyleKey<IUIFont> Font { get; private set; }

    public IStyleKey<Color> OutlineColor { get; private set; }

    public IStyleKey<int> OutlineSize { get; private set; }

    public IStyleKey<Color> SelectionColor { get; private set; }

    public IStyleKey<bool> StrikeThrough { get; private set; }

    public IStyleKey<Color> TextColor { get; private set; }

    public IStyleKey<bool> Underline { get; private set; }

    public IStyleKey<WrapText> WrapText { get; private set; }

    public void Init(IStyleSystem s)
    {
      Font = s.CreateKey<IUIFont>("font", true);
      TextColor = s.CreateKey<Color>("text-color", true);
      Alignment = s.CreateKey<Alignment>("text-alignment", true);
      OutlineColor = s.CreateKey<Color>("outline-color", true);
      OutlineSize = s.CreateKey<int>("outline-size", true);
      WrapText = s.CreateKey<WrapText>("wrap-text", true);
      Underline = s.CreateKey<bool>("underline", true);
      StrikeThrough = s.CreateKey<bool>("strike-through", true);
      CaretWidth = s.CreateKey<int>("caret-width", true);
      CaretBlinkRate = s.CreateKey<float>("caret-blink-rate", true);
      CaretBlinking = s.CreateKey<bool>("caret-blinking", true);
      SelectionColor = s.CreateKey<Color>("selection-color", true);
    }
  }
}