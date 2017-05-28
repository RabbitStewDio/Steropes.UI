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
using Steropes.UI.Styles.Io;
using Steropes.UI.Styles.Io.Values;

namespace Steropes.UI.Styles
{
  public class SubStyleSystem : IStyleSystem
  {
    readonly IStyleSystem parent;

    public SubStyleSystem(IStyleSystem parent, string context)
    {
      if (parent == null)
      {
        throw new ArgumentNullException(nameof(parent));
      }
      this.parent = parent;
      this.ContentLoader = new SubContextContentLoader(parent.ContentLoader, context);
    }

    public IContentLoader ContentLoader { get; }

    public void ConfigureStyleSerializer(IStyleSerializerConfiguration parser)
    {
      parent.ConfigureStyleSerializer(parser);
    }

    public IStyleKey<T> CreateKey<T>(string name, bool inherit)
    {
      return this.parent.CreateKey<T>(name, inherit);
    }

    public IPredefinedStyle CreatePredefinedStyle()
    {
      return this.parent.CreatePredefinedStyle();
    }

    public IStyle CreatePresentationStyle()
    {
      return this.parent.CreatePresentationStyle();
    }

    public bool IsRegisteredKey(IStyleKey key)
    {
      return this.parent.IsRegisteredKey(key);
    }

    public int LinearIndexFor(IStyleKey key)
    {
      return this.parent.LinearIndexFor(key);
    }

    public T StylesFor<T>() where T : IStyleDefinition, new()
    {
      return this.parent.StylesFor<T>();
    }

    public void RegisterSerializer(IStylePropertySerializer serializer)
    {
      parent.RegisterSerializer(serializer);
    }
  }
}