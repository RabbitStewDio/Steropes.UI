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
using System.Collections.ObjectModel;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Steropes.UI.Components;
using Steropes.UI.Platform;
using Steropes.UI.Styles;
using Steropes.UI.Widgets.Styles;

namespace Steropes.UI.Test
{
  public class Style
  {
    public IUIFont ExtraLargeFont { get; set; }

    public IUIFont LargeFont { get; set; }

    public IUIFont MediumFont { get; set; }

    public IUIFont ParagraphFont { get; set; }

    public IUIFont SmallFont { get; set; }
  }

  class TestStyleResolver : IStyleResolver
  {
    readonly IStyleResolver parent;
    readonly bool acceptAll;

    public TestStyleResolver(IStyleResolver parent, bool acceptAll)
    {
      this.parent = parent ?? throw new ArgumentNullException(nameof(parent));
      this.acceptAll = acceptAll;
    }

    public ObservableCollection<IStyleRule> StyleRules
    {
      get { return parent.StyleRules; }
    }

    public void AddRoot(IWidget root)
    {
      parent.AddRoot(root);
    }

    public IResolvedStyle CreateStyleFor(IWidget widget)
    {
      return parent.CreateStyleFor(widget);
    }

    public void RemoveRoot(IWidget root)
    {
      parent.RemoveRoot(root);
    }

    public bool Revalidate()
    {
      return parent.Revalidate();
    }

    public bool IsRegistered(IWidget widget)
    {
      if (acceptAll)
      {
        return true;
      }
      return parent.IsRegistered(widget);
    }
  }

  public class TestUIStyle : IUIStyle
  {
    public TestUIStyle(IContentLoader loader, Style style, bool acceptAll = true)
    {
      Style = style;
      StyleSystem = new StyleSystem(loader);
      StyleResolver = new TestStyleResolver(new StyleResolver(StyleSystem), acceptAll);
    }

    public Style Style { get; }

    public IStyleResolver StyleResolver { get; }

    public IStyleSystem StyleSystem { get; }
  }

  public class LayoutTestStyle
  {
    public static TestUIStyle Create(bool treatAllAsRegistered = true)
    {
      var legacyStyle = CreateStyle();
      var loader = new TestContentLoader(legacyStyle);
      var style = new TestUIStyle(loader, legacyStyle, treatAllAsRegistered);
      var styleRules = new StyleLoader().LoadRules(style.StyleSystem);
      var resolver = style.StyleResolver;
      foreach (var r in styleRules)
      {
        resolver.StyleRules.Add(r);
      }

      return style;
    }

    public static IBoxTexture CreateBoxTexture(string name, int width, int height, Insets border, Insets margin)
    {
      var bounds = new Rectangle(0, 0, Math.Max(width, border.Horizontal), Math.Max(height, border.Vertical));
      return new TestUITexture(bounds, border, margin, name);
    }

    public static IUIFont CreateFont(string name = null, int width = 11, int height = 12, int lineSpacing = 15)
    {
      if (name == null)
      {
        name = $"Direct[{width},{height}]";
      }

      var font = new TestSystemFont();
      font.LineSpacing = lineSpacing;
      font.Spacing = 0;
      font.Name = name;
      font.Width = width;
      font.Height = height;
      return font;
    }

    public static IStyleSystem CreateStyleSystem()
    {
      var legacyStyle = CreateStyle();
      var loader = new TestContentLoader(legacyStyle);
      return new StyleSystem(loader);
    }

    public static IStyle CreateTextStyle(IStyleSystem styleSystem)
    {
      var textStyles = styleSystem.StylesFor<TextStyleDefinition>();

      var style = new PresentationStyle(styleSystem);
      style.SetValue(textStyles.Font, CreateFont("CreateTextStyle[11,16]", 11, 16));
      style.SetValue(textStyles.TextColor, Color.Black);
      return style;
    }

    public static IUITexture CreateTexture(string name, int width, int height)
    {
      var bounds = new Rectangle(0, 0, width, height);
      return new TestUITexture(bounds, Insets.Zero, Insets.Zero, name);
    }

    static Style CreateStyle()
    {
      var style = new Style
                    {
                      // VerticalScrollbar = CreateBoxTexture(10, 20),
                      ExtraLargeFont = CreateFont("Fonts/ExtraLargeFont", 19, 26),
                      LargeFont = CreateFont("Fonts/LargeFont", 15, 20),
                      MediumFont = CreateFont("Fonts/MediumFont", 11, 16),
                      SmallFont = CreateFont("Fonts/SmallFont", 8, 12),
                      ParagraphFont = CreateFont("Fonts/MediumMonoFont", 11, 16)
                    };
      return style;
    }

    public class TestContentLoader : IContentLoader
    {
      readonly Dictionary<string, IUIFont> fonts;

      readonly Style style;

      public TestContentLoader(Style style)
      {
        this.style = style;
        fonts = new Dictionary<string, IUIFont>();
        fonts.Add(style.ParagraphFont.Name, style.ParagraphFont);
        fonts.Add(style.ExtraLargeFont.Name, style.ExtraLargeFont);
        fonts.Add(style.LargeFont.Name, style.LargeFont);
        fonts.Add(style.MediumFont.Name, style.MediumFont);
        fonts.Add(style.SmallFont.Name, style.SmallFont);
        WhitePixel = CreateTexture("WhitePixel", 1, 1);
      }

      public IUITexture WhitePixel { get; }

      public string ContextPath => "";

      public string FullContextPath { get; }

      public IUIFont LoadFont(string font)
      {
        IUIFont f;
        if (fonts.TryGetValue(font, out f))
        {
          return f;
        }

        throw new ArgumentException($"Font {font}");
      }

      public IUITexture LoadTexture(string texture)
      {
        return CreateTexture(texture, 50, 50);
      }

      public IBoxTexture LoadTexture(string texture, Insets border, Insets margin)
      {
        return CreateBoxTexture(texture, 0, 0, border, margin);
      }
    }

    class TestSystemFont : IUIFont, IEquatable<TestSystemFont>
    {
      public float Baseline => Height * 0.75f;

      public int Height { get; set; }

      public int LineSpacing { get; set; }

      public string Name { get; set; }

      public float Spacing { get; set; }

      public SpriteFont SpriteFont
      {
        get
        {
          throw new NotImplementedException();
        }
      }

      public int Width { get; set; }

      public int YOffset { get; set; }

      public static bool operator ==(TestSystemFont left, TestSystemFont right)
      {
        return Equals(left, right);
      }

      public static bool operator !=(TestSystemFont left, TestSystemFont right)
      {
        return !Equals(left, right);
      }

      public bool Equals(TestSystemFont other)
      {
        if (ReferenceEquals(null, other))
        {
          return false;
        }
        if (ReferenceEquals(this, other))
        {
          return true;
        }
        if (Width != other.Width)
        {
          return false;
        }
        if (Height != other.Height)
        {
          return false;
        }
        if (LineSpacing != other.LineSpacing)
        {
          return false;
        }
        if (!Spacing.Equals(other.Spacing))
        {
          return false;
        }
        if (YOffset != other.YOffset)
        {
          return false;
        }
        return string.Equals(Name, other.Name);
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
        return Equals((TestSystemFont)obj);
      }

      public override int GetHashCode()
      {
        unchecked
        {
          var hashCode = Width;
          hashCode = (hashCode * 397) ^ Height;
          hashCode = (hashCode * 397) ^ LineSpacing;
          hashCode = (hashCode * 397) ^ Spacing.GetHashCode();
          hashCode = (hashCode * 397) ^ YOffset;
          hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
          return hashCode;
        }
      }

      public Vector2 MeasureString(string text)
      {
        return new Vector2(text.Length * Width, Height);
      }

      public Vector2 MeasureString(StringBuilder text)
      {
        return new Vector2(text.Length * Width, Height);
      }
    }

    class TestUITexture : IBoxTexture, IEquatable<TestUITexture>
    {
      public TestUITexture(Rectangle bounds, Insets cornerArea, Insets margins, string name = null)
      {
        Bounds = bounds;
        CornerArea = cornerArea;
        Margins = margins;
        Name = name;
        Texture = null;
      }

      public Rectangle Bounds { get; }

      public IUITexture Rebase(Texture2D texture, Rectangle bounds, string name)
      {
        return new TestUITexture(bounds, CornerArea, Margins, name);
      }

      public Insets CornerArea { get; }

      public int Height => Bounds.Height;

      public Insets Margins { get; }

      public string Name { get; }

      public Texture2D Texture { get; }

      public int Width => Bounds.Width;

      public static bool operator ==(TestUITexture left, TestUITexture right)
      {
        return Equals(left, right);
      }

      public static bool operator !=(TestUITexture left, TestUITexture right)
      {
        return !Equals(left, right);
      }

      public bool Equals(TestUITexture other)
      {
        if (ReferenceEquals(null, other))
        {
          return false;
        }
        if (ReferenceEquals(this, other))
        {
          return true;
        }
        return string.Equals(Name, other.Name) && Width == other.Width && Height == other.Height && CornerArea.Equals(other.CornerArea) && Margins.Equals(other.Margins);
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
        return Equals((TestUITexture)obj);
      }

      public override int GetHashCode()
      {
        unchecked
        {
          var hashCode = Name != null ? Name.GetHashCode() : 0;
          hashCode = (hashCode * 397) ^ Width;
          hashCode = (hashCode * 397) ^ Height;
          hashCode = (hashCode * 397) ^ CornerArea.GetHashCode();
          hashCode = (hashCode * 397) ^ Margins.GetHashCode();
          return hashCode;
        }
      }

      public override string ToString()
      {
        return $"UITexture={{Name: {Name}, Width: {Width}, Height: {Height}}}";
      }
    }
  }
}