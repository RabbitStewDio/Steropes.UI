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
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Steropes.UI.Styles.Io.Parser
{
  public static class StyleParserExtensions
  {
    /// <summary>
    ///  Parses style information from the given xml string. That parsing will 
    ///  attempt to preserve line-information to provide better error messages.
    /// </summary>
    /// <param name="p">The style parser instance</param>
    /// <param name="xmltext">The style document as well-formed XML text.</param>
    /// <param name="loadOptions">Defaults to preserve line-info and base-uri for cleaner error messages</param>
    /// <returns></returns>
    public static List<IStyleRule> Parse(this IStyleParser p, string xmltext, LoadOptions loadOptions = LoadOptions.SetLineInfo | LoadOptions.SetBaseUri)
    {
      return p.Read(XDocument.Load(new StringReader(xmltext), loadOptions));
    }

    /// <summary>
    ///  Parses style information by reading from the given file. That parsing will 
    ///  attempt to preserve line-information to provide better error messages.
    /// </summary>
    /// <param name="p">The style parser instance</param>
    /// <param name="file">The file name of the XML document to read.</param>
    /// <param name="loadOptions">Defaults to preserve line-info and base-uri for cleaner error messages</param>
    /// <returns></returns>
    public static List<IStyleRule> ReadFile(this IStyleParser p, string file, LoadOptions loadOptions = LoadOptions.SetLineInfo | LoadOptions.SetBaseUri)
    {
      return p.Read(XDocument.Load(file, loadOptions));
    }

    public static XElement ElementLocal(this XElement e, string localName)
    {
      return e.Elements().FirstOrDefault(ex => ex.Name.LocalName == localName);
    }
    public static XAttribute AttributeLocal(this XElement e, string localName)
    {
      return e.Attributes().FirstOrDefault(ex => ex.Name.LocalName == localName);
    }
  }
}