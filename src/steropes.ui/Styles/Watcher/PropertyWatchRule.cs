﻿// MIT License
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

using Steropes.UI.Components;

namespace Steropes.UI.Styles.Watcher
{
  public class PropertyWatchRule : WatchRule, IEquatable<PropertyWatchRule>
  {
    public PropertyWatchRule(IWidget target, string property, object value) : base(target)
    {
      if (property == null)
      {
        throw new ArgumentNullException(nameof(property));
      }
      Property = property;
      Value = value;
      Init();
    }

    public string Property { get; }

    public object Value { get; }

    public static bool operator ==(PropertyWatchRule left, PropertyWatchRule right)
    {
      return Equals(left, right);
    }

    public static bool operator !=(PropertyWatchRule left, PropertyWatchRule right)
    {
      return !Equals(left, right);
    }

    public bool Equals(PropertyWatchRule other)
    {
      if (ReferenceEquals(null, other))
      {
        return false;
      }
      if (ReferenceEquals(this, other))
      {
        return true;
      }
      return base.Equals(other) && string.Equals(Property, other.Property) && Equals(Value, other.Value);
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
      return Equals((PropertyWatchRule)obj);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        var hashCode = base.GetHashCode();
        hashCode = (hashCode * 397) ^ Property.GetHashCode();
        hashCode = (hashCode * 397) ^ (Value?.GetHashCode() ?? 0);
        return hashCode;
      }
    }

    protected override bool IsMatch()
    {
      var existingValue = Target.GetPropertyValue(Property);
      if (Value == null)
      {
        return existingValue != null;
      }
      return Equals(existingValue, Value);
    }
  }
}