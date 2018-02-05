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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using Steropes.UI.Components;
using Steropes.UI.Styles.Watcher;
using Steropes.UI.Util;
using Steropes.UI.Widgets.Container;

namespace Steropes.UI.Styles
{
  public interface IStyleResolver
  {
    ObservableCollection<IStyleRule> StyleRules { get; }

    void AddRoot(IWidget root);

    IResolvedStyle CreateStyleFor(IWidget widget);

    // ReSharper disable once UnusedMember.Global
    void RemoveRoot(IWidget root);

    bool Revalidate();

    bool IsRegistered(IWidget widget);
  }

  /// <summary>
  ///   Resolves styles for the current UI. The style resolver monitors the composition of the UI tree and
  ///   re-resolves rules as the tree changes. For rules that have additional conditions (match by style-class,
  ///   pseudo-class or attribute contents), the system also monitors property-change events.
  /// </summary>
  public class StyleResolver : IStyleResolver
  {
    readonly HashSet<IWatchRule> affectedNodesSet;

    readonly EventHandler<ContainerEventArgs> onChildrenChangedHandler;

    readonly Dictionary<IWidget, WidgetChangeTracker> registeredWidgets;

    readonly WidgetWatchRuleFactory ruleFactory;

    readonly List<IStyleRule> styleRules;

    readonly Comparison<IStyleRule> styleRuleSortOrder = StyleRule.StyleRuleSortOrder;

    readonly IStyleSystem styleSystem;
    readonly TraceSource tracer = TracingUtil.Create<StyleResolver>();

    public StyleResolver(IStyleSystem styleSystem)
    {
      this.styleSystem = styleSystem;
      ruleFactory = new WidgetWatchRuleFactory();
      registeredWidgets = new Dictionary<IWidget, WidgetChangeTracker>();
      StyleRules = new ObservableCollection<IStyleRule>();
      StyleRules.CollectionChanged += OnRuleCompositionChanged;
      onChildrenChangedHandler = OnChildrenChanged;
      affectedNodesSet = new HashSet<IWatchRule>();
      styleRules = new List<IStyleRule>();
    }

    public bool Dirty { get; private set; }

    public ObservableCollection<IStyleRule> StyleRules { get; }

    public void AddRoot(IWidget root)
    {
      Install(root);
    }

    public IResolvedStyle CreateStyleFor(IWidget widget)
    {
      return new ResolvedStyle(styleSystem, widget);
    }

    public void RemoveRoot(IWidget root)
    {
      Uninstall(root);
    }

    public bool Revalidate()
    {
      if (Dirty)
      {
        var sw = Stopwatch.StartNew();
        var widgetCount = 0;
        foreach (var pair in registeredWidgets)
        {
          var widget = pair.Key;
          var value = pair.Value;
          if (ResolveAsRegisteredWidget(widget, value))
          {
            widgetCount += 1;
          }
        }

        Dirty = false;
        sw.Stop();

        tracer.TraceEvent(TraceEventType.Verbose, 0,
                          "Finished revalidate of {0} widgets in {1} seconds.", widgetCount,
                          sw.Elapsed.TotalSeconds);
        return true;
      }
      return false;
    }

    public bool IsRegistered(IWidget widget)
    {
      return registeredWidgets.ContainsKey(widget);
    }

    public void MarkDirty(IWidget w)
    {
      WidgetChangeTracker tracker;
      if (registeredWidgets.TryGetValue(w, out tracker))
      {
        // is a registered widget?
        tracker.Dirty = WidgetState.Dirty;
        Dirty = true;
      }
    }

    public void ResolveStyle(IWidget widget)
    {
      WidgetChangeTracker t;
      if (registeredWidgets.TryGetValue(widget, out t))
      {
        ResolveAsRegisteredWidget(widget, t);
        return;
      }

      ////Debug.WriteLine("Resolving unregistered element: " + widget);
      styleRules.Clear();
      for (var index = 0; index < StyleRules.Count; index++)
      {
        var rule = StyleRules[index];

        // we collect style information regardless on whether conditions match.
        // conditions may change often over the lifetime of this object, so we want
        // to keep an eye on every potential rule that  may apply.
        if (rule.Selector.Matches(widget))
        {
          styleRules.Add(rule);
        }
      }

      // apply resolved styles
      styleRules.Sort(styleRuleSortOrder);
      for (var index = 0; index < styleRules.Count; index++)
      {
        var rule = styleRules[index];
        rule.Style.CopyInto(widget.ResolvedStyle);
      }
      widget.ResolvedStyle.InvalidateCaches("After resolve.");
    }

    void Install(IWidget widget)
    {
      if (registeredWidgets.ContainsKey(widget))
      {
        return;
      }

      registeredWidgets.Add(widget, new WidgetChangeTracker(this, widget));
      Dirty = true;
      widget.ChildrenChanged += onChildrenChangedHandler;
      widget.VisitStructuralChildren(Install);
    }

    void OnChildrenChanged(object sender, ContainerEventArgs e)
    {
      var w = sender as IWidget;
      if (w == null)
      {
        return;
      }

      if (registeredWidgets.ContainsKey(w))
      {
        Dirty = true;
        if (e.RemovedChild != null)
        {
          Uninstall(e.RemovedChild);
        }
        if (e.AddedChild != null)
        {
          Install(e.AddedChild);
        }
      }
    }

    void OnRuleCompositionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      Dirty = true;
      foreach (var pair in registeredWidgets)
      {
        var widgetChangeTracker = registeredWidgets[pair.Key];
        widgetChangeTracker.Dirty = WidgetState.Invalidated;
      }
    }

    bool ResolveAsRegisteredWidget(IWidget widget, WidgetChangeTracker value)
    {
      if (value.Dirty != WidgetState.Clean)
      {
        ////Debug.WriteLine($"Resolving registered element: {widget}");
        // rebind style rules
        affectedNodesSet.Clear();
        styleRules.Clear();
        for (var index = 0; index < StyleRules.Count; index++)
        {
          var rule = StyleRules[index];

          // we collect style information regardless on whether conditions match.
          // conditions may change often over the lifetime of this object, so we want
          // to keep an eye on every potential rule that  may apply.
          rule.Selector.CollectConditionTargets(widget, ruleFactory, affectedNodesSet);
          if (rule.Selector.Matches(widget))
          {
            styleRules.Add(rule);
          }
        }

        value.ClearMonitors();
        foreach (var match in affectedNodesSet)
        {
          value.RegisterMonitor(match);
        }

        widget.ResolvedStyle.StartResolve();

        // apply resolved styles
        styleRules.Sort(styleRuleSortOrder);
        for (var index = 0; index < styleRules.Count; index++)
        {
          var rule = styleRules[index];
          rule.Style.CopyInto(widget.ResolvedStyle);
        }

        widget.ResolvedStyle.EndResolve();
        value.Dirty = WidgetState.Clean;
        return true;
      }
      return false;
    }

    void Uninstall(IWidget widget)
    {
      WidgetChangeTracker tracker;
      if (registeredWidgets.TryGetValue(widget, out tracker))
      {
        registeredWidgets.Remove(widget);
        tracker.ClearMonitors();
        widget.ChildrenChanged -= onChildrenChangedHandler;
        widget.VisitStructuralChildren(Uninstall);
      }
    }

    enum WidgetState
    {
      Clean,

      /// <summary>
      ///   Something in the widget or the widget-composition changed.
      /// </summary>
      Dirty,

      /// <summary>
      ///   The style rules have changed and we have to invalidate all caches.
      /// </summary>
      Invalidated
    }

    class WidgetChangeTracker
    {
      readonly EventHandler<EventArgs> onStateChangedHandler;

      readonly StyleResolver resolver;

      readonly IWidget widget;

      public WidgetChangeTracker(StyleResolver resolver, IWidget widget)
      {
        this.resolver = resolver;
        this.widget = widget;
        onStateChangedHandler = OnWidgetStateChanged;
        DependentWidgets = new HashSet<IWatchRule>();
        Dirty = WidgetState.Dirty;
      }

      HashSet<IWatchRule> DependentWidgets { get; }

      public WidgetState Dirty { get; set; }

      public void ClearMonitors()
      {
        foreach (var rule in DependentWidgets)
        {
          rule.Remove();
          rule.StateChanged -= onStateChangedHandler;
        }
        DependentWidgets.Clear();
      }

      public void RegisterMonitor(IWatchRule match)
      {
        DependentWidgets.Add(match);
        match.StateChanged += onStateChangedHandler;
      }

      void OnWidgetStateChanged(object sender, EventArgs e)
      {
        resolver.MarkDirty(widget);
        widget.ResolvedStyle.InvalidateCaches("Widget State change");
      }
    }

    class WidgetWatchRuleFactory : IWatchRuleFactory
    {
      public IWatchRule CreatePropertyWatcher(IStyledObject styledObject, string property, object value)
      {
        var widget = styledObject as IWidget;
        if (widget == null)
        {
          throw new ArgumentException();
        }
        return new PropertyWatchRule(widget, property, value);
      }

      public IWatchRule CreatePseudoClassWatcher(IStyledObject styledObject, string styleClass)
      {
        var widget = styledObject as IWidget;
        if (widget == null)
        {
          throw new ArgumentException();
        }
        return new PseudoClassWatchRule(widget, styleClass);
      }

      public IWatchRule CreateStyleClassWatcher(IStyledObject styledObject, string styleClass)
      {
        var widget = styledObject as IWidget;
        if (widget == null)
        {
          throw new ArgumentException();
        }

        return new StyleClassWatchRule(widget, styleClass);
      }
    }
  }
}