Steropes-UI - A monogame UI library
===================================

Steropes-UI is a lightweight UI framework built on top of Monogame. The library 
is a heavily redeveloped version of NuclearWinter. Writing at least some UI code 
is part of every game development, and this library makes it easier and less
error prone than manual coding. Steropes-UI currently does not contain an 
equivalent for NuclearWinter's RichText, Tree and Table widgets.

Steropes uses a simple object model to define UIs that are both performant and
testable. Similar to HTML-pages, Steropes separates the structure of your UI from
the actual styling. The widget implementations contain logic and layouting, while
style information is defined over the structure using selectors and style-rules.

## Why you would want to use Steropes-UI

The Steropes-UI library follows three simple guiding principles:

1. The library must be lightweight, non-intrusive and not opinionated in the way
   code is written. 
   
   Many UI libraries for Monogame come as part of a larger framework and it is 
   sometimes hard or impossible to use the UI code without the framework and
   its dependencies. The core of Steopes-UI does not come with any external 
   dependencies on either libraries or OS specific code.
    
2. The code of the library, and code using the library must be unit-testable.

   My biggest frustration with exsting UI libraries was the fact that if something
   goes wrong, debugging is made harder as external dependencies of classes are
   rarely separated from the code. 
   
   Steropes-UI widget behaviour including the actual rendering can be validated in
   simple unit-tests. Interfaces into Monogame that would require access to a running
   game or graphics context are kept behind easily mockable interfaces.  
  
3. Assembling UIs or creating new widgets should be as uncomplicated and simple as
   possible. 
   
   In Steropes-UI borrows many ideas from established UI frameworks. Each Widget
   can have child-widgets. Widgets are mostly assembled from existing basic building
   blocks. Code in Steropes-UI is highly reusable, and if you worked with other UI
   frameworks before, you should find many familiar concepts here. 
   
   Similar to how WPF uses a visual component tree to build complex behaviour out of
   predefined building blocks, most Steropes-UI widgets are assemblies of other widgets
   and primitive textures. 
   
   Styling is separate from the compiled code. Steropes-UI uses a CSS inspired style
   system with selectors and style-sheets to define the visual look and feel of widgets.
   Visual behaviour like representing selections, or applying hover or mouse-click 
   effects are defined via styles. 
   
   UI widgets can be easily - and consistently - styled while still making it easy to
   customize the look and feel of UI elements based on their assigned style-classes or
   programmatic state.
   
   And last but not least: Having expressive style-sheets for your UIs to define 
   visuals allows your designers more freedom when working on your GUI without having
   to request code-changes for simply adjustments. 

## Supported Widget types

Steopes widgets are split into three groups.

### Basic Widgets
These widgets provide basic building blocks for UIs and deal directly with collecting 
user input.

* Label - Displays static text
* Image - Displays an image
* IconLabel - Combines label and image.
* Button - A simple push-button.
* Checkbox - A check-box.
* Dropdown Box - Allows a single selection from a list of elements.
* Listview - A list of elements. Multiple selections are possible.
* Radiobutton-Set - A set of buttons for making a single selection.
* Slider - Numeric input control.
* Progress-Bar - Shows progress of long running tasks.
* Spinning-Wheel - A generic wait indicator that rotates a static image.
* Tooltip - A hovering tooltip. Available on all widgets.
* Scrollbar - A standalone scrollbar. Normally used inside a ScrollPanel.
* KeyBox - Records a single key-stroke.

### Text Components

* Text-Field - Single line text input.
* Password-Box - Same as text-field, but hides the input behind a masking character.
* Text-Area - Multiline text input, with optional line-numbering.

### Container

* ScrollPanel - Single element container for large content. Allows for vertical scrolling. 
* Notebook - A tabbed pane. 
* Splitter - Container for two components, with splitter bar to adjust relative sizes.
* OptionPane - Standard "yes, no, cancel" dialogs.
* Popup - An modal popup window. Disappears when loosing focus.
* Window - A window.

### Layout Container
* Group - Arranges child content based on their "AnchorRect" property. Similar to WPF's Canvas control.
* Grid - Arranges content into rows and columns. Similar to WPF's grid control, but without row-/col-span support.
* DockPanel - Docks content to the side of the panel. Same as the WPF class with the same name.
* BoxGroup - Arrange child content horizontally or vertically. Same as the StackPanel in WPF.

## Downloads

Steropes-UI is available on NuGet. 

The NuGet packages are compiled against MonoGame 3.6, but do _not_ contain 
dependencies for the MonoGame, you will need to add a reference to 
MonoGame 3.6 to your projects in addition to Steropes-UI. 

The code compiles fine against MonoGame 3.5 if needed.

Steropes-UI is split into a platform independend core library that only depends on the
public Monogame API and a Windows library that contains additional, platform dependent 
services.

## Windows support library

If you are on Windows, consider adding Steropes.UI.Windows as library. This assembly 
contains Windows clipboard support and enables localized keyboard input by translating 
the Monogame/XNA scan-codes into their equivalent characters based on the currently 
active keyboard-layout. 

Monogame does something similiar for the Key-typed events that are produced by the 
````GameWindow.TextInput```` event, but does not translate the normal ``Keys`` struct.

Similar APIs to translate hardware based scan-codes into KeyCodes that match the user's
currently selected keyboard layout should exist for Linux and Mac-OS as well. If you
are familiar with the relevant APIs on these systems, I would love to hear from you!

## Documentation

You can find all documentation in the [documentation folder](docs/README.md).

## Roadmap

I created Steropes-UI to solve my reoccuring pains of writing testable UIs for
my Monogame related projects. I also suffer from ongoing laziness when styling UIs (especially
prototypes). Therefore the current state of the library solves these two pain points with 
the testable architecture and the styling system.

There are a few areas I think that need additional work at a later stage:

### Performance

* At the moment, the rendering relies on having separate textures and the box-textures must be
 rendered with a texturemode of stretch. This prevents frames from having repeating patterns.
 To fix that, we need a custom shader that properly implements the repeated rendering.

* Having separate textures means we end up with a large number of draw calls. That is generally
  a bad thing. GUI textures are small, so we should be able to put all of them into a texture
  atlas and render subsets of the same texture instead. 

  There are two complementary ways to implement that:

  1) Support texture coordinates in the style descriptions. A variant of that: Support 
     a parsed texture atlas and resolve textures against that. This should make the affair
     more usable, as the texture atlas contains readable names for each contained sprite.

  2) Pack textures at runtime. This should be relatively cheap given the small sizes and
     fairly regular structures involved. This would mean that images can be kept as separate
     files and wont need a recompile to see changes.

  Bonus points for mapping the referenced spritefonts onto the same sprite-sheet. Under ideal
  conditions that could bring down the draw call count to 1.

* Add tree and table widgets for data centric UIs. Hardcore strategy games would benefit from that.

* Add a rich-text widget that is suitable for dispaying help texts. 

* Add basic charts (pie, bar, line). Those would also be useful for strategy games. These could be
  implemented right now using the CustomViewport widget, but having something out of the box would be
  much nicer.

* Add basic data binding. Thats here because I am way to lazy to manually and repeatedly write mostly
  similar listener code over and over again just to get data in and out of the UI. 