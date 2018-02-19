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
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Steropes.UI.Annotations;
using Steropes.UI.Bindings;
using Steropes.UI.Components;
using Steropes.UI.Components.Window;
using Steropes.UI.Input;
using Steropes.UI.Input.KeyboardInput;
using Steropes.UI.Widgets;
using Steropes.UI.Widgets.Container;
using Steropes.UI.Widgets.TextWidgets;

namespace Steropes.UI.Demo.Demos
{
  internal enum Flavor
  {
    Chocolate,

    Vanilla,

    Cheese,

    Chilli,

    Strawberry,

    Honey,

    Lemon,

    Raspberry
  }

  internal class BasicDemoPane : ScrollPanel
  {
    readonly BasicDemoModel model;

    public BasicDemoPane(IUIStyle style) : base(style)
    {
      model = new BasicDemoModel();

      Content = new Grid(UIStyle)
      {
        Spacing = 5,
        Columns = new[]
        {
          LengthConstraint.Auto,
          LengthConstraint.Relative(1),
          LengthConstraint.Relative(1)
        },
        Children = new[]
        {
          new[]
          {
            new Label(UIStyle, "PasswordBox"),
            CreatePasswordBox()
          },
          new[]
          {
            new Label(UIStyle, "Text-Box"),
            CreateTextBox()
          },
          new[]
          {
            new Label(UIStyle, "DropDown-Box"),
            CreateDropBox()
          },
          new[]
          {
            new Label(UIStyle, "RadioButtonSet"),
            CreateRadioButtonSet()
          },
          new[]
          {
            new Label(UIStyle, "Slider"),
            CreateSlider()
          },
          new[]
          {
            new Label(UIStyle, "Button"),
            CreateButton()
          },
          new[]
          {
            new Label(UIStyle, "Checkbox"),
            CreateCheckBox()
          },
          new[]
          {
            new Label(UIStyle, "Keybox"),
            CreateKeyBox()
          },
          new[]
          {
            new Label(UIStyle, "Spinner"),
            CreateSpinner()
          },
          new[]
          {
            new Label(UIStyle, "Progressbar"),
            CreateProgressBar()
          },
          new[]
          {
            new Label(UIStyle, "ListBox"),
            CreateListView()
          }
        }
      };
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
      return new CheckBox(UIStyle)
      {
        Text = "Add Extra Chocolate Flakes"
      }.DoWith(c =>
      {
        model.BindingFor(m => m.ExtraFlakes)
          .Map(SelectionState.Selected, SelectionState.Unselected)
          .BindTo((b) => c.Selected = b);
        c.BindingFor(cb => cb.Selected)
          .EqualTo(SelectionState.Selected)
          .BindTo(b => model.ExtraFlakes = b);
      });
    }

    IWidget CreateDropBox()
    {
      return new DropDownBox<Flavor>(UIStyle)
      {
        SelectedIndex = 0,
        Data = new[]
        {
          Flavor.Chocolate,
          Flavor.Vanilla,
          Flavor.Cheese,
          Flavor.Chilli,
          Flavor.Strawberry,
          Flavor.Honey,
          Flavor.Lemon,
          Flavor.Raspberry
        }
      }.DoWith(db =>
      {
        model.BindingFor(m => m.Flavor)
          .BindTo((b) => db.SelectedItem = b);
        db.BindingFor(cb => cb.SelectedItem)
          .BindTo(b => model.Flavor = b);
      });
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
          "Seven"
        }
      }.DoWith(lv =>
      {
        model.BindingFor(m => m.Count).Subtract(1).BindTo(v => lv.SelectedIndex = v);
        lv.BindingFor(l => l.SelectedIndex).Add(1).BindTo(m => model.Count = m);
      });
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
          new DropDownItem<Flavor>("Cheese", Flavor.Cheese),
          new DropDownItem<Flavor>("Chilli", Flavor.Chilli),
          new DropDownItem<Flavor>("Strawberry", Flavor.Strawberry),
          new DropDownItem<Flavor>("Honey", Flavor.Honey),
          new DropDownItem<Flavor>("Lemon", Flavor.Lemon),
          new DropDownItem<Flavor>("Raspberry", Flavor.Raspberry)
        },
        SelectedIndex = 0
      }.DoWith(db =>
      {
        model.BindingFor(m => m.Flavor).Map(b => db.DataItems.FirstOrDefault(f => f.Tag == b)).BindTo(b => db.SelectedItem = b);
        db.BindingFor(cb => cb.SelectedItem).Map(d => d.Tag).BindTo(b => model.Flavor = b);
      });
    }

    IWidget CreateSlider()
    {
      var slider = new Slider(UIStyle)
      {
        MinValue = 1,
        MaxValue = 7,
        Step = 1,
        Value = 1
      };
      slider.OnValueChanged = (sender, args) => slider.ShowTooltip(slider.Value.ToString(CultureInfo.CurrentCulture));
      
      model.BindingFor(m => m.Count).BindTo(v => slider.Value = v);
      slider.BindingFor(s => s.Value).Map(f => (int) f).BindTo(v => model.Count = v);
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

  internal class BasicDemoModel : INotifyPropertyChanged
  {
    int count;
    bool extraFlakes;
    Flavor flavor;

    public BasicDemoModel()
    {
      count = 2;
      flavor = Flavor.Chocolate;
    }

    public bool ExtraFlakes
    {
      get { return extraFlakes; }
      set
      {
        if (value == extraFlakes)
        {
          return;
        }

        extraFlakes = value;
        OnPropertyChanged();
        Console.WriteLine("Extra Flakes changed to " + value);
      }
    }

    public Flavor Flavor
    {
      get { return flavor; }
      set
      {
        if (value == flavor)
        {
          return;
        }

        flavor = value;
        OnPropertyChanged();
        Console.WriteLine("Flavour changed to " + value);
      }
    }

    public int Count
    {
      get { return count; }
      set
      {
        if (value == count)
        {
          return;
        }

        count = value;
        OnPropertyChanged();
        Console.WriteLine("Count changed to " + value);
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}