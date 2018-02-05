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
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;

using Steropes.UI.Components;
using Steropes.UI.Platform;
using Steropes.UI.Styles;
using Steropes.UI.Util;
using Steropes.UI.Widgets.Container;

namespace Steropes.UI.Widgets
{
  public interface IListDataItemRenderer : IWidget
  {
    event EventHandler<ListSelectionEventArgs> SelectionChanged;
    
    EventHandler<ListSelectionEventArgs> OnSelectionChanged { get; set; }

    bool Selected { get; set; }
  }

  public class ListViewStyleDefinition : IStyleDefinition
  {
    public IStyleKey<int> MaxHeight { get; private set; }

    public IStyleKey<int> MaxLinesVisible { get; private set; }

    public IStyleKey<int> MinHeight { get; private set; }

    public IStyleKey<bool> UniformItemHeight { get; private set; }

    public void Init(IStyleSystem s)
    {
      MinHeight = s.CreateKey<int>("min-height", false);
      MaxHeight = s.CreateKey<int>("max-height", false);
      MaxLinesVisible = s.CreateKey<int>("max-lines-visible", false);
      UniformItemHeight = s.CreateKey<bool>("uniform-item-height", false);
    }
  }

  /// <summary>
  ///   Structure:
  ///   Panel
  ///   BoxGroup
  ///   Content from renderer
  /// </summary>
  public class ListView<T> : InternalContentWidget<ScrollPanel<BoxGroup>>
    //where T : class
  {
    readonly ListViewStyleDefinition listViewStyle;

    Func<IUIStyle, T, IListDataItemRenderer> createRenderer;

    bool itemsUpdating;

    int minimumWidth;

    int selectedIndex;
    readonly EventSupport<ListSelectionEventArgs> selectionChangedSupport;

    public ListView(IUIStyle style) : base(style)
    {
      selectionChangedSupport = new EventSupport<ListSelectionEventArgs>();
      listViewStyle = StyleSystem.StylesFor<ListViewStyleDefinition>();

      selectedIndex = -1;

      InternalContent = new ScrollPanel<BoxGroup>(UIStyle) { Content = new BoxGroup(UIStyle) { Orientation = Orientation.Vertical } };

      DataItems = new ObservableCollection<T>();
      DataItems.CollectionChanged += (s, e) =>
        {
          RebuildDataRenderers();
          InvalidateLayout();
        };

      CreateRenderer = ListView.DefaultCreateRenderer;

      MinHeight = 0;
      MaxHeight = int.MaxValue;
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

    public Func<IUIStyle, T, IListDataItemRenderer> CreateRenderer
    {
      get
      {
        return createRenderer;
      }
      set
      {
        createRenderer = value;
        RebuildDataRenderers();
      }
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

    public int MaxHeight
    {
      get
      {
        return Style.GetValue(listViewStyle.MaxHeight);
      }
      set
      {
        Style.SetValue(listViewStyle.MaxHeight, value);
      }
    }

    public int MaxLinesVisible
    {
      get
      {
        return Style.GetValue(listViewStyle.MaxLinesVisible);
      }
      set
      {
        Style.SetValue(listViewStyle.MaxLinesVisible, value);
      }
    }

    public int MinHeight
    {
      get
      {
        return Style.GetValue(listViewStyle.MinHeight);
      }
      set
      {
        Style.SetValue(listViewStyle.MinHeight, value);
      }
    }

    public int MinimumWidth
    {
      get
      {
        return minimumWidth;
      }
      set
      {
        if (value == minimumWidth)
        {
          return;
        }
        
        minimumWidth = Math.Max(0, value);
        OnPropertyChanged();
        InvalidateLayout();
      }
    }

    public int SelectedIndex
    {
      get
      {
        return selectedIndex;
      }
      set
      {
        if (selectedIndex != value)
        {
          var oldItem = SelectedItem;
          for (var widgetItem = 0; widgetItem < InternalContent.Content.Count; widgetItem += 1)
          {
            if (InternalContent.Content[widgetItem] is IListDataItemRenderer r)
            {
              r.Selected = widgetItem == value;
            }
          }
          selectedIndex = value;
          OnPropertyChanged(nameof(SelectedIndex));
          if (!Equals(oldItem, SelectedItem))
          {
            OnPropertyChanged(nameof(SelectedItem));
          }
          RaiseSelectionChanged(ListSelectionEventArgs.Adjusted);
        }
      }
    }

    public T SelectedItem
    {
      get
      {
        if (selectedIndex != -1 && selectedIndex < DataItems.Count)
        {
          return DataItems[selectedIndex];
        }
        return default(T);
      }
      set
      {
        SelectedIndex = DataItems.IndexOf(value);
      }
    }

    public bool UniformItemHeight
    {
      get
      {
        return Style.GetValue(listViewStyle.UniformItemHeight);
      }
      set
      {
        Style.SetValue(listViewStyle.UniformItemHeight, value);
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

    protected override Rectangle ArrangeOverride(Rectangle layoutSize)
    {
      layoutSize.Height = MathHelper.Clamp(layoutSize.Height, MinHeight, MaxHeight);
      return base.ArrangeOverride(layoutSize);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
      var size = base.MeasureOverride(availableSize);
      return new Size(Math.Max(MinimumWidth, size.Width), MathHelper.Clamp(size.Height, MinHeight, MaxHeight));
    }

    protected void RaiseSelectionChanged(ListSelectionEventArgs args)
    {
      selectionChangedSupport.Raise(this, args);
    }

    void RebuildDataRenderers()
    {
      if (itemsUpdating)
      {
        return;
      }

      InternalContent.Content.Clear();
      for (var index = 0; index < DataItems.Count; index++)
      {
        var closureIndex = index;
        var item = DataItems[index];
        var renderer = CreateRenderer(UIStyle, item);
        renderer.MouseUp += (s, e) => InternalContent.DispatchEvent(e.Derive());
        renderer.MouseDown += (s, e) => InternalContent.DispatchEvent(e.Derive());
        renderer.MouseDragged += (s, e) => InternalContent.DispatchEvent(e.Derive());
        renderer.MouseWheel += (s, e) => InternalContent.DispatchEvent(e.Derive());

        renderer.SelectionChanged += (s, a) =>
          {
            SelectedIndex = closureIndex;
            if (a.Adjusting == false)
            {
              // the adjusting event is fired naturally already.
              RaiseSelectionChanged(a);
            }
          };

        InternalContent.Content.Add(renderer);
      }

      // todo: This is crude ..
      if (SelectedItem != null)
      {
        if (!DataItems.Contains(SelectedItem))
        {
          SelectedItem = default(T);
        }
      }
    }
  }

  public static class ListView
  {
    public static IListDataItemRenderer DefaultCreateRenderer<TX>(IUIStyle style, TX item)
    {
      var text = item?.ToString() ?? "<NULL>";
      return new ListDataItemRenderer(style) { Content = new Label(style, text) { Enabled = false, Padding = new Insets() }, Tag = item };
    }
  }
}