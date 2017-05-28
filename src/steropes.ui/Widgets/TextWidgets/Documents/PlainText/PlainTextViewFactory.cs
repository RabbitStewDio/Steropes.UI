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
using Steropes.UI.Styles;
using Steropes.UI.Widgets.TextWidgets.Documents.Helper;
using Steropes.UI.Widgets.TextWidgets.Documents.Views;

namespace Steropes.UI.Widgets.TextWidgets.Documents.PlainText
{
  public class PlainTextViewFactory : ITextNodeViewFactory<PlainTextDocument>
  {
    public PlainTextViewFactory()
    {
      TextProcessingRules = new TextProcessingRules();
    }

    public ITextProcessingRules TextProcessingRules { get; }

    public virtual ITextChunkView<PlainTextDocument> CreateChunkFor(ITextNode node, IStyle style)
    {
      var offset = node.Document.CreatePosition(node.Offset, Bias.Forward);
      var endOffset = node.Document.CreatePosition(node.EndOffset, Bias.Backward);
      var chunkView = new TextChunkView<PlainTextDocument>(TextProcessingRules, node, style, offset, endOffset);
      chunkView.Initialize();
      return chunkView;
    }

    public virtual ITextView<PlainTextDocument> CreateFor(ITextNode node, IStyle style)
    {
      if (node.Leaf)
      {
        return new ParagraphTextView<PlainTextDocument>(node, style, this);
      }
      return new BlockTextView<PlainTextDocument>(node, style);
    }
  }
}