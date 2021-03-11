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
using System.Linq;
using Steropes.UI.Platform;
using Steropes.UI.Styles.Conditions;
using Steropes.UI.Styles.Selector;

namespace Steropes.UI.Styles
{
  public class StyleBuilder
  {
    public StyleBuilder(IStyleSystem styleSystem)
    {
      StyleSystem = styleSystem;
    }

    public IStyleSystem StyleSystem { get; }

    public IStyleRule CreateRule(IStyleSelector selector, IPredefinedStyleBuilder style)
    {
      return new StyleRule(selector, style.Style);
    }

    public IPredefinedStyleBuilder CreateStyle()
    {
      return new PredefinedStyleBuilder(StyleSystem);
    }

    public ElementSelector SelectForAll()
    {
      return new ElementSelector(null);
    }

    public ElementSelector SelectForType<T>()
    {
      return new ElementSelector(typeof(T).Name);
    }

    public ElementSelector SelectForType(string name)
    {
      return new ElementSelector(name);
    }

    public ElementSelector SelectForType(string name, params string[] names)
    {
      return new ElementSelector(name, names);
    }
  }

  public interface IPredefinedStyleBuilder
  {
    IContentLoader ContentLoader { get; }

    IPredefinedStyle Style { get; }

    IStyleSystem StyleSystem { get; }

    void SetValue<T>(IStyleKey<T> key, T value);
  }

  public class PredefinedStyleBuilder : IPredefinedStyleBuilder
  {
    public PredefinedStyleBuilder(IStyleSystem styleSystem)
    {
      StyleSystem = styleSystem;
      ContentLoader = styleSystem.ContentLoader;
      Style = StyleSystem.CreatePredefinedStyle();
    }

    public IContentLoader ContentLoader { get; }

    public IPredefinedStyle Style { get; }

    public IStyleSystem StyleSystem { get; }

    public void SetValue<T>(IStyleKey<T> key, T value)
    {
      Style.SetValue(key, value);
    }
  }

  public static class StyleBuilderExtensions
  {
    public static AndCondition And(this ICondition c, ICondition c2)
    {
      return new AndCondition(c, c2);
    }

    public static AttributeCondition HasAttribute(string c, object v = null)
    {
      return new AttributeCondition(c, v);
    }

    public static ClassCondition HasClass(string c)
    {
      return new ClassCondition(c);
    }

    public static IdCondition HasId(string c)
    {
      return new IdCondition(c);
    }

    public static PseudoClassCondition HasPseudoClass(string c)
    {
      return new PseudoClassCondition(c);
    }

    public static NotCondition Not(ICondition c)
    {
      return new NotCondition(c);
    }

    public static OrCondition Or(this ICondition c, ICondition c2)
    {
      return new OrCondition(c, c2);
    }

    public static IPredefinedStyleBuilder WithBox(this IPredefinedStyleBuilder style, IStyleKey<IBoxTexture> key, string value)
    {
      return WithBox(style, key, value, Insets.Zero, Insets.Zero);
    }

    public static IPredefinedStyleBuilder WithBox(this IPredefinedStyleBuilder style, IStyleKey<IBoxTexture> key, string value, Insets insets)
    {
      return WithBox(style, key, value, insets, Insets.Zero);
    }

    public static IPredefinedStyleBuilder WithBox(this IPredefinedStyleBuilder style, IStyleKey<IBoxTexture> key, string value, Insets insets, Insets margin)
    {
      style.SetValue(key, style.ContentLoader.LoadTexture(value, insets, margin));
      return style;
    }

    public static ConditionalSelector WithCondition(this ISimpleSelector selector, ICondition condition)
    {
      return new ConditionalSelector(selector, condition);
    }

    public static DescendantSelector WithDescendent(this IStyleSelector parentElement, ISimpleSelector innerChild)
    {
      return new DescendantSelector(innerChild, parentElement);
    }

    public static DescendantSelector WithDirectChild(this IStyleSelector parentElement, ISimpleSelector innerChild)
    {
      return new DescendantSelector(innerChild, parentElement, true);
    }

    public static IPredefinedStyleBuilder WithFont(this IPredefinedStyleBuilder style, IStyleKey<IUIFont> key, string value)
    {
      style.SetValue(key, style.ContentLoader.LoadFont(value));
      return style;
    }

    public static IPredefinedStyleBuilder WithTexture(this IPredefinedStyleBuilder style, IStyleKey<IUITexture> key, string value)
    {
      style.SetValue(key, style.ContentLoader.LoadTexture(value));
      return style;
    }

    public static IPredefinedStyleBuilder WithValue<T>(this IPredefinedStyleBuilder style, IStyleKey<T> key, T value)
    {
      style.SetValue(key, value);
      return style;
    }
  }
}