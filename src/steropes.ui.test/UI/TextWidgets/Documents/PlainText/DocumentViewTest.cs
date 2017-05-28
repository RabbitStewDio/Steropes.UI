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
using Steropes.UI.Widgets.TextWidgets.Documents;
using Steropes.UI.Widgets.TextWidgets.Documents.PlainText;
using Steropes.UI.Widgets.TextWidgets.Documents.Views;

namespace Steropes.UI.Test.UI.TextWidgets.Documents.PlainText
{
  [Category("Text Behaviour")]
  public class DocumentViewTest
  {
    [Test]
    public void Input_NewLine_BreakAtEnd()
    {
      var parent = Substitute.For<IWidget>();

      var chunk = CreateView("Hello World");
      chunk.AddNotify(parent);
      chunk.Measure(Size.Auto);
      chunk.Arrange(new Rectangle(10, 20, 200, 400));
      chunk.Document.InsertAt(11, '\n');

      chunk.ExposedRootView.Count.Should().Be(2);
      chunk.ExposedRootView[0].Offset.Should().Be(0);
      chunk.ExposedRootView[0].EndOffset.Should().Be(12);
      chunk.ExposedRootView[1].Offset.Should().Be(12);
      chunk.ExposedRootView[1].EndOffset.Should().Be(12);
    }

    [Test]
    public void Input_NewLine_Breaks_Lines()
    {
      var parent = Substitute.For<IWidget>();

      var chunk = CreateView();
      chunk.AddNotify(parent);
      chunk.Measure(Size.Auto);
      chunk.Arrange(new Rectangle(10, 20, 200, 400));
      chunk.Document.InsertAt(5, '\n');

      parent.Received().InvalidateLayout();
    }

    [Test]
    public void Input_NewLine_Triggers_LayoutInvalid()
    {
      var parent = Substitute.For<IWidget>();

      var chunk = CreateView();
      chunk.AddNotify(parent);
      chunk.Measure(Size.Auto);
      chunk.Arrange(new Rectangle(10, 20, 200, 400));
      chunk.Document.InsertAt(5, '\n');

      parent.Received().InvalidateLayout();
    }

    [Test]
    public void LayoutTest_MultipleParagraphs()
    {
      var documentView = NodeTestExtensions.SetUp();

      documentView.Arrange(new Rectangle(10, 20, 200, 500));
      documentView.ExposedRootView.Count.Should().Be(2);
      documentView.ExposedRootView[0].Count.Should().Be(5);
      documentView.ExposedRootView[1].Count.Should().Be(5);
      documentView.ExposedRootView[0].Node.ExtractNodeText().Should().Be("Hello World, Here I am. Long text ahead here. This is the first paragraph.\n");
      documentView.ExposedRootView[1].Node.ExtractNodeText().Should().Be("After a line break, we should see a second paragraph in the document.");
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
      offset.Should().Be(chunk.ExposedRootView[1][0].Offset + 3); // 3rd char on line 2
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
      offset.Should().Be(chunk.ExposedRootView[1][0].Offset); // todo
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
      offset.Should().Be(chunk.ExposedRootView[1][0].EndOffset);
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
      chunk.ExposedRootView[0].LayoutRect.Contains(position).Should().BeTrue();

      // padding indicates the margins of the text.
      chunk.ExposedRootView[0].LayoutRect.Width.Should().BeLessThan(200 + 11);

      offset.Should().Be(chunk.ExposedRootView[0][0].EndOffset - 1);
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
      chunk.ExposedRootView[0].LayoutRect.Width.Should().Be(200);

      int offset;
      Bias bias;

      chunk.ViewToModel(new Point(chunk.DesiredSize.WidthInt + 10 + 1, 25), out offset, out bias).Should().Be(true);
      offset.Should().Be(chunk.ExposedRootView[0].EndOffset - 1); // trimmed by one, as there are trailing white spaces
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
    public void Navigate_Down()
    {
      var chunk = CreateView("Hello World!\n\nSome more text");
      chunk.Measure(Size.Auto);
      chunk.Arrange(new Rectangle(10, 20, 200, 400));

      chunk.Navigate(0, Direction.Down).Should().Be(13);
      chunk.Navigate(13, Direction.Up).Should().Be(0);

      chunk.Navigate(13, Direction.Down).Should().Be(14);
      chunk.Navigate(14, Direction.Up).Should().Be(13);
    }

    [Test]
    public void Navigate_DownAtBreak()
    {
      var chunk = CreateView("Hello World!\n\nSome more text");
      chunk.Measure(Size.Auto);
      chunk.Arrange(new Rectangle(10, 20, 200, 400));

      int result;
      chunk.ExposedRootView.Navigate(0, Direction.Down, out result).Should().Be(NavigationResult.Valid);
      result.Should().Be(13);
    }

    [Test]
    public void Navigate_LeftAtBreak()
    {
      var chunk = CreateView("Hello World!\n\nSome more text");
      chunk.Measure(Size.Auto);
      chunk.Arrange(new Rectangle(10, 20, 200, 400));

      int result;
      chunk.ExposedRootView.Navigate(14, Direction.Left, out result).Should().Be(NavigationResult.Valid);
      result.Should().Be(13);
    }

    [Test]
    public void Navigate_RightAtBreak()
    {
      var chunk = CreateView("Hello World!\n\nSome more text");
      chunk.Measure(Size.Auto);
      chunk.Arrange(new Rectangle(10, 20, 200, 400));

      int result;
      chunk.ExposedRootView.Navigate(12, Direction.Right, out result).Should().Be(NavigationResult.Valid);
      result.Should().Be(13);
    }

    [Test]
    public void NavigateDown_Cross_Lines()
    {
      var chunk = CreateView();
      chunk.Arrange(new Rectangle(10, 20, 400, 400));
      var position = chunk.ExposedRootView.RecordPosition(44);

      // offset is near the end of line 2, in word "here".
      var target = chunk.Navigate(44, Direction.Down);
      target.Should().Be(67);

      chunk.ExposedRootView.RecordPosition(target).Should().Be(position + new Point(0, +15));
    }

    [Test]
    public void NavigateDown_Enter_Element()
    {
      // This test demonstrates that a single box cannot provide navigation across sibling boxes.
      var chunk = CreateView();
      chunk.Arrange(new Rectangle(10, 20, 200, 400));

      var target = chunk.Navigate(chunk.Offset - 1, Direction.Down);
      target.Should().Be(chunk.Offset);
    }

    [Test]
    public void NavigateDown_Inside_Element()
    {
      var chunk = CreateView();
      chunk.Arrange(new Rectangle(10, 20, 200, 400));
      var source = chunk.ExposedRootView.RecordPosition(chunk.Offset + 1);

      var target = chunk.Navigate(chunk.Offset + 1, Direction.Down);
      target.Should().Be(19);

      chunk.ExposedRootView.RecordPosition(target).Should().Be(source + new Point(0, 15));
    }

    [Test]
    public void NavigateDown_Leave_Element()
    {
      var chunk = CreateView();
      chunk.Arrange(new Rectangle(10, 20, 200, 400));

      var target = chunk.Navigate(chunk.EndOffset - 1, Direction.Down);
      target.Should().Be(chunk.EndOffset);
    }

    [Test]
    public void NavigateLeft_Inside()
    {
      var chunk = CreateView();
      chunk.Arrange(new Rectangle(10, 20, 200, 400));

      var target = chunk.Navigate(chunk.EndOffset, Direction.Left);
      target.Should().Be(chunk.EndOffset - 1);
    }

    [Test]
    public void NavigateLeft_Inside_Leave_Element()
    {
      var chunk = CreateView();
      chunk.Arrange(new Rectangle(10, 20, 200, 400));

      var target = chunk.Navigate(chunk.Offset + 1, Direction.Left);
      target.Should().Be(chunk.Offset);
    }

    [Test]
    public void NavigateLeft_TowardsEnd()
    {
      var chunk = CreateView();
      chunk.Arrange(new Rectangle(10, 20, 200, 400));

      var target = chunk.Navigate(chunk.EndOffset + 1, Direction.Left);
      target.Should().Be(chunk.EndOffset);
    }

    [Test]
    public void NavigateRight_BeyondEnd()
    {
      var chunk = CreateView();

      chunk.Arrange(new Rectangle(10, 20, 200, 400));
      var target = chunk.Navigate(chunk.EndOffset, Direction.Right);
      target.Should().Be(chunk.EndOffset);
    }

    [Test]
    public void NavigateRight_Inside()
    {
      var chunk = CreateView();
      chunk.Arrange(new Rectangle(10, 20, 200, 400));
      var target = chunk.Navigate(chunk.Offset, Direction.Right);
      target.Should().Be(chunk.Offset + 1);
    }

    [Test]
    public void NavigateRight_Inside_Leave_Element()
    {
      var chunk = CreateView();
      chunk.Arrange(new Rectangle(10, 20, 200, 400));
      var target = chunk.Navigate(chunk.EndOffset - 1, Direction.Right);
      target.Should().Be(chunk.EndOffset);
    }

    [Test]
    public void NavigateUp_Cross_Lines()
    {
      var chunk = CreateView();
      chunk.Arrange(new Rectangle(10, 20, 400, 400));
      var position = chunk.ExposedRootView.RecordPosition(44);

      // offset is near the end of line 2, in word "here".
      var target = chunk.Navigate(44, Direction.Up);
      target.Should().Be(19);

      chunk.ExposedRootView.RecordPosition(target).Should().Be(position + new Point(0, -15));
    }

    [Test]
    public void NavigateUp_Enter_Element()
    {
      // This test demonstrates that a single box cannot provide navigation across sibling boxes.
      var chunk = CreateView();
      chunk.Arrange(new Rectangle(10, 20, 200, 400));

      var target = chunk.Navigate(chunk.EndOffset + 1, Direction.Up);
      target.Should().Be(chunk.EndOffset);
    }

    [Test]
    public void NavigateUp_Inside_Element()
    {
      var chunk = CreateView();
      chunk.Arrange(new Rectangle(10, 20, 200, 400));
      var position = chunk.ExposedRootView.RecordPosition(chunk.EndOffset - 1);
      var target = chunk.Navigate(chunk.EndOffset - 1, Direction.Up);
      target.Should().Be(77);

      chunk.ExposedRootView.RecordPosition(target).Should().Be(position + new Point(0, -15));
    }

    [Test]
    public void NavigateUp_Leave_Element()
    {
      var chunk = CreateView();
      chunk.Arrange(new Rectangle(10, 20, 200, 400));

      var target = chunk.Navigate(chunk.Offset + 1, Direction.Up);
      target.Should().Be(chunk.Offset);
    }

    [Test]
    public void Paragraphs_Contain_Correct_Text()
    {
      var documentView = NodeTestExtensions.SetUp();

      documentView.Arrange(new Rectangle(10, 20, 200, 500));
      documentView.ExposedRootView.Count.Should().Be(2);
      documentView.ExposedRootView[0].Node.ExtractNodeText().Should().Be("Hello World, Here I am. Long text ahead here. This is the first paragraph.\n");
      documentView.ExposedRootView[1].Node.ExtractNodeText().Should().Be("After a line break, we should see a second paragraph in the document.");
    }

    [Test]
    public void Paragraphs_Layout_Center()
    {
      var documentView = NodeTestExtensions.SetUp(Alignment.Center);

      documentView.Arrange(new Rectangle(10, 20, 200, 500));
      documentView.ExposedRootView.Count.Should().Be(2);
      documentView.ExposedRootView[0].ExtractFromChildren(v => v.LayoutRect.X).Should().BeEquivalentTo(11, 22, 16, 38, 49);
      documentView.ExposedRootView[0].ExtractFromChildren(v => v.LayoutRect.X + v.LayoutRect.Width).Should().BeEquivalentTo(209, 198, 203, 181, 170);
      documentView.ExposedRootView[1].ExtractFromChildren(v => v.LayoutRect.X).Should().BeEquivalentTo(38, 16, 38, 16, 60);
      documentView.ExposedRootView[1].ExtractFromChildren(v => v.LayoutRect.X + v.LayoutRect.Width).Should().BeEquivalentTo(181, 203, 181, 203, 159);
    }

    [Test]
    public void Paragraphs_Layout_Left()
    {
      var documentView = NodeTestExtensions.SetUp(Alignment.Start);

      documentView.Arrange(new Rectangle(10, 20, 200, 500));
      documentView.ExposedRootView.Count.Should().Be(2);
      documentView.ExposedRootView[0].ExtractFromChildren(v => v.LayoutRect.X).Should().BeEquivalentTo(10, 10, 10, 10, 10);
      documentView.ExposedRootView[1].ExtractFromChildren(v => v.LayoutRect.X).Should().BeEquivalentTo(10, 10, 10, 10, 10);
    }

    /// <summary>
    ///   Paragraph layout is independent of the actual text alignment. Paragraphs take up the
    ///   complete space within the layout area.
    /// </summary>
    [Test]
    public void Paragraphs_Layout_Paragraphs()
    {
      var documentView = NodeTestExtensions.SetUp();

      documentView.Arrange(new Rectangle(10, 20, 200, 500));
      documentView.ExposedRootView.Count.Should().Be(2);
      documentView.ExposedRootView[0].LayoutRect.Should().Be(new Rectangle(10, 20, 200, 75));
      documentView.ExposedRootView[0].ExtractFromChildren(v => v.LayoutRect.Width).Should().BeEquivalentTo(198, 176, 187, 143, 121);
      documentView.ExposedRootView[1].LayoutRect.Should().Be(new Rectangle(10, 95, 200, 75));
      documentView.ExposedRootView[1].ExtractFromChildren(v => v.LayoutRect.Width).Should().BeEquivalentTo(143, 187, 143, 187, 99);
    }

    [Test]
    public void Paragraphs_Layout_Right()
    {
      var documentView = NodeTestExtensions.SetUp(Alignment.End);

      documentView.Arrange(new Rectangle(10, 20, 200, 500));
      documentView.ExposedRootView.Count.Should().Be(2);
      documentView.ExposedRootView[0].ExtractFromChildren(v => v.LayoutRect.X).Should().BeEquivalentTo(12, 34, 23, 67, 89);
      documentView.ExposedRootView[0].ExtractFromChildren(v => v.LayoutRect.X + v.LayoutRect.Width).Should().BeEquivalentTo(210, 210, 210, 210, 210);
      documentView.ExposedRootView[1].ExtractFromChildren(v => v.LayoutRect.X).Should().BeEquivalentTo(67, 23, 67, 23, 111);
      documentView.ExposedRootView[1].ExtractFromChildren(v => v.LayoutRect.X + v.LayoutRect.Width).Should().BeEquivalentTo(210, 210, 210, 210, 210);
    }

    TestDocumentView<PlainTextDocument> CreateView(string text = null)
    {
      return NodeTestExtensions.SetUp(Alignment.Start, text ?? "Hello World, Here I am. \nLong text ahead here. \nA long word, that is impossible to break apart.");
    }
  }
}