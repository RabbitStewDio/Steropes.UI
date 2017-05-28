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
namespace Steropes.UI.Widgets.TextWidgets.Documents.Helper
{
  public interface ITextProcessingRules
  {
    LineBreakType IsLineBreak(char previous, char current);

    WordBreakType IsWordBreak(char previous, char current);
  }

  public class TextProcessingRules : ITextProcessingRules
  {
    public LineBreakType IsLineBreak(char previous, char current)
    {
      if (current == 0)
      {
        return LineBreakType.None;
      }

      switch ((int)current)
      {
        // LF
        case 0xA:
          {
            return previous == 0xD ? LineBreakType.Continuation : LineBreakType.LineBreak;
          }

        // Vertical Tab
        case 0xB:

        // Form-Feed
        case 0xC:

        // CR
        case 0xD:

        // Next-Line
        case 0x85:

        // Line Separator
        case 0x2028:

        // Paragraph Separator
        case 0x2029:
          {
            return LineBreakType.LineBreak;
          }
        default:
          {
            return LineBreakType.None;
          }
      }
    }

    public WordBreakType IsWordBreak(char previous, char current)
    {
      return char.IsWhiteSpace(previous) && !char.IsWhiteSpace(current) ? WordBreakType.WordBreak : WordBreakType.None;
    }
  }
}