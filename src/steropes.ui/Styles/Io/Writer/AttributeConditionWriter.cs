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

using Steropes.UI.Styles.Conditions;
using Steropes.UI.Styles.Io.Parser;
using Steropes.UI.Styles.Io.Values;

namespace Steropes.UI.Styles.Io.Writer
{
  public class AttributeConditionWriter : IConditionWriter
  {
    readonly List<IStylePropertySerializer> propertyParsers;

    public AttributeConditionWriter()
    {
      propertyParsers = new List<IStylePropertySerializer>();
    }

    public void Register(IStylePropertySerializer p)
    {
      propertyParsers.Add(p);
    }

    public void Write(IStyleSystem styleSystem, XContainer container, ICondition condition, IConditionWriter childWriter)
    {
      var idCond = (AttributeCondition)condition;
      var content = new XElement("attribute");
      content.Add(new XElement("name", idCond.Property));
      if (idCond.Value != null)
      {
        IStylePropertySerializer serializer;
        if (Find(idCond.Value, out serializer))
        {
          var valueElement = new XElement("value");
          serializer.Write(styleSystem, valueElement, idCond.Value);

          var id = serializer.TypeId ?? serializer.TargetType.FullName;
          content.Add(new XElement("type", id));
          content.Add(valueElement);
        }
        else
        {
          throw new StyleParseException("There is no handle for attribute-condition value of " + idCond.Value.GetType(), container);
        }
      }
      container.Add(content);
    }

    bool Find(object o, out IStylePropertySerializer serializer)
    {
      foreach (var p in propertyParsers)
      {
        if (p.TargetType.IsInstanceOfType(o))
        {
          serializer = p;
          return true;
        }
      }
      serializer = null;
      return false;
    }
  }
}