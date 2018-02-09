using System.ComponentModel;
using System.Runtime.CompilerServices;
using Steropes.UI.Annotations;

namespace Steropes.UI.Test.Bindings
{
  /// <summary>
  ///  A sample class that provides a fertile environment for testing.
  /// </summary>
  /// <typeparam name="TA"></typeparam>
  /// <typeparam name="TB"></typeparam>
  class BindingFixtureElement<TA, TB> : INotifyPropertyChanged
  {
    public TA Field;
    TB property;

    public TB Property
    {
      get { return property; }
      set
      {
        if (Equals(value, property))
          return;

        property = value;
        OnPropertyChanged();
      }
    }

    public BindingFixtureElement()
    {
    }

    public BindingFixtureElement(TA field, TB property)
    {
      Field = field;
      Property = property;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}