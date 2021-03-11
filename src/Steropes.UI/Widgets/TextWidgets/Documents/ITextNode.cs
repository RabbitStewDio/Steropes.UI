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
  public interface ITextNode
  {
    int Count { get; }

    ITextDocument Document { get; }

    int EndOffset { get; }

    bool Leaf { get; }

    int Offset { get; }

    ITextNode this[int index] { get; }

    ITextNode Replace(int offset, int removedChildrenCount, ITextNode[] addedChildren);
  }

  public interface IStyledTextNode
  {
    TextStyle Style { get; }

    ITextNode ReplaceStyle(TextStyle style);
  }

  public static class TextNodeExtensions
  {
    public static void CheckInEndOffsetRange(this ITextNode node, string parameter, int offset)
    {
      if (InEndOffsetRange(node, offset))
      {
        throw new ArgumentOutOfRangeException(parameter, offset, FormatMessage(node, parameter, offset));
      }
    }

    public static void CheckInRange(this ITextNode node, string parameter, int offset)
    {
      if (InRange(node, offset))
      {
        throw new ArgumentOutOfRangeException(parameter, offset, FormatMessage(node, parameter, offset));
      }
    }

    public static void CheckInRange(this ITextNode node, int offset, int endOffset)
    {
      CheckInRange(node, nameof(offset), offset);
      CheckInEndOffsetRange(node, nameof(endOffset), endOffset);
      if (offset > endOffset)
      {
        throw new ArgumentOutOfRangeException($"Offset [{offset}] cannot be greater than end offset [{endOffset}].");
      }
    }

    public static ITreePath<ITextNode> CreatePathForNodeAt(this ITextNode node, int offset)
    {
      if (!node.InRange(offset))
      {
        throw new ArgumentOutOfRangeException(nameof(offset), offset, null);
      }

      ITreePath<ITextNode> path = new TreePath<ITextNode>(node);
      for (var idx = 0; idx < node.Count;)
      {
        var child = node[idx];
        if (child.InRange(offset))
        {
          path = path.Append(child);

          // restart for-loop for the new node ..
          // this is a stack-friendly tail recursion.
          node = child;
          idx = 0;
        }
        else
        {
          idx += 1;
        }
      }
      return path;
    }

    public static string ExtractNodeText(this ITextNode node)
    {
      return node.Document.TextAt(node.Offset, node.EndOffset - node.Offset);
    }

    public static int IndexOf(this ITextNode node, ITextNode child)
    {
      for (var i = 0; i < node.Count; i++)
      {
        if (ReferenceEquals(child, node[i]))
        {
          return i;
        }
      }
      return -1;
    }

    public static bool InEndOffsetRange(this ITextNode node, int offset)
    {
      if (offset < node.Offset || offset > node.EndOffset)
      {
        return false;
      }
      return true;
    }

    public static bool InRange(this ITextNode node, int offset)
    {
      if (offset < node.Offset || offset >= node.EndOffset)
      {
        return false;
      }
      return true;
    }

    public static bool IsBefore(this ITextNode node, int offset)
    {
      return offset < node.Offset;
    }

    public static int NodeIndexForOffset(this ITextNode node, int offset)
    {
      if (node.Leaf)
      {
        return -1;
      }
      if (!node.InRange(offset))
      {
        return -1;
      }

      for (var idx = 0; idx < node.Count; idx += 1)
      {
        var n = node[idx];
        if (n.InRange(offset))
        {
          return idx;
        }
      }
      return -1;
    }

    public static ITreePath<ITextNode> ReplaceAll(ITreePath<ITextNode> oldPath, ITextNode newNode)
    {
      var oldNode = oldPath.Node;
      if (oldPath.Parent == null)
      {
        return new TreePath<ITextNode>(newNode);
      }

      var parentPath = oldPath.Parent;
      var parentNode = parentPath.Node;
      var idx = parentNode.IndexOf(oldNode);
      var changedNode = parentNode.Replace(idx, 1, new[] { newNode });
      var changedPath = ReplaceAll(parentPath, changedNode);
      return changedPath.Append(newNode);
    }

    static string FormatMessage(this ITextNode node, string parameterName, int value)
    {
      return $"Value for '{parameterName}' must be in range [{node.Offset}, {node.EndOffset}] but was {value}.";
    }
  }
}