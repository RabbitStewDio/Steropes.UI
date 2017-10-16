# Notes

## Keyboard input

https://unix.stackexchange.com/questions/116629/how-do-keyboard-input-and-text-output-work
https://unix.stackexchange.com/questions/12510/relationship-of-keyboard-layout-and-xmodmap/12518#12518

Windows mapping code partially based on the original code in NuclearWinter,
modified with the generous help of the MSDN documentation on MapVirtualKeyEx
https://msdn.microsoft.com/en-us/library/windows/desktop/ms646307(v=vs.85).aspx


## Todo

Replace the focus management with something central. Keyboard focus is central
to keyboard driven UIs and the current code makes this very hard to achieve.
Extend the FocusManager to handle focus events as fallback handler (ie if the
widget itself did not consume the event). 

Focus cycling (via tab, shift-tab) should work out of the box. Lets model it 
after Java/Swing's model, as it behaves solidly:

- Components can elect to be focusable 
- Components can declare themselves a focus-cycle-root
- Navigation rules
  
  Using the currently focused widget as starting point, first test all 
  child widgets recursively, using a depth-first strategy. If no widget 
  has been found yet and the current widget is not a focus cycle root, 
  try again by searching the sibling widget. When the current widget is
  a focus cycle root, start searching the first child until the search
  has evaluated all possible widgets, then abort and do not move the focus.

  Sibling widgets are traversed in layout sort order. For that sort
  widgets by their y, then x coordinate. This will move the focus along
  rows and columns in the UI.