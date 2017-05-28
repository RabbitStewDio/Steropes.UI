using FluentAssertions;
using Microsoft.Xna.Framework;
using NUnit.Framework;
using Steropes.UI.Components;
using Steropes.UI.Styles;
using Steropes.UI.Widgets.Styles;
using Steropes.UI.Widgets.TextWidgets.Documents.PlainText;
using Steropes.UI.Widgets.TextWidgets.Documents.Views;

namespace Steropes.UI.Test.UI.TextWidgets.Documents.PlainText
{
  public class TextAlignmentTest
  {
    IStyle textStyle;
    IStyleSystem styleSystem;
    TextStyleDefinition textStyleDefinition;

    [SetUp]
    public void SetUp()
    {
      // This test uses a 11x16 font. 
      styleSystem = LayoutTestStyle.CreateStyleSystem();
      textStyleDefinition = styleSystem.StylesFor<TextStyleDefinition>();
      textStyle = LayoutTestStyle.CreateTextStyle(styleSystem);
    }  
    
    [Test]
    public void Arrange_Left()
    {
      textStyle.SetValue(textStyleDefinition.Alignment, Alignment.Start);

      var text = "Hello World, Here I am.";
      var view = CreateView(text);
      view.Arrange(new Rectangle(10, 20, 400, 100));

      view[0].LayoutRect.Location.Should().Be(new Point(10, 20));
      view[0].LayoutRect.Size.Should().Be(new Point(text.Length * 11, 15));
    }

    [Test]
    public void Arrange_Right()
    {
      textStyle.SetValue(textStyleDefinition.Alignment, Alignment.End);

      var text = "Hello World, Here I am.";
      var view = CreateView(text);
      view.Arrange(new Rectangle(10, 20, 400, 100));

      var textSize = text.Length * 11;
      view[0].LayoutRect.Location.Should().Be(new Point(410 - textSize, 20));
      view[0].LayoutRect.Size.Should().Be(new Point(textSize, 15));
    }

    [Test]
    public void Arrange_Center()
    {
      textStyle.SetValue(textStyleDefinition.Alignment, Alignment.Center);

      var text = "Hello World, Here I am.";
      var view = CreateView(text);
      view.Arrange(new Rectangle(10, 20, 400, 100));
      view.Count.Should().Be(1);
      view[0].LayoutRect.Location.Should().Be(new Point(83, 20));
      view[0].LayoutRect.Size.Should().Be(new Point(text.Length * 11, 15));
    }

    [Test]
    public void Arrange_Justified_small_text()
    {
      textStyle.SetValue(textStyleDefinition.Alignment, Alignment.Fill);

      var text = "Hello World, Here I am.";
      var view = CreateView(text);
      view.Arrange(new Rectangle(10, 20, 400, 100));

      view[0].Should().BeAssignableTo<JustifiedTextChunkView<PlainTextDocument>>();
      view[0].LayoutRect.Location.Should().Be(new Point(10, 20));
      view[0].LayoutRect.Size.Should().Be(new Point(253, 15));
    }

    [Test]
    public void Arrange_Justified_large_text()
    {
      textStyle.SetValue(textStyleDefinition.Alignment, Alignment.Fill);

      var text = "Hello World, Here I am. Long text ahead here. A long word, here.";
      var view = CreateView(text);
      view.Arrange(new Rectangle(10, 20, 400, 100));
      view.Count.Should().Be(2);

      view[0].Should().BeAssignableTo<JustifiedTextChunkView<PlainTextDocument>>();
      view[0].LayoutRect.Location.Should().Be(new Point(10, 20));
      view[0].LayoutRect.Size.Should().Be(new Point(400, 15));

      view[1].Should().BeAssignableTo<JustifiedTextChunkView<PlainTextDocument>>();
      view[1].LayoutRect.Location.Should().Be(new Point(10, 35));
      view[1].LayoutRect.Size.Should().Be(new Point(330, 15));
    }

    ParagraphTextView<PlainTextDocument> CreateView(string text)
    {
      var doc = new PlainTextDocument();
      doc.InsertAt(0, text);
      var node = doc.Root[0];

      var view = new ParagraphTextView<PlainTextDocument>(node, textStyle, new PlainTextViewFactory());
      return view;
    }
  }
}