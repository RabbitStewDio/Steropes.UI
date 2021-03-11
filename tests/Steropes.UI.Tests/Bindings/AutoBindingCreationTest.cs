using System.ComponentModel;
using FluentAssertions;
using NUnit.Framework;
using Steropes.UI.Bindings;

namespace Steropes.UI.Test.Bindings
{
  public class AutoBindingCreationTest
  {
    [Test]
    public void Get_Binding_For_Property()
    {
      var subject = new BindingFixtureElement<int, string>(10, "test");
      var binding = subject.BindingFor(p => p.Property);

      using (var monitoredBinding = binding.Monitor<INotifyPropertyChanged>())
      {
        subject.Property = "Changed!";
        monitoredBinding.Should().RaisePropertyChange(binding, nameof(IReadOnlyObservableValue.Value));
        binding.Value.Should().Be("Changed!");
      }
    }

    [Test]
    public void Get_Binding_For_Field()
    {
      var subject = new BindingFixtureElement<int, string>(10, "test");
      var binding = subject.BindingFor(p => p.Field);

      using (var monitoredBinding = binding.Monitor<INotifyPropertyChanged>())
      {
        subject.Field = 11;
        subject.OnPropertyChanged(nameof(subject.Field));
        monitoredBinding.Should().RaisePropertyChange(binding, nameof(IReadOnlyObservableValue.Value));
        binding.Value.Should().Be(11);
      }
    }

    [Test]
    public void Get_Binding_For_Field_With_Cast()
    {
      var subject = new BindingFixtureElement<int, string>(10, "test");
      var binding = subject.BindingFor(p => (double) p.Field);

      using (var monitoredBinding = binding.Monitor<INotifyPropertyChanged>())
      {
        subject.Field = 11;
        subject.OnPropertyChanged(nameof(subject.Field));
        monitoredBinding.Should().RaisePropertyChange(binding, nameof(IReadOnlyObservableValue.Value));
        binding.Value.Should().Be(11.0);
      }
    }

    [Test]
    public void Get_Binding_For_Property_With_Cast()
    {
      var subject = new BindingFixtureElement<int, int>(10, 25);
      var binding = subject.BindingFor(p => (double) p.Field);

      using (var monitoredBinding = binding.Monitor<INotifyPropertyChanged>())
      {
        subject.Field = 11;
        subject.OnPropertyChanged(nameof(subject.Field));
        monitoredBinding.Should().RaisePropertyChange(binding, nameof(IReadOnlyObservableValue.Value));
        binding.Value.Should().Be(11.0);
      }
    }

    [Test]
    public void Get_Binding_For_Property_With_Arithmetics()
    {
      var subject = new BindingFixtureElement<int, int>(10, 25);
      var binding = subject.BindingFor(p => (double) p.Field + 1);

      using (var monitoredBinding = binding.Monitor<INotifyPropertyChanged>())
      {
        subject.Field = 11;
        subject.OnPropertyChanged(nameof(subject.Field));
        monitoredBinding.Should().RaisePropertyChange(binding, nameof(IReadOnlyObservableValue.Value));
        binding.Value.Should().Be(12.0);
      }
    }

    int CallMe(int v)
    {
      return v + 1;
    }

    [Test]
    public void Get_Binding_For_Property_With_FunctionCall()
    {
      var subject = new BindingFixtureElement<int, int>(10, 25);
      var binding = subject.BindingFor(p => (double) CallMe(p.Field));

      using (var monitoredBinding = binding.Monitor<INotifyPropertyChanged>())
      {
        subject.Field = 11;
        subject.OnPropertyChanged(nameof(subject.Field));
        monitoredBinding.Should().RaisePropertyChange(binding, nameof(IReadOnlyObservableValue.Value));
        binding.Value.Should().Be(12.0);
      }
    }

    int CallMe(int v, int x)
    {
      return v + x;
    }

    [Test]
    public void Get_Binding_For_Property_With_MultiParamFunctionCall()
    {
      var subject = new BindingFixtureElement<int, int>(10, 25);
      var binding = subject.BindingFor(p => (double) CallMe(p.Field, p.Property));

      using (var monitoredBinding = binding.Monitor<INotifyPropertyChanged>())
      {
        subject.Field = 11;
        subject.OnPropertyChanged(nameof(subject.Field));
        monitoredBinding.Should().RaisePropertyChange(binding, nameof(IReadOnlyObservableValue.Value));
        binding.Value.Should().Be(36.0);
      }
    }

    [Test]
    public void Get_Binding_For_Property_With_MultiParamFunctionCall_Property_Must_Be_First_Argument()
    {
      // This demonstates that the binding name automation always selects the first member access it 
      // comes across. I generally do not recommend relying on complex magic here.

      var subject = new BindingFixtureElement<int, int>(10, 25);
      var binding = subject.BindingFor(p => (double) CallMe(p.Property, p.Field));

      using (var monitoredBinding = binding.Monitor<INotifyPropertyChanged>())
      {
        subject.Field = 11;
        subject.OnPropertyChanged(nameof(subject.Field));

        // the binding will not react to field changes.
        monitoredBinding.Should().NotRaise(nameof(INotifyPropertyChanged.PropertyChanged));

        // the binding will react to changes to "property" instead, as this is the first 
        // member access the parser encountered.
        subject.OnPropertyChanged(nameof(subject.Property));
        monitoredBinding.Should().RaisePropertyChange(binding, nameof(IReadOnlyObservableValue.Value));
      }
    }
  }
}