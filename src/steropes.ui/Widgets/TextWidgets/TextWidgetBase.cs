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

using Microsoft.Xna.Framework;

using Steropes.UI.Components;
using Steropes.UI.Platform;
using Steropes.UI.Styles;
using Steropes.UI.Widgets.Styles;
using Steropes.UI.Widgets.TextWidgets.Documents;
using Steropes.UI.Widgets.TextWidgets.Documents.Views;

namespace Steropes.UI.Widgets.TextWidgets
{
  public abstract class TextWidgetBase<TView, TDocument> : ContentWidget<TView>
    where TDocument : ITextDocument where TView : class, IDocumentView<TDocument>
  {
    readonly TextStyleDefinition textStyles;

    protected TextWidgetBase(IUIStyle style, IDocumentEditor<TView, TDocument> editor) : base(style)
    {
      textStyles = StyleSystem.StylesFor<TextStyleDefinition>();

      DocumentEditor = editor;
      Content = DocumentEditor.CreateDocumentView(AnchoredRect.CreateTopAnchored());
      WrapText = WrapText.Auto;
    }

    public Alignment Alignment
    {
      get
      {
        return Style.GetValue(textStyles.Alignment);
      }
      set
      {
        if (Alignment == value)
        {
          return;
        }

        Style.SetValue(textStyles.Alignment, value);
        InvalidateLayout();
      }
    }

    public IUIFont Font
    {
      get
      {
        return Style.GetValue(textStyles.Font);
      }

      set
      {
        if (Equals(Font, value))
        {
          return;
        }

        Style.SetValue(textStyles.Font, value);
        InvalidateLayout();
      }
    }

    public Color OutlineColor
    {
      get
      {
        return Style.GetValue(textStyles.OutlineColor);
      }
      set
      {
        if (OutlineColor == value)
        {
          return;
        }

        Style.SetValue(textStyles.OutlineColor, value);
        InvalidateLayout();
      }
    }

    public float OutlineRadius
    {
      get
      {
        return Style.GetValue(textStyles.OutlineSize);
      }
      set
      {
        if (Math.Abs(OutlineRadius - value) < 0.0005)
        {
          return;
        }

        Style.SetValue(textStyles.OutlineSize, value);
        InvalidateLayout();
      }
    }

    public virtual string Text
    {
      get
      {
        return Content.Document.GetText();
      }

      set
      {
        var oldText = Content.Document.GetText();
        if (oldText != value)
        {
          Content.Document.SetText(value);
          InvalidateLayout();
        }
      }
    }

    public Color TextColor
    {
      get
      {
        return Style.GetValue(textStyles.TextColor);
      }
      set
      {
        if (TextColor == value)
        {
          return;
        }

        Style.SetValue(textStyles.TextColor, value);
        InvalidateLayout();
      }
    }

    public bool Underline
    {
      get
      {
        return Style.GetValue(textStyles.Underline);
      }
      set
      {
        if (Underline == value)
        {
          return;
        }

        Style.SetValue(textStyles.Underline, value);
        InvalidateLayout();
      }
    }

    public WrapText WrapText
    {
      get
      {
        return Style.GetValue(textStyles.WrapText);
      }
      set
      {
        if (WrapText == value)
        {
          return;
        }

        Style.SetValue(textStyles.WrapText, value);
        InvalidateLayout();
      }
    }

    IDocumentEditor<TView, TDocument> DocumentEditor { get; }
  }
}