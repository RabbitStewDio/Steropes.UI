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

using Microsoft.Xna.Framework;

using Steropes.UI.Components;
using Steropes.UI.Platform;
using Steropes.UI.Styles;
using Steropes.UI.Widgets.TextWidgets.Documents.Helper;

namespace Steropes.UI.Widgets.TextWidgets.Documents.Views
{
  /// <summary>
  ///   A view representing a chunk of text. This can be a sub-set of the leaf-node's full text to facilitate line breaking.
  /// </summary>
  public class TextChunkView<TDocument> : TextView<TDocument>, ITextChunkView<TDocument>
    where TDocument : ITextDocument
  {
    readonly ITextProcessingRules rules;

    string buffer;

    string displayText;

    Vector2 maxSize;

    public TextChunkView(ITextProcessingRules rules, ITextNode node, IStyle style, ITextPosition offset, ITextPosition endOffset) : base(node, style)
    {
      this.rules = rules;
      OffsetPosition = offset;
      EndOffsetPosition = endOffset;
    }

    public float BaseLine => Font.Baseline;

    public override int EndOffset => EndOffsetPosition.Offset;

    public ITextPosition EndOffsetPosition { get; }

    public int EndOffsetWithoutLineBreaks => EndOffset - HardTrimAtEnd;

    public override int Offset => OffsetPosition.Offset;

    public ITextPosition OffsetPosition { get; }

    public int StretchWeight { get; private set; }

    public string Text => TextFor(Offset, EndOffset);

    /// <summary>
    ///   Public for testing
    /// </summary>
    protected int HardTrimAtEnd { get; private set; }

    /// <summary>
    ///   Public for testing
    /// </summary>
    protected int TrimAtEnd { get; private set; }

    /// <summary>
    ///   Public for testing
    /// </summary>
    protected int TrimAtStart { get; private set; }

    float MaxHeight
    {
      get
      {
        if (float.IsInfinity(maxSize.X))
        {
          maxSize = Font.MeasureString(displayText);
          maxSize.Y = Font.LineSpacing;
        }
        return maxSize.Y;
      }
    }

    float MaxWidth
    {
      get
      {
        if (float.IsInfinity(maxSize.X))
        {
          maxSize = Font.MeasureString(displayText);
          maxSize.Y = Font.LineSpacing;
        }
        return maxSize.X;
      }
    }

    public int TrimmedEndOffset => EndOffsetPosition.Offset - TrimAtEnd;

    public int TrimmedStartOffset => OffsetPosition.Offset + TrimAtStart;

    /// <summary>
    ///   Computes the nearest valid break offset for the given width, and attempts to break this chunk
    ///   into two independent text-chunks. The first chunk will contain the text that should remain on
    ///   the current line, the second chunk contains the content that should be pushed to the next
    ///   line. 
    /// </summary>
    /// <param name="width">The width position.</param>
    /// <param name="first">The left hand chunk, never null.</param>
    /// <param name="second">The right hand chunk or null if the content did not break.</param>
    public void BreakAt(float width, out ITextChunkView<TDocument> first, out ITextChunkView<TDocument> second)
    {
      var breakPoint = FindWordBreakOffset(width);
      BreakAtOffset(breakPoint, out first, out second);
    }
    
    public void BreakAtOffset(int breakPoint, out ITextChunkView<TDocument> first, out ITextChunkView<TDocument> second)
    {
      // is the break within the leading whitespace content? If so, dont break.
      if (breakPoint <= TrimmedStartOffset)
      {
        first = this;
        second = null;
        return;
      }

      // is the break within the trailing whitespace content? If so, dont break.
      if (breakPoint >= TrimmedEndOffset)
      {
        first = this;
        second = null;
        return;
      }

      first = SubChunk(OffsetPosition, Node.Document.CreatePosition(breakPoint, Bias.Backward));
      second = SubChunk(Node.Document.CreatePosition(breakPoint, Bias.Forward), EndOffsetPosition);
    }

    public override void Draw(IBatchedDrawingService drawingService)
    {
      var x = LayoutRect.X;
      var y = LayoutRect.Y + Padding.Top;
      var width = DesiredSize.Width;
      var font = Font;
      var textColor = Style.GetValue(TextStyles.TextColor);
      var outlineColor = Style.GetValue(TextStyles.OutlineColor);
      var outlineRadius = Style.GetValue(TextStyles.OutlineSize);
      var underline = Style.GetValue(TextStyles.Underline);
      var strikeThrough = Style.GetValue(TextStyles.StrikeThrough);
      drawingService.DrawBlurredText(font, displayText, new Vector2(x + Padding.Left, y + font.Baseline), textColor, outlineRadius, outlineColor);
      if (underline)
      {
        var bottomLeft = new Vector2(x, y + font.LineSpacing + font.Baseline);
        drawingService.DrawLine(bottomLeft, bottomLeft + new Vector2(width, 0), textColor);
      }
      if (strikeThrough)
      {
        var bottomLeft = new Vector2(x, y + font.LineSpacing / 2f + font.Baseline);
        drawingService.DrawLine(bottomLeft, bottomLeft + new Vector2(width, 0), textColor);
      }
    }

    public void Initialize()
    {
      var length = EndOffsetPosition.Offset - OffsetPosition.Offset;
      buffer = Node.Document.TextAt(OffsetPosition.Offset, length);

      TrimAtStart = TextProcessing.CountFromStart(buffer, 0, length, TextProcessing.CanTrim);
      TrimAtEnd = TextProcessing.CountFromEnd(buffer, TrimAtStart, length, TextProcessing.CanTrim);

      HardTrimAtEnd = TextProcessing.CountFromEnd(buffer, 0, length, rules.IsInvalidCursorPosition);
      maxSize = new Vector2(float.NegativeInfinity, float.NegativeInfinity);
      StretchWeight = 0;
      Padding = ComputePaddings();

      displayText = TextFor(TrimmedStartOffset, TrimmedEndOffset);
    }

    public override bool ModelToView(int offset, out Rectangle result)
    {
      if (LayoutInvalid)
      {
        result = default(Rectangle);
        return false;
      }
      if (EndOffset == offset)
      {
        // special case to allow to place the cursor at the end of the line and to insert
        // text after the last visible character.
        result = new Rectangle(LayoutRect.Right, LayoutRect.Y, 0, Font.LineSpacing);
        return true;
      }

      if (!Node.InRange(offset))
      {
        result = default(Rectangle);
        return false;
      }

      var start = WidthFor(Offset, offset);
      var end = WidthFor(Offset, offset + 1);

      result = new Rectangle(LayoutRect.X + (int)start, LayoutRect.Y, (int)(end - start), Font.LineSpacing);
      return true;
    }

    public override NavigationResult Navigate(int editOffset, Direction direction, out int targetOffset)
    {
      if (LayoutInvalid)
      {
        targetOffset = -1;
        return NavigationResult.Invalid;
      }

      switch (direction)
      {
        case Direction.Right:
        case Direction.Down:
          {
            if (editOffset >= EndOffset)
            {
              targetOffset = -1;
              return NavigationResult.Invalid;
            }
            if (editOffset < Offset)
            {
              targetOffset = Offset;
              return NavigationResult.Valid;
            }

            targetOffset = editOffset + 1;
            if (targetOffset == EndOffset)
            {
              return NavigationResult.BoundaryChanged;
            }

            return NavigationResult.Valid;
          }
        case Direction.Left:
        case Direction.Up:
          {
            if (editOffset > EndOffset)
            {
              targetOffset = EndOffset;
              return NavigationResult.Valid;
            }
            if (editOffset <= Offset)
            {
              targetOffset = -1;
              return NavigationResult.Invalid;
            }

            targetOffset = editOffset - 1;
            if (targetOffset == Offset)
            {
              return NavigationResult.BoundaryChanged;
            }
            return NavigationResult.Valid;
          }
        default:
          {
            throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
          }
      }
    }

    public override void OnNodeContentChanged(IDocumentView<TDocument> docView, TextModificationEventArgs args)
    {
      Initialize();
      InvalidateLayout();
    }

    public override void OnNodeStructureChanged(IDocumentView<TDocument> docView, IElementEdit edit)
    {
      Initialize();
      InvalidateLayout();
    }

    public TextChunkView<TDocument> SubChunk(ITextPosition start, ITextPosition end)
    {
      if (start.Bias != Bias.Forward)
      {
        throw new ArgumentException();
      }
      if (end.Bias != Bias.Backward)
      {
        throw new ArgumentException();
      }
      var subChunk = new TextChunkView<TDocument>(rules, Node, Style, start, end);
      subChunk.Initialize();
      return subChunk;
    }

    public virtual string TextFor(int offset, int endOffset)
    {
      // text buffer is valid within the range of the node. 
      // buffer-offset tells us the number of characters to disregard at the start of the buffer.
      var length = endOffset - offset;
      return buffer.Substring(offset - Offset, length);
    }

    public override bool ViewToModel(Point position, out int offset, out Bias bias)
    {
      if (LayoutInvalid)
      {
        offset = -1;
        bias = Bias.Forward;
        return false;
      }

      if (!LayoutRect.Contains(position))
      {
        offset = -1;
        bias = Bias.Forward;
        return false;
      }

      var targetWidth = position.X - LayoutRect.X;
      offset = FindEditPositionBreakOffset(targetWidth);
      if (offset == EndOffset)
      {
        bias = Bias.Backward;
        return true;
      }

      var offsetWidth = WidthFor(Offset, offset);
      var nextWidth = WidthFor(Offset, offset + 1);
      var width = nextWidth - offsetWidth;
      bias = targetWidth - offsetWidth < width / 2 ? Bias.Backward : Bias.Forward;
      return true;
    }

    protected override Rectangle ArrangeOverride(Rectangle layoutSize)
    {
      var x = layoutSize.X;
      var y = layoutSize.Y;
      var width = (int)(Alignment == Alignment.Fill ? Math.Max(layoutSize.Width, MaxWidth) : MaxWidth);
      var height = (int)MaxHeight;
      return new Rectangle(x, y, width, height);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
      return new Size(MaxWidth, MaxHeight);
    }

    protected override void OnLayoutInvalidated()
    {
      maxSize = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
      base.OnLayoutInvalidated();
    }

    Insets ComputePaddings()
    {
      var left = (int)WidthFor(Offset, TrimmedStartOffset);
      var right = (int)WidthFor(TrimmedEndOffset, EndOffset);
      return new Insets(0, left, 0, right);
    }

    int FindBreakOffset(float targetPosition, Func<char, char, bool> breakEvaluator)
    {
      var start = Offset;
      var end = EndOffset - HardTrimAtEnd;

      var it = new BreakIterator<bool>(Node.Document.Content, breakEvaluator, start, end);
      var cursor = start;
      var lastGoodBreak = cursor;
      while (it.MoveNext())
      {
        if (it.Current == false)
        {
          cursor += 1;
          continue;
        }

        var width = WidthFor(start, cursor);
        if (width < targetPosition)
        {
          lastGoodBreak = cursor;
        }
        else if (lastGoodBreak == start || Math.Abs(width - targetPosition) < 0.5)
        {
          return cursor;
        }
        else
        {
          return lastGoodBreak;
        }
        cursor += 1;
      }
      return lastGoodBreak;
    }

    int FindEditPositionBreakOffset(float targetPosition)
    {
      return FindBreakOffset(targetPosition, rules.IsValidCursorPosition);
    }

    int FindWordBreakOffset(float targetPosition)
    {
      return FindBreakOffset(targetPosition, rules.IsWordBreakPosition);
    }

    float WidthFor(int offset, int positionWithinDocument)
    {
      return Font.MeasureString(TextFor(offset, positionWithinDocument)).X;
    }
  }
}