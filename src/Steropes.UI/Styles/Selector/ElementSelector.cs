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
  public class ElementSelector : ISimpleSelector, IEquatable<ElementSelector>
  {
    readonly HashSet<string> typeNames;

    public ElementSelector(string typeName)
    {
      TypeName = typeName;
      if (typeName != null)
      {
        typeNames = new HashSet<string>();
        var names = typeName.Split(null);
        for (var i = 0; i < names.Length; i++)
        {
          typeNames.Add(names[i]);
        }
      }
    }

    public ElementSelector(string primaryType, params string[] typeNames) : this(primaryType)
    {
      this.typeNames = new HashSet<string>(typeNames);
    }

    public string TypeName { get; }

    public int Weight => TypeName == null ? 1 : 10;

    public static bool operator ==(ElementSelector left, ElementSelector right)
    {
      return Equals(left, right);
    }

    public static bool operator !=(ElementSelector left, ElementSelector right)
    {
      return !Equals(left, right);
    }

    public void CollectConditionTargets(IStyledObject node, IWatchRuleFactory watchRuleFactory,
      ICollection<IWatchRule> affectedNodes)
    {
      // does nothing: Node types do not change at runtime.
    }

    public bool Equals(ElementSelector other)
    {
      if (ReferenceEquals(null, other))
      {
        return false;
      }
      if (ReferenceEquals(this, other))
      {
        return true;
      }
      if (string.Equals(TypeName, other.TypeName))
      {
        return true;
      }
      if (ReferenceEquals(typeNames, other.typeNames))
      {
        return true;
      }
      if (typeNames == null || other.typeNames == null)
      {
        return false;
      }
      return typeNames.SetEquals(other.typeNames);
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
      return Equals((ElementSelector) obj);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        return (GetHashCode(typeNames) * 397) ^ (TypeName != null ? TypeName.GetHashCode() : 0);
      }
    }
    
    int GetHashCode(HashSet<string> set)
    {
      int hc = 0;
      if (set != null)
      {
        foreach (var p in set)
        {
          hc = (hc * 397) ^ p.GetHashCode();
        }
      }
      return hc;
    }

    public bool Matches(IStyledObject styledObject)
    {
      if (TypeName == null)
      {
        return true;
      }
      return typeNames.Contains(styledObject.NodeType);
    }

    public override string ToString()
    {
      return TypeName;
    }
  }
}