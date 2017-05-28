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

TODO
  
### Initialize the UI

TODO

See the [Concepts documentation](concepts.md) for more information.
