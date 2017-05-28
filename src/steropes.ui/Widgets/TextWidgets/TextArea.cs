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
using System.ComponentModel;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Steropes.UI.Components;
using Steropes.UI.Input.KeyboardInput;
using Steropes.UI.Platform;
using Steropes.UI.Widgets.TextWidgets.Documents;
using Steropes.UI.Widgets.TextWidgets.Documents.PlainText;
using Steropes.UI.Widgets.TextWidgets.Documents.Views;

namespace Steropes.UI.Widgets.TextWidgets
{
  /// <summary>
  ///   A multi-line text editor. This class is limited to plain-text to simplify the API and calculations within.
  ///   To use rich-text, use the RichTextArea instead.
  /// </summary>
  public class TextArea : TextEditorWidgetBase<DocumentView<PlainTextDocument>, PlainTextDocument>, IScrollable
  {
    readonly LineNumberWidget lineNumberRenderer;

    public TextArea(IUIStyle style, IDocumentEditor<DocumentView<PlainTextDocument>, PlainTextDocument> editor) : base(style, editor)
    {
      ActionMap.Register(new KeyStroke(Keys.Enter), OnEnterPressed);
      ActionMap.Register(new KeyStroke(Keys.PageUp), OnPageUpPressed);
      ActionMap.Register(new KeyStroke(Keys.PageDown), OnPageDownPressed);

      lineNumberRenderer = new LineNumberWidget(UIStyle);
      lineNumberRenderer.AddNotify(this);
      lineNumberRenderer.Anchor = AnchoredRect.CreateLeftAnchored();
      lineNumberRenderer.DocumentView = Content;
      RaiseChildrenChanged(null, lineNumberRenderer);

      DisplayLineNumbers = true;
      Caret.CaretChanged += OnCaretChanged;
    }

    public TextArea(IUIStyle style) : this(style, new PlainTextDocumentEditor(style))
    {
    }

    public override int Count => base.Count + 1;

    public bool DisplayLineNumbers { get; set; }

    public Size PreferredViewPortSize => DesiredSize;

    public bool TrackViewportHeight => false;

    public bool TrackViewportWidth => WrapText == WrapText.Auto;

    public override IWidget this[int index]
    {
      get
      {
        if (index == 0)
        {
          return lineNumberRenderer;
        }
        return base[index - 1];
      }
    }

    public int ScrollBlockIncrement(Rectangle viewPort, Direction scrollDirection)
    {
      switch (scrollDirection)
      {
        case Direction.Down:
        case Direction.Up:
          {
            return Math.Max(1, viewPort.Height / Font.LineSpacing) * Font.LineSpacing;
          }
        case Direction.Left:
        case Direction.Right:
          {
            return 50;
          }
        default:
          {
            throw new InvalidEnumArgumentException(nameof(scrollDirection), (int)scrollDirection, typeof(Direction));
          }
      }
    }

    public int ScrollUnitIncrement(Rectangle viewPort, Direction scrollDirection)
    {
      switch (scrollDirection)
      {
        case Direction.Down:
        case Direction.Up:
          {
            return Font.LineSpacing;
          }
        case Direction.Left:
        case Direction.Right:
          {
            return 1;
          }
        default:
          {
            throw new InvalidEnumArgumentException(nameof(scrollDirection), (int)scrollDirection, typeof(Direction));
          }
      }
    }

    protected override Rectangle ArrangeOverride(Rectangle layoutSize)
    {
      lineNumberRenderer.Arrange(lineNumberRenderer.ArrangeChild(layoutSize.IncreaseRectBy(Padding)));

      if (DisplayLineNumbers)
      {
        var insets = new Insets(0, Math.Max(0, lineNumberRenderer.LayoutRect.Width - Padding.Left), 0, 0);
        var contentSize = layoutSize.ReduceRectBy(insets);
        var retval = base.ArrangeOverride(contentSize);
        return retval.IncreaseRectBy(insets);
      }

      return base.ArrangeOverride(layoutSize);
    }

    protected override void DrawChildren(IBatchedDrawingService drawingService)
    {
      base.DrawChildren(drawingService);
      if (DisplayLineNumbers)
      {
        lineNumberRenderer.Draw(drawingService);
      }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
      lineNumberRenderer.Measure(availableSize);
      if (DisplayLineNumbers)
      {
        return base.MeasureOverride(availableSize.ReduceBy(lineNumberRenderer.DesiredSize.Width, 0));
      }
      return base.MeasureOverride(availableSize);
    }

    protected override void OnContentUpdated()
    {
      if (lineNumberRenderer != null)
      {
        lineNumberRenderer.DocumentView = Content;
      }
    }

    void OnCaretChanged(object sender, EventArgs e)
    {
      var control = Parent as IScrollControl;
      if (control != null)
      {
        Rectangle result;
        if (Content.ModelToView(Caret.SelectionEndOffset, out result))
        {
          control.ScrollTo(result);
        }
      }
    }

    void OnEnterPressed(KeyEventArgs args)
    {
      OnKeyTyped('\n');
    }

    void OnPageDownPressed(KeyEventArgs obj)
    {
    }

    void OnPageUpPressed(KeyEventArgs obj)
    {
    }
  }
}