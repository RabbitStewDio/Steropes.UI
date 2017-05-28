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

using Microsoft.Xna.Framework.Input;

using Steropes.UI.Components;
using Steropes.UI.Input.KeyboardInput;
using Steropes.UI.Input.MouseInput;
using Steropes.UI.Widgets.Container;

namespace Steropes.UI.Widgets
{
  public interface INotebookTab : IWidget
  {
    event EventHandler<EventArgs> ActivationRequested;

    event EventHandler<EventArgs> CloseRequested;

    IWidget Content { get; }

    bool IsActive { get; set; }

    void Close();
  }

  public class NotebookTab : InternalContentWidget<DockPanel>, INotebookTab
  {
    public static readonly string CloseButtonStyleClass = "NotebookTabCloseButton";

    readonly Button closeButton;

    readonly IWidget emptyContent;

    IWidget content;

    IWidget headerContent;

    bool pinned;

    bool isActive;

    public NotebookTab(IUIStyle style, IWidget headerContent, IWidget content) : base(style)
    {
      emptyContent = new Label(UIStyle) { Anchor = AnchoredRect.CreateHorizontallyStretched() };
      
      closeButton = new Button(UIStyle);
      closeButton.AddStyleClass(CloseButtonStyleClass);
      closeButton.Anchor = AnchoredRect.CreateCentered();
      closeButton.ActionPerformed += OnCloseButtonOnClicked;

      InternalContent = new DockPanel(UIStyle);
      InternalContent.LastChildFill = true;
      InternalContent.Add(closeButton, DockPanelConstraint.Right);
      InternalContent.Add(emptyContent, DockPanelConstraint.Left);

      KeyPressed += OnKeyPressed;
      MouseClicked += OnMouseClick;

      Focusable = true;

      HeaderContent = headerContent;
      Content = content;
    }

    public event EventHandler<EventArgs> ActivationRequested;

    public event EventHandler<EventArgs> CloseRequested;

    public IWidget Content
    {
      get
      {
        return content;
      }
      set
      {
        if (ReferenceEquals(content, value))
        {
          return;
        }

        content = value;
        OnPropertyChanged();
      }
    }

    public IWidget HeaderContent
    {
      get
      {
        return headerContent;
      }
      set
      {
        if (ReferenceEquals(headerContent, value))
        {
          return;
        }

        InternalContent.Remove(headerContent ?? emptyContent);
        headerContent = value;
        InternalContent.Add(headerContent ?? emptyContent, DockPanelConstraint.Left);
        OnPropertyChanged();
      }
    }

    public bool IsActive
    {
      get
      {
        return isActive;
      }
      set
      {
        if (value == isActive)
        {
          return;
        }
        isActive = value;
        OnPropertyChanged();
        InvalidateLayout();
      }
    }

    public bool IsClosable
    {
      get
      {
        return !IsPinned;
      }
      set
      {
        IsPinned = !value;
      }
    }

    public bool IsPinned
    {
      get
      {
        return pinned;
      }
      set
      {
        pinned = value;
        closeButton.Visibility = value ? Visibility.Collapsed : Visibility.Visible;
        OnPropertyChanged();
        InvalidateLayout();
      }
    }

    public void Close()
    {
      if (IsClosable)
      {
        CloseRequested?.Invoke(this, EventArgs.Empty);
      }
    }

    void ActivateTab()
    {
      ActivationRequested?.Invoke(this, EventArgs.Empty);
    }

    void OnCloseButtonOnClicked(object sender, EventArgs e)
    {
      Close();
    }

    void OnKeyPressed(object source, KeyEventArgs args)
    {
      if (args.Consumed)
      {
        return;
      }

      switch (args.Key)
      {
        case Keys.Space:
        case Keys.Enter:
          {
            args.Consume();
            ActivateTab();
            break;
          }
        default:
          {
            break;
          }
      }
    }

    void OnMouseClick(object sender, MouseEventArgs args)
    {
      if (args.Button == MouseButton.Left)
      {
        args.Consume();
        ActivateTab();
      }
    }

    /*
    protected override void DrawWidget(IBatchedDrawingService drawingService)
    {
      base.DrawWidget(drawingService);
      drawingService.DrawBox(IsActive ? NotebookStyle.ActiveTab : NotebookStyle.Tab, LayoutRect, Color.White);

      if (Hovered && Screen?.FocusManager?.IsActive == true)
      {
        drawingService.DrawBox(UIStyle.Style.ButtonStyle.HoverOverlay, LayoutRect, Color.White);
      }

      if (Screen?.FocusManager?.IsActive == true && Focused)
      {
        drawingService.DrawBox(IsActive ? NotebookStyle.ActiveTabFocus : NotebookStyle.TabFocus, LayoutRect, Color.White);
      }
    }
    */
  }
}