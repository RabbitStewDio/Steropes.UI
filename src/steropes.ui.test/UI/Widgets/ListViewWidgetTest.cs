using System;
using FluentAssertions;
using NUnit.Framework;
using Steropes.UI.Components;
using Steropes.UI.Widgets;

namespace Steropes.UI.Test.UI.Widgets
{
  public class ListViewWidgetTest
  {
    [Test]
    public void TestLayout()
    {
      var style = LayoutTestStyle.Create();
      var l = new ListView<string>(style);
      style.StyleResolver.AddRoot(l);
      style.StyleResolver.Revalidate();

      l.DataItems.Add("One");
      l.DataItems.Add("Two");
      l.DataItems.Add("Three");
      l.DataItems.Add("Four");
      l.DataItems.Add("Five");
      l.DataItems.Add("Six");

      l.Measure(Size.Auto);
      l.DesiredSize.Should().Be(new Size(105, 20 + 6 * 35));
    }

    [Test]
    public void EstablishRendererSize()
    {
      var style = LayoutTestStyle.Create();
      var renderer = ListView.DefaultCreateRenderer(style, "Test");
      style.StyleResolver.AddRoot(renderer);

      renderer.Measure(Size.Auto);
      renderer.DesiredSize.Should().Be(new Size(64, 35));
    }

    [Test]
    public void TestEmptyLayout()
    {
      var style = LayoutTestStyle.Create();
      var l = new ListView<string>(style);
      style.StyleResolver.AddRoot(l);

      l.Measure(Size.Auto);
      l.DesiredSize.Should().Be(new Size(30, 30), 
                                "ScrollPanel is defined as Padding: 10 with scroll-thumb padding:5 on each side.");
    }
  }
}