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
      Content = new Grid(UIStyle)
      {
        Spacing = 5,
        Columns = new []
        {
          LengthConstraint.Auto, 
          LengthConstraint.Relative(1), 
          LengthConstraint.Relative(1), 
        },
        Children = new []
        {
          new [] {
            new Label(UIStyle, "PasswordBox"),
            CreatePasswordBox()
          },
          new [] {
            new Label(UIStyle, "Text-Box"),
            CreateTextBox()
          },
          new [] {
            new Label(UIStyle, "DropDown-Box"),
            CreateDropBox()
          },
          new [] {
            new Label(UIStyle, "RadioButtonSet"),
            CreateRadioButtonSet()
          },
          new [] {
            new Label(UIStyle, "Slider"),
            CreateSlider()
          },
          new [] {
            new Label(UIStyle, "Button"),
            CreateButton()
          },
          new [] {
            new Label(UIStyle, "Checkbox"),
            CreateCheckBox()
          },
          new [] {
            new Label(UIStyle, "Keybox"),
            CreateKeyBox()
          },
          new [] {
            new Label(UIStyle, "Spinner"),
            CreateSpinner()
          },
          new [] {
            new Label(UIStyle, "Progressbar"),
            CreateProgressBar()
          },
          new [] {
            new Label(UIStyle, "ListBox"),
            CreateListView()
          },
        }
      };
    }

    enum Flavor
    {
      Chocolate,

      Vanilla,

      Cheese
    }

    IWidget CreateButton()
    {
      IPopUp popup = null;
      var button = new Button(UIStyle, "Get Ice Cream!")
      {
        OnActionPerformed = (s, a) =>
        {
          popup?.Close();
          popup = Screen?.PopUpManager?.ShowConfirmDialog(new Point(600, 250), UIStyle, "Oh noes!",
                                                          "It melted already. Sorry.");
        }
      };
      return button;
    }

    IWidget CreateCheckBox()
    {
      return new CheckBox(UIStyle) { Text = "Click here" };
    }

    IWidget CreateDropBox()
    {
      return new DropDownBox<DropDownItem<Flavor>>(UIStyle)
      {
        SelectedIndex = 0,
        Data = new[]
        {
          new DropDownItem<Flavor>("Chocolate", Flavor.Chocolate),
          new DropDownItem<Flavor>("Vanilla", Flavor.Vanilla),
          new DropDownItem<Flavor>("Cheese", Flavor.Cheese),
          new DropDownItem<Flavor>("Cheese 1", Flavor.Cheese),
          new DropDownItem<Flavor>("Cheese 2", Flavor.Cheese),
          new DropDownItem<Flavor>("Cheese 3", Flavor.Cheese),
          new DropDownItem<Flavor>("Cheese 4", Flavor.Cheese),
          new DropDownItem<Flavor>("Cheese 5", Flavor.Cheese),
          new DropDownItem<Flavor>("Cheese 6", Flavor.Cheese),
          new DropDownItem<Flavor>("Cheese 7", Flavor.Cheese),
          new DropDownItem<Flavor>("Cheese 8", Flavor.Cheese),
          new DropDownItem<Flavor>("Cheese 9", Flavor.Cheese)
        }
      };
    }

    IWidget CreateKeyBox()
    {
      return new KeyBox(UIStyle, new KeyStroke(Keys.F1, InputFlags.Control | InputFlags.Shift));
    }

    IWidget CreateListView()
    {
      return new ListView<string>(UIStyle)
      {
        Data = new[]
        {
          "One",
          "Two",
          "Three",
          "Four",
          "Five",
          "Six",
          "Seven",
        }
      };
    }

    IWidget CreatePasswordBox()
    {
      return new PasswordBox(UIStyle) { Text = "secret!" };
    }

    IWidget CreateProgressBar()
    {
      return new ProgressBar(UIStyle) { Min = 0, Max = 400, Value = 200 };
    }

    IWidget CreateRadioButtonSet()
    {
      return new RadioButtonSet<DropDownItem<Flavor>>(UIStyle)
      {
        Data = new[]
        {
          new DropDownItem<Flavor>("Chocolate", Flavor.Chocolate),
          new DropDownItem<Flavor>("Vanilla", Flavor.Vanilla),
          new DropDownItem<Flavor>("Cheese", Flavor.Cheese)
        },
        SelectedIndex = 0
      };
    }

    IWidget CreateSlider()
    {
      var slider = new Slider(UIStyle, 1, 5, 1, 1);
      slider.OnValueChanged = (sender, args) => slider.ShowTooltip(slider.Value.ToString(CultureInfo.CurrentCulture));
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