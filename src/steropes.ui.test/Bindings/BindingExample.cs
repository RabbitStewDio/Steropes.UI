using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Steropes.UI.Annotations;
using Steropes.UI.Bindings;
using Steropes.UI.Components;
using Steropes.UI.Platform;
using Steropes.UI.Widgets;
using Steropes.UI.Widgets.Container;

namespace Steropes.UI.Test.Bindings
{
  /// <summary>
  ///  Sample code from the documentation.
  /// </summary>
  public class BindingExample
  {
    class PowerUp : IPowerUp
    {
      public string Name { get; set; }
    }

    interface IPowerUp
    {
      string Name { get; set; }
    }

    class Player : INotifyPropertyChanged
    {
      int hitPoints;
      string name;

      public Player()
      {
        Inventory = new ObservableCollection<IPowerUp>();
      }

      public int HitPoints
      {
        get { return hitPoints; }
        set
        {
          if (value == hitPoints)
            return;

          hitPoints = value;
          OnPropertyChanged();
        }
      }

      public string Name
      {
        get { return name; }
        set
        {
          if (value == name)
            return;

          name = value;
          OnPropertyChanged();
        }
      }

      public ObservableCollection<IPowerUp> Inventory { get; }

      public event PropertyChangedEventHandler PropertyChanged;

      [NotifyPropertyChangedInvocator]
      protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
      {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
      }
    }

    void CreateUI(IUIStyle style, Player player)
    {
      var powerUpIcons = new Dictionary<IPowerUp, IUITexture>();
      var notFoundIcon = new UITexture(null);

      var g = new Group(style)
      {
        // Name in the top-left corner of the screen
        new Label(style)
        {
          Anchor = AnchoredRect.CreateTopLeftAnchored(10, 10)
        }.DoWith(l => player.BindingFor(p => p.Name).BindTo(text => l.Text = text)),
        
        // Hitpoints in the top-right corner of the screen
        new Label(style)
        {
          Anchor = AnchoredRect.CreateTopRightAnchored(10, 10)
        }.DoWith(l => player
                   .BindingFor(p => p.HitPoints)
                   .Map(value => $"{value:D}")
                   .BindTo(text => l.Text = text)),

        // Powerups in the top center of the screen
        new BoxGroup(style)
          {
            Anchor = new AnchoredRect(null, 10, null, null, null, null)
          }
          // take the power-ups, map them to textures and build image-widgets.
          // then we'll add those to a list.
          .DoWith(bg => player.Inventory.ToBinding()
                    .Map(pu => powerUpIcons[pu] ?? notFoundIcon)
                    .Map(img => new Image(style) { Texture = img })
                    .Map(w => new WidgetAndConstraint<bool>(w))
                    .BindTo(bg))
      };
    }
  }
}