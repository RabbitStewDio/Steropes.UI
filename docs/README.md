# Documentation

``Steropes-UI`` is a platform independent UI library for MonoGame. The core library
in ``Steropes.UI`` and the tests in Steropes.UI.Test only uses standard MonoGame APIs 
and do not assume any particlar runtime environment or rendering system. 

There are a few optional, Windows specific classes in ``Steropes.UI.Windows`` to 
access the system clipboard and for localizing incoming KeyStrokes based on the 
system's current keyboard schema. 

For a quick overview over the major components have a look at the ``Steropes.UI.Demo``
project. The MonoGame/NuGet package is not able to express or platform specific 
dependencies.If you are not using Windows, you will have to switch the MonoGame 
dependency to a suitable other MonoGame 3.6 implementation for your target 
platform. 

The test project requires a MonoGame framework library to run. Any library that
contains a valid implementation will do, as the tests do not attempt to instantiate 
a Game or GraphicsContext. The tests do require the API implementations for the
various structs and other structural classes to run correctly. The tests will
not run with the portable version of MonoGame as that version is bare of any
implementation code.

The Steropes-UI documentation is split into two easily accessible documents.
  
* [Concepts guide](concepts.md)
 
  Reading through the concepts will help you understand how the various larger 
  elements of the Steropes-UI system fit together. 

* [Style creation guide](style.md)
 
  This will teach you how to define style rules either in code or as XML file.
  At some point you probably want to create your own, unique theme for your
  Steropes allows styles to be external to the code to ease maintenace and to
  allow designers to tweak the style without going through a full recompile 
  cycle.
  
   
The ``Steropes.UI.Quickstart`` project contains a lightweight version of the demo
project and can be used as a template for new projects and directly copied into 
your own project.