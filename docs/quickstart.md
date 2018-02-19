## Quick Start

### Prerequisites
1. **Create or copy a style definition and provide sprite-fonts and textures for the components.**
  
   The quickstart project contains a minimalistic set of styles and textures in the 
   "Content/UI/Metro" directory. The metro-style uses only a handful of standard 
   shapes as textures. This makes it particularily easy to make any custom widgets 
   you create look similar to the base widgets without having to worry to much about
   how to make the widgets look right. 
  
   The style.xml file contains the actual style rules for the UI. Include the file
   into the project and set the 'BuildAction' to 'Content' to include the file in
   the build directory when the project is built.

   Later, when your code is functional and you are ready to style the UI, you can
   hand over a build along with the style.xml file to a graphics designer to work
   on how the UI should look.

2. **Have a working MonoGame project.**

   Normally using any of the existing Monogame project templates will be enough to
   get you started here. If you are starting from scatch, consider using the quickstart
   project as template. 

   The UI system will load all referenced textures and sprite fonts as MonoGame content
   resources via a ``ContentManager`` reference given to it. 

### Project set-up

Steropes.UI can be added to any existing MonoGame project. The UI libary requires
style rules, fonts and textures to display the actual user interface. Place all your
textures and SpriteFont definitions that you want to use in your UI into your normal 
Content directory, so that the MonoGame Content Pipeline tool can create the XNA content 
files from it.

Additionally you will need a style definition. This tells the UI system which fonts
and textures to select when computing the layour and when rendering the content.

Styles are defined in an XML file and must be loaded during the set up phase of your
game or application. Please refer to the [style file format documentation](style.md) 
for further details. The quickstart project ships with an Metro-UI inspired UI that
can be used freely.

### Initialize the UI

The UI system hooks into the MonoGame system via the XNA component system.
During the set up phase of your game, you can create and register the neccessary
components via 

      Game game;
      string contentLoaderPath = "Content";
      IUIManager uiManager = UIManagerComponent.CreateAndInit(game, new InputManager(game), contentLoaderPath).Manager;


This call registers two components: 

1. The InputManager, which handles the polling of all input sources. This class 
   is responsible for creating an stream of input events for the UI manager.

   The input manager should run early in the game loop and must be run before
   the UIManager component so that inputs are ready when the UI manager processes
   the event queues.

   If your game uses keyboard navigation or focusable components in the UI (for 
   instance text fields) you should process events from within the UI to avoid
   conflicts between the user interface actions and your game's controls. This 
   avoids situations where a user types text into a text field and where those
   key presses are then also interpreted as game commands.

   Use either the IFocusManager explicitly, make the UIManager's IRootPane focusable 
   or add an InputPlaceholder into your UI to manage which part of your game 
   is allowed to processes inputs.

2. The UIManagerComponent is responsible for processing events, updating the
   UI state and performing the layouting and rendering of the UI. This component
   must be run after the input manager executes.

   By default the UIManagerComponent sets a Draw- and UpdateOrder of 100000 to
   ensure it is one of the last components to run.

The IUIManager is the central object with which you can configure and run the 
UI system. This interface grants access to the root widget where you can add
UI components, windows and popups and the screen services, which allows you to
manage the focus, game window and all active popup windows. 

Once you have an UIManager, you can configure the UI widgets you want to display.
The root component of the UI will always span the whole screen. All widgets need
access to the UIStyle object of the UIManager. The UIStyle holds the style rules
and style resolvers that are necessary to render the user interface correctly.

Compoents are added to the screen's root component.

    uiManager.Root.Content = new Group(styleSystem)
    {
        new Label(styleSystem)
        {
            Text = "Hello World",
            Anchor = AnchoredRect.CreateCentered()
        }
    };

Like other UI systems, Steropes.UI has two classes of widgets. Content widgets
are responsible for actually displaying content, like text or images, in the UI.
Container widgets like the Group class in the example above are used to combine
and arrange multiple widgets into larger structures.

See the [Concepts documentation](concepts.md) and the Steropes.UI.Demo project 
for more information and sample code that shows a wide range of components and
their usage.
