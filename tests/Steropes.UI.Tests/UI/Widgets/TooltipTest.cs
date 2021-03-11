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
using System;

using FluentAssertions;

using Microsoft.Xna.Framework;

using NSubstitute;

using NUnit.Framework;

using Steropes.UI.Components;
using Steropes.UI.Input.MouseInput;
using Steropes.UI.Platform;
using Steropes.UI.Widgets;

namespace Steropes.UI.Test.UI.Widgets
{
  [Category("Widgets")]
  public class TooltipTest
  {
    [Test]
    public void Arrange()
    {
      var tooltip = CreateTooltip("4");
      tooltip.Anchor = AnchoredRect.CreateTopLeftAnchored(150, 30);

      tooltip.Measure(new Size(float.PositiveInfinity, float.PositiveInfinity));
      tooltip.Arrange(tooltip.ArrangeChild(new Rectangle(10, 20, 200, 20)));
      tooltip.LayoutRect.Should().Be(new Rectangle(160, 50, 31, 35));
    }

    [Test]
    public void Arrange_2()
    {
      var tooltip = CreateTooltip("4");
      tooltip.Anchor = AnchoredRect.CreateTopLeftAnchored(200, 19);

      tooltip.Measure(new Size(float.PositiveInfinity, float.PositiveInfinity));
      tooltip.Arrange(tooltip.ArrangeChild(new Rectangle(10, 20, 200, 20)));
      tooltip.LayoutRect.Should().Be(new Rectangle(210, 39, 31, 35));
    }

    [Test]
    public void ToolTip_Default_Mode()
    {
      var widget = LayoutTestWidget.FixedSize(200, 20);
      widget.UIStyle.StyleResolver.AddRoot(widget);

      widget.Arrange(new Rectangle(10, 20, 200, 20));
      widget.ShowTooltip("4");
      widget.Tooltip.TooltipPosition.Should().Be(TooltipPositionMode.FollowMouse);
    }

    [Test]
    public void InWidget_after_parent_mouse_moved()
    {
      var widget = LayoutTestWidget.FixedSize(200, 20);
      widget.UIStyle.StyleResolver.AddRoot(widget);

      widget.Arrange(new Rectangle(10, 20, 200, 20));
      widget.ShowTooltip("4");
      widget.Tooltip.TooltipPosition = TooltipPositionMode.Fixed;
      widget.Tooltip.Anchor.Top.Should().Be(0);
      widget.Tooltip.Anchor.Left.Should().Be(0);
      widget.Tooltip.Anchor.Right.Should().Be(null);
      widget.Tooltip.Anchor.Bottom.Should().Be(null);
      widget.DispatchMouseMove(MouseButton.None, 190, 35);
      widget.Tooltip.Anchor.Top.Should().Be(0);
      widget.Tooltip.Anchor.Left.Should().Be(0);
      widget.Tooltip.Anchor.Right.Should().Be(null);
      widget.Tooltip.Anchor.Bottom.Should().Be(null);

      widget.Arrange(new Rectangle(10, 20, 200, 20));
      widget.Tooltip.LayoutRect.Should().Be(new Rectangle(10, 20, 51, 55));
    }

    [Test]
    public void InWidget_follow_mouse_after_parent_mouse_moved()
    {
      var widget = LayoutTestWidget.FixedSize(200, 20);
      widget.UIStyle.StyleResolver.AddRoot(widget);

      widget.Arrange(new Rectangle(10, 20, 200, 20));
      widget.ShowTooltip("4");
      widget.Tooltip.TooltipPosition = TooltipPositionMode.FollowMouse;
      widget.Tooltip.Anchor.Top.Should().Be(0);
      widget.Tooltip.Anchor.Left.Should().Be(0);
      widget.Tooltip.Anchor.Right.Should().Be(null);
      widget.Tooltip.Anchor.Bottom.Should().Be(null);
      widget.DispatchMouseMove(MouseButton.None, 190, 35);
      widget.Tooltip.Anchor.Top.Should().Be(15);
      widget.Tooltip.Anchor.Left.Should().Be(180);
      widget.Tooltip.Anchor.Right.Should().Be(null);
      widget.Tooltip.Anchor.Bottom.Should().Be(null);

      widget.Arrange(new Rectangle(10, 20, 200, 20));
      widget.Tooltip.LayoutRect.Should().Be(new Rectangle(190, 35, 51, 55));
    }

    [Test]
    public void Measure()
    {
      var tooltip = CreateTooltip("4");
      tooltip.Measure(new Size(float.PositiveInfinity, float.PositiveInfinity));
      tooltip.DesiredSize.Should().Be(new Size(31, 35), "ToolTip content plus 2x padding");
    }

    [Test]
    public void NodeName()
    {
      var tooltip = CreateTooltip("4");
      tooltip.NodeType.Should().Be("Tooltip");
    }

    [Test]
    public void PaddingsApplied()
    {
      var tooltip = CreateTooltip("4");
      tooltip.Measure(Size.Auto);
      tooltip.Padding.Should().Be(new Insets(10));
    }

    [Test]
    public void TooltipIsCreated()
    {
      var style = LayoutTestStyle.Create();

      var label = new Label(style);
      label.Tooltip.Should().BeNull();

      label.ShowTooltip("test");
      label.Tooltip.Should().NotBeNull();
    }

    [Test]
    public void TooltipReceivesUpdate()
    {
      var style = LayoutTestStyle.Create();
      var tooltip = Substitute.For<IToolTip>();

      var gameTime = new GameTime(TimeSpan.FromDays(1), TimeSpan.Zero);

      var label = new Label(style);
      label.Tooltip = tooltip;

      label.Update(gameTime);
      tooltip.Received().Update(gameTime);
    }

    [Test]
    public void TooltipUpdatesChangeVisibilityWhenDelay()
    {
      var style = LayoutTestStyle.Create();
      var tooltip = new Tooltip<Label>(style);
      tooltip.TooltipDelay = 1;
      tooltip.TooltipDisplayTime = 5;

      var gameTime = new GameTime(TimeSpan.FromDays(1), TimeSpan.Zero);
      tooltip.Update(gameTime);
      tooltip.Visibility.Should().Be(Visibility.Collapsed);

      gameTime = new GameTime(TimeSpan.FromDays(1), TimeSpan.FromSeconds(1));
      tooltip.Update(gameTime);
      tooltip.Visibility.Should().Be(Visibility.Visible);

      gameTime = new GameTime(TimeSpan.FromDays(1), TimeSpan.FromSeconds(4)); // at 5 seconds
      tooltip.Update(gameTime);
      tooltip.Visibility.Should().Be(Visibility.Visible);

      gameTime = new GameTime(TimeSpan.FromDays(1), TimeSpan.FromSeconds(0.99)); // at 5.999
      tooltip.Update(gameTime);
      tooltip.Visibility.Should().Be(Visibility.Visible);

      gameTime = new GameTime(TimeSpan.FromDays(1), TimeSpan.FromSeconds(0.01)); // at 6
      tooltip.Update(gameTime);
      tooltip.Visibility.Should().Be(Visibility.Collapsed);

      gameTime = new GameTime(TimeSpan.FromDays(1), TimeSpan.FromSeconds(0.01)); // at 6.01
      tooltip.Update(gameTime);
      tooltip.Visibility.Should().Be(Visibility.Collapsed);
    }

    [Test]
    public void TooltipUpdatesChangeVisibilityWhenZeroDelay()
    {
      var style = LayoutTestStyle.Create();
      var tooltip = new Tooltip<Label>(style);
      tooltip.TooltipDelay = 0;
      tooltip.TooltipDisplayTime = 0;

      var gameTime = new GameTime(TimeSpan.FromDays(1), TimeSpan.Zero);
      tooltip.Update(gameTime);
      tooltip.Visibility.Should().Be(Visibility.Visible);

      gameTime = new GameTime(TimeSpan.FromDays(1), TimeSpan.FromSeconds(100));
      tooltip.Update(gameTime);
      tooltip.Visibility.Should().Be(Visibility.Visible);
    }

    Tooltip<Label> CreateTooltip(string text = null)
    {
      var style = LayoutTestStyle.Create();
      var tt = new Tooltip<Label>(style) { Content = new Label(style) { Text = text, Padding = new Insets() } };

      style.StyleResolver.AddRoot(tt);
      return tt;
    }
  }
}