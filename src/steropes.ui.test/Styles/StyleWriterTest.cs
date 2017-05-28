// MIT License
// Copyright (c) 2011-2016 Elisée Maurer, Sparklin Labs, Creative Patterns
// Copyright (c) 2016 Thomas Morgner, Rabbit-StewDio Ltd.
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

using FluentAssertions;
using NUnit.Framework;

using Steropes.UI.Platform;
using Steropes.UI.Styles;
using Steropes.UI.Styles.Io.Writer;
using Steropes.UI.Widgets.Styles;

namespace Steropes.UI.Test.Styles
{
  [Category("Styles")]
  public class StyleWriterTest
  {
    [Test]
    public void ReparseStandard()
    {
      var uiStyle = LayoutTestStyle.Create();
      var styleWriter = uiStyle.StyleSystem.CreateWriter();
      var styles = uiStyle.StyleResolver.StyleRules;
      var document = styleWriter.Write(styles);

      var styleParser = uiStyle.StyleSystem.CreateParser();
      var styleRules = styleParser.Read(document);

      styleRules.Select(r => r.Selector).Should().Contain(styles.Select(r => r.Selector));
      styleRules.Should().BeEquivalentTo(styles);

      var l1 = new List<IStyleRule> { styleRules[0] };
      var l2 = new List<IStyleRule> { styles[0] };
      l1.Should().BeEquivalentTo(l2);
    }

    [Test]
    public void ReparseStandardSmall()
    {
      var uiStyle = LayoutTestStyle.Create();
      var styleWriter = uiStyle.StyleSystem.CreateWriter();
      var styles = uiStyle.StyleResolver.StyleRules;
      var document = styleWriter.Write(styles);

      var styleParser = uiStyle.StyleSystem.CreateParser();
      var styleRules = styleParser.Read(document);

      var r1 = styles[0];
      var r2 = styleRules[0];
      r1.Selector.Should().Be(r2.Selector);
      r1.Should().Be(r2);
    }

    [Test]
    public void Validate_Merge_With_Conditions()
    {
      var e1 = new XElement("style");
      e1.Add(new XAttribute("element", "button"));
      e1.Add(new XElement("conditions", new XElement("pseudo-class", "hovered")));
      e1.Add(new XElement("property", "value-1"));

      var e2 = new XElement("style");
      e2.Add(new XAttribute("element", "button"));
      e2.Add(new XElement("conditions", new XElement("pseudo-class", "hovered")));
      e2.Add(new XElement("property", "value-2"));

      var s1 = new StyleWriterExtensions.Selector(e1);
      var s2 = new StyleWriterExtensions.Selector(e2);

      s1.Equals(s2).Should().BeTrue();

      var list = new List<XElement> { e1, e2 };
      var result = list.Merge().ToList();
      result.Count.Should().Be(1);
      result.Elements("property").Count().Should().Be(2);
    }

    [Test]
    public void ValidateMerge()
    {
      var e1 = new XElement("style");
      e1.Add(new XAttribute("element", "button"));
      e1.Add(new XElement("property", "value-1"));

      var e2 = new XElement("style");
      e2.Add(new XAttribute("element", "button"));
      e2.Add(new XElement("property", "value-2"));

      var list = new List<XElement> { e1, e2 };
      var result = list.Merge().ToList();
      result.Count.Should().Be(1);
      result.Elements("property").Count().Should().Be(2);
    }

    [Test]
    public void ValidatePropertyParsing()
    {
      var e1 = new XElement("style");
      e1.Add(new XAttribute("element", "button"));
      e1.Add(new XElement("property", new XAttribute("name", "frame-texture"), new XElement("texture", "UI/Button/ButtonFrame")));

      var d = new XDocument(new XElement("styles", e1));

      var style = LayoutTestStyle.Create();
      var styleParser = style.StyleSystem.CreateParser();
      var styleRules = styleParser.Read(d);
      var text = styleRules[0].Style.GetValue(style.StyleSystem.StylesFor<WidgetStyleDefinition>().FrameTexture);
      text.Should().BeAssignableTo<IBoxTexture>();
    }

    [Test]
    public void WorkingDirectory()
    {
      Console.WriteLine(TestContext.CurrentContext.WorkDirectory);
      Console.WriteLine(TestContext.CurrentContext.TestDirectory);
      Console.WriteLine(Path.GetFullPath("."));
    }

    [Test]
    public void WriteDescendantRule()
    {
      var uiStyle = LayoutTestStyle.Create();
      var b = new StyleBuilder(uiStyle.StyleSystem);

      var widgetStyles = b.StyleSystem.StylesFor<WidgetStyleDefinition>();
      var rules =
        b.CreateRule(
          b.SelectForType("Button")
            .WithCondition(StyleBuilderExtensions.HasAttribute("name", "button"))
            .WithDirectChild(b.SelectForType("IconLabel").WithCondition(StyleBuilderExtensions.HasAttribute("name", "iconlabel")))
            .WithDirectChild(b.SelectForType("Label").WithCondition(StyleBuilderExtensions.HasAttribute("name", "label"))),
          b.CreateStyle().WithBox(widgetStyles.FrameTexture, "texture-on-label"));

      var styleWriter = uiStyle.StyleSystem.CreateWriter();
      var document = styleWriter.Write(new List<IStyleRule> { rules });
      var w = new StringWriter();
      document.Save(w);
      File.WriteAllText(Path.Combine(TestContext.CurrentContext.TestDirectory, "style-descendand-rule.xml"), w.ToString());

      var p = uiStyle.StyleSystem.CreateParser();
      var parsed = p.Read(document);
      parsed.Count.Should().Be(1);
      parsed[0].Weight.Should().Be(rules.Weight);
      parsed[0].Selector.ToString().Should().Be(rules.Selector.ToString());
    }

    [Test]
    public void WriteStandard()
    {
      var uiStyle = LayoutTestStyle.Create();
      var styleWriter = uiStyle.StyleSystem.CreateWriter();
      var document = styleWriter.Write(uiStyle.StyleResolver.StyleRules);
      var w = new StringWriter();
      document.Save(w);
      Console.WriteLine(w);
      File.WriteAllText(Path.Combine(TestContext.CurrentContext.TestDirectory, "style.xml"), w.ToString());
    }

    [Test]
    public void XNode_DeepEquals()
    {
      var e1 = new XElement("style");
      e1.Add(new XAttribute("element", "button"));
      e1.Add(new XElement("property", "value-1"));

      var e2 = new XElement("style");
      e2.Add(new XAttribute("element", "button"));
      e2.Add(new XElement("property", "value-1"));

      XNode.DeepEquals(e1, e2).Should().Be(true);
    }
  }
}