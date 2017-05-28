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
using System.Xml.Linq;
using Steropes.UI.Components;
using Steropes.UI.Styles.Io.Parser;
using Steropes.UI.Styles.Io.Writer;

namespace Steropes.UI.Styles
{
  public static class StyleSystemExtensions
  {
    public static IStyleParser CreateParser(this IStyleSystem s)
    {
      var parser = new StyleParser(s);
      s.ConfigureStyleSerializer(parser);
      return parser;
    }

    public static IStyleWriter CreateWriter(this IStyleSystem s)
    {
      var w = new StyleWriter(s);
      s.ConfigureStyleSerializer(w);
      return w;
    }

    public static IStyleSystem WithContext(this IStyleSystem s, string context)
    {
      if (string.IsNullOrEmpty(context))
      {
        return s;
      }
      return new SubStyleSystem(s, context);
    }

    public static List<IStyleRule> LoadStyles(this IUIStyle style, string styleFile, string context)
    {
      return style.StyleSystem.WithContext(context).CreateParser().ReadFile(styleFile);
    }
  }
}