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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Steropes.UI.Animation;
using Steropes.UI.Components;
using Steropes.UI.Input.KeyboardInput;
using Steropes.UI.Input.MouseInput;
using Steropes.UI.Platform;
using Steropes.UI.Styles;
using Steropes.UI.Util;
using Steropes.UI.Widgets.Styles;

namespace Steropes.UI.Widgets
{
  public static class ButtonPseudoClasses
  {
    public static readonly string DownPseudoClass = "down";
  }

  public enum SelectionState
  {
    Indeterminate = 0,

    Unselected = 1,

    Selected = 2
  }

  public class ButtonBase<TContent> : ContentWidget<TContent>
    where TContent : class, IWidget
  {
    readonly EventSupport<EventArgs> actionPerformedSupport;
    readonly ButtonStyleDefinition buttonStyle;
    readonly AnimatedValue pressedAnimation;
    readonly EventSupport<EventArgs> selectionChangedSupport;
    bool isPressed;
    SelectionState selected;

    public ButtonBase(IUIStyle style) : base(style)
    {
      buttonStyle = StyleSystem.StylesFor<ButtonStyleDefinition>();
      actionPerformedSupport = new EventSupport<EventArgs>();
      selectionChangedSupport = new EventSupport<EventArgs>();

      pressedAnimation = new SmoothValue(1f, 0f, 0.1f);
      pressedAnimation.FinishAnimation();

      Focusable = true;

      FocusedChanged += (sender, args) => ResetPressState();

      MouseDown += OnMouseDown;
      MouseUp += OnMouseUp;
      MouseClicked += OnMouseClick;
      KeyPressed += OnKeyPressed;
      KeyReleased += OnKeyReleased;
    }

    public Color DownOverlayColor
    {
      get { return Style.GetValue(buttonStyle.DownOverlayColor); }
      set { Style.SetValue(buttonStyle.DownOverlayColor, value); }
    }

    public IBoxTexture DownOverlayTexture
    {
      get { return Style.GetValue(buttonStyle.DownOverlay); }
      set { Style.SetValue(buttonStyle.DownOverlay, value); }
    }

    public EventHandler<EventArgs> OnActionPerformed
    {
      get { return actionPerformedSupport.Handler; }
      set { actionPerformedSupport.Handler = value; }
    }

    public EventHandler<EventArgs> OnSelectionChanged
    {
      get { return selectionChangedSupport.Handler; }
      set { selectionChangedSupport.Handler = value; }
    }

    public bool IsPressed
    {
      get { return isPressed; }
      set
      {
        if (value == isPressed)
        {
          return;
        }

        isPressed = value;
        if (value)
        {
          AddPseudoStyleClass(ButtonPseudoClasses.DownPseudoClass);
        }
        else
        {
          RemovePseudoStyleClass(ButtonPseudoClasses.DownPseudoClass);
        }

        OnPropertyChanged();
      }
    }

    public SelectionState Selected
    {
      get { return selected; }
      set
      {
        if (value == selected)
        {
          return;
        }

        selected = value;
        OnPropertyChanged();
        selectionChangedSupport.Raise(this, EventArgs.Empty);
      }
    }

    protected float PressedAnimationValue
    {
      get { return pressedAnimation.CurrentValue; }
    }

    public event EventHandler<EventArgs> ActionPerformed
    {
      add { actionPerformedSupport.Event += value; }
      remove { actionPerformedSupport.Event -= value; }
    }

    public event EventHandler<EventArgs> SelectionChanged
    {
      add { selectionChangedSupport.Event += value; }
      remove { selectionChangedSupport.Event -= value; }
    }

    public virtual void Click()
    {
      actionPerformedSupport.Raise(this, new EventArgs());
    }

    public override void Update(GameTime elapsedTime)
    {
      base.Update(elapsedTime);
      pressedAnimation.Update(elapsedTime);
    }

    protected internal void ResetPressState()
    {
      pressedAnimation.FinishAnimation();
      IsPressed = false;
    }

    protected override void DrawKeyboardFocus(IBatchedDrawingService drawingService)
    {
      if (!IsPressed)
      {
        base.DrawKeyboardFocus(drawingService);
      }
    }

    protected override void DrawMouseState(IBatchedDrawingService drawingService)
    {
      if (!IsPressed)
      {
        base.DrawMouseState(drawingService);
      }
      else
      {
        drawingService.DrawBox(DownOverlayTexture, BorderRect, DownOverlayColor * PressedAnimationValue);
      }
    }

    void OnActivateDown()
    {
      IsPressed = true;
      pressedAnimation.Direction = AnimationDirection.Forward;
      pressedAnimation.StartAnimation();
    }

    void OnActivateUp()
    {
      IsPressed = false;
      pressedAnimation.Direction = AnimationDirection.Backward;
      pressedAnimation.StartAnimation();
    }

    void OnKeyPressed(object source, KeyEventArgs args)
    {
      switch (args.Key)
      {
        case Keys.Space:
        case Keys.Enter:
        {
          args.Consume();
          OnActivateDown();
          break;
        }
      }
    }

    void OnKeyReleased(object source, KeyEventArgs args)
    {
      switch (args.Key)
      {
        case Keys.Space:
        case Keys.Enter:
        {
          args.Consume();
          Click();
          OnActivateUp();
          break;
        }
      }
    }

    void OnMouseClick(object source, MouseEventArgs args)
    {
      Click();
      args.Consume();
    }

    void OnMouseDown(object source, MouseEventArgs args)
    {
      if (args.Button != MouseButton.Left)
      {
        return;
      }

      args.Consume();
      OnActivateDown();
    }

    void OnMouseUp(object source, MouseEventArgs args)
    {
      if (args.Button != MouseButton.Left)
      {
        return;
      }

      args.Consume();
      OnActivateUp();
    }
  }
}