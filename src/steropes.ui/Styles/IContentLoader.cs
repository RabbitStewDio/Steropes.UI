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

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using Steropes.UI.Platform;

namespace Steropes.UI.Styles
{
  public interface IContentLoader
  {
    string ContextPath { get; }

    string FullContextPath { get; }

    IUIFont LoadFont(string font);

    IUITexture LoadTexture(string texture);

    IBoxTexture LoadTexture(string texture, Insets border, Insets margin);
  }

  public class ContentLoader : IContentLoader
  {
    readonly IFontFactory fontFactory;

    readonly ContentManager mgr;

    public ContentLoader(ContentManager mgr, GraphicsDevice dev)
    {
      this.mgr = mgr;
      fontFactory = new FontFactory(mgr, dev);
    }

    public string ContextPath => "";

    public string FullContextPath => "";

    public IUIFont LoadFont(string font)
    {
      return fontFactory.LoadFont(font);
    }

    public IUITexture LoadTexture(string name)
    {
      if (name == null)
      {
        return new UITexture(null);
      }
      return new UITexture(mgr.Load<Texture2D>(name));
    }

    public IBoxTexture LoadTexture(string texture, Insets border, Insets margin)
    {
      if (texture == null)
      {
        return new BoxTexture(null, border, margin);
      }
      return new BoxTexture(mgr.Load<Texture2D>(texture), border, margin);
    }
  }

  public class SubContextContentLoader : IContentLoader
  {
    readonly IContentLoader parent;

    public SubContextContentLoader(IContentLoader parent, string contextPath)
    {
      FullContextPath = Combine(parent.FullContextPath, contextPath);
      ContextPath = contextPath;
      this.parent = parent;
    }

    public string ContextPath { get; }

    public string FullContextPath { get; }

    public static string Combine(string contextPath, string subContext)
    {
      if (string.IsNullOrEmpty(contextPath))
      {
        return subContext;
      }
      if (string.IsNullOrEmpty(subContext))
      {
        return contextPath;
      }
      if (subContext.StartsWith("/", StringComparison.InvariantCulture))
      {
        return subContext.Substring(1);
      }

      if (contextPath.EndsWith("/", StringComparison.InvariantCulture))
      {
        return contextPath + subContext;
      }
      return contextPath + "/" + subContext;
    }

    public IUIFont LoadFont(string font)
    {
      return parent.LoadFont(Combine(ContextPath, font));
    }

    public IUITexture LoadTexture(string texture)
    {
      if (texture == null)
      {
        return parent.LoadTexture(null);
      }
      return parent.LoadTexture(Combine(ContextPath, texture));
    }

    public IBoxTexture LoadTexture(string texture, Insets border, Insets margin)
    {
      if (texture == null)
      {
        return parent.LoadTexture(null, border, margin);
      }
      return parent.LoadTexture(Combine(ContextPath, texture), border, margin);
    }
  }
}