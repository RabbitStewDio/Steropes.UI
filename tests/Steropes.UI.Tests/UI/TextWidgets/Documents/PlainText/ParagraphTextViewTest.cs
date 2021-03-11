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

using NUnit.Framework;

using Steropes.UI.Components;
using Steropes.UI.Platform;
using Steropes.UI.Styles;
using Steropes.UI.Widgets.TextWidgets.Documents;
using Steropes.UI.Widgets.TextWidgets.Documents.PlainText;
using Steropes.UI.Widgets.TextWidgets.Documents.Views;

namespace Steropes.UI.Test.UI.TextWidgets.Documents.PlainText
{
  [Category("Text Behaviour")]
  public class ParagraphTextViewTest
  {
    readonly IStyle textStyle = LayoutTestStyle.CreateTextStyle(LayoutTestStyle.CreateStyleSystem());

    [Test]
    public void Arrange_Should_Consume_All_Width()
    {
      var view = CreateView("Hello World, Here I am.");
      view.Arrange(new Rectangle(10, 20, 400, 100));
      view.LayoutRect.X.Should().Be(10);
      view.LayoutRect.Y.Should().Be(20);
      view.LayoutRect.Width.Should().Be(400);
      view.LayoutRect.Height.Should().Be(15);
    }

    [Test]
    public void MapModelToView_EndOffset()
    {
      var chunk = CreateView();
      chunk.Arrange(new Rectangle(10, 20, 200, 400));

      chunk.ModelToView(chunk.EndOffset, out var rect).Should().BeTrue();
      rect.Should().Be(new Rectangle(142, 95, 0, 15));
    }

    [Test]
    public void MapModelToView_First()
    {
      var chunk = CreateView();
      chunk.Arrange(new Rectangle(10, 20, 200, 400));

      chunk.ModelToView(chunk.Offset, out var rect).Should().BeTrue();
      rect.Should().Be(new Rectangle(10, 20, 11, 15));
    }

    [Test]
    public void MapModelToView_Last()
    {
      var chunk = CreateView();
      chunk.Arrange(new Rectangle(10, 20, 200, 400));

      chunk.ModelToView(chunk.EndOffset - 1, out var rect).Should().BeTrue();
      rect.Should().Be(new Rectangle(131, 95, 11, 15));
    }

    [Test]
    public void MapModelToView_Outside()
    {
      var chunk = CreateView();
      chunk.Arrange(new Rectangle(10, 20, 200, 400));
      Rectangle rect;
      chunk.ModelToView(-1, out rect).Should().BeFalse();
      chunk.ModelToView(chunk.Node.EndOffset + 1, out rect).Should().BeFalse();
    }

    [Test]
    public void MapViewToModel_2_lines_Inside()
    {
      var chunk = CreateView();
      chunk.Measure(Size.Auto);
      chunk.Arrange(new Rectangle(10, 20, 200, 400));
      chunk.ViewToModel(new Point(50, 55), out var offset, out var bias).Should().Be(true);
      offset.Should().Be(37);
      bias.Should().Be(Bias.Forward);
    }

    [Test]
    public void MapViewToModel_2_lines_Inside_BackwardBias()
    {
      var chunk = CreateView();
      chunk.Measure(Size.Auto);
      chunk.Arrange(new Rectangle(10, 20, 200, 400));
      chunk.ViewToModel(new Point(45, 55), out var offset, out var bias).Should().Be(true);
      offset.Should().Be(37, "Expect to break at 3 (available width: 35, per char-width 11, thus 3 chars fit the space fully)");
      bias.Should().Be(Bias.Backward);
    }

    [Test]
    public void MapViewToModel_2_lines_Inside_OnLeft_Edge()
    {
      var chunk = CreateView();
      chunk.Measure(Size.Auto);
      chunk.Arrange(new Rectangle(10, 20, 200, 400));
      chunk.ViewToModel(new Point(10, 55), out var offset, out var bias).Should().Be(true);
      offset.Should().Be(34); // todo
      bias.Should().Be(Bias.Backward);
    }

    [Test]
    public void MapViewToModel_2_lines_Inside_OnRight_Edge()
    {
      var chunk = CreateView();
      chunk.Measure(Size.Auto);
      chunk.Arrange(new Rectangle(10, 20, 200, 400));
      chunk.ViewToModel(new Point(10 + chunk.DesiredSize.WidthInt - 1, 55), out var offset, out var bias).Should().Be(true);
      offset.Should().Be(chunk[2].EndOffset);
      bias.Should().Be(Bias.Backward);
    }

    [Test]
    public void MapViewToModel_Inside()
    {
      var chunk = CreateView();
      chunk.Measure(Size.Auto);
      chunk.Arrange(new Rectangle(10, 20, 200, 400));
      chunk.ViewToModel(new Point(50, 25), out var offset, out var bias).Should().Be(true);
      offset.Should().Be(3, "Expect to break at 3 (available width: 40, per char-width 11, thus 3 chars fit the space fully)");
      bias.Should().Be(Bias.Forward);
    }

    [Test]
    public void MapViewToModel_Inside_BackwardBias()
    {
      var chunk = CreateView();
      chunk.Measure(Size.Auto);
      chunk.Arrange(new Rectangle(10, 20, 200, 400));
      chunk.ViewToModel(new Point(45, 25), out var offset, out var bias).Should().Be(true);
      offset.Should().Be(3, "Expect to break at 3 (available width: 35, per char-width 11, thus 3 chars fit the space fully)");
      bias.Should().Be(Bias.Backward);
    }

    [Test]
    public void MapViewToModel_Inside_OnLeft_Edge()
    {
      var chunk = CreateView();
      chunk.Measure(Size.Auto);
      chunk.Arrange(new Rectangle(10, 20, 200, 400));
      chunk.ViewToModel(new Point(10, 25), out var offset, out var bias).Should().Be(true);
      offset.Should().Be(0, "Expect to break at 3 (available width: 35, per char-width 11, thus 3 chars fit the space fully)");
      bias.Should().Be(Bias.Backward);
    }

    [Test]
    public void MapViewToModel_Inside_OnRight_Edge()
    {
      var chunk = CreateView();
      chunk.Arrange(new Rectangle(10, 20, 200, 400));
      var position = new Point(10 + chunk.DesiredSize.WidthInt - 1, 25);

      chunk.ViewToModel(position, out var offset, out var bias).Should().Be(true);
      chunk[0].LayoutRect.Contains(position).Should().BeTrue();

      // padding indicates the margins of the text.
      chunk[0].LayoutRect.Width.Should().BeLessThan(200 + 11);
      chunk[0].Padding.Should().Be(new Insets(0, 0, 0, 11));
      offset.Should().Be(chunk[0].EndOffset - 1);
      chunk.ModelToView(16, out var rect).Should().BeTrue();
      rect.Contains(position);
      bias.Should().Be(Bias.Forward);
    }

    [Test]
    public void MapViewToModel_Outside_Of_Layout_Area_Left()
    {
      var chunk = CreateView();
      chunk.Measure(Size.Auto);
      chunk.Arrange(new Rectangle(10, 20, 200, 400));

      chunk.ViewToModel(new Point(5, 25), out var offset, out var bias).Should().Be(true);
      offset.Should().Be(chunk[0].Offset);
      bias.Should().Be(Bias.Forward);
    }

    [Test]
    public void MapViewToModel_Outside_Of_Layout_Area_Right()
    {
      var chunk = CreateView("Hello World,\nHere I am.");
      chunk.Measure(Size.Auto);
      chunk.Arrange(new Rectangle(10, 20, 200, 400));
      chunk[0].LayoutRect.Width.Should().Be(chunk[0].DesiredSize.WidthInt);
      chunk.ViewToModel(new Point(chunk.DesiredSize.WidthInt + 10 + 1, 25), out var offset, out var bias).Should().Be(true);
      offset.Should().Be(chunk[0].EndOffset - 1); // trimmed by one, as there are trailing white spaces
      bias.Should().Be(Bias.Backward);
    }

    [Test]
    public void MapViewToModel_Outside_Of_Layout_Area_TopBottom()
    {
      var chunk = CreateView();
      chunk.Measure(Size.Auto);
      chunk.Arrange(new Rectangle(10, 20, 200, 400));
      chunk.ViewToModel(new Point(55, 5), out _, out _).Should().Be(false);
      chunk.ViewToModel(new Point(55, 505), out _, out _).Should().Be(false);
    }

    [Test]
    public void NavigateDown_Enter_Element()
    {
      // This test demonstrates that a single box cannot provide navigation across sibling boxes.
      var chunk = CreateView();
      chunk.Arrange(new Rectangle(10, 20, 200, 400));

      chunk.Navigate(chunk.Offset - 1, Direction.Down, out var target).Should().Be(NavigationResult.Invalid);
      target.Should().Be(-1);
    }

    [Test]
    public void NavigateDown_Inside_Element()
    {
      var chunk = CreateView();
      chunk.Arrange(new Rectangle(10, 20, 200, 400));
      var source = chunk.RecordPosition(chunk.Offset + 1);
      chunk.Navigate(chunk.Offset + 1, Direction.Down, out var target).Should().Be(NavigationResult.Valid);
      target.Should().Be(19);

      chunk.RecordPosition(target).Should().Be(source + new Point(0, 15));
    }

    [Test]
    public void NavigateDown_Leave_Element()
    {
      var chunk = CreateView();
      chunk.Arrange(new Rectangle(10, 20, 200, 400));

      chunk.Navigate(chunk.EndOffset - 1, Direction.Down, out var target).Should().Be(NavigationResult.BoundaryChanged);
      target.Should().Be(chunk.EndOffset);
    }

    [Test]
    public void NavigateLeft_BeyondStart()
    {
      var chunk = CreateView();
      chunk.Arrange(new Rectangle(10, 20, 200, 400));
      chunk.Navigate(-1, Direction.Left, out var target).Should().Be(NavigationResult.BoundaryChanged);
      target.Should().Be(0);
    }

    [Test]
    public void NavigateLeft_Inside()
    {
      var chunk = CreateView();
      chunk.Arrange(new Rectangle(10, 20, 200, 400));

      chunk.Navigate(chunk.EndOffset, Direction.Left, out var target).Should().Be(NavigationResult.Valid);
      target.Should().Be(chunk.EndOffset - 1);
    }

    [Test]
    public void NavigateLeft_Inside_Leave_Element()
    {
      var chunk = CreateView();
      chunk.Arrange(new Rectangle(10, 20, 200, 400));

      chunk.Navigate(chunk.Offset + 1, Direction.Left, out var target).Should().Be(NavigationResult.BoundaryChanged);
      target.Should().Be(chunk.Offset);
    }

    [Test]
    public void NavigateLeft_TowardsEnd()
    {
      var chunk = CreateView();
      chunk.Arrange(new Rectangle(10, 20, 200, 400));

      chunk.Navigate(chunk.EndOffset + 1, Direction.Left, out var target).Should().Be(NavigationResult.Valid);
      target.Should().Be(chunk.EndOffset);
    }

    [Test]
    public void NavigateRight_BeyondEnd()
    {
      var chunk = CreateView();
      chunk.Arrange(new Rectangle(10, 20, 200, 400));
      chunk.Navigate(chunk.EndOffset, Direction.Right, out var target).Should().Be(NavigationResult.BoundaryChanged);
      target.Should().Be(chunk.EndOffset);
    }

    [Test]
    public void NavigateRight_Inside()
    {
      var chunk = CreateView();
      chunk.Arrange(new Rectangle(10, 20, 200, 400));
      chunk.Navigate(chunk.Offset, Direction.Right, out var target).Should().Be(NavigationResult.Valid);
      target.Should().Be(chunk.Offset + 1);
    }

    [Test]
    public void NavigateRight_Inside_Leave_Element()
    {
      var chunk = CreateView();
      chunk.Arrange(new Rectangle(10, 20, 200, 400));
      chunk.Navigate(chunk.EndOffset - 1, Direction.Right, out var target).Should().Be(NavigationResult.BoundaryChanged);
      target.Should().Be(chunk.EndOffset);
    }

    [Test]
    public void NavigateRight_TowardsStart()
    {
      var chunk = CreateView();
      chunk.Arrange(new Rectangle(10, 20, 200, 400));
      chunk.Navigate(-1, Direction.Right, out var target).Should().Be(NavigationResult.Valid);
      target.Should().Be(chunk.Offset);
    }

    [Test]
    public void NavigateUp_Enter_Element()
    {
      // This test demonstrates that a single box cannot provide navigation across sibling boxes.
      var chunk = CreateView();
      chunk.Arrange(new Rectangle(10, 20, 200, 400));

      chunk.Navigate(chunk.EndOffset + 1, Direction.Up, out var target).Should().Be(NavigationResult.Invalid);
      target.Should().Be(-1);
    }

    [Test]
    public void NavigateUp_Inside_Element()
    {
      var chunk = CreateView();
      chunk.Arrange(new Rectangle(10, 20, 200, 400));
      var source = chunk.RecordPosition(chunk.EndOffset - 1);

      chunk.Navigate(chunk.EndOffset - 1, Direction.Up, out var target).Should().Be(NavigationResult.Valid);
      target.Should().Be(75);

      chunk.RecordPosition(target).Should().Be(source + new Point(0, -15));
    }

    [Test]
    public void NavigateUp_Leave_Element()
    {
      var chunk = CreateView();
      chunk.Arrange(new Rectangle(10, 20, 200, 400));

      chunk.Navigate(chunk.Offset + 1, Direction.Up, out var target).Should().Be(NavigationResult.BoundaryChanged);
      target.Should().Be(chunk.Offset);
    }

    [Test]
    public void RebuildWithLongText()
    {
      var view = CreateView("Hello World, Here I am. Can you see me? This is on the next line.");
      view.Measure(new Size(200, float.PositiveInfinity));
      view.DesiredSize.Width.Should().Be(198);
      view.DesiredSize.Height.Should().Be(60);
    }

    [Test]
    public void RebuildWithLongTextExactHit()
    {
      var view = CreateView("Hello World, Here I am. Can you see me? This is on the next line.");
      view.Measure(new Size(198, float.PositiveInfinity));
      view.DesiredSize.Width.Should().Be(198);
      view.DesiredSize.Height.Should().Be(60);
    }

    [Test]
    public void RebuildWithSingleLeaf()
    {
      var view = CreateView("Hello World, Here I am.");
      view.Measure(Size.Auto);
      view.DesiredSize.Width.Should().Be(23 * 11);
      view.DesiredSize.Height.Should().Be(15);
    }

    /// <summary>
    ///   For now, we are not filtering out line-break characters.
    ///   If that ever becomes a problem, we'll adjust the behaviour.
    /// </summary>
    [Test]
    public void RebuildWithSingleLeaftTrailingLineBreak()
    {
      var view = CreateView("Hello World, Here I am.\n");
      view.Measure(Size.Auto);
      view.DesiredSize.Width.Should().Be(24 * 11);
      view.DesiredSize.Height.Should().Be(15);
    }

    [Test]
    public void VisualLayoutTest_LongWordAtMiddle()
    {
      var view = CreateView("Hello Woooooooooooooooooorld");
      view.Measure(new Size(200, float.PositiveInfinity));

      view.Count.Should().Be(2);
      view[0].Should().BeOfType<TextChunkView<PlainTextDocument>>().Which.Text.Should().Be("Hello ");
      view[1].Should().BeOfType<TextChunkView<PlainTextDocument>>().Which.Text.Should().Be("Woooooooooooooooooorld");
    }

    [Test]
    public void VisualLayoutTest_LongWordAtStart()
    {
      var view = CreateView("Helloooooooooooo00ooo World");
      view.Measure(new Size(200, float.PositiveInfinity));

      view.Count.Should().Be(2);
      view[0].Should().BeOfType<TextChunkView<PlainTextDocument>>().Which.Text.Should().Be("Helloooooooooooo00ooo ");
      view[1].Should().BeOfType<TextChunkView<PlainTextDocument>>().Which.Text.Should().Be("World");
    }

    [Test]
    public void VisualLayoutTest_SingleElement()
    {
      var view = CreateView();
      view.Arrange(new Rectangle(10, 20, 200, 500));

      view.Count.Should().Be(6);
      view[0].Should().BeOfType<TextChunkView<PlainTextDocument>>().Which.Text.Should().Be("Hello World, Here ");
      view[1].Should().BeOfType<TextChunkView<PlainTextDocument>>().Which.Text.Should().Be("I am. Long text ");
      view[2].Should().BeOfType<TextChunkView<PlainTextDocument>>().Which.Text.Should().Be("ahead here. A ");
      view[3].Should().BeOfType<TextChunkView<PlainTextDocument>>().Which.Text.Should().Be("long word, that ");
      view[4].Should().BeOfType<TextChunkView<PlainTextDocument>>().Which.Text.Should().Be("is impossible to ");
      view[5].Should().BeOfType<TextChunkView<PlainTextDocument>>().Which.Text.Should().Be("break apart.");
    }

    [Test]
    public void VisualLayoutTest_SingleElementLineBreaks_YPositions()
    {
      var view = CreateView();
      view.Arrange(new Rectangle(10, 20, 200, 500));

      view.Count.Should().Be(6);
      view[0].Should().BeOfType<TextChunkView<PlainTextDocument>>().Which.LayoutRect.Y.Should().Be(20);
      view[1].Should().BeOfType<TextChunkView<PlainTextDocument>>().Which.LayoutRect.Y.Should().Be(35);
      view[2].Should().BeOfType<TextChunkView<PlainTextDocument>>().Which.LayoutRect.Y.Should().Be(50);
      view[3].Should().BeOfType<TextChunkView<PlainTextDocument>>().Which.LayoutRect.Y.Should().Be(65);
      view[4].Should().BeOfType<TextChunkView<PlainTextDocument>>().Which.LayoutRect.Y.Should().Be(80);
      view[5].Should().BeOfType<TextChunkView<PlainTextDocument>>().Which.LayoutRect.Y.Should().Be(95);
    }

    ParagraphTextView<PlainTextDocument> CreateView(string text = null)
    {
      var doc = new PlainTextDocument();
      doc.InsertAt(0, text ?? "Hello World, Here I am. Long text ahead here. A long word, that is impossible to break apart.");
      var node = doc.Root[0];

      var view = new ParagraphTextView<PlainTextDocument>(node, textStyle, new PlainTextViewFactory());
      return view;
    }
  }
}