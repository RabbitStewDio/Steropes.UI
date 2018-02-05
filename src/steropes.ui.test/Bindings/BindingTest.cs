using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using FluentAssertions;
using NUnit.Framework;
using Steropes.UI.Annotations;
using Steropes.UI.Bindings;

namespace Steropes.UI.Test.Bindings
{
  public class BindingTest
  {
    class A
    {
      public A(int intProperty)
      {
        IntProperty = intProperty;
      }

      public int IntProperty { get; }

      public override string ToString()
      {
        return $"A({nameof(IntProperty)}: {IntProperty})";
      }
    }

    class B
    {
      public B(A propertyA, string id)
      {
        PropertyA = propertyA ?? throw new ArgumentNullException(nameof(propertyA));
        FieldA = propertyA;
        this.Id = id;
      }

      public A PropertyA { get; }
      public A FieldA;
      public string Id { get; }

      public override string ToString()
      {
        return $"B({nameof(PropertyA)}: {PropertyA}, {nameof(Id)}: {Id})";
      }
    }

    class C: INotifyPropertyChanged
    {
      public B PropField;

      public C(B prop)
      {
        this.PropField = prop ?? throw new ArgumentNullException(nameof(prop));
      }

      public B Prop
      {
        get { return PropField; }
        set
        {
          if (Equals(value, PropField)) return;
          PropField = value;
          OnPropertyChanged();
        }
      }

      public event PropertyChangedEventHandler PropertyChanged;

      [NotifyPropertyChangedInvocator]
      protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
      {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
      }
    }

    [Test]
    public void BindingSetupForProperties()
    {
      var b = new B(new A(5), "first");
      var o = new C(b);
      var singleStep = Binding.Create(() => o.Prop);
      singleStep.Value.Should().Be(b);

      bool changeDetected = false;
      singleStep.PropertyChanged += (source, arg) => changeDetected = true;
      o.Prop = new B(new A(10), "second");
      changeDetected.Should().BeTrue();
    }

    [Test]
    public void BindingSetupForFields()
    {
      var b = new B(new A(5), "first");
      var o = new C(b);
      var singleStep = Binding.Create(() => o.Prop);
      singleStep.Value.Should().Be(b);

      bool changeDetected = false;
      singleStep.PropertyChanged += (source, arg) => changeDetected = true;
      o.Prop = new B(new A(10), "second");
      changeDetected.Should().BeTrue();
    }

    [Test]
    public void ExperimentWithBindings()
    {
      var o = new C(new B(new A(5), "first"));
      Console.WriteLine("---");

      var mutiStep = Binding.Create(() => o.Prop.PropertyA.IntProperty);
      mutiStep.PropertyChanged += (s, e) => Console.WriteLine("MultiStep changed");
      Console.WriteLine("--- " + mutiStep.Value);
      var fieldStep = Binding.Create(() => o.Prop.FieldA.IntProperty);
      fieldStep.PropertyChanged += (s, e) => Console.WriteLine("FieldStep changed");
      
      Console.WriteLine("--- " + fieldStep.Value);
      var constant = Binding.Create(() => 10);
      Console.WriteLine("--- " + constant.Value);

      var b2 = new B(new A(6), "second");
      o.Prop = b2;

      Console.WriteLine("---> " + mutiStep.Value);
      Console.WriteLine("---> " + fieldStep.Value);

    }

    [Test]
    public void ExperimentWithTypesafeBindings()
    {
      var o = new C(new B(new A(5), "first"));
      Console.WriteLine("---");

      IReadOnlyObservableValue<B> binding = o.BindingFor(v => v.Prop);
      binding.PropertyChanged += (s, e) => Console.WriteLine("Binding changed");

      IReadOnlyObservableValue<int> term = binding.Bind(b => b.PropertyA).Bind(a => a.IntProperty);
      term.PropertyChanged += (s, e) => Console.WriteLine("Term changed");

      IReadOnlyObservableValue<int> chain = binding.Chain(v => v.PropertyA.IntProperty);
      chain.PropertyChanged += (s, e) => Console.WriteLine("Chain changed");

      IReadOnlyObservableValue<int> chainFor = o.ChainFor(v => v.Prop.PropertyA.IntProperty);

      var b2 = new B(new A(6), "second");
      o.Prop = b2;

      Console.WriteLine("bind ---> " + binding.Value);
      Console.WriteLine("term ---> " + term.Value);
      Console.WriteLine("chai ---> " + chain.Value);
      Console.WriteLine("ch22 ---> " + chainFor.Value);
    }
  }
}