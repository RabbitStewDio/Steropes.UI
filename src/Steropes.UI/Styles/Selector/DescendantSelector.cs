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

using Steropes.UI.Styles.Watcher;

namespace Steropes.UI.Styles.Selector
{
  public class DescendantSelector : IStyleSelector, IEquatable<DescendantSelector>
  {
    public DescendantSelector(ISimpleSelector selector, IStyleSelector parentSelector, bool directChild = false)
    {
      DirectChild = directChild;
      Selector = selector;
      AnchestorSelector = parentSelector;
    }

    public IStyleSelector AnchestorSelector { get; }

    public bool DirectChild { get; }

    public ISimpleSelector Selector { get; }

    public int Weight
    {
      get
      {
        var value = 0;
        value += Selector?.Weight ?? 0;
        value += AnchestorSelector?.Weight ?? 0;
        return value;
      }
    }

    public static bool operator ==(DescendantSelector left, DescendantSelector right)
    {
      return Equals(left, right);
    }

    public static bool operator !=(DescendantSelector left, DescendantSelector right)
    {
      return !Equals(left, right);
    }

    public void CollectConditionTargets(IStyledObject node, IWatchRuleFactory watchRuleFactory, ICollection<IWatchRule> affectedNodes)
    {
      Selector.CollectConditionTargets(node, watchRuleFactory, affectedNodes);

      var parent = node.GetStyleParent();
      while (parent != null)
      {
        AnchestorSelector.CollectConditionTargets(parent, watchRuleFactory, affectedNodes);
        if (DirectChild)
        {
          break;
        }
        parent = parent.GetStyleParent();
      }
    }

    public bool Equals(DescendantSelector other)
    {
      if (ReferenceEquals(null, other))
      {
        return false;
      }
      if (ReferenceEquals(this, other))
      {
        return true;
      }
      return DirectChild == other.DirectChild && Equals(Selector, other.Selector) && Equals(AnchestorSelector, other.AnchestorSelector);
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
      if (obj.GetType() != this.GetType())
      {
        return false;
      }
      return Equals((DescendantSelector)obj);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        var hashCode = DirectChild.GetHashCode();
        hashCode = (hashCode * 397) ^ (Selector != null ? Selector.GetHashCode() : 0);
        hashCode = (hashCode * 397) ^ (AnchestorSelector != null ? AnchestorSelector.GetHashCode() : 0);
        return hashCode;
      }
    }

    public bool Matches(IStyledObject styledObject)
    {
      if (!Selector.Matches(styledObject))
      {
        return false;
      }

      var parent = styledObject.GetStyleParent();
      while (parent != null)
      {
        if (AnchestorSelector.Matches(parent))
        {
          return true;
        }
        if (DirectChild)
        {
          return false;
        }
        parent = parent.GetStyleParent();
      }
      return false;
    }

    public override string ToString()
    {
      if (DirectChild)
      {
        return $"{AnchestorSelector} > {Selector}";
      }
      return $"{AnchestorSelector} {Selector}";
    }
  }
}