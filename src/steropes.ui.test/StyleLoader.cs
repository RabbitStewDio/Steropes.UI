// MIT License
//
// Copyright (c) 2011-2016 Elisée Maurer, Sparklin Labs, Creative Patterns
// Copyright (c) 2016 Thomas Morgner, Rabbit-StewDio Ltd.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Steropes.UI.Components;
using Steropes.UI.Components.Window;
using Steropes.UI.Platform;
using Steropes.UI.Styles;
using Steropes.UI.Widgets;
using Steropes.UI.Widgets.Container;
using Steropes.UI.Widgets.Styles;
using Steropes.UI.Widgets.TextWidgets;

namespace Steropes.UI.Test
{
  /// <summary>
  ///   The style loader is a hard-coded version of a default style-sheet for testing purposes.
  ///   If you create new widget types for the main library, add some sensible default styles here too.
  ///   You can always override them later via specialized style rules if you need changes for any given test.
  /// </summary>
  public class StyleLoader
  {
    public StyleLoader()
    {
    }

    public virtual IEnumerable<IStyleRule> LoadRules(IStyleSystem style)
    {
      var b = new StyleBuilder(style);
      var rules = new List<IStyleRule>();
      rules.AddRange(CreateStyleFor(b));
      return rules;
    }

    protected virtual IEnumerable<IStyleRule> CreateStyleFor(StyleBuilder b)
    {
      var rules = new List<IStyleRule>();
      rules.AddRange(CreateStyleForAll(b));
      rules.AddRange(CreateStyleForButton(b));
      rules.AddRange(CreateStyleForNotebook(b));
      rules.AddRange(CreateStyleForProgressBar(b));
      rules.AddRange(CreateStyleForRadioButtons(b));
      rules.AddRange(CreateStyleForTooltip(b));
      rules.AddRange(CreateStyleForTextField(b));
      rules.AddRange(CreateStyleForPasswordField(b));
      rules.AddRange(CreateStyleForSpinningWheel(b));
      rules.AddRange(CreateStyleForSlider(b));
      rules.AddRange(CreateStyleForListView(b));
      rules.AddRange(CreateStyleForLabel(b));
      rules.AddRange(CreateStyleForIconLabel(b));
      rules.AddRange(CreateStyleForDropDown(b));
      rules.AddRange(CreateStyleForScrollbar(b));
      rules.AddRange(CreateStyleForOptionPane(b));
      rules.AddRange(CreateStyleForTextArea(b));
      rules.AddRange(CreateStyleForCheckBox(b));
      rules.AddRange(CreateStyleForGlassPane(b));
      rules.AddRange(CreateStyleForSplitter(b));
      return rules;
    }

    protected virtual IEnumerable<IStyleRule> CreateStyleForAll(StyleBuilder b)
    {
      var widgetStyles = b.StyleSystem.StylesFor<WidgetStyleDefinition>();
      return new List<IStyleRule>();
    }

    protected virtual IEnumerable<IStyleRule> CreateStyleForButton(StyleBuilder b)
    {
      var widgetStyles = b.StyleSystem.StylesFor<WidgetStyleDefinition>();
      var textStyles = b.StyleSystem.StylesFor<TextStyleDefinition>();

      return new List<IStyleRule>
               {
                 b.CreateRule(
                   b.SelectForType<Button>(),
                   b.CreateStyle()
                     .WithValue(widgetStyles.Padding, new Insets(20, 20))
                     .WithValue(textStyles.Alignment, Alignment.Center)
                     .WithValue(textStyles.TextColor, new Color(224, 224, 224))
                     .WithBox(widgetStyles.FrameTexture, "UI/Button/ButtonFrame", new Insets(20))),
                 b.CreateRule(
                   b.SelectForType<Button>().WithDirectChild(b.SelectForType<IconLabel>()).WithDirectChild(b.SelectForType<Label>()),
                   b.CreateStyle().WithValue(textStyles.Alignment, Alignment.Center)),
                 b.CreateRule(
                   b.SelectForType<Button>().WithDirectChild(b.SelectForType<IconLabel>()),
                   b.CreateStyle().WithValue(widgetStyles.Padding, new Insets(0))),
                 b.CreateRule(
                   b.SelectForType<Button>().WithCondition(StyleBuilderExtensions.HasPseudoClass(WidgetPseudoClasses.HoveredPseudoClass)),
                   b.CreateStyle().WithBox(widgetStyles.HoverOverlayTexture, "UI/Button/ButtonHover", new Insets(20))),
                 b.CreateRule(
                   b.SelectForType<Button>().WithCondition(StyleBuilderExtensions.HasPseudoClass(WidgetPseudoClasses.FocusedPseudoClass)),
                   b.CreateStyle().WithBox(widgetStyles.FocusedOverlayTexture, "UI/Button/ButtonFocus", new Insets(20))),
                 b.CreateRule(
                   b.SelectForType<Button>().WithCondition(StyleBuilderExtensions.HasPseudoClass(ButtonPseudoClasses.DownPseudoClass)),
                   b.CreateStyle()
                     .WithBox(widgetStyles.FrameTexture, "UI/Button/ButtonFrameDown", new Insets(20))
                     .WithBox(widgetStyles.FrameOverlayTexture, "UI/Button/ButtonPress", new Insets(20)))
               };
    }

    protected virtual IEnumerable<IStyleRule> CreateStyleForCheckBox(StyleBuilder b)
    {
      var widgetStyles = b.StyleSystem.StylesFor<WidgetStyleDefinition>();
      var buttonStyles = b.StyleSystem.StylesFor<ButtonStyleDefinition>();
      var iconLabelStyles = b.StyleSystem.StylesFor<IconLabelStyleDefinition>();

      return new List<IStyleRule>
               {
                 b.CreateRule(b.SelectForType<CheckBox>(), b.CreateStyle().WithValue(widgetStyles.Padding, new Insets(10, 15))),
                 b.CreateRule(
                   b.SelectForType<CheckBox>()
                     .WithDescendent(b.SelectForType<Label>().WithCondition(StyleBuilderExtensions.HasClass(CheckBox.CheckBoxLabelStyleClass))),
                   b.CreateStyle().WithValue(iconLabelStyles.IconTextGap, 10)),
                 b.CreateRule(
                   b.SelectForType<CheckBox>().WithDescendent(b.SelectForType<Button>()),
                   b.CreateStyle()
                     .WithValue(widgetStyles.Padding, new Insets(20))
                     .WithBox(widgetStyles.FrameTexture, "UI/CheckBox/CheckBoxFrame", new Insets(20))),
                 b.CreateRule(
                   b.SelectForType<CheckBox>()
                     .WithDescendent(b.SelectForType<Button>().WithCondition(StyleBuilderExtensions.HasPseudoClass(WidgetPseudoClasses.FocusedPseudoClass))),
                   b.CreateStyle().WithBox(widgetStyles.FocusedOverlayTexture, "UI/CheckBox/CheckBoxFrameFocus", new Insets(20))),
                 b.CreateRule(
                   b.SelectForType<CheckBox>()
                     .WithDescendent(b.SelectForType<Button>().WithCondition(StyleBuilderExtensions.HasPseudoClass(WidgetPseudoClasses.HoveredPseudoClass))),
                   b.CreateStyle().WithBox(widgetStyles.HoverOverlayTexture, "UI/CheckBox/CheckBoxFrameHover", new Insets(20))),
                 b.CreateRule(
                   b.SelectForType<CheckBox>()
                     .WithDescendent(
                       b.SelectForType<Button>().WithCondition(StyleBuilderExtensions.HasAttribute(nameof(Button.Selected), SelectionState.Selected))),
                   b.CreateStyle().WithBox(widgetStyles.WidgetStateOverlay, "UI/CheckBox/Checked", new Insets(20))),
                 b.CreateRule(
                   b.SelectForType<CheckBox>()
                     .WithDescendent(
                       b.SelectForType<Button>().WithCondition(StyleBuilderExtensions.HasAttribute(nameof(Button.Selected), SelectionState.Unselected))),
                   b.CreateStyle().WithBox(widgetStyles.WidgetStateOverlay, "UI/CheckBox/Unchecked", new Insets(20))),
                 b.CreateRule(
                   b.SelectForType<CheckBox>()
                     .WithDescendent(
                       b.SelectForType<Button>().WithCondition(StyleBuilderExtensions.HasAttribute(nameof(Button.Selected), SelectionState.Indeterminate))),
                   b.CreateStyle().WithBox(widgetStyles.WidgetStateOverlay, "UI/CheckBox/Indeterminate", new Insets(20)))
               };
    }

    protected virtual IEnumerable<IStyleRule> CreateStyleForDropDown(StyleBuilder b)
    {
      var imageStyles = b.StyleSystem.StylesFor<ImageStyleDefinition>();
      var listViewStyles = b.StyleSystem.StylesFor<ListViewStyleDefinition>();

      return new List<IStyleRule>
               {
                 b.CreateRule(
                   b.SelectForType<Image>().WithCondition(StyleBuilderExtensions.HasClass(DropDownBox.DropDownArrowStyleClass)),
                   b.CreateStyle().WithTexture(imageStyles.Texture, "UI/DropDown/DropDownArrow")),
                 b.CreateRule(
                   b.SelectForType("ListView").WithCondition(StyleBuilderExtensions.HasClass(DropDownBox.DropDownBoxListStyleClass)),
                   b.CreateStyle().WithValue(listViewStyles.MaxHeight, 300))
               };
    }

    protected virtual IEnumerable<IStyleRule> CreateStyleForGlassPane(StyleBuilder b)
    {
      var widgetStyles = b.StyleSystem.StylesFor<WidgetStyleDefinition>();
      var textStyles = b.StyleSystem.StylesFor<TextStyleDefinition>();

      return new List<IStyleRule>
               {
                 b.CreateRule(
                   b.SelectForType<GlassPane>(),
                   b.CreateStyle().WithValue(widgetStyles.Color, Color.White * 0.8f).WithFont(textStyles.Font, "Fonts/MediumFont"))
               };
    }

    protected virtual IEnumerable<IStyleRule> CreateStyleForIconLabel(StyleBuilder b)
    {
      var widgetStyles = b.StyleSystem.StylesFor<WidgetStyleDefinition>();
      var iconLabelStyles = b.StyleSystem.StylesFor<IconLabelStyleDefinition>();

      return new List<IStyleRule>
               {
                 b.CreateRule(
                   b.SelectForType<IconLabel>().WithDirectChild(b.SelectForType<Label>()),
                   b.CreateStyle().WithValue(widgetStyles.Padding, new Insets(0))),
                 b.CreateRule(
                   b.SelectForType<IconLabel>().WithDirectChild(b.SelectForType<Image>()),
                   b.CreateStyle().WithValue(widgetStyles.Padding, new Insets(0))),
                 b.CreateRule(
                   b.SelectForType<IconLabel>(),
                   b.CreateStyle().WithValue(widgetStyles.Padding, new Insets(10)).WithValue(iconLabelStyles.IconTextGap, 10))
               };
    }

    protected virtual IEnumerable<IStyleRule> CreateStyleForLabel(StyleBuilder b)
    {
      var widgetStyles = b.StyleSystem.StylesFor<WidgetStyleDefinition>();
      var textStyles = b.StyleSystem.StylesFor<TextStyleDefinition>();

      return new List<IStyleRule>
               {
                 b.CreateRule(
                   b.SelectForType<Label>(),
                   b.CreateStyle()
                     .WithValue(textStyles.TextColor, new Color(224, 224, 224))
                     .WithValue(widgetStyles.Padding, new Insets(10))
                     .WithValue(textStyles.Alignment, Alignment.Start)
                     .WithFont(textStyles.Font, "Fonts/MediumFont"))
               };
    }

    protected virtual IEnumerable<IStyleRule> CreateStyleForListView(StyleBuilder b)
    {
      var widgetStyles = b.StyleSystem.StylesFor<WidgetStyleDefinition>();

      return new List<IStyleRule>
               {
                 b.CreateRule(
                   b.SelectForType("ListView").WithDirectChild(b.SelectForType("Panel")),
                   b.CreateStyle().WithBox(widgetStyles.FrameTexture, "UI/ListView/ListFrame", new Insets(10)).WithValue(widgetStyles.Padding, new Insets(10))),
                 b.CreateRule(
                   b.SelectForType<ListDataItemRenderer>(),
                   b.CreateStyle().WithBox(widgetStyles.FrameTexture, "UI/ListView/ListRowFrame", new Insets(10)).WithValue(widgetStyles.Padding, new Insets(10))),
                 b.CreateRule(
                   b.SelectForType<ListDataItemRenderer>().WithCondition(StyleBuilderExtensions.HasAttribute(nameof(IListDataItemRenderer.Selected), true)),
                   b.CreateStyle()
                     .WithBox(widgetStyles.FrameTexture, "UI/ListView/ListRowFrameSelected", new Insets(10))
                     .WithValue(widgetStyles.Padding, new Insets(10))),
                 b.CreateRule(
                   b.SelectForType<ListDataItemRenderer>().WithCondition(StyleBuilderExtensions.HasPseudoClass(WidgetPseudoClasses.FocusedPseudoClass)),
                   b.CreateStyle().WithBox(widgetStyles.FocusedOverlayTexture, "UI/ListView/ListRowFrameFocused", new Insets(10))),
                 b.CreateRule(
                   b.SelectForType<ListDataItemRenderer>().WithCondition(StyleBuilderExtensions.HasPseudoClass(WidgetPseudoClasses.HoveredPseudoClass)),
                   b.CreateStyle().WithBox(widgetStyles.HoverOverlayTexture, "UI/ListView/ListRowFrameHover", new Insets(10)))
               };
    }

    protected virtual IEnumerable<IStyleRule> CreateStyleForNotebook(StyleBuilder b)
    {
      var widgetStyles = b.StyleSystem.StylesFor<WidgetStyleDefinition>();
      var notebookStyles = b.StyleSystem.StylesFor<NotebookStyleDefinition>();

      return new List<IStyleRule>
               {
                 b.CreateRule(b.SelectForType<Notebook>(), b.CreateStyle().WithValue(notebookStyles.NotebookTabOverlapY, 15)),
                 b.CreateRule(
                   b.SelectForType<Notebook>().WithDirectChild(b.SelectForType<ScrollPanel>()),
                   b.CreateStyle().WithBox(widgetStyles.FrameTexture, "UI/Notebook/NotebookFrame", new Insets(15)).WithValue(notebookStyles.NotebookTabOverlapY, 5)),
                 b.CreateRule(
                   b.SelectForType<NotebookTabList>(),
                   b.CreateStyle().WithValue(widgetStyles.Padding, new Insets(0, 20, 0, 20)).WithValue(notebookStyles.NotebookTabOverlapX, 15)),
                 b.CreateRule(
                   b.SelectForType<NotebookTab>(),
                   b.CreateStyle()
                     .WithValue(widgetStyles.Padding, new Insets(0, 20, 0, 20))
                     .WithBox(widgetStyles.FrameTexture, "UI/Notebook/Tab", new Insets(15))
                     .WithBox(widgetStyles.HoverOverlayTexture, "UI/Notebook/TabHover", new Insets(15))),
                 b.CreateRule(
                   b.SelectForType<NotebookTab>().WithCondition(StyleBuilderExtensions.HasAttribute(nameof(NotebookTab.IsActive), "true")),
                   b.CreateStyle().WithBox(widgetStyles.FrameTexture, "UI/Notebook/ActiveTab", new Insets(15))),
                 b.CreateRule(
                   b.SelectForType<NotebookTab>().WithCondition(StyleBuilderExtensions.HasPseudoClass(WidgetPseudoClasses.FocusedPseudoClass)),
                   b.CreateStyle().WithBox(widgetStyles.FocusedOverlayTexture, "UI/Notebook/TabFocus", new Insets(15))),
                 b.CreateRule(
                   b.SelectForType<NotebookTab>()
                     .WithCondition(
                       StyleBuilderExtensions.HasAttribute(nameof(NotebookTab.IsActive), "true")
                         .And(StyleBuilderExtensions.HasPseudoClass(WidgetPseudoClasses.FocusedPseudoClass))),
                   b.CreateStyle().WithBox(widgetStyles.FocusedOverlayTexture, "UI/Notebook/ActiveTabFocused", new Insets(15))),
                 b.CreateRule(
                   b.SelectForType<Button>().WithCondition(StyleBuilderExtensions.HasClass(NotebookTab.CloseButtonStyleClass)),
                   b.CreateStyle()
                     .WithValue(widgetStyles.Padding, new Insets(0, 20, 0, 20))
                     .WithBox(widgetStyles.FrameTexture, "UI/Notebook/TabClose", new Insets(15))),
                 b.CreateRule(
                   b.SelectForType<Button>()
                     .WithCondition(
                       StyleBuilderExtensions.HasClass(NotebookTab.CloseButtonStyleClass)
                         .And(StyleBuilderExtensions.HasPseudoClass(WidgetPseudoClasses.HoveredPseudoClass))),
                   b.CreateStyle()
                     .WithValue(widgetStyles.Padding, new Insets(0, 20, 0, 20))
                     .WithBox(widgetStyles.HoverOverlayTexture, "UI/Notebook/TabCloseHover", new Insets(15))),
                 b.CreateRule(
                   b.SelectForType<Button>()
                     .WithCondition(
                       StyleBuilderExtensions.HasClass(NotebookTab.CloseButtonStyleClass)
                         .And(StyleBuilderExtensions.HasPseudoClass(ButtonPseudoClasses.DownPseudoClass))),
                   b.CreateStyle()
                     .WithValue(widgetStyles.Padding, new Insets(0, 20, 0, 20))
                     .WithBox(widgetStyles.HoverOverlayTexture, "UI/Notebook/TabCloseDown", new Insets(15)))
               };
    }

    protected virtual IEnumerable<IStyleRule> CreateStyleForOptionPane(StyleBuilder b)
    {
      var widgetStyles = b.StyleSystem.StylesFor<WidgetStyleDefinition>();

      return new List<IStyleRule>
               {
                 b.CreateRule(
                   b.SelectForType("OptionPane"),
                   b.CreateStyle().WithBox(widgetStyles.FrameTexture, "UI/OptionPane/PopupFrame", new Insets(30)).WithValue(widgetStyles.Padding, new Insets(30)))
               };
    }

    protected virtual IEnumerable<IStyleRule> CreateStyleForPasswordField(StyleBuilder b)
    {
      var widgetStyles = b.StyleSystem.StylesFor<WidgetStyleDefinition>();
      var textStyles = b.StyleSystem.StylesFor<TextStyleDefinition>();

      return new List<IStyleRule>
               {
                 b.CreateRule(
                   b.SelectForType<PasswordBox>(),
                   b.CreateStyle()
                     .WithBox(widgetStyles.FrameTexture, "UI/TextField/TextFieldFrame", new Insets(20))
                     .WithValue(widgetStyles.Padding, new Insets(15))
                     .WithFont(textStyles.Font, "Fonts/MediumFont")
                     .WithValue(textStyles.TextColor, Color.White)),
                 b.CreateRule(
                   b.SelectForType<PasswordBox>().WithDirectChild(b.SelectForType("Caret")),
                   b.CreateStyle()
                     .WithValue(widgetStyles.Visibility, Visibility.Collapsed)
                     .WithValue(widgetStyles.Color, Color.White)
                     .WithValue(textStyles.SelectionColor, Color.Gray)
                     .WithValue(textStyles.CaretWidth, 1)),
                 b.CreateRule(
                   b.SelectForType<PasswordBox>()
                     .WithCondition(StyleBuilderExtensions.HasPseudoClass(WidgetPseudoClasses.FocusedPseudoClass))
                     .WithDirectChild(b.SelectForType("Caret")),
                   b.CreateStyle().WithValue(widgetStyles.Visibility, Visibility.Visible))
               };
    }

    protected virtual IEnumerable<IStyleRule> CreateStyleForProgressBar(StyleBuilder b)
    {
      var widgetStyles = b.StyleSystem.StylesFor<WidgetStyleDefinition>();

      return new List<IStyleRule>
               {
                 b.CreateRule(
                   b.SelectForType<ProgressBar>(),
                   b.CreateStyle()
                     .WithBox(widgetStyles.FrameTexture, "UI/ProgressBar/ProgressBarFrame", new Insets(15))
                     .WithBox(widgetStyles.FrameOverlayTexture, "UI/ProgressBar/Progressbar", new Insets(15)))
               };
    }

    protected virtual IEnumerable<IStyleRule> CreateStyleForRadioButtons(StyleBuilder b)
    {
      var widgetStyles = b.StyleSystem.StylesFor<WidgetStyleDefinition>();
      var textStyles = b.StyleSystem.StylesFor<TextStyleDefinition>();

      return new List<IStyleRule>
               {
                 b.CreateRule(b.SelectForType("RadioButtonSet"), b.CreateStyle().WithValue(widgetStyles.Padding, new Insets())),
                 b.CreateRule(
                   b.SelectForType<Button>().WithCondition(StyleBuilderExtensions.HasClass(RadioButtonSetContent.StyleClass)),
                   b.CreateStyle()
                     .WithBox(widgetStyles.FrameTexture, "UI/RadioButtonSet/ButtonFrameMiddle", new Insets(20))
                     .WithBox(widgetStyles.HoverOverlayTexture, null)
                     .WithValue(textStyles.TextColor, Color.White)),
                 b.CreateRule(
                   b.SelectForType<Button>()
                     .WithCondition(
                       StyleBuilderExtensions.HasPseudoClass(WidgetPseudoClasses.FirstChildPseudoClass)
                         .And(StyleBuilderExtensions.HasClass(RadioButtonSetContent.StyleClass))),
                   b.CreateStyle()
                     .WithBox(widgetStyles.FrameTexture, "UI/RadioButtonSet/ButtonFrameLeft", new Insets(20))
                     .WithBox(widgetStyles.HoverOverlayTexture, null)),
                 b.CreateRule(
                   b.SelectForType<Button>()
                     .WithCondition(
                       StyleBuilderExtensions.HasPseudoClass(WidgetPseudoClasses.LastChildPseudoClass)
                         .And(StyleBuilderExtensions.HasClass(RadioButtonSetContent.StyleClass))),
                   b.CreateStyle()
                     .WithBox(widgetStyles.FrameTexture, "UI/RadioButtonSet/ButtonFrameRight", new Insets(20))
                     .WithBox(widgetStyles.HoverOverlayTexture, null)),
                 b.CreateRule(
                   b.SelectForType<Button>()
                     .WithCondition(
                       StyleBuilderExtensions.HasClass(RadioButtonSetContent.StyleClass)
                         .And(StyleBuilderExtensions.HasPseudoClass(ButtonPseudoClasses.DownPseudoClass))),
                   b.CreateStyle()
                     .WithBox(widgetStyles.FrameTexture, "UI/RadioButtonSet/ButtonFrameMiddleDown", new Insets(20))
                     .WithValue(textStyles.TextColor, Color.Black)),
                 b.CreateRule(
                   b.SelectForType<Button>()
                     .WithCondition(
                       StyleBuilderExtensions.HasPseudoClass(WidgetPseudoClasses.FirstChildPseudoClass)
                         .And(StyleBuilderExtensions.HasClass(RadioButtonSetContent.StyleClass))
                         .And(StyleBuilderExtensions.HasPseudoClass(ButtonPseudoClasses.DownPseudoClass))),
                   b.CreateStyle().WithBox(widgetStyles.FrameTexture, "UI/RadioButtonSet/ButtonFrameLeftDown", new Insets(20))),
                 b.CreateRule(
                   b.SelectForType<Button>()
                     .WithCondition(
                       StyleBuilderExtensions.HasPseudoClass(WidgetPseudoClasses.LastChildPseudoClass)
                         .And(StyleBuilderExtensions.HasClass(RadioButtonSetContent.StyleClass))
                         .And(StyleBuilderExtensions.HasPseudoClass(ButtonPseudoClasses.DownPseudoClass))),
                   b.CreateStyle().WithBox(widgetStyles.FrameTexture, "UI/RadioButtonSet/ButtonFrameRightDown", new Insets(20)))
               };
    }

    protected virtual IEnumerable<IStyleRule> CreateStyleForScrollbar(StyleBuilder b)
    {
      var scrollbarStyles = b.StyleSystem.StylesFor<ScrollbarStyleDefinition>();
      var widgetStyles = b.StyleSystem.StylesFor<WidgetStyleDefinition>();

      return new List<IStyleRule>
               {
                 b.CreateRule(
                   b.SelectForType<Scrollbar>(),
                   b.CreateStyle()
                     .WithValue(scrollbarStyles.ScrollbarMode, ScrollbarMode.Auto)
                     .WithValue(widgetStyles.Color, Color.Gray)),
                 b.CreateRule(
                   b.SelectForType<ScrollbarThumb>(),
                   b.CreateStyle()
                     .WithValue(widgetStyles.Padding, new Insets(5))),
               };
    }

    protected virtual IEnumerable<IStyleRule> CreateStyleForSlider(StyleBuilder b)
    {
      var widgetStyles = b.StyleSystem.StylesFor<WidgetStyleDefinition>();

      return new List<IStyleRule>
               {
                 b.CreateRule(
                   b.SelectForType<Slider>()
                     .WithDirectChild(b.SelectForType<Button>().WithCondition(StyleBuilderExtensions.HasClass(Slider.SliderHandleStyleClass))),
                   b.CreateStyle().WithValue(widgetStyles.Padding, new Insets(15))),
                 b.CreateRule(b.SelectForType<Slider>(), b.CreateStyle().WithBox(widgetStyles.FrameTexture, "UI/Slider/SliderFrame", new Insets(10)))
               };
    }

    protected virtual IEnumerable<IStyleRule> CreateStyleForSpinningWheel(StyleBuilder b)
    {
      var imageStyles = b.StyleSystem.StylesFor<ImageStyleDefinition>();

      return new List<IStyleRule>
               {
                 b.CreateRule(
                   b.SelectForType<SpinningWheel>(),
                   b.CreateStyle().WithTexture(imageStyles.Texture, "UI/SpinningWheel/SpinningWheel").WithValue(imageStyles.TextureScale, ScaleMode.None)),
                 b.CreateRule(
                   b.SelectForType<SpinningWheel>().WithCondition(StyleBuilderExtensions.HasClass("Small")),
                   b.CreateStyle().WithTexture(imageStyles.Texture, "UI/SpinningWheel/SmallSpinningWheel").WithValue(imageStyles.TextureScale, ScaleMode.None))
               };
    }

    protected virtual IEnumerable<IStyleRule> CreateStyleForSplitter(StyleBuilder b)
    {
      var widgetStyles = b.StyleSystem.StylesFor<WidgetStyleDefinition>();

      return new List<IStyleRule>
               {
                 b.CreateRule(
                   b.SelectForType<Splitter>()
                     .WithCondition(StyleBuilderExtensions.HasAttribute(nameof(Splitter.IsDragging), true))
                     .WithDirectChild(b.SelectForType<SplitterBar>()),
                   b.CreateStyle().WithValue(widgetStyles.Color, Color.White)),
                 b.CreateRule(
                   b.SelectForType<SplitterBar>(),
                   b.CreateStyle()
                     .WithBox(widgetStyles.FrameOverlayTexture, "UI/Splitter/SplitterDragHandle")
                     .WithBox(widgetStyles.FrameTexture, "UI/Splitter/SplitterFrame", new Insets(5))
                     .WithValue(widgetStyles.Padding, new Insets(5))
                     .WithValue(widgetStyles.Color, Color.White)),
                 b.CreateRule(
                   b.SelectForType<SplitterBar>().WithCondition(StyleBuilderExtensions.HasAttribute(nameof(SplitterBar.Collapsable), true)),
                   b.CreateStyle().WithBox(widgetStyles.FrameOverlayTexture, "UI/Splitter/SplitterCollapseArrow"))
               };
    }

    protected virtual IEnumerable<IStyleRule> CreateStyleForTextArea(StyleBuilder b)
    {
      var widgetStyles = b.StyleSystem.StylesFor<WidgetStyleDefinition>();
      var textStyles = b.StyleSystem.StylesFor<TextStyleDefinition>();

      return new List<IStyleRule>
               {
                 b.CreateRule(
                   b.SelectForType<TextArea>(),
                   b.CreateStyle()
                     .WithBox(widgetStyles.FrameTexture, "UI/TextArea/TextAreaFrame", new Insets(15))
                     .WithValue(widgetStyles.Padding, new Insets(20))
                     .WithValue(textStyles.TextColor, new Color(224, 224, 224))
                     .WithFont(textStyles.Font, "Fonts/MediumMonoFont")),
                 b.CreateRule(
                   b.SelectForType<TextArea>().WithDirectChild(b.SelectForType("Caret")),
                   b.CreateStyle()
                     .WithValue(widgetStyles.Visibility, Visibility.Collapsed)
                     .WithValue(widgetStyles.Color, Color.White)
                     .WithValue(textStyles.SelectionColor, Color.Gray)
                     .WithValue(textStyles.CaretWidth, 1)),
                 b.CreateRule(
                   b.SelectForType<TextArea>()
                     .WithCondition(StyleBuilderExtensions.HasPseudoClass(WidgetPseudoClasses.FocusedPseudoClass))
                     .WithDirectChild(b.SelectForType("Caret")),
                   b.CreateStyle().WithValue(widgetStyles.Visibility, Visibility.Visible)),
                 b.CreateRule(
                   b.SelectForType<LineNumberWidget>(),
                   b.CreateStyle()
                     .WithBox(widgetStyles.FrameTexture, "UI/TextArea/TextAreaGutterFrame", new Insets(15))
                     .WithValue(widgetStyles.Padding, new Insets(15))
                     .WithValue(textStyles.TextColor, Color.White)
                     .WithFont(textStyles.Font, "Fonts/MediumMonoFont"))
               };
    }

    protected virtual IEnumerable<IStyleRule> CreateStyleForTextField(StyleBuilder b)
    {
      var widgetStyles = b.StyleSystem.StylesFor<WidgetStyleDefinition>();
      var textStyles = b.StyleSystem.StylesFor<TextStyleDefinition>();

      return new List<IStyleRule>
               {
                 b.CreateRule(
                   b.SelectForType<TextField>(),
                   b.CreateStyle()
                     .WithBox(widgetStyles.FrameTexture, "UI/TextField/TextFieldFrame", new Insets(20))
                     .WithValue(widgetStyles.Padding, new Insets(15))
                     .WithFont(textStyles.Font, "Fonts/MediumFont")
                     .WithValue(textStyles.TextColor, Color.White)),
                 b.CreateRule(
                   b.SelectForType<TextField>().WithDirectChild(b.SelectForType("Caret")),
                   b.CreateStyle()
                     .WithValue(widgetStyles.Visibility, Visibility.Collapsed)
                     .WithValue(widgetStyles.Color, Color.White)
                     .WithValue(textStyles.SelectionColor, Color.Gray)
                     .WithValue(textStyles.CaretWidth, 1)),
                 b.CreateRule(
                   b.SelectForType<TextField>()
                     .WithCondition(StyleBuilderExtensions.HasPseudoClass(WidgetPseudoClasses.FocusedPseudoClass))
                     .WithDirectChild(b.SelectForType("Caret")),
                   b.CreateStyle().WithValue(widgetStyles.Visibility, Visibility.Visible))
               };
    }

    protected virtual IEnumerable<IStyleRule> CreateStyleForTooltip(StyleBuilder b)
    {
      var widgetStyles = b.StyleSystem.StylesFor<WidgetStyleDefinition>();

      return new List<IStyleRule>
               {
                 b.CreateRule(
                   b.SelectForType("Tooltip"),
                   b.CreateStyle().WithBox(widgetStyles.FrameTexture, "UI/Tooltip/TooltipFrame", new Insets(20)).WithValue(widgetStyles.Padding, new Insets(10)))
               };
    }

    IEnumerable<IStyleRule> CreateStyleForTemplate(StyleBuilder b)
    {
      var widgetStyles = b.StyleSystem.StylesFor<WidgetStyleDefinition>();

      return new List<IStyleRule>();
    }
  }
}