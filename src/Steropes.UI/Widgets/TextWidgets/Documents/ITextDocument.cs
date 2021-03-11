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
using System.Text;

using Steropes.UI.Widgets.TextWidgets.Documents.Edits;

namespace Steropes.UI.Widgets.TextWidgets.Documents
{
  public interface ITextDocument
  {
    event EventHandler<TextModificationEventArgs> DocumentModified;

    event EventHandler<UndoableEditEventArgs> UndoableEditCreated;

    IDocumentContent Content { get; }

    ITextNode Root { get; }

    int TextLength { get; }

    void CopyInto(StringBuilder b, int offset, int length);

    ITextPosition CreatePosition(int offset, Bias bias);

    void DeleteAt(int offset, int length);

    void InsertAt(int offset, char text);

    void InsertAt(int offset, string text);

    bool PopFilter(out ITextDocumentFilter filter);

    void PushFilter(ITextDocumentFilter filter);

    string TextAt(int offset, int length);
  }

  public static class TextDocumentExtensions
  {
    public static string GetText(this ITextDocument doc)
    {
      return doc.TextAt(0, doc.TextLength);
    }

    public static void SetText(this ITextDocument doc, string text)
    {
      doc.DeleteAt(0, doc.TextLength);
      doc.InsertAt(0, text);
    }
  }
}