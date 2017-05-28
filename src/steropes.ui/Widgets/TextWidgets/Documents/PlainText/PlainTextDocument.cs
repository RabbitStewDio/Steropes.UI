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

using Steropes.UI.Widgets.TextWidgets.Documents.Edits;
using Steropes.UI.Widgets.TextWidgets.Documents.Helper;

namespace Steropes.UI.Widgets.TextWidgets.Documents.PlainText
{
  /// <summary>
  ///   Simple document that stores all lines as leaf-nodes in a single branch-node.
  /// </summary>
  public class PlainTextDocument : AbstractDocument
  {
    static readonly ITextNode[] EmptyNodes = new ITextNode[0];

    readonly List<ITextNode> addedNodes = new List<ITextNode>();

    readonly List<ITextNode> removedNodes = new List<ITextNode>();

    readonly ITextProcessingRules rules;

    ImmutableBranchNode rootNode;

    public PlainTextDocument() : base(new StringDocumentContent())
    {
      rootNode = new ImmutableBranchNode(this);
      rules = new TextProcessingRules();
    }

    public override ITextNode Root => rootNode;

    protected override IUndoableEdit InsertUpdate(int offset, int length)
    {
      if (length <= 0)
      {
        throw new ArgumentOutOfRangeException();
      }

      var splitPending = false;

      removedNodes.Clear();
      addedNodes.Clear();

      ITextNode removeCandidate;
      ITextPosition startOffset;
      ITextPosition endOffset;

      int start, end;
      var insertIdx = FindRemovedParagraphs(offset, out start, out end);
      if (insertIdx != -1)
      {
        startOffset = Content.CreatePosition(start, Bias.Forward);
        endOffset = Content.CreatePosition(end, Bias.Backward);
      }
      else if (rootNode.Count > 0)
      {
        removeCandidate = rootNode[rootNode.Count - 1];
        insertIdx = rootNode.Count - 1;
        startOffset = Content.CreatePosition(removeCandidate.Offset, Bias.Forward);
        endOffset = Content.CreatePosition(removeCandidate.EndOffset, Bias.Backward);
      }
      else
      {
        startOffset = Content.CreatePosition(offset, Bias.Forward);
        endOffset = Content.CreatePosition(offset + length, Bias.Backward);
        removeCandidate = null;
        insertIdx = 0;
      }

      var it = new BreakIterator<LineBreakType>(Content, rules.IsLineBreak, Math.Max(0, offset - 1), offset + length);
      if (offset > 0)
      {
        it.MoveNext();
        splitPending = it.Current != LineBreakType.None;
      }

      var cursor = offset;
      while (it.MoveNext())
      {
        var c = it.Current;
        if (c != LineBreakType.Continuation)
        {
          if (splitPending)
          {
            addedNodes.Add(new ImmutableLeafTextNode(this, startOffset, Content.CreatePosition(cursor, Bias.Backward)));
            startOffset = Content.CreatePosition(cursor, Bias.Forward);
            offset = cursor;
            splitPending = false;
          }

          if (c == LineBreakType.LineBreak)
          {
            splitPending = true;
          }
        }
        cursor += 1;
      }

      if (cursor != offset)
      {
        if (splitPending)
        {
          addedNodes.Add(new ImmutableLeafTextNode(this, startOffset, Content.CreatePosition(cursor, Bias.Backward)));
          addedNodes.Add(new ImmutableLeafTextNode(this, Content.CreatePosition(cursor, Bias.Forward), endOffset));
        }
        else
        {
          addedNodes.Add(new ImmutableLeafTextNode(this, startOffset, endOffset));
        }
      }

      var addedNodesArray = addedNodes.ToArray();
      var oldRoot = Root;
      var newRoot = rootNode.Replace(insertIdx, removedNodes.Count, addedNodesArray);
      ReplaceRoot(newRoot);
      return new ElementEdit(this, TreePath.Create(oldRoot), TreePath.Create(Root), insertIdx, addedNodesArray, removedNodes.ToArray());
    }

    protected override IUndoableEdit RemoveUpdate(int offset, int length)
    {
      var indexForOffset = rootNode.NodeIndexForOffset(offset);
      if (indexForOffset < 0)
      {
        throw new ArgumentException();
      }

      var indexForEndOffset = rootNode.NodeIndexForOffset(offset + length);
      if (indexForEndOffset < 0)
      {
        if (offset + length == rootNode.EndOffset)
        {
          indexForEndOffset = rootNode.Count - 1;
        }
        if (indexForEndOffset < 0)
        {
          throw new ArgumentException();
        }
      }

      if (indexForEndOffset == indexForOffset)
      {
        // simple case. Nothing happens.
        return new ElementEdit(this, TreePath.Create(Root), TreePath.Create(Root), indexForEndOffset, EmptyNodes, EmptyNodes);
      }

      removedNodes.Clear();
      addedNodes.Clear();

      for (var line = indexForOffset; line <= indexForEndOffset; line += 1)
      {
        removedNodes.Add(rootNode[line]);
      }

      var start = Content.CreatePosition(rootNode[indexForOffset].Offset, Bias.Forward);
      var end = Content.CreatePosition(rootNode[indexForEndOffset].EndOffset, Bias.Backward);
      addedNodes.Add(new ImmutableLeafTextNode(this, start, end));

      var replacedNodes = addedNodes.ToArray();
      var oldRoot = Root;
      ReplaceRoot(rootNode.Replace(indexForOffset, removedNodes.Count, replacedNodes));
      return new ElementEdit(this, TreePath.Create(oldRoot), TreePath.Create(Root), indexForOffset, replacedNodes, removedNodes.ToArray());
    }

    protected override void ReplaceRoot(ITextNode element)
    {
      var node = element as ImmutableBranchNode;
      if (node != null)
      {
        if (!ReferenceEquals(node.Document, this))
        {
          throw new ArgumentException("Not one of my nodes");
        }
        if (node.Parent != null)
        {
          throw new ArgumentException("Not a root node");
        }
        rootNode = node;
      }
      else
      {
        throw new ArgumentException("This document does not allow nested elements.");
      }
    }

    int FindRemovedParagraphs(int offset, out int startOffset, out int endOffset)
    {
      startOffset = offset;
      endOffset = offset;
      var index = -1;

      for (var i = 0; i < rootNode.Count; i++)
      {
        var node = rootNode[i];
        if (node.InEndOffsetRange(offset))
        {
          removedNodes.Add(node);

          startOffset = Math.Min(startOffset, node.Offset);
          endOffset = Math.Max(endOffset, node.EndOffset);

          if (index == -1)
          {
            index = i;
          }
        }
      }
      return index;
    }
  }
}