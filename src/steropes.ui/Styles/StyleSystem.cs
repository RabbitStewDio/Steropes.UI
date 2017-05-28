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
using Steropes.UI.Components;
using Steropes.UI.Styles.Io;
using Steropes.UI.Styles.Io.Values;
using Steropes.UI.Widgets;
using Steropes.UI.Widgets.Styles;
using Steropes.UI.Widgets.TextWidgets;

namespace Steropes.UI.Styles
{
  public class StyleSystem : IStyleSystem
  {
    readonly Dictionary<Type, IStyleDefinition> instances;

    readonly Dictionary<string, IStyleKey> registeredKeys;

    readonly List<IStylePropertySerializer> serializers;

    public StyleSystem(IContentLoader contentLoader)
    {
      this.ContentLoader = contentLoader;
      this.registeredKeys = new Dictionary<string, IStyleKey>();
      this.instances = new Dictionary<Type, IStyleDefinition>();
      this.serializers = new List<IStylePropertySerializer>();
    }

    public IContentLoader ContentLoader { get; }

    public void RegisterSerializer(IStylePropertySerializer serializer)
    {
      this.serializers.Add(serializer);
    }
    
    public void ConfigureStyleSerializer(IStyleSerializerConfiguration parser)
    {
      foreach (var d in this.CreateDefaultStyles())
      {
        parser.RegisterStyles(d);
      }

      foreach (var d in CreateDefaultPropertyParsers())
      {
        parser.RegisterPropertyParsers(d);
      }

      foreach (var d in serializers)
      {
        parser.RegisterPropertyParsers(d);
      }
    }

    public IStyleKey<T> CreateKey<T>(string name, bool inherit)
    {
      if (this.registeredKeys.ContainsKey(name))
      {
        throw new ArgumentException();
      }

      var styleKey = new StyleKey<T>(this, this.registeredKeys.Count, name, typeof(T), inherit);
      this.registeredKeys.Add(name, styleKey);
      return styleKey;
    }

    public IPredefinedStyle CreatePredefinedStyle()
    {
      return new PredefinedStyle(this);
    }

    public IStyle CreatePresentationStyle()
    {
      return new PresentationStyle(this);
    }

    public bool IsRegisteredKey(IStyleKey key)
    {
      var sk = key as SecretStyleKey;
      if (sk == null)
      {
        return false;
      }
      return ReferenceEquals(sk.Creator, this);
    }

    public int LinearIndexFor(IStyleKey key)
    {
      var sk = key as SecretStyleKey;
      if (sk == null)
      {
        throw new ArgumentException();
      }
      return sk.LinearIndex;
    }

    public T StylesFor<T>() where T : IStyleDefinition, new()
    {
      IStyleDefinition def;
      if (this.instances.TryGetValue(typeof(T), out def))
      {
        return (T)def;
      }

      var created = new T();
      created.Init(this);
      this.instances.Add(typeof(T), created);
      return created;
    }

    static IEnumerable<IStylePropertySerializer> CreateDefaultPropertyParsers()
    {
      var defs = new List<IStylePropertySerializer>
      {
        new UIFontStylePropertySerializer(),
        new UITextureStylePropertySerializer(),
        new BoxTextureStylePropertySerializer(),
        new EnumStylePropertySerializer(typeof(Alignment)),
        new EnumStylePropertySerializer(typeof(ScrollbarMode)),
        new EnumStylePropertySerializer(typeof(Visibility)),
        new EnumStylePropertySerializer(typeof(WrapText)),
        new EnumStylePropertySerializer(typeof(SelectionState)),
        new EnumStylePropertySerializer(typeof(TooltipPositionMode)),
        new EnumStylePropertySerializer(typeof(ScaleMode)),
      };
      return defs;
    }

    IEnumerable<IStyleDefinition> CreateDefaultStyles()
    {
      var defs = new List<IStyleDefinition>
                   {
                     this.StylesFor<BoxStyleDefinition>(),
                     this.StylesFor<ButtonStyleDefinition>(),
                     this.StylesFor<IconLabelStyleDefinition>(),
                     this.StylesFor<ImageStyleDefinition>(),
                     this.StylesFor<ListViewStyleDefinition>(),
                     this.StylesFor<NotebookStyleDefinition>(),
                     this.StylesFor<ScrollbarStyleDefinition>(),
                     this.StylesFor<TextStyleDefinition>(),
                     this.StylesFor<WidgetStyleDefinition>()
                   };
      return defs;
    }

    class SecretStyleKey
    {
      protected SecretStyleKey(IStyleSystem creator, int linearIndex)
      {
        this.Creator = creator;
        this.LinearIndex = linearIndex;
      }

      public IStyleSystem Creator { get; }

      public int LinearIndex { get; }
    }

    class StyleKey<T> : SecretStyleKey, IStyleKey<T>, IEquatable<StyleKey<T>>
    {
      public StyleKey(IStyleSystem creator, int index, string name, Type valueType, bool inherit) : base(creator, index)
      {
        if (name == null)
        {
          throw new ArgumentNullException(nameof(name));
        }
        if (valueType == null)
        {
          throw new ArgumentNullException(nameof(valueType));
        }
        this.Name = name;
        this.ValueType = valueType;
        this.Inherit = inherit;
      }

      public bool Inherit { get; }

      public string Name { get; }

      public Type ValueType { get; }

      public static bool operator ==(StyleKey<T> left, StyleKey<T> right)
      {
        return Equals(left, right);
      }

      public static bool operator !=(StyleKey<T> left, StyleKey<T> right)
      {
        return !Equals(left, right);
      }

      public bool Equals(StyleKey<T> other)
      {
        if (ReferenceEquals(null, other))
        {
          return false;
        }
        if (ReferenceEquals(this, other))
        {
          return true;
        }
        return string.Equals(this.Name, other.Name) && this.ValueType == other.ValueType;
      }

      public override bool Equals(object obj)
      {
        if (ReferenceEquals(null, obj))
        {
          return false;
        }
        if (ReferenceEquals(this, obj))
        {
          return true;
        }
        if (obj.GetType() != this.GetType())
        {
          return false;
        }
        return this.Equals((StyleKey<T>)obj);
      }

      public override int GetHashCode()
      {
        unchecked
        {
          return (this.Name.GetHashCode() * 397) ^ this.ValueType.GetHashCode();
        }
      }

      public override string ToString()
      {
        return $"StyleKey={{Name: {this.Name}, ValueType: {this.ValueType}, Inherit: {this.Inherit}}}";
      }
    }
  }
}