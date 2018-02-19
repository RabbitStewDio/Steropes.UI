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
using Steropes.UI.Util;
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

  public interface INotebookTab<out TWidget, out THeaderWidget> : INotebookTab where TWidget : IWidget
                                                                               where THeaderWidget : IWidget
  {
    new TWidget Content { get; }
    THeaderWidget HeaderContent { get; }
  }

  public class NotebookTab<TWidget, THeaderWidget> : InternalContentWidget<DockPanel>,
                                                     INotebookTab<TWidget, THeaderWidget> where TWidget : IWidget
                                                                                          where THeaderWidget : class,
                                                                                          IWidget
  {
    public static readonly string CloseButtonStyleClass = "NotebookTabCloseButton";

    readonly EventSupport<EventArgs> activationRequestedSupport;
    readonly EventSupport<EventArgs> closeRequestedSupport;
    readonly Button closeButton;
    readonly IWidget emptyContent;

    TWidget content;
    THeaderWidget headerContent;
    bool pinned;
    bool isActive;

    public NotebookTab(IUIStyle style) : base(style)
    {
      activationRequestedSupport = new EventSupport<EventArgs>();
      closeRequestedSupport = new EventSupport<EventArgs>();

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
    }

    public NotebookTab(IUIStyle style, THeaderWidget headerContent, TWidget content) : this(style)
    {
      HeaderContent = headerContent;
      Content = content;
    }

    public event EventHandler<EventArgs> ActivationRequested
    {
      add { activationRequestedSupport.Event += value; }
      remove { activationRequestedSupport.Event -= value; }
    }

    public EventHandler<EventArgs> OnActivationRequested
    {
      get { return activationRequestedSupport.Handler; }
      set { activationRequestedSupport.Handler = value; }
    }

    public event EventHandler<EventArgs> CloseRequested
    {
      add { closeRequestedSupport.Event += value; }
      remove { closeRequestedSupport.Event -= value; }
    }

    public EventHandler<EventArgs> OnCloseRequested
    {
      get { return closeRequestedSupport.Handler; }
      set { closeRequestedSupport.Handler = value; }
    }

    public TWidget Content
    {
      get { return content; }
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

    public THeaderWidget HeaderContent
    {
      get { return headerContent; }
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

    IWidget INotebookTab.Content
    {
      get { return Content; }
    }

    public bool IsActive
    {
      get { return isActive; }
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
      get { return !IsPinned; }
      set { IsPinned = !value; }
    }

    public bool IsPinned
    {
      get { return pinned; }
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
        closeRequestedSupport.Raise(this, EventArgs.Empty);
      }
    }

    void ActivateTab()
    {
      activationRequestedSupport.Raise(this, EventArgs.Empty);
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
  }

  public class NotebookTab : NotebookTab<IWidget, IWidget>
  {
    public NotebookTab(IUIStyle style) : base(style)
    {
    }

    public NotebookTab(IUIStyle style, IWidget headerContent, IWidget content) : base(style, headerContent, content)
    {
    }
  }
}