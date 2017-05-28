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
using Steropes.UI.Animation;
using Steropes.UI.Components;
using Steropes.UI.Platform;
using Steropes.UI.Styles;
using Steropes.UI.Widgets.Styles;
using Steropes.UI.Widgets.TextWidgets.Documents;
using Steropes.UI.Widgets.TextWidgets.Documents.Views;

namespace Steropes.UI.Widgets.TextWidgets
{
  /// <summary>
  ///   A caret is an indicator of the edit-position of text documents. It contains both the information on
  ///   the current cursor/insert-position and an potential text selection.
  ///   <p />
  ///   The caret's selection can be either empty (start and end are the same offset), forward (start is lower
  ///   than end) or backward (end is lower than start). This directionality is important when we handle text-editing.
  /// </summary>
  public interface ICaret : IWidget
  {
    event EventHandler<EventArgs> CaretChanged;

    int MaximumOffset { get; }

    int SelectionEndOffset { get; }

    int SelectionStartOffset { get; }

    void MoveTo(int offset);

    void Select(int offset);
  }

  public class Caret<TView, TDocument> : Widget, ICaret
    where TDocument : ITextDocument where TView : IDocumentView<TDocument>
  {
    readonly StepValue blinkAnimation;

    readonly Highlight<TDocument> selectionHighlight;

    readonly TextStyleDefinition styleDefinition;

    ITextPosition endPosition;

    ITextPosition startPosition;

    public Caret(IUIStyle style, TView textInformation) : base(style)
    {
      if (textInformation == null)
      {
        throw new ArgumentNullException(nameof(textInformation));
      }

      styleDefinition = StyleSystem.StylesFor<TextStyleDefinition>();

      TextInformation = textInformation;
      TextInformation.Document.DocumentModified += UpdatePositions;
      startPosition = null;
      endPosition = TextInformation.Document.CreatePosition(0, Bias.Backward);

      selectionHighlight = new Highlight<TDocument>(startPosition, endPosition, Style);
      textInformation.Highlighter.AddHighlight(selectionHighlight);

      blinkAnimation = new StepValue(0, 1) { Duration = 1, Loop = AnimationLoop.Loop };

      Style.ValueChanged += OnStyleChanged;
    }

    void OnStyleChanged(object sender, StyleEventArgs e)
    {
      if (ReferenceEquals(e.Key, styleDefinition.CaretBlinking))
      {
        blinkAnimation.Stopped = !Style.GetValue(styleDefinition.CaretBlinking);
      }
      if (ReferenceEquals(e.Key, styleDefinition.CaretBlinkRate))
      {
        blinkAnimation.Duration = Style.GetValue(styleDefinition.CaretBlinkRate);
      }
    }

    public float CaretBlinkRate
    {
      get { return Style.GetValue(styleDefinition.CaretBlinkRate); }
      set { Style.SetValue(styleDefinition.CaretBlinkRate, value); }
    }

    public bool CaretBlinking
    {
      get { return Style.GetValue(styleDefinition.CaretBlinking); }
      set { Style.SetValue(styleDefinition.CaretBlinking, value); }
    }

    public event EventHandler<EventArgs> CaretChanged;

    public int MaximumOffset => TextInformation.Document.TextLength;

    public override string NodeType => "Caret";

    /// <summary>
    ///   Selection end is where the cursor blinks.
    /// </summary>
    public int SelectionEndOffset => endPosition.Offset;

    /// <summary>
    ///   An optional start offset for the selection range.
    /// </summary>
    public int SelectionStartOffset => startPosition?.Offset ?? SelectionEndOffset;

    public TView TextInformation { get; }

    public int Width => Style.GetValue(styleDefinition.CaretWidth);

    public void MoveTo(int offset)
    {
      startPosition = null;
      endPosition = TextInformation.Document.CreatePosition(offset, Bias.Backward);

      CaretChanged?.Invoke(this, EventArgs.Empty);
      UpdateSelectionHighlight();
      InvalidateLayout();
    }

    public void Select(int offset)
    {
      if (startPosition == null)
      {
        startPosition = TextInformation.Document.CreatePosition(endPosition.Offset, Bias.Forward);
      }
      endPosition = TextInformation.Document.CreatePosition(offset, Bias.Backward);

      CaretChanged?.Invoke(this, EventArgs.Empty);
      UpdateSelectionHighlight();
      InvalidateLayout();
    }

    public override void Update(GameTime elapsedTime)
    {
      base.Update(elapsedTime);
      blinkAnimation.Update(elapsedTime);
    }

    protected override Rectangle ArrangeOverride(Rectangle layoutSize)
    {
      Rectangle rect;
      TextInformation.ModelToView(SelectionEndOffset, out rect);
      return new Rectangle(rect.X, rect.Y, Width, rect.Height);
    }

    protected override void DrawWidget(IBatchedDrawingService drawingService)
    {
      if (blinkAnimation.CurrentValue > 0)
      {
        drawingService.FillRect(LayoutRect, Color);
      }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
      Rectangle rect;
      TextInformation.ModelToView(SelectionEndOffset, out rect);
      return new Size(Width, rect.Height);
    }

    void UpdatePositions(object sender, TextModificationEventArgs e)
    {
      if (SelectionEndOffset == SelectionStartOffset)
      {
        startPosition = null;
      }

      CaretChanged?.Invoke(this, EventArgs.Empty);
      UpdateSelectionHighlight();
      InvalidateLayout();
    }

    void UpdateSelectionHighlight()
    {
      selectionHighlight.Offset = startPosition;
      selectionHighlight.EndOffset = endPosition;
    }
  }
}