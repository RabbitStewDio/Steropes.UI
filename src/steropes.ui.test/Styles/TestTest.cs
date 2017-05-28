using System;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework;
using NUnit.Framework;
using Steropes.UI.Styles;
using Steropes.UI.Widgets;
using Steropes.UI.Widgets.Container;
using Steropes.UI.Widgets.Styles;

namespace Steropes.UI.Test.Styles
{
  [Category("Styles")]
  public class TestTest
  {
    void defineDocumentationStyleExample()
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