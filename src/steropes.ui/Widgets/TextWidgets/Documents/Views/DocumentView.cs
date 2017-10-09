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
using Steropes.UI.Widgets.Styles;
using Steropes.UI.Widgets.TextWidgets.Documents.Edits;

namespace Steropes.UI.Widgets.TextWidgets.Documents.Views
{
  public interface IDocumentView<TDocument> : IWidget
    where TDocument : ITextDocument
  {
    event EventHandler<TextModificationEventArgs> DocumentModified;

    event EventHandler<UndoableEditEventArgs> UndoableEditCreated;

    TDocument Document { get; set; }

    IHighlighter<TDocument> Highlighter { get; }

    ITextNodeViewFactory<TDocument> ViewFactory { get; }

    bool ModelToView(int offset, out Rectangle result);

    int Navigate(int currentOffset, Direction direction);

    bool ViewToModel(Point position, out int offset, out Bias bias);
  }

  public class DocumentView<TDocument> : Widget, IDocumentView<TDocument>
    where TDocument : ITextDocument
  {
    TDocument document;

    ITextNodeViewFactory<TDocument> viewFactory;

    public DocumentView(IDocumentEditor<DocumentView<TDocument>, TDocument> editor) : base(editor.Style)
    {
      if (editor == null)
      {
        throw new ArgumentNullException(nameof(editor));
      }

      Style.ValueChanged += OnStyleValueChanged;
      StyleInvalidated += (s, a) => ResetDocumentView();

      TextStyles = Style.StyleSystem.StylesFor<TextStyleDefinition>();

      Highlighter = new Highligher<TDocument>(this);

      ViewFactory = editor.CreateViewFactory();
      Document = editor.CreateDocument();
      Focusable = false;
      Enabled = false;
    }

    TextStyleDefinition TextStyles { get; }

    public event EventHandler<TextModificationEventArgs> DocumentModified;

    public event EventHandler<UndoableEditEventArgs> UndoableEditCreated;

    public TDocument Document
    {
      get { return document; }
      set
      {
        if (document != null)
        {
          document.UndoableEditCreated -= OnHostedDocumentChanged;
          document.DocumentModified -= OnTextContentsChanged;
        }
        document = value;
        if (document != null)
        {
          document.UndoableEditCreated += OnHostedDocumentChanged;
          document.DocumentModified += OnTextContentsChanged;
        }
        ResetDocumentView();
      }
    }

    public int EndOffset => RootView?.EndOffset ?? 0;

    public IHighlighter<TDocument> Highlighter { get; }

    public int Offset => RootView?.Offset ?? 0;

    public ITextNodeViewFactory<TDocument> ViewFactory
    {
      get { return viewFactory; }
      set
      {
        viewFactory = value;
        ResetDocumentView();
      }
    }

    protected ITextView<TDocument> RootView { get; private set; }

    public virtual bool ModelToView(int offset, out Rectangle result)
    {
      if (RootView != null)
      {
        if (LayoutInvalid)
        {
          Arrange(LayoutRect);
        }
        return RootView.ModelToView(offset, out result);
      }
      result = default(Rectangle);
      return false;
    }

    public int Navigate(int currentOffset, Direction direction)
    {
      if (RootView != null)
      {
        if (LayoutInvalid)
        {
          Arrange(LayoutRect);
        }
        int offset;
        switch (RootView.Navigate(currentOffset, direction, out offset))
        {
          case NavigationResult.Valid:
          {
            return offset;
          }
          case NavigationResult.Invalid:
          {
            if (currentOffset < Offset)
            {
              // correct the invalid offset to the nearest valid offset within the document.
              RootView.Navigate(RootView.Offset - 1, Direction.Right, out offset);
              return offset;
            }
            if (currentOffset > EndOffset)
            {
              // correct the invalid offset to the nearest valid offset within the document.
              RootView.Navigate(RootView.EndOffset + 1, Direction.Left, out offset);
              return offset;
            }

            // Nothing we can do, make it a no-op.
            return currentOffset;
          }
          case NavigationResult.BoundaryChanged:
          {
            switch (direction)
            {
              case Direction.Right:
              case Direction.Down:
              {
                RootView.Navigate(RootView.EndOffset + 1, Direction.Left, out offset);
                return offset;
              }
              case Direction.Left:
              case Direction.Up:
              {
                RootView.Navigate(RootView.Offset - 1, Direction.Right, out offset);
                return offset;
              }
              default:
              {
                throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
              }
            }
          }
          default:
            throw new ArgumentOutOfRangeException();
        }
      }

      return currentOffset;
    }

    /// <summary>
    ///   Indicates that the document has changed in some non-traceable way. Rebuild everything from scratch.
    /// </summary>
    public void ResetDocumentView()
    {
      if (Document == null)
      {
        RootView = null;
        InvalidateLayout();
        return;
      }

      var d = Document;
      var view = ViewFactory.CreateFor(d.Root, Style);

      IterateTreeOnDocumentChange(new RebuildDocumentEditInfo(d), view);
      RootView = view;
      InvalidateLayout();
    }

    public virtual bool ViewToModel(Point position, out int offset, out Bias bias)
    {
      if (RootView != null)
      {
        if (LayoutInvalid)
        {
          Arrange(LayoutRect);
        }
        return RootView.ViewToModel(position, out offset, out bias);
      }
      offset = 0;
      bias = Bias.Forward;
      return false;
    }

    protected override Rectangle ArrangeOverride(Rectangle layoutSize)
    {
      if (RootView == null)
      {
        return new Rectangle(layoutSize.X, layoutSize.Y, 0, 0);
      }

      RootView.Arrange(layoutSize);
      return RootView.LayoutRect;
    }

    protected override void DrawWidget(IBatchedDrawingService drawingService)
    {
      Highlighter.Draw(drawingService);
      RootView?.Draw(drawingService);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
      if (RootView == null)
      {
        var font = Style.GetValue(TextStyles.Font);
        return new Size(0, font?.LineSpacing ?? 0);
      }

      RootView.Measure(availableSize);
      return RootView.DesiredSize;
    }

    void IterateTreeOnContentChange(ITextView<TDocument> view, TextModificationEventArgs e)
    {
      view.OnNodeContentChanged(this, e);

      for (var i = 0; i < view.Count; i++)
      {
        var child = view[i];
        if (child.InEditRange(e.Offset, e.Offset + e.Length))
        {
          IterateTreeOnContentChange(child, e);
        }
      }
    }

    void IterateTreeOnDocumentChange(IDocumentEditInfo edit, ITextView<TDocument> view)
    {
      IElementEdit editInfo;
      if (edit.IsNodeAffected(view.Node, out editInfo))
      {
        view.OnNodeStructureChanged(this, editInfo);
      }

      for (var i = 0; i < view.Count; i++)
      {
        var child = view[i];
        if (child.Node.InRange(edit.Offset))
        {
          IterateTreeOnDocumentChange(edit, child);
        }
      }
    }

    void OnDocumentChanged(UndoableEditEventArgs e)
    {
      if (Document == null)
      {
        RootView = null;
        InvalidateLayout();
        return;
      }

      if (RootView == null)
      {
        var d = Document;
        RootView = ViewFactory.CreateFor(d.Root, Style);
      }
      IterateTreeOnDocumentChange(e.Edit, RootView);
      InvalidateLayout();
    }

    void OnHostedDocumentChanged(object sender, UndoableEditEventArgs e)
    {
      if (Document == null)
      {
        return;
      }

      if (RootView == null)
      {
        e = new UndoableEditEventArgs(new RebuildDocumentEditInfo(Document), e.UndoOrRedo);
      }
      OnDocumentChanged(e);
      UndoableEditCreated?.Invoke(this, e);
    }

    void OnStyleValueChanged(object sender, StyleEventArgs e)
    {
      ResetDocumentView();
    }

    void OnTextContentsChanged(object sender, TextModificationEventArgs e)
    {
      IterateTreeOnContentChange(RootView, e);
      DocumentModified?.Invoke(this, e);
    }
  }
}