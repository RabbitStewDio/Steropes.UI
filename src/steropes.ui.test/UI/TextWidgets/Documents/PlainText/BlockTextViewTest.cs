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
using Steropes.UI.Styles;
using Steropes.UI.Widgets.TextWidgets.Documents;
using Steropes.UI.Widgets.TextWidgets.Documents.PlainText;
using Steropes.UI.Widgets.TextWidgets.Documents.Views;

namespace Steropes.UI.Test.UI.TextWidgets.Documents.PlainText
{
  [Category("Text Behaviour")]
  public class BlockTextViewTest
  {
    readonly IStyleSystem styleSystem = LayoutTestStyle.CreateStyleSystem();

    [Test]
    public void Arrange()
    {
      var chunk = CreateView();
      chunk.Arrange(new Rectangle(10, 20, 200, 400));
      chunk.LayoutRect.Should().Be(new Rectangle(10, 20, 200, 105));
    }

    [Test]
    public void ArrangeSingleChar()
    {
      var chunk = CreateView("x");
      chunk.Arrange(new Rectangle(10, 20, 200, 400));
      chunk.LayoutRect.Should().Be(new Rectangle(10, 20, 200, 15));
    }

    [Test]
    public void MapModelToView_EndOffset()
    {
      var chunk = CreateView();
      chunk.Arrange(new Rectangle(10, 20, 200, 400));

      Rectangle rect;
      chunk.ModelToView(chunk.EndOffset, out rect).Should().BeTrue();
      rect.Should().Be(new Rectangle(142, 110, 0, 15));
    }

    [Test]
    public void MapModelToView_First()
    {
      var chunk = CreateView();
      chunk.Arrange(new Rectangle(10, 20, 200, 400));

      Rectangle rect;
      chunk.ModelToView(chunk.Offset, out rect).Should().BeTrue();
      rect.Should().Be(new Rectangle(10, 20, 11, 15));
    }

    [Test]
    public void MapModelToView_Last()
    {
      var chunk = CreateView();
      chunk.Arrange(new Rectangle(10, 20, 200, 400));

      Rectangle rect;
      chunk.ModelToView(chunk.EndOffset - 1, out rect).Should().BeTrue();
      rect.Should().Be(new Rectangle(131, 110, 11, 15));
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
      int offset;
      Bias bias;
      chunk.Arrange(new Rectangle(10, 20, 200, 400));
      chunk.ViewToModel(new Point(50, 55), out offset, out bias).Should().Be(true);
      offset.Should().Be(28);
      bias.Should().Be(Bias.Forward);
    }

    [Test]
    public void MapViewToModel_2_lines_Inside_BackwardBias()
    {
      var chunk = CreateView();
      chunk.Measure(Size.Auto);
      int offset;
      Bias bias;
      chunk.Arrange(new Rectangle(10, 20, 200, 400));
      chunk.ViewToModel(new Point(45, 55), out offset, out bias).Should().Be(true);
      offset.Should().Be(chunk[1][0].Offset + 3); // 3rd char on line 2
      bias.Should().Be(Bias.Backward);
    }

    [Test]
    public void MapViewToModel_2_lines_Inside_OnLeft_Edge()
    {
      var chunk = CreateView();
      chunk.Measure(Size.Auto);
      int offset;
      Bias bias;
      chunk.Arrange(new Rectangle(10, 20, 200, 400));
      chunk.ViewToModel(new Point(10, 55), out offset, out bias).Should().Be(true);
      offset.Should().Be(chunk[1][0].Offset); // todo
      bias.Should().Be(Bias.Backward);
    }

    [Test]
    public void MapViewToModel_2_lines_Inside_OnRight_Edge()
    {
      var chunk = CreateView();
      chunk.Measure(Size.Auto);
      int offset;
      Bias bias;
      chunk.Arrange(new Rectangle(10, 20, 200, 400));
      chunk.ViewToModel(new Point(10 + chunk.DesiredSize.WidthInt - 1, 55), out offset, out bias).Should().Be(true);
      offset.Should().Be(chunk[1][0].EndOffset);
      bias.Should().Be(Bias.Backward);
    }

    [Test]
    public void MapViewToModel_Inside()
    {
      var chunk = CreateView();
      chunk.Measure(Size.Auto);
      int offset;
      Bias bias;
      chunk.Arrange(new Rectangle(10, 20, 200, 400));
      chunk.ViewToModel(new Point(50, 25), out offset, out bias).Should().Be(true);
      offset.Should().Be(3, "Expect to break at 3 (available width: 40, per char-width 11, thus 3 chars fit the space fully)");
      bias.Should().Be(Bias.Forward);
    }

    [Test]
    public void MapViewToModel_Inside_BackwardBias()
    {
      var chunk = CreateView();
      chunk.Measure(Size.Auto);
      int offset;
      Bias bias;
      chunk.Arrange(new Rectangle(10, 20, 200, 400));
      chunk.ViewToModel(new Point(45, 25), out offset, out bias).Should().Be(true);
      offset.Should().Be(3, "Expect to break at 3 (available width: 35, per char-width 11, thus 3 chars fit the space fully)");
      bias.Should().Be(Bias.Backward);
    }

    [Test]
    public void MapViewToModel_Inside_OnLeft_Edge()
    {
      var chunk = CreateView();
      chunk.Measure(Size.Auto);
      int offset;
      Bias bias;
      chunk.Arrange(new Rectangle(10, 20, 200, 400));
      chunk.ViewToModel(new Point(10, 25), out offset, out bias).Should().Be(true);
      offset.Should().Be(0, "Expect to break at 3 (available width: 35, per char-width 11, thus 3 chars fit the space fully)");
      bias.Should().Be(Bias.Backward);
    }

    [Test]
    public void MapViewToModel_Inside_OnRight_Edge()
    {
      var chunk = CreateView();
      chunk.Arrange(new Rectangle(10, 20, 200, 400));
      var position = new Point(10 + chunk.DesiredSize.WidthInt - 1, 25);

      int offset;
      Bias bias;

      chunk.ViewToModel(position, out offset, out bias).Should().Be(true);
      chunk[0].LayoutRect.Contains(position).Should().BeTrue();

      // padding indicates the margins of the text.
      chunk[0].LayoutRect.Width.Should().BeLessThan(200 + 11);

      offset.Should().Be(chunk[0][0].EndOffset - 1);
      Rectangle rect;
      chunk.ModelToView(16, out rect).Should().BeTrue();
      rect.Contains(position);
      bias.Should().Be(Bias.Forward);
    }

    [Test]
    public void MapViewToModel_Outside_Of_Layout_Area_Left()
    {
      var chunk = CreateView();
      chunk.Measure(Size.Auto);
      int offset;
      Bias bias;
      chunk.Arrange(new Rectangle(10, 20, 200, 400));
      chunk.ViewToModel(new Point(5, 25), out offset, out bias).Should().Be(false);
    }

    [Test]
    public void MapViewToModel_Outside_Of_Layout_Area_Right()
    {
      var chunk = CreateView("Hello World,\nHere I am.");
      chunk.Arrange(new Rectangle(10, 20, 200, 400));
      chunk[0].LayoutRect.Width.Should().Be(200);

      int offset;
      Bias bias;

      chunk.ViewToModel(new Point(chunk.DesiredSize.WidthInt + 10 + 1, 25), out offset, out bias).Should().Be(true);
      offset.Should().Be(chunk[0].EndOffset - 1); // trimmed by one, as there are trailing white spaces
      bias.Should().Be(Bias.Backward);
    }

    [Test]
    public void MapViewToModel_Outside_Of_Layout_Area_TopBottom()
    {
      var chunk = CreateView();
      chunk.Measure(Size.Auto);
      int offset;
      Bias bias;
      chunk.Arrange(new Rectangle(10, 20, 200, 400));
      chunk.ViewToModel(new Point(55, 5), out offset, out bias).Should().Be(false);
      chunk.ViewToModel(new Point(55, 505), out offset, out bias).Should().Be(false);
    }

    [Test]
    public void NavigateDown_Cross_Lines()
    {
      var chunk = CreateView();
      chunk.Arrange(new Rectangle(10, 20, 400, 400));
      var position = chunk.RecordPosition(44);

      // offset is near the end of line 2, in word "here".
      int target;
      chunk.Navigate(44, Direction.Down, out target).Should().Be(NavigationResult.Valid);
      target.Should().Be(67);

      chunk.RecordPosition(target).Should().Be(position + new Point(0, +15));
    }

    [Test]
    public void NavigateDown_Enter_Element()
    {
      // This test demonstrates that a single box cannot provide navigation across sibling boxes.
      var chunk = CreateView();
      chunk.Arrange(new Rectangle(10, 20, 200, 400));

      int target;
      chunk.Navigate(chunk.Offset - 1, Direction.Down, out target).Should().Be(NavigationResult.Invalid);
      target.Should().Be(-1);
    }

    [Test]
    public void NavigateDown_Inside_Element()
    {
      var chunk = CreateView();
      chunk.Arrange(new Rectangle(10, 20, 200, 400));
      var source = chunk.RecordPosition(chunk.Offset + 1);

      int target;
      chunk.Navigate(chunk.Offset + 1, Direction.Down, out target).Should().Be(NavigationResult.Valid);
      target.Should().Be(19);

      chunk.RecordPosition(target).Should().Be(source + new Point(0, 15));
    }

    [Test]
    public void NavigateDown_Leave_Element()
    {
      var chunk = CreateView();
      chunk.Arrange(new Rectangle(10, 20, 200, 400));

      int target;
      chunk.Navigate(chunk.EndOffset - 1, Direction.Down, out target).Should().Be(NavigationResult.BoundaryChanged);
      target.Should().Be(chunk.EndOffset);
    }

    [Test]
    public void NavigateLeft_BeyondStart()
    {
      var chunk = CreateView();
      chunk.Arrange(new Rectangle(10, 20, 200, 400));
      int target;
      chunk.Navigate(-1, Direction.Left, out target).Should().Be(NavigationResult.BoundaryChanged);
      target.Should().Be(0);
    }

    [Test]
    public void NavigateLeft_Inside()
    {
      var chunk = CreateView();
      chunk.Arrange(new Rectangle(10, 20, 200, 400));

      int target;
      chunk.Navigate(chunk.EndOffset, Direction.Left, out target).Should().Be(NavigationResult.Valid);
      target.Should().Be(chunk.EndOffset - 1);
    }

    [Test]
    public void NavigateLeft_Inside_Leave_Element()
    {
      var chunk = CreateView();
      chunk.Arrange(new Rectangle(10, 20, 200, 400));

      int target;
      chunk.Navigate(chunk.Offset + 1, Direction.Left, out target).Should().Be(NavigationResult.Valid);
      target.Should().Be(chunk.Offset);
    }

    [Test]
    public void NavigateLeft_TowardsEnd()
    {
      var chunk = CreateView();
      chunk.Arrange(new Rectangle(10, 20, 200, 400));

      int target;
      chunk.Navigate(chunk.EndOffset + 1, Direction.Left, out target).Should().Be(NavigationResult.Valid);
      target.Should().Be(chunk.EndOffset);
    }

    [Test]
    public void NavigateRight_BeyondEnd()
    {
      var chunk = CreateView();
      chunk.Arrange(new Rectangle(10, 20, 200, 400));
      int target;
      chunk.Navigate(chunk.EndOffset, Direction.Right, out target).Should().Be(NavigationResult.BoundaryChanged);
      target.Should().Be(chunk.EndOffset);
    }

    [Test]
    public void NavigateRight_Inside()
    {
      var chunk = CreateView();
      chunk.Arrange(new Rectangle(10, 20, 200, 400));
      int target;
      chunk.Navigate(chunk.Offset, Direction.Right, out target).Should().Be(NavigationResult.Valid);
      target.Should().Be(chunk.Offset + 1);
    }

    [Test]
    public void NavigateRight_Inside_Leave_Element()
    {
      var chunk = CreateView();
      chunk.Arrange(new Rectangle(10, 20, 200, 400));
      int target;
      chunk.Navigate(chunk.EndOffset - 1, Direction.Right, out target).Should().Be(NavigationResult.Valid);
      target.Should().Be(chunk.EndOffset);
    }

    [Test]
    public void NavigateRight_TowardsStart()
    {
      var chunk = CreateView();
      chunk.Arrange(new Rectangle(10, 20, 200, 400));
      int target;
      chunk.Navigate(-1, Direction.Right, out target).Should().Be(NavigationResult.Valid);
      target.Should().Be(chunk.Offset);
    }

    [Test]
    public void NavigateUp_Cross_Lines()
    {
      var chunk = CreateView();
      chunk.Arrange(new Rectangle(10, 20, 400, 400));
      var position = chunk.RecordPosition(44);

      // offset is near the end of line 2, in word "here".
      int target;
      chunk.Navigate(44, Direction.Up, out target).Should().Be(NavigationResult.Valid);
      target.Should().Be(19);

      chunk.RecordPosition(target).Should().Be(position + new Point(0, -15));
    }

    [Test]
    public void NavigateUp_Enter_Element()
    {
      // This test demonstrates that a single box cannot provide navigation across sibling boxes.
      var chunk = CreateView();
      chunk.Arrange(new Rectangle(10, 20, 200, 400));

      int target;
      chunk.Navigate(chunk.EndOffset + 1, Direction.Up, out target).Should().Be(NavigationResult.Invalid);
      target.Should().Be(-1);
    }

    [Test]
    public void NavigateUp_Inside_Element()
    {
      var chunk = CreateView();
      chunk.Arrange(new Rectangle(10, 20, 200, 400));
      var position = chunk.RecordPosition(chunk.EndOffset - 1);
      int target;
      chunk.Navigate(chunk.EndOffset - 1, Direction.Up, out target).Should().Be(NavigationResult.Valid);
      target.Should().Be(77);

      chunk.RecordPosition(target).Should().Be(position + new Point(0, -15));
    }

    [Test]
    public void NavigateUp_Leave_Element()
    {
      var chunk = CreateView();
      chunk.Arrange(new Rectangle(10, 20, 200, 400));

      int target;
      chunk.Navigate(chunk.Offset + 1, Direction.Up, out target).Should().Be(NavigationResult.BoundaryChanged);
      target.Should().Be(chunk.Offset);
    }

    BlockTextView<PlainTextDocument> CreateView(string text = null)
    {
      var doc = new PlainTextDocument();
      doc.InsertAt(0, text ?? "Hello World, Here I am. \nLong text ahead here. \nA long word, that is impossible to break apart.");

      var textStyle = LayoutTestStyle.CreateTextStyle(styleSystem);
      var factory = new PlainTextViewFactory();
      var view = new BlockTextView<PlainTextDocument>(doc.Root, textStyle);
      for (var i = 0; i < doc.Root.Count; i += 1)
      {
        view.Add(new ParagraphTextView<PlainTextDocument>(doc.Root[i], textStyle, factory));
      }
      return view;
    }
  }
}