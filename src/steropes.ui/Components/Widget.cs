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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Steropes.UI.Components.Window;
using Steropes.UI.Input;
using Steropes.UI.Input.GamePadInput;
using Steropes.UI.Input.KeyboardInput;
using Steropes.UI.Input.MouseInput;
using Steropes.UI.Input.TouchInput;
using Steropes.UI.Platform;
using Steropes.UI.Styles;
using Steropes.UI.Util;
using Steropes.UI.Widgets;
using Steropes.UI.Widgets.Container;
using Steropes.UI.Widgets.Styles;

namespace Steropes.UI.Components
{
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
  [SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
  [SuppressMessage("ReSharper", "UnusedMember.Global")]
  [SuppressMessage("ReSharper", "EventNeverSubscribedTo.Global")]
  [SuppressMessage("ReSharper", "UnusedMemberInSuper.Global")]
  public interface IWidget : IVisualContent, IEnumerable<IWidget>, IStyledObject
  {
    event EventHandler<ContainerEventArgs> ChildrenChanged;

    event EventHandler<EventArgs> FocusedChanged;

    event EventHandler<GamePadEventArgs> GamePadButtonDown;

    event EventHandler<GamePadEventArgs> GamePadButtonUp;

    event EventHandler<GamePadEventArgs> GamePadThumbStickMoved;

    event EventHandler<GamePadEventArgs> GamePadTriggerMoved;

    event EventHandler<EventArgs> HoveredChanged;

    event EventHandler<KeyEventArgs> KeyPressed;

    event EventHandler<KeyEventArgs> KeyReleased;

    event EventHandler<KeyEventArgs> KeyRepeated;

    event EventHandler<KeyEventArgs> KeyTyped;

    event EventHandler<EventArgs> LayoutInvalidated;

    event EventHandler<MouseEventArgs> MouseClicked;

    event EventHandler<MouseEventArgs> MouseDown;

    event EventHandler<MouseEventArgs> MouseDragged;

    event EventHandler<MouseEventArgs> MouseEntered;

    event EventHandler<MouseEventArgs> MouseExited;

    event EventHandler<MouseEventArgs> MouseMoved;

    event EventHandler<MouseEventArgs> MouseUp;

    event EventHandler<MouseEventArgs> MouseWheel;

    event EventHandler<EventArgs> ParentChanged;

    event EventHandler<TouchEventArgs> TouchCancelled;

    event EventHandler<TouchEventArgs> TouchGestured;

    event EventHandler<TouchEventArgs> TouchMoved;

    event EventHandler<TouchEventArgs> TouchPressed;

    event EventHandler<TouchEventArgs> TouchReleased;

    AnchoredRect Anchor { get; }

    Rectangle BorderRect { get; }

    int Count { get; }

    MouseCursor Cursor { get; set; }

    /// <summary>
    ///   The enabled state is a switch that enables or disables event processin on Widgets. A widget that is disabled
    ///   cannot receive the focus and will not process any events. It will still update to allow Animations to play.
    ///   <p />
    ///   Disabled Widgets are usually helper widgets that cannot receive any events from the event dispatcher. They (and their
    ///   children) will be ignored for focus management and hit-testing.
    /// </summary>
    bool Enabled { get; set; }

    bool Focusable { get; }

    bool Focused { get; set; }

    bool Hovered { get; set; }

    bool LayoutInvalid { get; }

    IWidget Parent { get; }

    /// <summary>
    ///   A container for receiving resolved style properties.
    ///   Public as implementation side effect, dont manipulate it unless you are writing a style resolver.
    /// </summary>
    IResolvedStyle ResolvedStyle { get; }

    IScreenService Screen { get; }

    /// <summary>
    ///   Controls whether this widget is visible. Invisible widgets are treated as disabled, they will not receive events,
    ///   will not take part in hit-testing and will not be able to receive the keyboard focus.
    /// </summary>
    Visibility Visibility { get; set; }

    IWidget this[int index] { get; }

    void AddNotify(IWidget parent);

    void DispatchEvent(KeyEventArgs eventData);

    void DispatchEvent(MouseEventArgs eventData);

    void DispatchEvent(TouchEventArgs eventData);

    void DispatchEvent(GamePadEventArgs eventData);

    int GetDrawOrderForChild(IWidget w);

    IWidget GetFirstFocusableDescendant(Direction direction);

    IWidget GetSibling(Direction direction, IWidget sourceWidget);

    /// <summary>
    ///   Triggers a re-evaluation of styles. Use the boolean to control the scope. Use a full revalidation if the widget's
    ///   properties have changed.
    /// </summary>
    /// <param name="full"></param>
    void InvalidateStyle(bool full);

    // get rid of this ..
    IWidget PerformHitTest(Point mouseHitPoint);

    void RemoveNotify(IWidget parent);

    void ValidateStyle();

    /// <summary>
    ///  Style resolver helper method. Iterate the structural tree of this widget and visit all nodes 
    ///  that have this widget as direct parent. Include both standard children and tooltips and other
    ///  nodes that may participate in style resolving, measurements or arrange operations.
    /// </summary>
    /// <param name="action"></param>
    void VisitStructuralChildren(Action<IWidget> action);
  }

  public static class WidgetPseudoClasses
  {
    public static readonly string FirstChildPseudoClass = "first";

    public static readonly string FocusedPseudoClass = "focused";

    public static readonly string HoveredPseudoClass = "hovered";

    public static readonly string LastChildPseudoClass = "last";
  }

  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
  [SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
  [SuppressMessage("ReSharper", "UnusedMember.Global")]
  public class Widget : VisualContent, IWidget
  {
    protected readonly WidgetStyleDefinition WidgetStyle;

    readonly EventSupport<ContainerEventArgs> childrenChangedSupport;

    readonly EventSupport<EventArgs> focusedChangedSupport;

    readonly EventSupport<GamePadEventArgs> gamePadButtonDownSupport;

    readonly EventSupport<GamePadEventArgs> gamePadButtonUpSupport;

    readonly EventSupport<GamePadEventArgs> gamePadThumbStickMovedSupport;

    readonly EventSupport<GamePadEventArgs> gamePadTriggerMovedSupport;

    readonly EventSupport<EventArgs> hoveredChangedSupport;

    readonly EventSupport<KeyEventArgs> keyPressedSupport;

    readonly EventSupport<KeyEventArgs> keyReleasedSupport;

    readonly EventSupport<KeyEventArgs> keyRepeatedSupport;

    readonly EventSupport<KeyEventArgs> keyTypedSupport;

    readonly EventSupport<MouseEventArgs> mouseClickedSupport;

    readonly EventSupport<MouseEventArgs> mouseDownSupport;

    readonly EventSupport<MouseEventArgs> mouseDraggedSupport;

    readonly EventSupport<MouseEventArgs> mouseEnteredSupport;

    readonly EventSupport<MouseEventArgs> mouseExitedSupport;

    readonly EventSupport<MouseEventArgs> mouseMovedSupport;

    readonly EventSupport<MouseEventArgs> mouseUpSupport;

    readonly EventSupport<MouseEventArgs> mouseWheelSupport;

    readonly HashStyleClassSet pseudoStyleClasses;

    readonly HashStyleClassSet styleClasses;

    readonly EventSupport<EventArgs> styleInvalidatedSupport;

    readonly EventSupport<TouchEventArgs> touchCancelledSupport;

    readonly EventSupport<TouchEventArgs> touchGesturedSupport;

    readonly EventSupport<TouchEventArgs> touchMovedSupport;

    readonly EventSupport<TouchEventArgs> touchPressedSupport;

    readonly EventSupport<TouchEventArgs> touchReleasedSupport;

    AnchoredRect anchor;

    Stack<EventArgs> currentEvents;

    MouseCursor cursor;

    bool enabled;

    bool focused;

    bool hovered;

    string nodeType;

    IWidget parent;

    string styleId;

    IToolTip tooltip;

    public Widget(IUIStyle style)
    {
      Enabled = true;

      focusedChangedSupport = new EventSupport<EventArgs>();
      hoveredChangedSupport = new EventSupport<EventArgs>();
      keyPressedSupport = new EventSupport<KeyEventArgs>();
      keyReleasedSupport = new EventSupport<KeyEventArgs>();
      keyRepeatedSupport = new EventSupport<KeyEventArgs>();
      keyTypedSupport = new EventSupport<KeyEventArgs>();
      mouseClickedSupport = new EventSupport<MouseEventArgs>();
      mouseDownSupport = new EventSupport<MouseEventArgs>();
      mouseDraggedSupport = new EventSupport<MouseEventArgs>();
      mouseEnteredSupport = new EventSupport<MouseEventArgs>();
      mouseExitedSupport = new EventSupport<MouseEventArgs>();
      mouseMovedSupport = new EventSupport<MouseEventArgs>();
      mouseUpSupport = new EventSupport<MouseEventArgs>();
      mouseWheelSupport = new EventSupport<MouseEventArgs>();
      touchCancelledSupport = new EventSupport<TouchEventArgs>();
      touchGesturedSupport = new EventSupport<TouchEventArgs>();
      touchMovedSupport = new EventSupport<TouchEventArgs>();
      touchPressedSupport = new EventSupport<TouchEventArgs>();
      touchReleasedSupport = new EventSupport<TouchEventArgs>();
      gamePadButtonDownSupport = new EventSupport<GamePadEventArgs>();
      gamePadButtonUpSupport = new EventSupport<GamePadEventArgs>();
      gamePadTriggerMovedSupport = new EventSupport<GamePadEventArgs>();
      gamePadThumbStickMovedSupport = new EventSupport<GamePadEventArgs>();
      childrenChangedSupport = new EventSupport<ContainerEventArgs>();
      styleInvalidatedSupport = new EventSupport<EventArgs>();

      UIStyle = style;
      StyleSystem = style.StyleSystem;
      WidgetStyle = StyleSystem.StylesFor<WidgetStyleDefinition>();

      Anchor = AnchoredRect.Full;

      pseudoStyleClasses = new HashStyleClassSet();
      styleClasses = new HashStyleClassSet();

      ResolvedStyle = UIStyle.StyleResolver.CreateStyleFor(this);

      GamePadButtonDown += OnGamePadButtonDown;
      KeyPressed += OnKeyPressed;
      MouseMoved += OnMouseMove;
    }

    public event EventHandler<EventArgs> StyleInvalidated
    {
      add
      {
        styleInvalidatedSupport.Event += value;
      }
      remove
      {
        styleInvalidatedSupport.Event -= value;
      }
    }

    public event EventHandler<ContainerEventArgs> ChildrenChanged
    {
      add
      {
        childrenChangedSupport.Event += value;
      }
      remove
      {
        childrenChangedSupport.Event -= value;
      }
    }

    public event EventHandler<EventArgs> FocusedChanged
    {
      add
      {
        focusedChangedSupport.Event += value;
      }
      remove
      {
        focusedChangedSupport.Event -= value;
      }
    }

    public event EventHandler<GamePadEventArgs> GamePadButtonDown
    {
      add
      {
        gamePadButtonDownSupport.Event += value;
      }
      remove
      {
        gamePadButtonDownSupport.Event -= value;
      }
    }

    public event EventHandler<GamePadEventArgs> GamePadButtonUp
    {
      add
      {
        gamePadButtonUpSupport.Event += value;
      }
      remove
      {
        gamePadButtonUpSupport.Event -= value;
      }
    }

    public event EventHandler<GamePadEventArgs> GamePadThumbStickMoved
    {
      add
      {
        gamePadThumbStickMovedSupport.Event += value;
      }
      remove
      {
        gamePadThumbStickMovedSupport.Event -= value;
      }
    }

    public event EventHandler<GamePadEventArgs> GamePadTriggerMoved
    {
      add
      {
        gamePadTriggerMovedSupport.Event += value;
      }
      remove
      {
        gamePadTriggerMovedSupport.Event -= value;
      }
    }

    public event EventHandler<EventArgs> HoveredChanged
    {
      add
      {
        hoveredChangedSupport.Event += value;
      }
      remove
      {
        hoveredChangedSupport.Event -= value;
      }
    }

    public event EventHandler<KeyEventArgs> KeyPressed
    {
      add
      {
        keyPressedSupport.Event += value;
      }
      remove
      {
        keyPressedSupport.Event -= value;
      }
    }

    public event EventHandler<KeyEventArgs> KeyReleased
    {
      add
      {
        keyReleasedSupport.Event += value;
      }
      remove
      {
        keyReleasedSupport.Event -= value;
      }
    }

    public event EventHandler<KeyEventArgs> KeyRepeated
    {
      add
      {
        keyRepeatedSupport.Event += value;
      }
      remove
      {
        keyRepeatedSupport.Event -= value;
      }
    }

    public event EventHandler<KeyEventArgs> KeyTyped
    {
      add
      {
        keyTypedSupport.Event += value;
      }
      remove
      {
        keyTypedSupport.Event -= value;
      }
    }

    public event EventHandler<MouseEventArgs> MouseClicked
    {
      add
      {
        mouseClickedSupport.Event += value;
      }
      remove
      {
        mouseClickedSupport.Event -= value;
      }
    }

    public event EventHandler<MouseEventArgs> MouseDown
    {
      add
      {
        mouseDownSupport.Event += value;
      }
      remove
      {
        mouseDownSupport.Event -= value;
      }
    }

    public event EventHandler<MouseEventArgs> MouseDragged
    {
      add
      {
        mouseDraggedSupport.Event += value;
      }
      remove
      {
        mouseDraggedSupport.Event -= value;
      }
    }

    public event EventHandler<MouseEventArgs> MouseEntered
    {
      add
      {
        mouseEnteredSupport.Event += value;
      }
      remove
      {
        mouseEnteredSupport.Event -= value;
      }
    }

    public event EventHandler<MouseEventArgs> MouseExited
    {
      add
      {
        mouseExitedSupport.Event += value;
      }
      remove
      {
        mouseExitedSupport.Event -= value;
      }
    }

    public event EventHandler<MouseEventArgs> MouseMoved
    {
      add
      {
        mouseMovedSupport.Event += value;
      }
      remove
      {
        mouseMovedSupport.Event -= value;
      }
    }

    public event EventHandler<MouseEventArgs> MouseUp
    {
      add
      {
        mouseUpSupport.Event += value;
      }
      remove
      {
        mouseUpSupport.Event -= value;
      }
    }

    public event EventHandler<MouseEventArgs> MouseWheel
    {
      add
      {
        mouseWheelSupport.Event += value;
      }
      remove
      {
        mouseWheelSupport.Event -= value;
      }
    }

    public event EventHandler<TouchEventArgs> TouchCancelled
    {
      add
      {
        touchCancelledSupport.Event += value;
      }
      remove
      {
        touchCancelledSupport.Event -= value;
      }
    }

    public event EventHandler<TouchEventArgs> TouchGestured
    {
      add
      {
        touchGesturedSupport.Event += value;
      }
      remove
      {
        touchGesturedSupport.Event -= value;
      }
    }

    public event EventHandler<TouchEventArgs> TouchMoved
    {
      add
      {
        touchMovedSupport.Event += value;
      }
      remove
      {
        touchMovedSupport.Event -= value;
      }
    }

    public event EventHandler<TouchEventArgs> TouchPressed
    {
      add
      {
        touchPressedSupport.Event += value;
      }
      remove
      {
        touchPressedSupport.Event -= value;
      }
    }

    public event EventHandler<TouchEventArgs> TouchReleased
    {
      add
      {
        touchReleasedSupport.Event += value;
      }
      remove
      {
        touchReleasedSupport.Event -= value;
      }
    }

    public AnchoredRect Anchor
    {
      get
      {
        return anchor;
      }
      set
      {
        if (value.Equals(anchor))
        {
          return;
        }

        anchor = value;
        OnPropertyChanged();
        InvalidateLayout();
      }
    }

    public Color Color
    {
      get
      {
        return Style.GetValue(WidgetStyle.Color, Color.White);
      }
      set
      {
        Style.SetValue(WidgetStyle.Color, value);
      }
    }

    public virtual int Count => 0;

    public MouseCursor Cursor
    {
      get
      {
        return cursor;
      }
      set
      {
        if (cursor == value)
        {
          return;
        }
        cursor = value;
        OnPropertyChanged();
        if (Hovered)
        {
          UpdateWindowCursor();
        }
      }
    }

    public bool DebugLayout { get; set; }

    public bool Enabled
    {
      get
      {
        return enabled;
      }
      set
      {
        enabled = value;
        OnPropertyChanged();
      }
    }

    public bool Focusable { get; set; }

    public bool Focused
    {
      get
      {
        return focused;
      }
      set
      {
        if (focused == value)
        {
          return;
        }
        
        focused = value;
        OnPropertyChanged();
        if (value)
        {
          AddPseudoStyleClass(WidgetPseudoClasses.FocusedPseudoClass);
        }
        else
        {
          RemovePseudoStyleClass(WidgetPseudoClasses.FocusedPseudoClass);
        }
        focusedChangedSupport.Raise(this, EventArgs.Empty);
      }
    }

    public Color FocusedOverlayColor
    {
      get
      {
        return Style.GetValue(WidgetStyle.FocusedOverlayColor, Color);
      }
      set
      {
        Style.SetValue(WidgetStyle.FocusedOverlayColor, value);
      }
    }

    public IBoxTexture FocusedOverlayTexture
    {
      get
      {
        return Style.GetValue(WidgetStyle.FocusedOverlayTexture);
      }
      set
      {
        Style.SetValue(WidgetStyle.FocusedOverlayTexture, value);
      }
    }

    public Color FrameOverlayColor
    {
      get
      {
        return Style.GetValue(WidgetStyle.FrameOverlayColor, Color);
      }
      set
      {
        Style.SetValue(WidgetStyle.FrameOverlayColor, value);
      }
    }

    public IBoxTexture FrameOverlayTexture
    {
      get
      {
        return Style.GetValue(WidgetStyle.FrameOverlayTexture);
      }
      set
      {
        Style.SetValue(WidgetStyle.FrameOverlayTexture, value);
      }
    }

    public bool Hovered
    {
      get
      {
        return hovered;
      }
      set
      {
        if (value == hovered)
        {
          return;
        }

        hovered = value;
        OnPropertyChanged();
        if (value)
        {
          AddPseudoStyleClass(WidgetPseudoClasses.HoveredPseudoClass);
        }
        else
        {
          RemovePseudoStyleClass(WidgetPseudoClasses.HoveredPseudoClass);
        }
        hoveredChangedSupport.Raise(this, EventArgs.Empty);
        if (value)
        {
          UpdateWindowCursor();
        }
      }
    }

    public Color HoverOverlayColor
    {
      get
      {
        return Style.GetValue(WidgetStyle.HoverOverlayColor, Color);
      }
      set
      {
        Style.SetValue(WidgetStyle.HoverOverlayColor, value);
      }
    }

    public IBoxTexture HoverOverlayTexture
    {
      get
      {
        return Style.GetValue(WidgetStyle.HoverOverlayTexture);
      }
      set
      {
        Style.SetValue(WidgetStyle.HoverOverlayTexture, value);
      }
    }

    public sealed override Insets Margin
    {
      get
      {
        return Style.GetValue(WidgetStyle.Margin);
      }
      set
      {
        Style.SetValue(WidgetStyle.Margin, value);
      }
    }

    public virtual string NodeType
    {
      get
      {
        if (nodeType != null)
        {
          return nodeType;
        }
        var t = GetType();
        if (t.IsGenericType)
        {
          var n = GetType().Name;
          var idx = n.IndexOf('`');
          if (idx != -1)
          {
            nodeType = n.Substring(0, idx);
            return nodeType;
          }
        }
        nodeType = t.Name;
        return nodeType;
      }
    }

    public sealed override Insets Padding
    {
      get
      {
        return Style.GetValue(WidgetStyle.Padding);
      }
      set
      {
        Style.SetValue(WidgetStyle.Padding, value);
      }
    }

    // public override IVisualContent ParentVisual => Parent;
    public IWidget Parent
    {
      get
      {
        return parent;
      }
      protected set
      {
        if (Equals(value, parent))
        {
          return;
        }

        if (value == null)
        {
          Screen?.FocusManager?.OnWidgetRemoved(this);
        }
        parent = value;

        OnPropertyChanged();
        OnPropertyChanged(nameof(Screen));
        RaiseParentChangedEvent();
      }
    }

    public IStyleClassSet PseudoClasses => pseudoStyleClasses;

    public IResolvedStyle ResolvedStyle { get; }

    public virtual IScreenService Screen => Parent?.Screen;

    public IStyle Style => ResolvedStyle;

    public IStyleClassSet StyleClasses => styleClasses;

    public string StyleId
    {
      get
      {
        return styleId;
      }
      set
      {
        if (value == styleId)
        {
          return;
        }

        styleId = value;
        OnPropertyChanged();
        InvalidateStyle(true);
        InvalidateLayout();
      }
    }

    public IStyleSystem StyleSystem { get; }

    public object Tag { get; set; }

    public IToolTip Tooltip
    {
      get
      {
        return tooltip;
      }
      set
      {
        if (ReferenceEquals(value, tooltip))
        {
          return;
        }
        var old = tooltip;
        tooltip?.RemoveNotify(this);
        tooltip = value;
        tooltip?.AddNotify(this);
        RaiseChildrenChanged(-1, old, null, tooltip, null);
        OnPropertyChanged();
      }
    }

    public IUIStyle UIStyle { get; }

    public Visibility Visibility
    {
      get
      {
        return Style.GetValue(WidgetStyle.Visibility);
      }
      set
      {
        Style.SetValue(WidgetStyle.Visibility, value);
        OnPropertyChanged();
        InvalidateLayout();
      }
    }

    public Color WidgetStateOverlayColor
    {
      get
      {
        return Style.GetValue(WidgetStyle.WidgetStateOverlayColor, Color);
      }
      set
      {
        Style.SetValue(WidgetStyle.WidgetStateOverlayColor, value);
      }
    }

    public IBoxTexture WidgetStateOverlayTexture
    {
      get
      {
        return Style.GetValue(WidgetStyle.WidgetStateOverlay);
      }
      set
      {
        Style.SetValue(WidgetStyle.WidgetStateOverlay, value);
      }
    }

    public bool WidgetStateOverlayTextureScale
    {
      get
      {
        return Style.GetValue(WidgetStyle.WidgetStateOverlayScale, true);
      }
      set
      {
        Style.SetValue(WidgetStyle.WidgetStateOverlayScale, value);
      }
    }

    protected IBoxTexture FrameTexture
    {
      get
      {
        return Style.GetValue(WidgetStyle.FrameTexture);
      }
      set
      {
        Style.SetValue(WidgetStyle.FrameTexture, value);
      }
    }

    public virtual IWidget this[int index]
    {
      get
      {
        throw new IndexOutOfRangeException();
      }
    }

    [SuppressMessage("ReSharper", "ParameterHidesMember")]
    public virtual void AddNotify(IWidget parent)
    {
      if (Parent != null)
      {
        throw new InvalidOperationException();
      }
      Parent = parent;
    }

    /// <summary>
    ///  A DSL helper to allow a more fluent assembly of GUIs similar to the ScalaFX DSL.
    /// </summary>
    public Action<Widget> WithInitializer
    {
      set { value?.Invoke(this); }
    }

    public void AddPseudoStyleClass(string styleClass)
    {
      pseudoStyleClasses.Add(styleClass);
      OnPropertyChanged(nameof(PseudoClasses));
      InvalidateStyle(true);
    }

    public void AddStyleClass(string styleClass)
    {
      styleClasses.Add(styleClass);
      OnPropertyChanged(nameof(StyleClasses));
      InvalidateStyle(true);
    }

    public void DispatchEvent(KeyEventArgs eventData)
    {
      if (eventData.Consumed)
      {
        return;
      }
      if (!PushEvent(eventData))
      {
        return;
      }

      try
      {
        switch (eventData.EventType)
        {
          case KeyEventType.KeyPressed:
            {
              keyPressedSupport.RaiseReverse(this, eventData);
              break;
            }
          case KeyEventType.KeyReleased:
            {
              keyReleasedSupport.RaiseReverse(this, eventData);
              break;
            }
          case KeyEventType.KeyTyped:
            {
              keyTypedSupport.RaiseReverse(this, eventData);
              break;
            }
          case KeyEventType.KeyRepeat:
            {
              keyRepeatedSupport.RaiseReverse(this, eventData);
              break;
            }
          default:
            {
              throw new ArgumentOutOfRangeException();
            }
        }
      }
      finally
      {
        PopEvent(eventData);
      }
    }

    public void DispatchEvent(MouseEventArgs eventData)
    {
      if (eventData.Consumed)
      {
        return;
      }
      if (!PushEvent(eventData))
      {
        return;
      }

      try
      {
        switch (eventData.EventType)
        {
          case MouseEventType.Down:
            {
              RequestKeyboardFocusAfterMouseDown();
              mouseDownSupport.RaiseReverse(this, eventData);
              break;
            }
          case MouseEventType.Up:
            {
              mouseUpSupport.RaiseReverse(this, eventData);
              break;
            }
          case MouseEventType.Clicked:
            {
              mouseClickedSupport.RaiseReverse(this, eventData);
              break;
            }
          case MouseEventType.Moved:
            {
              mouseMovedSupport.RaiseReverse(this, eventData);
              break;
            }
          case MouseEventType.Dragged:
            {
              mouseDraggedSupport.RaiseReverse(this, eventData);
              break;
            }
          case MouseEventType.WheelMoved:
            {
              mouseWheelSupport.RaiseReverse(this, eventData);
              break;
            }
          case MouseEventType.Entered:
            {
              UpdateHovered(eventData);
              mouseEnteredSupport.RaiseReverse(this, eventData);
              break;
            }
          case MouseEventType.Exited:
            {
              mouseExitedSupport.RaiseReverse(this, eventData);
              UpdateHovered(eventData);
              break;
            }
          default:
            {
              throw new ArgumentOutOfRangeException();
            }
        }

        if (!eventData.Consumed)
        {
          Parent?.DispatchEvent(eventData);
        }
      }
      finally
      {
        PopEvent(eventData);
      }
    }

    public virtual void DispatchEvent(TouchEventArgs eventData)
    {
      if (eventData.Consumed)
      {
        return;
      }

      if (!PushEvent(eventData))
      {
        return;
      }

      try
      {
        switch (eventData.EventType)
        {
          case TouchEventType.Cancelled:
            {
              touchCancelledSupport.RaiseReverse(this, eventData);
              break;
            }
          case TouchEventType.Pressed:
            {
              RequestKeyboardFocusAfterMouseDown();
              touchPressedSupport.RaiseReverse(this, eventData);
              break;
            }
          case TouchEventType.Moved:
            {
              touchMovedSupport.RaiseReverse(this, eventData);
              break;
            }
          case TouchEventType.Released:
            {
              touchReleasedSupport.RaiseReverse(this, eventData);
              break;
            }
          case TouchEventType.Gestured:
            {
              RequestKeyboardFocusAfterMouseDown();
              touchGesturedSupport.RaiseReverse(this, eventData);
              break;
            }
          default:
            {
              throw new ArgumentOutOfRangeException();
            }
        }

        if (!eventData.Consumed)
        {
          Parent?.DispatchEvent(eventData);
        }
      }
      finally
      {
        PopEvent(eventData);
      }
    }

    public void DispatchEvent(GamePadEventArgs eventData)
    {
      if (eventData.Consumed)
      {
        return;
      }

      if (!PushEvent(eventData))
      {
        return;
      }
      try
      {
        switch (eventData.EventType)
        {
          case GamePadEventType.ButtonDown:
            {
              gamePadButtonDownSupport.RaiseReverse(this, eventData);
              break;
            }
          case GamePadEventType.ButtonUp:
            {
              gamePadButtonUpSupport.RaiseReverse(this, eventData);
              break;
            }
          case GamePadEventType.ThumbStickMoved:
            {
              gamePadThumbStickMovedSupport.RaiseReverse(this, eventData);
              break;
            }
          case GamePadEventType.TriggerMoved:
            {
              gamePadTriggerMovedSupport.RaiseReverse(this, eventData);
              break;
            }
          default:
            {
              throw new ArgumentOutOfRangeException();
            }
        }
      }
      finally
      {
        PopEvent(eventData);
      }
    }

    public sealed override void Draw(IBatchedDrawingService drawingService)
    {
      DrawWidget(drawingService);
      try
      {
        DrawChildren(drawingService);
      }
      finally
      {
        EndDrawContent(drawingService);
        if (DebugLayout)
        {
          drawingService.DrawRect(LayoutRect, Color.Blue);
          drawingService.DrawRect(BorderRect, Color.Yellow);
          drawingService.DrawRect(ContentRect, Color.Red);
        }
      }
    }

    public override void DrawOverlay(IBatchedDrawingService drawingService)
    {
      if (Hovered && Tooltip?.Visibility == Visibility.Visible)
      {
        Tooltip?.Draw(drawingService);
      }

      for (var i = 0; i < this.Count; i++)
      {
        var widget = this[i];
        if (widget.Visibility == Visibility.Visible)
        {
          widget.DrawOverlay(drawingService);
        }
      }
    }

    public virtual int GetDrawOrderForChild(IWidget w)
    {
      return 0;
    }

    public WidgetEnumerator GetEnumerator()
    {
      return new WidgetEnumerator(this);
    }

    IEnumerator<IWidget> IEnumerable<IWidget>.GetEnumerator()
    {
      return GetEnumerator();
    }

    public virtual IWidget GetFirstFocusableDescendant(Direction direction)
    {
      return Focusable ? this : null;
    }

    public object GetPropertyValue(string property)
    {
      var fn = ExpressionCache.Default.Get(GetType(), property);
      return fn?.Invoke(this);
    }

    public virtual IWidget GetSibling(Direction direction, IWidget sourceWidget)
    {
      return Parent?.GetSibling(direction, this);
    }

    public IStyledObject GetStyleParent()
    {
      return Parent;
    }

    public void InvalidateStyle(bool full)
    {
      ResolvedStyle.InvalidateCaches("From Widget.InvalidateStyle");
      styleInvalidatedSupport.Raise(this, EventArgs.Empty);

      for (var i = 0; i < Count; i++)
      {
        this[i].InvalidateStyle(false);
      }
    }

    public virtual IWidget PerformHitTest(Point mouseHitPoint)
    {
      return WidgetExtensions.PerformHitTest(this, mouseHitPoint);
    }

    [SuppressMessage("ReSharper", "ParameterHidesMember")]
    public virtual void RemoveNotify(IWidget parent)
    {
      if (Parent == null || !ReferenceEquals(parent, Parent))
      {
        throw new InvalidOperationException();
      }
      Parent = null;
    }

    public void RemovePseudoStyleClass(string styleClass)
    {
      pseudoStyleClasses.Remove(styleClass);
      OnPropertyChanged(nameof(PseudoClasses));
      InvalidateStyle(true);
    }

    public void RemoveStyleClass(string styleClass)
    {
      styleClasses.Remove(styleClass);
      OnPropertyChanged(nameof(StyleClasses));
      InvalidateStyle(true);
    }

    public void ShowTooltip(string text)
    {
      var t = Tooltip;
      if (t is Tooltip<Label> tooltipLabel)
      {
        tooltipLabel.Content.Text = text;
      }
      else if (t is Tooltip<IconLabel> tooltipIconLabel)
      {
        tooltipIconLabel.Content.Text = text;
      }
      else
      {
        Tooltip = new Tooltip<IconLabel>(UIStyle) { Anchor = AnchoredRect.CreateTopLeftAnchored(), Content = new IconLabel(UIStyle) { Text = text } };
      }
    }

    public override string ToString()
    {
      var type = GetType();
      var layoutRectText = IsArranging ? "-" : LayoutRect.ToString();
      return $"{type}={{Tag: {Tag}, Layout={layoutRectText}}}";
    }

    public override void Update(GameTime elapsedTime)
    {
      Tooltip?.Update(elapsedTime);
    }

    public sealed override void ValidateStyle()
    {
      if (!RegisteredInStyleSystem)
      {
        throw new InvalidOperationException("A widget cannot be arranged if it is not registered in the style system.");
      }
      UIStyle.StyleResolver.Revalidate();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    protected override void ArrangeOverlays(Rectangle layoutSize)
    {
      if (Tooltip != null)
      {
        Tooltip.Measure(new Size(float.PositiveInfinity, float.PositiveInfinity));
        Tooltip.Arrange(Tooltip.ArrangeChild(layoutSize));
      }
    }

    protected override Rectangle ArrangeOverride(Rectangle layoutSize)
    {
      return layoutSize;
    }

    protected virtual void DrawChildren(IBatchedDrawingService drawingService)
    {
      for (var i = 0; i < Count; i++)
      {
        var widget = this[i];
        if (widget.Visibility == Visibility.Visible)
        {
          widget.Draw(drawingService);
        }
      }
    }

    protected virtual void DrawFrame(IBatchedDrawingService drawingService)
    {
      drawingService.DrawBox(FrameTexture, BorderRect, Color);
      DrawFrameOverlay(drawingService);
      DrawWidgetStateOverlay(drawingService);
    }

    protected virtual void DrawFrameOverlay(IBatchedDrawingService drawingService)
    {
      drawingService.DrawBox(FrameOverlayTexture, BorderRect, FrameOverlayColor);
    }

    protected virtual void DrawKeyboardFocus(IBatchedDrawingService drawingService)
    {
      if (Screen?.FocusManager?.IsActive == true && Focused)
      {
        drawingService.DrawBox(FocusedOverlayTexture, BorderRect, FocusedOverlayColor);
      }
    }

    protected virtual void DrawMouseState(IBatchedDrawingService drawingService)
    {
      if (Hovered && Screen?.FocusManager?.IsActive == true)
      {
        drawingService.DrawBox(HoverOverlayTexture, BorderRect, HoverOverlayColor);
      }
    }

    protected virtual void DrawWidget(IBatchedDrawingService drawingService)
    {
      DrawFrame(drawingService);
      DrawMouseState(drawingService);
    }

    protected virtual void DrawWidgetStateOverlay(IBatchedDrawingService drawingService)
    {
      if (!WidgetStateOverlayTextureScale)
      {
        var tx = WidgetStateOverlayTexture;
        if (tx != null)
        {
          var pos = BorderRect.Center.ToVector2();
          pos.X -= tx.Width / 2f;
          pos.Y -= tx.Height / 2f;
          drawingService.DrawImage(tx, pos, WidgetStateOverlayColor);
        }
      }
      else
      {
        drawingService.DrawBox(WidgetStateOverlayTexture, BorderRect, WidgetStateOverlayColor);
      }
    }

    protected virtual void EndDrawContent(IBatchedDrawingService drawingService)
    {
      DrawKeyboardFocus(drawingService);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
      return new Size();
    }

    protected override void OnLayoutInvalidated()
    {
      Parent?.InvalidateLayout();
    }

    protected virtual void OnPadMove(Direction direction)
    {
      GetSibling(direction, this)?.GetFirstFocusableDescendant(direction)?.RequestFocus();
    }

    protected void RaiseChildAdded(int index, IWidget widget, object constraints = null)
    {
      RaiseChildrenChanged(index, null, null, widget, constraints);
    }

    protected void RaiseChildRemoved(int index, IWidget widget, object constraints = null)
    {
      RaiseChildrenChanged(index, widget, constraints, null, null);
    }

    protected void RaiseChildrenChanged(int index, IWidget oldWidget, object oldConstraints, IWidget newWidget, object newConstraints)
    {
      childrenChangedSupport?.Raise(this, new ContainerEventArgs(index, oldWidget, oldConstraints, newWidget, newConstraints));
    }

    void OnGamePadButtonDown(object source, GamePadEventArgs args)
    {
      if (args.Consumed)
      {
        return;
      }

      switch (args.Button)
      {
        case Buttons.DPadUp:
          {
            OnPadMove(Direction.Up);
            args.Consume();
            break;
          }
        case Buttons.DPadLeft:
          {
            OnPadMove(Direction.Left);
            args.Consume();
            break;
          }
        case Buttons.DPadDown:
          {
            OnPadMove(Direction.Down);
            args.Consume();
            break;
          }
        case Buttons.DPadRight:
          {
            OnPadMove(Direction.Right);
            args.Consume();
            break;
          }
      }
    }

    void OnKeyPressed(object source, KeyEventArgs args)
    {
      if (args.Consumed)
      {
        return;
      }
      if (args.Flags.IsAnyDown(InputFlags.Alt | InputFlags.Control | InputFlags.Meta))
      {
        return;
      }

      switch (args.Key)
      {
        case Keys.Tab:
          {
            if (args.Flags.IsShiftDown())
            {
              this.FocusPrevious();
            }
            else
            {
              this.FocusNext();
            }
            args.Consume();
            break;
          }
        case Keys.Left:
          {
            OnPadMove(Direction.Left);
            args.Consume();
            break;
          }
        case Keys.Right:
          {
            OnPadMove(Direction.Right);
            args.Consume();
            break;
          }
        case Keys.Up:
          {
            OnPadMove(Direction.Up);
            args.Consume();
            break;
          }
        case Keys.Down:
          {
            OnPadMove(Direction.Down);
            args.Consume();
            break;
          }
      }
    }

    void OnMouseMove(object source, MouseEventArgs args)
    {
      if (Tooltip != null && Tooltip.TooltipPosition == TooltipPositionMode.FollowMouse)
      {
        var tooltipAnchor = Tooltip.Anchor;
        tooltipAnchor.Top = args.Position.Y - LayoutRect.Y;
        tooltipAnchor.Left = args.Position.X - LayoutRect.X;
        Tooltip.Anchor = tooltipAnchor;
        InvalidateLayout();
      }
    }

    void PopEvent(EventArgs evt)
    {
      var e = currentEvents.Pop();
      if (e != evt)
      {
        throw new InvalidOperationException();
      }
    }

    bool PushEvent(EventArgs evt)
    {
      if (currentEvents == null)
      {
        currentEvents = new Stack<EventArgs>(5);
      }
      if (currentEvents.Contains(evt))
      {
        return false;
      }
      currentEvents.Push(evt);
      return true;
    }

    void RequestKeyboardFocusAfterMouseDown()
    {
      if (Focusable)
      {
        this.RequestFocus();
      }
    }

    void UpdateHovered(MouseEventArgs args)
    {
      if (ReferenceEquals(args.Source, this))
      {
        Hovered = args.EventType == MouseEventType.Entered;
      }
      else
      {
        // indirect change. 
        if (args.EventType == MouseEventType.Entered != Hovered)
        {
          Hovered = LastValidLayout().Contains(args.Position);
        }
      }
    }

    void UpdateWindowCursor()
    {
      var win = Screen?.WindowService;
      if (win != null)
      {
        win.Cursor = Cursor;
      }
    }

    public struct WidgetEnumerator : IEnumerator<IWidget>
    {
      readonly IWidget widget;

      int index;

      IWidget current;

      internal WidgetEnumerator(IWidget widget) : this()
      {
        this.widget = widget;
        index = -1;
        current = null;
      }

      public void Dispose()
      {
      }

      public bool MoveNext()
      {
        if (index + 1 < widget.Count)
        {
          index += 1;
          current = widget[index];
          return true;
        }

        current = null;
        return false;
      }

      public void Reset()
      {
        index = -1;
        current = null;
      }

      object IEnumerator.Current => Current;

      public IWidget Current
      {
        get
        {
          if (index < 0 || index >= widget.Count)
          {
            throw new InvalidOperationException();
          }
          return current;
        }
      }
    }

    public virtual void VisitStructuralChildren(Action<IWidget> action)
    {
      for (var i = 0; i < this.Count; i++)
      {
        var widget = this[i];
        action(widget);
      }

      if (Tooltip != null)
      {
        action(Tooltip);
      }
    }

    public bool RegisteredInStyleSystem
    {
      get { return UIStyle.StyleResolver.IsRegistered(this); }
    }
  }
}