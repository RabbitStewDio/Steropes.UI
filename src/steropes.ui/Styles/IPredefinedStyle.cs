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

using Steropes.UI.Util;

namespace Steropes.UI.Styles
{
  /// <summary>
  ///   A style normally used for stylesheets. It has been predefined at the start of the program and is not
  ///   expected to change at runtime.
  /// </summary>
  public interface IPredefinedStyle : IStyle
  {
    void CopyInto(IResolvedStyle style);

    void Clear();
  }

  public class PredefinedStyle : IPredefinedStyle, IEquatable<PredefinedStyle>
  {
    readonly FlexibleList<IStyleKey> keys;

    readonly FlexibleList<object> values;

    public PredefinedStyle(IStyleSystem styleSystem)
    {
      this.StyleSystem = styleSystem;
      values = new FlexibleList<object>();
      keys = new FlexibleList<IStyleKey>();
    }

    public event EventHandler<StyleEventArgs> ValueChanged;

    public IStyleSystem StyleSystem { get; }

    public static bool operator ==(PredefinedStyle left, PredefinedStyle right)
    {
      return Equals(left, right);
    }

    public static bool operator !=(PredefinedStyle left, PredefinedStyle right)
    {
      return !Equals(left, right);
    }

    public void CopyInto(IResolvedStyle style)
    {
      for (var index = 0; index < keys.Count; index += 1)
      {
        var value = values[index];
        if (value != null)
        {
          style.SetResolvedValue(keys[index], value);
        }
      }
    }

    public void Clear()
    {
      keys.Clear();
      values.Clear();
    }

    public bool Equals(PredefinedStyle other)
    {
      if (ReferenceEquals(null, other))
      {
        return false;
      }
      if (ReferenceEquals(this, other))
      {
        return true;
      }

      for (var i = 0; i < keys.Count; i++)
      {
        var k = keys[i];
        if (k == null)
        {
          continue;
        }

        var v = values[i];
        object otherValue;
        if (!other.StyleSystem.IsRegisteredKey(k))
        {
          return false;
        }
        if (!other.GetValue(k, out otherValue))
        {
          return false;
        }
        if (!Equals(otherValue, v))
        {
          return false;
        }
      }
      return true;
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
      return Equals((PredefinedStyle)obj);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        return ((values?.GetHashCode() ?? 0) * 397) ^ (keys?.GetHashCode() ?? 0);
      }
    }

    public bool GetValue<T>(IStyleKey key, out T value)
    {
      if (!StyleSystem.IsRegisteredKey(key))
      {
        throw new ArgumentException();
      }

      var index = StyleSystem.LinearIndexFor(key);
      var o = values[index];
      if (o is T)
      {
        value = (T)o;
        return true;
      }
      value = default(T);
      return false;
    }

    public bool IsExplicitlyInherited(IStyleKey key)
    {
      if (!StyleSystem.IsRegisteredKey(key))
      {
        throw new ArgumentException($"StyleKey {key} is not registered here.");
      }

      var index = StyleSystem.LinearIndexFor(key);
      return InheritMarker.IsInheritMarker(values[index]);
    }

    public bool SetValue(IStyleKey key, object value)
    {
      if (!StyleSystem.IsRegisteredKey(key))
      {
        throw new ArgumentException();
      }

      if (!InheritMarker.IsInheritMarker(value) && value != null && !key.ValueType.IsInstanceOfType(value))
      {
        throw new ArgumentException();
      }

      var index = StyleSystem.LinearIndexFor(key);
      keys[index] = value == null ? null : key;
      var changed = !Equals(values[index], value);
      values[index] = value;
      if (changed)
      {
        ValueChanged?.Invoke(this, new StyleEventArgs(key));
      }
      return changed;
    }
  }
}