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
using System.Linq;
using System.Xml.Linq;

using Steropes.UI.Components;
using Steropes.UI.Styles.Io.Parser;
using Steropes.UI.Styles.Io.Values;
using Steropes.UI.Styles.Selector;
using Steropes.UI.Util;
using Steropes.UI.Widgets;
using Steropes.UI.Widgets.TextWidgets;

namespace Steropes.UI.Styles.Io.Writer
{
  public interface IStyleWriter : IStyleSerializerConfiguration
  {
    IStyleSystem StyleSystem { get; }

    XDocument Write(IEnumerable<IStyleRule> rules);

    XElement WriteRule(IStyleRule r);
  }

  public class StyleWriter : IStyleWriter
  {
    readonly ConditionWriter conditionWriter;

    readonly Dictionary<Type, IStylePropertySerializer> propertyParsers;

    readonly Dictionary<string, IStyleKey> registeredKeys;

    public StyleWriter(IStyleSystem styleSystem)
    {
      StyleSystem = styleSystem;
      registeredKeys = new Dictionary<string, IStyleKey>();
      propertyParsers = new Dictionary<Type, IStylePropertySerializer>();
      conditionWriter = new ConditionWriter();

      RegisterPropertyParsers(new BoolValueStylePropertySerializer());
      RegisterPropertyParsers(new ColorValueStylePropertySerializer());
      RegisterPropertyParsers(new FloatValueStylePropertySerializer());
      RegisterPropertyParsers(new IntValueStylePropertySerializer());
      RegisterPropertyParsers(new StringValueStylePropertySerializer());
      RegisterPropertyParsers(new InsetsStylePropertySerializer());
    }

    public IStyleSystem StyleSystem { get; }

    public void RegisterPropertyParsers(IStylePropertySerializer p)
    {
      conditionWriter.Register(p);
      propertyParsers.Add(p.TargetType, p);
    }

    public void RegisterStyles(IStyleDefinition styleDefinitions)
    {
      var properties = styleDefinitions.GetType().GetProperties();
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
        if (!typeof(IStyleKey).IsAssignableFrom(property.PropertyType))
        {
          continue;
        }
        var getter = property.GetGetMethod();
        var key = (IStyleKey)getter?.Invoke(styleDefinitions, new object[0]);
        if (key != null && StyleSystem.IsRegisteredKey(key))
        {
          registeredKeys[key.Name] = key;
        }
      }
    }

    public XDocument Write(IEnumerable<IStyleRule> rules)
    {
      var root = new XElement(StyleParser.StyleNamespace + "styles");
      root.Add(new XAttribute("xmlns", StyleParser.StyleNamespace));
      rules.Select(WriteRule).Merge().ForEach(root.Add);

      var doc = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"));
      doc.Add(root);
      return doc;
    }

    public XElement WriteRule(IStyleRule r)
    {
      var element = new XElement("style");
      var innerStyle = ProcessSelector(element, r.Selector);

      foreach (var pair in registeredKeys)
      {
        object raw;
        if (r.Style.GetValue(pair.Value, out raw))
        {
          var propertyElement = new XElement("property");
          propertyElement.Add(new XAttribute("name", pair.Key));
          if (InheritMarker.IsInheritMarker(raw))
          {
            propertyElement.Add(new XAttribute("inherit", "true"));
          }
          else
          {
            IStylePropertySerializer p;
            if (propertyParsers.TryGetValue(pair.Value.ValueType, out p))
            {
              p.Write(StyleSystem, propertyElement, raw);
            }
            else
            {
              throw new StyleWriterException("Key " + pair.Key + " was not registered.");
            }
          }
          innerStyle.Add(propertyElement);
        }
      }

      return element;
    }

    List<Tuple<bool, ISimpleSelector>> ExtractSelectors(DescendantSelector s)
    {
      var retval = new List<Tuple<bool, ISimpleSelector>>();
      while (true)
      {
        retval.Add(Tuple.Create(s.DirectChild, s.Selector));
        if (s.AnchestorSelector is DescendantSelector)
        {
          s = (DescendantSelector)s.AnchestorSelector;
        }
        else if (s.AnchestorSelector is ISimpleSelector)
        {
          retval.Add(Tuple.Create(true, (ISimpleSelector)s.AnchestorSelector));
          return retval;
        }
        else
        {
          throw new StyleWriterException();
        }
      }
    }

    XElement ProcessSelector(XElement style, IStyleSelector selector)
    {
      if (selector is DescendantSelector)
      {
        var ds = (DescendantSelector)selector;
        var extractSelectors = ExtractSelectors(ds);
        extractSelectors.Reverse();
        var first = true;
        foreach (var tuple in extractSelectors)
        {
          if (!first)
          {
            var inner = new XElement("style");
            style.Add(inner);
            style = inner;

            style.Add(new XAttribute("direct-child", tuple.Item1));
          }
          else
          {
            first = false;
          }

          ProcessSimpleSelector(style, tuple.Item2);
        }

        return style;
      }

      if (selector is ISimpleSelector)
      {
        ProcessSimpleSelector(style, (ISimpleSelector)selector);
        return style;
      }

      throw new StyleWriterException();
    }

    void ProcessSimpleSelector(XElement style, ISimpleSelector selector)
    {
      if (selector is ConditionalSelector)
      {
        var cs = (ConditionalSelector)selector;
        var conditions = new XElement("conditions");
        conditionWriter.Write(StyleSystem, conditions, cs.Condition, conditionWriter);
        style.Add(conditions);
        selector = cs.Selector;
      }

      if (selector is ElementSelector)
      {
        var es = (ElementSelector)selector;
        style.Add(new XAttribute("element", es.TypeName));
      }
    }
  }
}