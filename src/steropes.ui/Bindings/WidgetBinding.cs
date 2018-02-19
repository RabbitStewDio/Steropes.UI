using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using Steropes.UI.Components;
using Steropes.UI.Widgets.Container;

namespace Steropes.UI.Bindings
{
  public static class WidgetBinding
  {
    public static IObservableListBinding<WidgetAndConstraint<TConstraint>> ToBinding<TConstraint>(
      this WidgetContainer<TConstraint> container)
    {
      return new WidgetBinding<TConstraint>(container);
    }

    public static IBindingSubscription BindTo<TConstraint>(
      this IReadOnlyObservableListBinding<WidgetAndConstraint<TConstraint>> self,
      WidgetContainer<TConstraint> widget)
    {
      return new WidgetSink<TConstraint>(self, widget);
    }
  }

  class WidgetSink<TConstraint> : WidgetContainerSynchronizer<TConstraint>, IBindingSubscription
  {
    bool processingEvent;

    public WidgetSink(IReadOnlyObservableListBinding<WidgetAndConstraint<TConstraint>> sourceList,
                      WidgetContainer<TConstraint> target) : base(sourceList, target)
    {
      if (target.Count != 0)
      {
        throw new InvalidOperationException("Cannot bind to a widget that already contains other content.");
      }

      target.ChildrenChanged += OnValidateChildrenChanged;

      SourceList.CollectionChanged += CheckedChangeHandler;
    }

    void CheckedChangeHandler(object sender, NotifyCollectionChangedEventArgs e)
    {
      if (processingEvent)
      {
        return;
      }

      try
      {
        processingEvent = true;
        HandleSourceCollectionChanged(sender, e);
      }
      finally
      {
        processingEvent = false;
      }
    }

    void OnValidateChildrenChanged(object sender, ContainerEventArgs e)
    {
      if (processingEvent)
      {
        // probably ok
        return;
      }

      // Not a tooltip or overlay change?
      if (e.Index != -1)
      {
        throw new InvalidOperationException(
          "This widget has an active binding and should not be modified from elsewhere.");
      }
    }

    public void Dispose()
    {
      SourceList.CollectionChanged -= HandleSourceCollectionChanged;
      Target.ChildrenChanged -= OnValidateChildrenChanged;
    }

    public IReadOnlyList<IBindingSubscription> Sources => new IBindingSubscription[] { SourceList };
  }

  public class WidgetBinding<TConstraint> : ObservableListBindingBase<WidgetAndConstraint<TConstraint>>
  {
    const string IndexerName = "Item[]";
    readonly WidgetContainer<TConstraint> widget;

    public WidgetBinding(WidgetContainer<TConstraint> widget)
    {
      this.widget = widget ?? throw new ArgumentNullException(nameof(widget));
      this.widget.ChildrenChanged += OnWidgetChanged;
    }

    void OnWidgetChanged(object sender, ContainerEventArgs e)
    {
      if (e.Index == -1)
      {
        // ignore tooltip and other overlay elements.
        return;
      }

      if (e.RemovedChild != null)
      {
        CollectionChanged?.Invoke(
          this,
          Create(NotifyCollectionChangedAction.Remove, e.Index, e.RemovedChild, (TConstraint) e.RemovedConstraints));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IndexerName)));
      }

      if (e.AddedChild != null)
      {
        CollectionChanged?.Invoke(
          this, Create(NotifyCollectionChangedAction.Add, e.Index, e.AddedChild, (TConstraint) e.AddedConstraints));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IndexerName)));
      }
    }

    NotifyCollectionChangedEventArgs Create(NotifyCollectionChangedAction action,
                                            int index,
                                            IWidget w,
                                            TConstraint constraint)
    {
      var items = new List<WidgetAndConstraint<TConstraint>> { new WidgetAndConstraint<TConstraint>(w, constraint) };
      return new NotifyCollectionChangedEventArgs(action, items, index);
    }

    public override int Count => widget.Count;

    public override void Dispose()
    {
    }

    public override IReadOnlyList<IBindingSubscription> Sources => new IBindingSubscription[0];

    public override void Move(int sourceIdx, int targetIdx)
    {
      var removedItem = this[sourceIdx];
      RemoveAt(sourceIdx);
      Insert(targetIdx, removedItem);
    }

    public override void Add(WidgetAndConstraint<TConstraint> item)
    {
      widget.Add(item.Widget, item.Constraint);
    }

    public override void Clear()
    {
      widget.Clear();
    }

    public override bool Remove(WidgetAndConstraint<TConstraint> item)
    {
      if (widget.IndexOf(item.Widget) == -1)
      {
        return false;
      }

      widget.Remove(item.Widget);
      return true;
    }

    public override void Insert(int index, WidgetAndConstraint<TConstraint> item)
    {
      widget.Add(item.Widget, index, item.Constraint);
    }

    public override void RemoveAt(int index)
    {
      widget.Remove(index);
    }

    public override WidgetAndConstraint<TConstraint> this[int index]
    {
      get { return widget.WidgetsWithConstraints[index]; }
      set
      {
        widget.Remove(index);
        widget.Add(value.Widget, index, value.Constraint);
      }
    }

    public override bool IsFixedSize => false;

    public override event PropertyChangedEventHandler PropertyChanged;
    public override event NotifyCollectionChangedEventHandler CollectionChanged;
  }
}