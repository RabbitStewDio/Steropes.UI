using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Steropes.UI.Annotations;
using Steropes.UI.Styles;
using Steropes.UI.Widgets;
using Steropes.UI.Widgets.Container;
using Steropes.UI.Widgets.Styles;

namespace Steropes.UI.Test.Styles
{
  [NUnit.Framework.Category("Styles")]
  public class TestTest
  {
    void DefineDocumentationStyleExample()
    {
      var uiStyle = LayoutTestStyle.Create();
      var b = new StyleBuilder(uiStyle.StyleSystem);

      var widgetStyles = b.StyleSystem.StylesFor<WidgetStyleDefinition>();
      var textStyles = b.StyleSystem.StylesFor<TextStyleDefinition>();
      var rules =
        b.CreateRule(
          b.SelectForType(nameof(Button), nameof(Label))
            .WithCondition(
              StyleBuilderExtensions.HasAttribute(nameof(Label.Text))
                .And(StyleBuilderExtensions.HasId("hovered"))
                .And(StyleBuilderExtensions.HasClass("StyleClass"))
            )
            .WithDescendent(b.SelectForType(nameof(DockPanel))),
          b.CreateStyle()
            .WithValue(widgetStyles.FrameOverlayColor, new Color(0x4f, 0x4f, 0x4f))
            .WithValue(widgetStyles.Color, new Color(0x6f, 0x6f, 0x6f))
            .WithValue(textStyles.TextColor, new Color(0xe0, 0xe0, 0xe0)));
    }
  }


}