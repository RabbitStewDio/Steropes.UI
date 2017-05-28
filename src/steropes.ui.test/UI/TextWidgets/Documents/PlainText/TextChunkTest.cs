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
using Steropes.UI.Platform;
using Steropes.UI.Styles;
using Steropes.UI.Widgets.Styles;
using Steropes.UI.Widgets.TextWidgets.Documents;
using Steropes.UI.Widgets.TextWidgets.Documents.Helper;
using Steropes.UI.Widgets.TextWidgets.Documents.PlainText;
using Steropes.UI.Widgets.TextWidgets.Documents.Views;

namespace Steropes.UI.Test.UI.TextWidgets.Documents.PlainText
{
  [Category("Text Behaviour")]
  public class TextChunkTest
  {
    readonly IStyleSystem styleSystem;

    readonly IStyle textStyle;

    public TextChunkTest()
    {
      styleSystem = LayoutTestStyle.CreateStyleSystem();
      textStyle = LayoutTestStyle.CreateTextStyle(styleSystem);
    }

    [Test]
    public void BufferOffsets()
    {
      var text = "AAAAABBBBBCCCCC";
      var chunk = CreateChunk(5, 15, text);
      chunk.TextFor(chunk.Offset, 15).Should().Be("BBBBBCCCCC");
    }

    [Test]
    public void DocumentChanges_Trigger_InvalidateLayout()
    {
      var docView = Substitute.For<IDocumentView<PlainTextDocument>>();
      var chunk = CreateChunk();
      var parentView = Substitute.For<ITextView<PlainTextDocument>>();
      chunk.AddNotify(parentView);
      chunk.Arrange(new Rectangle(10, 20, 200, 50));
      chunk.LayoutInvalid.Should().Be(false);

      var edit = Substitute.For<IElementEdit>();
      edit.NewElement.Returns(chunk.Node);

      chunk.OnNodeStructureChanged(docView, edit);

      chunk.Node.Should().Be(edit.NewElement);
      parentView.Received().InvalidateLayout();
    }

    [Test]
    public void Draw()
    {
      var drawingService = Substitute.For<IBatchedDrawingService>();

      var textStyle = styleSystem.StylesFor<TextStyleDefinition>();

      var text = "  Hello World! ";

      var chunk = CreateChunk(null, null, text);
      chunk.Arrange(new Rectangle(10, 20, 200, 20));

      var font = chunk.Style.GetValue(textStyle.Font);
      var textColor = chunk.Style.GetValue(textStyle.TextColor);
      var outlineColor = chunk.Style.GetValue(textStyle.OutlineColor);
      var outlineRadius = chunk.Style.GetValue(textStyle.OutlineSize);

      chunk.Draw(drawingService);

      drawingService.Received().DrawBlurredText(font, "Hello World!", new Vector2(10 + 22, 20 + font.Baseline), textColor, outlineRadius, outlineColor);
    }

    [Test]
    public void InitialCounts()
    {
      var chunk = ExposingTestChunk.Create("  Test this    \n", styleSystem);
      chunk.TrimStart.Should().Be(2);
      chunk.TrimEnd.Should().Be(5);
      chunk.TrimLineBreaks.Should().Be(1);
    }

    [Test]
    public void MapModelToView_EndOffset()
    {
      var chunk = CreateChunk();
      chunk.Measure(Size.Auto);
      chunk.Arrange(new Rectangle(10, 20, chunk.DesiredSize.WidthInt, chunk.DesiredSize.HeightInt));

      Rectangle rect;
      chunk.ModelToView(chunk.EndOffset, out rect).Should().BeTrue();
      rect.Should().Be(new Rectangle(chunk.LayoutRect.Right, 20, 0, 15));
    }

    [Test]
    public void MapModelToView_First()
    {
      var chunk = CreateChunk();
      chunk.Measure(Size.Auto);
      chunk.Arrange(new Rectangle(10, 20, chunk.DesiredSize.WidthInt, chunk.DesiredSize.HeightInt));

      Rectangle rect;
      chunk.ModelToView(chunk.Offset, out rect).Should().BeTrue();
      rect.Should().Be(new Rectangle(10, 20, 11, 15));
    }

    [Test]
    public void MapModelToView_Last()
    {
      var chunk = CreateChunk();
      chunk.Measure(Size.Auto);
      chunk.Arrange(new Rectangle(10, 20, chunk.DesiredSize.WidthInt, chunk.DesiredSize.HeightInt));

      Rectangle rect;
      chunk.ModelToView(chunk.EndOffset - 1, out rect).Should().BeTrue();
      rect.Should().Be(new Rectangle(chunk.LayoutRect.Right - 11, 20, 11, 15));
    }

    [Test]
    public void MapModelToView_Outside()
    {
      var chunk = CreateChunk();
      chunk.Measure(Size.Auto);
      chunk.Arrange(new Rectangle(10, 20, chunk.DesiredSize.WidthInt, chunk.DesiredSize.HeightInt));
      Rectangle rect;
      chunk.ModelToView(-1, out rect).Should().BeFalse();
      chunk.ModelToView(chunk.Node.EndOffset + 1, out rect).Should().BeFalse();
    }

    [Test]
    public void MapViewToModel_Inside()
    {
      var chunk = CreateChunk();
      chunk.Measure(Size.Auto);
      int offset;
      Bias bias;
      chunk.Arrange(new Rectangle(10, 20, chunk.DesiredSize.WidthInt, chunk.DesiredSize.HeightInt));
      chunk.ViewToModel(new Point(50, 25), out offset, out bias).Should().Be(true);
      offset.Should().Be(3, "Expect to break at 3 (available width: 40, per char-width 11, thus 3 chars fit the space fully)");
      bias.Should().Be(Bias.Forward);
    }

    [Test]
    public void MapViewToModel_Inside_BackwardBias()
    {
      var chunk = CreateChunk();
      chunk.Measure(Size.Auto);
      int offset;
      Bias bias;
      chunk.Arrange(new Rectangle(10, 20, chunk.DesiredSize.WidthInt, chunk.DesiredSize.HeightInt));
      chunk.ViewToModel(new Point(45, 25), out offset, out bias).Should().Be(true);
      offset.Should().Be(3, "Expect to break at 3 (available width: 35, per char-width 11, thus 3 chars fit the space fully)");
      bias.Should().Be(Bias.Backward);
    }

    [Test]
    public void MapViewToModel_Inside_OnLeft_Edge()
    {
      var chunk = CreateChunk();
      chunk.Measure(Size.Auto);
      int offset;
      Bias bias;
      chunk.Arrange(new Rectangle(10, 20, chunk.DesiredSize.WidthInt, chunk.DesiredSize.HeightInt));
      chunk.ViewToModel(new Point(10, 25), out offset, out bias).Should().Be(true);
      offset.Should().Be(0, "Expect to break at 3 (available width: 35, per char-width 11, thus 3 chars fit the space fully)");
      bias.Should().Be(Bias.Backward);
    }

    [Test]
    public void MapViewToModel_Inside_OnRight_Edge()
    {
      var chunk = CreateChunk();
      chunk.Measure(Size.Auto);
      int offset;
      Bias bias;
      chunk.Arrange(new Rectangle(10, 20, chunk.DesiredSize.WidthInt, chunk.DesiredSize.HeightInt));
      chunk.ViewToModel(new Point(10 + chunk.DesiredSize.WidthInt - 1, 25), out offset, out bias).Should().Be(true);
      offset.Should().Be(chunk.EndOffset - 1);
      bias.Should().Be(Bias.Forward);
    }

    [Test]
    public void MapViewToModel_Outside_Of_Layout_Area()
    {
      var chunk = CreateChunk();
      chunk.Measure(Size.Auto);
      int offset;
      Bias bias;
      chunk.Arrange(new Rectangle(10, 20, chunk.DesiredSize.WidthInt, chunk.DesiredSize.HeightInt));
      chunk.ViewToModel(new Point(chunk.DesiredSize.WidthInt + 10 + 1, 25), out offset, out bias).Should().Be(false);
      chunk.ViewToModel(new Point(5, 25), out offset, out bias).Should().Be(false);
      chunk.ViewToModel(new Point(55, 5), out offset, out bias).Should().Be(false);
      chunk.ViewToModel(new Point(55, 505), out offset, out bias).Should().Be(false);
    }

    [Test]
    public void NavigateLeft_BeyondStart()
    {
      var chunk = CreateChunk();
      chunk.Measure(Size.Auto);
      chunk.Arrange(new Rectangle(10, 20, chunk.DesiredSize.WidthInt, chunk.DesiredSize.HeightInt));
      int target;
      chunk.Navigate(-1, Direction.Left, out target).Should().Be(NavigationResult.Invalid);
      target.Should().Be(-1);
    }

    [Test]
    public void NavigateLeft_Inside()
    {
      var chunk = CreateChunk();
      chunk.Measure(Size.Auto);
      chunk.Arrange(new Rectangle(10, 20, chunk.DesiredSize.WidthInt, chunk.DesiredSize.HeightInt));

      int target;
      chunk.Navigate(chunk.EndOffset, Direction.Left, out target).Should().Be(NavigationResult.Valid);
      target.Should().Be(chunk.EndOffset - 1);
    }

    [Test]
    public void NavigateLeft_Inside_Leave_Element()
    {
      var chunk = CreateChunk();
      chunk.Measure(Size.Auto);
      chunk.Arrange(new Rectangle(10, 20, chunk.DesiredSize.WidthInt, chunk.DesiredSize.HeightInt));

      int target;
      chunk.Navigate(chunk.Offset + 1, Direction.Left, out target).Should().Be(NavigationResult.BoundaryChanged);
      target.Should().Be(chunk.Offset);
    }

    [Test]
    public void NavigateLeft_TowardsEnd()
    {
      var chunk = CreateChunk();
      chunk.Measure(Size.Auto);
      chunk.Arrange(new Rectangle(10, 20, chunk.DesiredSize.WidthInt, chunk.DesiredSize.HeightInt));

      int target;
      chunk.Navigate(chunk.EndOffset + 1, Direction.Left, out target).Should().Be(NavigationResult.Valid);
      target.Should().Be(chunk.EndOffset);
    }

    [Test]
    public void NavigateRight_BeyondEnd()
    {
      var chunk = CreateChunk();
      chunk.Measure(Size.Auto);
      chunk.Arrange(new Rectangle(10, 20, chunk.DesiredSize.WidthInt, chunk.DesiredSize.HeightInt));
      int target;
      chunk.Navigate(chunk.EndOffset, Direction.Right, out target).Should().Be(NavigationResult.Invalid);
      target.Should().Be(-1);
    }

    [Test]
    public void NavigateRight_Inside()
    {
      var chunk = CreateChunk();
      chunk.Measure(Size.Auto);
      chunk.Arrange(new Rectangle(10, 20, chunk.DesiredSize.WidthInt, chunk.DesiredSize.HeightInt));
      int target;
      chunk.Navigate(chunk.Offset, Direction.Right, out target).Should().Be(NavigationResult.Valid);
      target.Should().Be(chunk.Offset + 1);
    }

    [Test]
    public void NavigateRight_Inside_Leave_Element()
    {
      var chunk = CreateChunk();
      chunk.Measure(Size.Auto);
      chunk.Arrange(new Rectangle(10, 20, chunk.DesiredSize.WidthInt, chunk.DesiredSize.HeightInt));
      int target;
      chunk.Navigate(chunk.EndOffset - 1, Direction.Right, out target).Should().Be(NavigationResult.BoundaryChanged);
      target.Should().Be(chunk.EndOffset);
    }

    [Test]
    public void NavigateRight_TowardsStart()
    {
      var chunk = CreateChunk();
      chunk.Measure(Size.Auto);
      chunk.Arrange(new Rectangle(10, 20, chunk.DesiredSize.WidthInt, chunk.DesiredSize.HeightInt));
      int target;
      chunk.Navigate(-1, Direction.Right, out target).Should().Be(NavigationResult.Valid);
      target.Should().Be(chunk.Offset);
    }

    [Test]
    public void Navigation_Is_Not_allowed_on_Invalid_layout()
    {
      var chunk = CreateChunk();
      Rectangle rect;
      int offset;
      Bias bias;
      chunk.ModelToView(10, out rect).Should().Be(false, "View mapping operations are not allowed when the layout is not valid.");
      chunk.ViewToModel(new Point(10, 10), out offset, out bias).Should().Be(false, "View mapping operations are not allowed when the layout is not valid.");
      chunk.Navigate(10, Direction.Left, out offset).Should().Be(NavigationResult.Invalid);
    }

    [Test]
    public void Spacing_At_Start_End_Adjusts_Arrange()
    {
      var text = "  Hello World! ";
      var chunk = CreateChunk(null, null, text);
      chunk.Arrange(new Rectangle(10, 20, 200, 20));
      chunk.LayoutRect.Width.Should().Be(11 * text.Length);
      chunk.LayoutRect.X.Should().Be(10);
    }

    [Test]
    public void Spacing_At_Start_End_Adjusts_Measure()
    {
      var text = "  Hello World! ";
      var chunk = CreateChunk(null, null, text);
      chunk.Arrange(new Rectangle(10, 20, 200, 20));
      chunk.DesiredSize.Width.Should().Be(11 * text.Length);
    }

    [Test]
    public void Spacing_At_Start_End_Adjusts_Padding()
    {
      var chunk = CreateChunk(null, null, "  Hello World! ");
      chunk.Padding.Should().Be(new Insets(0, 22, 0, 11));
    }

    [Test]
    public void SubChunkCreation()
    {
      var text = "xxxxxxAAAAABBBBBCCCCC";
      var chunk = CreateChunk(6, 21, text);
      var subChunk = chunk.SubChunk(chunk.Node.Document.CreatePosition(11, Bias.Forward), chunk.Node.Document.CreatePosition(16, Bias.Backward));
      subChunk.TextFor(subChunk.Offset, 16).Should().Be("BBBBB");
    }

    [Test]
    public void TestAllSpace()
    {
      var text = "     ";
      var chunk = CreateChunk(null, null, text);
      chunk.Measure(Size.Auto);
      chunk.DesiredSize.Should().Be(new Size(text.Length * 11, 15));
    }

    [Test]
    public void TestWidthAndHeight()
    {
      var text = "Some text";
      var chunk = CreateChunk(null, null, text);
      chunk.Measure(Size.Auto);
      chunk.DesiredSize.Should().Be(new Size(text.Length * 11, 15));
    }

    [Test]
    public void TextChunkBreakingTestAtNonZeroOffset()
    {
      var chunk = CreateChunk(10);
      ITextChunkView<PlainTextDocument> first;
      ITextChunkView<PlainTextDocument> second;
      chunk.BreakAt(200, out first, out second);
      first.Offset.Should().Be(10);
      first.EndOffset.Should().Be(28);
      first.Text.Should().Be("d, Here I am. Can ");
      first.Padding.Should().Be(new Insets(0, 0, 0, 11));
      first.DesiredSize.Width.Should().BeLessOrEqualTo(200);
      second.Offset.Should().Be(28);
      second.EndOffset.Should().Be(65);
      second.Text.Should().Be("you see me? This is on the next line.");

      second.BreakAt(200, out first, out second);
      first.Offset.Should().Be(28);
      first.EndOffset.Should().Be(45);
      first.Text.Should().Be("you see me? This ");
      first.Padding.Should().Be(new Insets(0, 0, 0, 11));
      first.DesiredSize.Width.Should().BeLessOrEqualTo(200);
      second.Offset.Should().Be(45);
      second.EndOffset.Should().Be(65);
      second.Text.Should().Be("is on the next line.");
    }

    [Test]
    public void TextChunkBreakingTestAtZero()
    {
      var chunk = CreateChunk();
      ITextChunkView<PlainTextDocument> first;
      ITextChunkView<PlainTextDocument> second;
      chunk.BreakAt(200, out first, out second);
      first.Offset.Should().Be(0);
      first.EndOffset.Should().Be(18);
      first.DesiredSize.Width.Should().BeLessOrEqualTo(200);
      first.Text.Should().Be("Hello World, Here ");
      first.Padding.Should().Be(new Insets(0, 0, 0, 11));
      second.Offset.Should().Be(18);
      second.EndOffset.Should().Be(65);
      second.Text.Should().Be("I am. Can you see me? This is on the next line.");
    }

    TextChunkView<PlainTextDocument> CreateChunk(int? start = null, int? end = null, string text = null)
    {
      var doc = new PlainTextDocument();
      doc.InsertAt(0, text ?? "Hello World, Here I am. Can you see me? This is on the next line.");
      var node = doc.Root[0];

      var startOffset = start ?? node.Offset;
      var endOffset = end ?? node.EndOffset;

      var chunk = new TextChunkView<PlainTextDocument>(
        new TextProcessingRules(),
        node,
        textStyle,
        node.Document.CreatePosition(startOffset, Bias.Forward),
        node.Document.CreatePosition(endOffset, Bias.Backward));
      chunk.Initialize();
      return chunk;
    }
  }
}