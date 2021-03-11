# Changelog

Test

## 2018-05-25 - 2.1.0

    [API] Grid container now honor the Anchor property of all its child widgets.
          This allows you to set a preferred size and alignment on the widgets
          without having to wrap them into a GroupBox element.

    [API] Consolidated the tracing code into two tracing targets. 
          "Steropes.UI.Input" records all input events. If this source is 
          enabled at the 'Verbose' level, the system will automatically add the
          tracers as input filters.
          "Steropes.UI.Style" records all style related events.

    [API] A new IInputState component in the IScreenService allows you to track
          the current mouse position and input flags globally. 

    [API] Mouse and Touch inputs can now be transformed using an Affine-Transform
          matrix. That allows you to scale, translate and rotate the inputs before
          they are processed by the widgets. Use it in combination with transforms
          on the SpriteBatch in the BatchedDrawingService to scale the UI.

    [API] Group and BoxGroup now have a write-only 'Children' property to support
          declaring child widgets in the fluent API.

    [API] The ProgressBar can now change rendering direction from Left-To-Right to 
          Right-To-Left and can request a minimum size based on the value range
          covered.

## 2018-02-18 - 2.0.0

    [API] The project is now built with C# 7 features to reduce some clutter in
          the code. 
          
    [API] BatchDrawingService now allows you to override how the spritebatch 
          instance is configured.
          
    [API] Changed the return type of ``UIManagerComponent.Create`` to return 
          the component instead of the UI manager. The manager is accessible as 
          property on the component, and having a reference to the component means 
          you can change the rendering order if needed.
    
    [API] ``GameState.Draw`` now supplies the current gametime object to make
          it easier to delegate the call to standard XNA code if needed.
    
    [API] Animated values now implement the IAnimatedValue interface for easier
          mocking in tests. 
          
    [API] Added a more fluent GUI definition API. Most properties and behaviours 
          can now be set via the object initialization syntax, and where that 
          fails (ie for widgets that are both collections and property holders),
          we now supply a ``DoWith(Action<Widget> a)`` extension method. 
          
          A standard event handler can now be set via an property to make it easier
          to use the object intializer syntax for simple cases. The standard event
          handling methods continue to exist as well, in case you need more than 
          one handler.  
          
          List, RadioButtonSet and Dropdown now have Data property that can be used 
          in an object initializer. Grid has a new Columns, Rows and Children property 
          for similar object initialization.
          
    [API] Added a debug mode printer that prints the widget tree along with its
          layout state (DesiredSize, LayoutRect and AnchorRect). This makes it 
          easier to see where the layout may go wrong and is more fun to deal with
          than the Visual Studio Debugger.      
          
    [API] Added better event notifications when widgets are added or removed.
          The ``Widget.ChildrenChanged`` event now also supplies the index of the
          new widget and the widget constraints used for the widget.
          
          Widgets that are part of the overlay section (mainly tooltips) report
          these changes with an index of -1. 
                
    [Bug] Animations did not handle zero values gracefully and thus could easily
          die from DIV/0 exceptions.      
    
    [Bug] BatchDrawingService used the wrong texture mode. When working with 
          textures that are clamped any mode that evaluates neighboring pixels
          (like the anisotropic or linear texture sampler modes) can pull in
          pixels from unrelated textures. This problem gets even worse when using
          a texture atlas. We now use the safer ``SamplerState.PointWrap`` as 
          default mode.            

    [Bug] The layouting phase can fail with a NPE when you try to display a GUI 
          without intializing the style system. We now explicitly check for an empty
          style rule set and provide a more targetted error message in that case.

    [Bug] TextWidget now only rebuilds the text document if there are real changes
          to the text content. The ``Text`` property now fires a PropertyChanged
          event when the document changes. 
     
    [Bug] SplitterPane did not correctly clip its contents when the Show/Hide animation
          was running. It looked ugly.
 
    [New] Added data binding code. This code is pretty independent of the widget
          classes and relies soley on INotifyPropertyChanged and INotifyCollectionChanged
          mechanics. You can find the [Data Binding](docs/databinding.md) Documentation
          in the ``docs`` folder.

    [New] The rendering and update order of the UIManager component is now set 
          to 10000 to execute after all user components.

    [New] Added a new container component. The ``LayeredPane`` uses the root-pane
          behaviour and provides an easy API to control the rendering order of 
          its children. 

    [New] Added support for building an texture atlas from smaller sources at 
          runtime. GUI component textures are normally scattered in small texture
          files and (especially during early development) it is not fun baking them 
          into a texture atlas during build time. The new texture atlas builder
          takes all loaded textures and transparently combines them into a single 
          texture atlas at runtime to reduce the number of draw calls issued.  

## 2017-10-16 - 1.1.0

    [Bug] Non-consumed KeyEvents were passed on to parent components, which made it 
          ugly to limit event handling to just one component without adding a lot
          of dummy handlers to all potential keyboard-event receivers.

          Keyboard and Gamepad events are now only delivered to the focused component
          and are no longer sent down the parent chain if the local component does not
          handle them.

    [Bug] TextFields and other text based components computed a size of (0,0) when
          there was no text set. Although mathematically correct, it breaks the
          expectation that an empty text-field should have at least enough height to 
          contain a single line of text. This in return makes sure that dynamic 
          layouting reserves enough space when the text-field eventually receives
          input and avoid visible and ugly relayouting issues.

    [Bug] Monogame sends control characters as part of the GameWindow.TextInput
          event stream. This confuses the text input classes. We now filter out
          control characters for all text inputs. TextAreas now only accept Tab
          and linebreak characters as control inputs.

    [API] Made Widget#PerformHitTest virtual so that subclasses can override the
          hittesting behaviour if they choose to. 

    [API] Added AnchoredRect#CreateSizeConstraint(width, height) to make it clearer
          to see the intent of the AnchoredRect use on some widget container childs.

## 2017-05-28 - 1.0.0

    Initial release


