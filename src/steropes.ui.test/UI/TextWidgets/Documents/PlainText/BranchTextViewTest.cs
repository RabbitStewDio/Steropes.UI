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
using Steropes.UI.Styles;
using Steropes.UI.Widgets.TextWidgets.Documents;
using Steropes.UI.Widgets.TextWidgets.Documents.Views;

namespace Steropes.UI.Test.UI.TextWidgets.Documents.PlainText
{
  [Category("Text Behaviour")]
  public class BranchTextViewTest
  {
    IDocumentView<ITextDocument> doc;

    IStyleSystem styleSystem;

    public int Count { get; set; }

    [Test]
    public void DocumentChanges_Trigger_InvalidateLayout()
    {
      var docView = Substitute.For<IDocumentView<ITextDocument>>();
      var edit = Substitute.For<IElementEdit>();
      var parentView = Substitute.For<ITextView<ITextDocument>>();
      var chunk = new BranchImpl(Substitute.For<ITextNode>(), styleSystem);
      chunk.AddNotify(parentView);
      chunk.Arrange(new Rectangle(10, 20, 200, 50));
      chunk.LayoutInvalid.Should().Be(false);

      chunk.OnNodeStructureChanged(docView, edit);

      chunk.Node.Should().Be(edit.NewElement);
      parentView.Received().InvalidateLayout();
    }

    [Test]
    public void Edit_Events_Update_Nodes()
    {
      var replacementChild1 = CreateView();
      var replacementChild2 = CreateView();

      var factory = doc.ViewFactory;
      factory.CreateFor(replacementChild1.Node, Arg.Any<IStyle>()).Returns(replacementChild1);
      factory.CreateFor(replacementChild2.Node, Arg.Any<IStyle>()).Returns(replacementChild2);

      var child1 = CreateView();
      var child2 = CreateView();
      var child3 = CreateView();

      var branch = new BranchImpl(Substitute.For<ITextNode>(), styleSystem);
      branch.Add(child1);
      branch.Add(child2);
      branch.Add(child3);

      var removedNodesArray = new[] { child2.Node };
      var addedNodesArray = new[] { replacementChild1.Node, replacementChild2.Node };

      var edit = Substitute.For<IElementEdit>();
      edit.Index.Returns(1);
      edit.NewElement.Returns(Substitute.For<ITextNode>());
      edit.OldElement.Returns(branch.Node);
      edit.RemovedNodes.Returns(removedNodesArray);
      edit.AddedNodes.Returns(addedNodesArray);

      branch.OnNodeStructureChanged(doc, edit);

      branch.Node.Should().Be(edit.NewElement);
      branch.Count.Should().Be(4);
      branch[0].Should().BeSameAs(child1);
      branch[1].Should().BeSameAs(replacementChild1);
      branch[2].Should().BeSameAs(replacementChild2);
      branch[3].Should().BeSameAs(child3);

      factory.Received().CreateFor(replacementChild1.Node, Arg.Any<IStyle>());
      factory.Received().CreateFor(replacementChild2.Node, Arg.Any<IStyle>());
    }

    [Test]
    public void Navigation_Is_Not_allowed_on_Invalid_layout()
    {
      var doc = NodeTestExtensions.SetUp();
      var chunk = doc.ExposedRootView;
      Rectangle rect;
      int offset;
      Bias bias;
      chunk.ModelToView(10, out rect).Should().Be(false, "View mapping operations are not allowed when the layout is not valid.");
      chunk.ViewToModel(new Point(10, 10), out offset, out bias).Should().Be(false, "View mapping operations are not allowed when the layout is not valid.");
      chunk.Navigate(10, Direction.Left, out offset).Should().Be(NavigationResult.Invalid);
    }

    [SetUp]
    public void SetUp()
    {
      Count = 0;
      styleSystem = LayoutTestStyle.CreateStyleSystem();
      var textStyle = LayoutTestStyle.CreateTextStyle(styleSystem);

      var documentView = Substitute.For<IDocumentView<ITextDocument>>();
      documentView.Document.Returns((ITextDocument)null);
      documentView.Style.Returns(textStyle);
      this.doc = documentView;
    }

    ITextView<ITextDocument> CreateView(ITextNode node = null)
    {
      var realNode = node ?? Substitute.For<ITextNode>();
      realNode.Offset.Returns(Count);

      var view = Substitute.For<ITextView<ITextDocument>>();
      view.Node.Returns(realNode);
      return view;
    }

    class BranchImpl : BranchTextView<ITextDocument>
    {
      public BranchImpl(ITextNode node, IStyleSystem styleSystem) : base(node, LayoutTestStyle.CreateTextStyle(styleSystem))
      {
      }

      protected override Rectangle ArrangeOverride(Rectangle layoutSize)
      {
        return new Rectangle(layoutSize.X, layoutSize.Y, DesiredSize.WidthInt, DesiredSize.HeightInt);
      }

      protected override Size MeasureOverride(Size availableSize)
      {
        return new Size(100, 10);
      }
    }
  }
}