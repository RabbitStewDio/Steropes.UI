using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Steropes.UI.Bindings;
using Steropes.UI.Widgets;
using Steropes.UI.Widgets.Container;

namespace Steropes.UI.Test.Bindings
{
  public class WidgetBindingTests
  {
    [Test]
    public void CreateBinding()
    {
      var style = LayoutTestStyle.Create();
      BoxGroup backend = new BoxGroup(style);
      var label = new Label(style);

      var binding = backend.ToBinding();
      using (var monitoredBinding = binding.Monitor<INotifyCollectionChanged>())
      {
        backend.Add(label);
        monitoredBinding.Should().RaiseCollectionChange(
          binding,
          new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,
                                               BindingAssertions.AsList(new WidgetAndConstraint<bool>(label)),
                                               0));

        backend.WidgetsWithConstraints.Should().BeEquivalentTo(new WidgetAndConstraint<bool>(label));
        binding.Should().BeEquivalentTo(new WidgetAndConstraint<bool>(label));
      }
    }

    [Test]
    public void ValidateWidgetEvents()
    {
      var style = LayoutTestStyle.Create();
      BoxGroup backend = new BoxGroup(style);
      var label = new Label(style);

      var d = Substitute.For<EventHandler<ContainerEventArgs>>();
      backend.ChildrenChanged += d;

      backend.Add(label);
      d.Received().Invoke(backend, new ContainerEventArgs(0, null, null, label, false));

      backend.Should().BeEquivalentTo(new[] { label });
    }

    [Test]
    public void CreateBinding_And_Modify_Binding()
    {
      var style = LayoutTestStyle.Create();
      BoxGroup backend = new BoxGroup(style);
      var label = new Label(style);

      var binding = backend.ToBinding();
      using (var monitoredBinding = binding.Monitor<INotifyCollectionChanged>())
      {
        binding.Add(new WidgetAndConstraint<bool>(label));
        var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,
                                                             BindingAssertions.AsList(new WidgetAndConstraint<bool>(label)),
                                                             0);
        monitoredBinding.Should().RaiseCollectionChange(
          binding,
          eventArgs);

        backend.WidgetsWithConstraints.Should().BeEquivalentTo(new WidgetAndConstraint<bool>(label));
        binding.Should().BeEquivalentTo(new WidgetAndConstraint<bool>(label));
      }
    }

    [Test]
    public void BindToMappingTest()
    {
      // Note: T
      var style = LayoutTestStyle.Create();
      var backend = new BoxGroup(style);
      
      var sourceList = new ObservableCollection<WidgetAndConstraint<bool>>();
      sourceList.ToBinding().BindTo(backend);

      var widget = new Label(style);
      sourceList.Add(new WidgetAndConstraint<bool>(widget));
      backend[0].ShouldBeSameObjectReference(widget);
    }

    [Test]
    public void BindTo_Fails_When_Widget_not_empty()
    {
      // Note: T
      var style = LayoutTestStyle.Create();
      var backend = new BoxGroup(style)
      {
        new Label(style)
      };

      try
      {
        var sourceList = new ObservableCollection<WidgetAndConstraint<bool>>();
        sourceList.ToBinding().BindTo(backend);
        throw new AssertionException("Expected to raise exception.");
      }
      catch (InvalidOperationException)
      {
        // ok 
      }


    }

    [Test]
    public void BindTo_Conflicts_with_widget_changes()
    {
      // Note: T
      var style = LayoutTestStyle.Create();
      var backend = new BoxGroup(style);

        var sourceList = new ObservableCollection<WidgetAndConstraint<bool>>();
        sourceList.ToBinding().BindTo(backend);
      try
      {
        backend.Add(new Label(style));
        throw new AssertionException("Expected to raise exception.");
      }
      catch (InvalidOperationException)
      {
        // ok 
      }
    }
  }
}