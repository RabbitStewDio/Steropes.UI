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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Steropes.UI.Components;
using Steropes.UI.Components.Helper;
using Steropes.UI.Input;
using Steropes.UI.Input.KeyboardInput;
using Steropes.UI.Input.MouseInput;
using Steropes.UI.Widgets.TextWidgets.Documents;
using Steropes.UI.Widgets.TextWidgets.Documents.Views;

namespace Steropes.UI.Widgets.TextWidgets
{
  public abstract class TextEditorWidgetBase<TView, TDocument> : TextWidgetBase<TView, TDocument>
    where TDocument : ITextDocument where TView : class, IDocumentView<TDocument>
  {
    StringBuilder insertBuffer;

    bool isDragging;

    protected TextEditorWidgetBase(IUIStyle style, IDocumentEditor<TView, TDocument> editor) : base(style, editor)
    {
      if (editor == null)
      {
        throw new ArgumentNullException();
      }

      ActionMap = new KeyboardActionMap();
      RemoteCarets = new ObservableCollection<ICaret>();
      RemoteCarets.CollectionChanged += OnRemoteCaretsChanged;

      Caret = new Caret<TView, TDocument>(UIStyle, Content);
      Caret.AddNotify(this);
      RaiseChildrenChanged(null, Caret);

      KeyTyped += OnKeyTyped;
      KeyPressed += OnKeyPressed;
      KeyRepeated += OnKeyPressed;

      MouseUp += OnMouseUp;
      MouseDown += OnMouseDown;
      MouseDragged += OnMouseDragged;

      Cursor = MouseCursor.IBeam;
      Focusable = true;
      ReadOnly = false;

      InstallKeyMap();
    }

    public KeyboardActionMap ActionMap { get; }

    public ICaret Caret { get; }

    public override int Count => base.Count + 1 + RemoteCarets.Count;

    public bool ReadOnly { get; set; }

    public ObservableCollection<ICaret> RemoteCarets { get; }

    public override string Text
    {
      get
      {
        return base.Text;
      }
      set
      {
        base.Text = value;
        Caret.MoveTo(Content.Document.TextLength);
      }
    }

    TDocument Document => Content.Document;

    public override IWidget this[int index]
    {
      get
      {
        if (index < base.Count)
        {
          return base[index];
        }
        if (index == base.Count)
        {
          return Caret;
        }
        return RemoteCarets[index - base.Count + 1];
      }
    }

    public void CopySelectedText()
    {
      if (!Caret.HasSelection())
      {
        return;
      }

      var selectedText = Caret.TextForSelection(Document);
      var clipboardService = Screen?.WindowService.Clipboard;
      if (clipboardService != null)
      {
        clipboardService.ClipboardText = selectedText;
      }
    }

    public void CutSelectedText()
    {
      CopySelectedText();
      DeleteSelectedText();
    }

    public void PasteText()
    {
      if (ReadOnly)
      {
        return;
      }

      var pastedText = Screen?.WindowService.Clipboard.ClipboardText;
      if (string.IsNullOrEmpty(pastedText))
      {
        return;
      }

      if (insertBuffer == null)
      {
        insertBuffer = new StringBuilder(pastedText.Length);
      }
      insertBuffer.Clear();
      insertBuffer.Append(pastedText);
      DoInsertFromBuffer(insertBuffer);
    }

    public override void Update(GameTime elapsedTime)
    {
      base.Update(elapsedTime);
      Caret.Update(elapsedTime);
      for (var index = 0; index < RemoteCarets.Count; index++)
      {
        var caret = RemoteCarets[index];
        caret.Update(elapsedTime);
      }
    }

    protected void ArrangeCarets(Rectangle rectangle)
    {
      Caret.Arrange(rectangle);
      for (var index = 0; index < RemoteCarets.Count; index++)
      {
        var caret = RemoteCarets[index];
        caret.Arrange(rectangle);
      }
    }

    protected override Rectangle ArrangeOverride(Rectangle layoutSize)
    {
      var rectangle = base.ArrangeOverride(layoutSize);
      ArrangeCarets(rectangle);
      return rectangle;
    }

    protected void DeleteSelectedText()
    {
      if (!Caret.HasSelection())
      {
        return;
      }
      var start = Math.Min(Caret.SelectionStartOffset, Caret.SelectionEndOffset);
      var end = Math.Max(Caret.SelectionStartOffset, Caret.SelectionEndOffset);
      Document.DeleteAt(start, end - start);
    }

    protected void DoInsertFromBuffer(StringBuilder buffer)
    {
      if (ReadOnly)
      {
        return;
      }

      DeleteSelectedText();
      Document.InsertAt(Caret.SelectionStartOffset, buffer.ToString());
    }

    protected void MeasureCarets(Size availableSize)
    {
      Caret.Measure(availableSize);
      for (var index = 0; index < RemoteCarets.Count; index++)
      {
        var caret = RemoteCarets[index];
        caret.Measure(availableSize);
      }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
      MeasureCarets(availableSize);
      return base.MeasureOverride(availableSize);
    }

    protected void OnKeyTyped(char ch)
    {
      if (ReadOnly)
      {
        return;
      }

      if (IsIgnored(ch))
      {
        return;
      }

      if (insertBuffer == null)
      {
        insertBuffer = new StringBuilder(10);
      }
      insertBuffer.Clear();
      insertBuffer.Append(ch);
      DoInsertFromBuffer(insertBuffer);
    }

    protected virtual bool IsIgnored(char ch)
    {
      if (ch == 0x7f)
      {
        // ASCII delete
        return true;
      }

      if (ch < 0x20)
      {
        if (ch == 0xa || ch == 0xd)
        {
          return false;
        }
        if (ch == '\t')
        {
          return false;
        }
        return true;
      }
      return false;
    }


    protected void SelectAll()
    {
      Caret.SelectAll();
    }

    void ClearSelection(KeyEventArgs args)
    {
      Caret.ClearSelection();
    }

    void InstallKeyMap()
    {
      ActionMap.Register(new KeyStroke(Keys.A, InputFlags.ShortCutKey), args => SelectAll());
      ActionMap.Register(new KeyStroke(Keys.X, InputFlags.ShortCutKey), args => CutSelectedText());
      ActionMap.Register(new KeyStroke(Keys.C, InputFlags.ShortCutKey), args => CopySelectedText());
      ActionMap.Register(new KeyStroke(Keys.V, InputFlags.ShortCutKey), args => PasteText());
      ActionMap.Register(new KeyStroke(Keys.Back), OnBackKey);
      ActionMap.Register(new KeyStroke(Keys.Delete), OnDeleteKey);
      ActionMap.Register(new KeyStroke(Keys.Escape), ClearSelection);
    }

    int Navigate(Direction direction, bool word)
    {
      return Content.Navigate(Caret.SelectionEndOffset, direction);
    }

    void NavigateTo(int offset, bool expandSelection)
    {
      if (expandSelection)
      {
        Caret.Select(offset);
      }
      else
      {
        Caret.MoveTo(offset);
      }
    }

    int NavigateToEndOfLine()
    {
      return Content.Document.TextLength;
    }

    int NavigateToStartOfLine()
    {
      return 0;
    }

    void OnBackKey(KeyEventArgs args)
    {
      if (!ReadOnly)
      {
        if (Caret.HasSelection())
        {
          DeleteSelectedText();
        }
        else if (Caret.SelectionEndOffset > 0)
        {
          Document.DeleteAt(Caret.SelectionEndOffset - 1, 1);
        }
      }
    }

    void OnDeleteKey(KeyEventArgs args)
    {
      if (!ReadOnly)
      {
        if (Caret.HasSelection())
        {
          DeleteSelectedText();
        }
        else if (Caret.SelectionEndOffset < Document.TextLength)
        {
          Document.DeleteAt(Caret.SelectionEndOffset, 1);
        }
      }
    }

    void OnKeyPressed(object source, KeyEventArgs args)
    {
      if (ActionMap.Consume(args))
      {
        args.Consume();
        return;
      }

      var isControlDown = args.Flags.IsControlDown();
      var isShiftDown = args.Flags.IsShiftDown();

      // ReSharper disable once SwitchStatementMissingSomeCases
      switch (args.Key)
      {
        case Keys.Left:
          {
            NavigateTo(Navigate(Direction.Left, isControlDown), isShiftDown);
            args.Consume();
            break;
          }
        case Keys.Right:
          {
            NavigateTo(Navigate(Direction.Right, isControlDown), isShiftDown);
            args.Consume();
            break;
          }
        case Keys.Up:
          {
            NavigateTo(Navigate(Direction.Up, isControlDown), isShiftDown);
            args.Consume();
            break;
          }
        case Keys.Down:
          {
            NavigateTo(Navigate(Direction.Down, isControlDown), isShiftDown);
            args.Consume();
            break;
          }
        case Keys.End:
          {
            NavigateTo(isControlDown ? Content.Document.TextLength : NavigateToEndOfLine(), isShiftDown);
            args.Consume();
            break;
          }
        case Keys.Home:
          {
            NavigateTo(isControlDown ? 0 : NavigateToStartOfLine(), isShiftDown);
            args.Consume();
            break;
          }
        default:
          {
            break;
          }
      }
    }

    void OnKeyTyped(object source, KeyEventArgs args)
    {
      if (args.Consumed)
      {
        return;
      }

      OnKeyTyped(args.Character);
      args.Consume();
    }

    void OnMouseDown(object source, MouseEventArgs args)
    {
      if (args.Button != MouseButton.Left)
      {
        return;
      }

      args.Consume();
      isDragging = true;

      int offset;
      Bias bias;
      if (Content.ViewToModel(args.Position, out offset, out bias))
      {
        NavigateTo(offset, args.Flags.IsShiftDown());
      }
    }

    void OnMouseDragged(object source, MouseEventArgs args)
    {
      if (isDragging)
      {
        args.Consume();

        int offset;
        Bias bias;
        if (Content.ViewToModel(args.Position, out offset, out bias))
        {
          NavigateTo(offset, true);
        }
      }
    }

    void OnMouseUp(object source, MouseEventArgs args)
    {
      if (args.Button != MouseButton.Left)
      {
        return;
      }

      args.Consume();
      isDragging = false;
    }

    void OnRemoteCaretsChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      for (var index = 0; index < e.OldItems.Count; index++)
      {
        var item = (ICaret)e.OldItems[index];
        RaiseChildrenChanged(item, null);
        item.RemoveNotify(this);
      }

      for (var index = 0; index < e.NewItems.Count; index++)
      {
        var item = (ICaret)e.NewItems[index];
        item.AddNotify(this);
        RaiseChildrenChanged(null, item);
      }
    }
  }
}