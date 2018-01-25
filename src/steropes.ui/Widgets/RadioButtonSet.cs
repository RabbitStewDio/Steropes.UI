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
using System.Collections.Specialized;
using System.Linq;
using Steropes.UI.Components;
using Steropes.UI.Util;
using Steropes.UI.Widgets.Container;

namespace Steropes.UI.Widgets
{
  public class RadioButtonSet<T> : InternalContentWidget<BoxGroup>
  {
    readonly EventSupport<ListSelectionEventArgs> selectionChangedSupport;
    bool itemsUpdating;

    int selectedButtonIndex;

    public RadioButtonSet(IUIStyle style, IEnumerable<T> items = null) : base(style)
    {
      selectionChangedSupport = new EventSupport<ListSelectionEventArgs>();
      InternalContent = new BoxGroup(UIStyle) { Orientation = Orientation.Horizontal };
      Renderer = DefaultRenderFunction;

      DataItems = new ObservableCollection<T>();
      if (items != null)
      {
        DataItems.AddRange(items);
      }
      DataItems.CollectionChanged += OnItemsChanged;
    }

    public event EventHandler<ListSelectionEventArgs> SelectionChanged
    {
      add { selectionChangedSupport.Event += value; }
      remove { selectionChangedSupport.Event -= value; }
    }

    public EventHandler<ListSelectionEventArgs> OnSelectionChanged
    {
      get { return selectionChangedSupport.Handler; }
      set { selectionChangedSupport.Handler = value; }
    }

    public ObservableCollection<T> DataItems { get; }

    public IReadOnlyCollection<T> Data
    {
      get { return DataItems.ToArray(); }
      set
      {
        DataItems.Clear();
        DataItems.AddRange(value);
      }
    }

    public Orientation Orientation
    {
      get
      {
        return InternalContent.Orientation;
      }
      set
      {
        InternalContent.Orientation = value;
      }
    }

    public int Spacing
    {
      get
      {
        return InternalContent.Spacing;
      }
      set
      {
        InternalContent.Spacing = value;
      }
    }

    public Func<RadioButtonSet<T>, T, IRadioButtonSetContent> Renderer { get; set; }

    public int SelectedIndex
    {
      get
      {
        return selectedButtonIndex;
      }

      set
      {
        if (selectedButtonIndex == value)
        {
          return;
        }
        GetButtonAt(selectedButtonIndex).Selected = SelectionState.Unselected;
        selectedButtonIndex = value;
        GetButtonAt(selectedButtonIndex).Selected = SelectionState.Selected;
        selectionChangedSupport.Raise(this, new ListSelectionEventArgs(false));
      }
    }

    public void AddAll(IEnumerable<T> items)
    {
      itemsUpdating = true;
      try
      {
        foreach (var item in items)
        {
          DataItems.Add(item);
        }
      }
      finally
      {
        itemsUpdating = false;
      }

      RebuildDataRenderers();
      InvalidateLayout();
    }

    public bool LookUpSelectedItem(out T item)
    {
      var index = selectedButtonIndex;
      if (index < 0 || index >= DataItems.Count)
      {
        item = default(T);
        return false;
      }
      item = DataItems[index];
      return true;
    }

    protected static IRadioButtonSetContent DefaultRenderFunction(RadioButtonSet<T> me, T value)
    {
      var text = $"{value}";
      return new RadioButtonSetContent(me.UIStyle, text);
    }

    IRadioButtonSetContent GetButtonAt(int index)
    {
      if (index < 0 || index >= InternalContent.Count)
      {
        return null;
      }
      return InternalContent[index] as IRadioButtonSetContent;
    }

    void OnItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      if (itemsUpdating)
      {
        return;
      }
      RebuildDataRenderers();
    }

    void RebuildDataRenderers()
    {
      // rebuild buttons ..
      InternalContent.Clear();
      for (var index = 0; index < DataItems.Count; index++)
      {
        var fixedIndex = index;
        var item = DataItems[index];
        var content = Renderer(this, item);
        content.Last = index == DataItems.Count - 1;
        content.First = index == 0;
        content.Selected = index == selectedButtonIndex ? SelectionState.Selected : SelectionState.Unselected;
        content.ActionPerformed += (s, arg) => SelectedIndex = fixedIndex;
        InternalContent.Add(content);
      }
    }
  }
}