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
using Steropes.UI.Widgets.TextWidgets;
using Steropes.UI.Widgets.TextWidgets.Documents;
using Steropes.UI.Widgets.TextWidgets.Documents.Edits;
using Steropes.UI.Widgets.TextWidgets.Documents.Views;

namespace Steropes.UI.Test.UI.TextWidgets.Documents.PlainText
{
  [Category("Text Behaviour")]
  public class DocumentView_ChangeNotification_Test
  {
    public TestDocumentView<ITextDocument> CreateView()
    {
      IUIStyle style = LayoutTestStyle.Create();

      var chunk0 = Create(0, 5);
      var chunk1 = Create(5, 10);
      var chunk2 = Create(10, 15);

      var rootNode = Substitute.For<ITextNode>();
      rootNode.Count.Returns(2);
      rootNode[0].Returns(chunk0);
      rootNode[1].Returns(chunk1);
      rootNode[2].Returns(chunk2);
      rootNode.Offset.Returns(0);
      rootNode.EndOffset.Returns(10);

      var rootDocument = Substitute.For<ITextDocument>();
      rootDocument.Root.Returns(rootNode);

      var view0 = Substitute.For<ITextView<ITextDocument>>();
      view0.Node.Returns(chunk0);
      view0.Offset.Returns(chunk0.Offset);
      view0.EndOffset.Returns(chunk0.EndOffset);

      var view1 = Substitute.For<ITextView<ITextDocument>>();
      view1.Node.Returns(chunk1);
      view1.Offset.Returns(chunk1.Offset);
      view1.EndOffset.Returns(chunk1.EndOffset);

      var view2 = Substitute.For<ITextView<ITextDocument>>();
      view2.Node.Returns(chunk2);
      view2.Offset.Returns(chunk2.Offset);
      view2.EndOffset.Returns(chunk2.EndOffset);

      var rootView = Substitute.For<ITextView<ITextDocument>>();
      rootView.Node.Returns(rootNode);
      rootView.Count.Returns(3);
      rootView[0].Returns(view0);
      rootView[1].Returns(view1);
      rootView[2].Returns(view2);

      var viewFactory = Substitute.For<ITextNodeViewFactory<ITextDocument>>();
      viewFactory.CreateFor(rootNode, Arg.Any<IStyle>()).Returns(rootView);
      viewFactory.CreateFor(rootNode[0], Arg.Any<IStyle>()).Returns(view0);
      viewFactory.CreateFor(rootNode[1], Arg.Any<IStyle>()).Returns(view1);
      viewFactory.CreateFor(rootNode[2], Arg.Any<IStyle>()).Returns(view2);

      var editor = Substitute.For<IDocumentEditor<DocumentView<ITextDocument>, ITextDocument>>();
      editor.CreateDocument().Returns(rootDocument);
      editor.Style.Returns(style);
      editor.CreateViewFactory().Returns(viewFactory);

      var documentView = new TestDocumentView<ITextDocument>(editor);
      rootNode[0].ClearReceivedCalls();
      rootNode[1].ClearReceivedCalls();
      rootNode[2].ClearReceivedCalls();
      rootNode.ClearReceivedCalls();
      rootDocument.ClearReceivedCalls();
      editor.ClearReceivedCalls();
      rootView.ClearReceivedCalls();
      rootView[0].ClearReceivedCalls();
      rootView[1].ClearReceivedCalls();
      rootView[2].ClearReceivedCalls();
      return documentView;
    }

    [Test]
    public void DocumentChanges_Invalidate_DeepNodes()
    {
      var documentView = CreateView();

      var parent = Substitute.For<IWidget>();
      parent.Parent.Returns((IWidget)null);

      var editForNode = Substitute.For<IElementEdit>();

      var editInfo = Substitute.For<IDocumentEditInfo>();
      editInfo.Offset.Returns(0);
      editInfo.Length.Returns(1);
      editInfo.Document.Returns(documentView.Document);
      editInfo.ModificationType.Returns(TextModificationType.Change);
      IElementEdit edit;
      editInfo.IsNodeAffected(documentView.Document.Root, out edit).Returns(
        x =>
          {
            x[1] = editForNode;
            return true;
          });
      editInfo.IsNodeAffected(documentView.Document.Root[0], out edit).Returns(
        x =>
          {
            x[1] = editForNode;
            return true;
          });

      documentView.AddNotify(parent);
      documentView.Document.UndoableEditCreated += Raise.EventWith(documentView.Document, new UndoableEditEventArgs(editInfo, false));

      documentView.ExposedRootView.Received().OnNodeStructureChanged(documentView, Arg.Any<IElementEdit>());
      documentView.ExposedRootView[0].Received().OnNodeStructureChanged(documentView, Arg.Any<IElementEdit>());
      documentView.ExposedRootView[1].DidNotReceive().OnNodeStructureChanged(documentView, Arg.Any<IElementEdit>());
      documentView.ExposedRootView[2].DidNotReceive().OnNodeStructureChanged(documentView, Arg.Any<IElementEdit>());
    }

    [Test]
    public void DocumentChanges_InvalidateLayout()
    {
      var documentView = CreateView();

      var parent = Substitute.For<IWidget>();
      parent.Parent.Returns((IWidget)null);

      var editForNode = Substitute.For<IElementEdit>();

      var editInfo = Substitute.For<IDocumentEditInfo>();
      editInfo.Offset.Returns(0);
      editInfo.Length.Returns(1);
      editInfo.Document.Returns(documentView.Document);
      editInfo.ModificationType.Returns(TextModificationType.Change);
      IElementEdit edit;
      editInfo.IsNodeAffected(documentView.Document.Root, out edit).Returns(
        x =>
          {
            x[1] = editForNode;
            return true;
          });

      documentView.AddNotify(parent);
      documentView.Arrange(new Rectangle(10, 20, 200, 50));
      documentView.LayoutInvalid.Should().Be(false);

      documentView.Document.UndoableEditCreated += Raise.EventWith(documentView.Document, new UndoableEditEventArgs(editInfo, true));

      parent.Received().InvalidateLayout();
    }

    ITextNode Create(int start, int end)
    {
      var chunk0 = Substitute.For<ITextNode>();
      chunk0.Offset.Returns(start);
      chunk0.EndOffset.Returns(end);
      return chunk0;
    }
  }
}