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

using Steropes.UI.Widgets.TextWidgets.Documents;

namespace Steropes.UI.Widgets.TextWidgets
{
  public static class CaretExtensions
  {
    public static void ClearSelection(this ICaret caret)
    {
      caret.MoveTo(caret.SelectionEndOffset);
    }

    public static bool HasSelection(this ICaret c)
    {
      return c.SelectionStartOffset != c.SelectionEndOffset;
    }

    public static void SelectAll(this ICaret caret)
    {
      caret.MoveTo(0);
      caret.Select(caret.MaximumOffset);
    }

    public static string TextForSelection(this ICaret caret, ITextDocument doc)
    {
      if (caret.MaximumOffset != doc.TextLength)
      {
        throw new ArgumentException();
      }

      var start = Math.Min(caret.SelectionStartOffset, caret.SelectionEndOffset);
      var end = Math.Max(caret.SelectionStartOffset, caret.SelectionEndOffset);
      return doc.TextAt(start, end - start);
    }
  }
}