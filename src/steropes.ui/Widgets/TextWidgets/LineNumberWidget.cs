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

using Microsoft.Xna.Framework;

using Steropes.UI.Components;
using Steropes.UI.Platform;
using Steropes.UI.Styles;
using Steropes.UI.Widgets.Styles;
using Steropes.UI.Widgets.TextWidgets.Documents.PlainText;
using Steropes.UI.Widgets.TextWidgets.Documents.Views;

namespace Steropes.UI.Widgets.TextWidgets
{
  public class LineNumberWidget : Widget
  {
    readonly List<Tuple<int, string>> cachedTextPositions;

    readonly TextStyleDefinition textStyle;

    DocumentView<PlainTextDocument> documentView;

    public LineNumberWidget(IUIStyle style) : base(style)
    {
      textStyle = StyleSystem.StylesFor<TextStyleDefinition>();
      cachedTextPositions = new List<Tuple<int, string>>();
    }

    public DocumentView<PlainTextDocument> DocumentView
    {
      get
      {
        return documentView;
      }
      set
      {
        if (Equals(value, documentView))
        {
          return;
        }

        if (documentView != null)
        {
          documentView.LayoutInvalidated -= OnLayoutInvalidated;
        }
        documentView = value;
        if (documentView != null)
        {
          documentView.LayoutInvalidated += OnLayoutInvalidated;
        }

        OnPropertyChanged();
        InvalidateLayout();
      }
    }

    public IUIFont Font
    {
      get
      {
        return Style.GetValue(textStyle.Font);
      }
      set
      {
        Style.SetValue(textStyle.Font, value);
      }
    }

    public Color TextColor
    {
      get
      {
        return Style.GetValue(textStyle.TextColor);
      }
      set
      {
        Style.SetValue(textStyle.TextColor, value);
      }
    }

    int LineCount
    {
      get
      {
        if (DocumentView?.Document?.Root == null)
        {
          return 0;
        }
        return DocumentView.Document.Root.Count;
      }
    }

    protected override Rectangle ArrangeOverride(Rectangle layoutSize)
    {
      // nothing to arrange.
      return layoutSize;
    }

    protected override void DrawWidget(IBatchedDrawingService drawingService)
    {
      base.DrawWidget(drawingService);

      if (Font != null)
      {
        RebuildCache();
        var borderRect = BorderRect;
        for (var index = 0; index < cachedTextPositions.Count; index++)
        {
          var pos = cachedTextPositions[index];
          if (pos.Item1 == -1)
          {
            continue;
          }

          drawingService.DrawString(Font, pos.Item2, new Vector2(borderRect.X + Padding.Left, borderRect.Y + pos.Item1), TextColor);
        }
      }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
      var font = Font;
      if (font == null)
      {
        return new Size();
      }

      var lines = LineCount;
      var textSize = font.MeasureString(lines.ToString());
      return new Size(textSize.X, lines * textSize.Y);
    }

    protected override void OnLayoutInvalidated()
    {
      base.OnLayoutInvalidated();
      cachedTextPositions?.Clear();
    }

    void OnLayoutInvalidated(object sender, EventArgs e)
    {
      OnLayoutInvalidated();
    }

    void RebuildCache()
    {
      if (DocumentView?.Document?.Root == null)
      {
        cachedTextPositions.Clear();
        return;
      }

      if (cachedTextPositions.Count != LineCount)
      {
        cachedTextPositions.Clear();

        var root = DocumentView.Document.Root;
        var baseLine = Font.Baseline;
        var borderRect = BorderRect;
        for (var line = 0; line < LineCount; line += 1)
        {
          var lineNode = root[line];
          Rectangle bounds;
          if (DocumentView.ModelToView(lineNode.Offset, out bounds))
          {
            var pos = (int)(bounds.Y + baseLine) - borderRect.Y;
            var text = $"{line + 1}";
            cachedTextPositions.Add(Tuple.Create(pos, text));
          }
          else
          {
            cachedTextPositions.Add(Tuple.Create(-1, ""));
          }
        }
      }
    }
  }
}