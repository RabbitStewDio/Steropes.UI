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
using System.Diagnostics;
using System.Globalization;
using System.Xml.Linq;

using Microsoft.Xna.Framework;

using Steropes.UI.Styles.Io.Parser;

namespace Steropes.UI.Styles.Io.Values
{
  public class ColorValueStylePropertySerializer : IStylePropertySerializer
  {
    static readonly Dictionary<string, Color> KnownColors;

    static ColorValueStylePropertySerializer()
    {
      KnownColors = new Dictionary<string, Color>();
      RegisterColors();
    }

    public ColorValueStylePropertySerializer()
    {
    }

    public Type TargetType => typeof(Color);

    public string TypeId => "Color";

    public static Color ParseFromString(string colorAsText, bool premultiplied = false)
    {
      var parsed = int.Parse(colorAsText.Substring(1), NumberStyles.HexNumber);
      var red = (0xFF0000 & parsed) >> 16;
      var green = (0xFF00 & parsed) >> 8;
      var blue = 0xFF & parsed;
      var alpha = (int)(colorAsText.Length == 9 ? (0xFF000000 & parsed) >> 24 : 255);
      if (premultiplied)
      {
        return new Color(red, green, blue, alpha);
      }

      var alphaFloat = alpha / 255f;
      return new Color(red, green, blue) * alphaFloat;
    }

    public object Parse(IStyleSystem styleSystem, XElement reader)
    {
      var colorAsText = (string)reader;
      if (string.IsNullOrWhiteSpace(colorAsText))
      {
        throw new StyleParseException("When providing a color, the text cannot be empty.", reader);
      }

      Color c;
      if (KnownColors.TryGetValue(colorAsText, out c))
      {
        return c;
      }

      if (colorAsText.StartsWith("#"))
      {
        bool? premultipliedFlag = (bool?)reader.AttributeLocal("premultiplied");
        return ParseFromString(colorAsText, premultipliedFlag ?? false);
      }

      throw new StyleParseException($"The color {colorAsText} is neither a known color or a hex-notation color literal.", reader);
    }

    public void Write(IStyleSystem styleSystem, XElement propertyElement, object value)
    {
      var c = (Color)value;

      foreach (var knownColor in KnownColors)
      {
        if (knownColor.Value.Equals(c))
        {
          propertyElement.Value = knownColor.Key;
          return;
        }
      }

      if (c.A == 255)
      {
        // fully opaque. 
        propertyElement.Value = $"#{c.R.ToString("X2")}{c.G.ToString("X2")}{c.B.ToString("X2")}";
      }
      else
      {
        propertyElement.SetAttributeValue("premultiplied", true);
        propertyElement.Value = $"#{c.A.ToString("X2")}{c.R.ToString("X2")}{c.G.ToString("X2")}{c.B.ToString("X2")}";
      }
    }

    static void RegisterColors()
    {
      var properties = typeof(Color).GetProperties();
      for (var i = 0; i < properties.Length; i++)
      {
        var property = properties[i];
        if (!property.CanRead)
        {
          continue;
        }
        if (property.GetIndexParameters().Length > 0)
        {
          continue;
        }
        if (!typeof(Color).IsAssignableFrom(property.PropertyType))
        {
          continue;
        }
        var getter = property.GetGetMethod();
        if (getter == null)
        {
          continue;
        }
        if (!getter.IsStatic)
        {
          continue;
        }
        try
        {
          var key = (Color)getter.Invoke(null, new object[0]);
          KnownColors.Add(property.Name.ToLowerInvariant(), key);
        }
        catch (Exception e)
        {
          Debug.WriteLine("Failed to read color for {0} [{1}]", property.Name, e);
        }
      }
    }
  }
}