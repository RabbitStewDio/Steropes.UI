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

using Steropes.UI.Styles.Watcher;

namespace Steropes.UI.Styles.Conditions
{
  public class AttributeCondition : ICondition, IEquatable<AttributeCondition>
  {
    public AttributeCondition(string property, object value)
    {
      Property = property;
      Value = value;
    }

    public string Property { get; }

    public object Value { get; }

    public int Weight => 1000;

    public static bool operator ==(AttributeCondition left, AttributeCondition right)
    {
      return Equals(left, right);
    }

    public static bool operator !=(AttributeCondition left, AttributeCondition right)
    {
      return !Equals(left, right);
    }

    public void CollectConditionTargets(IStyledObject node, IWatchRuleFactory watchRuleFactory, ICollection<IWatchRule> affectedNodes)
    {
      affectedNodes.Add(watchRuleFactory.CreatePropertyWatcher(node, Property, Value));
    }

    public bool Equals(AttributeCondition other)
    {
      if (ReferenceEquals(null, other))
      {
        return false;
      }
      if (ReferenceEquals(this, other))
      {
        return true;
      }
      return string.Equals(Property, other.Property) && Equals(Value, other.Value);
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
      return Equals((AttributeCondition)obj);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        return ((Property != null ? Property.GetHashCode() : 0) * 397) ^ (Value != null ? Value.GetHashCode() : 0);
      }
    }

    public bool Matches(IStyledObject styledObject)
    {
      var propertyValue = styledObject.GetPropertyValue(Property);
      if (Value == null)
      {
        return propertyValue != null;
      }
      return Equals(propertyValue, Value);
    }

    public override string ToString()
    {
      if (Value == null)
      {
        return $"[{Property}]";
      }
      return $"[{Property}={Quote(Value)}]";
    }

    protected string Quote(object valueRaw)
    {
      if (valueRaw == null)
      {
        return "\"\"";
      }

      var b = new StringBuilder();
      b.Append("\"");
      var rawString = valueRaw.ToString();
      for (var index = 0; index < rawString.Length; index++)
      {
        var c = rawString[index];
        if (c == '\n')
        {
          b.Append("\\n");
          continue;
        }
        if (c == '\r')
        {
          b.Append("\\r");
          continue;
        }
        if (c == '\t')
        {
          b.Append("\\t");
          continue;
        }
        if (c == '\\')
        {
          b.Append("\\\\");
          continue;
        }
        if (c == '"')
        {
          b.Append("\\\"");
          continue;
        }
        b.Append(c);
      }
      b.Append("\"");
      return b.ToString();
    }
  }
}