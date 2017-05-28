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
using System.Collections.ObjectModel;

using Microsoft.Xna.Framework;

using Steropes.UI.Components;
using Steropes.UI.Components.Window;
using Steropes.UI.Widgets.Container;

namespace Steropes.UI.Widgets
{
  public static class DropDownBox
  {
    public const string DropDownArrowStyleClass = "DropDownArrow";

    public const string DropDownBoxListStyleClass = "DropDownBoxList";
  }

  /// <summary>
  ///   Structure:
  ///   Button
  ///   DockPanel
  ///   Image   (right)
  ///   content (left, stretch)
  ///   ListRenderer ( = Label)
  ///   ListView
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class DropDownBox<T> : InternalContentWidget<IWidget>
    where T : class
  {
    readonly ContentWidget<IWidget> dropDownButtonContent;

    readonly ListView<T> dropDownContainer;

    IPopUp<ListView<T>> dropDownPopUp;

    bool ignoreClickEventsForThisFrame;

    bool isOpen;

    public DropDownBox(IUIStyle style, IEnumerable<T> items) : base(style)
    {
      dropDownButtonContent = new ContentWidget<IWidget>(UIStyle);

      dropDownContainer = new ListView<T>(UIStyle) { Tag = "DropDownContainer" };

      // UIStyle.Style.DropdownBoxStyle.DropDownListStyle
      dropDownContainer.AddStyleClass(DropDownBox.DropDownBoxListStyleClass);
      dropDownContainer.AddAll(items);
      dropDownContainer.SelectionChanged += (s, e) =>
        {
          UpdateLabelText();
          SelectionChanged?.Invoke(this, new SelectionEventArgs<T>(SelectedIndex, SelectedItem));
          if (!e.Adjusting)
          {
            IsOpen = false;
          }
        };

      InternalContent = CreateDropDownButton();
    }

    public event EventHandler<SelectionEventArgs<T>> SelectionChanged;

    public Func<IUIStyle, T, IListDataItemRenderer> CreateRenderer
    {
      get
      {
        return dropDownContainer.CreateRenderer;
      }
      set
      {
        dropDownContainer.CreateRenderer = value;
      }
    }

    public bool IsOpen
    {
      get
      {
        return isOpen;
      }
      private set
      {
        isOpen = value;
        if (value)
        {
          dropDownPopUp = Screen?.PopUpManager?.CreatePopup(new Point(BorderRect.X, BorderRect.Bottom), dropDownContainer);
          if (dropDownPopUp != null)
          {
            dropDownPopUp.Closed += (source, args) =>
              {
                dropDownPopUp.Content = null;
                dropDownPopUp = null;
                IsOpen = false;
              };
          }
        }
        else
        {
          dropDownPopUp?.Close();
          dropDownPopUp = null;
        }
        InvalidateLayout();
        OnPropertyChanged();
      }
    }

    public ObservableCollection<T> Items => dropDownContainer.DataItems;

    public int SelectedIndex
    {
      get
      {
        return dropDownContainer.SelectedIndex;
      }
      set
      {
        dropDownContainer.SelectedIndex = value;
      }
    }

    public T SelectedItem
    {
      get
      {
        return dropDownContainer.SelectedItem;
      }
      set
      {
        dropDownContainer.SelectedItem = value;
      }
    }

    DropDownButton CreateDropDownButton()
    {
      var dropDownImage = new Image(UIStyle);
      dropDownImage.AddStyleClass(DropDownBox.DropDownArrowStyleClass);

      var buttonContent = new DockPanel(UIStyle) { LastChildFill = true, Enabled = false };

      buttonContent.Add(dropDownImage, DockPanelConstraint.Right);
      buttonContent.Add(dropDownButtonContent, DockPanelConstraint.Left);

      var dropDownButton = new DropDownButton(UIStyle);
      dropDownButton.Content = buttonContent;
      dropDownButton.ActionPerformed += ToggleDropDownPopup;
      dropDownButton.MouseDown += (s, e) =>
        {
          e.Consume();
          if (IsOpen)
          {
            ignoreClickEventsForThisFrame = true;
          }
        };
      dropDownButton.MouseUp += (s, e) =>
        {
          ignoreClickEventsForThisFrame = false;
          e.Consume();
        };
      return dropDownButton;
    }

    void ToggleDropDownPopup(object sender, EventArgs args)
    {
      if (ignoreClickEventsForThisFrame)
      {
        return;
      }

      IsOpen = !IsOpen;
    }

    void UpdateLabelText()
    {
      var selectedItem = SelectedItem;
      var itemRenderer = dropDownContainer.CreateRenderer(UIStyle, selectedItem);
      itemRenderer.Enabled = false;
      dropDownButtonContent.Content = itemRenderer;
    }

    protected override Rectangle ArrangeOverride(Rectangle layoutSize)
    {
      var rectangle = base.ArrangeOverride(layoutSize);
      dropDownContainer.MinimumWidth = rectangle.Width;
      return rectangle;
    }

    class DropDownButton : ButtonBase<DockPanel>
    {
      public DropDownButton(IUIStyle style) : base(style)
      {
      }
    }
  }
}