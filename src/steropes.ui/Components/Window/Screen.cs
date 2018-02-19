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
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Steropes.UI.Components.Window.Events;
using Steropes.UI.Input;
using Steropes.UI.Platform;
using Steropes.UI.Styles;
using Steropes.UI.Widgets.Container;

namespace Steropes.UI.Components.Window
{
  /// <summary>
  ///   A Screen handles passing on user input, updating and drawing a bunch of widgets
  /// </summary>
  public class Screen : IScreenService
  {
    readonly Queue<Action> actions;

    readonly ScreenEventHandling eventHandler;

    int ignoreClickFrames;

    Rectangle lastLayoutSize;
    IBatchedDrawingService drawingService;

    public Screen(IInputManager rawInputs, 
                  IUIStyle style, 
                  IBatchedDrawingService drawingService, 
                  IGameWindowService windowService)
    {
      actions = new Queue<Action>();

      DrawingService = drawingService ?? throw new ArgumentNullException(nameof(drawingService));
      WindowService = windowService ?? throw new ArgumentNullException(nameof(windowService));
      FocusManager = new ScreenFocusManager(this);
      MouseHoverManager = new MouseHoverManager();
      Root = new RootPane(this, style);
      style.StyleResolver.AddRoot(Root);

      PopUpManager = new PopupManager(Root);

      eventHandler = new ScreenEventHandling(this, Root, rawInputs);
      eventHandler.AddMousePostProcessor(new PopupClosingEventProcessor(Root));
    }

    public IFocusManager FocusManager { get; }

    public bool IsActive { get; set; }

    public IMouseHoverManager MouseHoverManager { get; }

    public IPopupManager PopUpManager { get; }

    public RootPane Root { get; }

    public IStyleResolver StyleResolver => Root.UIStyle.StyleResolver;

    public GameTime Time { get; private set; }

    public IGameWindowService WindowService { get; }

    Rectangle Bounds => DrawingService.GraphicsDevice.Viewport.TitleSafeArea;

    public IBatchedDrawingService DrawingService
    {
      get { return drawingService; }
      set
      {
        drawingService = value ?? throw new ArgumentNullException(nameof(value));
      }
    }

    public void Draw()
    {
      // re-layout. This is a no-op if there had been no changes since the last
      // arrange call, but it avoids subtle bugs when UI components get updated
      // during an earlier operation in another component's Draw call.
      ArrangeRoot();
      DrawingService.StartDrawing();
      try
      {
        Root.Draw(DrawingService);
        Root.DrawOverlay(DrawingService);
      }
      finally
      {
        DrawingService.EndDrawing();
      }
    }

    public void InvokeAfterEvents(Action action)
    {
      if (action == null)
      {
        throw new ArgumentNullException(nameof(action));
      }
      actions.Enqueue(action);
    }

    public void Update(GameTime elapsedTime)
    {
      Time = elapsedTime;

      if (ignoreClickFrames > 0)
      {
        ignoreClickFrames -= 1;
      }
      else
      {
        eventHandler.ProcessEvents(elapsedTime);
      }

      while (actions.Count > 0)
      {
        try
        {
          actions.Dequeue().Invoke();
        }
        catch (Exception)
        {
          // log, then ignore ..
        }
      }

      ArrangeRoot();
      Root.Update(elapsedTime);
    }

    void ArrangeRoot()
    {
      StyleResolver.Revalidate();
      if (StyleResolver.StyleRules.Count == 0)
      {
        throw new InvalidOperationException("The style system contains no style-rules. This will result in errors. Aborting!");
      }

      var screenSpace = Bounds;
      if (lastLayoutSize != screenSpace || Root.LayoutInvalid)
      {
        Root.InvalidateLayout();
        Root.Arrange(screenSpace);
        lastLayoutSize = screenSpace;
      }
    }

    class PopupManager : IPopupManager
    {
      readonly RootPane rootPane;

      public PopupManager(RootPane rootPane)
      {
        this.rootPane = rootPane;
      }

      public IPopUp<TContent> CreatePopup<TContent>(Point location, TContent content = null) where TContent : class, IWidget
      {
        var popUp = new PopUp<TContent>(rootPane.UIStyle) { Anchor = AnchoredRect.CreateTopLeftAnchored(location.X, location.Y), Content = content };

        popUp.Closed += HandlePopUpClosed;
        rootPane.Screen.InvokeAfterEvents(() => { rootPane.AddPopUp(popUp); });
        return popUp;
      }

      public IWindow<TContent> CreateWindow<TContent>(Point location, TContent content = default(TContent)) where TContent : class, IWidget
      {
        throw new NotImplementedException();
      }

      void HandlePopUpClosed(object sender, EventArgs e)
      {
        if (sender is IPopUp popUp)
        {
          rootPane.Remove(popUp);
        }
      }
    }

    class ScreenFocusManager : IFocusManager
    {
      readonly Screen screen;

      IWidget focusedWidget;

      public ScreenFocusManager(Screen screen)
      {
        this.screen = screen;
      }

      public IWidget FocusedWidget
      {
        get
        {
          return focusedWidget;
        }
        set
        {
          if (ReferenceEquals(focusedWidget, value))
          {
            return;
          }

          if (focusedWidget != null)
          {
            focusedWidget.Focused = false;
          }
          focusedWidget = value;
          if (focusedWidget != null)
          {
            focusedWidget.Focused = true;
          }
        }
      }

      public bool IsActive => screen.IsActive;

      public void OnWidgetRemoved(IWidget widgetRaw)
      {
        var widget = widgetRaw;
        while (widget != null && FocusedWidget != null)
        {
          if (ReferenceEquals(widget, FocusedWidget))
          {
            FocusedWidget = null;
            return;
          }
          widget = widget.Parent;
        }

        var focusParent = FocusedWidget?.Parent;
        while (focusParent != null)
        {
          if (ReferenceEquals(focusParent, widget))
          {
            FocusedWidget = null;
            return;
          }
          focusParent = focusParent.Parent;
        }
      }
    }
  }
}