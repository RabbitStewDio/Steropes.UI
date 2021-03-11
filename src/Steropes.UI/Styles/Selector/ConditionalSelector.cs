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

using Steropes.UI.Styles.Conditions;
using Steropes.UI.Styles.Watcher;

namespace Steropes.UI.Styles.Selector
{
  public class ConditionalSelector : ISimpleSelector, IEquatable<ConditionalSelector>
  {
    public ConditionalSelector(ISimpleSelector selector, ICondition condition)
    {
      Selector = selector;
      Condition = condition;
    }

    public ICondition Condition { get; }

    public ISimpleSelector Selector { get; }

    public int Weight => Selector.Weight + Condition.Weight;

    public static bool operator ==(ConditionalSelector left, ConditionalSelector right)
    {
      return Equals(left, right);
    }

    public static bool operator !=(ConditionalSelector left, ConditionalSelector right)
    {
      return !Equals(left, right);
    }

    public void CollectConditionTargets(IStyledObject node, IWatchRuleFactory watchRuleFactory, ICollection<IWatchRule> affectedNodes)
    {
      // node types and composition are not changing. If the composition changed, we rebuild the
      // whole monitoring from scratch.
      if (Selector.Matches(node))
      {
        Condition.CollectConditionTargets(node, watchRuleFactory, affectedNodes);
      }
    }

    public bool Equals(ConditionalSelector other)
    {
      if (ReferenceEquals(null, other))
      {
        return false;
      }
      if (ReferenceEquals(this, other))
      {
        return true;
      }
      return Equals(Selector, other.Selector) && Equals(Condition, other.Condition);
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
      return Equals((ConditionalSelector)obj);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        return ((Selector != null ? Selector.GetHashCode() : 0) * 397) ^ (Condition != null ? Condition.GetHashCode() : 0);
      }
    }

    public bool Matches(IStyledObject styledObject)
    {
      if (Selector.Matches(styledObject))
      {
        return Condition.Matches(styledObject);
      }
      return false;
    }

    public override string ToString()
    {
      return $"{Selector}{Condition}";
    }
  }
}