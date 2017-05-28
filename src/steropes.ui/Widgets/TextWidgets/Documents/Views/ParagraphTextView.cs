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
using Microsoft.Xna.Framework;

using Steropes.UI.Components;
using Steropes.UI.Platform;
using Steropes.UI.Styles;
using Steropes.UI.Util;

namespace Steropes.UI.Widgets.TextWidgets.Documents.Views
{
  /// <summary>
  ///   A paragraph view is a block-element that contains several lines of text. When the alignment is simple, all chunks are directly
  ///   added as child nodes, potentially after being broken into line-length chunks. For justified text we have to break the paragraph
  ///   into words (a more expensive operation) so that we can adjust the spacing between nodes.
  /// 
  ///   This design should be reworked to simply track the wordbreaks/spaces as list of offsets in the normal TextChunkView here. That
  ///   view in coordination with the paragraph would then be responsible for justifying text. 
  /// </summary>
  public class ParagraphTextView<TDocument> : TextView<TDocument>
    where TDocument : ITextDocument
  {
    readonly List<PositionedChunk> lineBreakContent;

    readonly List<LineMetrics> lineMetrics;

    readonly List<ITextChunkView<TDocument>> rawContent;

    readonly ITextNodeViewFactory<TDocument> viewFactory;

    float lineWidth;

    public ParagraphTextView(ITextNode node, IStyle style, ITextNodeViewFactory<TDocument> viewFactory) : base(node, style)
    {
      this.viewFactory = viewFactory;
      lineMetrics = new List<LineMetrics>();
      rawContent = new List<ITextChunkView<TDocument>>();
      lineBreakContent = new List<PositionedChunk>();
      lineWidth = -1;
      Dirty = true;
    }

    public override int Count => lineBreakContent.Count;

    public override ITextView<TDocument> this[int index] => lineBreakContent[index].Chunk;

    public override void Draw(IBatchedDrawingService drawingService)
    {
      for (var index = 0; index < lineBreakContent.Count; index++)
      {
        var chunk = lineBreakContent[index];
        var view = chunk.Chunk;
        view.Draw(drawingService);
      }
    }

    public override bool ModelToView(int offset, out Rectangle result)
    {
      if (LayoutInvalid)
      {
        result = default(Rectangle);
        return false;
      }
      if (!Node.InEndOffsetRange(offset))
      {
        result = default(Rectangle);
        return false;
      }

      result = LayoutRect;
      for (var index = 0; index < lineBreakContent.Count; index++)
      {
        var chunk = lineBreakContent[index];
        var view = chunk.Chunk;
        if (view.InRange(offset))
        {
          return view.ModelToView(offset, out result);
        }
        if (view.InEndOffsetRange(offset))
        {
          view.ModelToView(offset, out result);
        }
      }

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
          {
            for (var i = 0; i < Count; i += 1)
            {
              var child = this[i];
              if (editOffset > child.EndOffset)
              {
                continue;
              }
              var navResult = child.Navigate(editOffset, direction, out targetOffset);
              switch (navResult)
              {
                case NavigationResult.Valid:
                  {
                    return NavigationResult.Valid;
                  }
                case NavigationResult.Invalid:
                  {
                    break;
                  }
                case NavigationResult.BoundaryChanged:
                  {
                    editOffset = targetOffset;
                    break;
                  }
                default:
                  throw new ArgumentOutOfRangeException();
              }
            }

            targetOffset = EndOffset;
            return NavigationResult.BoundaryChanged;
          }
        case Direction.Left:
          {
            for (var i = Count - 1; i >= 0; i -= 1)
            {
              var child = this[i];
              if (editOffset < child.Offset)
              {
                continue;
              }
              var navResult = child.Navigate(editOffset, direction, out targetOffset);
              switch (navResult)
              {
                case NavigationResult.Valid:
                  {
                    return NavigationResult.Valid;
                  }
                case NavigationResult.Invalid:
                  {
                    break;
                  }
                case NavigationResult.BoundaryChanged:
                  {
                    editOffset = targetOffset;
                    break;
                  }
                default:
                  throw new ArgumentOutOfRangeException();
              }
            }

            targetOffset = Offset;
            return NavigationResult.BoundaryChanged;
          }
        case Direction.Up:
        case Direction.Down:
          {
            return this.NavigateVerticalFlat(editOffset, direction, out targetOffset);
          }
        default:
          {
            throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
          }
      }
    }

    public override void OnNodeContentChanged(IDocumentView<TDocument> docView, TextModificationEventArgs args)
    {
      InvalidateLayout();
    }

    public override void OnNodeStructureChanged(IDocumentView<TDocument> docView, IElementEdit edit)
    {
      Dirty = true;
      InvalidateLayout();
    }

    public override bool ViewToModel(Point position, out int offset, out Bias bias)
    {
      if (LayoutInvalid)
      {
        offset = -1;
        bias = Bias.Forward;
        return false;
      }

      var lineStartOffset = -1;
      var lineStartPosition = int.MaxValue;
      var lineEndOffset = -1;
      var lineEndPosition = 0;

      for (var index = 0; index < lineBreakContent.Count; index++)
      {
        var chunk = lineBreakContent[index];
        var view = chunk.Chunk;
        if (view.ViewToModel(position, out offset, out bias))
        {
          return true;
        }

        if (position.Y >= view.LayoutRect.Y && position.Y < view.LayoutRect.Bottom)
        {
          if (lineStartPosition > view.LayoutRect.X)
          {
            lineStartOffset = view.Offset;
            lineStartPosition = view.LayoutRect.X;
          }
          if (lineEndPosition < view.LayoutRect.Right)
          {
            lineEndOffset = chunk.EndOffsetWithoutLineBreaks;
            lineEndPosition = view.LayoutRect.Right;
          }
        }
      }

      if (position.X <= lineStartPosition)
      {
        offset = lineStartOffset;
        bias = Bias.Forward;
        return offset != -1;
      }

      if (position.X >= lineEndPosition)
      {
        offset = lineEndOffset;
        bias = Bias.Backward;
        return offset != -1;
      }

      offset = -1;
      bias = Bias.Forward;
      return false;
    }

    protected override Rectangle ArrangeOverride(Rectangle layoutSize)
    {
      Reflow(new Size(layoutSize.Width, layoutSize.Height));

      if (lineMetrics.Count == 0)
      {
        return new Rectangle(layoutSize.X, layoutSize.Y, 0, Style.GetValue(TextStyles.Font).LineSpacing);
      }

      var yPos = layoutSize.Y;
      var line = 0;
      for (var index = 0; index < lineBreakContent.Count; index++)
      {
        var chunk = lineBreakContent[index];
        var view = chunk.Chunk;
        if (chunk.Line != line)
        {
          yPos += (int)lineMetrics[line].LineHeight;
          line += 1;
        }

        var rectangle = new Rectangle(layoutSize.X + chunk.X, yPos, layoutSize.Width, view.DesiredSize.HeightInt);
        view.Arrange(rectangle);
      }

      yPos += (int)lineMetrics[line].LineHeight;
      var height = yPos - layoutSize.Y;
      Dirty = false;
      return new Rectangle(layoutSize.X, layoutSize.Y, layoutSize.Width, height);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
      RebuildContents();

      float width = 0;
      float height = 0;
      if (float.IsInfinity(availableSize.Width))
      {
        for (var index = 0; index < rawContent.Count; index++)
        {
          var view = rawContent[index];
          view.Measure(availableSize);
          width += view.DesiredSize.Width;
          height = Math.Max(height, view.DesiredSize.Height);
        }
        return new Size(width, height);
      }

      // line breaking ..
      Reflow(availableSize);
      for (var index = 0; index < lineMetrics.Count; index++)
      {
        var lm = lineMetrics[index];
        width = Math.Max(width, lm.MinWidth);
        height += lm.LineHeight;
      }

      Dirty = false;
      return new Size(width, height);
    }

    protected override void OnLayoutInvalidated()
    {
      lineWidth = -1;
      base.OnLayoutInvalidated();
    }

    void AlignLineChunks(LineMetrics currentLineMetric, float availableWidth)
    {
      var extraSpace = availableWidth - currentLineMetric.MinWidth;
      float extraX;
      switch (GetAlignment())
      {
        case Alignment.Fill:
        case Alignment.Start:
          {
            extraX = 0;
            break;
          }
        case Alignment.End:
          {
            extraX = extraSpace;
            break;
          }
        case Alignment.Center:
          extraX = extraSpace / 2f;
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }

      for (var idx = currentLineMetric.StartIndex; idx < currentLineMetric.EndIndex; idx += 1)
      {
        var chunk = lineBreakContent[idx];
        chunk.X += (int)extraX;
        chunk.BaseLine = currentLineMetric.BaseLine;
        lineBreakContent[idx] = chunk;
      }
    }

    Alignment GetAlignment()
    {
      return Style.GetValue(TextStyles.Alignment);
    }

    /// <summary>
    ///   Todo: Take y-offset into account to produce proper base-lines.
    /// </summary>
    /// <param name="lineStartIndex"></param>
    /// <param name="lineEndIndex"></param>
    /// <returns></returns>
    LineMetrics ComputeLineMetrics(int lineStartIndex, int lineEndIndex)
    {
      float baseLine = 0;
      float textHeight = 0;
      float currentLineWidth = 0;
      for (var idx = lineStartIndex; idx < lineEndIndex; idx += 1)
      {
        var chunk = lineBreakContent[idx];
        baseLine = Math.Max(baseLine, chunk.BaseLine);
        textHeight = Math.Max(textHeight, chunk.Chunk.DesiredSize.Height);
        currentLineWidth = Math.Max(currentLineWidth, chunk.X + chunk.Chunk.DesiredSize.Width);
      }
      return new LineMetrics(baseLine, textHeight, currentLineWidth, lineStartIndex, lineEndIndex);
    }

    void RebuildContents()
    {
      if (!Dirty)
      {
        return;
      }

      for (var i = rawContent.Count - 1; i >= 0; i--)
      {
        var v = rawContent[i];
        v.RemoveNotify(this);
        rawContent.RemoveAt(i);
      }

      RebuildFromElement(Node);
    }

    void RebuildFromElement(ITextNode node)
    {
      if (node.Leaf)
      {
        // add text-chunk
        var chunk = viewFactory.CreateChunkFor(node, Style);
        rawContent.Add(chunk);
        chunk.AddNotify(this);
      }
      else
      {
        for (var idx = 0; idx < node.Count; idx += 1)
        {
          RebuildFromElement(node[idx]);
        }
      }
    }

    void Reflow(Size size)
    {
      float width = size.Width;
      if (Style.GetValue(TextStyles.WrapText) != WrapText.Auto)
      {
        width = float.PositiveInfinity;
      }

      if (!Dirty && Math.Abs(width - lineWidth) < 0.5)
      {
        return;
      }

      lineWidth = -1;

      for (var i = lineBreakContent.Count - 1; i >= 0; i--)
      {
        var v = lineBreakContent[i];
        v.Chunk.RemoveNotify(this);
        lineBreakContent.RemoveAt(i);
      }

      lineMetrics.Clear();
      bool justified = GetAlignment() == Alignment.Fill;

      var lineStartIndex = 0;
      var lineEndIndex = 0;
      float x = 0;

      var enumerator = new PushBackEnumerator<ITextChunkView<TDocument>>(rawContent, 0, rawContent.Count);
      var lineContent = new List<ITextChunkView<TDocument>>();
      while (enumerator.MoveNext())
      {
        var chunk = enumerator.Current;
        chunk.Measure(Size.Auto);
        if (chunk.DesiredSize.Width <= width)
        {
          lineContent.Add(chunk);
          x += chunk.DesiredSize.Width;
          continue;
        }

        // Attempt a linebreak
        ITextChunkView<TDocument> first;
        ITextChunkView<TDocument> second;
        chunk.BreakAt(width - x, out first, out second);
        first.Measure(Size.Auto);
        lineContent.Add(first);
        width = Math.Max(x + first.DesiredSize.Width, width);

        // Add all chunks for the current line into the break content.
        lineEndIndex += AddNodes(lineContent, justified, size);
        lineContent.Clear();
        // only start a new line, if there is actual content in that line.
        // This check avoids an empty line if the first line's content is 
        // too large to fit on the line.
        var lineMetric = ComputeLineMetrics(lineStartIndex, lineEndIndex);
        lineMetrics.Add(lineMetric);
        AlignLineChunks(lineMetric, width);
        lineStartIndex = lineEndIndex;
        x = 0;
        
        if (second != null)
        {
          enumerator.PushBack(second);
        }
      }

      if (lineContent.Count > 0)
      {
        lineEndIndex += AddNodes(lineContent, justified, size);
        var metrics = ComputeLineMetrics(lineStartIndex, lineEndIndex);
        lineMetrics.Add(metrics);
        AlignLineChunks(metrics, width);
      }

      if (justified && lineBreakContent.Count > 0)
      {
        var count = lineBreakContent.Count;
        for (var i = 0; i < count; i++)
        {
          var jc = lineBreakContent[i].Chunk as JustifiedTextChunkView<TDocument>;
          if (jc != null)
          {
            jc.LastLine = i == count - 1;
          }
        }
      }

      lineWidth = width;
    }

    int AddNodes(List<ITextChunkView<TDocument>> lineContent, bool justified, Size size)
    {
      if (justified)
      {
        // Justified content covers the whole line. We need to break the text into words to insert the extra
        // spaces on rendering.
        var jc = new JustifiedTextChunkView<TDocument>(this.viewFactory.TextProcessingRules, Node, Style, lineContent);
        jc.Measure(size);
        lineBreakContent.Add(new PositionedChunk(jc, lineMetrics.Count, 0, jc.BaseLine));
        return 1;
      }

      int pos = 0;
      for (var i = 0; i < lineContent.Count; i++)
      {
        var currentChunk = lineContent[i];
        lineBreakContent.Add(new PositionedChunk(currentChunk, lineMetrics.Count, pos, currentChunk.BaseLine));
        pos += currentChunk.DesiredSize.WidthInt;
      }
      return lineContent.Count;
    }

    struct LineMetrics
    {
      public float BaseLine { get; }

      public float LineHeight { get; }

      public float MinWidth { get; }

      public int StartIndex { get; }

      public int EndIndex { get; }

      public LineMetrics(float baseLine, float lineHeight, float minWidth, int startIndex, int endIndex)
      {
        BaseLine = baseLine;
        LineHeight = lineHeight;
        MinWidth = minWidth;
        StartIndex = startIndex;
        EndIndex = endIndex;
      }
    }

    struct PositionedChunk
    {
      public PositionedChunk(ITextChunkView<TDocument> chunk, int line, int x, float baseLine)
      {
        Chunk = chunk;
        Line = line;
        X = x;
        BaseLine = baseLine;
        EndOffsetWithoutLineBreaks = chunk.EndOffsetWithoutLineBreaks;
      }
      
      public PositionedChunk(JustifiedTextChunkView<TDocument> chunk, int line, int x, float baseLine)
      {
        Chunk = chunk;
        Line = line;
        X = x;
        BaseLine = baseLine;
        EndOffsetWithoutLineBreaks = chunk.EndOffsetWithoutLineBreaks;
      }

      public int EndOffsetWithoutLineBreaks { get; }
      
      public int Line { get; }

      public int X { get; set; }

      public float BaseLine { get; set; }

      public ITextView<TDocument> Chunk { get; }

      public override string ToString()
      {
        var text = Chunk.Node.Document.Content.TextAt(Chunk.Offset, Chunk.Offset - Chunk.EndOffset);
        return $"PositionedChunk={{Line: {Line}, X: {X}, BaseLine: {BaseLine}, Chunk={{'{text}', [{Chunk.Offset} .. {Chunk.EndOffset}]}}}}";
      }
    }
  }
}