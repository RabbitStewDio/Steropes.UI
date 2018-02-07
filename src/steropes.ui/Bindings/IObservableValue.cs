namespace Steropes.UI.Bindings
{
  public interface IObservableValue<T> : IReadOnlyObservableValue<T>
  {
    new T Value { get; set; }
  }
}