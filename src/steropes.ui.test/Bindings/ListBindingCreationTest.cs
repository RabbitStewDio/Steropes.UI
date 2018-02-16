using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using FluentAssertions;
using FluentAssertions.Events;
using NUnit.Framework;
using Steropes.UI.Bindings;

namespace Steropes.UI.Test.Bindings
{
  public class ListBindingCreationTest
  {
    [Test]
    public void Create_From_ObservableCollection()
    {
      var backend = new ObservableCollection<string>();
      var binding = backend.ToBinding();

      using (var monitoredBinding = binding.Monitor<INotifyCollectionChanged>())
      {
        backend.Add("first");
        EventAssertions<INotifyCollectionChanged> ass = monitoredBinding.Should();
        ass
          .RaiseCollectionChange(binding, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, BindingAssertions.AsList("first"), 0));
      }
    }

    [Test]
    public void Create_From_ReadOnlyObservableCollection()
    {
      var backend = new ObservableCollection<string>();
      var roBackend = new ReadOnlyObservableCollection<string>(backend);
      var binding = roBackend.ToBinding();
      using (var monitoredBinding = binding.Monitor<INotifyCollectionChanged>())
      {
        backend.Add("first");
        monitoredBinding.Should()
          .RaiseCollectionChange(binding, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, BindingAssertions.AsList("first"), 0));

        roBackend.Should().BeEquivalentTo("first");
        binding.Should().BeEquivalentTo("first");
      }
    }

    [Test]
    public void Create_From_Array()
    {
      var backend = new BindingFixtureElement<string[], List<string>>();
      var binding = backend.BindingFor(e => e.Field).ToListBinding();

      using (var monitoredBinding = binding.Monitor<INotifyCollectionChanged>())
      {
        backend.Field = new[] { "Test" };
        backend.OnPropertyChanged(nameof(backend.Field));

        monitoredBinding.Should()
          .RaiseCollectionChange(binding, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

        binding.Should().BeEquivalentTo("Test");
      }
    }

    [Test]
    public void Create_From_ReadOnlyList()
    {
      var backend = new BindingFixtureElement<string[], List<string>>();
      var binding = backend.BindingFor(e => e.Property).ToListBinding();

      using (var monitoredBinding = binding.Monitor<INotifyCollectionChanged>())
      {
        backend.Property = new List<string> { "Test" };
        backend.OnPropertyChanged(nameof(backend.Property));

        monitoredBinding.Should()
          .RaiseCollectionChange(binding, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

        binding.Should().BeEquivalentTo("Test");
      }
    }

    [Test]
    public void Modify_Binding_Reflects_In_Backend()
    {
      var backend = new ObservableCollection<string>();
      var binding = backend.ToBinding();

      using (var monitoredBinding = binding.Monitor<INotifyCollectionChanged>())
      {
        binding.Add("first");
        monitoredBinding.Should()
          .RaiseCollectionChange(binding, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, BindingAssertions.AsList("first"), 0));
        backend.Should().BeEquivalentTo("first");
        binding.Should().BeEquivalentTo("first");
      }
    }

    [Test]
    public void Create_ReadOnlyList_From_ObservableList()
    {
      var backend = new ObservableCollection<string>();
      var binding = backend.ToBinding().ItemsBinding();

      using (var monitoredBinding = binding.Monitor<INotifyPropertyChanged>())
      {
        backend.Add("Test");

        monitoredBinding.Should().RaisePropertyChange(binding, nameof(IReadOnlyObservableValue.Value));

        binding.Value.Should().BeEquivalentTo("Test");
      }
    }

    [Test]
    public void Create_ItemBinding()
    {
      var backend = new ObservableCollection<string>();
      var binding = new ReadOnlyObservableCollection<string>(backend).ToBinding().ItemAt(1);

      using (var monitoredBinding = binding.Monitor<INotifyPropertyChanged>())
      {
        backend.Add("Test");
        monitoredBinding.Should().NotRaise(nameof(INotifyPropertyChanged.PropertyChanged));

        binding.Value.Should().BeNull();
      }

      using (var monitoredBinding = binding.Monitor<INotifyPropertyChanged>())
      {
        backend.Add("Test2");
        monitoredBinding.Should().RaisePropertyChange(binding, nameof(IReadOnlyObservableValue.Value));

        binding.Value.Should().Be("Test2");
      }

      using (var monitoredBinding = binding.Monitor<INotifyPropertyChanged>())
      {
        backend.Insert(0, "TestFirst");
        monitoredBinding.Should().RaisePropertyChange(binding, nameof(IReadOnlyObservableValue.Value));

        binding.Value.Should().Be("Test");
      }
    }

    [Test]
    public void Create_TwoWay_ItemBinding()
    {
      var backend = new ObservableCollection<string>();
      var binding = backend.ToBinding().ItemAt(1);

      using (var monitoredBinding = binding.Monitor<INotifyPropertyChanged>())
      {
        backend.Add("Test");
        monitoredBinding.Should().NotRaise(nameof(INotifyPropertyChanged.PropertyChanged));

        binding.Value.Should().BeNull();
      }

      using (var monitoredBinding = binding.Monitor<INotifyPropertyChanged>())
      {
        backend.Add("Test2");
        monitoredBinding.Should().RaisePropertyChange(binding, nameof(IReadOnlyObservableValue.Value));

        binding.Value.Should().Be("Test2");
      }

      using (var monitoredBinding = binding.Monitor<INotifyPropertyChanged>())
      {
        backend.Insert(0, "TestFirst");
        monitoredBinding.Should().RaisePropertyChange(binding, nameof(IReadOnlyObservableValue.Value));

        binding.Value.Should().Be("Test");
      }
    }

    [Test]
    public void Create_TwoWay_ItemBinding_Modifications()
    {
      var backend = new ObservableCollection<string>();
      var binding = backend.ToBinding().ItemAt(10);

      using (var monitoredBinding = binding.Monitor<INotifyPropertyChanged>())
      {
        binding.Value = "test";
        monitoredBinding.Should().RaisePropertyChange(binding, nameof(IReadOnlyObservableValue.Value));

        binding.Value.Should().Be("test");
        backend.Count.Should().Be(11);
        backend[10].Should().Be("test");
      }

    }
  }
}