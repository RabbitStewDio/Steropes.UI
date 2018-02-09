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
using System.Collections.Generic;

using FluentAssertions;

using Microsoft.Xna.Framework;

using NSubstitute;

using NUnit.Framework;

using Steropes.UI.Components;
using Steropes.UI.Components.Window;
using Steropes.UI.Components.Window.Events;
using Steropes.UI.Input;
using Steropes.UI.Input.MouseInput;

namespace Steropes.UI.Test.UI.Inputs
{
  [Category("Event Handling Behaviour")]
  public class MouseToComponentMapperTest
  {
    MouseToComponentMapper componentMapper;

    EventQueue<MouseEventData> events;

    TimeSpan time;

    IWidget widget;

    public interface INamedWidget : IWidget
    {
      string Name { get; }
    }

    [SetUp]
    public void SetUp()
    {
      events = new EventQueue<MouseEventData>();
      widget = Substitute.For<IWidget>();
      time = TimeSpan.Zero;

      componentMapper = new MouseToComponentMapper(events, new MouseHoverManager(), widget);
    }

    [Test]
    public void TestHoverSubComponent()
    {
      var hitTestResult = Substitute.For<INamedWidget>();
      hitTestResult.Name.Returns("HitResult-Global");

      widget.PerformHitTest(new Point(10, 10)).Returns(hitTestResult);
      widget.PerformHitTest(new Point(30, 30)).Returns(hitTestResult);

      var hitTestResult2 = Substitute.For<INamedWidget>();
      hitTestResult2.Name.Returns("HitResult-Local");
      widget.PerformHitTest(Arg.Is(new Point(20, 20))).Returns(hitTestResult2);

      var x = widget.PerformHitTest(new Point(20, 20));
      x.Should().BeSameAs(hitTestResult2);

      events.PushEvent(CreateMove(10, 10));
      events.PushEvent(CreateMove(20, 20));
      events.PushEvent(CreateMove(30, 30));

      var result = CollectAll();
      result.Should()
        .BeEquivalentTo(
          Tuple.Create(MouseEventType.Entered, new Point(10, 10), "HitResult-Global"),
          Tuple.Create(MouseEventType.Moved, new Point(10, 10), "HitResult-Global"),
          Tuple.Create(MouseEventType.Exited, new Point(20, 20), "HitResult-Global"),
          Tuple.Create(MouseEventType.Entered, new Point(20, 20), "HitResult-Local"),
          Tuple.Create(MouseEventType.Moved, new Point(20, 20), "HitResult-Local"),
          Tuple.Create(MouseEventType.Exited, new Point(30, 30), "HitResult-Local"),
          Tuple.Create(MouseEventType.Entered, new Point(30, 30), "HitResult-Global"),
          Tuple.Create(MouseEventType.Moved, new Point(30, 30), "HitResult-Global"));
    }

    [Test]
    public void TestInitialButtonDown()
    {
      var hitTestResult = Substitute.For<IWidget>();
      widget.PerformHitTest(Arg.Any<Point>()).Returns(hitTestResult);

      events.PushEvent(CreateMove(10, 10, InputFlags.Mouse1));
      events.PushEvent(CreateMove(20, 20, InputFlags.Mouse1));
      events.PushEvent(CreateMove(30, 30, InputFlags.Mouse1));

      componentMapper.PullEventData(out var data);
      data.EventType.Should().Be(MouseEventType.Entered);
      data.Position.Should().Be(new Point(10, 10));

      componentMapper.PullEventData(out data);
      data.EventType.Should().Be(MouseEventType.Dragged);
      data.Position.Should().Be(new Point(10, 10));

      componentMapper.PullEventData(out data);
      data.EventType.Should().Be(MouseEventType.Dragged);
      data.Position.Should().Be(new Point(20, 20));

      componentMapper.PullEventData(out data);
      data.EventType.Should().Be(MouseEventType.Dragged);
      data.Position.Should().Be(new Point(30, 30));
    }

    [Test]
    public void TestMove()
    {
      var hitTestResult = Substitute.For<IWidget>();
      widget.PerformHitTest(Arg.Any<Point>()).Returns(hitTestResult);

      events.PushEvent(CreateMove(10, 10));
      events.PushEvent(CreateMove(20, 20));
      events.PushEvent(CreateMove(30, 30));

      componentMapper.PullEventData(out var data);
      data.EventType.Should().Be(MouseEventType.Entered);
      data.Position.Should().Be(new Point(10, 10));

      componentMapper.PullEventData(out data);
      data.EventType.Should().Be(MouseEventType.Moved);
      data.Position.Should().Be(new Point(10, 10));

      componentMapper.PullEventData(out data);
      data.EventType.Should().Be(MouseEventType.Moved);
      data.Position.Should().Be(new Point(20, 20));

      componentMapper.PullEventData(out data);
      data.EventType.Should().Be(MouseEventType.Moved);
      data.Position.Should().Be(new Point(30, 30));
    }

    IEnumerable<Tuple<MouseEventType, Point, string>> CollectAll()
    {
      var retval = new List<Tuple<MouseEventType, Point, string>>();

      while (componentMapper.PullEventData(out var data))
      {
        var currentWidget = componentMapper.Component;
        var name = currentWidget.As<INamedWidget>()?.Name ?? currentWidget?.ToString();
        retval.Add(Tuple.Create(data.EventType, data.Position, name));
      }
      return retval;
    }

    MouseEventData CreateMove(int x, int y, InputFlags flags = InputFlags.None)
    {
      time += TimeSpan.FromSeconds(1);
      return new MouseEventData(MouseEventType.Moved, flags, time, 0, new Point(x, y));
    }
  }
}