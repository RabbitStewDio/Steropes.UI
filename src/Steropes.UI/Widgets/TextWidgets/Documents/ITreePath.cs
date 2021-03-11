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
  public interface ITreePath<T>
  {
    T Node { get; }

    ITreePath<T> Parent { get; }

    ITreePath<T> Append(T node);
  }

  public class TreePath<T> : ITreePath<T>, IEquatable<TreePath<T>>
  {
    public TreePath(T node, ITreePath<T> parent = null)
    {
      if (node == null)
      {
        throw new ArgumentNullException(nameof(node));
      }

      Node = node;
      Parent = parent;
    }

    public T Node { get; }

    public ITreePath<T> Parent { get; }

    public static bool operator ==(TreePath<T> left, TreePath<T> right)
    {
      return Equals(left, right);
    }

    public static bool operator !=(TreePath<T> left, TreePath<T> right)
    {
      return !Equals(left, right);
    }

    public ITreePath<T> Append(T node)
    {
      return new TreePath<T>(node, this);
    }

    public bool Equals(TreePath<T> other)
    {
      if (ReferenceEquals(null, other))
      {
        return false;
      }
      if (ReferenceEquals(this, other))
      {
        return true;
      }
      return ReferenceEquals(Node, other.Node) && Equals(Parent, other.Parent);
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj))
      {
        return false;
      }
      if (ReferenceEquals(this, obj))
      {
        return true;
      }
      if (obj.GetType() != GetType())
      {
        return false;
      }
      return Equals((TreePath<T>)obj);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        return (Node.GetHashCode() * 397) ^ (Parent?.GetHashCode() ?? 0);
      }
    }
  }

  public static class TreePath
  {
    public static ITreePath<T> Create<T>(T root, params T[] others)
    {
      ITreePath<T> path = new TreePath<T>(root);
      for (var index = 0; index < others.Length; index++)
      {
        var other = others[index];
        path = path.Append(other);
      }
      return path;
    }

    public static T GetRoot<T>(this ITreePath<T> path)
    {
      while (path.Parent != null)
      {
        path = path.Parent;
      }
      return path.Node;
    }
  }
}