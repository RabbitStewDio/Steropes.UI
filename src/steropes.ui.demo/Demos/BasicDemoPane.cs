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
using System.Collections.Generic;
using System.Globalization;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Steropes.UI.Components;
using Steropes.UI.Components.Window;
using Steropes.UI.Input;
using Steropes.UI.Input.KeyboardInput;
using Steropes.UI.Widgets;
using Steropes.UI.Widgets.Container;
using Steropes.UI.Widgets.TextWidgets;

namespace Steropes.UI.Demo.Demos
{
  class BasicDemoPane : ScrollPanel
  {
    public BasicDemoPane(IUIStyle style) : base(style)
    {
      var gridGroup = new Grid(UIStyle);
      gridGroup.ColumnConstraints.Add(LengthConstraint.Auto);
      gridGroup.ColumnConstraints.Add(LengthConstraint.Relative(1));
      gridGroup.ColumnConstraints.Add(LengthConstraint.Relative(1));

      var rowIndex = 0;

      gridGroup.AddChildAt(new Label(UIStyle, "PasswordBox"), 0, rowIndex);
      gridGroup.AddChildAt(CreatePasswordBox(), 1, rowIndex);

      rowIndex++;
      gridGroup.AddChildAt(new Label(UIStyle, "Text-Box"), 0, rowIndex);
      gridGroup.AddChildAt(CreateTextBox(), 1, rowIndex);

      rowIndex++;
      gridGroup.AddChildAt(new Label(UIStyle, "DropDownBox"), 0, rowIndex);
      gridGroup.AddChildAt(CreateDropBox(), 1, rowIndex);

      rowIndex++;

      gridGroup.AddChildAt(new Label(UIStyle, "RadioButtonSet"), 0, rowIndex);
      gridGroup.AddChildAt(CreateRadioButtonSet(), 1, rowIndex);

      rowIndex++;

      gridGroup.AddChildAt(new Label(UIStyle, "Slider"), 0, rowIndex);
      gridGroup.AddChildAt(CreateSlider(), 1, rowIndex);

      rowIndex++;

      gridGroup.AddChildAt(new Label(UIStyle, "Button"), 0, rowIndex);
      gridGroup.AddChildAt(CreateButton(), 1, rowIndex);

      rowIndex++;

      gridGroup.AddChildAt(new Label(UIStyle, "CheckBox"), 0, rowIndex);
      gridGroup.AddChildAt(CreateCheckBox(), 1, rowIndex);

      rowIndex++;

      gridGroup.AddChildAt(new Label(UIStyle, "KeyBox"), 0, rowIndex);
      gridGroup.AddChildAt(CreateKeyBox(), 1, rowIndex);

      rowIndex++;

      gridGroup.AddChildAt(new Label(UIStyle, "Spinner"), 0, rowIndex);
      gridGroup.AddChildAt(CreateSpinner(), 1, rowIndex);

      rowIndex++;

      gridGroup.AddChildAt(new Label(UIStyle, "ProgressBar"), 0, rowIndex);
      gridGroup.AddChildAt(CreateProgressBar(), 1, rowIndex);

      rowIndex++;

      gridGroup.AddChildAt(new Label(UIStyle, "ListBox"), 0, rowIndex);
      gridGroup.AddChildAt(CreateListView(), 1, rowIndex);

      Content = gridGroup;
    }

    enum Flavor
    {
      Chocolate,

      Vanilla,

      Cheese
    }

    IWidget CreateButton()
    {
      var button = new Button(UIStyle, "Get Ice Cream!");
      IPopUp popup = null;
      button.ActionPerformed += (sender, args) =>
        {
          popup?.Close();
          popup = Screen?.PopUpManager?.ShowConfirmDialog(new Point(600, 250), UIStyle, "Oh noes!", "It melted already. Sorry.");
        };
      return button;
    }

    IWidget CreateCheckBox()
    {
      return new CheckBox(UIStyle) { Text = "Click here" };
    }

    IWidget CreateDropBox()
    {
      var items = new List<DropDownItem<Flavor>>();
      items.Add(new DropDownItem<Flavor>("Chocolate", Flavor.Chocolate));
      items.Add(new DropDownItem<Flavor>("Vanilla", Flavor.Vanilla));
      items.Add(new DropDownItem<Flavor>("Cheese", Flavor.Cheese));
      items.Add(new DropDownItem<Flavor>("Cheese 1", Flavor.Cheese));
      items.Add(new DropDownItem<Flavor>("Cheese 2", Flavor.Cheese));
      items.Add(new DropDownItem<Flavor>("Cheese 3", Flavor.Cheese));
      items.Add(new DropDownItem<Flavor>("Cheese 4", Flavor.Cheese));
      items.Add(new DropDownItem<Flavor>("Cheese 5", Flavor.Cheese));
      items.Add(new DropDownItem<Flavor>("Cheese 6", Flavor.Cheese));
      items.Add(new DropDownItem<Flavor>("Cheese 7", Flavor.Cheese));
      items.Add(new DropDownItem<Flavor>("Cheese 8", Flavor.Cheese));
      items.Add(new DropDownItem<Flavor>("Cheese 9", Flavor.Cheese));

      var dropDownBox = new DropDownBox<DropDownItem<Flavor>>(UIStyle, items);
      dropDownBox.SelectedIndex = 0;
      return dropDownBox;
    }

    IWidget CreateKeyBox()
    {
      return new KeyBox(UIStyle, new KeyStroke(Keys.F1, InputFlags.Control | InputFlags.Shift));
    }

    IWidget CreateListView()
    {
      var list = new ListView<string>(UIStyle);
      list.DataItems.Add("One");
      list.DataItems.Add("Two");
      list.DataItems.Add("Three");
      list.DataItems.Add("Four");
      list.DataItems.Add("Five");
      list.DataItems.Add("Five A");
      list.DataItems.Add("Five B");
      return list;
    }

    IWidget CreatePasswordBox()
    {
      return new PasswordBox(UIStyle) { Text = "secret!" };
    }

    IWidget CreateProgressBar()
    {
      var progressBar = new ProgressBar(UIStyle) { Min = 0, Max = 400, Value = 200 };
      return progressBar;
    }

    IWidget CreateRadioButtonSet()
    {
      var items = new List<DropDownItem<Flavor>>();
      items.Add(new DropDownItem<Flavor>("Chocolate", Flavor.Chocolate));
      items.Add(new DropDownItem<Flavor>("Vanilla", Flavor.Vanilla));
      items.Add(new DropDownItem<Flavor>("Cheese", Flavor.Cheese));

      var dropDownBox = new RadioButtonSet<DropDownItem<Flavor>>(UIStyle);
      dropDownBox.AddAll(items);
      dropDownBox.SelectedIndex = 0;
      return dropDownBox;
    }

    IWidget CreateSlider()
    {
      var slider = new Slider(UIStyle, 1, 5, 1, 1);
      slider.ValueChanged += (sender, args) => slider.ShowTooltip(slider.Value.ToString(CultureInfo.CurrentCulture));
      return slider;
    }

    IWidget CreateSpinner()
    {
      return new SpinningWheel(UIStyle);
    }

    IWidget CreateTextBox()
    {
      return new TextField(UIStyle) { Text = "  Hello World! " };
    }
  }
}