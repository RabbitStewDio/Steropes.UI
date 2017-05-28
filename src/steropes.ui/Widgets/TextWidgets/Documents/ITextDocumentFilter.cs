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

using Steropes.UI.Widgets.TextWidgets.Documents.Helper;

namespace Steropes.UI.Widgets.TextWidgets.Documents
{
  public interface ITextDocumentFilter
  {
    void DeleteAt(int offset, int length, ITextDocumentFilterChain bypass);

    void InsertAt(int offset, char text, ITextDocumentFilterChain bypass);

    void InsertAt(int offset, string text, ITextDocumentFilterChain bypass);
  }

  public class CharTextDocumentFilter : ITextDocumentFilter
  {
    readonly StringBuilder buffer;

    public CharTextDocumentFilter(Func<char, bool> filter)
    {
      if (filter == null)
      {
        throw new ArgumentNullException();
      }

      Filter = filter;
      buffer = new StringBuilder();
    }

    Func<char, bool> Filter { get; }

    public static bool ControlCharacterFilter(char input)
    {
      return char.IsControl(input) == false;
    }

    public static bool FloatFilter(char input)
    {
      return (input >= '0' && input <= '9') || input == '.' || input == '-';
    }

    public static bool IntegerFilter(char input)
    {
      return (input >= '0' && input <= '9') || input == '-';
    }

    public void DeleteAt(int offset, int length, ITextDocumentFilterChain bypass)
    {
      bypass.DeleteAt(offset, length);
    }

    public void InsertAt(int offset, char text, ITextDocumentFilterChain bypass)
    {
      if (Filter(text))
      {
        bypass.InsertAt(offset, text);
      }
    }

    public void InsertAt(int offset, string text, ITextDocumentFilterChain bypass)
    {
      buffer.Clear();
      buffer.EnsureCapacity(text.Length);
      for (var index = 0; index < text.Length; index++)
      {
        var c = text[index];
        if (Filter(c))
        {
          buffer.Append(c);
        }
      }
      if (buffer.Length > 0)
      {
        bypass.InsertAt(offset, buffer.ToString());
      }
    }
  }

  public class LineBreakFilter : ITextDocumentFilter
  {
    readonly StringBuilder buffer;

    readonly StringDocumentContent bufferContent;

    readonly ITextProcessingRules rules;

    public LineBreakFilter(ITextProcessingRules rules)
    {
      this.rules = rules;
      this.bufferContent = new StringDocumentContent();
      this.buffer = new StringBuilder();
    }

    public void DeleteAt(int offset, int length, ITextDocumentFilterChain bypass)
    {
      bypass.DeleteAt(offset, length);
    }

    public void InsertAt(int offset, char text, ITextDocumentFilterChain bypass)
    {
      if (rules.IsLineBreak('\0', text) == LineBreakType.None)
      {
        bypass.InsertAt(offset, text);
      }
    }

    public void InsertAt(int offset, string text, ITextDocumentFilterChain bypass)
    {
      bufferContent.Clear();
      bufferContent.EnsureCapacity(text.Length);
      bufferContent.Insert(0, text);

      buffer.Clear();
      buffer.EnsureCapacity(text.Length);

      var breaker = new BreakIterator<LineBreakType>(bufferContent, rules.IsLineBreak, 0, bufferContent.Length);
      while (breaker.MoveNext())
      {
        if (breaker.Current == LineBreakType.None)
        {
          buffer.Append(breaker.CurrentChar);
        }
      }

      if (buffer.Length > 0)
      {
        bypass.InsertAt(offset, buffer.ToString());
      }
    }
  }

  public class MaxLengthFilter : ITextDocumentFilter
  {
    readonly ITextDocument document;

    readonly Func<int> maxLengthFunction;

    public MaxLengthFilter(ITextDocument document, Func<int> maxLengthFunction)
    {
      this.document = document;
      this.maxLengthFunction = maxLengthFunction;
    }

    public void DeleteAt(int offset, int length, ITextDocumentFilterChain bypass)
    {
      bypass.DeleteAt(offset, length);
    }

    public void InsertAt(int offset, char text, ITextDocumentFilterChain bypass)
    {
      var ml = maxLengthFunction();
      if (ml > 0)
      {
        if (document.TextLength + 1 < ml)
        {
          bypass.InsertAt(offset, text);
        }
      }
      else
      {
        bypass.InsertAt(offset, text);
      }
    }

    public void InsertAt(int offset, string text, ITextDocumentFilterChain bypass)
    {
      var ml = maxLengthFunction();
      if (ml > 0)
      {
        var remainingSpace = Math.Max(0, ml - document.TextLength);
        if (remainingSpace > 0)
        {
          bypass.InsertAt(offset, text.Substring(0, Math.Min(text.Length, remainingSpace)));
        }
      }
      else
      {
        bypass.InsertAt(offset, text);
      }
    }
  }
}