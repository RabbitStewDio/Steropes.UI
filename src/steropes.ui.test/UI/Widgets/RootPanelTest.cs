// MIT License
// Copyright (c) 2011-2016 Elisée Maurer, Sparklin Labs, Creative Patterns
// Copyright (c) 2016 Thomas Morgner, Rabbit-StewDio Ltd.
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
using FluentAssertions;

using Microsoft.Xna.Framework;

using NSubstitute;

using NUnit.Framework;

using Steropes.UI.Components;
using Steropes.UI.Components.Window;
using Steropes.UI.Platform;
using Steropes.UI.Widgets.Container;

namespace Steropes.UI.Test.UI.Widgets
{
  [Category("Window Widget Behaviour")]
  public class RootPanelTest
  {
    [Test]
    public void InsertOrderTest()
    {
      var p = new PopUp<IWidget>(LayoutTestStyle.Create()) { Padding = new Insets(10), Content = LayoutTestWidget.FixedSize(500, 300) };
      p.Anchor = AnchoredRect.CreateTopLeftAnchored(10, 20);

      var screen = Substitute.For<IScreenService>();

      var root = new RootPane(screen, LayoutTestStyle.Create());
      root.Content = LayoutTestWidget.FixedSize(400, 400).WithAnchorRect(AnchoredRect.Full);
      root.AddPopUp(p);

      root.Count.Should().Be(3); // glasspane 
      root[0].Should().BeSameAs(root.Content);
      root[1].Should().BeAssignableTo<GlassPane>();
      root[2].Should().BeSameAs(p);
    }

    [Test]
    public void PopUpLayoutTest()
    {
      var p = new PopUp<IWidget>(LayoutTestStyle.Create()) { Padding = new Insets(10), Content = LayoutTestWidget.FixedSize(500, 300) };
      p.Anchor = AnchoredRect.CreateTopLeftAnchored(10, 20);

      var screen = Substitute.For<IScreenService>();

      var root = new RootPane(screen, LayoutTestStyle.Create());
      root.UIStyle.StyleResolver.AddRoot(root);
      root.AddPopUp(p);

      root.Arrange(new Rectangle(0, 0, 1270, 770));

      p.LayoutRect.Should().Be(new Rectangle(10, 20, 520, 320));
    }

    [Test]
    public void PopUpsHaveHighestPriorityInHitTestZOrder()
    {
      var popUp1 = new PopUp<IWidget>(LayoutTestStyle.Create()) { Padding = new Insets(10), Content = LayoutTestWidget.FixedSize(500, 300) };
      popUp1.Anchor = AnchoredRect.CreateTopLeftAnchored(10, 20);

      var popUp2 = new PopUp<IWidget>(LayoutTestStyle.Create()) { Padding = new Insets(10), Content = LayoutTestWidget.FixedSize(500, 300) };
      popUp2.Anchor = AnchoredRect.CreateTopLeftAnchored(10, 20);

      var screen = Substitute.For<IScreenService>();

      var root = new RootPane(screen, LayoutTestStyle.Create());
      root.UIStyle.StyleResolver.AddRoot(root);
      root.Content = LayoutTestWidget.FixedSize(400, 400).WithAnchorRect(AnchoredRect.Full);
      root.AddPopUp(popUp1);
      root.AddPopUp(popUp2);

      root.Arrange(new Rectangle(0, 0, 1270, 770));

      root.Content.PerformHitTest(new Point(100, 100)).ShouldBeSameObjectReference(root.Content);
      popUp1.PerformHitTest(new Point(100, 100)).ShouldBeSameObjectReference(popUp1.Content);
      popUp2.PerformHitTest(new Point(100, 100)).ShouldBeSameObjectReference(popUp2.Content);
      root.PerformHitTest(new Point(100, 100)).ShouldBeSameObjectReference(popUp2.Content);
    }
  }
}