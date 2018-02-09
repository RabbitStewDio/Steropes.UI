using System.Collections.ObjectModel;
using FluentAssertions;
using NUnit.Framework;
using Steropes.UI.Bindings;

namespace Steropes.UI.Test.Bindings
{
  public class ListBindingSinkTest
  {
    [Test]
    public void TestOneWayBinding()
    {
      ObservableCollection<string> source = new ObservableCollection<string>();
      ObservableCollection<string> target = new ObservableCollection<string>();

      source.ToBinding().BindTo(target);

      source.Add("A");
      target.Should().BeEquivalentTo("A");
      source.Add("B");
      source.Add("C");
      source.Add("D");
      target.Should().BeEquivalentTo("A", "B", "C", "D");
      source.Move(1,2);
      target.Should().BeEquivalentTo("A", "C", "B", "D");
      source.RemoveAt(2);
      target.Should().BeEquivalentTo("A", "C", "D");
    }

    [Test]
    public void TestTwoWayBinding_Source_To_Target()
    {
      ObservableCollection<string> source = new ObservableCollection<string>();
      ObservableCollection<string> target = new ObservableCollection<string>();

      source.ToBinding().BindTwoWay(target);

      source.Add("A");
      target.Should().BeEquivalentTo("A");
      source.Add("B");
      source.Add("C");
      source.Add("D");
      target.Should().BeEquivalentTo("A", "B", "C", "D");
      source.Move(1,2);
      target.Should().BeEquivalentTo("A", "C", "B", "D");
      source.RemoveAt(2);
      target.Should().BeEquivalentTo("A", "C", "D");
    }
    
    [Test]
    public void TestTwoWayBinding_Target_To_Source()
    {
      ObservableCollection<string> source = new ObservableCollection<string>();
      ObservableCollection<string> target = new ObservableCollection<string>();

      target.ToBinding().BindTwoWay(source);

      source.Add("A");
      target.Should().BeEquivalentTo("A");
      source.Add("B");
      source.Add("C");
      source.Add("D");
      target.Should().BeEquivalentTo("A", "B", "C", "D");
      source.Move(1,2);
      target.Should().BeEquivalentTo("A", "C", "B", "D");
      source.RemoveAt(2);
      target.Should().BeEquivalentTo("A", "C", "D");
    }
    
  }
}