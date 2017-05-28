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

namespace Steropes.UI.Widgets.TextWidgets.Documents
{
  public class ImmutableBranchNode : ITextNode
  {
    static readonly ITextNode[] EmptyNodesArray = new ITextNode[0];

    readonly ITextNode[] nodes;

    public ImmutableBranchNode(ITextDocument document, ITextNode[] nodes = null)
    {
      this.nodes = nodes ?? EmptyNodesArray;
      Document = document;
      Parent = null;
    }

    public int Count => nodes.Length;

    public ITextDocument Document { get; }

    public int EndOffset
    {
      get
      {
        if (Count == 0)
        {
          return -1;
        }
        return nodes[nodes.Length - 1].EndOffset;
      }
    }

    public bool Leaf => false;

    public int Offset
    {
      get
      {
        if (Count == 0)
        {
          return -1;
        }
        return this[0].Offset;
      }
    }

    public ITextNode Parent { get; }

    public ITextNode this[int index] => nodes[index];

    public ITextNode Replace(int offset, int length, ITextNode[] replacedNodes)
    {
      if (length == 0 && replacedNodes.Length == 0)
      {
        return this;
      }

      var newNodes = new ITextNode[nodes.Length + replacedNodes.Length - length];

      // preserve existing nodes ..
      Array.Copy(nodes, 0, newNodes, 0, offset);
      Array.Copy(replacedNodes, 0, newNodes, offset, replacedNodes.Length);
      Array.Copy(nodes, offset + length, newNodes, offset + replacedNodes.Length, nodes.Length - offset - length);
      return new ImmutableBranchNode(Document, newNodes);
    }
  }
}