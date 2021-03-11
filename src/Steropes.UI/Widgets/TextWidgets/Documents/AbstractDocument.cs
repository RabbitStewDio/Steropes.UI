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
using System.Text;

using Steropes.UI.Util;
using Steropes.UI.Widgets.TextWidgets.Documents.Edits;

namespace Steropes.UI.Widgets.TextWidgets.Documents
{
  public abstract class AbstractDocument : ITextDocument
  {
    readonly EventSupport<TextModificationEventArgs> documentModifiedSupport;

    readonly EventSupport<UndoableEditEventArgs> undoableEditCreatedSupport;

    ITextDocumentFilterChain filterChain;

    protected AbstractDocument(IDocumentContent content)
    {
      Content = content;
      documentModifiedSupport = new EventSupport<TextModificationEventArgs>();
      undoableEditCreatedSupport = new EventSupport<UndoableEditEventArgs>();
      filterChain = new RootFilterChain(this);
    }

    public event EventHandler<TextModificationEventArgs> DocumentModified
    {
      add
      {
        documentModifiedSupport.Event += value;
      }
      remove
      {
        documentModifiedSupport.Event -= value;
      }
    }

    public event EventHandler<UndoableEditEventArgs> UndoableEditCreated
    {
      add
      {
        undoableEditCreatedSupport.Event += value;
      }
      remove
      {
        undoableEditCreatedSupport.Event -= value;
      }
    }

    public IDocumentContent Content { get; }

    public abstract ITextNode Root { get; }

    public int TextLength => Content.Length;

    public void CopyInto(StringBuilder b, int offset, int length)
    {
      Content.CopyInto(b, offset, length);
    }

    public ITextPosition CreatePosition(int offset, Bias bias) => Content.CreatePosition(offset, bias);

    public void DeleteAt(int offset, int length)
    {
      filterChain.DeleteAt(offset, length);
    }

    public void InsertAt(int offset, string text)
    {
      filterChain.InsertAt(offset, text);
    }

    public void InsertAt(int offset, char text)
    {
      filterChain.InsertAt(offset, text);
    }

    public bool PopFilter(out ITextDocumentFilter filter)
    {
      if (filterChain.Parent == null)
      {
        filter = null;
        return false;
      }

      filter = filterChain.Filter;
      filterChain = filterChain.Parent;
      return true;
    }

    public void PushFilter(ITextDocumentFilter filter)
    {
      filterChain = new TextDocumentFilterChain(filterChain, filter);
    }

    public string TextAt(int offset, int length)
    {
      return Content.TextAt(offset, length);
    }

    protected void DeleteAtUnfiltered(int offset, int length)
    {
      if (length == 0)
      {
        return;
      }

      var edt = new DocumentEditInfo(this, TextModificationType.Remove, offset, length);
      edt.Add(RemoveUpdate(offset, length));
      edt.Add(Content.Remove(offset, length));

      Raise(new TextModificationEventArgs(TextModificationType.Remove, offset, length));
      Raise(new UndoableEditEventArgs(edt, false));
    }

    protected void InsertAtUnfiltered(int offset, string text)
    {
      if (text.Length == 0)
      {
        return;
      }

      var edt = new DocumentEditInfo(this, TextModificationType.Insert, offset, text.Length);
      edt.Add(Content.Insert(offset, text));
      edt.Add(InsertUpdate(offset, text.Length));

      Raise(new TextModificationEventArgs(TextModificationType.Insert, offset, text.Length));
      Raise(new UndoableEditEventArgs(edt, false));
    }

    protected void InsertAtUnfiltered(int offset, char text)
    {
      var edt = new DocumentEditInfo(this, TextModificationType.Insert, offset, 1);
      edt.Add(Content.Insert(offset, text));
      edt.Add(InsertUpdate(offset, 1));

      Raise(new TextModificationEventArgs(TextModificationType.Insert, offset, 1));
      Raise(new UndoableEditEventArgs(edt, false));
    }

    protected abstract IUndoableEdit InsertUpdate(int offset, int length);

    protected void Raise(TextModificationEventArgs args)
    {
      documentModifiedSupport.Raise(this, args);
    }

    protected void Raise(UndoableEditEventArgs args)
    {
      undoableEditCreatedSupport.Raise(this, args);
    }

    protected abstract IUndoableEdit RemoveUpdate(int offset, int length);

    protected abstract void ReplaceRoot(ITextNode element);

    public class DocumentEditInfo : CompoundUndoableEdit, IDocumentEditInfo
    {
      readonly AbstractDocument document;

      readonly Dictionary<ITextNode, IElementEdit> editsByNode;

      public DocumentEditInfo(AbstractDocument document, TextModificationType modificationType, int offset, int length)
      {
        editsByNode = new Dictionary<ITextNode, IElementEdit>(IdentityComparator<ITextNode>.Instance);
        this.document = document;
        ModificationType = modificationType;
        Offset = offset;
        Length = length;
      }

      public ITextDocument Document => document;

      public int Length { get; }

      public TextModificationType ModificationType { get; }

      public int Offset { get; }

      public bool IsNodeAffected(ITextNode node, out IElementEdit edit)
      {
        return editsByNode.TryGetValue(node, out edit);
      }

      protected override void AddNotify(IUndoableEdit edit)
      {
        if (edit is IElementEdit)
        {
          var elementEdit = (IElementEdit)edit;
          editsByNode[elementEdit.NewElement] = elementEdit;
          editsByNode[elementEdit.OldElement] = elementEdit;
        }
      }

      protected override void EndRedo()
      {
        document.Raise(new UndoableEditEventArgs(this, true));
        document.Raise(new TextModificationEventArgs(ModificationType, Offset, Length));
      }

      protected override void EndUndo()
      {
        document.Raise(new UndoableEditEventArgs(this, true));
        document.Raise(new TextModificationEventArgs(ReverseModification(), Offset, Length));
      }

      TextModificationType ReverseModification()
      {
        switch (ModificationType)
        {
          case TextModificationType.Insert:
            return TextModificationType.Remove;
          case TextModificationType.Remove:
            return TextModificationType.Insert;
          case TextModificationType.Change:
            return TextModificationType.Change;
          default:
            throw new ArgumentOutOfRangeException();
        }
      }
    }

    /// <summary>
    ///   Represents a change in the document.
    /// </summary>
    protected class ElementEdit : UndoableEditBase, IElementEdit
    {
      readonly ITextNode[] addedNodes;

      readonly AbstractDocument document;

      readonly ITreePath<ITextNode> newElementPath;

      readonly ITreePath<ITextNode> oldElementPath;

      readonly ITextNode[] removedNodes;

      public ElementEdit(
        AbstractDocument document,
        ITreePath<ITextNode> oldElementPath,
        ITreePath<ITextNode> newElementPath,
        int index,
        ITextNode[] addedNodes,
        ITextNode[] removedNodes) : base(false, "")
      {
        Index = index;
        this.addedNodes = addedNodes;
        this.removedNodes = removedNodes;
        this.document = document;
        this.oldElementPath = oldElementPath;
        this.newElementPath = newElementPath;
      }

      public ITextNode[] AddedNodes => IsUndo ? addedNodes : removedNodes;

      public int Index { get; }

      public ITextNode NewElement => IsUndo ? newElementPath.Node : oldElementPath.Node;

      public ITextNode OldElement => IsUndo ? oldElementPath.Node : newElementPath.Node;

      public ITextNode[] RemovedNodes => IsUndo ? removedNodes : addedNodes;

      protected override void PerformRedo()
      {
        document.ReplaceRoot(newElementPath.GetRoot());
      }

      protected override void PerformUndo()
      {
        document.ReplaceRoot(oldElementPath.GetRoot());
      }
    }

    protected class RootFilterChain : ITextDocumentFilterChain
    {
      readonly AbstractDocument document;

      public RootFilterChain(AbstractDocument document)
      {
        this.document = document;
      }

      public IDocumentContent Content => document.Content;

      public ITextDocumentFilter Filter => null;

      public ITextDocumentFilterChain Parent => null;

      public void DeleteAt(int offset, int length)
      {
        document.DeleteAtUnfiltered(offset, length);
      }

      public void InsertAt(int offset, char text)
      {
        document.InsertAtUnfiltered(offset, text);
      }

      public void InsertAt(int offset, string text)
      {
        document.InsertAtUnfiltered(offset, text);
      }
    }
  }
}