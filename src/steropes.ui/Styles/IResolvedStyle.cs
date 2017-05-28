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

using Steropes.UI.Components;
using Steropes.UI.Util;

namespace Steropes.UI.Styles
{
  /// <summary>
  ///   A resolved style is the target of the style-resolver. It receives values from presentation styles.
  /// </summary>
  public interface IResolvedStyle : IStyle
  {
    void InvalidateCaches(string reason);

    void SetResolvedValue(IStyleKey key, object value);

    void StartResolve();

    void EndResolve();
  }

  public class ResolvedStyle : IResolvedStyle
  {
    readonly FlexibleList<object> cachedValues;

    readonly IStyle elementStyle;

    readonly IWidget self;

    public ResolvedStyle(IStyleSystem styleSystem, IWidget self)
    {
      this.self = self;
      StyleSystem = styleSystem;
      ResolvedStyles = new MarkSweepStyleSheet(styleSystem);

      elementStyle = styleSystem.CreatePresentationStyle();
      elementStyle.ValueChanged += OnValueChanged;

      cachedValues = new FlexibleList<object>();
    }

    public event EventHandler<StyleEventArgs> ValueChanged;

    public IStyledObject Self => self;

    public IStyleSystem StyleSystem { get; }

    MarkSweepStyleSheet ResolvedStyles { get; }

    public bool GetValue<T>(IStyleKey key, out T value)
    {
      Resolve();

      if (elementStyle.GetValue(key, out value))
      {
        return true;
      }
      if (!elementStyle.IsExplicitlyInherited(key))
      {
        if (ResolvedStyles.GetValue(key, out value))
        {
          return true;
        }
        if (!ResolvedStyles.IsExplicitlyInherited(key) && !key.Inherit)
        {
          value = default(T);
          return false;
        }
      }

      // inheritance is potentially expensive, as we may traverse multiple 
      // layers to get to the bottom of the tree. Caching is mandatory.
      object cachedValue;
      if (TryGetValue(key, out cachedValue))
      {
        value = (T)cachedValue;
        return true;
      }

      var parentStyle = Self.GetStyleParent()?.Style;
      if (parentStyle != null)
      {
        if (parentStyle.GetValue(key, out value))
        {
          Store(key, value);
          return true;
        }
      }

      value = default(T);
      Store(key, value);
      return false;
    }

    public void InvalidateCaches(string reason)
    {
      cachedValues.Clear();
      /*
      if (invalid == false)
      {
        invalid = true;
      }
      */
    }

    public bool IsExplicitlyInherited(IStyleKey key)
    {
      if (!StyleSystem.IsRegisteredKey(key))
      {
        throw new ArgumentException($"StyleKey {key} is not registered here.");
      }
      if (elementStyle.IsExplicitlyInherited(key))
      {
        return true;
      }
      if (ResolvedStyles.IsExplicitlyInherited(key))
      {
        return true;
      }
      return false;
    }

    bool inheritableValuesChanged;

    bool valueChanged;

    public void SetResolvedValue(IStyleKey key, object value)
    {
      if (ResolvedStyles.SetValue(key, value))
      {
        if (key.Inherit)
        {
          inheritableValuesChanged = true;
        }
        valueChanged = true;
      }
    }

    public void StartResolve()
    {
      ResolvedStyles.Mark();
      inheritableValuesChanged = false;
      valueChanged = false;
    }

    public void EndResolve()
    {
      // todo This is called by the style resolver when all style rules have been applied.
      SweepChanges changesAfterSweep = ResolvedStyles.Sweep();
      if (inheritableValuesChanged || changesAfterSweep == SweepChanges.Inherited)
      {
        // inform all child-widgets that the style has changed in a way that may affect them ..
        // This will clear their caches, but does not necessarily re-apply the style rules.
        self.InvalidateStyle(true);
      }
      if (valueChanged || changesAfterSweep != SweepChanges.None)
      {
        // inform the widget that the layout has changed. This should trigger an measure/arrange cycle.
        // This implicitly informs all parent widgets that they need to recompute their layouts too.
        self.InvalidateLayout();
      }
    }

    public bool SetValue(IStyleKey key, object value)
    {
      var idx = StyleSystem.LinearIndexFor(key);
      cachedValues[idx] = null;
      if (elementStyle.SetValue(key, value))
      {
        if (key.Inherit || InheritMarker.IsInheritMarker(value))
        {
          // inherited styles are distributed to child elements, so we need to explicitly clear them all.
          self.InvalidateStyle(false);
        }

        self.InvalidateLayout();
        return true;
      }
      return false;
    }

    void OnValueChanged(object o, StyleEventArgs args)
    {
      ValueChanged?.Invoke(this, args);
    }

    void Resolve()
    {
      /*
      //// Invalid means that either the widget or the widget's parents have had changes in their style properties.
      //// Style properties can be inherited, so we have to at least clear the caches for them.
      if (invalid)
      {
        cachedValues.Clear();
        if (styleResolver.Revalidate())
        {
          Debug.WriteLine("WARNING: StyleResolver detected invalid styles.");
        }
      }
      invalid = false;
      */
    }

    void Store(IStyleKey key, object value)
    {
      var idx = StyleSystem.LinearIndexFor(key);
      cachedValues[idx] = value;
    }

    bool TryGetValue(IStyleKey key, out object value)
    {
      var idx = StyleSystem.LinearIndexFor(key);
      value = cachedValues[idx];
      return value != null;
    }
  }

  enum SweepChanges
  {
    None, Local, Inherited
  }

  class MarkSweepStyleSheet
  {
    readonly FlexibleList<IStyleKey> keys;
    readonly FlexibleList<object> values;
    readonly FlexibleList<bool> valuesTouched;

    public MarkSweepStyleSheet(IStyleSystem styleSystem)
    {
      this.StyleSystem = styleSystem;
      keys = new FlexibleList<IStyleKey>();
      values = new FlexibleList<object>();
      valuesTouched = new FlexibleList<bool>();
    }

    IStyleSystem StyleSystem { get; }

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

    public void Mark()
    {
      for (int i = 0; i < valuesTouched.Count; i += 1)
      {
        valuesTouched[i] = false;
      }
    }

    public SweepChanges Sweep()
    {
      SweepChanges haveChanges = SweepChanges.None;
      for (int i = 0; i < valuesTouched.Count; i += 1)
      {
        if (!valuesTouched[i] && values[i] != null)
        {
          values[i] = null;
          if (keys[i].Inherit)
          {
            haveChanges = SweepChanges.Inherited;
          }
          else if (haveChanges == SweepChanges.None)
          {
            haveChanges = SweepChanges.Local;
          }
        }
        valuesTouched[i] = false;
      }
      return haveChanges;
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
      var changed = !Equals(values[index], value);
      if (changed)
      {
        values[index] = value;
      }
      keys[index] = key;
      valuesTouched[index] = true;
      return changed;
    }
  }
}