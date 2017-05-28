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
using Microsoft.Xna.Framework;

using NSubstitute;

using NUnit.Framework;

using Steropes.UI.Components;
using Steropes.UI.Platform;
using Steropes.UI.Test.UI.TextWidgets.Documents.PlainText;
using Steropes.UI.Widgets.TextWidgets.Documents;
using Steropes.UI.Widgets.TextWidgets.Documents.PlainText;
using Steropes.UI.Widgets.TextWidgets.Documents.Views;

namespace Steropes.UI.Test.UI.TextWidgets.Documents
{
  [Category("Text Behaviour")]
  public class HighlighterTest
  {
    [Test]
    public void Draw_Multiple_lines()
    {
      var start = Substitute.For<ITextPosition>();
      start.Bias.Returns(Bias.Forward);
      start.Offset.Returns(5);

      var end = Substitute.For<ITextPosition>();
      end.Bias.Returns(Bias.Backward);
      end.Offset.Returns(10);

      var drawingService = Substitute.For<IBatchedDrawingService>();
      var docView = Substitute.For<IDocumentView<ITextDocument>>();
      docView.LayoutRect.Returns(new Rectangle(10, 20, 120, 200));
      Rectangle rect;
      docView.ModelToView(5, out rect).Returns(
        x =>
          {
            x[1] = new Rectangle(50, 20, 11, 15);
            return true;
          });
      docView.ModelToView(10, out rect).Returns(
        x =>
          {
            x[1] = new Rectangle(80, 100, 11, 15);
            return true;
          });

      var style = LayoutTestStyle.Create();

      var h = new Highlight<ITextDocument>(start, end, style.StyleSystem.CreatePresentationStyle());
      h.Draw(drawingService, docView);

      Received.InOrder(
        () =>
          {
            drawingService.FillRect(new Rectangle(50, 20, 80, 15), Arg.Any<Color>());
            drawingService.FillRect(new Rectangle(10, 35, 120, 65), Arg.Any<Color>());
            drawingService.FillRect(new Rectangle(10, 100, 70, 15), Arg.Any<Color>());
          });
    }

    [Test]
    public void Draw_Single_line()
    {
      var start = Substitute.For<ITextPosition>();
      start.Bias.Returns(Bias.Forward);
      start.Offset.Returns(5);

      var end = Substitute.For<ITextPosition>();
      end.Bias.Returns(Bias.Backward);
      end.Offset.Returns(10);

      var drawingService = Substitute.For<IBatchedDrawingService>();
      var docView = Substitute.For<IDocumentView<ITextDocument>>();
      docView.LayoutRect.Returns(new Rectangle(10, 20, 120, 200));
      Rectangle rect;
      docView.ModelToView(5, out rect).Returns(
        x =>
          {
            x[1] = new Rectangle(50, 20, 11, 15);
            return true;
          });
      docView.ModelToView(10, out rect).Returns(
        x =>
          {
            x[1] = new Rectangle(80, 20, 11, 15);
            return true;
          });
      var style = LayoutTestStyle.Create();

      var h = new Highlight<ITextDocument>(start, end, style.StyleSystem.CreatePresentationStyle());
      h.Draw(drawingService, docView);

      Received.InOrder(() => { drawingService.FillRect(new Rectangle(50, 20, 30, 15), Arg.Any<Color>()); });
    }

    TestDocumentView<PlainTextDocument> CreateView(string text = null)
    {
      return NodeTestExtensions.SetUp(Alignment.Start, text ?? "Hello World, Here I am. \nLong text ahead here. \nA long word, that is impossible to break apart.");
    }
  }
}