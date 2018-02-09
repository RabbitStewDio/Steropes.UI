using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Events;
using NUnit.Framework;
using Steropes.UI.Bindings;
using static System.Collections.Specialized.NotifyCollectionChangedAction;
using static Steropes.UI.Test.Bindings.BindingAssertions;

namespace Steropes.UI.Test.Bindings
{
  public class BindingTargetTest
  {
    [Test]
    public void BindToTest()
    {
      var sink = "";
      var value = new ObservableValue<string>("A");
      value.BindTo(s => sink = s);

      value.Value = "B";
      sink.Should().Be("B");
    }
  }

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
          .RaiseCollectionChange(binding, new NotifyCollectionChangedEventArgs(Add, AsList("first"), 0));
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
          .RaiseCollectionChange(binding, new NotifyCollectionChangedEventArgs(Add, AsList("first"), 0));

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
          .RaiseCollectionChange(binding, new NotifyCollectionChangedEventArgs(Reset));

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
          .RaiseCollectionChange(binding, new NotifyCollectionChangedEventArgs(Reset));

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
          .RaiseCollectionChange(binding, new NotifyCollectionChangedEventArgs(Add, AsList("first"), 0));
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
  }

  public class ListBindingTransformationTest
  {
    [Test]
    public void MapBinding()
    {
      var backend = new ObservableCollection<string>()
      {
        "A",
        "B",
        "C"
      };

      var binding = backend.ToBinding().Map(s => s.ToLowerInvariant());
      binding.Should().BeEquivalentTo("a", "b", "c");
    }

    [Test]
    public void BulkChange()
    {
      var backend = new ObservableCollection<string>()
      {
        "D",
        "C",
        "E",
        "B",
        "A"
      };

      var binding = backend.ToBinding().MapAll(l => l.OrderBy(v => v).ToList());
      binding.Should().BeEquivalentTo("A", "B", "C", "D", "E");
    }

    [Test]
    public void Sorting()
    {
      var backend = new ObservableCollection<string>()
      {
        "D",
        "C",
        "E",
        "B",
        "A"
      };

      var binding = backend.ToBinding().OrderByBinding();
      binding.Should().BeEquivalentTo("A", "B", "C", "D", "E");
    }

    [Test]
    public void RangeBinding()
    {
      var backend = new ObservableCollection<string>()
      {
        "D",
        "C",
        "E",
        "B",
        "A"
      };

      var binding = backend.ToBinding().RangeBinding(1, 2);
      binding.Should().BeEquivalentTo("C", "E");
      backend.Clear();
      binding.Should().BeEmpty();
      backend.Add("a");
      backend.Add("b");
      binding.Should().BeEquivalentTo("b");
    }
  }
}