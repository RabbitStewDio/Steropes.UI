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
using Microsoft.Xna.Framework;

using Steropes.UI.Components;
using Steropes.UI.Widgets.TextWidgets.Documents;
using Steropes.UI.Widgets.TextWidgets.Documents.PlainText;
using Steropes.UI.Widgets.TextWidgets.Documents.Views;

namespace Steropes.UI.Widgets.TextWidgets
{
  /// <summary>
  ///   A single-line text box. This widget will ignore linebreaks. If you want to edit larger text,
  ///   use a TextArea.
  /// </summary>
  public class TextField : TextEditorWidgetBase<DocumentView<PlainTextDocument>, PlainTextDocument>
  {
    public TextField(IUIStyle style) : this(style, new PlainTextDocumentEditor(style))
    {
    }

    /// <summary>
    ///   For testing to override the document view used.
    /// </summary>
    /// <param name="style"></param>
    /// <param name="editor"></param>
    public TextField(IUIStyle style, IDocumentEditor<DocumentView<PlainTextDocument>, PlainTextDocument> editor) : base(style, editor)
    {
      WrapText = WrapText.None;
      Content.Document.PushFilter(new LineBreakFilter(Content.ViewFactory.TextProcessingRules));
      Content.Document.PushFilter(new MaxLengthFilter(Content.Document, () => MaxLength));
      Clip = true;
    }

    public bool CanBeEscapeCleared { get; set; }

    public bool EnableScrolling { get; set; } = true;

    public int MaxLength { get; set; }

    protected override Rectangle ArrangeOverride(Rectangle layoutSize)
    {
      if (!EnableScrolling || Content == null)
      {
        return base.ArrangeOverride(layoutSize);
      }

      Content.Arrange(layoutSize);
      var offset = EnsureCaretIsVisible(Content.LayoutRect);
      if (offset != Point.Zero)
      {
        // shift all content around to keep the cursor visible.
        var ls = layoutSize;
        ls.Location = layoutSize.Location + offset;
        ls.Width = ls.Width - offset.X;
        Content.Arrange(ls);
      }

      var retval = Content.LayoutRect;
      ArrangeCarets(retval);
      return layoutSize;
    }

    Point EnsureCaretIsVisible(Rectangle layoutSize)
    {
      Rectangle caretPosition;
      if (!Content.ModelToView(Caret.SelectionEndOffset, out caretPosition))
      {
        return new Point();
      }

      if (layoutSize.Contains(caretPosition.Location))
      {
        return new Point();
      }

      int x;

      // compute necessary offset to make caret visible.
      if (caretPosition.Left < layoutSize.Left)
      {
        x = layoutSize.Left - caretPosition.Left;
      }
      else if (caretPosition.Right > layoutSize.Right)
      {
        x = layoutSize.Right - caretPosition.Right;
      }
      else
      {
        x = 0;
      }

      return new Point(x, 0);
    }
  }
}