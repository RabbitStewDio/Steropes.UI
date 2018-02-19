using System.Collections.ObjectModel;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Steropes.UI.Bindings;

namespace Steropes.UI.Test.Bindings
{
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