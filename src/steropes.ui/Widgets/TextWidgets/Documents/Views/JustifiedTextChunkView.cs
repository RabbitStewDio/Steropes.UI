using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Steropes.UI.Components;
using Steropes.UI.Styles;
using Steropes.UI.Widgets.TextWidgets.Documents.Helper;

namespace Steropes.UI.Widgets.TextWidgets.Documents.Views
{
  public class JustifiedTextChunkView<TDocument> : BranchTextView<TDocument>
    where TDocument : ITextDocument
  {
    readonly ITextProcessingRules rules;

    public JustifiedTextChunkView(ITextProcessingRules rules,
                                  ITextNode node,
                                  IStyle style,
                                  IList<ITextChunkView<TDocument>> chunks) : base(node, style)
    {
      this.rules = rules;
      if (chunks.Count == 0)
      {
        throw new ArgumentException();
      }

      Offset = chunks[0].Offset;
      EndOffset = chunks[chunks.Count - 1].EndOffset;
      EndOffsetWithoutLineBreaks = chunks[chunks.Count - 1].EndOffsetWithoutLineBreaks;
      for (int i = 0; i < chunks.Count; i += 1)
      {
        ChopText(chunks[i]);
      }
    }

    public bool LastLine { get; set; }

    public override int EndOffset { get; }

    public override int Offset { get; }

    void ChopText(ITextChunkView<TDocument> chunk)
    {
      var doc = chunk.Node.Document.Content;
      var it = new BreakIterator<WordBreakType>(doc, rules.IsWordBreak, chunk.TrimmedStartOffset, chunk.TrimmedEndOffset);
      var cursor = chunk.TrimmedStartOffset;
      var lastBreak = chunk.TrimmedStartOffset;
      while (it.MoveNext())
      {
        var wb = it.Current;
        if (wb != WordBreakType.WordBreak)
        {
          cursor += 1;
          continue;
        }

        if (cursor == lastBreak)
        {
          continue;
        }
        
        // have a new word 
        ITextChunkView<TDocument> first;
        ITextChunkView<TDocument> second;
        chunk.BreakAtOffset(cursor, out first, out second);
        if (second != null)
        {
          Add(first);
          chunk = second;
        }
      }
      
      Add(chunk);
    }

    public float BaseLine => Font.Baseline;

    protected override Rectangle ArrangeOverride(Rectangle layoutSize)
    {
      Size size = this.DesiredSize;
      int extra = Math.Max(0, layoutSize.Width - size.WidthInt);
      int x = layoutSize.X;

      if (Count > 0)
      {
        int extraPerItem = 0;
        int extraOnLast = 0;
        if (!LastLine)
        {
          extraPerItem = extra / Count;
          extraOnLast = extra - extraPerItem * Count;
        }

        for (int i = 0; i < Count; i += 1)
        {
          if (i == Count - 1)
          {
            x += extraOnLast;
          }
          
          var textView = this[i];
          var rect = new Rectangle(x, layoutSize.Y, textView.DesiredSize.WidthInt, size.HeightInt);
          textView.Arrange(rect);
          x += textView.DesiredSize.WidthInt + extraPerItem;
        }
      }
      
      return new Rectangle(layoutSize.X, layoutSize.Y, x - layoutSize.X, size.HeightInt);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
      float width = 0;
      float height = 0;
      for (int i = 0; i < Count; i += 1)
      {
        var textView = this[i];
        textView.Measure(availableSize);
        width += textView.DesiredSize.Width;
        height = Math.Max(textView.DesiredSize.Height, height);
      }
      return new Size(width, height);
    }

    public override NavigationResult Navigate(int editOffset, Direction direction, out int targetOffset)
    {
      if (direction == Direction.Down || direction == Direction.Up)
      {
        targetOffset = editOffset;
        return NavigationResult.BoundaryChanged;
      }

      return base.Navigate(editOffset, direction, out targetOffset);
    }

    public int EndOffsetWithoutLineBreaks { get; }
  }
}