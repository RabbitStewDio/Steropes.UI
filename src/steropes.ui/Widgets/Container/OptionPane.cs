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
using System.Diagnostics.CodeAnalysis;

using Microsoft.Xna.Framework;

using Steropes.UI.Components;
using Steropes.UI.Components.Window;
using Steropes.UI.I18N;
using Steropes.UI.Platform;

namespace Steropes.UI.Widgets.Container
{
  public class OptionPane<TOptionContent> : InternalContentWidget<DockPanel>
    where TOptionContent : IWidget
  {
    readonly Grid buttonContainer;

    readonly Label dummyContent;

    TOptionContent optionContent;

    public OptionPane(IUIStyle style) : base(style)
    {
      TitleLabel = new Label(UIStyle);

      dummyContent = new Label(UIStyle);

      buttonContainer = new Grid(UIStyle);
      buttonContainer.RowConstraints.Add(LengthConstraint.Auto);

      InternalContent = new DockPanel(UIStyle);
      InternalContent.LastChildFill = true;
      InternalContent.Add(TitleLabel, DockPanelConstraint.Top);
      InternalContent.Add(buttonContainer, DockPanelConstraint.Bottom);
      InternalContent.Add(dummyContent, DockPanelConstraint.Left);
    }

    public event EventHandler<OptionPaneActionArgs> ActionPerformed;

    public TOptionContent OptionContent
    {
      get
      {
        return optionContent;
      }

      set
      {
        if (Equals(value, optionContent))
        {
          return;
        }

        if (optionContent != null)
        {
          InternalContent.Remove(optionContent);
        }
        else
        {
          InternalContent.Remove(dummyContent);
        }

        optionContent = value;
        if (optionContent != null)
        {
          InternalContent.Add(optionContent, DockPanelConstraint.Left);
        }
        else
        {
          InternalContent.Add(dummyContent, DockPanelConstraint.Left);
        }

        InvalidateLayout();
        OnPropertyChanged();
      }
    }

    public Label TitleLabel { get; }

    public void ConfigureButtons(OptionPane.Buttons buttonStyle)
    {
      buttonContainer.Clear();
      buttonContainer.ColumnConstraints.Clear();

      CreateButtonFor(buttonStyle, OptionPane.Buttons.Ok, Common.OK);
      CreateButtonFor(buttonStyle, OptionPane.Buttons.Yes, Common.Yes);
      CreateButtonFor(buttonStyle, OptionPane.Buttons.No, Common.No);
      CreateButtonFor(buttonStyle, OptionPane.Buttons.Cancel, Common.Cancel);
    }

    void CreateButtonFor(OptionPane.Buttons flags, OptionPane.Buttons bs, string text)
    {
      if (!flags.HasFlag(bs))
      {
        return;
      }

      var b = new Button(UIStyle);
      b.Content.Label.Text = text;
      b.ActionPerformed += (s, o) => ActionPerformed?.Invoke(this, new OptionPaneActionArgs(bs));
      buttonContainer.ColumnConstraints.Add(LengthConstraint.Auto);
      buttonContainer.Add(b, new Point(buttonContainer.Count, 0));
    }
  }

  [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1204:StaticElementsMustAppearBeforeInstanceElements", Justification = "Mixing generic and non-generic class requires this.")]
  [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Mixing generic and non-generic class requires this.")]
  public static class OptionPane
  {
    [Flags]
    public enum Buttons
    {
      Ok = 1,

      Cancel = 2,

      Yes = 4,

      No = 8
    }

    public static OptionPane<IconLabel> CreateConfirmBox(IUIStyle style, string title, string message, IUITexture icon, Buttons buttonStyle, Action<Buttons> resultCallback = null)
    {
      var optionPane = new OptionPane<IconLabel>(style);
      optionPane.OptionContent = new IconLabel(style);
      optionPane.OptionContent.Label.Text = message;
      optionPane.OptionContent.Image.Texture = icon;
      optionPane.TitleLabel.Text = title;
      optionPane.ConfigureButtons(buttonStyle);
      if (resultCallback != null)
      {
        optionPane.ActionPerformed += (s, e) => resultCallback.Invoke(e.Button);
      }

      return optionPane;
    }

    public static OptionPane<IconLabel> CreateConfirmBox(IUIStyle style, string title, string message, IUITexture icon = null, Action<Buttons> resultCallback = null)
    {
      return CreateConfirmBox(style, title, message, icon, Buttons.Ok, resultCallback);
    }

    public static IPopUp<OptionPane<IconLabel>> ShowConfirmDialog(
      this IPopupManager mgr,
      Point location,
      IUIStyle style,
      string title,
      string message,
      IUITexture icon,
      Buttons buttonStyle,
      Action<Buttons> resultCallback = null)
    {
      var handler = new CloseHandler(resultCallback);
      var popup = mgr.CreatePopup(location, CreateConfirmBox(style, title, message, icon, buttonStyle, handler.OnButtonPressed));
      handler.Popup = popup;
      return popup;
    }

    public static IPopUp<OptionPane<IconLabel>> ShowConfirmDialog(
      this IPopupManager mgr,
      Point location,
      IUIStyle style,
      string title,
      string message,
      IUITexture icon = null,
      Action<Buttons> resultCallback = null)
    {
      return ShowConfirmDialog(mgr, location, style, title, message, icon, Buttons.Ok, resultCallback);
    }

    class CloseHandler
    {
      public CloseHandler(Action<Buttons> action)
      {
        Action = action;
      }

      public IPopUp Popup { private get; set; }

      Action<Buttons> Action { get; }

      public void OnButtonPressed(Buttons b)
      {
        Action?.Invoke(b);
        Popup?.Close();
      }
    }
  }
}