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
using System.Collections;
using System.Collections.Generic;

namespace Steropes.UI.Widgets.TextWidgets.Documents.Helper
{
  public struct BreakIterator<T> : IEnumerator<T>
  {
    readonly IDocumentContent content;

    readonly int startOffset;

    readonly int endOffset;

    readonly Func<char, char, T> rule;

    int cursor;

    public BreakIterator(IDocumentContent content, Func<char, char, T> rule, int startOffset, int endOffset)
    {
      this.content = content;
      this.rule = rule;
      this.startOffset = startOffset;
      this.endOffset = endOffset;
      this.cursor = startOffset - 1;
    }

    public void Dispose()
    {
    }

    public bool MoveNext()
    {
      if (cursor < endOffset)
      {
        cursor += 1;
      }
      return cursor < endOffset;
    }

    public void Reset()
    {
      cursor = startOffset - 1;
    }

    char Previous
    {
      get
      {
        var prevCursor = cursor - 1;
        if (prevCursor >= startOffset && prevCursor < endOffset)
        {
          return content[prevCursor];
        }
        return (char)0;
      }
    }

    public char CurrentChar => content[cursor];

    object IEnumerator.Current => Current;

    public T Current
    {
      get
      {
        // todo: Apply unicode rules here ..
        // http://unicode.org/reports/tr29/#Word_Boundaries
        // http://www.unicode.org/Public/UCD/latest/ucd/auxiliary/WordBreakProperty.txt
        if (cursor < startOffset)
        {
          throw new InvalidOperationException();
        }
        if (cursor >= endOffset)
        {
          throw new InvalidOperationException();
        }
        return rule(Previous, content[cursor]);
      }
    }
  }
}