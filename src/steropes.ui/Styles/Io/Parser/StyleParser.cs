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
using System.Linq;
using System.Xml.Linq;
using Steropes.UI.Components;
using Steropes.UI.Styles.Conditions;
using Steropes.UI.Styles.Io.Values;
using Steropes.UI.Styles.Selector;
using Steropes.UI.Widgets;
using Steropes.UI.Widgets.TextWidgets;

namespace Steropes.UI.Styles.Io.Parser
{
  /// <summary>
  ///   This class loads style rules from XML files. Unlike more modern methods, XML parsers are
  ///   built into the runtime and thus require no external libraries.
  /// </summary>
  public class StyleParser : IStyleParser
  {
    public static readonly XNamespace StyleNamespace = "http://www.steropes-ui.org/namespaces/style/1.0";

    readonly Dictionary<Type, IStylePropertySerializer> propertyParsers;

    readonly Dictionary<string, IStyleKey> registeredKeys;

    readonly Dictionary<string, IStylePropertySerializer> typeParsers;

    public StyleParser(IStyleSystem styleSystem)
    {
      registeredKeys = new Dictionary<string, IStyleKey>();
      propertyParsers = new Dictionary<Type, IStylePropertySerializer>();
      typeParsers = new Dictionary<string, IStylePropertySerializer>();
      StyleSystem = styleSystem;

      RegisterPropertyParsers(new BoolValueStylePropertySerializer());
      RegisterPropertyParsers(new ColorValueStylePropertySerializer());
      RegisterPropertyParsers(new FloatValueStylePropertySerializer());
      RegisterPropertyParsers(new IntValueStylePropertySerializer());
      RegisterPropertyParsers(new StringValueStylePropertySerializer());
      RegisterPropertyParsers(new InsetsStylePropertySerializer());
    }

    public IStyleSystem StyleSystem { get; }

    public List<IStyleRule> Read(XDocument document)
    {
      if (document.Root == null)
      {
        throw new ArgumentNullException();
      }

      ValidateSetup();

      var rules = new List<IStyleRule>();
      var x = from e in document.Root.Elements() where e.Name.LocalName == "style" select e;
      foreach (var node in x)
      {
        ReadStyleRule(node, rules);
      }
      return rules;
    }

    public void RegisterPropertyParsers(IStylePropertySerializer serializer)
    {
      propertyParsers[serializer.TargetType] = serializer;
      if (serializer.TypeId != null)
      {
        typeParsers[serializer.TypeId] = serializer;
      }
      typeParsers[serializer.TargetType.FullName] = serializer;
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
        var key = (IStyleKey) getter?.Invoke(styleDefinitions, new object[0]);
        if (key != null && StyleSystem.IsRegisteredKey(key))
        {
          registeredKeys[key.Name] = key;
        }
      }
    }

    IStyleSelector CreateSelector(XElement style, IStyleSelector parent, bool directChild, string element)
    {
      if (string.IsNullOrWhiteSpace(element) || "*".Equals(element))
      {
        element = null;
      }

      ISimpleSelector es = new ElementSelector(element);
      var c = style.ElementLocal("conditions");
      if (c != null)
      {
        var cond = ParseAndCondition(c);
        es = new ConditionalSelector(es, cond);
      }

      if (parent != null)
      {
        return new DescendantSelector(es, parent, directChild);
      }

      return es;
    }

    IStylePropertySerializer LookupPropertyParser(Type propertyType, XObject context)
    {
      IStylePropertySerializer p;
      if (propertyParsers.TryGetValue(propertyType, out p))
      {
        return p;
      }
      throw new StyleParseException($"Unable to locate a property serializer for type {propertyType.FullName}",
        context);
    }

    IStyleKey LookupStyleKey(string name, XObject context)
    {
      IStyleKey p;
      if (registeredKeys.TryGetValue(name, out p))
      {
        return p;
      }
      throw new StyleParseException(
        $"There is no style-key with name '{name}' defined in the style system. Did you forget to register a style-definition?",
        context);
    }

    ICondition ParseAndCondition(XElement s)
    {
      ICondition c = null;
      foreach (var e in s.Elements())
      {
        var x = ParseCondition(e);
        c = c == null ? x : new AndCondition(c, x);
      }

      return c;
    }

    ICondition ParseCondition(XElement s)
    {
      var localName = s.Name.LocalName;
      if (localName == "or")
      {
        return ParseOrCondition(s);
      }
      if (localName == "not")
      {
        return ParseNotCondition(s);
      }
      if (localName == "id")
      {
        return new IdCondition(s.Value);
      }
      if (localName == "class")
      {
        return new ClassCondition(s.Value);
      }
      if (localName == "pseudo-class")
      {
        return new PseudoClassCondition(s.Value);
      }
      if (localName == "attribute")
      {
        var name = s.ElementLocal("name")?.Value;
        if (name == null)
        {
          throw new StyleParseException("Attribute 'name' is mandatory when declaring an attribute-condition.", s);
        }

        var value = s.ElementLocal("value");
        if (value == null)
        {
          return new AttributeCondition(name, null);
        }

        var type = s.ElementLocal("type")?.Value;
        if (type == null)
        {
          throw new StyleParseException(
            "Attribute 'type' is mandatory when declaring an attribute-condition with a value comparison.", s);
        }

        IStylePropertySerializer serializer;
        if (typeParsers.TryGetValue(type, out serializer))
        {
          return new AttributeCondition(name, serializer.Parse(StyleSystem, value));
        }
      }

      throw new StyleParseException($"Unable to handle condition type {localName}", s);
    }

    NotCondition ParseNotCondition(XElement s)
    {
      return new NotCondition(ParseAndCondition(s));
    }

    ICondition ParseOrCondition(XElement s)
    {
      ICondition c = null;
      foreach (var e in s.Elements())
      {
        var x = ParseCondition(e);
        c = c == null ? x : new OrCondition(c, x);
      }

      return c;
    }

    KeyValuePair<IStyleKey, object> ReadProperty(XElement reader)
    {
      var name = reader.AttributeLocal("name")?.Value;
      if (string.IsNullOrWhiteSpace(name))
      {
        throw new StyleParseException("A property element must provide the name of a style-key.", reader);
      }

      var key = LookupStyleKey(name, reader);
      object result;
      if (string.Equals(reader.AttributeLocal("inherit")?.Value, "true", StringComparison.InvariantCulture))
      {
        result = InheritMarker.Inherited;
      }
      else
      {
        var type = reader.AttributeLocal("type")?.Value;
        IStylePropertySerializer serializer = null;
        if (type != null)
        {
          typeParsers.TryGetValue(type, out serializer);
        }
        if (serializer == null)
        {
          serializer = LookupPropertyParser(key.ValueType, reader);
        }

        result = serializer.Parse(StyleSystem, reader);
      }

      return new KeyValuePair<IStyleKey, object>(key, result);
    }

    void ReadStyleRule(XElement reader, List<IStyleRule> rules, IStyleSelector parent = null, bool directChild = false)
    {
      var element = reader.AttributeLocal("element")?.Value;
      var selector = CreateSelector(reader, parent, directChild, element);

      var style = new PredefinedStyle(StyleSystem);

      var hasStyle = false;
      foreach (var propertyNode in reader.Elements().Where(pn => pn.Name.LocalName == "property"))
      {
        var p = ReadProperty(propertyNode);
        style.SetValue(p.Key, p.Value);
        hasStyle = true;
      }

      if (hasStyle)
      {
        var rule = new StyleRule(selector, style);
        rules.Add(rule);
      }

      foreach (var propertyNode in reader.Elements().Where(pn => pn.Name.LocalName == "style"))
      {
        var attr = propertyNode.AttributeLocal("direct-child");
        ReadStyleRule(propertyNode, rules, selector, attr?.Value == "true");
      }
    }

    void ValidateSetup()
    {
      if (registeredKeys.Count == 0)
      {
        throw new StyleParseException("No style-keys registered. Parsing will never succeed.");
      }

      foreach (var k in registeredKeys.Values)
      {
        if (!propertyParsers.ContainsKey(k.ValueType))
        {
          throw new StyleParseException(
            $"No value parser registered for type {k.ValueType}. It wont be possible to define values for this type.");
        }
      }
    }
  }
}