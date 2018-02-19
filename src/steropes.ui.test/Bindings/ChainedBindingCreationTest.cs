using System.ComponentModel;
using FluentAssertions;
using NUnit.Framework;
using Steropes.UI.Bindings;

namespace Steropes.UI.Test.Bindings
{
  public class ChainedBindingCreationTest
  {
    // deeply nested test subject
    BindingFixtureElement<int,
      BindingFixtureElement<
        BindingFixtureElement<string, byte>,
        BindingFixtureElement<double, float>
      >
    > element;

    [SetUp]
    public void SetUp()
    {
      element = new BindingFixtureElement<int,
        BindingFixtureElement<
          BindingFixtureElement<string, byte>,
          BindingFixtureElement<double, float>>>
      (10,
       new BindingFixtureElement<BindingFixtureElement<string, byte>, BindingFixtureElement<double, float>>(
         new BindingFixtureElement<string, byte>("Test", 128),
         new BindingFixtureElement<double, float>(12.0, 5.5f)));
    }

    [Test]
    public void Get_Binding_From_PropertyChain()
    {
      var binding = element.ChainFor(e => e.Property.Field.Field);

      using (var monitoredBinding = binding.Monitor<INotifyPropertyChanged>())
      {
        // test changes to the field itself.
        element.Property.Field.Field = "Change!";
        element.Property.Field.OnPropertyChanged(nameof(element.Field));

        monitoredBinding.Should().RaisePropertyChange(binding, nameof(IReadOnlyObservableValue.Value));
        binding.Value.Should().Be("Change!");

        // test changes to a field in the chain of objects.
        element.Property.Field = new BindingFixtureElement<string, byte>("More Change", 1);
        element.Property.OnPropertyChanged(nameof(element.Field));

        monitoredBinding.Should().RaisePropertyChange(binding, nameof(IReadOnlyObservableValue.Value));
        binding.Value.Should().Be("More Change");

        // test changes to a property in the chain of objects.
        element.Property = new BindingFixtureElement<BindingFixtureElement<string, byte>, BindingFixtureElement<double, float>>(
          new BindingFixtureElement<string, byte>("Even More Change!", 1), 
          new BindingFixtureElement<double, float>());

        monitoredBinding.Should().RaisePropertyChange(binding, nameof(IReadOnlyObservableValue.Value));
        binding.Value.Should().Be("Even More Change!");
      }
    }
  }
}