using System.ComponentModel;
using FluentAssertions;
using NUnit.Framework;
using Steropes.UI.Bindings;

namespace Steropes.UI.Test.Bindings
{
  /// <summary>
  ///  Test all method that acquire bindings from ordinary objects.
  /// </summary>
  public class ExplicitBindingCreationTest
  {
    [Test]
    public void Get_Binding_For_Property()
    {
      var subject = new BindingFixtureElement<int, string>(10, "test");
      var binding = subject.BindingFor(nameof(subject.Property), p => p.Property);

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
      var binding = subject.BindingFor(nameof(subject.Field), p => p.Field);

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
      var binding = subject.BindingFor(nameof(subject.Field), p => (double) p.Field);

      using (var monitoredBinding = binding.Monitor<INotifyPropertyChanged>())
      {
        subject.Field = 11;
        subject.OnPropertyChanged(nameof(subject.Field));
        monitoredBinding.Should().RaisePropertyChange(binding, nameof(IReadOnlyObservableValue.Value));
        binding.Value.Should().Be(11.0);
      }
    }
  }
}