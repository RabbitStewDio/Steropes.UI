# Concepts

Steropes-UI reuses many concepts found in other UI
libraries. A UI consists of multiple ``Widget`` instances
that are controlled by a single ``Screen`` instance. 

Important: Steropes-UI is meant to be used from within the
games main thread. The widgets and helper classes are not
thread-safe and any use outside of the main thread must be
properly synchronized or will cause subtle errors and not
so subtle crashes.

## Architecture

When a UI is running the screen instance receives input events
from the associated input manager, interprets and maps those 
events to a Widget, lets all widgets update their state based
on the time elapsed since the last update and finally renders
all widgets. 

To see how the various components work together, it is useful to
look at how the system is initialized and after that, how a 
frame is rendered in a game.

### Initialisation

All of the initialisation of the UI framework happens during 
the component setup phase. The Steropes-UI code wraps all core
functions into standard XNA/MonoGame components. These 
components take part in the standard game loop and receive 
Update() and Draw() events from the MonoGame system.

For reference, here is the initialisation code of the
Quickstart module:

    class SimpleGame: Game
    {
      public GraphicsDeviceManager Graphics { get; private set; }
  
      public SimpleGame()
      {
        Content.RootDirectory = "Content";
        Graphics = new GraphicsDeviceManager(this) 
        { 
          PreferredBackBufferWidth = 1280,
          PreferredBackBufferHeight = 720 
        };
      }
  
      protected override void Initialize()
      {
        base.Initialize();
  
        IsMouseVisible = true;
  
        var uiManager = UIManagerComponent.CreateAndInit
                (this, new InputManager(this), "Content");
  
        var styleSystem = uiManager.UIStyle;
        var styles = styleSystem.LoadStyles
                ("Content/UI/Metro/style.xml", "UI/Metro");
        styleSystem.StyleResolver.StyleRules.AddRange(styles);
  
        uiManager.Root.Content = 
                 new Label(styleSystem, "Hello World!");
      }
    }
  
The core component you will be interacting with is the 
IUIManager component. This class provides you with the ability
to define styles and to set up the Widget component tree.

``UIManagerComponent`` contains a few easy setup methods 
to simply the wiring up of the system components.

Central to all event processing is the ``InputManager``, which
polls the MonoGame event queue for input events. The 
InputManager translates the Monogame input states into a 
stream of InputEvents. Whenever the state changes, the input
manager generates new events for the UI system to process.

The ``CreateAndInit`` method instantiates the core service
needed to render the (this code is :

    var drawingService = new BatchedDrawingService(game);
    var windowService = new GameWindowService(game);
    var cm = new ContentManager(game.Services) { RootDirectory = rootDirectory };

    var uiManager = new UIManager(inputManager, drawingService, windowService, cm);

    var uiManagerComponent =  new UIManagerComponent(game, uiManager);
    var inputManagerAsComponent = inputManager as IGameComponent;
    if (inputManagerAsComponent != null)
    {
      game.Components.Add(inputManagerAsComponent);
    }
    game.Components.Add(uiManagerComponent);

    var uiManager = uiManagerComponent.Manager;
    uiManager.Start();

The UIManager is a pure helper class to simplify the common task 
of setting up an UI. It is responsible for creating the 
``BatchedDrawingService``, a wrapper around ``SpriteBatch`` that 
makes rendering operations unit-testable and the 
``GameWindowService``. 

The ``GameWindowService`` connects the UI to the native platform 
to support localised input and access to the clipboard. The 
default implementation uses the standard .NET and MonoClasses 
without using any platform dependent code and is safe to use 
on any platform supported by MonoGame. A Windows specific 
version with extended support exists in the Steropes.UI.Windows 
module. This code allows access to the native clipboard and can 
translate raw keystrokes into localized keystrokes that match 
the user's current keyboard layout.

The ``UIStyle`` class is responsible for managing styles and 
fonts. The visual appearance of each Widget is defined by its 
style. Widgets use the stylesheet associated with it to lookup 
fonts and textures. The stylesystem contains a CSS inspired 
style resolver that matches style rules to the structure of the 
UI component tree. This allows for flexible and efficient 
styling rules without having to explicitly define visual style 
properties for each instance.

The ``UIManager`` forwards all Update() and Draw() requests to 
a ``Screen`` instance. The screen is responsible for 
transforming user input into Widget events and for updating 
and maintaining the Widget and layout state. 

There can be only one screen per input manager and there should 
never be more than one input manager per Game instance in the 
system. MonoGame uses a polling based input system and input 
managers cannot tell whether another input manager has already 
generated an event for an particular state update. This can lead
to the same input being processed multiple times. The Screen 
class exists to feed events to the RootPane and the Widget tree 
and to validate the styling and layout of the tree after its 
state has changed.

When defining an UI, you are most likely to work directly with 
the RootPane and its child Widgets. The root pane is a 
multi-layered pane that manages the main components and any 
internal dialogs and popups generated by Widgets.

# Types of Widgets

Widgets in the system can be divided into two main classes. 
Basic widgets are simple components that collect input from 
the user or that display data items (text, images) to the user. 

Container widget are widgets that contain or manage other 
widgets. This group can be further subdivided into Single-Child 
container, which exist to manage a single Widget and Multi-Child 
container, which exist to layout multiple widgets relative to 
each other. Container widgets can be nested into each other.

Most widgets are composite objects utilizing multiple simpler 
widgets to do their job. The difference between a basic widget 
and a container widget is whether those internal details are 
exposed via the public API. Container widgets have an open API 
where properties and public methods make it easy to add new
widgets together. Basic widgets discourage poking around in 
the internals by hiding as much implementation details as 
possible. 

## Basic widgets

The basic widgets the system offers are:

* Button
* CheckBox
* CustomViewPort
* IconLabel
* Image
* KeyBox
* Label
* ProgressBar
* ScrollBar
* Slider
* SpinningWheel
* TextArea
* TextField
* PasswordField

The two item-container widgets
* DropDownBox
* ListView

are normally used like basic widgets. When rendering items, 
these widgets generate child widgets on the fly to fill the
itemlist and thus are somewhat related to container items. 

## Container

Container widgets contain other publicly accessible widgets. 

### Single-Child container

Single child container manage exactly one publicly accessible 
child element.

* ScrollPanel

The scrollpanel allows you to add vertical scrolling to a widget. 
The widget contained inside the scrollpanel can be larger than 
the space availble in the layout. The scrollpanel will manage 
the scroll position and viewport for you. At the moment, only 
vertical scrolling is implemented.

* Popup
* Window
* OptionPane

These container widgets represent windows and accept exactly one 
widget as child. Normally you will be using one of the layout 
container widgets as this first child. 

To implement your own single-child widgets, use the ContentWidget 
or InternalContentWidget class as convenient starting point. 

### Layout (Multi-Child ) container

Container that can contain more than one child are usually used 
for layouting element. The following container are supported:

* BoxGroup

  Stacks elements either horizontally or vertically. This class 
  is essentially the same as the WPF StackPanel
  
  The ``bool`` constraint defines whether an widget will be 
  marked as expanded. Expanded widgets consume any extra space 
  that may be available.
 
* DockPanel

  Arranges elements along the edges of the bounding box. This 
  widget is the same as WPF's DockPanel and has eaxtly the same 
  behaviour. 
  
  A DockPanelConstraint is used to define the edge to which an 
  widget is bound. The last component added can be stretched to 
  consume all remaining available space.

* Grid

  A simple non-uniform grid. This grid panel does not support 
  column- or row-span,  but columns and rows can be sized either 
  using static, relative or remaining sizing.
  
  The Point layout constraint defines the grid coordinates of the 
  widget to be added.

* Group

  A general non-layouting container. This container is similar 
  to the Canvas panel in WPF. You can position widgets by 
  defining the Anchor property on the child widgets.

* Splitter

  A horizontal or vertical split-panel. This pane is similar to 
  the SplitPane found in Java Swing. It has two slots for 
  child-widgets divided by a draggable splitter bar. 
  

# Event processing

Input events are generated by the input manager and consumed by 
the screen. There are four classes of input events provided by 
MonoGame:

* Mouse events
* Touch events
* Key events
* GamePad/Joystick events

Mouse and touch events are delivered to whatever widget is most 
visible at the given coordinate. To select the correct widget 
for a given mouse or touch event, the Screen class examines the 
Widget's visibility and layout position. Widgets that are invisible 
cannot receive events.

If a widget receives a mouse-down event and is capable of 
receiving the keyboard focus, then this widget will also 
automatically receive the keyboard focus during the processing 
of this event.

Key and GamePad events are delivered to whatever widget has the 
keyboard focus. The focus can be changed via the Tab-Key or via 
the GamePad or Joystick directional input. Only one widget can 
receive the keyboard focus. The widget having the focus must be 
visible and must be directly or indirectly attached to the root 
panel. 

An Widget can control whether it wants to receive the keyboard 
focus or how the focus should be passed on to the next component. 

Events are sent to the most specific Widget in the tree. When an 
event is not handled by the widget, the event will be passed on
to the parent widget until either the event is marked as processed
or there is no parent left.

Event pre-processing is defined in the ``ScreenEventHandling`` 
class.

# Box sizes

Every widget consumes a rectangular space in the layout. A 
widget's final layout size can be queried using Widget.LayoutRect 
property. The layout rect is absolutely positioned and can be 
directly used to render the widget.

Widgets have margins and padding. 

The widget's content space is the rectangular space available to 
lay out child widgets and to render content. The border-rect takes 
this space and adds the padding to each edge. The widget's final 
layout size is then calculated as the border box space expanded 
by the margings. 

    width = margin-left + padding-left + content-width + padding-right + margin-right
    height = margin-top + padding-top + content-height + padding-bottom + margin-bottom

There is no explicit space reserved for the border itself. Use 
paddings and margins to reserve space for any border texture as 
needed.


# Arrange/Measure

Whenever a widget state or style changes, the system may have 
to recalculate the layout of the widget tree.

Calculating a new layout happens in two stages. First, a measure 
pass iterates over the whole tree and tries to compute the 
desired size of all elements, and after that an arrange pass 
iterates the tree again attempting to position elements at their 
final location.

All positions after arrange are global positions that can be 
passed to the rendering code without any further calculation. 
The results of arrange and measure are cached on the widgets, 
so that layout calculations only happen when there are changes 
either to the widget properties or the position and/or size of 
the widget on the screen.

1. Measure
 
   During the measure phase, a widget can compute a desired 
   size for use in layout decisions. That size represents the 
   space the widget would given the contraints provided 
   (which can be infinite).

   A computed desired size is valid for the last measure size 
   and is cached for performance reasons. 
   
2. Arrange

   During arrange a container positions all child widgets 
   to give each widget a location and space according to their 
   reported desired size and other layout constraints imposed 
   by a container. 

   Arrange always receives a computed location and size from 
   their parent widget and the widget tries to fit all child 
   widgets into the given size. It is possible for widgets 
   to exceed the size given and to be positioned outside of 
   the arrange-boundaries. 

   Widgets that limit the available space usually use 
   clipping (via the ViewPort's scissor rect) to avoid painting
   over other widget.

   The system will not allow you to query the LayoutRect of 
   a widget that is currently undergoing an Arrange-Calculation. 
   You can normally can query the ``LayoutRect`` of child widgets 
   during the arrange of its container widget, but not the 
   other way around. 

Both MeasureOverride and ArrangeOverride receive Border-Boundary 
boxes.

When you implement your own widgets, prefer composition over 
sub-classing and make use of the existing container and widget 
implementations. You can hide the internal widget tree by 
using a ``InternalContentWidget`` as base class. Most of the 
library widgets make use of this pattern instead of 
reimplementing similar layout strategies in their own classes. 
If needed, you can override both the ArrangeOverride and 
MeasureOverride functions in your widget implementations to 
calculate your own layout positions. 

# Styling

In Steropes-UI all widgets separate behavioural control 
properties from visual styling. Style information is stored in 
style-rules. Widgets have a ResolvedStyle instance that 
combines both the manually defined styles and the styles 
received from the global style-rules (as resolved by the
style system). 

## Style resolution and style inheritance

When a style property is needed for rendering, the Widget queries 
the resolved style collection. This collection maintains both the 
manual styles, resolved styles and styles inherited from the 
parent widget. Manual styles take precedence over all other 
styles. Resolved styles are computed by the style-resolver. This 
process is triggered by changes in the Widget state (via 
``INotifyPropertyChanged`` events), but actual resolving is 
delayed until the layout calculation starts. This way the system 
can combine all changes into one single calculation as and when 
needed.

## Style classes and pseudo classes

Although it might be tempting to just set styles via those 
properties, the more maintainable strategy is to define 
pseudo-classes and normal style classes to represent the various 
states of the widget in your UI.

Use style-classes to describe a business-logic state of your 
widget or widget tree. Examples of logical states are 
"mandatory-input-missing" when validating form inputs or for 
games a "low-health" state could render text in bright red 
as warning to the user.

In contrast a "pseudo-class" state should be used to describe 
internal technical states of the widget implementation. A 
push-button's "down" state is a good example for a pseudo-class 
as is a link's "visited" state.

A a rule of thumb: Style classes should be defined and managed 
by your business- or game logic, while pseudo-classes are 
defined and managed by the widget-implementation.

## Invalidating style information

Sometimes you will need to force the system to recompute style 
information for a widget subtree based on some internal logic. 
Use ``Widget.InvalidateStyle(bool)`` to trigger a style refresh 
for a widget and all its children. Where needed you can use the 
``Widget.LayoutInvalidated`` event to listen to changes (to 
invalidate caches, for instance).

If you define properties on your widget, make sure that changes 
to the property values trigger a ``PropertyChanged`` event. The 
layout system will selectively listen to these events whenever 
an attribute-rule references this particular property. 

# Drawing

Once the layout is calculated, the Screen can trigger the 
drawing phase. All Drawing operations are encapsulated in the 
IBatchedDrawingService, which requires IUITexture and IUIFont 
instances to render content.

IBatchedDrawingService, IUITexture and IUIFont are thin 
wrappers around the standard XNA/Monogame classes SpriteBatch, 
Texture and SpriteFont. As interfaces they are easy to mock 
in unit-tests and allow you to fully test widgets and complex 
UIs in code.

Rendering happens in the ``Draw(IBatchedDrawingService)`` method. 
The default implementation defines several independent layers 
that are rendered in a defined order.

The standard Widget rendering order of Widget.Draw() is

  1. Widget.DrawWidget()
  
     1. Widget.DrawFrame()
        
        Draws frame-texture

        1. Widget.DrawFrameOverlay()
           
           Draws the frame-overlay texture

        2. Widget.DrawWidgetState()
           
           Draws the widget-state texture

     2. Widget.DrawMouseState()
        
        Draws the hover-texture 

  2. Widget.DrawChildren()

     Calls Widget.Draw(..) on each visible child.

  3. Widget.EndDrawContent()
  
     Draws the focus-overlay

  4. (if enabled: Draw debugging indicator boxes)

     This drwas some boxes to indicate the layout-rect (blue), 
     border-rect (yellow) and content-rect(red).

Rendering depends on textures. If there is no texture for a 
given layer, the rendering will be skipped.

## SpriteBatch and custom rendering

When working with complex UIs, not everything can be expressed 
as static textures. If you need to add your own rendering code, 
but want to take part in the layouting and style processing, you 
can use a CustomViewport implementation to add your own rendering. 

The ``CustomViewport```class takes care of suspending the 
SpriteBatch and saves and restores the current viewport for you. 
Be aware that the default implementation of CustomViewport does 
not set up any clipping for you.

# Text processing

Widget text processing is encapsulated in the ``TextWidgetBase``
class and the IDocument implementation. Text is contained in 
documents, which can be modified using edit operations. 

The document represents the raw text content. Internally, 
documents may reorganize the content into a Document Object 
Model (DOM) to better represent the logical structure of the 
document. The DOM can be accessed via ``ITextDocument.Root``. 
Text ndoes represent content with shared attributes or 
formatting. If text contains multiple formats (ie rich-text) 
each uniquely formatted text is represented as a new text node. 
Text nodes are immutable - changing the state or composition 
never modifies existing instances, it always generates new  
nodes instead. This greatly simplifies the internal state 
management in text components. When an edit operation occurs, 
the document updates its internal state and rebuilds the 
modified text-nodes to represent the new text content. 

Text-Documents are able to generate markers that represent 
positions in the current text. Markers are a convenient way 
to remember positions and to keep those positions up to date 
during and after edit operations. 


Text documents are rendered using TextViews. Text views are 
responsible for arranging text and for flowing nodes into 
sections, paragraphs and lines. You can use a text view to 
navigate documents (that is repositioning the cursor in response 
to user actions) and to map screen coordinates to text 
coordinates. 

TextViews are managed by a document view. The document view 
monitors the document and invalidates its state when the 
document changes. The DocumentView will ensure that all text 
nodes have a valid layout before they are rendered. Multiple 
edit operations er frame can be combined into a single layout 
and reflow operation.

Documents can be shared across multiple views. 

The ``TextWidgetBase`` class is a convenient wrapper around a 
IDocumentView implementation and is used for read-only views 
of text documents. It can display text, but does not allow user 
input to modify the text or to select text content. A Label is 
a convenient way of displaying read-only text.

Use the ``TextEditorWidgetBase`` if you want to edit text or want 
to let users interact with the text by selecting content. The 
TextEditorWidgetBase class maintains the primary caret that 
represents the positions where new text will be inserted in 
response to keyboard events. It also maintains a list of 
RemoteCarets that represent additional users making edits 
concurrently. Each caret maintains its own text selection range 
and offers operations to expand or clear selections. 

The TextEditorWidgetBase contains a large number of standard 
operations and adds standard key-combinations to the widget for 
navigation, copy&paste and for managing selections. Use this 
class when you need to create new widgets that allow the editing 
of text.

Steropes-UI ships with three standard widgets that should cover 
most common usecases:

* TextField

  Use this for allowing users to enter single-lined text. Any 
  linebreaks entered will be filtered out of the document.

* PasswordBox

  A TextField used to enter passwords. All input is masked by 
  a masking character.

* TextArea

  A multi-line text input. The text area can also display 
  line-numbers in front of the text.
