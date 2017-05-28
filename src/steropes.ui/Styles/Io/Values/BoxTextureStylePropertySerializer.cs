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
using System.Xml.Linq;

using Steropes.UI.Platform;
using Steropes.UI.Styles.Io.Parser;

namespace Steropes.UI.Styles.Io.Values
{
  public class BoxTextureStylePropertySerializer : IStylePropertySerializer
  {
    public BoxTextureStylePropertySerializer()
    {
    }

    public Type TargetType => typeof(IBoxTexture);

    public string TypeId => "BoxTexture";

    public object Parse(IStyleSystem styleSystem, XElement reader)
    {
      var textureElement = reader.ElementLocal("texture");
      var texture = (string) textureElement?.ElementLocal("name");
      if (string.IsNullOrWhiteSpace(texture))
      {
        texture = null;
      }

      var insets = (Insets)new InsetsStylePropertySerializer("corners").Parse(styleSystem, textureElement);
      var margins = (Insets)new InsetsStylePropertySerializer("margins").Parse(styleSystem, textureElement);
      var contentLoader = styleSystem.ContentLoader;
      return contentLoader.LoadTexture(texture, insets, margins);
    }

    public void Write(IStyleSystem styleSystem, XElement propertyElement, object value)
    {
      var texture = (IBoxTexture)value;
      var element = new XElement(StyleParser.StyleNamespace + "texture");
      element.Add(new XElement(StyleParser.StyleNamespace + "name", texture.Name));
      if (texture.CornerArea != Insets.Zero)
      {
        new InsetsStylePropertySerializer("corners").Write(styleSystem, element, texture.CornerArea);
      }
      if (texture.Margins != Insets.Zero)
      {
        new InsetsStylePropertySerializer("margins").Write(styleSystem, element, texture.Margins);
      }

      propertyElement.Add(element);
    }
  }
}