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
using Microsoft.Xna.Framework;

using Steropes.UI.Platform;
using Steropes.UI.Styles;
using Steropes.UI.Util;
using Steropes.UI.Widgets.Styles;

namespace Steropes.UI.Widgets.TextWidgets.Documents.Views
{
  public interface IHighlight<TDocument>
    where TDocument : ITextDocument
  {
    ITextPosition EndOffset { get; }

    ITextPosition Offset { get; }

    /// <summary>
    ///   Draws a highlight on the given region. The bounds given represent the whole view (ie the line being drawn)
    /// </summary>
    /// <param name="drawingService">The drawing service.</param>
    /// <param name="view">The document view for which this highligher draws.</param>
    void Draw(IBatchedDrawingService drawingService, IDocumentView<TDocument> view);
  }

  public class Highlight<TDocument> : IHighlight<TDocument>
    where TDocument : ITextDocument
  {
    readonly IStyle style;

    readonly TextStyleDefinition widgetStyles;

    public Highlight(ITextPosition offset, ITextPosition endOffset, IStyle style)
    {
      this.style = style;
      widgetStyles = style.StyleSystem.StylesFor<TextStyleDefinition>();

      Offset = offset;
      EndOffset = endOffset;
    }

    public Color Color => style.GetValue(widgetStyles.SelectionColor);

    public ITextPosition EndOffset { get; set; }

    public ITextPosition Offset { get; set; }

    public void Draw(IBatchedDrawingService drawingService, IDocumentView<TDocument> view)
    {
      if (Offset == null || EndOffset == null)
      {
        return;
      }

      Rectangle start;
      view.ModelToView(Offset.Offset, out start);
      if (Offset.Bias == Bias.Backward)
      {
        start.Width = 0;
      }

      Rectangle end;
      view.ModelToView(EndOffset.Offset, out end);
      if (EndOffset.Bias == Bias.Backward)
      {
        end.Width = 0;
      }

      var bounds = view.LayoutRect;
      if (start.Y == end.Y)
      {
        // same line. Can take shortcut ..
        drawingService.FillRect(start.Union(end), Color);
      }
      else
      {
        // draw first line 
        drawingService.FillRect(new Rectangle(start.X, start.Y, bounds.Right - start.X, start.Height), Color);

        // draw inbetween lines using the full view-width as bounds 
        drawingService.FillRect(new Rectangle(bounds.X, start.Bottom, bounds.Width, end.Y - start.Bottom), Color);

        // draw last line
        drawingService.FillRect(new Rectangle(bounds.X, end.Y, end.Right - bounds.X, end.Height), Color);
      }
    }
  }
}