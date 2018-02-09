using FluentAssertions;
using NUnit.Framework;
using Steropes.UI.Bindings;

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
}