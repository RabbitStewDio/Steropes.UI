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

namespace Steropes.UI.Styles.Conditions
{
  public class NotCondition : ICondition, IEquatable<NotCondition>
  {
    public NotCondition(ICondition condition)
    {
      Condition = condition;
    }

    public ICondition Condition { get; }

    public int Weight => Condition.Weight + 1;

    public static bool operator ==(NotCondition left, NotCondition right)
    {
      return Equals(left, right);
    }

    public static bool operator !=(NotCondition left, NotCondition right)
    {
      return !Equals(left, right);
    }

    public void CollectConditionTargets(IStyledObject node, IWatchRuleFactory watchRuleFactory, ICollection<IWatchRule> affectedNodes)
    {
      Condition.CollectConditionTargets(node, watchRuleFactory, affectedNodes);
    }

    public bool Equals(NotCondition other)
    {
      if (ReferenceEquals(null, other))
      {
        return false;
      }
      if (ReferenceEquals(this, other))
      {
        return true;
      }
      return Equals(Condition, other.Condition);
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
      return Equals((NotCondition)obj);
    }

    public override int GetHashCode()
    {
      return Condition != null ? Condition.GetHashCode() : 0;
    }

    public bool Matches(IStyledObject styledObject)
    {
      return !Condition.Matches(styledObject);
    }

    public override string ToString()
    {
      return $"not({Condition})";
    }
  }
}