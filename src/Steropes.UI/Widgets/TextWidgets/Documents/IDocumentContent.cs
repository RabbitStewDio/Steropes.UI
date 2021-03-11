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
using Steropes.UI.Widgets.TextWidgets.Documents.Helper;

namespace Steropes.UI.Widgets.TextWidgets.Documents
{
  public interface IDocumentContent
  {
    int Length { get; }

    char this[int index] { get; }

    void CopyInto(StringBuilder stringBuilder, int offset, int length);

    ITextPosition CreatePosition(int offset, Bias bias);

    IUndoableEdit Insert(int offset, string text);

    IUndoableEdit Insert(int offset, char text);

    IUndoableEdit Remove(int offset, int length);

    string TextAt(int offset, int length);
  }

  public class StringDocumentContent : IDocumentContent
  {
    readonly PositionCollection positions;

    public StringDocumentContent()
    {
      Buffer = new StringBuilder();
      positions = new PositionCollection();
    }

    public StringBuilder Buffer { get; }

    public int Length => Buffer.Length;

    public char this[int index] => Buffer[index];

    public void Clear()
    {
      Remove(0, Length);
    }

    public void CopyInto(StringBuilder stringBuilder, int offset, int length)
    {
      stringBuilder.Clear();
      var endOffset = Math.Min(offset + length, Buffer.Length);
      for (var idx = offset; idx < endOffset; idx += 1)
      {
        stringBuilder.Append(Buffer[idx]);
      }
    }

    public ITextPosition CreatePosition(int offset, Bias bias)
    {
      return positions.Create(offset, bias);
    }

    public void EnsureCapacity(int length)
    {
      Buffer.EnsureCapacity(length);
    }

    public IUndoableEdit Insert(int offset, string text)
    {
      Buffer.Insert(offset, text);
      positions.InsertAt(offset, text.Length);
      return new InsertStringEdit(this, offset, text);
    }

    public IUndoableEdit Insert(int offset, char text)
    {
      Buffer.Insert(offset, text);
      positions.InsertAt(offset, 1);
      return new InsertCharEdit(this, offset, text);
    }

    public IUndoableEdit Remove(int offset, int length)
    {
      var text = Buffer.ToString(offset, length);
      Buffer.Remove(offset, length);
      positions.RemoveAt(offset, length);
      return new RemoveEdit(this, offset, text);
    }

    public string TextAt(int offset, int length)
    {
      return Buffer.ToString(offset, length);
    }

    class InsertCharEdit : UndoableEditBase
    {
      readonly IDocumentContent content;

      readonly int offset;

      readonly char text;

      public InsertCharEdit(IDocumentContent content, int offset, char text) : base(true, "Insert " + text)
      {
        this.content = content;
        this.offset = offset;
        this.text = text;
      }

      protected override void PerformRedo()
      {
        content.Insert(offset, text);
      }

      protected override void PerformUndo()
      {
        content.Remove(offset, 1);
      }
    }

    class InsertStringEdit : UndoableEditBase
    {
      readonly IDocumentContent content;

      readonly int offset;

      readonly string text;

      public InsertStringEdit(IDocumentContent content, int offset, string text) : base(true, "Insert " + text)
      {
        this.content = content;
        this.offset = offset;
        this.text = text;
      }

      protected override void PerformRedo()
      {
        content.Insert(offset, text);
      }

      protected override void PerformUndo()
      {
        content.Remove(offset, text.Length);
      }
    }

    class RemoveEdit : UndoableEditBase
    {
      readonly IDocumentContent content;

      readonly int offset;

      readonly string text;

      public RemoveEdit(IDocumentContent content, int offset, string text) : base(true, "Insert " + text)
      {
        this.content = content;
        this.offset = offset;
        this.text = text;
      }

      protected override void PerformRedo()
      {
        content.Remove(offset, text.Length);
      }

      protected override void PerformUndo()
      {
        content.Insert(offset, text);
      }
    }
  }
}