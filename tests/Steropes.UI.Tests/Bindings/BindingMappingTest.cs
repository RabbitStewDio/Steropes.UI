using System;
using System.ComponentModel;
using FluentAssertions;
using NUnit.Framework;
using Steropes.UI.Bindings;

namespace Steropes.UI.Test.Bindings
{
  public class BindingMappingTest
  {
    [Test]
    public void Binding_Mapping_Works()
    {
      ObservableValue<string> subject = new ObservableValue<string>("One");
      
      var binding = subject.Map(s => s + s);

      using (var monitoredBinding = binding.Monitor<INotifyPropertyChanged>())
      {
        binding.Value.Should().Be("OneOne");

        subject.Value = "A";

        monitoredBinding.Should().RaisePropertyChange(binding, nameof(IReadOnlyObservableValue.Value));
        binding.Value.Should().Be("AA");
      }
    }

    [Test]
    public void Binding_ThatMaps_To_Same_Value_Should_Not_Raise_Duplicate_Events()
    {
      ObservableValue<double> subject = new ObservableValue<double>(1000);
      
      var binding = subject.Map(s => Math.Round(s / 500));

      using (var monitoredBinding = binding.Monitor<INotifyPropertyChanged>())
      {
        binding.Value.Should().Be(2);
        subject.Value = 1100;

        monitoredBinding.Should().NotRaise(nameof(INotifyPropertyChanged.PropertyChanged));
        binding.Value.Should().Be(2);
      }
    }

    [Test]
    public void Filtering_Bindings()
    {
      ObservableValue<double?> subject = new ObservableValue<double?>(1000);

      var binding = subject.Filter(v => v != null && v > 100.0);

      using (var monitoredBinding = binding.Monitor<INotifyPropertyChanged>())
      {
        binding.Value.Should().Be(1000);
        subject.Value = 10;

        monitoredBinding.Should().RaisePropertyChange(binding, nameof(IReadOnlyObservableValue.Value));
        binding.Value.Should().BeNull();
      }
    }

    [Test]
    public void OrElseBinding_Reacts_To_Null()
    {
      ObservableValue<string> subject = new ObservableValue<string>("A");

      var binding = subject.OrElse("Fallback");

      using (var monitoredBinding = binding.Monitor<INotifyPropertyChanged>())
      {
        binding.Value.Should().Be("A");
        subject.Value = null;

        monitoredBinding.Should().RaisePropertyChange(binding, nameof(IReadOnlyObservableValue.Value));
        binding.Value.Should().Be("Fallback");
      }
  
      // In case the actual value is the same as the defined fallback value,
      // no event should be triggered.
      using (var monitoredBinding = binding.Monitor<INotifyPropertyChanged>())
      {
        subject.Value = "Fallback";
        monitoredBinding.Should().NotRaise(nameof(INotifyPropertyChanged.PropertyChanged));
        binding.Value.Should().Be("Fallback");
      }
    }

    [Test, MaxTime(500)]
    public void BindingLoopTest()
    {
      ObservableValue<string> subject = new ObservableValue<string>("A");
      ObservableValue<string> target = new ObservableValue<string>("A");

      subject.Map(v => v + v).BindTo(v => target.Value = v);
      target.BindTo(v => subject.Value = v);

      subject.Value = "B";

      subject.Value.Should().Be("BB");
      target.Value.Should().Be("BB");
    }
  }
}