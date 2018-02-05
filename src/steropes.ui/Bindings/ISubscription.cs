using System.ComponentModel;

namespace Steropes.UI.Bindings
{
  public interface IReadOnlyObservableValue : INotifyPropertyChanged
  {
    object Value { get; }
  }

  public interface IReadOnlyObservableValue<out T> : IReadOnlyObservableValue
  {
    new T Value { get; }
  }

  public interface IObservableValue : IReadOnlyObservableValue
  {
    new object Value { get; set; }
  }
}