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

using Microsoft.Xna.Framework;

using Steropes.UI.Components;
using Steropes.UI.Components.Helper;
using Steropes.UI.Platform;
using Steropes.UI.Styles;

namespace Steropes.UI.Widgets
{
  public class IconLabelStyleDefinition : IStyleDefinition
  {
    public IStyleKey<int> IconTextGap { get; private set; }

    public void Init(IStyleSystem s)
    {
      IconTextGap = s.CreateKey<int>("icon-text-gap", false);
    }
  }

  public class IconLabel : Widget
  {
    readonly IconLabelStyleDefinition iconLabelStyle;

    public IconLabel(IUIStyle style) : base(style)
    {
      iconLabelStyle = StyleSystem.StylesFor<IconLabelStyleDefinition>();

      Image = new Image(style) { Padding = new Insets(), Enabled = false };
      Image.AddNotify(this);
      RaiseChildAdded(0, Image);

      Label = new Label(style) { Padding = new Insets(), Enabled = false };
      Label.AddNotify(this);
      RaiseChildAdded(1, Label);
    }

    public Alignment Alignment
    {
      get
      {
        return Label.Alignment;
      }
      set
      {
        Label.Alignment = value;
      }
    }

    public override int Count => 2;

    public IUIFont Font
    {
      get
      {
        return Label.Font;
      }
      set
      {
        Label.Font = value;
      }
    }

    public bool HasTexture => Image.HasTexture;

    public Color IconColor
    {
      get
      {
        return Image.Color;
      }
      set
      {
        Image.Color = value;
      }
    }

    public int IconTextGap
    {
      get
      {
        return Style.GetValue(iconLabelStyle.IconTextGap);
      }
      set
      {
        Style.SetValue(iconLabelStyle.IconTextGap, value);
        InvalidateLayout();
      }
    }

    public Image Image { get; }

    public Label Label { get; }

    public Color OutlineColor
    {
      get
      {
        return Label.OutlineColor;
      }
      set
      {
        Label.OutlineColor = value;
      }
    }

    public float OutlineRadius
    {
      get
      {
        return Label.OutlineRadius;
      }
      set
      {
        Label.OutlineRadius = value;
      }
    }

    public string Text
    {
      get
      {
        return Label.Text;
      }
      set
      {
        Label.Text = value;
      }
    }

    public Color TextColor
    {
      get
      {
        return Label.TextColor;
      }
      set
      {
        Label.TextColor = value;
      }
    }

    public IUITexture Texture
    {
      get
      {
        return Image.Texture;
      }
      set
      {
        Image.Texture = value;
      }
    }

    public bool Underline
    {
      get
      {
        return Label.Underline;
      }
      set
      {
        Label.Underline = value;
      }
    }

    public override IWidget this[int index]
    {
      get
      {
        switch (index)
        {
          case 0:
            return Image;
          case 1:
            return Label;
          default:
            throw new IndexOutOfRangeException();
        }
      }
    }

    protected override Rectangle ArrangeOverride(Rectangle layoutSize)
    {
      if (string.IsNullOrEmpty(Label.Text))
      {
        Image.Arrange(layoutSize);
        return Image.LayoutRect;
      }

      if (!Image.HasTexture)
      {
        Label.Arrange(layoutSize);
        return Label.LayoutRect;
      }

      var actualWidth = DesiredSize.WidthInt;

      var center = layoutSize.Center;
      int left;
      int spaceToRight;
      switch (Label.Alignment)
      {
        case Alignment.Start:
        case Alignment.Fill:
          {
            left = layoutSize.X;
            spaceToRight = Math.Max(0, layoutSize.Width - actualWidth);
            break;
          }
        case Alignment.Center:
          {
            left = center.X - actualWidth / 2;
            spaceToRight = Math.Max(0, layoutSize.Width - (center.X + actualWidth / 2));
          }
          break;
        case Alignment.End:
          {
            left = layoutSize.Right - actualWidth;
            spaceToRight = 0;
            break;
          }
        default:
          {
            throw new NotSupportedException();
          }
      }

      var arr = new ArrangerHorizontal(layoutSize);
      arr.Advance(left - layoutSize.X);
      arr.Reserve(spaceToRight);
      arr.Arrange(Image, Image.DesiredSize.WidthInt).Advance(Image.DesiredSize.WidthInt);
      arr.Advance(IconTextGap);
      arr.Arrange(Label, arr.AvailableWidth);

      var x = Image.LayoutRect.X;
      var y = Math.Min(Image.LayoutRect.Y, Label.LayoutRect.Y);
      var width = Math.Max(0, Label.LayoutRect.Right - Image.LayoutRect.X);
      var height = Math.Max(0, Math.Max(Image.LayoutRect.Bottom, Label.LayoutRect.Bottom) - y);
      return new Rectangle(x, y, width, height);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
      if (string.IsNullOrEmpty(Label.Text))
      {
        Image.Measure(availableSize);
        return Image.DesiredSize;
      }

      if (!Image.HasTexture)
      {
        Label.Measure(availableSize);
        return Label.DesiredSize;
      }

      if (float.IsPositiveInfinity(availableSize.Width))
      {
        Label.Measure(availableSize);
        Image.Measure(availableSize);
      }
      else
      {
        Image.Measure(new Size(float.PositiveInfinity, availableSize.Height));
        var availableWidth = Math.Max(0, availableSize.Width - Image.DesiredSize.Width - IconTextGap);
        Label.Measure(new Size(availableWidth, availableSize.Height));
      }
      return new Size(Image.DesiredSize.Width + IconTextGap + Label.DesiredSize.Width, Math.Max(Image.DesiredSize.Height, Label.DesiredSize.Height));
    }
  }
}