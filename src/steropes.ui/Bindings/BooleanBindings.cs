namespace Steropes.UI.Bindings
{
  public static class BooleanBindings
  {
    public static IReadOnlyObservableValue<bool> Not(this IReadOnlyObservableValue<bool> that)
    {
      return that.Map(b => !b);
    }

    public static IReadOnlyObservableValue<bool> And(this IReadOnlyObservableValue<bool> that,
                                                     IReadOnlyObservableValue<bool> other)
    {
      return Binding.Combine(that, other, (a, b) => a && b);
    }

    public static IReadOnlyObservableValue<bool> Or(this IReadOnlyObservableValue<bool> that,
                                                    IReadOnlyObservableValue<bool> other)
    {
      return Binding.Combine(that, other, (a, b) => a || b);
    }

    public static IReadOnlyObservableValue<T> Map<T>(this IReadOnlyObservableValue<bool> that,
                                                    T onTrue, T onFalse)
    {
      return that.Map((b) => b ? onTrue: onFalse);
    }

    public static IReadOnlyObservableValue<T> Map<T>(this IReadOnlyObservableValue<bool> that,
                                                    IReadOnlyObservableValue<T> onTrueVal, IReadOnlyObservableValue<T> onFalseVal)
    {
      return Binding.Combine(that, onTrueVal, onFalseVal, (b, onTrue, onFalse) => b ? onTrue: onFalse);
    }

    public static IReadOnlyObservableValue<bool> Null<T>(this IReadOnlyObservableValue<T> that) where T : class
    {
      return that.Map(b => (b == null));
    }

    public static IReadOnlyObservableValue<bool> NotNull<T>(this IReadOnlyObservableValue<T> that) where T : class
    {
      return that.Map(b => (b != null));
    }

    public static IReadOnlyObservableValue<bool> EqualTo<T>(this IReadOnlyObservableValue<T> that, T other)
    {
      return that.Map(b => Equals(b, other));
    }

    public static IReadOnlyObservableValue<bool> EqualTo<T>(this IReadOnlyObservableValue<T> that,
                                                            IReadOnlyObservableValue<T> other) where T : class
    {
      return Binding.Combine(that, other, Equals);
    }
  }
}