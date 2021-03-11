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
using System.Xml.Linq;

using Steropes.UI.Styles.Io.Parser;
using Steropes.UI.Util;

namespace Steropes.UI.Styles.Io.Writer
{
  public static class StyleWriterExtensions
  {
    public static IEnumerable<XElement> Merge(this IEnumerable<XElement> ruleElements)
    {
      var nodesBySelector = new CollectionDictionary<Selector, XElement, List<XElement>>();
      foreach (var rule in ruleElements)
      {
        var selector = new Selector(rule);
        nodesBySelector.Add(selector, rule);
      }

      var result = new List<XElement>();
      foreach (var k in nodesBySelector)
      {
        if (k.Value.Count >= 1)
        {
          var first = k.Value[0];
          for (var i = 1; i < k.Value.Count; i++)
          {
            var node = k.Value[i];
            node.Elements("property").ForEach(first.Add);
            node.Elements("style").ForEach(first.Add);
            if (node.Parent != null)
            {
              node.Remove();
            }
          }

          first.Elements("style").Merge();
          result.Add(first);
        }
      }
      return result;
    }

    public struct Selector : IEquatable<Selector>
    {
      readonly string element;

      readonly string directChild;

      readonly XElement condition;

      public Selector(XElement style)
      {
        this.element = style.AttributeLocal("element")?.Value;
        this.directChild = style.AttributeLocal("direct-child")?.Value;
        this.condition = style.ElementLocal("conditions");
      }

      public bool Equals(Selector other)
      {
        if (!Equals(this.element, other.element))
        {
          return false;
        }
        if (!Equals(this.directChild, other.directChild))
        {
          return false;
        }
        return XNode.DeepEquals(this.condition, other.condition);
      }

      public override bool Equals(object obj)
      {
        if (ReferenceEquals(null, obj))
        {
          return false;
        }
        return obj is Selector && this.Equals((Selector)obj);
      }

      public override int GetHashCode()
      {
        unchecked
        {
          var hashCode = this.element?.GetHashCode() ?? 0;
          hashCode = (hashCode * 397) ^ (this.directChild?.GetHashCode() ?? 0);

          // XElements have no stable hashcode implementation.
          return hashCode;
        }
      }

      public static bool operator ==(Selector left, Selector right)
      {
        return left.Equals(right);
      }

      public static bool operator !=(Selector left, Selector right)
      {
        return !left.Equals(right);
      }
    }
  }
}