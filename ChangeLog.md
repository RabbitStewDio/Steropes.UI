# Changelog

2017-10-16 - 1.1.0

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

2017-05-28 - 1.0.0

    Initial release


