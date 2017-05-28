# Style rules and Style Sheets

## Introduction

In Steropes-UI the visual appearance of elements is defined in stylesheets.
Every element has a private stylesheet for directly defined properties,
and a resovled stylesheet for indirectly defined properties. 

When the layout system detects changes in the composition of the UI component
tree or the state of components, it recomputes the resolved style of elements
and updates the attached resolved stylesheet. 

The style resolver uses a CSS inspired style-rule system. Each style-rule
has a selector that controls whether the style-properties for that rule get 
applied to a given node.

The selectors are modelled after the CSS selectors. 
   
Some style properties are inheritable. If an element has no definition for an
inherited style and no style-rule provides a value for that style, the style
system will lookup the style-property from the node's parent elements.     

HTML/CSS uses the same system when resolving font or text-color properties.

Styles are defined in XML or in code. 

DotNet already comes with XML parsing abilities, so instead of writing an 
own CSS parser and writer I am reusing the stable code that already exists 
in the runtime.
 
### A Sample style rule 
 
This is an simple example rule defined in XML.  

    <style element="Button DropDownButton">
      <conditions>
        <pseudo-class>hovered</pseudo-class>
      </conditions>
      <property name="frame-overlay-color">#4F4F4F</property>
      <property name="text-color">#E0E0E0</property>
      <property name="color">#6F6F6F</property>
    </style>

The same rule defined in code would look like this:

      var b = new StyleBuilder(uiStyle.StyleSystem);

      var widgetStyles = b.StyleSystem.StylesFor<WidgetStyleDefinition>();
      var textStyles = b.StyleSystem.StylesFor<TextStyleDefinition>();
      var rules =
        b.CreateRule(
          b.SelectForTypes(nameof(Button), nameof(DropDownBox))
            .WithCondition(StyleBuilderExtensions.HasPseudoClass("hovered")),
          b.CreateStyle()
            .WithValue(widgetStyles.FrameOverlayColor, new Color(0x4f, 0x4f, 0x4f))
            .WithValue(widgetStyles.Color, new Color(0x6f, 0x6f, 0x6f))
            .WithValue(textStyles.TextColor, new Color(0xe0, 0xe0, 0xe0)));

### Elements of a Style Rule 

A style rule consists of 2 parts:

- A element selector, with optional additional selectors attached 
- A list of style property definitions
   
If you define styles in code, create the styles and selectors using 
the StyleBuilder instance that has been initialized with your style
system instance. Style instances are bound to an instance of a 
StyleSystem. You cannot share style instances between two style systems.
 
StyleRules are applied in the order of their declaration, with more 
specific rules overriding properties of less specific rules.

A rule's specifity is the sum of all weights of all matchers and 
conditions of that rule. The following weights are used:

* Element Selector: 10
* Pseudo-Class, Class, Attribute Selector: 1000
* ID Selector: 100000
 
## Selector reference

### Matching Widget type: ElementSelector

As in CSS, all Widget matching starts with an ElementSelector. Element
selectors match widgets by the widget's NodeType property. The default
implementation return's the Widget's class name without any generic 
type-arguments. The NodeType property but can be overriden in code if 
needed.

A single ElementSelector can match multiple element types. In that case
the Widget NodeType names listed are treated as logical OR condition.

Use ``StyleBuilder#SelectForType`` or ``StyleBuilder#SelectForTypes`` 
to create a new element selector based on simple TypeNames.

     StyleBuilder b;
     ElementSelector e = b.SelectForType(nameof(Button)); 

Use ``b.SelectForAll`` in your code to match all elements regardless of
their node type.

In XML the element name is specified as ``element`` attribute on the 
``<style>`` tag. 

     <style element="Button">
       ..
     </style>  

Use ``element="*"`` to match all elements regardless of their node type.   

The selector given in the example is equivalent to the following CSS 
selector: 

     Button {
     }

### Matching Children and direct Descendants

Element selectors can be chained to create structural selector that
match nodes based on the position in the UI Widget tree.

When using chained selectors you have to be aware that the style 
properties for the rule will apply to the inner most element specified.
This is the same behaviour as in HTML/CSS. 

To match a Button that has a DockPanel as direct parent, use the
following syntax:

     StyleBuilder b;
     DescendantSelector e = b.SelectForType(nameof(DockPanel))
        .WithDirectChild(b.SelectForType(nameof(Button));
        
(and as XML)

     <style element="DockPanel">
        ..
        <style element="Button">
          ..
        </style>
     </style>
         
This is equivalent to the CSS selector

     DockPanel Button {
     }


If the Button is an indirect descendant of the DockPanel, use the
indirect DescandantSelector:

     StyleBuilder b;
     DescendantSelector e = b.SelectForType(nameof(DockPanel))
        .WithDescendant(b.SelectForType(nameof(Button)); 

(and as XML)

     <style element="DockPanel">
        ..
        <style element="Button" direct-child="true">
          ..
        </style>
     </style>
 

This is equivalent to the CSS selector

     DockPanel > Button {
     }

### Matching Style classes

A style class is a selector property that allows you assign style rules to
an Widget. This function mirrors CSS style classes. Widgets can have multiple
classes attached. Use ``Widget.AddStyleClass(string)``, ``Widget.HasStyleClass`` 
and ``Widget.RemoveStyleClass(string)`` to add, check or remove style classes.

Within a style rule, you can use a StyleClass selector to match nodes that
have a given style class.

     StyleBuilder b;
     DescendantSelector e = b.SelectForType(nameof(DockPanel))
        .WithCondition(StyleBuilderExtensions.HasClass("styleClass"); 

(and in XML)

     <style element="DockPanel">
        <conditions>
           <class>styleClass</class>
        </conditions>
        ..
     </style>

This is the same as the CSS selector 

     DockPanel.styleClass {
     }
    

### Matching Pseudo-Classes

A pseudo-class is a style class that is maintained by the system and 
added and removed in response to certain events. There are a number of
built-in pseudo-classes that all implemented widgets use, and you can
add your own classes using the API if needed. 

The following pseudo-classes are built-in:

* hovered 
 
  This class is active when the mouse has entered the visible bounding
  box of a given widget. The class is automatically removed when the
  mouse leaves the bounding box.
  
  The class is also active if one of the descendants of this widget contains
  the mouse in its bounding box.
  
* focused

  This pseudo-class is active when the widget currently has the keyboard
  focus. Only one element can be focused at a time.
  
* first

  This pseudo-class is added on all direct children of a container that
  occupy the first visual slot.
  
* last

  This pseudo-class is added on all direct children of a container that
  occupy the last visual slot.
  
Button types (Button, RadioButton, DropDownButton and ScrollbarThumb) 
additionally implement the ```down``` pseudo-class that is active when
the mouse or associated activation key is pressed.

To match against pseudo-classes in code use 

    StyleBuilder b;
    DescendantSelector e = b.SelectForType(nameof(Button))
       .WithCondition(StyleBuilderExtensions.HasPseudoClass("hovered"); 

Using XML the same code would be written as

     <style element="Button">
        <conditions>
           <pseudoClass>styleClass</pseudoClass>
        </conditions>
        ..
     </style>

which is equivalent to the CSS selector

     Button:hovered {
     }

### Matching nodes by a StyleID

Nodes can have a unique style-id associated with them. Style-IDs are 
used as a very strong matcher overridding most other matchers, similar
to HTML/CSS ID matchers.

    StyleBuilder b;
    DescendantSelector e = b.SelectForType(nameof(Button))
       .WithCondition(StyleBuilderExtensions.HasId("hovered"); 
 
Using XML the same code would be written as

     <style element="Button">
        <conditions>
           <id>styleClass</id>
        </conditions>
        ..
     </style>

which is equivalent to the CSS selector

     Button#hovered {
     }

### Matching nodes by Attributes

In addition to matching nodes by the predefined style properties above,
you can also match styles against Widget properties. Be aware that this
matching uses reflection and is therefore more resource intensive as 
other selectors. 

When matching attributes values, the style engine only matches for 
equality. String matches like "starts with", "ends with" or "contains" 
as in CSS are not implemented.

The attribute matching has two modes: It can either match an attribute
against ``<null>`` (check whether an attribute is set, this only works
for objects, not reference values) or match the value against a given
value. 

To define an attribute matcher in code that matches when a Label's 
text is not null, use:

    StyleBuilder b;
    DescendantSelector e = b.SelectForType(nameof(Label))
       .WithCondition(StyleBuilderExtensions.HasAttribute(nameof(Label.Text)); 

To match when that Text property has the value "Hello World", use
 
    StyleBuilder b;
    DescendantSelector e = b.SelectForType(nameof(Label))
       .WithCondition(StyleBuilderExtensions.HasAttribute(nameof(Label.Text), "Hello World); 


When using XML, any value that is matched must have a registered 
``IStylePropertySerializer`` registered in the parser to translate
object values from and to XML.

Serializers exist for the following types:

* System.Bool
* System.Float
* System.Int
* System.String
* Microsoft.Xna.Framework.Color
* Steropes.UI.Platform.IBoxTexture
* Steropes.UI.Platform.Insets
* Steropes.UI.Platform.IUIFont
* Steropes.UI.Platform.IUITexture
* all Enum types

You can register serializers via the ``IStyleSystem#registerSerializer``
method. When referencing type names, you can either use the fully qualified
type name (as returned by Type.FullName) or a short name (as returned
by ``IStylePropertySerializer.TypeId``). As a convention, the type id is
the simple class-name of the serialized type.

Matching whether an non-null value exists:

    <style element="Label">
      <conditions>
         <attribute>
           <name>Text</name>
         </attribute>
       </conditions>
     </style>    

and matching a specific value:

    <style element="Label">
      <conditions>
         <attribute>
           <name>Text</name>
           <type>String</type>
           <value>Hello World</value>
         </attribute>
       </conditions>
     </style>    

### Logical operations and combining multiple values

Style rules can combine multiple conditions into a more complex style
condition. By default, if multiple conditions are specified, these
conditions are combined as logical AND conditions - that means all
conditions must match for this rule to be active.

    StyleBuilder b;
    DescendantSelector e = b.SelectForType(nameof(Label))
      .WithCondition(
        StyleBuilderExtensions.HasAttribute(nameof(Label.Text))
          .And(StyleBuilderExtensions.HasId("hovered"))
          .And(StyleBuilderExtensions.HasClass("StyleClass"))
      )

or as XML

    <style element="Label">
      <conditions>
         <attribute>
           <name>Text</name>
         </attribute>
         <class>AStyleClass</class>
         <pseudoClass>hovered</pseudoClass>
       </conditions>
     </style>    

Conditions can be negated via the Not-condition:

     StyleBuilderExtensions.Not(..)
     
as XML:

    <style element="Label">
      <conditions>
         <not>
            .. 
         </not>
       </conditions>
     </style>    

and a logical OR is defined via 

    StyleBuilder b;
    DescendantSelector e = b.SelectForType(nameof(Label))
      .WithCondition(
        StyleBuilderExtensions.HasAttribute(nameof(Label.Text))
          .Or(StyleBuilderExtensions.HasId("hovered"))
          .Or(StyleBuilderExtensions.HasClass("StyleClass"))
      )

as XML:

    <style element="Label">
      <conditions>
         <or>
            .. 
         </or>
       </conditions>
     </style>    

These conditions can be nested as needed:

    <style element="Label">
      <conditions>
         <or>
           <and>
             <not>
                ..
             </not>
            .. 
           </and>
            .. 
         </or>
       </conditions>
     </style>    

     
## Style property reference

All durations are given as floats specifying seconds. This aligns this
with the GameTime struct. 

### Widget Styles

#### Layouting

* **visibility** (enum Visibility)

  The visible style controls whether and how an Widget takes part in the
  layout and rendering process. Like the visibility in WPF, this is a
  tri-state property: 

  * Visible
    
    The widget consumes space in the layout and is rendered. 
  
  * Hidden 
  
    The widget consumes space, but is not rendered. Use this if you want
    to hide UI elements without affecting the visual layout. 
    
  * Collapsed
  
    The widget is ignored, does not consume space and is not rendered.
    Use this if you want to fully remove elements from the layout. 
    Any space previously used by the widget will be reclaimed and other
    elements can move into this space.

* **padding** (Steropes.UI.Platform.Insets)

  The padding defines empty space between the Widget's frame border and
  any child content contained in the widget. 
  
  Padding uses insets, a simple struct that defins spacing for the four 
  cardinal edges (``top``, ``left``, ``bottom``, ``right``). In the XML 
  parser, a special ``all`` property can be used to set the same value
  to all four edges. 
  
* **margin** (Steropes.UI.Platform.Insets)

  The margin defines empty space around the Widget's frame border. This
  space is allocated to the widget and can be used to put space between
  widgets. Margins of widgets arre static and do not collapse into each 
  other.
  
  Margin uses insets, a simple struct that defins spacing for the four 
  cardinal edges (``top``, ``left``, ``bottom``, ``right``). In the XML 
  parser, a special ``all`` property can be used to set the same value
  to all four edges. 

#### Rendering

* **color** (Microsoft.Xna.Framework.Color)

  Color defines the primary color of a Widget. This ``color`` property 
  is used as fallback value for all other ``*-color`` properties found
  on widgets if they do not provide an defined value.
  
  This color is used to tint the frame-texture when rendering an element.

* **frame-texture** (Steropes.UI.Platform.IBoxTexture)

  The frame texture forms the first layer of textures when drawing
  Widget. Textures for widget rendering are specified as IBoxTexture.
  
  A box texture divides a single texture into 9 smaller textures using
  a standard UI texture reference and an Insets struct to define the
  sections. The inset specifies the width of the border on an edge.
  
  <table>
    <tr>
      <td>Top-Left-Edge</td>
      <td>Top-Edge</td>
      <td>Top-Right-Edge</td>
    </tr>
    <tr>
      <td>Left-Edge</td>
      <td>Content</td>
      <td>Right-Edge</td>
    </tr>
    <tr>
      <td>Bottom-Left-Edge</td>
      <td>Bottom-Edge</td>
      <td>Bottom-Right-Edge</td>
    </tr>
  </table>  
  
  To specify a box-texture in XML, use
  
      <texture>
        <name>FrameSelected</name>
        <corners>
          <top>4</top>
          <left>4</left>
          <bottom>4</bottom>
          <right>4</right>
          <!-- or shorter : <all>4</all> -->
        </corners>
      </texture>
 
  
  Widgets use several overlaid textures to render the full widget.
  Any texture that does not exist is ignored.

  The standard Widget rendering order is:
    
  1. frame-texture
  2. frame-overlay-texture
  3. widget-state-texture
  4. hover-overlay-texture
  5. (child-elements and custom content)
  6. focused-overlay-texture
  
* **frame-overlay-texture** (Steropes.UI.Platform.IBoxTexture)
* **frame-overlay-color** (Microsoft.Xna.Framework.Color)

  Defines the second layer of a widget rendering. This is normally
  used for decorations if those decorations cannot be integrated into
  the main texture or for state representations.

* **widget-state-overlay-texture** (Steropes.UI.Platform.IBoxTexture)
* **widget-state-overlay-color** (Microsoft.Xna.Framework.Color)
* **widget-state-overlay-scale** (bool)

  Defines the third layer of a widget rendering. This is reserved 
  for state representations. The CheckBox Widget uses this layer to
  set a texture representing the current selection state. 
  
  Based on the widget implementation, this texture may be rendered
  at different bounds. The ProgressBar uses the ``widget-state-overlay``
  to render a filled section based on the relative progress it visualises.
  
  This texture can be scaled to fit the border-box or can be rendered 
  in its original size by using the ``widget-state-overlay-scale``property.
  If the texture is not scaled to fit it will be rendered centred in
  the border-box by default.
  
* **hover-overlay-texture** (Steropes.UI.Platform.IBoxTexture)
* **hover-overlay-color** (Microsoft.Xna.Framework.Color)

  Defines the fourth rendering layer, usually reserved for mouse-hover
  effects. The default implementation will only render this texture if
  the current screen has the keyboard focus and this Widget has the
  hover-property active.
  
  This effect could have been achieved with styles, but hard-coding this
  common feature makes it less verbose to define new Widgets.

* **focused-overlay-texture** (Steropes.UI.Platform.IBoxTexture)
* **focused-overlay-color** (Microsoft.Xna.Framework.Color)

  Defines the last pre-defined rendering layer. This layer is rendered
  after all content and all child-elements have been rendered. The default 
  implementation will only render this texture if the widget has the 
  keyboard focus.  
  
  This effect could have been achieved with styles, but hard-coding this
  common feature makes it less verbose to define new Widgets.

#### Tooltips

* **tooltip-delay** (float, inherited) 
* **tooltip-display-time** (float, inherited) 
* **tooltip-position** (enum TooltipPositionModel, inherited)

All widgets have built-in support for displaying tooltips. A tooltip
should be a lightweight component. Tooltips do not receive input events
and are rendered on a separate layer and do not affect the layout of the
main layout tree. 

If a tooltip is defined (via ``Widget.ShowTooltip`` or by assigning a
``Tooltip`` instance to ``Widget.Tooltip``), it will triggered by the
mouse-hover state, will start to be displayed after the defined delay
and will be displayed for the given amount of display-time. The 
``tooltip-delay`` and ``tooltip-display-time`` properties use float 
values speciifying seconds and fractional seconds. Use ``0.5`` for 500ms,
``1.0`` for 1 second and so on. 

The ``tooltip-position`` style defines how the tooltip is positioned.
``tooltip-position`` can be either ``Fixed`` or ``FollowMouse``.
Tooltips are arranged based on the Anchor property of the tooltip Widget.
If the style is ``FollowMouse`` mouse-events will modify the anchor to
set the absolute position of the Tooltip 

### Button Styles

Buttons are specialized widgets that react to input by toggling their 
selection state or firing action events. 

Button, RadioButtonSetContent, ScrollbarThumb and DropDownButton are
direct implementations of buttons. CheckBox internally uses a button
along with a label to control the selection state.
 
* **down-overlay-texture** (Steropes.UI.Platform.IBoxTexture)
* **down-overlay-color** (Microsoft.Xna.Framework.Color)

Buttons extend the normal widget rendering sequence with an additional
render layer to indicate the "down" state of an button. A button is 
down when a key or mouse-button has been pressed but not yet released.
The button's action will be fired when the button is released.   
 
### Image styles

* **texture** (Steropes.UI.Platform.IUITexture)
* **texture-color** (Microsoft.Xna.Framework.Color)
* **texture-scale** (bool)

An image displays the content of a texture. The scale property defines 
how the texture is rendered:

* None - The image is centered into the Widgets bounding box.
* KeepAspectRatio - the image is scaled with an uniform scale factor.
* Scale - the image is stretched across the box to fill the complete space.

### IconLabel styles

An icon-label combines an image with text. 

* **icon-text-gap** (int)

Use the icon-text-gap property
to control the spacing between image and text. The image and text can be
formatted separately as "Image" and "Label" child elements.

### Notebook styles

A notebook is a tabbed view that allows you to separate the different
aspects of a GUI into separate panes. 

* **notebook-tab-overlap-x** (int)
* **notebook-tab-overlap-y** (int)

Depending on the frame-texture used, you may want to let tab-buttons
overlap slightly. In that case, use ``notebook-tab-overlap-x`` or
``notebook-tab-overlap-x`` to define the offset.  

### Scrollbar styles

A vertical scrollbar. This component is rarely used on its own, must
user will use a ScrollPanel instead. At the moment there is no horizontal
scrolling. 

* **scrollbar-mode** (Steropes.UI.Widgets.ScrollbarMode)

This property controls how a scrollbar appears. If ``scrollbar-mode`` 
is set to none, the scroll-panel will try to fit all content into whatever
space is available without assuming that that area can be expanded.

Possible values are:
* None 

  disable scrolling and hide the scrollbar.
  
* Auto 

  allow scrolling and show the scrollbar if the content is larger than
  the available visible space.

* Always

  show the scrollbar regardless whether the displayed content requires
  scrolling. This can help with jumpy UIs where the scrollbar's appearance 
  or disappearnce causes noticable visual changes to the final layout.

### Text styles

These styles apply to all text widgets. Available text widgets are Label, TextField,
TextArea and PasswordBox and any of the DocumentView implementations. All text
styles are inherited, so you can define them on the root Widget or a common parent 
in your GUI and all child widgets then will share the same text styling.  

* **font** (Steropes.UI.Platform.IUIFont, inherited)

Defines the font, text color and alignment of the text. A UI-font is a thin
wrapper around Monogames sprite-font class to allow GUIs to be unit-testable.
As Sprite-Fonts encode both the font-face (how the font looks) and the font-size
(how tall each character is) there is no point in separating font-size or style
from the font-face in this library.

To define a bold or italic version of any font you have to define a separate 
sprite font.    
   
Text components will fail if no font is defined, with Sprite-Fonts there is no 
notion of built-in fonts in the system.
   
* **text-color** (Microsoft.Xna.Framework.Color, inherited)

Defines the text-color of the font. If not defined, the main color of the 
Widget or its parent is used.

* **text-alignment** (Steropes.UI.Components.Alignment, inherited)

Defines how text is aligned. Possible values are:

* Start
 
  Aligns the text to the left edge of the widget. Multi-line text will have a 
  ragged right-hand edge.

* Center
 
  Aligns the text in the centre of the widget leaving equal space to the left 
  and right.

* End 
 
  Aligns the text to the right edge of the widget. Multi-line text will have a 
  ragged left-hand edge.
  
* Fill

  Justifies text. Multi-line text will be spaced out to fill the available space
  to both edges. The last line of any text will be left-aligned. 
  
  In an IconLabel the image and content will be rendered as 
  ``Start`` aligned, but any multiline label text contained will be rendered 
  justified within the label.

* **outline-color** (Microsoft.Xna.Framework.Color, inherited)
* **outline-size** (int, inherited)

When an font outline is defined, the text will be rendered several times using 
outline-size as offset. When using the same color as the main text-color,
this yields a bold-effect without having to explicitly provide a bold font.
When using a different color, the rendered text receives a glow effect.

No extra rendering is done if the outline-size is zero or negative.

* **wrap-text** (Steropes.UI.Widgets.TextWidgets.WrapText, inherited)

Controls whether and how long lines of text are broken into lines. 

* Auto

  Long lines of text will be broken at the end of the available space
  and when the renderer encounters a linebreak.
  
* Manual

  Text is only broken into multiple lines when a linebreak is encountered
  in the text.
 
* None

  The text is never broken into lines and all line-breaking is disabled.

* **underline** (bool, inherited)
* **strike-through** (bool, inherited)

Defines whether underline or strikethrough text decorations are added to
the rendered text.

* **caret-width** (int, inherited)

Defines the width of the caret when rendering editable text widgets. The
caret is attached as a child-widget to the text element. This allows you
to match the caret width to the size and intent of the text widget.

* **caret-blinking** (bool, inherited)

Defines whether the carred is blinking.

* **caret-blinkrate** (float, inherited)

Defines the blink-rate of the caret.

* **selection-color** (Microsoft.Xna.Framework.Color, inherited)

Defines the selection color. This color will be used as background on the 
selected text.

* **ListView** styles

These properties only applies to the ListView Widget. A list-view can be used
as standalone Widget and is used to render the drop-down in a DropDown widget.

* **min-height** (int)
* **max-height** (int) 

Defines the maximum and minimum size of a list view. This is mainly useful
for drop-down buttons where the list view is rendered in its own pop-up.

* **max-lines-visible** (int) 
* **uniform-item-height** (bool) 

Defines whether all items in the list-box should be rendered at the same 
height and how many items should be shown at most. The ``max-lines-visible``
setting further restricts the computed maximum height in addition to the
value defined in ``max-height``.


