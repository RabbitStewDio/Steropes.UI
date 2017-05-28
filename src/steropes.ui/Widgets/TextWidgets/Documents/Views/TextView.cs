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

namespace Steropes.UI.Widgets.TextWidgets.Documents.Views
{
  public abstract class TextView<TDocument> : VisualContent, ITextView<TDocument>
    where TDocument : ITextDocument
  {
    protected readonly TextStyleDefinition TextStyles;

    ITextNode node;

    protected TextView(ITextNode node, IStyle style)
    {
      Node = node;
      Style = style;

      TextStyles = Style.StyleSystem.StylesFor<TextStyleDefinition>();
    }

    public virtual int Count => 0;

    public bool Dirty { get; protected set; }

    public virtual int EndOffset => Node.EndOffset;

    public ITextNode Node
    {
      get
      {
        return node;
      }
      protected set
      {
        if (ReferenceEquals(node, value))
        {
          return;
        }
        node = value;
        InvalidateLayout();
      }
    }

    public virtual int Offset => Node.Offset;

    public ITextView<TDocument> Parent { get; private set; }

    public IStyle Style { get; }

    protected Alignment Alignment => Style.GetValue(TextStyles.Alignment);

    protected IUIFont Font => Style.GetValue(TextStyles.Font);

    public virtual ITextView<TDocument> this[int index]
    {
      get
      {
        throw new IndexOutOfRangeException();
      }
    }

    public virtual void AddNotify(ITextView<TDocument> parent)
    {
      // dont invalidate the parent's layout here ...
      Parent = parent;
    }

    public override void DrawOverlay(IBatchedDrawingService drawingService)
    {
    }

    public abstract bool ModelToView(int offset, out Rectangle result);

    public abstract NavigationResult Navigate(int editOffset, Direction direction, out int targetOffset);

    public virtual void OnNodeContentChanged(IDocumentView<TDocument> docView, TextModificationEventArgs args)
    {
    }

    public virtual void OnNodeStructureChanged(IDocumentView<TDocument> docView, IElementEdit edit)
    {
      Node = edit.NewElement;
      InvalidateLayout();
    }

    public virtual void RemoveNotify(ITextView<TDocument> parent)
    {
      // dont invalidate the parent's layout here ...
      Parent = parent;
    }

    public abstract bool ViewToModel(Point position, out int offset, out Bias bias);

    protected override void OnLayoutInvalidated()
    {
      base.OnLayoutInvalidated();
      Parent?.InvalidateLayout();
    }
  }
}