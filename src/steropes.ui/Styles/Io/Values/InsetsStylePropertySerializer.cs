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
using System.Globalization;
using System.Xml.Linq;

using Steropes.UI.Platform;
using Steropes.UI.Styles.Io.Parser;

namespace Steropes.UI.Styles.Io.Values
{
  public class InsetsStylePropertySerializer : IStylePropertySerializer
  {
    readonly string elementName;

    public InsetsStylePropertySerializer() : this("insets")
    {
    }

    public InsetsStylePropertySerializer(string elementName)
    {
      if (elementName == null)
      {
        throw new ArgumentNullException(nameof(elementName));
      }
      this.elementName = elementName;
    }

    public Type TargetType => typeof(Insets);

    public string TypeId => "Insets";

    public object Parse(IStyleSystem styleSystem, XElement reader)
    {
      if (reader == null)
      {
        return new Insets();
      }
      var element = reader.ElementLocal(elementName);
      if (element == null)
      {
        return new Insets();
      }

      var all = (int?)element.ElementLocal("all");
      if (all != null)
      {
        return new Insets(all.GetValueOrDefault());
      }

      var top = (int?)element.ElementLocal("top");
      var left = (int?)element.ElementLocal("left");
      var bottom = (int?)element.ElementLocal("bottom");
      var right = (int?)element.ElementLocal("right");
      return new Insets(top.GetValueOrDefault(), left.GetValueOrDefault(), bottom.GetValueOrDefault(), right.GetValueOrDefault());
    }

    public void Write(IStyleSystem styleSystem, XElement propertyElement, object value)
    {
      var insets = (Insets)value;
      var insetsElement = new XElement(StyleParser.StyleNamespace + elementName);
      if (insets.Top == insets.Left && insets.Top == insets.Right && insets.Top == insets.Bottom)
      {
        insetsElement.Add(new XElement(StyleParser.StyleNamespace + "all", insets.Top.ToString(CultureInfo.InvariantCulture)));
      }
      else
      {
        insetsElement.Add(new XElement(StyleParser.StyleNamespace + "top", insets.Top.ToString(CultureInfo.InvariantCulture)));
        insetsElement.Add(new XElement(StyleParser.StyleNamespace + "left", insets.Left.ToString(CultureInfo.InvariantCulture)));
        insetsElement.Add(new XElement(StyleParser.StyleNamespace + "bottom", insets.Bottom.ToString(CultureInfo.InvariantCulture)));
        insetsElement.Add(new XElement(StyleParser.StyleNamespace + "right", insets.Right.ToString(CultureInfo.InvariantCulture)));
      }

      propertyElement.Add(insetsElement);
    }
  }
}