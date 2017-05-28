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

using FluentAssertions;

using Microsoft.Xna.Framework;

using Steropes.UI.Components;
using Steropes.UI.Widgets.Styles;
using Steropes.UI.Widgets.TextWidgets.Documents;
using Steropes.UI.Widgets.TextWidgets.Documents.PlainText;
using Steropes.UI.Widgets.TextWidgets.Documents.Views;

namespace Steropes.UI.Test.UI.TextWidgets.Documents.PlainText
{
  public static class NodeTestExtensions
  {
    public static List<T> ExtractFromChildren<T>(this ITextView<PlainTextDocument> view, Func<ITextView<PlainTextDocument>, T> extractor)
    {
      var retval = new List<T>();
      for (var i = 0; i < view.Count; i++)
      {
        var c = view[i];
        retval.Add(extractor(c));
      }
      return retval;
    }

    public static Point RecordPosition<T>(this ITextView<T> p, int offset) where T : ITextDocument
    {
      Rectangle rect;
      p.ModelToView(offset, out rect).Should().Be(true);
      return rect.Location;
    }

    public static TestDocumentView<PlainTextDocument> SetUp(Alignment alignment = Alignment.Start, string text = null)
    {
      var doc = new PlainTextDocument();
      doc.InsertAt(0, text ?? "Hello World, Here I am. Long text ahead here. This is the first paragraph.\nAfter a line break, we should see a second paragraph in the document.");

      var style = LayoutTestStyle.Create();
      var textStyles = style.StyleSystem.StylesFor<TextStyleDefinition>();
      var documentView = new TestDocumentView<PlainTextDocument>(new PlainTextDocumentEditor(style));
      documentView.Style.SetValue(textStyles.Alignment, alignment);
      documentView.Style.SetValue(textStyles.Font, style.Style.MediumFont);
      documentView.Document = doc;
      return documentView;
    }
  }

  public class TestDocumentViewEditor<TDocument> : IDocumentEditor<DocumentView<TDocument>, TDocument>
    where TDocument : ITextDocument
  {
    readonly IDocumentEditor<DocumentView<TDocument>, TDocument> backend;

    public TestDocumentViewEditor(IDocumentEditor<DocumentView<TDocument>, TDocument> backend)
    {
      this.backend = backend;
    }

    [Obsolete]
    public IUIStyle Style => backend.Style;

    public TDocument CreateDocument() => backend.CreateDocument();

    public DocumentView<TDocument> CreateDocumentView(AnchoredRect? anchoredRect = default(AnchoredRect?))
    {
      return new TestDocumentView<TDocument>(this);
    }

    public ITextNodeViewFactory<TDocument> CreateViewFactory() => backend.CreateViewFactory();
  }

  public class TestDocumentView<TDocument> : DocumentView<TDocument>
    where TDocument : ITextDocument
  {
    public TestDocumentView(IDocumentEditor<DocumentView<TDocument>, TDocument> editor) : base(editor)
    {
    }

    public ITextView<TDocument> ExposedRootView => RootView;
  }
}